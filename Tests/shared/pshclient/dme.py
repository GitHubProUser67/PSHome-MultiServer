"""A DME (game relay) client: the TCP connection plus the auxiliary UDP leg a client-server game with transport type 3
uses.

A player counts as joined only once both legs are up: the server ignores a player that has no UDP endpoint, so it sends
neither the other players' notifications nor its own CONNECT_COMPLETE before the UDP exchange has finished.
"""

import logging
import select
import socket
import struct
import time
from dataclasses import dataclass, field
from typing import Dict, List, Optional

from . import rt
from .connection import Connection
from .errors import ConnectionClosedError, DecodeError, ReplyTimeoutError
from .medius import fixed, text_of

log = logging.getLogger(__name__)

CLIENT_INFO_LENGTH = 0x4C  # size of the game's ClientInfo message, which every player sends to the players it knows
CLIENT_INFO_NAME_LENGTH = 16  # ASSUMED: the player's name as NUL-padded ASCII at the start of the body

PEER_NOTIFY_LENGTH = 22  # i16 index, i16 scert id, i16 unknown, char ip[16]
CONNECT_NOTIFY_LENGTH = PEER_NOTIFY_LENGTH + 64  # a connect notice also carries a 64-byte key


@dataclass
class PeerNotice:
    """SERVER_CONNECT_NOTIFY / SERVER_DISCONNECT_NOTIFY (Medius version above 108): i16 DME index [0], i16 scert id [2],
    i16 unknown [4], char ip[16] [6]; a connect notice has a 64-byte key after that."""

    index: int
    scert_id: int
    ip: str


def decode_peer_notice(payload: bytes, minimum: int = PEER_NOTIFY_LENGTH) -> PeerNotice:
    if len(payload) < minimum:
        raise DecodeError(f"peer notice: {len(payload)} bytes, need {minimum}")
    index, scert_id, _unknown = struct.unpack_from("<hhh", payload, 0)
    return PeerNotice(index, scert_id, text_of(payload[6:22]))


@dataclass
class Accepted:
    """SERVER_CONNECT_ACCEPT_TCP / SERVER_CONNECT_ACCEPT_AUX_UDP: u16 player (DME index) [0], u32 scert id [2], u16
    address count [6], char ip[16] [8]; the UDP form adds u16 port [0x18]."""

    index: int
    scert_id: int
    ip: str


def _decode_accept(payload: bytes, minimum: int, name: str) -> Accepted:
    if len(payload) < minimum:
        raise DecodeError(f"{name}: {len(payload)} bytes, need {minimum}")
    return Accepted(
        struct.unpack_from("<H", payload, 0)[0], struct.unpack_from("<I", payload, 2)[0], text_of(payload[8:24])
    )


def encode_connect_tcp_aux_udp(world_id: int, app_id: int, session_key: str, access_key: str) -> bytes:
    """CLIENT_CONNECT_TCP_AUX_UDP: i32 world id, i32 application id, 64 bytes of server key (zero), session key[17],
    access key[17]."""
    return struct.pack("<ii", world_id, app_id) + bytes(64) + fixed(session_key, 17) + fixed(access_key, 17)


def encode_connect_aux_udp(world_id: int, app_id: int, address: str, port: int, index: int, scert_id: int) -> bytes:
    """CLIENT_CONNECT_AUX_UDP (UDP, 32 bytes): i32 world id, i32 application id, char address[16], u16 local port, u16
    DME index, u32 scert id (the last two echo SERVER_CONNECT_ACCEPT_TCP)."""
    return struct.pack("<ii", world_id, app_id) + fixed(address, 16) + struct.pack("<HHI", port, index, scert_id)


@dataclass
class Received:
    """A frame the client received, with the time it was read."""

    time: float
    frame_id: int
    payload: bytes


@dataclass
class AppMessage:
    """An application message another player sent, as the server relays it."""

    time: float
    sender: int
    payload: bytes


