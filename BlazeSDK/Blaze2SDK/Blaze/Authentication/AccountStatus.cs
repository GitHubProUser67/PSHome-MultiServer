namespace Blaze2SDK.Blaze.Authentication
{
    public enum AccountStatus : int
    {
        UNKNOWN = 0x0,
        ACTIVE = 0x1,
        BANNED = 0x2,
        CHILD_APPROVED = 0x3,
        CHILD_PENDING = 0x4,
        DEACTIVATED = 0x5,
        DELETED = 0x6,
        DISABLED = 0x7,
        PENDING = 0x8,
        TENTATIVE = 0x9,
        VOLATILE = 0xA,
    }
}
