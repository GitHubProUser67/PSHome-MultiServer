using EndianTools;
using System;

namespace CastleLibrary.Sony.Edge
{
    internal struct ZlibChunkHeader
    {
        public const byte sizeOf = 4;

#if NETCOREAPP || NETSTANDARD2_1_OR_GREATER
        internal readonly byte[] GetBytes()
#else
        internal byte[] GetBytes()
#endif
        {
            byte[] array = new byte[4];
            Array.Copy(BitConverter.GetBytes(!BitConverter.IsLittleEndian ? EndianUtils.ReverseUshort(SourceSize) : SourceSize), 0, array, 2, 2);
            Array.Copy(BitConverter.GetBytes(!BitConverter.IsLittleEndian ? EndianUtils.ReverseUshort(CompressedSize) : CompressedSize), 0, array, 0, 2);
            return array;
        }

        internal static ZlibChunkHeader FromBytes(byte[] inData)
        {
            ZlibChunkHeader result = default;
            byte[] array = inData;

            if (inData.Length > sizeOf)
            {
                array = new byte[4];
                Array.Copy(inData, array, 4);
            }

            if (!BitConverter.IsLittleEndian)
                Array.Reverse(array);

            result.SourceSize = BitConverter.ToUInt16(array, 2);
            result.CompressedSize = BitConverter.ToUInt16(array, 0);
            return result;
        }

        internal ushort SourceSize;

        internal ushort CompressedSize;
    }
}