@dataclass
class DmeClient:
    """One player's DME connection. After `connect` call `service` regularly (it answers the server's echo and ping like
    the game does, and records every other frame in `received`).

    It also keeps the player's peer set like the game does. A peer is added when the server announces it
    (SERVER_CONNECT_NOTIFY) and when an application message arrives from it (the sender index), and removed on
    SERVER_DISCONNECT_NOTIFY; `peers` maps each DME index to what first told this player about it ("notice" or
    "message"). When a peer is new, the player sends its ClientInfo as APP_LIST to its whole peer set, which is how the
    players already in a game make themselves known to a joiner the server never announced them to. A peer learned
    from a received ClientInfo is answered with this player's own ClientInfo straight away (ASSUMED: it is not known
    whether the game answers at once or only on its 10 s timer; the timer is not simulated). With
    `peer_exchange=False` the client sends nothing of its own."""

    host: str
    port: int
    name: str
    timeout: float = 5.0
    index: Optional[int] = None
    scert_id: Optional[int] = None
    received: List[Received] = field(default_factory=list)
    connection: Optional[Connection] = None
    udp: Optional[socket.socket] = None
    udp_server: Optional[tuple] = None
    peer_exchange: bool = True
    peers: Dict[int, str] = field(default_factory=dict)
    completed: bool = False
    _announce: bool = False  # the peer set grew and the ClientInfo has not been sent to it yet

    # ----- connect

    def connect(
        self,
        version: int,
        world_id: int,
        app_id: int,
        session_key: str,
        access_key: str,
        start_option: int,
        recv_flags: int,
    ) -> int:
        """The connect sequence of a game client: hello, CONNECT_TCP_AUX_UDP, ready-require, ready (`start_option` and
        `recv_flags` bytes), the aux UDP exchange, CONNECT_COMPLETE. Returns this player's DME index."""
        self.connection = Connection(self.host, self.port, self.name, self.timeout)
        self.connection.open()
        self.connection.send(rt.CLIENT_HELLO, struct.pack("<HHHH", 1, version, 1, 0))
        self._expect(rt.SERVER_HELLO)
        self.connection.send(
            rt.CLIENT_CONNECT_TCP_AUX_UDP, encode_connect_tcp_aux_udp(world_id, app_id, session_key, access_key)
        )
        self._expect(rt.SERVER_CONNECT_REQUIRE)
        self.connection.send(rt.CLIENT_CONNECT_READY_REQUIRE, b"\x00")
        accepted = _decode_accept(self._expect(rt.SERVER_CONNECT_ACCEPT_TCP).payload, 24, "SERVER_CONNECT_ACCEPT_TCP")
        self.index, self.scert_id = accepted.index, accepted.scert_id
        self.connection.send(rt.CLIENT_CONNECT_READY_TCP, bytes([start_option, recv_flags]))
        info = self._expect(rt.SERVER_INFO_AUX_UDP).payload
        if len(info) < 0x12:
            raise DecodeError(f"SERVER_INFO_AUX_UDP: {len(info)} bytes, need 18")
        udp_ip, udp_port = text_of(info[:16]), struct.unpack_from("<H", info, 16)[0]
        if udp_port == 0:
            raise DecodeError("SERVER_INFO_AUX_UDP names port 0")
        self._connect_udp(world_id, app_id, udp_ip or self.host, udp_port)
        self.connection.send(rt.CLIENT_CONNECT_READY_AUX_UDP, b"")
        self._expect(rt.SERVER_CONNECT_COMPLETE)
        self.completed = True
        return self.index

    def _connect_udp(self, world_id: int, app_id: int, server_ip: str, server_port: int) -> None:
        """Send CLIENT_CONNECT_AUX_UDP from a new UDP socket (up to 3 times, 1 s apart) until
        SERVER_CONNECT_ACCEPT_AUX_UDP answers with this player's index and scert id."""
        self.udp = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        self.udp.bind((self.host, 0))
        self.udp_server = (server_ip, server_port)
        packet = rt.encode_frame(
            rt.CLIENT_CONNECT_AUX_UDP,
            encode_connect_aux_udp(world_id, app_id, self.host, self.udp.getsockname()[1], self.index, self.scert_id),
        )
        for _attempt in range(3):
            self.udp.sendto(packet, self.udp_server)
            deadline = time.monotonic() + 1.0
            while time.monotonic() < deadline:
                readable, _, _ = select.select([self.udp], [], [], max(deadline - time.monotonic(), 0))
                if not readable:
                    break
                for frame in self._read_udp():
                    if frame.id == rt.SERVER_CONNECT_ACCEPT_AUX_UDP:
                        accepted = _decode_accept(frame.payload, 0x1A, "SERVER_CONNECT_ACCEPT_AUX_UDP")
                        if (accepted.index, accepted.scert_id) != (self.index, self.scert_id):
                            raise DecodeError(
                                f"{self.name}: aux UDP accept names player {accepted.index} / scert "
                                f"{accepted.scert_id}, the TCP accept named {self.index} / {self.scert_id}"
                            )
                        return
        raise ReplyTimeoutError(f"{self.name}: no SERVER_CONNECT_ACCEPT_AUX_UDP within 3 attempts")

    def _read_udp(self) -> List[rt.Frame]:
        data, _source = self.udp.recvfrom(0x700)
        frames, _rest = rt.decode_frames(data)
        out: List[rt.Frame] = []
        for frame in frames:
            if frame.id in (rt.AGGREGATE_RECV, rt.AGGREGATE_SEND):
                out.extend(rt.decode_aggregate(frame.payload))
            else:
                out.append(frame)
        return out

    def _expect(self, wanted: int) -> rt.Frame:
        """Wait for the frame `wanted`, recording every other frame that arrives first (a peer may be announced while
        this player is still connecting)."""
        deadline = time.monotonic() + self.timeout
        while time.monotonic() < deadline:
            for frame in self.connection.read_frames(max(deadline - time.monotonic(), 0)):
                if frame.id == wanted:
                    return frame
                self._handle(frame)
            if self.connection.peer_closed:
                raise ConnectionClosedError(
                    f"{self.name}: server closed the connection while waiting for {rt.frame_name(wanted)}"
                )
        raise ReplyTimeoutError(f"{self.name}: no {rt.frame_name(wanted)} within {self.timeout:.1f} s")

    # ----- afterwards

    def sockets(self) -> list:
        """The sockets `service` reads, for `select` over several clients."""
        out = []
        if self.connection is not None and self.connection.is_open and not self.connection.peer_closed:
            out.append(self.connection)
        if self.udp is not None:
            out.append(self.udp)
        return out

    @property
    def peer_closed(self) -> bool:
        """True once the server closed this client's DME connection."""
        return self.connection is not None and self.connection.peer_closed

    def service(self) -> None:
        """Read whatever has arrived (without blocking): the server's echo is answered with the same payload and its
        ping (class 0, id 1) with an 8-byte reply, as the game does; every other frame is recorded in `received`."""
        if self.connection is not None and self.connection.is_open:
            for frame in self.connection.read_frames(0):
                self._handle(frame)
        if self.udp is not None:
            while select.select([self.udp], [], [], 0)[0]:
                for frame in self._read_udp():
                    self._handle(frame)
        if self._announce and self.completed and self.peers and self.connection.is_open:
            self._announce = False
            self.send_app_list(sorted(self.peers), self.client_info())

    def _handle(self, frame: rt.Frame) -> None:
        if frame.id == rt.SERVER_ECHO:
            self.connection.send(rt.CLIENT_ECHO, frame.payload)
        elif frame.id == rt.SERVER_APP and frame.payload[:2] == b"\x00\x01":
            self.connection.send(rt.CLIENT_APP_TOSERVER, b"\x00\x01" + bytes(8))
        elif frame.id != rt.CLIENT_ECHO:  # the server's reply to our echo
            self.received.append(Received(time.monotonic(), frame.id, frame.payload))
            self._track_peers(frame)

    def _track_peers(self, frame: rt.Frame) -> None:
        if not self.peer_exchange:
            return
        if frame.id == rt.SERVER_CONNECT_NOTIFY and len(frame.payload) >= CONNECT_NOTIFY_LENGTH:
            self._add_peer(decode_peer_notice(frame.payload, CONNECT_NOTIFY_LENGTH).index, "notice")
        elif frame.id == rt.SERVER_DISCONNECT_NOTIFY and len(frame.payload) >= PEER_NOTIFY_LENGTH:
            self.peers.pop(decode_peer_notice(frame.payload).index, None)
        elif frame.id == rt.CLIENT_APP_SINGLE and len(frame.payload) >= 2:
            sender = struct.unpack_from("<h", frame.payload, 0)[0]
            is_new = self._add_peer(sender, "message")
            if is_new and len(frame.payload) - 2 == CLIENT_INFO_LENGTH:
                self._announce = True  # the immediate answer to a first ClientInfo (ASSUMED, see the class)

    def _add_peer(self, index: int, source: str) -> bool:
        """Add DME `index` to the peer set (never this player); True when it was not in it."""
        if index == self.index or index in self.peers:
            return False
        self.peers[index] = source
        self._announce = (
            self._announce or source == "notice"
        )  # a notice makes the peer address this player: send it a ClientInfo
        return True

    def client_info(self) -> bytes:
        """This player's ClientInfo message body (CLIENT_INFO_LENGTH bytes). Only the length is known from the game;
        the name at the start is ASSUMED and the rest is zero."""
        name = self.name.encode("ascii", "replace")[:CLIENT_INFO_NAME_LENGTH]
        return name.ljust(CLIENT_INFO_LENGTH, b"\x00")

    def notices(self, frame_id: int) -> List[PeerNotice]:
        """The peer notices received so far for `frame_id` (rt.SERVER_CONNECT_NOTIFY or rt.SERVER_DISCONNECT_NOTIFY)."""
        minimum = CONNECT_NOTIFY_LENGTH if frame_id == rt.SERVER_CONNECT_NOTIFY else PEER_NOTIFY_LENGTH
        return [decode_peer_notice(item.payload, minimum) for item in self.received if item.frame_id == frame_id]

    def app_messages(self) -> List[AppMessage]:
        """The application messages relayed to this player so far (RT id 3: i16 sender DME index, then the payload)."""
        return [
            AppMessage(item.time, struct.unpack_from("<h", item.payload, 0)[0], item.payload[2:])
            for item in self.received
            if item.frame_id == rt.CLIENT_APP_SINGLE and len(item.payload) >= 2
        ]

    # ----- send, leave, drop

    def send_app_list(self, targets: List[int], payload: bytes) -> None:
        """An application message to the players at DME indexes `targets` (APP_LIST: u8 mask length, bit mask with bit
        i = DME index i, payload), which is how the game sends its ClientInfo and GameInfo messages."""
        length = max(targets) // 8 + 1
        mask = bytearray(length)
        for target in targets:
            mask[target // 8] |= 1 << (target % 8)
        self.connection.send(rt.CLIENT_APP_LIST, bytes([length]) + bytes(mask) + payload)

    def leave(self) -> None:
        """The game's DME disconnect: CLIENT_DISCONNECT_WITH_REASON (reason 0), then both sockets closed."""
        if self.connection is not None and self.connection.is_open:
            try:
                self.connection.disconnect()
            except OSError:
                log.debug("%s: disconnect not delivered", self.name)
        self.close()

    def close(self) -> None:
        """Close both sockets without telling the server anything."""
        if self.connection is not None:
            self.connection.close()
        if self.udp is not None:
            self.udp.close()
            self.udp = None
