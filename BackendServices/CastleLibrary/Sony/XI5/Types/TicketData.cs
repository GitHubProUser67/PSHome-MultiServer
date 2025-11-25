namespace CastleLibrary.Sony.XI5.Types
{
    public readonly struct TicketData
    {
        public readonly TicketDataType Type;
        public readonly ushort Length;

        public TicketData(TicketDataType type, ushort length)
        {
            Type = type;
            Length = length;
        }
    }
}
