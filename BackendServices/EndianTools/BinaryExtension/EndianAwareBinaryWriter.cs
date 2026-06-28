namespace EndianTools.BinaryExtension
{
    public abstract class EndianAwareBinaryWriter(Stream output)
    {
        public static EndianAwareBinaryWriter Create(Stream output, Endianness endian)
        {
            return endian == Endianness.LittleEndian
                ? new LEBinaryWriter(output)
                : new BEBinaryWriter(output);
        }

        public abstract void Write(byte[] bytes);

        public abstract void Write(uint val);

        public abstract void Write(ushort val);

        public abstract void Write(int val);

        public abstract void Write(short val);

        public abstract void Write(float val);

        public abstract void Write(long val);

        public abstract void Write(ulong val);

        public void Close()
        {
            m_bw.Close();
        }

        protected BinaryWriter m_bw = new(output);
    }
}
