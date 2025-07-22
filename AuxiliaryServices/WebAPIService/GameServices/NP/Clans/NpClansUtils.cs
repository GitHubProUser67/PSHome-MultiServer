using NetHasher;
using System;
using System.Security.Cryptography;

namespace WebAPIService.GameServices.NP.Clans
{
    public static class NpClansUtils
    {
        public static readonly byte[] clansHMACKey = GeneratedXoredHmacKey();

        private static byte[] GeneratedXoredHmacKey()
        {
            byte[] key = new byte[20]
            {
                0x3D, 0x0D, 0x69, 0x2F, 0x31, 0xA6, 0x43, 0x44,
                0x5A, 0x06, 0xED, 0x16, 0xDE, 0x3D, 0xC6, 0x66,
                0x28, 0x55, 0xB1, 0x7C
            };
            int i = key.Length;

            do
            {
                key[i - 1] = (byte)(key[i - 1] ^ 0x7F);
                --i;
            }
            while (i != 0);

            return key;
        }

        public static byte[] GenerateNPSignature(byte[] message, int messageLength, byte[] key, int keySize)
        {
            const int blockSize = 64; // SHA1 block size in bytes
            const int hashSize = 20;  // SHA1 hash size in bytes

            byte[] actualKey;

            // If key size > block size, hash it first
            if (keySize > blockSize)
            {
                actualKey = DotNetHasher.ComputeSHA1(key);
                using (SHA1 sha1 = SHA1.Create())
                    actualKey = sha1.ComputeHash(key, 0, keySize);
            }
            else
            {
                actualKey = new byte[keySize];
                Array.Copy(key, actualKey, keySize);
            }

            // Pad key to blockSize with zeros
            byte[] keyPadded = new byte[blockSize];
            Array.Clear(keyPadded, 0, blockSize);
            Array.Copy(actualKey, keyPadded, actualKey.Length);

            // Create inner and outer padding
            byte[] ipad = new byte[blockSize];
            byte[] opad = new byte[blockSize];
            for (int i = 0; i < blockSize; i++)
            {
                ipad[i] = (byte)(keyPadded[i] ^ 0x36);
                opad[i] = (byte)(keyPadded[i] ^ 0x5c);
            }

            byte[] innerHash;

            // Inner SHA1: SHA1(ipad || message)
            using (SHA1 sha1 = SHA1.Create())
            {
                sha1.TransformBlock(ipad, 0, blockSize, ipad, 0);
                sha1.TransformFinalBlock(message, 0, messageLength);
                innerHash = sha1.Hash;
            }

            byte[] finalHash;

            // Outer SHA1: SHA1(opad || innerHash)
            using (SHA1 sha1 = SHA1.Create())
            {
                sha1.TransformBlock(opad, 0, blockSize, opad, 0);
                sha1.TransformFinalBlock(innerHash, 0, hashSize);
                finalHash = sha1.Hash;
            }

            return finalHash;
        }
    }
}
