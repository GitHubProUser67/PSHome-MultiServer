using System.IO;

namespace EndianTools.BinaryExtension
{
    internal class LEBinaryWriter : EndianAwareBinaryWriter
    {
        internal LEBinaryWriter(Stream output) : base(output)
        {
        }

        public override void Write(byte[] bytes)
        {
            m_bw.Write(bytes);
        }

        public override void Write(uint val)
        {
            m_bw.Write(val);
        }

        public override void Write(ushort val)
        {
            m_bw.Write(val);
        }

        public override void Write(int val)
        {
            m_bw.Write(val);
        }

        public override void Write(short val)
        {
            m_bw.Write(val);
        }

        public override void Write(float val)
        {
            m_bw.Write(val);
        }

        public override void Write(long val)
        {
            m_bw.Write(val);
        }

        public override void Write(ulong val)
        {
            m_bw.Write(val);
        }
    }
}