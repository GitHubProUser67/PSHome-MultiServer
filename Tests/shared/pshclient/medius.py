"""Medius messages: type table, request encoders and reply decoders.

A Medius message travels inside an RT frame as `u8 class, u8 id, body`. Bodies start with a 21-byte NUL padded message
id (the client's own label, echoed in the reply); offsets below are body offsets, counted from the start of that id.
Status codes are little endian i32.
"""

import struct
from dataclasses import dataclass
from typing import NamedTuple, Optional, Tuple

from .errors import DecodeError

MESSAGE_ID_LENGTH = 21
SESSION_KEY_LENGTH = 17
STATUS_OFFSET = 0x18


class MessageType(NamedTuple):
    class_id: int
    message_id: int
    name: str


# Requests (client to server)
GET_UNIVERSE_INFORMATION = MessageType(1, 0xC8, "GetUniverseInformation")
SESSION_BEGIN = MessageType(4, 0x8B, "SessionBegin")
DNAS_SIGNATURE_POST = MessageType(4, 0x0A, "DnasSignaturePost")
SET_LOCALIZATION_PARAMS = MessageType(4, 0x7B, "SetLocalizationParams")
ACCOUNT_LOGIN = MessageType(1, 0x07, "AccountLogin")
ACCOUNT_REGISTRATION = MessageType(1, 0x09, "AccountRegistration")
TICKET_LOGIN = MessageType(4, 0x58, "TicketLogin")
SESSION_END = MessageType(1, 0x05, "SessionEnd")
ACCOUNT_UPDATE_STATS = MessageType(1, 0x11, "AccountUpdateStats")
PLAYER_INFO = MessageType(1, 0x31, "PlayerInfo")
NPID_POST = MessageType(4, 0x6D, "NpIdPost")
GET_SERVER_TIME = MessageType(4, 0x2A, "GetServerTime")
GET_LOCATIONS = MessageType(1, 0x8C, "GetLocations")
CHANNEL_LIST = MessageType(4, 0x1F, "ChannelList_ExtraInfo")
JOIN_CHANNEL = MessageType(1, 0x25, "JoinChannel")
SET_LOBBY_WORLD_FILTER = MessageType(4, 0x12, "SetLobbyWorldFilter")
SET_GAME_LIST_FILTER = MessageType(4, 0x33, "SetGameListFilter")
GAME_LIST = MessageType(4, 0x37, "GetGames_ExtraInfo")
GAME_INFO = MessageType(4, 0x35, "GameInfo_ExtraInfo")
CREATE_GAME = MessageType(4, 0x2F, "CreateGame_ExtraInfo")
JOIN_GAME = MessageType(1, 0xF3, "JoinGame")
WORLD_REPORT = MessageType(4, 0x30, "WorldReport")
PLAYER_REPORT = MessageType(1, 0x01, "PlayerReport")
END_GAME_REPORT = MessageType(1, 0x02, "EndGameReport")
UPDATE_USER_STATE = MessageType(1, 0x49, "UpdateUserState")

