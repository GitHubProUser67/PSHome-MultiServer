using System.IO;

namespace EndianTools.BinaryExtension
{
    public abstract class EndianAwareBinaryReader
    {
        public EndianAwareBinaryReader(Stream input)
        {
            m_br = new BinaryReader(input);
        }

        public static EndianAwareBinaryReader Create(Stream input, Endianness endian)
        {
            if (endian == Endianness.LittleEndian)
                return new LEBinaryReader(input);
            return new BEBinaryReader(input);
        }

        public abstract byte[] ReadBytes(int length);

        public abstract uint ReadUInt32();

        public abstract ushort ReadUInt16();

        public abstract int ReadInt32();

        public abstract short ReadInt16();

        public abstract float ReadSingle();

        public abstract long ReadInt64();

        public abstract ulong ReadUInt64();

        protected BinaryReader m_br;
    }
}