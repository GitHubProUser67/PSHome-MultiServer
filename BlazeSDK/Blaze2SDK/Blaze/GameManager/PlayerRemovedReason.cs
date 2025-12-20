namespace Blaze2SDK.Blaze.GameManager
{
    public enum PlayerRemovedReason : int
    {
        PLAYER_JOIN_TIMEOUT = 0x0,
        PLAYER_CONN_LOST = 0x1,
        BLAZESERVER_CONN_LOST = 0x2,
        MIGRATION_FAILED = 0x3,
        GAME_DESTROYED = 0x4,
        GAME_ENDED = 0x5,
        PLAYER_LEFT = 0x6,
        GROUP_LEFT = 0x7,
        PLAYER_KICKED = 0x8,
        PLAYER_KICKED_WITH_BAN = 0x9,
        PLAYER_JOIN_FROM_QUEUE_FAILED = 0xA,
    }
}
