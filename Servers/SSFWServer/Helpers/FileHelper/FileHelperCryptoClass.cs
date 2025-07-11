using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using System.Security.Cryptography;
using System.Text;
using CustomLogger;
using MultiServerLibrary.Extension;

namespace SSFWServer.Helpers.FileHelper
{
    public class FileHelperCryptoClass
    {
        /// <summary>
        /// Gets an encryption key from rave secret key.
        /// </summary>
        /// <param name="secretKey">The secret key generated from your rave dashboard</param>
        /// <returns>a string value encrypted</returns>
        public static string GetEncryptionKey(string secretKey)
        {
            // MD5 is the hash algorithm expected by rave to generate encryption key
            // MD5 works with bytes so a conversion of plain secretKey to it bytes equivalent is required.
            byte[] hashedSecret = NetHasher.DotNetHasher.ComputeMD5(Encoding.UTF8.GetBytes(secretKey));
            byte[] hashedSecretLast12Bytes = new byte[12];

            Array.Copy(hashedSecret, hashedSecret.Length - 12, hashedSecretLast12Bytes, 0, 12);

            byte[] hashedSecretLast12HexBytes = Encoding.UTF8.GetBytes(BitConverter.ToString(hashedSecretLast12Bytes).ToLower().Replace("-", string.Empty));
            byte[] secretFirst12Bytes = Encoding.UTF8.GetBytes(secretKey.Replace("FLWSECK-", string.Empty)[..12]);
            byte[] combineKey = new byte[24];

            Array.Copy(secretFirst12Bytes, 0, combineKey, 0, secretFirst12Bytes.Length);
            Array.Copy(hashedSecretLast12HexBytes, hashedSecretLast12HexBytes.Length - 12, combineKey, 12, 12);

            return Encoding.UTF8.GetString(combineKey);
        }

        public static byte[] DecryptData(byte[] InData, string encryptionKey)
        {
            if (string.IsNullOrEmpty(encryptionKey))
                return Array.Empty<byte>();

            CustomXTEA? xtea = new();

            try
            {
                TripleDES des = TripleDES.Create();
                des.Key = Encoding.UTF8.GetBytes(encryptionKey);
                des.Mode = CipherMode.ECB;
                des.Padding = PaddingMode.PKCS7;

                byte[] firstSixBytes = new byte[6];

                Array.Copy(InData, 0, firstSixBytes, 0, firstSixBytes.Length);

                if (ByteUtils.FindBytePattern(firstSixBytes, new byte[] { 0x58, 0x54, 0x4e, 0x44, 0x56, 0x32 }) != -1)
                {
                    byte[] xteakey = new byte[16];

                    Array.Copy(des.Key, 0, xteakey, 0, xteakey.Length);

                    // With MultiServer 1.3 and up, TripleDes is improved with a custom crypto on top.

                    byte[] dst = new byte[InData.Length - 6];

                    Array.Copy(InData, 6, dst, 0, dst.Length);

                    InData = xtea.Decrypt(dst, InitiateCustomXTEACipheredKey(xteakey, ReverseByteArray(xteakey)));

                    if (InData == Array.Empty<byte>()) // Decryption failed.
                    {
                        des.Dispose();
                        xtea = null;
                        return Array.Empty<byte>();
                    }
                }

                ICryptoTransform cryptoTransform = des.CreateDecryptor();
                InData = cryptoTransform.TransformFinalBlock(InData, 0, InData.Length);

                cryptoTransform.Dispose();

                des.Dispose();
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[FileHelperCryptoClass] : Thrown an exception in DecryptData - {ex}");
            }

            xtea = null;

            return InData;
        }

        private static byte[] InitiateCustomXTEACipheredKey(byte[] KeyBytes, byte[] ReversedKeyBytes)
        {
            // Create the cipher
            IBufferedCipher? cipher = CipherUtilities.GetCipher("AES/ECB/NOPADDING");

            cipher.Init(true, new KeyParameter(ReversedKeyBytes));

            // Encrypt the plaintext
            byte[] ciphertextBytes = new byte[cipher.GetOutputSize(KeyBytes.Length)];
            int ciphertextLength = cipher.ProcessBytes(KeyBytes, 0, KeyBytes.Length, ciphertextBytes, 0);
            cipher.DoFinal(ciphertextBytes, ciphertextLength);

            cipher = null;

            return ciphertextBytes;
        }

        /// <summary>
        /// Reverse a byte array (When c# fails in some very rare occurances).
        /// <para>Retourne un tableau de bytes (Quand c# n'arrive pas � le faire � de rare moments).</para>
        /// </summary>
        /// <param name="input">The input byte array.</param>
        /// <returns>A byte array.</returns>
        private static byte[] ReverseByteArray(byte[] input)
        {
            byte[] reversedArray = new byte[input.Length];

            for (int i = 0; i < input.Length; i++)
            {
                reversedArray[i] = input[input.Length - 1 - i];
            }

            return reversedArray;
        }
    }
}
