"""One RT connection to a Medius server: framing, the unencrypted session handshake and Medius request/reply."""

import logging
import select
import socket
import struct
from collections import deque
from dataclasses import dataclass
from typing import Deque, Dict, List, Optional, Tuple

from . import rt
from .errors import ConnectionClosedError, DecodeError, ReplyTimeoutError
from .medius import FRAGMENT, MessageType, message_name, text_of

log = logging.getLogger(__name__)

CERTIFICATE_SUITE_FLAG = 0x200


@dataclass
class ServerHello:
    """SERVER_HELLO: u16 version [0], u16 cipher suite [2], and when suite & 0x200: u16 certificate length [4]."""

    version: int
    suite: int
    certificate_length: Optional[int]
    payload_length: int


@dataclass
class ConnectRequire:
    """SERVER_CONNECT_REQUIRE: u8 flags [0], then u16 values."""

    flags: Optional[int]
    values: Tuple[int, ...]


@dataclass
class ConnectAccept:
    """SERVER_CONNECT_ACCEPT_TCP: u16 player id [0], u32 server certificate [2], u16 address count [6], char ip[16] [8].
    The count and ip are None / '' when the payload is shorter than 0x18 bytes."""

    player_id: int
    address_count: Optional[int]
    ip: str
    payload_length: int


@dataclass
class ConnectComplete:
    """SERVER_CONNECT_COMPLETE: u16 value [0]; None when the payload is shorter than 2 bytes."""

    value: Optional[int]
    payload_length: int


@dataclass
class Handshake:
    """The four server messages of the connection handshake, in the order they arrive."""

    hello: ServerHello
    require: ConnectRequire
    accept: ConnectAccept
    complete: ConnectComplete


