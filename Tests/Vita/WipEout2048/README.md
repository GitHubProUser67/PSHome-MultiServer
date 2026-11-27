# WipEout 2048 tests

The WipEout 2048 module (`wipeout2048_constants`) holds the title's values and its account naming rule, on top of the
shared protocol client `pshclient` (standard library only, Python 3.8+). WipEout 2048 for PS Vita is title id PCSA00015,
app id 23360. The live integration tests in this directory drive the server through them. The message codecs follow the server's own
models in `AuxiliaryServices/HorizonService`.

## Layout

- `wipeout2048_constants.py`: every title value the shared client takes as a parameter (title id, application id,
  Medius version, world id, the ticket version the account server accepts) and `account_credentials` (the
  `<id>+Vita` / `<id>` rule). The title is in the module name on purpose: this directory is on pytest's `pythonpath`, so
  its modules are top-level names and a generic `constants.py` would collide with another title's.
- `conftest.py`: the title's fixtures: `title`, `online_id` (a new id per test), `ticket`, `muis`, `universe`, `mas`,
  `mas_session`, `logged_in`, `make_player` (a factory of independent players, each logged in like the game does and
  ready to list games; every socket is closed when the test ends) and `room` (four players in one game). The server
  fixture `horizon` is in `Tests/conftest.py`.
- Test files (`test_*.py`) may share a basename with another title's (`test_login.py`): pytest runs in
  `--import-mode=importlib` (`pyproject.toml`), which does not need unique basenames.
- `test_muis.py`: the MUIS handshake and universe entry. `test_login.py`: session, account login, lobby server, tickets. The two ticket-login tests carry the
  `legacy_path` marker: they exercise MediusTicketLogin, which the shipped game does not use (it logs in with name and
  password); deselect them with `-m "not legacy_path"`.

- `test_game_room.py`: four players in one game (see below).

## The game room tests

Each player (`pshclient.room.Player`) sends what the game sends, in the game's order:

1. MUIS, then MAS: SessionBegin, DnasSignaturePost, SetLocalizationParams, AccountLogin (AccountRegistration first for a new
   name); the account server connection is closed before the lobby server is connected, as the game does.
2. MLS: PlayerInfo, AccountUpdateStats, NpIdPost, GetServerTime, GetLocations, ChannelList_ExtraInfo, JoinChannel,
   SetLobbyWorldFilter, SetGameListFilter.
3. The host: CreateGame_ExtraInfo (host type 5), JoinGame on the new world, the DME connect, WorldReport. Each guest:
   GetGames_ExtraInfo until the game is listed, GameInfo_ExtraInfo, JoinGame, the DME connect.
4. DME connect: CLIENT_HELLO, CLIENT_CONNECT_TCP_AUX_UDP (session and access key of the JoinGame reply), READY_REQUIRE,
   READY_TCP with `[0x0E, 0x05]` (the game's values; the server adds the list and notification flags the game depends on),
   CLIENT_CONNECT_AUX_UDP over UDP, READY_AUX_UDP, CONNECT_COMPLETE. A player counts as joined only after the UDP leg.
5. Peers: each player keeps a peer set. SERVER_CONNECT_NOTIFY (only for later joiners) and the sender index of a relayed
   application message add a peer, SERVER_DISCONNECT_NOTIFY removes it. When a notice adds a peer, the player sends its
   ClientInfo (0x4c bytes, the name first, the rest zero: both assumed) as APP_LIST to its whole peer set; the first
   ClientInfo of a new peer is answered at once (assumed; the 10 s resend is not simulated).
6. Leaving: DME disconnect, UpdateUserState 2 then 1, then EndGameReport when the game record counts fewer than two players
   (a created game's record counts 0; a joiner's is the count GameInfo reported when it joined).
7. While idle, a player answers the server's echo and the DME ping and posts PlayerReport every 25 s, as the game does.

Not sent, on purpose: SET_RECV_FLAG; MatchFindGame (the game's request layout was never read). Unverified values: the
SetLobbyWorldFilter and SetGameListFilter bodies (the layout is the server model's; the game's own values were not read), and
the WorldReport player count (the server ignores it and counts the players in the DME itself).

The server updates a game's player count in the lobby only when the host posts a WorldReport (the game posts one every 28 s);
the tests post one right after the event they check instead of waiting for the next period. This is the one place where
timing is compressed; the message is the one the game sends.

## Tickets

The client sends unsigned NP tickets of version 4.0 (the PS Vita layout; 2.1 is selectable). Upstream does not verify 4.0
signatures (`XI5Ticket.cs`: "TODO, figuring out the 4.0 hash algorithm") and compiles out the 2.x PSN verification
(`DISABLE_PSN_XI5_VERIFICATION` in `BackendServices/CastleLibrary/CastleLibrary.csproj`); timestamps and the service id
are checked. These tickets are for tests only.

## Running

From `Tests/`, with the venv from `Tests/README.md` active:

    pytest Vita/WipEout2048                      # starts the built Horizon (ports must be free), about 15 s
    pytest -v --log-cli-level=DEBUG Vita/WipEout2048   # one DEBUG line per message sent and received
    PSH_TEST_HORIZON_HOST=127.0.0.1 pytest Vita/WipEout2048   # use a Horizon that is already running

Ticket logins use version 2.1 tickets: the account server rejects 4.0 in that message. The 4.0 tickets (the PS Vita
layout) are for SVO tests, which do not exist yet.

The tests do not build the server; build it first (see `Tests/README.md`, "Server for the tests").
