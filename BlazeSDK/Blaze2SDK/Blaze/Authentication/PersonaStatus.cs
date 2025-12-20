namespace Blaze2SDK.Blaze.Authentication
{
    public enum PersonaStatus : int
    {
        UNKNOWN = 0x0,
        PENDING = 0x1,
        ACTIVE = 0x2,
        DEACTIVATED = 0x3,
        DISABLED = 0x4,
        DELETED = 0x5,
        BANNED = 0x6,
    }
}