# Replies (server to client)
UNIVERSE_SVO_URL = MessageType(4, 0x1E, "UniverseSvoUrl")
UNIVERSE_INFORMATION_RESPONSE = MessageType(4, 0x11, "UniverseVariableInformationResponse")
SESSION_BEGIN_RESPONSE = MessageType(1, 0x04, "SessionBeginResponse")
STATUS_RESPONSE = MessageType(1, 0xA4, "StatusResponse")
ACCOUNT_LOGIN_RESPONSE = MessageType(1, 0x08, "AccountLoginResponse")
ACCOUNT_REGISTRATION_RESPONSE = MessageType(1, 0x0A, "AccountRegistrationResponse")
TICKET_LOGIN_RESPONSE = MessageType(4, 0x59, "TicketLoginResponse")
PLAYER_INFO_RESPONSE = MessageType(1, 0x32, "PlayerInfoResponse")
ACCOUNT_UPDATE_STATS_RESPONSE = MessageType(1, 0x12, "AccountUpdateStatsResponse")
NPID_POST_RESPONSE = MessageType(4, 0x6E, "NpIdPostResponse")
GET_SERVER_TIME_RESPONSE = MessageType(4, 0x2B, "GetServerTimeResponse")
GET_LOCATIONS_RESPONSE = MessageType(1, 0x8D, "GetLocationsResponse")
SESSION_END_RESPONSE = MessageType(1, 0x06, "SessionEndResponse")
CHANNEL_LIST_RESPONSE = MessageType(1, 0xED, "ChannelListResponse")
JOIN_CHANNEL_RESPONSE = MessageType(1, 0x26, "JoinChannelResponse")
SET_LOBBY_WORLD_FILTER_RESPONSE = MessageType(4, 0x13, "SetLobbyWorldFilterResponse")
SET_GAME_LIST_FILTER_RESPONSE = MessageType(4, 0x34, "SetGameListFilterResponse")
GAME_LIST_RESPONSE = MessageType(4, 0x38, "GameListResponse")
GAME_INFO_RESPONSE = MessageType(4, 0x36, "GameInfoResponse")
CREATE_GAME_RESPONSE = MessageType(1, 0x1E, "CreateGameResponse")
JOIN_GAME_RESPONSE = MessageType(1, 0x24, "JoinGameResponse")

FRAGMENT = MessageType(0, 2, "Fragment")


class Status:
    """Status codes seen in replies (MediusCallbackStatus)."""

    SUCCESS = 0
    NO_RESULT = 1
    GAME_NOT_FOUND = -977  # GameInfo for a game the server no longer has
    TRANSACTION_TIMED_OUT = -931  # CreateGame before the DME has registered with the lobby server
    ACCOUNT_NOT_FOUND = -998
    DB_ERROR = -988


def message_name(class_id: int, message_id: int) -> str:
    for message_type in _ALL_TYPES:
        if (message_type.class_id, message_type.message_id) == (class_id, message_id):
            return message_type.name
    return f"class {class_id} id 0x{message_id:02X}"


_ALL_TYPES = [value for value in list(globals().values()) if isinstance(value, MessageType)]


# ----- field helpers


def message_id_field(label: str) -> bytes:
    return label.encode("latin-1").ljust(MESSAGE_ID_LENGTH, b"\0")


def fixed(text, length: int) -> bytes:
    """NUL padded, truncated field of `length` bytes."""
    data = text.encode("latin-1") if isinstance(text, str) else bytes(text)
    return data[:length].ljust(length, b"\0")


def text_of(data: bytes) -> str:
    return data.split(b"\0", 1)[0].decode("latin-1")


def status_of(body: bytes, message: str) -> int:
    """The i32 status at body offset 0x18 that every reply carries."""
    if len(body) < STATUS_OFFSET + 4:
        raise DecodeError(f"{message}: body is {len(body)} bytes, the status at 0x18 needs {STATUS_OFFSET + 4}")
    return struct.unpack_from("<i", body, STATUS_OFFSET)[0]


# ----- request encoders


def encode_universe_request(label: str, info_filter: int) -> bytes:
    """GetUniverseInformation (0x2C bytes): message id [0x00], 3 pad, u32 info filter [0x18], u32 character encoding
    [0x1C] = 1, u32 language [0x20] = 1."""
    return message_id_field(label) + bytes(3) + struct.pack("<III", info_filter, 1, 1)


def encode_session_begin(label: str, connection_class: int = 2) -> bytes:
    """SessionBegin (0x2C bytes): message id, 3 pad, i32 connection class [0x18], u32 x4 [0x1C] = 3, 5, 0, 0x2E4DB."""
    return (
        message_id_field(label)
        + bytes(3)
        + struct.pack("<i", connection_class)
        + struct.pack("<IIII", 3, 5, 0, 0x2E4DB)
    )


def encode_dnas_signature(label: str, session_key: str, console_id: bytes = bytes(16)) -> bytes:
    """DnasSignaturePost (0x50 bytes): message id, session key [0x15], 2 pad, i32 type [0x28] = 0 (console id),
    u8 length [0x2C] = 0x10, signature[0x20] [0x2D], 3 pad."""
    return (
        message_id_field(label)
        + fixed(session_key, SESSION_KEY_LENGTH)
        + bytes(2)
        + struct.pack("<i", 0)
        + bytes([0x10])
        + fixed(console_id, 0x20)
        + bytes(3)
    )


