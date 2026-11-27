"""The lobby server (MLS) after login: channels, the game list, creating and joining a game, and the reports a client
posts while it is in a game and when it leaves.

Bodies are `message id, ...` like every Medius request (see medius.py); offsets are body offsets. The three reports
(WorldReport, PlayerReport, EndGameReport) and UpdateUserState are posted without a reply.
"""

import struct
from dataclasses import dataclass
from typing import List, Optional, Tuple

from . import medius
from .connection import Connection
from .errors import DecodeError
from .medius import (
    SESSION_KEY_LENGTH,
    ConnectionInfo,
    decode_connection_info,
    fixed,
    message_id_field,
    status_of,
    text_of,
)

# MediusUserAction values of UpdateUserState: a client that leaves a game posts "left game world", then "joined chat
# world".
USER_JOINED_CHAT_WORLD = 1
USER_LEFT_GAME_WORLD = 2

# Smallest reply bodies the decoders accept.
CHANNEL_ROW_LENGTH = 0x84
GAME_ROW_LENGTH = 0x1A4
GAME_INFO_LENGTH = 0x1A0


@dataclass(frozen=True)
class GameParameters:
    """What a client puts in a CreateGame request and repeats in its WorldReport. A title chooses the values (they are
    the attributes the game shows in its lobby list)."""

    name: str
    min_players: int
    max_players: int
    level: int
    skill: int
    rules: int
    generic: Tuple[int, ...]  # GenericField1..8
    host_type: int


# ----- request encoders


def encode_channel_list(label: str, page_id: int, page_size: int) -> bytes:
    """ChannelList_ExtraInfo and GetGames_ExtraInfo (0x1A bytes): message id, 1 pad, u16 page id [0x16], u16 page size
    [0x18]. The game sets the page id to page * page size."""
    return message_id_field(label) + b"\0" + struct.pack("<HH", page_id, page_size)


def encode_join_channel(label: str, session_key: str, world_id: int, password: str = "") -> bytes:
    """JoinChannel (0x4C bytes): message id, session key [0x15], 2 pad, i32 world id [0x28], char password[32]
    [0x2C]."""
    return (
        message_id_field(label)
        + fixed(session_key, SESSION_KEY_LENGTH)
        + bytes(2)
        + struct.pack("<i", world_id)
        + fixed(password, 32)
    )


def encode_lobby_world_filter(label: str, masks: Tuple[int, int, int, int] = (0, 0, 0, 0)) -> bytes:
    """SetLobbyWorldFilter (0x30 bytes): message id, 3 pad, u32 mask x4 [0x18], i32 filter type [0x28], i32 mask level
    [0x2C]. The game's own values are not known; the layout is the server model's (ASSUMED)."""
    return message_id_field(label) + bytes(3) + struct.pack("<IIIIii", *masks, 0, 0)


def encode_game_list_filter(label: str, field: int, mask: int, operator: int, baseline: int) -> bytes:
    """SetGameListFilter (0x28 bytes): message id, 3 pad, i32 filter field [0x18], u32 mask, i32 comparison operator,
    i32 baseline value. Layout is the server model's and the values are the game's best guess (ASSUMED)."""
    return message_id_field(label) + bytes(3) + struct.pack("<iIii", field, mask, operator, baseline)


def encode_game_info(label: str, session_key: str, world_id: int) -> bytes:
    """GameInfo_ExtraInfo (0x2C bytes): message id, session key [0x15], 2 pad, i32 world id [0x28]."""
    return message_id_field(label) + fixed(session_key, SESSION_KEY_LENGTH) + bytes(2) + struct.pack("<i", world_id)


