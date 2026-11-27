"""Four players in one game: they join it, see each other, and the others are told when one leaves or vanishes.

Every player is a client that sends what the game sends and in its order (see the README): login, lobby steps,
CreateGame or GameInfo and JoinGame, the DME connect, and on leaving the DME disconnect, UpdateUserState 2 and 1 and
(when the game's record counts fewer than two players) EndGameReport. Waits are bounded polls on what the server sends
(`wait_until`).
"""

import logging
import time
from typing import List

from pshclient import medius, rt
from pshclient.dme import CLIENT_INFO_LENGTH
from pshclient.room import Player, wait_until

log = logging.getLogger(__name__)

NOTICE_DEADLINE = 5.0  # seconds for the server to tell the others about a player who left cleanly
DROP_DEADLINE = 20.0  # seconds for the server to tell the others about a player whose sockets vanished
TEARDOWN_DEADLINE = 10.0  # seconds for the server to close the DME connections of a game it ended
LOBBY_COUNT_DEADLINE = 5.0  # seconds for a WorldReport to show in the lobby's player count
PEER_DEADLINE = 10.0  # seconds for the players' ClientInfo exchange to give every player its peers
HOST_INDEX = 0  # the first player to connect to a game's DME gets index 0


def joined_indexes(player: Player) -> List[int]:
    """DME indexes of the players the server announced to `player` as connected, in the order announced."""
    return [notice.index for notice in player.dme.notices(rt.SERVER_CONNECT_NOTIFY)]


def left_indexes(player: Player) -> List[int]:
    """DME indexes of the players the server announced to `player` as disconnected, in the order announced."""
    return [notice.index for notice in player.dme.notices(rt.SERVER_DISCONNECT_NOTIFY)]


def notice_time(player: Player, index: int) -> float:
    """When `player` received the disconnect notice naming DME `index`."""
    for item, notice in zip(
        (item for item in player.dme.received if item.frame_id == rt.SERVER_DISCONNECT_NOTIFY),
        player.dme.notices(rt.SERVER_DISCONNECT_NOTIFY),
    ):
        if notice.index == index:
            return item.time
    raise AssertionError(f"{player.name} received no disconnect notice for DME index {index}")


def lobby_player_count(room, reporter: Player, observer: Player, count: int) -> int:
    """The player count the lobby shows for the room's game after the host-side `reporter` posts a WorldReport: the
    server updates it only on a report (the game posts one every 28 s). Posts and asks every 0.25 s until it shows
    `count`, at most LOBBY_COUNT_DEADLINE seconds; returns the last count seen."""
    end = time.monotonic() + LOBBY_COUNT_DEADLINE
    while True:
        reporter.post_world_report(count)
        players = observer.lobby.game_info(room.world_id).players
        if players == count or time.monotonic() > end:
            return players
        time.sleep(0.25)  # the named, bounded poll for the server to process the report


def test_four_players_are_in_one_game(room):
    """Every player joined the same lobby game and holds a different DME index (0 for the host, then in join order)."""
    assert {player.world_id for player in room.players} == {room.world_id}
    assert {player.joined.connection.world_id for player in room.players} == {room.world_id}
    assert [player.dme.index for player in room.players] == [0, 1, 2, 3]


def test_the_lobby_counts_four_players(room):
    """The game's list row and its GameInfo show four players once the host has reported (the count is updated by the
    host's WorldReport only)."""
    assert lobby_player_count(room, room.host, room.guests[0], 4) == 4
    rows = [row for row in room.guests[0].lobby.list_games() if row.name == room.game.name]
    assert [(row.world_id, row.players) for row in rows] == [(room.world_id, 4)]


def test_server_announces_a_joiner_to_the_players_already_in_the_game(room):
    """A player that is already in the game gets SERVER_CONNECT_NOTIFY for each later player, in join order, and a
    joiner gets none for the players that were there before it: three notices for the host, then two, one and none.
    (That is how the server works; the players learn each other through their own messages, see the next test.)"""
    host, first, second, third = room.players
    wait_until(room.players, lambda: len(joined_indexes(first)) == 2, NOTICE_DEADLINE, "notices at the first guest")
    wait_until(room.players, lambda: len(joined_indexes(second)) == 1, NOTICE_DEADLINE, "notices at the second guest")

    assert joined_indexes(host) == [1, 2, 3]
    assert joined_indexes(first) == [2, 3]
    assert joined_indexes(second) == [3]
    assert joined_indexes(third) == []


def test_every_player_learns_all_peers(room):
    """Each player starts from what the server announced and the ClientInfo messages the others send (every player that
    is told about a joiner sends it its ClientInfo, and a player answers the first ClientInfo of a new peer). After
    the exchange each player's peer set is exactly the other three DME indexes. The last joiner, announced nobody,
    learned all three from the sender index of the application messages it received."""
    everyone = {player.dme.index for player in room.players}
    wait_until(
        room.players,
        lambda: all(set(player.dme.peers) == everyone - {player.dme.index} for player in room.players),
        PEER_DEADLINE,
        "every player knowing the other three",
    )

    for player in room.players:
        assert set(player.dme.peers) == everyone - {player.dme.index}
    last = room.players[-1]
    assert joined_indexes(last) == []
    assert {message.sender for message in last.dme.app_messages()} == everyone - {last.dme.index}
    assert {last.dme.peers[index] for index in last.dme.peers} == {"message"}


