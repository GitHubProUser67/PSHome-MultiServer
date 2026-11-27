"""A player in a game room: a logged-in client with a lobby connection, a DME connection once it has joined a game,
and the message sequences the game runs around them (enter the lobby, create or join a game, leave it, drop out).

Several players share one thread. `service` and `wait_until` read every player's sockets, answer the server's echo and
ping messages like the game does and post the periodic PlayerReport, so a test only reads what each player received.
"""

import logging
import select
import time
from dataclasses import dataclass, field
from typing import Callable, List, Optional, Sequence

from . import medius, rt
from .dme import DmeClient
from .errors import ClientError, LoginFailedError, ReplyTimeoutError
from .lobby import (
    USER_JOINED_CHAT_WORLD,
    USER_LEFT_GAME_WORLD,
    GameInfoReply,
    GameParameters,
    GameRow,
    JoinGameReply,
    LobbyClient,
)
from .login import LoggedIn

log = logging.getLogger(__name__)

PLAYER_REPORT_INTERVAL = 25.0  # seconds between a client's PlayerReports; the server drops a client silent for 30 s
SELECT_SLICE = 0.05  # the longest a wait blocks in select before it re-checks its condition
CREATE_GAME_DEADLINE = 15.0  # CreateGame is answered "timed out" until the DME has registered with the lobby server
CREATE_GAME_RETRY = 0.5
PUBLISH_INTERVAL = 0.25
MIN_PLAYERS_FOR_NO_END_REPORT = 2  # the game posts EndGameReport on leaving when its game record counts fewer players


