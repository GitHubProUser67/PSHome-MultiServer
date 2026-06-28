using EndianTools;
using Org.BouncyCastle.Math;

namespace MultiServerLibrary.Extension
{
    public static class MathUtils
    {
        extension(int number)
        {
            public string ToUuid()
            {
                return $"00000000-00000000-00000000-{number:D8}";
            }
        }

        public static uint[] BigEndianRsa512BytesToUIntArray(byte[] bigEndianBytes)
        {
            if (bigEndianBytes.Length != 64)
                throw new ArgumentException("[MathUtils] - BigEndianRsa512BytesToUIntArray: Array length must be 64 bytes.");

            var result = new uint[16];

            for (var i = 0; i < result.Length; i += 4)
                result[i] = EndianAwareConverter.ToUInt32(bigEndianBytes, Endianness.LittleEndian, (uint)i);

            return result;
        }

        public static BigInteger UIntArrayToRsa512Modulus(uint[] words)
        {
            if (words.Length != 16)
                throw new ArgumentException("[MathUtils] - UIntArrayToRsa512Modulus: modulus must contain exactly 16 uints.");

            var modulusBytes = new byte[64];

            for (var i = 0; i < 16; i++)
                EndianAwareConverter.WriteUInt32(modulusBytes, Endianness.LittleEndian, (uint)(i * 4), words[i]);

            // BouncyCastle expects unsigned big-endian
            return new BigInteger(1, modulusBytes);
        }
    }
}
