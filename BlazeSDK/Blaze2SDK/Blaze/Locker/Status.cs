namespace Blaze2SDK.Blaze.Locker
{
    public enum Status : int
    {
        ACTIVE = 0x0,
        PENDING = 0x1,
        DELETED = 0x2,
        SHAREABLE = 0x4,
        BANNED = 0x8,
    }
}
