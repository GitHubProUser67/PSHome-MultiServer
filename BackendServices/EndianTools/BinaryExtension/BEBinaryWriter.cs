using System;
using System.IO;

namespace EndianTools.BinaryExtension
{
    public class BEBinaryWriter : EndianAwareBinaryWriter
    {
        public BEBinaryWriter(Stream output) : base(output)
        {
        }

        public override void Write(byte[] bytes)
        {
            m_bw.Write(EndianUtils.EndianSwap(bytes));
        }

        public override void Write(uint val)
        {
            byte[] bytes = BitConverter.GetBytes(!BitConverter.IsLittleEndian ? EndianUtils.ReverseUint(val) : val);
            Array.Reverse(bytes);
            m_bw.Write(bytes);
        }

        public override void Write(ushort val)
        {
            byte[] bytes = BitConverter.GetBytes(!BitConverter.IsLittleEndian ? EndianUtils.ReverseUshort(val) : val);
            Array.Reverse(bytes);
            m_bw.Write(bytes);
        }

        public override void Write(int val)
        {
            byte[] bytes = BitConverter.GetBytes(!BitConverter.IsLittleEndian ? EndianUtils.ReverseInt(val) : val);
            Array.Reverse(bytes);
            m_bw.Write(bytes);
        }

        public override void Write(short val)
        {
            byte[] bytes = BitConverter.GetBytes(!BitConverter.IsLittleEndian ? EndianUtils.ReverseShort(val) : val);
            Array.Reverse(bytes);
            m_bw.Write(bytes);
        }

        public override void Write(float val)
        {
            byte[] bytes = BitConverter.GetBytes(!BitConverter.IsLittleEndian ? EndianUtils.ReverseFloat(val) : val);
            Array.Reverse(bytes);
            m_bw.Write(bytes);
        }

        public override void Write(long val)
        {
            byte[] bytes = BitConverter.GetBytes(!BitConverter.IsLittleEndian ? EndianUtils.ReverseLong(val) : val);
            Array.Reverse(bytes);
            m_bw.Write(bytes);
        }

        public override void Write(ulong val)
        {
            byte[] bytes = BitConverter.GetBytes(!BitConverter.IsLittleEndian ? EndianUtils.ReverseUlong(val) : val);
            Array.Reverse(bytes);
            m_bw.Write(bytes);
        }
    }
}