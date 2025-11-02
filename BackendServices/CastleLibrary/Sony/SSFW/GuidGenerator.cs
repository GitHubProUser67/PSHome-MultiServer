using NetHasher;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Text;
using System.Text.Json;

namespace CastleLibrary.Sony.SSFW
{
    public static class GuidGenerator
    {
        private static readonly byte[] _authIV = new byte[] { 0x30, 0x57, 0xB5, 0x1F, 0x32, 0xD4, 0xAD, 0xBF, 0xAA, 0xAA, 0x21, 0x41, 0x6C, 0xDC, 0x5D, 0xF5 };

        public static string SSFWGenerateGuid(string input1, string input2, string key = null)
        {
            string md5hash = null;
            string sha512hash = null;
            byte[] input1Bytes = Encoding.UTF8.GetBytes(input1 + "**H0mEIsG3reAT!!!!!!!!!!!!!!");
            byte[] input2Bytes = Encoding.UTF8.GetBytes("C0MeBaCKHOm3*!*!*!*!*!*!*!*!" + input2);

            if (!string.IsNullOrEmpty(key))
            {
                const char equalSign = '=';
                Span<byte> buffer = new byte[((key.Length * 3) + 3) / 4 -
                    (key.Length > 0 && key[^1] == equalSign ?
                        key.Length > 1 && key[^2] == equalSign ?
                            2 : 1 : 0)];

                if (Convert.TryFromBase64String(key, buffer, out int bytesWritten))
                {
                    byte[] keyBytes = buffer[..bytesWritten].ToArray();
                    md5hash = DotNetHasher.ComputeMD5String(InitiateCBCEncryptBufferTobase64String(JsonSerializer.Serialize(input1Bytes), keyBytes, _authIV));
                    sha512hash = DotNetHasher.ComputeSHA512String(InitiateCBCEncryptBufferTobase64String(JsonSerializer.Serialize(input2Bytes), keyBytes, _authIV));
                }
            }

            // Fallback to the older method.
            md5hash ??= DotNetHasher.ComputeMD5String(input1Bytes);
            sha512hash ??= DotNetHasher.ComputeSHA512String(input2Bytes);

            return (md5hash.Substring(1, 8) + "-" + sha512hash.Substring(2, 4) + "-" + md5hash.Substring(10, 4) + "-" + sha512hash.Substring(16, 4) + "-" + sha512hash.Substring(19, 12)).ToLower();
        }

        private static byte[] InitiateCBCEncryptBufferTobase64String(string FileString, byte[] KeyBytes, byte[] m_iv)
        {
            if (KeyBytes.Length >= 16 && m_iv.Length == 16)
                return Encoding.UTF8.GetBytes(Convert.ToBase64String(InitiateCBCEncryptBuffer(Encoding.UTF8.GetBytes(FileString), KeyBytes, m_iv)));
            else
                CustomLogger.LoggerAccessor.LogError("[GuidGenerator] - InitiateCBCEncryptBufferTobase64String - Invalid KeyBytes or IV!");

            return null;
        }

        private static byte[] InitiateCBCEncryptBuffer(byte[] FileBytes, byte[] KeyBytes, byte[] m_iv)
        {
            if (KeyBytes.Length >= 16 && m_iv.Length == 16)
            {
                // Create the cipher
                IBufferedCipher cipher = CipherUtilities.GetCipher("AES/CBC/OAEPWITHSHA224ANDMGF1PADDING");

                cipher.Init(true, new ParametersWithIV(new KeyParameter(KeyBytes), m_iv));

                // Encrypt the plaintext
                byte[] ciphertextBytes = new byte[cipher.GetOutputSize(FileBytes.Length)];
                int ciphertextLength = cipher.ProcessBytes(FileBytes, 0, FileBytes.Length, ciphertextBytes, 0);
                cipher.DoFinal(ciphertextBytes, ciphertextLength);

                return ciphertextBytes;
            }

            CustomLogger.LoggerAccessor.LogError("[GuidGenerator] - InitiateCBCEncryptBuffer - Invalid KeyBytes or IV!");

            return null;
        }
    }
}
