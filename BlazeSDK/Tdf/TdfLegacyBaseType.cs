namespace Tdf
{
    public enum TdfLegacyBaseType : byte
    {
        TYPE_STRUCT = 0x0,
        TYPE_STRING = 0x1,
        TYPE_INT8 = 0x2,
        TYPE_UINT8 = 0x3,
        TYPE_INT16 = 0x4,
        TYPE_UINT16 = 0x5,
        TYPE_INT32 = 0x6,
        TYPE_UINT32 = 0x7,
        TYPE_INT64 = 0x8,
        TYPE_UINT64 = 0x9,
        TYPE_ARRAY = 0xA,
        TYPE_BLOB = 0xB,
        TYPE_MAP = 0xC,
        TYPE_UNION = 0xD,
    }
}
