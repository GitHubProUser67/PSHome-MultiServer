namespace EndianTools.BinaryExtension
{
    public class BEBinaryWriter(Stream output) : EndianAwareBinaryWriter(output)
    {
        public override void Write(byte[] bytes)
        {
            m_bw.Write(EndianUtils.EndianSwap(bytes));
        }

        public override void Write(uint val)
        {
            var bytes = BitConverter.GetBytes(
                !EndianTools.EndianAwareConverter.isLittleEndianSystem
                    ? EndianUtils.ReverseUint(val)
                    : val
            );
            Array.Reverse(bytes);
            m_bw.Write(bytes);
        }

        public override void Write(ushort val)
        {
            var bytes = BitConverter.GetBytes(
                !EndianTools.EndianAwareConverter.isLittleEndianSystem
                    ? EndianUtils.ReverseUshort(val)
                    : val
            );
            Array.Reverse(bytes);
            m_bw.Write(bytes);
        }

        public override void Write(int val)
        {
            var bytes = BitConverter.GetBytes(
                !EndianTools.EndianAwareConverter.isLittleEndianSystem
                    ? EndianUtils.ReverseInt(val)
                    : val
            );
            Array.Reverse(bytes);
            m_bw.Write(bytes);
        }

        public override void Write(short val)
        {
            var bytes = BitConverter.GetBytes(
                !EndianTools.EndianAwareConverter.isLittleEndianSystem
                    ? EndianUtils.ReverseShort(val)
                    : val
            );
            Array.Reverse(bytes);
            m_bw.Write(bytes);
        }

        public override void Write(float val)
        {
            var bytes = BitConverter.GetBytes(
                !EndianTools.EndianAwareConverter.isLittleEndianSystem
                    ? EndianUtils.ReverseFloat(val)
                    : val
            );
            Array.Reverse(bytes);
            m_bw.Write(bytes);
        }

        public override void Write(long val)
        {
            var bytes = BitConverter.GetBytes(
                !EndianTools.EndianAwareConverter.isLittleEndianSystem
                    ? EndianUtils.ReverseLong(val)
                    : val
            );
            Array.Reverse(bytes);
            m_bw.Write(bytes);
        }

        public override void Write(ulong val)
        {
            var bytes = BitConverter.GetBytes(
                !EndianTools.EndianAwareConverter.isLittleEndianSystem
                    ? EndianUtils.ReverseUlong(val)
                    : val
            );
            Array.Reverse(bytes);
            m_bw.Write(bytes);
        }
    }
}
