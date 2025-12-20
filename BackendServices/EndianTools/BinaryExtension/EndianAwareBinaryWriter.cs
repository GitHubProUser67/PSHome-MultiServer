using System.IO;

namespace EndianTools.BinaryExtension
{
    public abstract class EndianAwareBinaryWriter
    {
        public EndianAwareBinaryWriter(Stream output)
        {
            m_bw = new BinaryWriter(output);
        }

        public static EndianAwareBinaryWriter Create(Stream output, Endianness endian)
        {
            if (endian == Endianness.LittleEndian)
                return new LEBinaryWriter(output);
            return new BEBinaryWriter(output);
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

        protected BinaryWriter m_bw;
    }
}