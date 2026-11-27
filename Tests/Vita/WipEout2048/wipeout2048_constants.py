"""WipEout 2048 (PS Vita): every title value the shared pshclient library takes as a parameter, and the title's account
naming rule. The title is in the module name on purpose: `Vita/WipEout2048` is on pytest's `pythonpath`, so its modules
are top-level names and a generic one (`constants`) would collide with a second title's."""

from typing import Tuple

TITLE_ID = "PCSA00015"  # NP ticket service id is UP9000-<TITLE_ID>_00
APP_ID = 23360  # Medius application id (0x5B40)
MEDIUS_VERSION = 113  # CLIENT_HELLO version word; SERVER_HELLO must report at least this
WORLD_ID = 1  # world id of the MUIS and account server connects
# the account server's ticket login takes version 2.x tickets and drops the connection on 4.0
LOGIN_TICKET_VERSION = "2.1"


def account_credentials(online_id: str) -> Tuple[str, str]:
    """The account name and password a PS Vita client derives from its online id: '<id>+Vita' and '<id>'."""
    return online_id + "+Vita", online_id


# The DME connect's READY message: [start option, receive flags]. The game sends 0x0E and 0x05 (broadcast and single
# messages); the server adds the list and notification flags the game relies on without ever asking for them.
DME_START_OPTION = 0x0E
DME_RECV_FLAGS = 0x05

# A game as the game's multiplayer lobby creates it (online game, host type 5 = Medius client-server with aux UDP).
GAME_MIN_PLAYERS = 1
GAME_MAX_PLAYERS = 8
GAME_LEVEL = 3  # track id
GAME_SKILL = 2  # speed class
GAME_RULES = 0x10  # game type
GAME_GENERIC_FIELDS = (1, 0, 0, 0, 0, 0, 0, 0)  # field 1 = weapons on
GAME_HOST_TYPE = 5
# SetGameListFilter the game sends before listing: (field, mask, operator, baseline). The values are an assumption: the
# game's own were not read.
GAME_LIST_FILTERS = ((6, 0xFFFFFFFF, 2, 0x10),)
GAME_NAME_PREFIX = (
    "W2048 room test"  # the game shows the text before the last '~'; a suffix after it makes the name unique
)
ROOM_SIZE = 4  # players needed for a campaign race
