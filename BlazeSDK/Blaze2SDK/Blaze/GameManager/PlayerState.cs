namespace Blaze2SDK.Blaze.GameManager
{
    public enum PlayerState : int
    {
        RESERVED = 0x0,
        QUEUED = 0x1,
        ACTIVE_CONNECTING = 0x2,
        ACTIVE_MIGRATING = 0x3,
        ACTIVE_CONNECTED = 0x4,
        ACTIVE_KICK_PENDING = 0x5,
    }
}
