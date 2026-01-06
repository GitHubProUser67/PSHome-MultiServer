namespace CastleLibrary.S0ny.XI5.Types
{
    public readonly struct TicketVersion
    {
        public byte Major { get; }
        public byte Minor { get; }

        internal TicketVersion(ushort val)
        {
            Major = (byte)(((byte)(val >> 8)) >> 4);
            Minor = (byte)(val & byte.MaxValue);
        }

        public override string ToString()
        {
            return Major + "." + Minor;
        }
    }
}