def encode_localization(label: str, session_key: str, language: int = 1, location: int = 0) -> bytes:
    """SetLocalizationParams (0x38 bytes): message id, session key [0x15], 2 pad, i32 character encoding [0x28] = 2,
    i32 language [0x2C], i32 time zone [0x30] = 0, i32 location id [0x34]."""
    return (
        message_id_field(label)
        + fixed(session_key, SESSION_KEY_LENGTH)
        + bytes(2)
        + struct.pack("<iiii", 2, language, 0, location)
    )


def encode_account_login(label: str, session_key: str, name: str, password: str) -> bytes:
    """AccountLogin (0x66 bytes): message id, session key [0x15], char name[32] [0x26], char password[32] [0x46]."""
    return message_id_field(label) + fixed(session_key, SESSION_KEY_LENGTH) + fixed(name, 32) + fixed(password, 32)


def encode_account_registration(label: str, session_key: str, name: str, password: str, account_type: int = 1) -> bytes:
    """AccountRegistration (0x6C bytes): message id, session key [0x15], 2 pad, i32 account type [0x28] (1 = master),
    char name[32] [0x2C], char password[32] [0x4C]."""
    return (
        message_id_field(label)
        + fixed(session_key, SESSION_KEY_LENGTH)
        + bytes(2)
        + struct.pack("<i", account_type)
        + fixed(name, 32)
        + fixed(password, 32)
    )


def encode_ticket_login(label: str, session_key: str, ticket: bytes) -> bytes:
    """TicketLogin: message id, session key [0x15], u32 unknown [0x26] = 0, then the NP ticket verbatim: its first four
    bytes (the version word), its next four bytes (a big endian body length) and that many bytes of ticket data. Only a
    ticket whose header is `u32 version, u32 length` (version 2.x) can be carried; see pshclient.ticket."""
    return message_id_field(label) + fixed(session_key, SESSION_KEY_LENGTH) + struct.pack("<I", 0) + ticket


def encode_session_end(label: str, session_key: str) -> bytes:
    """SessionEnd (0x26 bytes): message id, session key [0x15]."""
    return message_id_field(label) + fixed(session_key, SESSION_KEY_LENGTH)


def encode_player_info(label: str, session_key: str, account_id: int) -> bytes:
    """PlayerInfo (0x2C bytes): message id, session key [0x15], 2 pad, i32 account id [0x28]."""
    return message_id_field(label) + fixed(session_key, SESSION_KEY_LENGTH) + bytes(2) + struct.pack("<i", account_id)


def encode_account_update_stats(label: str, session_key: str, stats: bytes = b"") -> bytes:
    """AccountUpdateStats (0x126 bytes): message id, session key [0x15], stats[0x100] [0x26]."""
    return message_id_field(label) + fixed(session_key, SESSION_KEY_LENGTH) + fixed(stats, 256)


def encode_npid_post(label: str, session_key: str, handle: str) -> bytes:
    """NpIdPost (0x4A bytes): message id, session key [0x15], SceNpId: handle[16] [0x26], u8 terminator, 3 pad,
    opt[8], reserved[8]."""
    return message_id_field(label) + fixed(session_key, SESSION_KEY_LENGTH) + fixed(handle, 16) + bytes(4) + bytes(16)


def encode_get_server_time(label: str) -> bytes:
    """GetServerTime (0x15 bytes): message id only."""
    return message_id_field(label)


def encode_get_locations(label: str, session_key: str) -> bytes:
    """GetLocations (0x28 bytes): message id, session key [0x15], 2 pad."""
    return message_id_field(label) + fixed(session_key, SESSION_KEY_LENGTH) + bytes(2)


# ----- reply decoders


@dataclass
class StatusReply:
    """Any reply whose only content is a status code."""

    status: int
    body_length: int