def test_every_player_receives_the_clientinfo_of_the_others(room):
    """After the exchange each player has received a ClientInfo-sized message from each of the other three players,
    naming the sender."""
    indexes = {player.dme.index: player for player in room.players}
    wait_until(
        room.players,
        lambda: all(
            {m.sender for m in player.dme.app_messages() if len(m.payload) == CLIENT_INFO_LENGTH}
            == set(indexes) - {player.dme.index}
            for player in room.players
        ),
        PEER_DEADLINE,
        "every player receiving a ClientInfo from the other three",
    )
    for player in room.players:
        for message in player.dme.app_messages():
            if len(message.payload) == CLIENT_INFO_LENGTH:
                assert message.payload.rstrip(b"\x00") == indexes[message.sender].dme.name.encode()[:16]


def test_a_guest_leaving_is_announced_to_the_other_three(room):
    """The game's leave sequence (DME disconnect, UpdateUserState 2 and 1, EndGameReport) of a guest: the other three
    get
    SERVER_DISCONNECT_NOTIFY naming it, within moments, and after the host's next report the lobby shows three
    players."""
    leaver = room.guests[1]
    stayers = [player for player in room.players if player is not leaver]
    left_at = time.monotonic()

    leaver.leave()
    wait_until(
        stayers,
        lambda: all(left_indexes(player) == [leaver.dme.index] for player in stayers),
        NOTICE_DEADLINE,
        "the disconnect notice reaching the other three",
    )

    latencies = [notice_time(player, leaver.dme.index) - left_at for player in stayers]
    log.info("clean leave: the others were notified after %s s", ", ".join(f"{value:.2f}" for value in latencies))
    assert max(latencies) < NOTICE_DEADLINE
    for player in stayers:
        notice = player.dme.notices(rt.SERVER_DISCONNECT_NOTIFY)[0]
        assert (notice.index, notice.scert_id, notice.ip) == (leaver.dme.index, leaver.dme.scert_id, "127.0.0.1")
        assert not player.dme.peer_closed
    assert lobby_player_count(room, room.host, room.guests[0], 3) == 3


def test_the_remaining_players_drop_a_guest_who_left_from_their_peers(room):
    """After the peer exchange a guest leaves: SERVER_DISCONNECT_NOTIFY removes it from every remaining player's peer
    set, which keeps the other two."""
    everyone = {player.dme.index for player in room.players}
    wait_until(
        room.players,
        lambda: all(set(player.dme.peers) == everyone - {player.dme.index} for player in room.players),
        PEER_DEADLINE,
        "every player knowing the other three",
    )
    leaver = room.guests[1]
    stayers = [player for player in room.players if player is not leaver]

    leaver.leave()
    remaining = everyone - {leaver.dme.index}
    wait_until(
        stayers,
        lambda: all(set(player.dme.peers) == remaining - {player.dme.index} for player in stayers),
        NOTICE_DEADLINE,
        "the remaining players dropping the guest from their peers",
    )


def test_a_vanished_guest_is_noticed(room):
    """A guest's connections are closed with no goodbye (the host's operating system closes a vanished process's
    sockets the same way). The server notices at once and tells the other three. A client that goes silent without
    closing its sockets is not simulated: that is detected by the server's echo timeout, not measured here."""
    vanished = room.guests[1]
    stayers = [player for player in room.players if player is not vanished]
    dropped_at = time.monotonic()

    vanished.drop()
    elapsed = wait_until(
        stayers,
        lambda: all(vanished.dme.index in left_indexes(player) for player in stayers),
        DROP_DEADLINE,
        "the others being told about the vanished guest",
    )

    log.info("abrupt drop: noticed by all three after %.2f s", elapsed)
    assert max(notice_time(player, vanished.dme.index) for player in stayers) - dropped_at < DROP_DEADLINE
    assert lobby_player_count(room, room.host, room.guests[0], 3) == 3


def test_the_host_leaving_ends_the_game_for_the_others(room):
    """The host's game record counts no players, so its leave sequence includes EndGameReport, and a host's report
    ends the game. The others are told about the host, then the server closes their DME connections and forgets the
    game. (The server
    also moves the DME session master to the lowest remaining index; no message announces that to a client.)"""
    host, guests = room.host, room.guests

    host.leave()
    wait_until(
        guests,
        lambda: all(HOST_INDEX in left_indexes(guest) for guest in guests),
        NOTICE_DEADLINE,
        "the disconnect notice for the host reaching the guests",
    )
    wait_until(
        guests, lambda: all(guest.dme.peer_closed for guest in guests), TEARDOWN_DEADLINE, "the server closing the DME"
    )

    assert guests[0].lobby.game_info(room.world_id).status == medius.Status.GAME_NOT_FOUND
    assert all(not guest.logged_in.mls.connection.peer_closed for guest in guests)  # the lobby connections stay up
