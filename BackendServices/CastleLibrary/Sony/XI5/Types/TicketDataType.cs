namespace CastleLibrary.Sony.XI5.Types
{
    public enum TicketDataType : ushort
    {
        Empty = 0,
        UInt32 = 1,
        UInt64 = 2,

        String = 4,

        Timestamp = 7,
        Binary = 8,
    }
}
