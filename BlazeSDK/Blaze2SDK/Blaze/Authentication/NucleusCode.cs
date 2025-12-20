namespace Blaze2SDK.Blaze.Authentication
{
    public enum NucleusCode : int
    {
        UNKNOWN = 0x0,
        VALIDATION_FAILED = 0x1,
        NO_SUCH_USER = 0x2,
        NO_SUCH_COUNTRY = 0x3,
        NO_SUCH_PERSONA = 0x4,
        NO_SUCH_EXTERNAL_REFERENCE = 0x5,
        NO_SUCH_NAMESPACE = 0x6,
        INVALID_PASSWORD = 0x7,
        CODE_ALREADY_USED = 0x8,
        INVALID_CODE = 0x9,
        PARSE_EXCEPTION = 0xA,
        NO_SUCH_GROUP = 0xB,
        NO_SUCH_GROUP_NAME = 0xC,
        NO_ASSOCIATED_PRODUCT = 0xD,
        CODE_ALREADY_DISABLED = 0xE,
        GROUP_NAME_DOES_NOT_MATCH = 0xF,
    }
}
