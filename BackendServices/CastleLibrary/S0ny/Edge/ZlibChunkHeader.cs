using EndianTools;
using System;

namespace CastleLibrary.S0ny.Edge
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
            byte[] array = new byte[sizeOf];
            EndianAwareConverter.WriteUInt16(array, Endianness.LittleEndian, 2, SourceSize);
            EndianAwareConverter.WriteUInt16(array, Endianness.LittleEndian, 0, CompressedSize);
            return array;
        }

        internal static ZlibChunkHeader FromBytes(byte[] inData)
        {
            ZlibChunkHeader result = default;
            byte[] array = inData;

            if (inData.Length > sizeOf)
            {
                array = new byte[sizeOf];
                Array.Copy(inData, array, sizeOf);
            }

            result.SourceSize = EndianAwareConverter.ToUInt16(array, Endianness.LittleEndian, 2);
            result.CompressedSize = EndianAwareConverter.ToUInt16(array, Endianness.LittleEndian, 0);
            return result;
        }

        internal ushort SourceSize;

        internal ushort CompressedSize;
    }
}
