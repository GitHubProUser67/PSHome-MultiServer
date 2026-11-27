"""RT framing: the transport layer under every Medius message (unencrypted mode only)."""

import struct
from dataclasses import dataclass
from typing import List, Tuple

from .errors import DecodeError

CLIENT_CONNECT_TCP = 0x00
CLIENT_DISCONNECT = 0x01
CLIENT_APP_BROADCAST = 0x02
CLIENT_APP_SINGLE = 0x03
CLIENT_APP_LIST = 0x04
CLIENT_ECHO = 0x05
SERVER_CONNECT_REJECT = 0x06
SERVER_CONNECT_ACCEPT_TCP = 0x07
SERVER_CONNECT_NOTIFY = 0x08
SERVER_DISCONNECT_NOTIFY = 0x09
SERVER_APP = 0x0A
CLIENT_APP_TOSERVER = 0x0B
SERVER_FORCED_DISCONNECT = 0x11
CLIENT_CONNECT_TCP_AUX_UDP = 0x15
CLIENT_CONNECT_AUX_UDP = 0x16
CLIENT_CONNECT_READY_AUX_UDP = 0x17
SERVER_INFO_AUX_UDP = 0x18
SERVER_CONNECT_ACCEPT_AUX_UDP = 0x19
SERVER_CONNECT_COMPLETE = 0x1A
SERVER_ECHO = 0x1F
CLIENT_DISCONNECT_WITH_REASON = 0x20
CLIENT_CONNECT_READY_TCP = 0x21
SERVER_CONNECT_REQUIRE = 0x22
CLIENT_CONNECT_READY_REQUIRE = 0x23
CLIENT_HELLO = 0x24
SERVER_HELLO = 0x25
SERVER_STARTUP_INFO_NOTIFY = 0x26
AGGREGATE_SEND = 0x3B
AGGREGATE_RECV = 0x3C

_NAMES = {value: name for name, value in list(globals().items()) if name.isupper() and isinstance(value, int)}


def frame_name(frame_id: int) -> str:
    """Symbolic name of an RT message id, for log lines and error messages."""
    return _NAMES.get(frame_id & 0x7F, "RT_0x%02X" % (frame_id & 0x7F))


@dataclass
class Frame:
    id: int
    payload: bytes


def encode_frame(frame_id: int, payload: bytes = b"") -> bytes:
    """One RT frame as sent unencrypted.

    Layout: u8 id (bit 7 clear), u16 payload length (little endian), payload.
    """
    if len(payload) > 0xFFFF:
        raise ValueError(f"RT payload of {len(payload)} bytes does not fit a u16 length")
    return struct.pack("<BH", frame_id & 0x7F, len(payload)) + bytes(payload)


def decode_frames(buffer: bytes) -> Tuple[List[Frame], bytes]:
    """Split as many whole frames as the buffer holds; returns (frames, remaining bytes of a partial frame).

    Layout: u8 id, u16 payload length; when bit 7 of the id is set and the payload is not empty a u32 hash word follows
    the length (it is skipped: encrypted sessions are not spoken). The id is masked with 0x7F.
    """
    frames, position = [], 0
    while len(buffer) - position >= 3:
        raw_id = buffer[position]
        length = buffer[position + 1] | (buffer[position + 2] << 8)
        header = 7 if (raw_id & 0x80 and length) else 3
        if len(buffer) - position < header + length:
            break
        frames.append(Frame(raw_id & 0x7F, bytes(buffer[position + header : position + header + length])))
        position += header + length
    return frames, bytes(buffer[position:])


def decode_aggregate(payload: bytes) -> List[Frame]:
    """Sub-frames of an aggregate frame (ids 0x3B/0x3C). Layout, repeated: u8 id, u16 length, data (no hash word)."""
    frames, position = [], 0
    while position < len(payload):
        if len(payload) - position < 3:
            raise DecodeError("aggregate frame: truncated sub-frame header")
        sub_id = payload[position]
        length = payload[position + 1] | (payload[position + 2] << 8)
        if len(payload) - position - 3 < length:
            raise DecodeError(f"aggregate frame: sub-frame 0x{sub_id:02X} claims {length} bytes, fewer remain")
        frames.append(Frame(sub_id & 0x7F, bytes(payload[position + 3 : position + 3 + length])))
        position += 3 + length
    return frames
