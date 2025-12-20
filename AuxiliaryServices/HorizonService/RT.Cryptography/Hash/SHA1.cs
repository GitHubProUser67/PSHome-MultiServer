using NetHasher;
using System;

namespace Horizon.RT.Cryptography.Hash
{
    public static class SHA1
    {
        public static byte[] Hash(byte[] input, CipherContext context)
        {
            byte[] result = new byte[4];
            Hash(input, result, 0, (byte)context);
            return result;
        }

        private static void Hash(
            byte[] input,
                byte[] output,
                int outOff,
                byte encryptionType)
        {
            // Compute sha1 hash
            byte[] result = DotNetHasher.ComputeSHA1(input);

            // Inject context inter highest 3 bits
            result[3] = (byte)((result[3] & 0x1F) | ((encryptionType & 7) << 5));

            Array.Copy(result, 0, output, outOff, 4);
        }
    }
}