def encode_create_game(label: str, session_key: str, app_id: int, game: GameParameters, size: int = 0xFC) -> bytes:
    """CreateGame_ExtraInfo (0xFC bytes): message id, session key [0x15], 2 pad, i32 application id [0x28], i32 min
    players [0x2C], i32 max players [0x30], i32 game level [0x34], char game name[64] [0x38], char game password[32]
    [0x78], char spectator password[32] [0x98], i32 skill level [0xB8], i32 rules set [0xBC], i32 generic field 1..8
    [0xC0..0xDC], i32 host type [0xE0], i32 world attributes [0xE4], zero padding to `size` (the game declares 0xFC)."""
    body = (
        message_id_field(label)
        + fixed(session_key, SESSION_KEY_LENGTH)
        + bytes(2)
        + struct.pack("<iiii", app_id, game.min_players, game.max_players, game.level)
        + fixed(game.name, 64)
        + fixed("", 32)
        + fixed("", 32)
        + struct.pack("<ii", game.skill, game.rules)
        + struct.pack("<8i", *game.generic)
        + struct.pack("<ii", game.host_type, 0)
    )
    return body.ljust(size, b"\0")


def encode_join_game(label: str, session_key: str, world_id: int, host_type: int) -> bytes:
    """JoinGame (0xC4 bytes): message id, session key [0x15], 2 pad, i32 world id [0x28], i32 join type [0x2C] = 0,
    char game password[32] [0x30], i32 host type [0x50], public key[64] [0x54] = 0, address list 2 x 0x18 [0x94] = 0."""
    return (
        message_id_field(label)
        + fixed(session_key, SESSION_KEY_LENGTH)
        + bytes(2)
        + struct.pack("<ii", world_id, 0)
        + fixed("", 32)
        + struct.pack("<i", host_type)
        + bytes(64)
        + bytes(48)
    )


def encode_world_report(world_id: int, player_count: int, game: GameParameters, status: int = 2) -> bytes:
    """WorldReport (0x180 bytes, no message id): i32 world id [0], i32 player count [4], char game name[64] [8], game
    stats[256] [0x48] = 0, i32 min [0x148], max, level, skill, rules [0x158], generic field 1..8 [0x15C], i32 world
    status [0x17C] (2 = active)."""
    return (
        struct.pack("<ii", world_id, player_count)
        + fixed(game.name, 64)
        + bytes(256)
        + struct.pack("<5i", game.min_players, game.max_players, game.level, game.skill, game.rules)
        + struct.pack("<8i", *game.generic)
        + struct.pack("<i", status)
    )


def encode_player_report(session_key: str, world_id: int) -> bytes:
    """PlayerReport (0x1C bytes, no message id): session key[17], 3 pad, i32 world id [0x14], 4 bytes of stats."""
    return (fixed(session_key, SESSION_KEY_LENGTH) + bytes(3) + struct.pack("<i", world_id)).ljust(0x1C, b"\0")


def encode_update_user_state(session_key: str, action: int) -> bytes:
    """UpdateUserState (0x1C bytes, no message id): session key[17], 3 pad, i32 user action [0x14], 4 spare bytes."""
    return (fixed(session_key, SESSION_KEY_LENGTH) + bytes(3) + struct.pack("<i", action)).ljust(0x1C, b"\0")


def encode_end_game_report(session_key: str, world_id: int) -> bytes:
    """EndGameReport (0x7C bytes, no message id): session key[17], 3 pad, i32 world id [0x14], winning team[64] [0x18]
    and winning player[32] [0x58] empty, i32 final score [0x78] = 0."""
    return fixed(session_key, SESSION_KEY_LENGTH) + bytes(3) + struct.pack("<i", world_id) + bytes(64 + 32 + 4)


# ----- reply decoders


@dataclass
class ChannelRow:
    """One ChannelList_ExtraInfo reply row (one message per channel; the last has end_of_list)."""

    status: int
    world_id: int
    players: int
    max_players: int
    games: int
    name: str
    end_of_list: bool


def decode_channel_row(body: bytes) -> ChannelRow:
    """Layout: status [0x18], i32 world id [0x1C], u16 players [0x20], u16 max players [0x22], u16 game worlds [0x24],
    i32 security [0x28], generic field 1..4 [0x2C], i32 level [0x3C], char name[64] [0x40], u8 end of list [0x80]."""
    if len(body) < CHANNEL_ROW_LENGTH:
        raise DecodeError(f"ChannelList row: {len(body)} bytes, need {CHANNEL_ROW_LENGTH}")
    players, max_players, games = struct.unpack_from("<HHH", body, 0x20)
    return ChannelRow(
        status_of(body, "ChannelList row"),
        struct.unpack_from("<i", body, 0x1C)[0],
        players,
        max_players,
        games,
        text_of(body[0x40:0x80]),
        body[0x80] != 0,
    )


