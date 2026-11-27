"""The login path of a Medius client: MUIS -> MAS (account server) -> MLS (lobby server)."""

import logging
from dataclasses import dataclass
from typing import Callable, Optional, Tuple

from . import medius
from .connection import Connection, Handshake
from .errors import LoginFailedError
from .medius import (
    AccountLoginReply,
    ConnectionInfo,
    ServerTimeReply,
    SessionBeginReply,
    StatusReply,
    decode_account_login_reply,
    decode_server_time_reply,
    decode_session_begin_reply,
    decode_status_reply,
)
from .muis import MuisClient

log = logging.getLogger(__name__)


@dataclass
class TitleParameters:
    """What a title sends to every Medius server: application id, Medius version, world id of the first connects."""

    app_id: int
    version: int
    world_id: int


class MasClient:
    """The account server (MAS): session setup and account login."""

    def __init__(self, host: str, port: int, title: TitleParameters):
        self.title = title
        self.connection = Connection(host, port, name="mas")
        self.session_key = ""
        self.handshake: Optional[Handshake] = None

    def connect(self) -> Handshake:
        self.connection.open()
        self.handshake = self.connection.handshake(self.title.app_id, self.title.version, self.title.world_id)
        return self.handshake

    def begin_session(self) -> SessionBeginReply:
        """SessionBegin; keeps the returned session key for the requests that follow."""
        body = self.connection.request(
            medius.SESSION_BEGIN, medius.encode_session_begin("begin"), medius.SESSION_BEGIN_RESPONSE
        )
        reply = decode_session_begin_reply(body)
        self.session_key = reply.session_key
        return reply

    def post_dnas_signature(self) -> None:
        """DnasSignaturePost with a zero console id; the server sends no reply."""
        self.connection.send_medius(medius.DNAS_SIGNATURE_POST, medius.encode_dnas_signature("dnas", self.session_key))

    def set_localization(self) -> StatusReply:
        body = self.connection.request(
            medius.SET_LOCALIZATION_PARAMS,
            medius.encode_localization("localization", self.session_key),
            medius.STATUS_RESPONSE,
        )
        return decode_status_reply(body, medius.STATUS_RESPONSE.name)

    def login(self, account_name: str, password: str) -> AccountLoginReply:
        body = self.connection.request(
            medius.ACCOUNT_LOGIN,
            medius.encode_account_login("login", self.session_key, account_name, password),
            medius.ACCOUNT_LOGIN_RESPONSE,
        )
        return decode_account_login_reply(body)

    def register(self, account_name: str, password: str) -> StatusReply:
        body = self.connection.request(
            medius.ACCOUNT_REGISTRATION,
            medius.encode_account_registration("register", self.session_key, account_name, password),
            medius.ACCOUNT_REGISTRATION_RESPONSE,
        )
        return decode_status_reply(body, medius.ACCOUNT_REGISTRATION_RESPONSE.name)

    def ticket_login(self, ticket: bytes) -> StatusReply:
        """Log in with an NP ticket instead of a name and password. Returns the ticket-login status; the account data
        that follows it in the reply is not decoded."""
        body = self.connection.request(
            medius.TICKET_LOGIN,
            medius.encode_ticket_login("ticket", self.session_key, ticket),
            medius.TICKET_LOGIN_RESPONSE,
        )
        return decode_status_reply(body, medius.TICKET_LOGIN_RESPONSE.name)

    def close(self) -> None:
        if self.connection.is_open:
            _leave(self.connection)
        self.connection.close()


