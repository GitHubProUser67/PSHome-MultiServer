using EndianTools;

namespace CastleLibrary.S0ny.Edge
{
    internal struct LZMAChunkHeader
    {
        public const byte sizeOf = 2;

        internal static LZMAChunkHeader FromBytes(byte[] inData)
        {
            LZMAChunkHeader result = default;
            var array = inData;

            if (inData.Length > sizeOf)
            {
                array = new byte[sizeOf];
                Array.Copy(inData, array, sizeOf);
            }

            result.CompressedSize = EndianAwareConverter.ToUInt16(
                array,
                Endianness.LittleEndian,
                0
            );
            return result;
        }

        internal ushort CompressedSize;
    }
}
