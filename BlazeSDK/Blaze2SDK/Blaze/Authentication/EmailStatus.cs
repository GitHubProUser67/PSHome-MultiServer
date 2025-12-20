namespace Blaze2SDK.Blaze.Authentication
{
    public enum EmailStatus : int
    {
        BAD = 0x0,
        UNKNOWN = 0x1,
        VERIFIED = 0x2,
        GUEST = 0x3,
        ANONYMOUS = 0x4,
    }
}