@dataclass
class GameRow:
    """One GetGames_ExtraInfo reply row."""

    status: int
    world_id: int
    players: int
    min_players: int
    max_players: int
    level: int
    skill: int
    rules: int
    generic: Tuple[int, ...]
    world_status: int
    host_type: int
    name: str
    end_of_list: bool


def decode_game_row(body: bytes) -> GameRow:
    """Layout: status [0x18], i32 world id [0x1C], u16 players [0x20], u16 min [0x22], u16 max [0x24], i32 level [0x28],
    i32 skill [0x2C], i32 rules [0x30], i32 generic field 1..8 [0x34], i32 security [0x54], i32 world status [0x58], i32
    host type [0x5C], char name[64] [0x60], char stats[256] [0xA0], u8 end of list [0x1A0]."""
    if len(body) < GAME_ROW_LENGTH:
        raise DecodeError(f"GameList row: {len(body)} bytes, need {GAME_ROW_LENGTH}")
    players, minimum, maximum = struct.unpack_from("<HHH", body, 0x20)
    level, skill, rules = struct.unpack_from("<iii", body, 0x28)
    return GameRow(
        status_of(body, "GameList row"),
        struct.unpack_from("<i", body, 0x1C)[0],
        players,
        minimum,
        maximum,
        level,
        skill,
        rules,
        struct.unpack_from("<8i", body, 0x34),
        struct.unpack_from("<i", body, 0x58)[0],
        struct.unpack_from("<i", body, 0x5C)[0],
        text_of(body[0x60:0xA0]),
        body[0x1A0] != 0,
    )


@dataclass
class GameInfoReply:
    status: int
    app_id: int
    min_players: int
    max_players: int
    players: int
    name: str
    world_status: int
    host_type: int


def decode_game_info(body: bytes) -> GameInfoReply:
    """GameInfo reply: status [0x18], i32 app id [0x1C], i32 min [0x20], i32 max [0x24], i32 level [0x28], i32 skill
    [0x2C], i32 player count [0x30], char stats[256] [0x34], char name[64] [0x134], i32 rules [0x174], generic
    field 1..8 [0x178], i32 world status [0x198], i32 host type [0x19C]. A failed lookup is a status-only body (the
    fields are 0 and '')."""
    status = status_of(body, "GameInfo")
    if len(body) < GAME_INFO_LENGTH:
        return GameInfoReply(status, 0, 0, 0, 0, "", 0, 0)
    app_id, minimum, maximum = struct.unpack_from("<iii", body, 0x1C)
    return GameInfoReply(
        status,
        app_id,
        minimum,
        maximum,
        struct.unpack_from("<i", body, 0x30)[0],
        text_of(body[0x134:0x174]),
        struct.unpack_from("<i", body, 0x198)[0],
        struct.unpack_from("<i", body, 0x19C)[0],
    )


@dataclass
class CreateGameReply:
    status: int
    world_id: Optional[int]


def decode_create_game_reply(body: bytes) -> CreateGameReply:
    """CreateGameResponse: status [0x18], i32 world id [0x1C] (absent from a failed reply)."""
    status = status_of(body, medius.CREATE_GAME_RESPONSE.name)
    return CreateGameReply(status, struct.unpack_from("<i", body, 0x1C)[0] if len(body) >= 0x20 else None)


@dataclass
class JoinGameReply:
    status: int
    host_type: Optional[int]
    connection: Optional[ConnectionInfo]


def decode_join_game_reply(body: bytes) -> JoinGameReply:
    """JoinGameResponse: status [0x18], i32 host type [0x1C], NetConnectionInfo [0x20] (0x9C bytes; the DME address, the
    world id the DME knows the game by, and the session and access keys the DME connect carries)."""
    status = status_of(body, medius.JOIN_GAME_RESPONSE.name)
    host_type = struct.unpack_from("<i", body, 0x1C)[0] if len(body) >= 0x20 else None
    connection = None
    if len(body) >= 0x20 + medius.CONNECTION_INFO_LENGTH:
        connection = decode_connection_info(body[0x20 : 0x20 + medius.CONNECTION_INFO_LENGTH])
    return JoinGameReply(status, host_type, connection)


