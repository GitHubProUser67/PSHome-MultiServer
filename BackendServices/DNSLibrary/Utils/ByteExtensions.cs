namespace DNS.Protocol.Utils {
    public static class ByteExtensions {
        public static byte GetBitValueAt(this byte b, byte offset, byte length) {
            return (byte)((b >> offset) & ~(byte.MaxValue << length));
        }

        public static byte GetBitValueAt(this byte b, byte offset) {
            return b.GetBitValueAt(offset, 1);
        }

        public static byte SetBitValueAt(this byte b, byte offset, byte length, byte value) { 
            int mask = ~(byte.MaxValue << length);
            value = (byte)(value & mask);

            return (byte)((value << offset) | (b & ~(mask << offset)));
        }

        public static byte SetBitValueAt(this byte b, byte offset, byte value) {
            return b.SetBitValueAt(offset, 1, value);
        }
    }
}
