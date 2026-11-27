"""Medius Universe Information Server (MUIS) client: lists the universes and where their login servers are."""

import logging
import struct
from dataclasses import dataclass
from typing import Optional

from . import rt
from .connection import Connection, Handshake
from .errors import DecodeError
from .medius import (
    GET_UNIVERSE_INFORMATION,
    MESSAGE_ID_LENGTH,
    UNIVERSE_INFORMATION_RESPONSE,
    UNIVERSE_SVO_URL,
    encode_universe_request,
    text_of,
)

log = logging.getLogger(__name__)

# Bits of the universe information filter: which optional fields a reply carries.
FILTER_ID = 1 << 2
FILTER_NAME = 1 << 3
FILTER_DNS = 1 << 4
FILTER_DESCRIPTION = 1 << 5
FILTER_STATUS = 1 << 6
FILTER_BILLING = 1 << 7
FILTER_EXTRA = 1 << 8
FILTER_SVO_URL = 1 << 9
DEFAULT_FILTER = (
    FILTER_ID | FILTER_NAME | FILTER_DNS | FILTER_DESCRIPTION | FILTER_STATUS | FILTER_EXTRA | FILTER_SVO_URL
)


@dataclass
class UniverseInfo:
    """One universe entry. Fields the filter did not select are None."""

    status: int
    filter: int
    universe_id: Optional[int] = None
    name: Optional[str] = None
    dns: Optional[str] = None
    port: Optional[int] = None
    description: Optional[str] = None
    universe_status: Optional[int] = None
    users: Optional[int] = None
    max_users: Optional[int] = None
    extra: Optional[str] = None
    end_of_list: Optional[bool] = None
    svo_url: Optional[str] = None


def decode_universe_info(body: bytes) -> UniverseInfo:
    """UniverseVariableInformationResponse: message id [0x00], i32 status [0x15], u32 filter [0x19], then
    in this order, each only if its filter bit is set: u32 id; char name[128]; char dns[128] and i32 port; char
    description[256]; i32 status, i32 users, i32 max users; char billing[8] and char billing name[128]; char extra[128].
    A SVO url[128] follows for some applications; the last byte is the end-of-list flag."""
    if len(body) < MESSAGE_ID_LENGTH + 8:
        raise DecodeError(
            f"{UNIVERSE_INFORMATION_RESPONSE.name}: body is {len(body)} bytes, "
            f"the status and filter need {MESSAGE_ID_LENGTH + 8}"
        )
    offset = MESSAGE_ID_LENGTH
    status, info_filter = struct.unpack_from("<iI", body, offset)
    offset += 8
    info = UniverseInfo(status, info_filter)
    try:
        offset = _read_selected_fields(info, body, offset)
    except struct.error:
        raise DecodeError(
            f"{UNIVERSE_INFORMATION_RESPONSE.name}: body of {len(body)} bytes ends inside a selected field"
        ) from None
    info.end_of_list = (body[-1] != 0) if len(body) > offset else None
    return info


def _read_selected_fields(info: UniverseInfo, body: bytes, offset: int) -> int:
    selected = info.filter
    if selected & FILTER_ID:
        info.universe_id = struct.unpack_from("<I", body, offset)[0]
        offset += 4
    if selected & FILTER_NAME:
        info.name = text_of(body[offset : offset + 128])
        offset += 128
    if selected & FILTER_DNS:
        info.dns = text_of(body[offset : offset + 128])
        info.port = struct.unpack_from("<i", body, offset + 128)[0]
        offset += 132
    if selected & FILTER_DESCRIPTION:
        info.description = text_of(body[offset : offset + 256])
        offset += 256
    if selected & FILTER_STATUS:
        info.universe_status, info.users, info.max_users = struct.unpack_from("<iii", body, offset)
        offset += 12
    if selected & FILTER_BILLING:
        offset += 8 + 128
    if selected & FILTER_EXTRA:
        info.extra = text_of(body[offset : offset + 128])
        offset += 128
    return offset


class MuisClient:
    """A MUIS connection for one application. The title supplies its application id, Medius version and world id."""

    def __init__(self, host: str, port: int, app_id: int, version: int, world_id: int):
        self.app_id, self.version, self.world_id = app_id, version, world_id
        self.connection = Connection(host, port, name="muis")
        self.handshake: Optional[Handshake] = None

    def __enter__(self) -> "MuisClient":
        return self

    def __exit__(self, *exc_info) -> None:
        self.close()

    def connect(self) -> Handshake:
        """Open the connection and run the handshake."""
        self.connection.open()
        self.handshake = self.connection.handshake(self.app_id, self.version, self.world_id)
        return self.handshake

    def get_universe(self, info_filter: int = DEFAULT_FILTER) -> UniverseInfo:
        """Ask for the universe list; returns the first entry. If the server sent its SVO url message (class 4 id 0x1E:
        u8 length, char url[]) before the entry, it is in `svo_url`."""
        self.connection.send_medius(GET_UNIVERSE_INFORMATION, encode_universe_request("universe", info_filter))
        svo_url = None
        while True:
            message, body = self.connection.receive_medius(UNIVERSE_SVO_URL, UNIVERSE_INFORMATION_RESPONSE)
            if message == UNIVERSE_SVO_URL:
                svo_url = text_of(body[MESSAGE_ID_LENGTH + 1 :])
                continue
            info = decode_universe_info(body)
            info.svo_url = svo_url
            if info.end_of_list is not False:
                return info

    def echo(self, payload: bytes) -> bytes:
        """Send a CLIENT_ECHO frame and return the payload of the server's echo."""
        self.connection.send(rt.CLIENT_ECHO, payload)
        return self.connection.receive(rt.CLIENT_ECHO).payload

    def close(self) -> None:
        """Tell the server this client is leaving, then close the socket."""
        if self.connection.is_open:
            try:
                self.connection.disconnect()
            except OSError:
                log.debug("muis: disconnect not delivered")
        self.connection.close()
