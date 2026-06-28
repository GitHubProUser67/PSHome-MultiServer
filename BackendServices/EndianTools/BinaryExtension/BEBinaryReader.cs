namespace EndianTools.BinaryExtension
{
    public class BEBinaryReader(Stream input) : EndianAwareBinaryReader(input)
    {
        public override byte[] ReadBytes(int length)
        {
            //.NET8 m_br.BaseStream will have length 0 sometimes.like:https://github.com/dotnet/wcf/issues/5205
            return m_br.BaseStream.Length == 0
                ? []
                : EndianUtils.EndianSwap(m_br.ReadBytes(length));
        }

        public override byte ReadByte()
        {
            var bytes = ReadBytes(1);
            return bytes.Length == 0 ? (byte)0 : bytes[0];
        }

        public override short ReadInt16()
        {
            //.NET8 m_br.BaseStream will have length 0 sometimes.like:https://github.com/dotnet/wcf/issues/5205
            if (m_br.BaseStream.Length == 0)
                return 0;

            var num = 2;
            var array = new byte[num];
            m_br.Read(array, 0, num);
            Array.Reverse(array);
            return BitConverter.ToInt16(
                !EndianTools.EndianAwareConverter.isLittleEndianSystem
                    ? EndianUtils.ReverseArray(array)
                    : array,
                0
            );
        }

        public override int ReadInt32()
        {
            //.NET8 m_br.BaseStream will have length 0 sometimes.like:https://github.com/dotnet/wcf/issues/5205
            if (m_br.BaseStream.Length == 0)
                return 0;

            var num = 4;
            var array = new byte[num];
            m_br.Read(array, 0, num);
            Array.Reverse(array);
            return BitConverter.ToInt32(
                !EndianTools.EndianAwareConverter.isLittleEndianSystem
                    ? EndianUtils.ReverseArray(array)
                    : array,
                0
            );
        }

        public override float ReadSingle()
        {
            //.NET8 m_br.BaseStream will have length 0 sometimes.like:https://github.com/dotnet/wcf/issues/5205
            if (m_br.BaseStream.Length == 0)
                return 0;

            var num = 4;
            var array = new byte[num];
            m_br.Read(array, 0, num);
            Array.Reverse(array);
            return BitConverter.ToSingle(
                !EndianTools.EndianAwareConverter.isLittleEndianSystem
                    ? EndianUtils.ReverseArray(array)
                    : array,
                0
            );
        }

        public override ushort ReadUInt16()
        {
            //.NET8 m_br.BaseStream will have length 0 sometimes.like:https://github.com/dotnet/wcf/issues/5205
            if (m_br.BaseStream.Length == 0)
                return 0;

            var num = 2;
            var array = new byte[num];
            m_br.Read(array, 0, num);
            Array.Reverse(array);
            return BitConverter.ToUInt16(
                !EndianTools.EndianAwareConverter.isLittleEndianSystem
                    ? EndianUtils.ReverseArray(array)
                    : array,
                0
            );
        }

        public override uint ReadUInt32()
        {
            //.NET8 m_br.BaseStream will have length 0 sometimes.like:https://github.com/dotnet/wcf/issues/5205
            if (m_br.BaseStream.Length == 0)
                return 0;

            var num = 4;
            var array = new byte[num];
            m_br.Read(array, 0, num);
            Array.Reverse(array);
            return BitConverter.ToUInt32(
                !EndianTools.EndianAwareConverter.isLittleEndianSystem
                    ? EndianUtils.ReverseArray(array)
                    : array,
                0
            );
        }

        public override long ReadInt64()
        {
            //.NET8 m_br.BaseStream will have length 0 sometimes.like:https://github.com/dotnet/wcf/issues/5205
            if (m_br.BaseStream.Length == 0)
                return 0;

            var num = 8;
            var array = new byte[num];
            m_br.Read(array, 0, num);
            Array.Reverse(array);
            return BitConverter.ToInt64(
                !EndianTools.EndianAwareConverter.isLittleEndianSystem
                    ? EndianUtils.ReverseArray(array)
                    : array,
                0
            );
        }

        public override ulong ReadUInt64()
        {
            //.NET8 m_br.BaseStream will have length 0 sometimes.like:https://github.com/dotnet/wcf/issues/5205
            if (m_br.BaseStream.Length == 0)
                return 0;

            var num = 8;
            var array = new byte[num];
            m_br.Read(array, 0, num);
            Array.Reverse(array);
            return BitConverter.ToUInt64(
                !EndianTools.EndianAwareConverter.isLittleEndianSystem
                    ? EndianUtils.ReverseArray(array)
                    : array,
                0
            );
        }
    }
}