class MlsClient:
    """The lobby server (MLS), reached with the connection info from the account login."""

    def __init__(self, connection_info: ConnectionInfo, title: TitleParameters, session_key: str):
        address = connection_info.addresses[0]
        self.title, self.info, self.session_key = title, connection_info, session_key
        self.connection = Connection(address.ip, address.port, name="mls")
        self.handshake: Optional[Handshake] = None

    def connect(self) -> Handshake:
        """Connect; the session and access keys from the account login go after the connect message as 17 bytes each."""
        trailer = medius.fixed(self.info.session_key, 17) + medius.fixed(self.info.access_key, 17)
        self.connection.open()
        self.handshake = self.connection.handshake(self.title.app_id, self.title.version, self.info.world_id, trailer)
        return self.handshake

    def get_player_info(self, account_id: int) -> StatusReply:
        body = self.connection.request(
            medius.PLAYER_INFO,
            medius.encode_player_info("player", self.session_key, account_id),
            medius.PLAYER_INFO_RESPONSE,
        )
        return decode_status_reply(body, medius.PLAYER_INFO_RESPONSE.name)

    def update_account_stats(self) -> StatusReply:
        """AccountUpdateStats with an all-zero stats block."""
        body = self.connection.request(
            medius.ACCOUNT_UPDATE_STATS,
            medius.encode_account_update_stats("stats", self.session_key),
            medius.ACCOUNT_UPDATE_STATS_RESPONSE,
        )
        return decode_status_reply(body, medius.ACCOUNT_UPDATE_STATS_RESPONSE.name)

    def post_np_id(self, online_id: str) -> StatusReply:
        body = self.connection.request(
            medius.NPID_POST, medius.encode_npid_post("npid", self.session_key, online_id), medius.NPID_POST_RESPONSE
        )
        return decode_status_reply(body, medius.NPID_POST_RESPONSE.name)

    def get_server_time(self) -> ServerTimeReply:
        body = self.connection.request(
            medius.GET_SERVER_TIME, medius.encode_get_server_time("time"), medius.GET_SERVER_TIME_RESPONSE
        )
        return decode_server_time_reply(body)

    def get_locations(self) -> StatusReply:
        body = self.connection.request(
            medius.GET_LOCATIONS,
            medius.encode_get_locations("locations", self.session_key),
            medius.GET_LOCATIONS_RESPONSE,
        )
        return decode_status_reply(body, medius.GET_LOCATIONS_RESPONSE.name)

    def end_session(self) -> StatusReply:
        body = self.connection.request(
            medius.SESSION_END, medius.encode_session_end("end", self.session_key), medius.SESSION_END_RESPONSE
        )
        return decode_status_reply(body, medius.SESSION_END_RESPONSE.name)

    def close(self) -> None:
        if self.connection.is_open:
            _leave(self.connection)
        self.connection.close()


def _leave(connection: Connection) -> None:
    try:
        connection.disconnect()
    except OSError:
        log.debug("%s: disconnect not delivered", connection.name)


@dataclass
class LoggedIn:
    """A client that completed the login path and is connected to the lobby server."""

    online_id: str
    account: AccountLoginReply
    session_key: str
    mas: MasClient
    mls: MlsClient

    def close(self) -> None:
        self.mls.close()
        self.mas.close()


def connect_mas(muis_host: str, muis_port: int, title: TitleParameters) -> MasClient:
    """Ask the MUIS where the account server is and connect to it (no session yet)."""
    with MuisClient(muis_host, muis_port, title.app_id, title.version, title.world_id) as muis:
        muis.connect()
        universe = muis.get_universe()
    if not universe.dns or not universe.port:
        raise LoginFailedError(
            f"the MUIS universe entry names no account server (dns {universe.dns!r}, port {universe.port!r})"
        )
    mas = MasClient(universe.dns, universe.port, title)
    mas.connect()
    return mas


def start_session(mas: MasClient) -> None:
    """SessionBegin, DnasSignaturePost and SetLocalizationParams, the steps a client takes before it logs in."""
    reply = mas.begin_session()
    if reply.status != medius.Status.SUCCESS:
        raise LoginFailedError(f"SessionBegin answered status {reply.status}")
    mas.post_dnas_signature()
    status = mas.set_localization().status
    if status != medius.Status.SUCCESS:
        raise LoginFailedError(f"SetLocalizationParams answered status {status}")


Credentials = Callable[[str], Tuple[str, str]]  # online id -> (account name, password); a title defines the rule


def log_in(
    muis_host: str,
    muis_port: int,
    title: TitleParameters,
    online_id: str,
    credentials: Credentials,
    close_mas_first: bool = False,
) -> LoggedIn:
    """The whole path of a client that starts the game: MUIS, MAS (registering the account if the server does not know
    it), then MLS. `credentials` maps the online id to the account name and password the title's client sends.
    `close_mas_first` makes the client disconnect from the account server before it connects to the lobby server,
    which is what the game does; by default the account connection stays open."""
    mas = connect_mas(muis_host, muis_port, title)
    try:
        start_session(mas)
        name, password = credentials(online_id)
        account = mas.login(name, password)
        if account.status == medius.Status.ACCOUNT_NOT_FOUND:
            mas.register(name, password)
            account = mas.login(name, password)
        if account.status != medius.Status.SUCCESS or account.connection is None:
            raise LoginFailedError(f"AccountLogin for {name} answered status {account.status}")
        if close_mas_first:
            mas.close()
        mls = MlsClient(account.connection, title, mas.session_key)
        mls.connect()
    except BaseException:
        mas.close()
        raise
    return LoggedIn(online_id, account, mas.session_key, mas, mls)
