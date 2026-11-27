# PSHome-MultiServer integration tests

Scripted clients and live tests for the server, in standard-library Python (3.8+).

## Layout

- `shared/pshclient/`: the protocol client (RT frames, Medius messages, MUIS, account and lobby login, NP tickets).
  Title independent: every title value is a function or class parameter. Covers MUIS, login, the lobby (channels, game list,
  create and join), the DME connection (TCP and auxiliary UDP) and `room.Player`, one client that does all of it.
- `Vita/<Title>/`: per title, one `<title>_constants.py` (the values passed to `pshclient`, plus the title's account
  rule), a `conftest.py` with the title's fixtures, and the tests (`test_*.py`). Today: `Vita/WipEout2048`.
- `conftest.py`: the `horizon` fixture, shared by every title. It starts a server you built; it never builds one.
- `_legacy/`: the earlier stage clients, kept as a reference while the tests are rewritten; not collected, not linted.

How to write a test: [CONTRIBUTING.md](CONTRIBUTING.md).

## Requirements

| | Linux | Windows |
|---|---|---|
| Python 3.8+ with venv | distribution package (Debian/Ubuntu: `sudo apt install python3 python3-venv`) | python.org installer or `winget install Python.Python.3.12` (adds the `py` launcher) |
| .NET 8 or newer (`dotnet`), to build and start the servers | [dotnet.microsoft.com](https://dotnet.microsoft.com/download) or the distribution package | same, or `winget search Microsoft.DotNet` and install the .NET 8 package |
| pytest | installed into the venv by the commands below | same |

Horizon targets `net6.0`. The tests do not build it: build once (and again after server changes), then the `horizon`
fixture starts the newest `Servers/Horizon/bin/Release/*/Horizon.dll` with `DOTNET_ROLL_FORWARD=Major`, so any newer .NET
works without a .NET 6 runtime. Pointing the tests at a server that is already running needs no .NET at all.

    cd Servers/Horizon
    DOTNET_ROLL_FORWARD=Major dotnet build -c Release --no-incremental -nodeReuse:false -p:UseSharedCompilation=false

On Linux the build ends with one error from the project's PowerShell post-build step (MSB3073, DepsFixer); the compile
succeeded and `Horizon.dll` is produced (check its modification time).

## Server for the tests

| | |
|---|---|
| Default | the fixture checks that ports 10071, 10075, 10077 and 10078 are free (a game's DME listens on 10073, with UDP from 50000 up), starts the built Horizon, waits until it is ready (about 10 s) and stops it at the end of the session. Without a `Horizon.dll` the run stops and prints the build command above. The server log is in pytest's temporary directory; its path is printed at the end of the run. |
| `PSH_TEST_HORIZON_DLL=/path/Horizon.dll` | start that build instead of the newest one under `Servers/Horizon/bin/Release`. |
| `PSH_TEST_HORIZON_HOST=127.0.0.1` | use the Horizon already running on that host (default ports); nothing is built or started. |

If the ports are in use the run stops with a message saying so: set the variable to use that server, or stop it.

## Run

Linux (bash, zsh):

    cd Tests
    python3 -m venv .venv
    . .venv/bin/activate
    pip install -e ".[test]"
    pytest

Windows (PowerShell):

    cd Tests
    py -m venv .venv
    .venv\Scripts\Activate.ps1
    pip install -e ".[test]"
    pytest

Keep the quotes around `".[test]"`: zsh treats an unquoted `[test]` as a glob and stops with "no matches found". A venv is
needed on current Debian/Ubuntu, where pip refuses to install into the system Python. Run `pytest Vita/WipEout2048` for one
title. Live tests need a server; how one is used or started is in the title README.
Without installing, `PYTHONPATH=shared:Vita/WipEout2048` (Windows: `$env:PYTHONPATH="shared;Vita/WipEout2048"`) makes the
shared package and the title module importable. Deselect the tests of a message the shipped game does not use with
`-m "not legacy_path"`.

## Add a title

Create `Vita/<Title>/` with one `<title>_constants.py` (the title's values and account rule; the title stays in the file
name because `Vita/<Title>` is added to `pythonpath` in `pyproject.toml`, which makes its modules top-level names, and a
generic name would collide with another title's), a `conftest.py`, and the tests beside them. For `pip install -e`, add the
module to `[tool.setuptools]` `py-modules`; since all titles share one root mapping there, a second title needs its own
`package-dir` handling (or drop the installable form). Test basenames may repeat across titles (`test_login.py` in two
titles): pytest runs with `--import-mode=importlib`, and without it the second one fails to collect with "import file
mismatch". Fixture names a title defines (`title`, `online_id`, ...) only live in its own directory.
