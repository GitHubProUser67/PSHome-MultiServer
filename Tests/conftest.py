"""Fixtures shared by every title: the Horizon server, either external or started here from a build you made."""

import logging
import os
import socket
import subprocess
import sys
import time
from dataclasses import dataclass
from pathlib import Path
from shutil import which
from typing import Iterator, List, Optional

import pytest

log = logging.getLogger(__name__)

HOST = "127.0.0.1"
HOST_ENV = "PSH_TEST_HORIZON_HOST"
DLL_ENV = "PSH_TEST_HORIZON_DLL"
MUIS_PORT = 10071  # universe information server
MAS_PORT = 10075  # account server
DME_PORT = 10077  # DME server (opened by Horizon; no test talks to it yet)
MLS_PORT = 10078  # lobby server
HORIZON_PORTS = (MUIS_PORT, MAS_PORT, DME_PORT, MLS_PORT)
START_TIMEOUT = 60.0
READY_MARKER = "[DmeClass] - Initalized all servers."  # the last line of Horizon's start-up log

REPOSITORY = Path(__file__).resolve().parent.parent
HORIZON_PROJECT = REPOSITORY / "Servers" / "Horizon"
HORIZON_BUILD_DIR = HORIZON_PROJECT / "bin" / "Release"
BUILD_COMMAND = (
    f"cd {HORIZON_PROJECT} && DOTNET_ROLL_FORWARD=Major dotnet build -c Release --no-incremental "
    "-nodeReuse:false -p:UseSharedCompilation=false"
)

_started_log: Optional[Path] = None


@dataclass(frozen=True)
class ServerAddress:
    """Where the tests find Horizon. The ports are fixed by the server's default configuration."""

    host: str
    muis_port: int = MUIS_PORT
    mas_port: int = MAS_PORT
    mls_port: int = MLS_PORT


def _port_accepts(host: str, port: int) -> bool:
    try:
        with socket.create_connection((host, port), timeout=1.0):
            return True
    except OSError:
        return False


def _ports_where(accepting: bool) -> List[int]:
    """The Horizon ports that do (accepting=True) or do not (False) accept a connection on HOST."""
    return [port for port in HORIZON_PORTS if _port_accepts(HOST, port) == accepting]


def _find_horizon_dll() -> Path:
    """The Horizon.dll to run: $PSH_TEST_HORIZON_DLL, else the newest build under Servers/Horizon/bin/Release.
    The tests never build the server; without a library they stop and print the build command."""
    override = os.environ.get(DLL_ENV)
    candidates = (
        [Path(override)]
        if override
        else sorted(HORIZON_BUILD_DIR.glob("*/Horizon.dll"), key=lambda path: path.stat().st_mtime)
    )
    if not candidates or not candidates[-1].is_file():
        where = override or f"{HORIZON_BUILD_DIR}/*/Horizon.dll"
        pytest.exit(
            f"No Horizon.dll found ({where}). Build the server first (the tests never build it):\n  {BUILD_COMMAND}\n"
            "On Linux the build ends with one PowerShell post-build error; the library is still produced. "
            f"Or point {DLL_ENV} at a Horizon.dll, or {HOST_ENV} at a running server.",
            returncode=3,
        )
    return candidates[-1]


def _dotnet() -> str:
    dotnet = which("dotnet")
    if dotnet is None:
        pytest.exit(
            f"dotnet is not on PATH; install .NET (the `dotnet` command) or set {HOST_ENV} to a running Horizon",
            returncode=3,
        )
    return dotnet


def _start_horizon(folder: Path, server_log: Path) -> subprocess.Popen:
    """Start Horizon with stdin held open (it exits when stdin closes) and its output in `server_log`."""
    env = dict(os.environ, DOTNET_ROLL_FORWARD="Major")
    options = {}
    if sys.platform == "win32":
        options["creationflags"] = subprocess.CREATE_NEW_PROCESS_GROUP
    output = server_log.open("wb")
    try:
        return subprocess.Popen(
            [_dotnet(), "Horizon.dll"],
            cwd=str(folder),
            env=env,
            stdin=subprocess.PIPE,
            stdout=output,
            stderr=subprocess.STDOUT,
            **options,
        )
    finally:
        output.close()


def _wait_until_ready(process: subprocess.Popen, server_log: Path) -> None:
    """Wait for Horizon's last start-up step, then check that every port accepts a connection. The ports open several
    seconds before the server answers anything (it initialises its DME components last), so connecting to them earlier
    would only queue connections behind that step."""
    deadline = time.monotonic() + START_TIMEOUT
    while time.monotonic() < deadline:
        if process.poll() is not None:
            pytest.exit(f"Horizon exited with code {process.returncode} during start; log: {server_log}", returncode=3)
        if READY_MARKER in server_log.read_text(errors="replace") and not _ports_where(False):
            return
        time.sleep(0.25)
    pytest.exit(f"Horizon was not ready within {START_TIMEOUT:.0f} s; log: {server_log}", returncode=3)


def _stop(process: subprocess.Popen) -> None:
    if process.poll() is None:
        process.terminate()
        try:
            process.wait(timeout=15)
        except subprocess.TimeoutExpired:
            process.kill()
            process.wait()
    if process.stdin is not None:
        process.stdin.close()


@pytest.fixture(scope="session")
def horizon(tmp_path_factory: pytest.TempPathFactory) -> Iterator[ServerAddress]:
    """The Horizon server (MUIS, MAS and MLS). With PSH_TEST_HORIZON_HOST set, that running server is used and
    nothing is started; otherwise the Horizon build found by `_find_horizon_dll` is started and stopped when the
    session ends."""
    global _started_log
    external = os.environ.get(HOST_ENV)
    if external:
        yield ServerAddress(external)
        return
    busy = _ports_where(True)
    if busy:
        pytest.exit(
            f"Ports {busy} are already in use, so a server seems to be running. "
            f"Use it with {HOST_ENV}=127.0.0.1 or stop it first.",
            returncode=3,
        )
    folder = _find_horizon_dll().parent
    _started_log = tmp_path_factory.mktemp("horizon") / "horizon.log"
    process = _start_horizon(folder, _started_log)
    try:
        _wait_until_ready(process, _started_log)
        log.debug("Horizon started from %s, log %s", folder, _started_log)
        yield ServerAddress(HOST)
    finally:
        _stop(process)


def pytest_terminal_summary(terminalreporter: pytest.TerminalReporter) -> None:
    if _started_log is not None:
        terminalreporter.write_line(f"Horizon server log: {_started_log}")