@dataclass
class Player:
    """One client: its login, lobby connection, the game it joined and the DME connection to it."""

    online_id: str
    logged_in: LoggedIn
    lobby: LobbyClient
    game: Optional[GameParameters] = None
    world_id: Optional[int] = None  # the lobby's game world id
    joined: Optional[JoinGameReply] = None
    dme: Optional[DmeClient] = None
    record_players: int = 0  # PlayerCount of the game record: 0 for a game it created, else what GameInfo reported
    lobby_frames: List[rt.Frame] = field(default_factory=list)  # frames the lobby server sent unasked
    _last_player_report: float = 0.0

    @property
    def name(self) -> str:
        return self.online_id

    # ----- lobby

    def enter_lobby(self, filters: Sequence[tuple]) -> int:
        """What the game does after the login: player info, account stats, NP id, server time, locations, then the
        channel list and JoinChannel on the first channel, the lobby filter and the game list filters (`filters`:
        tuples of field, mask, operator, baseline). Returns the world id of the channel joined."""
        mls = self.logged_in.mls
        _require(mls.get_player_info(self.logged_in.account.account_id).status, "PlayerInfo")
        _require(mls.update_account_stats().status, "AccountUpdateStats")
        _require(mls.post_np_id(self.online_id).status, "NpIdPost")
        _require(mls.get_server_time().status, "GetServerTime")
        mls.get_locations()
        channels = [row for row in self.lobby.list_channels() if row.status == medius.Status.SUCCESS]
        if not channels:
            raise LoginFailedError("the lobby lists no channel")
        _require(self.lobby.join_channel(channels[0].world_id).status, "JoinChannel")
        _require(self.lobby.set_lobby_world_filter().status, "SetLobbyWorldFilter")
        for game_filter in filters:
            _require(self.lobby.set_game_list_filter(*game_filter).status, "SetGameListFilter")
        return channels[0].world_id

    def create_game(self, game: GameParameters) -> int:
        """CreateGame, repeated every CREATE_GAME_RETRY seconds while the server answers 'timed out' (its DME is still
        registering), for at most CREATE_GAME_DEADLINE seconds. Returns the world id."""
        deadline = time.monotonic() + CREATE_GAME_DEADLINE
        while True:
            reply = self.lobby.create_game(game)
            if reply.status == medius.Status.SUCCESS:
                self.game, self.world_id, self.record_players = game, reply.world_id, 0
                return reply.world_id
            if reply.status != medius.Status.TRANSACTION_TIMED_OUT or time.monotonic() > deadline:
                raise LoginFailedError(f"CreateGame answered status {reply.status}")
            time.sleep(CREATE_GAME_RETRY)  # the named, bounded poll for the DME to register

    def publish_game(self, name: str, deadline: float) -> None:
        """Host: post the WorldReport (player count 1) every PUBLISH_INTERVAL seconds until the game appears in the
        list, at most `deadline` seconds. The game posts it every 28 s; the server lists a game only after a report
        that arrives once its host is counted in the DME, which is a moment after CONNECT_COMPLETE, so the first report
        can be too early."""
        end = time.monotonic() + deadline
        while True:
            self.post_world_report(1)
            if any(row.name == name for row in self.lobby.list_games()):
                return
            if time.monotonic() > end:
                raise ReplyTimeoutError(
                    f"{self.name}: game {name!r} not listed within {deadline:.0f} s of a WorldReport"
                )
            service([self], PUBLISH_INTERVAL)  # the named, bounded poll for the host to be counted

    def find_game(self, name: str, deadline: float) -> GameRow:
        """The row for the game called `name` in the game list, polling the list (every 0.25 s, at most `deadline`
        seconds): the server lists a game only after its host's first WorldReport."""
        end = time.monotonic() + deadline
        while True:
            for row in self.lobby.list_games():
                if row.status == medius.Status.SUCCESS and row.name == name:
                    return row
            if time.monotonic() > end:
                raise ReplyTimeoutError(f"{self.name}: game {name!r} not listed within {deadline:.0f} s")
            time.sleep(0.25)  # the named, bounded poll for the host's WorldReport

    def join_game(self, world_id: int, host_type: int, game: Optional[GameParameters] = None) -> JoinGameReply:
        """GameInfo for the world (the game's record keeps its player count), then JoinGame."""
        info: GameInfoReply = self.lobby.game_info(world_id)
        _require(info.status, "GameInfo")
        self.record_players = info.players
        reply = self.lobby.join_game(world_id, host_type)
        _require(reply.status, "JoinGame")
        self.world_id, self.joined, self.game = world_id, reply, game or self.game
        self._last_player_report = time.monotonic()
        return reply

    def post_world_report(self, player_count: int) -> None:
        """The host's WorldReport (the game posts it every 28 s while it hosts)."""
        self.lobby.post_world_report(self.world_id, player_count, self.game)

    # ----- DME

    def connect_dme(self, version: int, start_option: int, recv_flags: int) -> int:
        """Connect to the DME the JoinGame reply names. Returns this player's DME index."""
        info = self.joined.connection
        address = info.addresses[0]
        self.dme = DmeClient(address.ip, address.port, f"dme-{self.online_id}")
        return self.dme.connect(
            version,
            info.world_id & 0xFFFF,
            self.lobby.app_id,
            info.session_key,
            info.access_key,
            start_option,
            recv_flags,
        )

    # ----- running

    def sockets(self) -> list:
        mls = self.logged_in.mls.connection
        out = [mls] if mls.is_open and not mls.peer_closed else []
        if self.dme is not None:
            out.extend(self.dme.sockets())
        return out

    def service(self) -> None:
        """Read what has arrived on every connection (never blocks) and post the PlayerReport when it is due."""
        connection = self.logged_in.mls.connection
        if connection.is_open and not connection.peer_closed:
            for frame in connection.read_frames(0):
                if frame.id == rt.SERVER_ECHO:
                    connection.send(rt.CLIENT_ECHO, frame.payload)
                elif frame.id != rt.CLIENT_ECHO:  # a CLIENT_ECHO is the server's reply to ours
                    self.lobby_frames.append(frame)
        if self.dme is not None:
            self.dme.service()
        due = time.monotonic() - self._last_player_report >= PLAYER_REPORT_INTERVAL
        if self.dme is not None and connection.is_open and not connection.peer_closed and self.joined and due:
            self.lobby.post_player_report(self.world_id)
            self._last_player_report = time.monotonic()

    # ----- leaving

    def leave(self) -> None:
        """What the game does when the player leaves a game: the DME disconnect, then UpdateUserState 2 and 1, then an
        EndGameReport when the game record counts fewer than two players (host or not)."""
        if self.dme is not None:
            self.dme.leave()
        self.lobby.post_user_state(USER_LEFT_GAME_WORLD)
        self.lobby.post_user_state(USER_JOINED_CHAT_WORLD)
        if self.record_players < MIN_PLAYERS_FOR_NO_END_REPORT:
            self.lobby.post_end_game_report(self.world_id)

    def drop(self) -> None:
        """Vanish without a goodbye: every socket is closed and nothing is sent."""
        if self.dme is not None:
            self.dme.close()
        self.logged_in.mls.connection.close()
        self.logged_in.mas.connection.close()

    def close(self) -> None:
        """Teardown: close every socket, ignoring errors (the server may already have closed them)."""
        if self.dme is not None:
            self.dme.close()
        try:
            self.logged_in.close()
        except (OSError, ClientError):
            log.debug("%s: close failed", self.name, exc_info=True)


def _require(status: int, what: str) -> None:
    if status != medius.Status.SUCCESS:
        raise LoginFailedError(f"{what} answered status {status}")


def service(players: Sequence[Player], timeout: float) -> None:
    """Wait up to `timeout` seconds for data on any player's sockets, then service every player once."""
    sockets = [sock for player in players for sock in player.sockets()]
    if sockets:
        select.select(sockets, [], [], timeout)
    elif timeout > 0:
        time.sleep(timeout)
    for player in players:
        player.service()


def wait_until(players: Sequence[Player], condition: Callable[[], bool], deadline: float, what: str) -> float:
    """Service the players until `condition()` is true; the seconds it took. This is the suite's wait on server-side
    events (a notice another player's departure causes): it polls in SELECT_SLICE steps and fails after `deadline`
    seconds with a message naming `what`."""
    start = time.monotonic()
    while True:
        service(players, 0)
        if condition():
            return time.monotonic() - start
        if time.monotonic() - start > deadline:
            raise ReplyTimeoutError(f"{what} did not happen within {deadline:.1f} s")
        service(players, SELECT_SLICE)