def decode_status_reply(body: bytes, message: str) -> StatusReply:
    """Layout: message id [0x00], 3 pad, i32 status [0x18]."""
    return StatusReply(status_of(body, message), len(body))


@dataclass
class SessionBeginReply:
    status: int
    session_key: str


def decode_session_begin_reply(body: bytes) -> SessionBeginReply:
    """SessionBeginResponse: message id, 3 pad, i32 status [0x18], session key[17] [0x1C], 3 pad. The key is '' when the
    body is too short to hold it."""
    status = status_of(body, SESSION_BEGIN_RESPONSE.name)
    key = text_of(body[0x1C : 0x1C + SESSION_KEY_LENGTH]) if len(body) >= 0x1C + SESSION_KEY_LENGTH else ""
    return SessionBeginReply(status, key)


@dataclass
class Address:
    kind: int
    ip: str
    port: int


@dataclass
class ConnectionInfo:
    """Where and how to connect to the next server (NetConnectionInfo)."""

    kind: int
    addresses: Tuple[Address, Address]
    world_id: int
    server_key: bytes
    session_key: str
    access_key: str


CONNECTION_INFO_LENGTH = 0x9C


def decode_connection_info(data: bytes) -> ConnectionInfo:
    """NetConnectionInfo (0x9C bytes): i32 type [0x00], two addresses of {u32 kind, char ip[16], i32 port} [0x04, 0x1C],
    i32 world id [0x34], u8 server key[64] [0x38], char session key[17] [0x78], char access key[17] [0x89], 2 pad."""
    if len(data) < CONNECTION_INFO_LENGTH:
        raise DecodeError(f"NetConnectionInfo: {len(data)} bytes, need {CONNECTION_INFO_LENGTH}")
    kind = struct.unpack_from("<i", data, 0)[0]
    addresses, offset = [], 4
    for _ in range(2):
        address_kind = struct.unpack_from("<I", data, offset)[0]
        ip = text_of(data[offset + 4 : offset + 20])
        port = struct.unpack_from("<i", data, offset + 20)[0]
        addresses.append(Address(address_kind, ip, port))
        offset += 24
    world_id = struct.unpack_from("<i", data, offset)[0]
    return ConnectionInfo(
        kind,
        (addresses[0], addresses[1]),
        world_id,
        bytes(data[offset + 4 : offset + 68]),
        text_of(data[offset + 68 : offset + 85]),
        text_of(data[offset + 85 : offset + 102]),
    )


@dataclass
class AccountLoginReply:
    status: int
    account_id: Optional[int]
    account_type: Optional[int]
    world_id: Optional[int]
    connection: Optional[ConnectionInfo]
    body_length: int


def decode_account_login_reply(body: bytes) -> AccountLoginReply:
    """AccountLoginResponse (0xC4 bytes): message id, 3 pad, i32 status [0x18], i32 account id [0x1C], i32 account type
    [0x20], i32 world id [0x24], NetConnectionInfo [0x28]. The fields after the status are None when the body is too
    short to hold them (a failed login sends a short body)."""
    status = status_of(body, ACCOUNT_LOGIN_RESPONSE.name)
    account_id = account_type = world_id = connection = None
    if len(body) >= 0x28:
        account_id, account_type, world_id = struct.unpack_from("<iii", body, 0x1C)
    if len(body) >= 0x28 + CONNECTION_INFO_LENGTH:
        connection = decode_connection_info(body[0x28 : 0x28 + CONNECTION_INFO_LENGTH])
    return AccountLoginReply(status, account_id, account_type, world_id, connection, len(body))


@dataclass
class ServerTimeReply:
    status: int
    gmt_seconds: Optional[int]


def decode_server_time_reply(body: bytes) -> ServerTimeReply:
    """GetServerTimeResponse: message id, 3 pad, i32 status [0x18], i32 GMT seconds since 1970 [0x1C]."""
    status = status_of(body, GET_SERVER_TIME_RESPONSE.name)
    seconds = struct.unpack_from("<i", body, 0x1C)[0] if len(body) >= 0x20 else None
    return ServerTimeReply(status, seconds)
