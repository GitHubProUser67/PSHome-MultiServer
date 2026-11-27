"""The universe information server (MUIS): the first server a client talks to, and the one that says where to log in.

Observed values (Horizon, verified by running): SERVER_HELLO version 113, suite 0x600 with a 743-byte certificate;
connect requirements flags 0 and values (0x248, 0x248); connect accept player id 0, one address, 127.0.0.1;
connect complete value 1."""

# SERVER_HELLO: u16 version, u16 suite, u16 certificate length, then the certificate (see Connection hello decoder).
HELLO_FIXED_LENGTH = 6
# The client rejects any value above this in the connect requirements.
CLIENT_MAX_REQUIRE_VALUE = 0x643
# Connect accept: u16 player id, u32 server certificate, u16 address count, char ip[16].
ACCEPT_MIN_LENGTH = 0x18
EXPECTED_REQUIRE_VALUES = (0x248, 0x248)
CONNECT_COMPLETE_VALUE = 1


def test_server_hello_version_is_at_least_the_clients(muis, title):
    """A client refuses a server that speaks an older protocol version than its own."""
    assert muis.handshake.hello.version >= title.version


def test_server_hello_certificate_fits_in_the_message(muis):
    """The client reads the certificate length from the hello and then that many bytes, so they must be present."""
    hello = muis.handshake.hello
    if hello.certificate_length is not None:
        assert HELLO_FIXED_LENGTH + hello.certificate_length <= hello.payload_length


def test_connect_require_values_are_within_the_clients_limit(muis):
    """The client rejects any value above 0x643 in the connect requirements."""
    require = muis.handshake.require
    assert require.flags == 0
    assert require.values == EXPECTED_REQUIRE_VALUES
    assert all(value <= CLIENT_MAX_REQUIRE_VALUE for value in require.values)


def test_connect_accept_carries_a_player_id_and_address(muis):
    """The client needs the player id, the server certificate word and the address block (at least 0x18 bytes)."""
    accept = muis.handshake.accept
    assert accept.payload_length >= ACCEPT_MIN_LENGTH
    assert accept.address_count == 1
    assert accept.ip == "127.0.0.1"


def test_handshake_ends_with_connect_complete(muis):
    assert muis.handshake.complete.value == CONNECT_COMPLETE_VALUE


def test_universe_reply_is_a_success(universe):
    assert universe.status == 0


def test_universe_entry_is_named(universe):
    assert universe.name


def test_universe_entry_points_at_the_account_server(universe, horizon):
    """The client connects to the address in the universe entry; a login can only work if it is the account server."""
    assert universe.dns == horizon.host
    assert universe.port == horizon.mas_port


def test_universe_list_ends_with_the_entry(universe):
    """The client keeps waiting for more entries until one carries the end-of-list flag."""
    assert universe.end_of_list is True


def test_echo_is_returned_unchanged(muis):
    assert muis.echo(b"\xa5") == b"\xa5"
