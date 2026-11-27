"""Logging in: the account server (MAS) creates a session and an account login, and hands the client to the lobby server
(MLS), where the client announces itself."""

import time

import pytest
import wipeout2048_constants as title_constants
from pshclient import medius
from pshclient.ticket import HOUR_MS, build_ticket

account_credentials = title_constants.account_credentials

# The server's GMT clock and the test host's agree to the second on one machine; a day is slack for a skewed host.
SERVER_TIME_TOLERANCE_SECONDS = 24 * 3600
# Connect complete value of every server's handshake (see test_muis.py).
CONNECT_COMPLETE_VALUE = 1


def test_session_begin_returns_a_session_key(mas):
    """Every later request carries the session key, so an empty one makes the rest of the login impossible."""
    reply = mas.begin_session()

    assert reply.status == medius.Status.SUCCESS
    assert reply.session_key


def test_account_server_handshake_completes(mas):
    assert mas.handshake.complete.value == CONNECT_COMPLETE_VALUE


def test_set_localization_succeeds(mas_session):
    assert mas_session.set_localization().status == medius.Status.SUCCESS


def test_registration_of_a_new_account_succeeds(mas_session, online_id):
    name, password = account_credentials(online_id)

    reply = mas_session.register(name, password)

    assert reply.status == medius.Status.SUCCESS


def test_login_of_a_registered_account_returns_the_lobby_server(mas_session, online_id, horizon):
    """The client copies the connection info from the login reply and connects to the address in it."""
    name, password = account_credentials(online_id)
    mas_session.register(name, password)

    reply = mas_session.login(name, password)

    assert reply.status == medius.Status.SUCCESS
    lobby = reply.connection.addresses[0]
    assert lobby.ip
    assert lobby.port == horizon.mls_port
    assert reply.connection.session_key
    assert reply.connection.access_key


def test_login_of_an_unknown_account_creates_it(mas_session, online_id):
    """The server registers an account it does not know on the first login, so a new player needs no extra step."""
    name, password = account_credentials(online_id)

    reply = mas_session.login(name, password)

    assert reply.status == medius.Status.SUCCESS
    assert reply.account_id > 0


def test_lobby_server_handshake_completes(logged_in):
    assert logged_in.mls.handshake.complete.value == CONNECT_COMPLETE_VALUE


def test_player_info_succeeds(logged_in):
    assert logged_in.mls.get_player_info(logged_in.account.account_id).status == medius.Status.SUCCESS


def test_account_stats_update_succeeds(logged_in):
    assert logged_in.mls.update_account_stats().status == medius.Status.SUCCESS


def test_np_id_post_succeeds(logged_in):
    assert logged_in.mls.post_np_id(logged_in.online_id).status == medius.Status.SUCCESS


def test_server_time_is_the_current_time(logged_in):
    reply = logged_in.mls.get_server_time()

    assert reply.status == medius.Status.SUCCESS
    assert abs(reply.gmt_seconds - time.time()) < SERVER_TIME_TOLERANCE_SECONDS


def test_get_locations_is_answered(logged_in):
    """A fresh server lists no locations: the reply is a status-only 'no result' (observed: status 1, 100-byte body)."""
    assert logged_in.mls.get_locations().status == medius.Status.NO_RESULT


def test_session_end_succeeds(logged_in):
    assert logged_in.mls.end_session().status == medius.Status.SUCCESS


@pytest.mark.legacy_path
def test_ticket_login_accepts_a_valid_ticket(mas_session, ticket):
    """Exercises MediusTicketLogin, which the shipped game does not use (it logs in with name and password); kept as
    coverage of the server's NP ticket path."""
    reply = mas_session.ticket_login(ticket)

    assert reply.status == medius.Status.SUCCESS


@pytest.mark.legacy_path
def test_expired_ticket_is_refused(mas_session, online_id):
    """A ticket whose validity window ended must not log anyone in. The server answers with a database error status.
    Exercises MediusTicketLogin, which the shipped game does not use; kept as server coverage."""
    now = int(time.time() * 1000)
    expired = build_ticket(
        online_id,
        title_constants.TITLE_ID,
        issued_ms=now - 3 * HOUR_MS,
        expires_ms=now - HOUR_MS,
        version=title_constants.LOGIN_TICKET_VERSION,
    )

    reply = mas_session.ticket_login(expired)

    assert reply.status == medius.Status.DB_ERROR
