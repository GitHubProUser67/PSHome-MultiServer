"""Fixtures for the WipEout 2048 tests: title parameters, accounts, tickets and logged-in clients."""

import uuid
from dataclasses import dataclass
from typing import Callable, Iterator, List

import pytest
import wipeout2048_constants as title_constants
from pshclient import rt
from pshclient.lobby import GameParameters, LobbyClient
from pshclient.login import LoggedIn, MasClient, TitleParameters, connect_mas, log_in, start_session
from pshclient.muis import MuisClient, UniverseInfo
from pshclient.room import Player, wait_until
from pshclient.ticket import build_ticket


@pytest.fixture(scope="session")
def title() -> TitleParameters:
    return TitleParameters(title_constants.APP_ID, title_constants.MEDIUS_VERSION, title_constants.WORLD_ID)


@pytest.fixture
def online_id() -> str:
    """A new online id (3 to 16 characters) so that no test depends on accounts left by another run."""
    return "w2048" + uuid.uuid4().hex[:8]


@pytest.fixture
def ticket(online_id: str) -> bytes:
    """A valid NP ticket for `online_id`, issued an hour ago and valid for another hour. The account server only accepts
    version 2.x tickets in a ticket login (it drops the connection on a 4.0 ticket), so that is the version built."""
    return build_ticket(online_id, title_constants.TITLE_ID, version=title_constants.LOGIN_TICKET_VERSION)


@pytest.fixture
def muis(horizon, title: TitleParameters) -> Iterator[MuisClient]:
    """A MUIS client that has completed the connection handshake."""
    with MuisClient(horizon.host, horizon.muis_port, title.app_id, title.version, title.world_id) as client:
        client.connect()
        yield client


@pytest.fixture(scope="module")
def universe(horizon, title: TitleParameters) -> Iterator[UniverseInfo]:
    """The first universe entry the MUIS lists."""
    with MuisClient(horizon.host, horizon.muis_port, title.app_id, title.version, title.world_id) as client:
        client.connect()
        yield client.get_universe()


@pytest.fixture
def mas(horizon, title: TitleParameters) -> Iterator[MasClient]:
    """An account server connection with no session started."""
    client = connect_mas(horizon.host, horizon.muis_port, title)
    yield client
    client.close()


@pytest.fixture
def mas_session(mas: MasClient) -> MasClient:
    """An account server connection with a session begun and the localization set, ready for a login."""
    start_session(mas)
    return mas


@pytest.fixture
def logged_in(horizon, title: TitleParameters, online_id: str) -> Iterator[LoggedIn]:
    """A client that logged in (registering a new account) and is connected to the lobby server."""
    client = log_in(horizon.host, horizon.muis_port, title, online_id, title_constants.account_credentials)
    yield client
    client.close()


PlayerFactory = Callable[[int], List[Player]]


@pytest.fixture
def make_players(horizon, title: TitleParameters) -> Iterator[PlayerFactory]:
    """A factory of independent players: `make_players(n)` logs n new accounts in (each the way the game does:
    the account server connection is closed before the lobby server is connected) and runs each player's lobby steps up
    to the game list. Every socket of every player made is closed when the test ends, whether it passed or not."""
    made: List[Player] = []

    def make_one() -> Player:
        online_id = "w2048" + uuid.uuid4().hex[:8]
        logged_in = log_in(
            horizon.host, horizon.muis_port, title, online_id, title_constants.account_credentials, close_mas_first=True
        )
        player = Player(
            online_id, logged_in, LobbyClient(logged_in.mls.connection, logged_in.session_key, title.app_id)
        )
        made.append(player)  # before the lobby steps, so a failure in them still closes the sockets
        player.enter_lobby(title_constants.GAME_LIST_FILTERS)
        return player

    def make(count: int) -> List[Player]:
        # Deliberately one login after another, never in parallel. Four first logins of new accounts started at the same
        # moment on a freshly started server timed out in about 2 of 5 runs: the account server never answered one
        # AccountLogin (its log showed 3 of 4 "LOGGING IN", then "Destroying Client"). That is an open server issue,
        # not test noise, and these tests are about the game room, not login concurrency, so they avoid it. Logging in
        # serially costs about 12 s per room test. Real consoles rarely log in at the same instant, but nobody has
        # shown the server is safe when they do. A test of concurrent logins belongs in test_login.py, as an xfail
        # until the server issue is settled.
        return [make_one() for _ in range(count)]

    yield make
    for player in reversed(made):
        player.close()


SETTLE_DEADLINE = 10.0  # seconds for the server to finish counting every player into the game
LISTING_DEADLINE = 10.0  # seconds for a game to appear in the list after its host's WorldReport


@dataclass
class Room:
    """A game with four players in it, all connected to its DME. players[0] is the host (the player who created it)."""

    world_id: int
    game: GameParameters
    players: List[Player]

    @property
    def host(self) -> Player:
        return self.players[0]

    @property
    def guests(self) -> List[Player]:
        return self.players[1:]


@pytest.fixture
def room(make_players: PlayerFactory, title: TitleParameters) -> Room:
    """Four players in one game, in the order the game does it. The first creates the game, joins it, connects to its
    DME and posts its WorldReport; the others find the game in the list, read its GameInfo, join it and connect to the
    DME. On
    return the server has counted all four into the game (the host has been notified of the three that joined)."""
    players = make_players(title_constants.ROOM_SIZE)
    game = GameParameters(
        f"{title_constants.GAME_NAME_PREFIX}~{uuid.uuid4().hex[:4]}",
        title_constants.GAME_MIN_PLAYERS,
        title_constants.GAME_MAX_PLAYERS,
        title_constants.GAME_LEVEL,
        title_constants.GAME_SKILL,
        title_constants.GAME_RULES,
        title_constants.GAME_GENERIC_FIELDS,
        title_constants.GAME_HOST_TYPE,
    )
    host, guests = players[0], players[1:]
    world_id = host.create_game(game)
    host.join_game(world_id, game.host_type)
    host.connect_dme(title.version, title_constants.DME_START_OPTION, title_constants.DME_RECV_FLAGS)
    host.publish_game(game.name, LISTING_DEADLINE)
    for guest in guests:
        row = guest.find_game(game.name, LISTING_DEADLINE)
        guest.join_game(row.world_id, game.host_type, game)
        guest.connect_dme(title.version, title_constants.DME_START_OPTION, title_constants.DME_RECV_FLAGS)
    wait_until(
        players,
        lambda: len(host.dme.notices(rt.SERVER_CONNECT_NOTIFY)) == len(guests),
        SETTLE_DEADLINE,
        "the host being notified of every guest",
    )
    return Room(world_id, game, players)
