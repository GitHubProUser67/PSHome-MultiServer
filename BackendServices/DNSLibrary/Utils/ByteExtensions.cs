namespace DNSLibrary.Utils
{
    public static class ByteExtensions
    {
        extension(byte b)
        {
            public byte GetBitValueAt(byte offset, byte length)
            {
                return (byte)((b >> offset) & ~(byte.MaxValue << length));
            }

            public byte GetBitValueAt(byte offset)
            {
                return b.GetBitValueAt(offset, 1);
            }

            public byte SetBitValueAt(byte offset, byte length, byte value)
            {
                var mask = ~(byte.MaxValue << length);
                value = (byte)(value & mask);

                return (byte)((value << offset) | (b & ~(mask << offset)));
            }

            public byte SetBitValueAt(byte offset, byte value)
            {
                return b.SetBitValueAt(offset, 1, value);
            }
        }
    }
}
