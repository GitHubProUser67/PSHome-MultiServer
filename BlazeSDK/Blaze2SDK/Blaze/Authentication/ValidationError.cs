namespace Blaze2SDK.Blaze.Authentication
{
    public enum ValidationError : int
    {
        UNKNOWN = 0x0,
        INVALID_VALUE = 0x1,
        ILLEGAL_VALUE = 0x2,
        MISSING_VALUE = 0x3,
        DUPLICATE_VALUE = 0x4,
        INVALID_EMAIL_DOMAIN = 0x5,
        SPACES_NOT_ALLOWED = 0x6,
        TOO_SHORT = 0x7,
        TOO_LONG = 0x8,
        TOO_YOUNG = 0x9,
        TOO_OLD = 0xA,
        ILLEGAL_FOR_COUNTRY = 0xB,
        BANNED_COUNTRY = 0xC,
    }
}
