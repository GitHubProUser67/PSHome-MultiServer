namespace Tdf
{
    public enum TdfBaseType : byte
    {
        TDF_TYPE_INTEGER = 0x0, //replaced the order of min and integer in order for ToString() work properly for this enum
        TDF_TYPE_MIN = 0x0,
        TDF_TYPE_STRING = 0x1,
        TDF_TYPE_BINARY = 0x2,
        TDF_TYPE_STRUCT = 0x3,
        TDF_TYPE_LIST = 0x4,
        TDF_TYPE_MAP = 0x5,
        TDF_TYPE_UNION = 0x6,
        TDF_TYPE_VARIABLE = 0x7,
        TDF_TYPE_BLAZE_OBJECT_TYPE = 0x8,
        TDF_TYPE_BLAZE_OBJECT_ID = 0x9,
        TDF_TYPE_FLOAT = 0xA,
        TDF_TYPE_TIMEVALUE = 0xB,
        TDF_TYPE_MAX = 0xC,
    };


    /*
        ### TDF_BASE_TYPE <-> C# TYPE ###
    
        TDF_TYPE_INTEGER -> bool, sbyte, byte, short, ushort, int, uint, long, ulong and TimeValue
        TDF_TYPE_STRING -> string
        TDF_TYPE_BINARY -> byte[]
        TDF_TYPE_STRUCT -> structs with TdfStruct attribute
        TDF_TYPE_LIST -> List<T>
        TDF_TYPE_MAP -> Dictionary<K, V> or SortedDictionary<K, V> (depends on use case, if <string, string> then need to use SortedDictionary, because receiving client will assume it is sorted) //TODO: Maybe remove Dictionary<K, V> support, because other map types might be sorted too?
        TDF_TYPE_UNION -> classes derived from TdfUnion
        TDF_TYPE_VARIABLE -> object? (can be anything) //TODO: Implement basic type support for variables (int, string, etc.)
        TDF_TYPE_BLAZE_OBJECT_TYPE -> BlazeObjectType
        TDF_TYPE_BLAZE_OBJECT_ID -> BlazeObjectId
        TDF_TYPE_FLOAT -> float
        TDF_TYPE_TIMEVALUE -> unused
     */
}