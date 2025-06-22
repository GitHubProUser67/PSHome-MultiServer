namespace XI5.Types
{
    public readonly struct TicketDataSection
    {
        public TicketDataSectionType Type { get; }
        public ushort Length { get; }
        public int Position { get; }

        public TicketDataSection(TicketDataSectionType type, ushort length, long position)
        {
            Type = type;
            Length = length;
            Position = (int)position;
        }
    }
}