class Connection:
    """An RT connection. `name` labels the log lines; `timeout` is the wait for each awaited message, in seconds."""

    def __init__(self, host: str, port: int, name: str, timeout: float = 5.0):
        self.host, self.port, self.name, self.timeout = host, port, name, timeout
        self._socket: Optional[socket.socket] = None
        self._buffer = b""
        self._pending: Deque[rt.Frame] = deque()
        self._fragments: Dict[Tuple[int, int, int], Dict[int, bytes]] = {}
        self.peer_closed = False  # set by read_frames when the server closed or reset the connection

    def __enter__(self) -> "Connection":
        self.open()
        return self

    def __exit__(self, *exc_info) -> None:
        self.close()

    def open(self) -> None:
        self._socket = socket.create_connection((self.host, self.port), timeout=self.timeout)
        log.debug("%s: connected to %s:%d", self.name, self.host, self.port)

    @property
    def is_open(self) -> bool:
        return self._socket is not None

    def close(self) -> None:
        if self._socket is not None:
            self._socket.close()
            self._socket = None

    def fileno(self) -> int:
        """The socket's descriptor, for `select` over several connections."""
        return self._socket.fileno()

    # ----- frames

    def send(self, frame_id: int, payload: bytes = b"") -> None:
        log.debug("%s -> %s len=%d", self.name, rt.frame_name(frame_id), len(payload))
        self._socket.sendall(rt.encode_frame(frame_id, payload))

    def disconnect(self) -> None:
        """Tell the server this client is leaving (CLIENT_DISCONNECT_WITH_REASON, reason 0)."""
        self.send(rt.CLIENT_DISCONNECT_WITH_REASON, b"\x00")

    def receive(self, *wanted: int) -> rt.Frame:
        """The next frame whose id is in `wanted`; other frames are skipped with a DEBUG line."""
        while True:
            frame = self._next_frame(", ".join(rt.frame_name(i) for i in wanted))
            if frame.id in wanted:
                return frame
            log.debug("%s <- %s len=%d (skipped)", self.name, rt.frame_name(frame.id), len(frame.payload))

    def read_frames(self, timeout: float) -> List[rt.Frame]:
        """Every frame that arrives within `timeout` seconds of the first wait (0 = only what is readable now); [] when
        nothing came. Unlike `receive` it keeps all frames, so a caller that watches for unsolicited messages loses
        none. Sets `peer_closed` (and returns what was decoded) when the server closed or reset the connection."""
        if not self._pending and not self.peer_closed and self._socket is not None:
            readable, _, _ = select.select([self._socket], [], [], timeout)
            if readable:
                try:
                    chunk = self._socket.recv(0x1000)
                except OSError:
                    chunk = b""
                if chunk:
                    self._buffer += chunk
                    frames, self._buffer = rt.decode_frames(self._buffer)
                    for frame in frames:
                        self._queue(frame)
                else:
                    self.peer_closed = True
        frames = list(self._pending)
        self._pending.clear()
        return frames

    def _next_frame(self, awaiting: str) -> rt.Frame:
        while not self._pending:
            frames, self._buffer = rt.decode_frames(self._buffer)
            if frames:
                for frame in frames:
                    self._queue(frame)
                continue
            self._buffer += self._read_chunk(awaiting)
        frame = self._pending.popleft()
        log.debug("%s <- %s len=%d", self.name, rt.frame_name(frame.id), len(frame.payload))
        return frame

    def _queue(self, frame: rt.Frame) -> None:
        if frame.id in (rt.AGGREGATE_RECV, rt.AGGREGATE_SEND):
            self._pending.extend(rt.decode_aggregate(frame.payload))
        else:
            self._pending.append(frame)

    def _read_chunk(self, awaiting: str) -> bytes:
        self._socket.settimeout(self.timeout)
        try:
            chunk = self._socket.recv(0x1000)
        except socket.timeout:
            raise ReplyTimeoutError(f"{self.name}: no {awaiting} within {self.timeout:.1f} s") from None
        except OSError as error:
            raise ConnectionClosedError(
                f"{self.name}: connection failed while waiting for {awaiting}: {error}"
            ) from None
        if not chunk:
            raise ConnectionClosedError(f"{self.name}: server closed the connection while waiting for {awaiting}")
        return chunk

    # ----- handshake

    def handshake(self, app_id: int, version: int, world_id: int, trailer: bytes = b"") -> Handshake:
        """Run the unencrypted connection handshake: hello, connect, ready. `trailer` is appended to the connect message
        (the Medius lobby server expects the session and access keys there)."""
        self.send(rt.CLIENT_HELLO, struct.pack("<HHHH", 1, version, 1, 0))
        hello = _decode_hello(self.receive(rt.SERVER_HELLO).payload)
        self.send(rt.CLIENT_CONNECT_TCP, struct.pack("<ii", world_id, app_id) + bytes(64) + trailer)
        require = _decode_require(self.receive(rt.SERVER_CONNECT_REQUIRE).payload)
        self.send(rt.CLIENT_CONNECT_READY_REQUIRE, b"\x00")
        accept = _decode_accept(self.receive(rt.SERVER_CONNECT_ACCEPT_TCP).payload)
        self.send(rt.CLIENT_CONNECT_READY_TCP, b"\x00\x00")
        complete = _decode_complete(self.receive(rt.SERVER_CONNECT_COMPLETE).payload)
        return Handshake(hello, require, accept, complete)

    # ----- Medius messages

    def send_medius(self, message: MessageType, body: bytes) -> None:
        """Send a Medius request: RT frame CLIENT_APP_TOSERVER carrying `u8 class, u8 id, body`."""
        log.debug("%s -> %s len=%d", self.name, message.name, len(body))
        self._socket.sendall(
            rt.encode_frame(rt.CLIENT_APP_TOSERVER, bytes([message.class_id, message.message_id]) + body)
        )

    def receive_medius(self, *wanted: MessageType) -> Tuple[MessageType, bytes]:
        """The next Medius message of one of the `wanted` types as (type, body); others are skipped with a DEBUG line.
        Messages the server split into fragments (class 0, id 2) are reassembled first."""
        awaiting = " or ".join(message.name for message in wanted)
        while True:
            payload = self.receive(rt.SERVER_APP).payload
            if len(payload) < 2:
                raise DecodeError(f"{self.name}: server message of {len(payload)} bytes has no class and id")
            class_id, message_id, body = payload[0], payload[1], payload[2:]
            if (class_id, message_id) == (FRAGMENT.class_id, FRAGMENT.message_id):
                assembled = self._add_fragment(body)
                if assembled is None:
                    continue
                class_id, message_id, body = assembled
            for message in wanted:
                if (message.class_id, message.message_id) == (class_id, message_id):
                    log.debug("%s <- %s len=%d (awaiting %s)", self.name, message.name, len(body), awaiting)
                    return message, body
            log.debug("%s <- %s len=%d (skipped)", self.name, message_name(class_id, message_id), len(body))

    def request(self, message: MessageType, body: bytes, *replies: MessageType) -> bytes:
        """Send a request and return the body of the first reply of one of the given types."""
        self.send_medius(message, body)
        return self.receive_medius(*replies)[1]

    def _add_fragment(self, body: bytes) -> Optional[Tuple[int, int, bytes]]:
        """Fragment body: u8 class, u8 id, u16 data size, u16 count, u16 index, u8 multi-message, 3 pad, i32 total
        length [12], i32 offset [16], data [20]. Returns the whole message once all of it arrived, else None."""
        if len(body) < 20:
            raise DecodeError(f"{self.name}: fragment of {len(body)} bytes, the header needs 20")
        class_id, message_id, size, count, index, multi = struct.unpack_from("<BBHHHB", body, 0)
        total, offset = struct.unpack_from("<ii", body, 12)
        log.debug("%s <- fragment %d/%d of class %d id 0x%02X", self.name, index + 1, count, class_id, message_id)
        parts = self._fragments.setdefault((class_id, message_id, multi), {})
        parts[offset] = body[20 : 20 + size]
        if sum(len(part) for part in parts.values()) < total:
            return None
        del self._fragments[(class_id, message_id, multi)]
        return class_id, message_id, b"".join(parts[key] for key in sorted(parts))


def _decode_hello(payload: bytes) -> ServerHello:
    if len(payload) < 4:
        raise DecodeError(f"SERVER_HELLO: {len(payload)} bytes, need at least 4")
    version, suite = struct.unpack_from("<HH", payload, 0)
    certificate_length = None
    if len(payload) >= 6 and suite & CERTIFICATE_SUITE_FLAG:
        certificate_length = struct.unpack_from("<H", payload, 4)[0]
    return ServerHello(version, suite, certificate_length, len(payload))


def _decode_require(payload: bytes) -> ConnectRequire:
    flags = payload[0] if payload else None
    count = (len(payload) - 1) // 2
    values = struct.unpack_from("<" + "H" * count, payload, 1) if count > 0 else ()
    return ConnectRequire(flags, tuple(values))


def _decode_accept(payload: bytes) -> ConnectAccept:
    if len(payload) < 2:
        raise DecodeError(f"SERVER_CONNECT_ACCEPT_TCP: {len(payload)} bytes, the player id needs 2")
    player_id = struct.unpack_from("<H", payload, 0)[0]
    count = struct.unpack_from("<H", payload, 6)[0] if len(payload) >= 8 else None
    ip = text_of(payload[8:24]) if len(payload) >= 24 else ""
    return ConnectAccept(player_id, count, ip, len(payload))


def _decode_complete(payload: bytes) -> ConnectComplete:
    value = struct.unpack_from("<H", payload, 0)[0] if len(payload) >= 2 else None
    return ConnectComplete(value, len(payload))
