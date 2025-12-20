namespace Blaze2SDK.Blaze.Authentication
{
    public enum StatusReason : int
    {
        UNKNOWN = 0x0,
        NONE = 0x1,
        REACTIVATED_CUSTOMER = 0x2,
        INVALID_EMAIL = 0x3,
        PRIVACY_POLICY = 0x4,
        PARENTS_REQUEST = 0x5,
        PARENTAL_REQUEST = 0x6,
        SUSPENDED_MISCONDUCT_GENERAL = 0x7,
        SUSPENDED_MISCONDUCT_HARASSMENT = 0x8,
        SUSPENDED_MISCONDUCT_MACROING = 0x9,
        SUSPENDED_MISCONDUCT_EXPLOITATION = 0xA,
        CUSTOMER_OPT_OUT = 0xB,
        CUSTOMER_UNDER_AGE = 0xC,
        EMAIL_CONFIRMATION_REQUIRED = 0xD,
        MISTYPED_ID = 0xE,
        ABUSED_ID = 0xF,
        DEACTIVATED_EMAIL_LINK = 0x10,
        DEACTIVATED_CS = 0x11,
        CLAIMED_BY_TRUE_OWNER = 0x12,
        BANNED = 0x13,
    }
}