# ----- the lobby connection


class LobbyClient:
    """Lobby requests on an MLS connection that already completed its handshake. `session_key` is the one from the
    login."""

    def __init__(self, connection: Connection, session_key: str, app_id: int):
        self.connection, self.session_key, self.app_id = connection, session_key, app_id

    def _rows(self, request: medius.MessageType, body: bytes, reply: medius.MessageType, decode) -> list:
        """Send a list request and read one reply message per row up to the row flagged end of list."""
        self.connection.send_medius(request, body)
        rows = []
        while True:
            row = decode(self.connection.receive_medius(reply)[1])
            rows.append(row)
            if row.end_of_list:
                return rows

    def list_channels(self, page_size: int = 10) -> List[ChannelRow]:
        return self._rows(
            medius.CHANNEL_LIST,
            encode_channel_list("channels", 0, page_size),
            medius.CHANNEL_LIST_RESPONSE,
            decode_channel_row,
        )

    def join_channel(self, world_id: int) -> medius.StatusReply:
        body = self.connection.request(
            medius.JOIN_CHANNEL,
            encode_join_channel("channel", self.session_key, world_id),
            medius.JOIN_CHANNEL_RESPONSE,
        )
        return medius.decode_status_reply(body, medius.JOIN_CHANNEL_RESPONSE.name)

    def set_lobby_world_filter(self) -> medius.StatusReply:
        body = self.connection.request(
            medius.SET_LOBBY_WORLD_FILTER,
            encode_lobby_world_filter("worldfilter"),
            medius.SET_LOBBY_WORLD_FILTER_RESPONSE,
        )
        return medius.decode_status_reply(body, medius.SET_LOBBY_WORLD_FILTER_RESPONSE.name)

    def set_game_list_filter(self, field: int, mask: int, operator: int, baseline: int) -> medius.StatusReply:
        body = self.connection.request(
            medius.SET_GAME_LIST_FILTER,
            encode_game_list_filter("gamefilter", field, mask, operator, baseline),
            medius.SET_GAME_LIST_FILTER_RESPONSE,
        )
        return medius.decode_status_reply(body, medius.SET_GAME_LIST_FILTER_RESPONSE.name)

    def list_games(self, page_size: int = 10) -> List[GameRow]:
        """The first page of the game list: one row per game, or a single status-only row (NO_RESULT) when there is
        none."""
        return self._rows(
            medius.GAME_LIST, encode_channel_list("games", 0, page_size), medius.GAME_LIST_RESPONSE, decode_game_row
        )

    def game_info(self, world_id: int) -> GameInfoReply:
        body = self.connection.request(
            medius.GAME_INFO, encode_game_info("info", self.session_key, world_id), medius.GAME_INFO_RESPONSE
        )
        return decode_game_info(body)

    def create_game(self, game: GameParameters) -> CreateGameReply:
        body = self.connection.request(
            medius.CREATE_GAME,
            encode_create_game("create", self.session_key, self.app_id, game),
            medius.CREATE_GAME_RESPONSE,
        )
        return decode_create_game_reply(body)

    def join_game(self, world_id: int, host_type: int) -> JoinGameReply:
        body = self.connection.request(
            medius.JOIN_GAME,
            encode_join_game("join", self.session_key, world_id, host_type),
            medius.JOIN_GAME_RESPONSE,
        )
        return decode_join_game_reply(body)

    def post_world_report(self, world_id: int, player_count: int, game: GameParameters) -> None:
        self.connection.send_medius(medius.WORLD_REPORT, encode_world_report(world_id, player_count, game))

    def post_player_report(self, world_id: int) -> None:
        self.connection.send_medius(medius.PLAYER_REPORT, encode_player_report(self.session_key, world_id))

    def post_user_state(self, action: int) -> None:
        self.connection.send_medius(medius.UPDATE_USER_STATE, encode_update_user_state(self.session_key, action))

    def post_end_game_report(self, world_id: int) -> None:
        self.connection.send_medius(medius.END_GAME_REPORT, encode_end_game_report(self.session_key, world_id))
