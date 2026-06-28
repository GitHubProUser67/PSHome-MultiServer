using EndianTools;

namespace CastleLibrary.S0ny.Edge
{
    internal struct ZlibChunkHeader
    {
        public const byte sizeOf = 4;

        internal readonly byte[] GetBytes()
        {
            var array = new byte[sizeOf];
            EndianAwareConverter.WriteUInt16(array, Endianness.BigEndian, 0, SourceSize);
            EndianAwareConverter.WriteUInt16(array, Endianness.BigEndian, 2, CompressedSize);
            return array;
        }

        internal static ZlibChunkHeader FromBytes(byte[] inData)
        {
            ZlibChunkHeader result = default;
            var array = inData;

            if (inData.Length > sizeOf)
            {
                array = new byte[sizeOf];
                Array.Copy(inData, array, sizeOf);
            }

            result.SourceSize = EndianAwareConverter.ToUInt16(array, Endianness.BigEndian, 0);
            result.CompressedSize = EndianAwareConverter.ToUInt16(array, Endianness.BigEndian, 2);
            return result;
        }

        internal ushort SourceSize;

        internal ushort CompressedSize;
    }
}
