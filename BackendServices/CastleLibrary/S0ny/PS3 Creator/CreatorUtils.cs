using System;
using System.Numerics;
using System.Security.Cryptography;

namespace CastleLibrary.S0ny.PS3_Creator
{
    public class CreatorUtils
    {
        public const int ENCRYPT_MODE = 1;
        public const int DECRYPT_MODE = 2;

        public static void Fail(String a)
        {
            CustomLogger.LoggerAccessor.LogError($"[PS3 Creator] - CreatorUtils - Fail:{a}");
        }

        public static void AesecbDecrypt(byte[] key, byte[] i, int inOffset, byte[] o, int outOffset, int len)
        {
            CipherMode mode = CipherMode.ECB;
            PaddingMode padding = PaddingMode.None;
            
            int opMode = DECRYPT_MODE;
            Crypto(key, mode, padding, null, opMode, i, inOffset, len, o, outOffset);
        }

        public static void AesecbEncrypt(byte[] key, byte[] i, int inOffset, byte[] o, int outOffset, int len)
        {
            CipherMode mode = CipherMode.ECB;
            PaddingMode padding = PaddingMode.None;
            int opMode = ENCRYPT_MODE;
            Crypto(key, mode, padding, null, opMode, i, inOffset, len, o, outOffset);
        }

        public static void AescbcDecrypt(byte[] key, byte[] iv, byte[] i, int inOffset, byte[] o, int outOffset, int len)
        {
            CipherMode mode = CipherMode.CBC;
            PaddingMode padding = PaddingMode.None;
            int opMode = DECRYPT_MODE;
            Crypto(key, mode, padding, iv, opMode, i, inOffset, len, o, outOffset);
        }

        private static void CalculateSubkey(byte[] key, byte[] K1, byte[] K2)
        {
            byte[] zero = new byte[0x10];
            byte[] L = new byte[0x10];
            AesecbEncrypt(key, zero, 0, L, 0, zero.Length);
            BigInteger aux = new BigInteger(ConversionUtils.ReverseByteWithSizeFIX(L));

            if ((L[0] & 0x80) != 0)
                // Case MSB is set
                aux = (aux << 1) ^ (new BigInteger(0x87));
            else
                aux <<= 1;
            byte[] aux1 = ConversionUtils.ReverseByteWithSizeFIX(aux.ToByteArray());
            if (aux1.Length >= 0x10)
                ConversionUtils.Arraycopy(aux1, aux1.Length - 0x10, K1, 0, 0x10);
            else
            {
                ConversionUtils.Arraycopy(zero, 0, K1, 0, zero.Length);
                ConversionUtils.Arraycopy(aux1, 0, K1, 0x10 - aux1.Length, aux1.Length);
            }
            aux = new BigInteger(ConversionUtils.ReverseByteWithSizeFIX(K1));

            if ((K1[0] & 0x80) != 0)
                aux = (aux << 1) ^ (new BigInteger(0x87));
            else
                aux <<= 1;
            aux1 = ConversionUtils.ReverseByteWithSizeFIX(aux.ToByteArray());
            if (aux1.Length >= 0x10)
                ConversionUtils.Arraycopy(aux1, aux1.Length - 0x10, K2, 0, 0x10);
            else
            {
                ConversionUtils.Arraycopy(zero, 0, K2, 0, zero.Length);
                ConversionUtils.Arraycopy(aux1, 0, K2, 0x10 - aux1.Length, aux1.Length);
            }
        }

        private static void Crypto(byte[] key, CipherMode mode, PaddingMode padding, byte[] iv, int opMode, byte[] i, int inOffset, int len, byte[] o, int outOffset)
        {
            try
            {
                Aes cipher = Aes.Create();
                cipher.Padding = padding;
                cipher.Mode = mode;
                cipher.KeySize = 0x80;
                cipher.BlockSize = 0x80;
                cipher.Key = key;
                if (iv != null)
                    cipher.IV = iv;

                byte[] aux = null;
                if (opMode == DECRYPT_MODE)
                    aux = cipher.CreateDecryptor().TransformFinalBlock(i, inOffset, len);
                else if (opMode == ENCRYPT_MODE)
                    aux = cipher.CreateEncryptor().TransformFinalBlock(i, inOffset, len);
                else
                    Fail("NOT SUPPORTED OPMODE");
                ConversionUtils.Arraycopy(aux, 0, o, outOffset, len);
            }
            catch (Exception ex)
            {
                Fail(ex.Message);
            }
        }

        public static byte[] CMAC128(byte[] key, byte[] i, int inOffset, int len)
        {
            byte[] K1 = new byte[0x10];
            byte[] K2 = new byte[0x10];
            CalculateSubkey(key, K1, K2);
            byte[] input = new byte[0x10];
            byte[] previous = new byte[0x10];
            int currentOffset = inOffset;
            int remaining = len;
            while (remaining > 0x10)
            {
                ConversionUtils.Arraycopy(i, currentOffset, input, 0, 0x10);
                XOR(input, input, previous);

                AesecbEncrypt(key, input, 0, previous, 0, input.Length);
                currentOffset += 0x10;
                remaining -= 0x10;
            }
            input = new byte[0x10]; // Memset 0
            ConversionUtils.Arraycopy(i, currentOffset, input, 0, remaining);
            if (remaining == 0x10)
            {
                XOR(input, input, previous);
                XOR(input, input, K1);
            }
            else
            {
                input[remaining] = (byte)0x80;
                XOR(input, input, previous);
                XOR(input, input, K2);
            }
            AesecbEncrypt(key, input, 0, previous, 0, input.Length);
            return previous;

        }

        public static void XOR(byte[] output, byte[] inputA, byte[] inputB)
        {
            for (int i = 0; i < inputA.Length; i++)
                output[i] = (byte)(inputA[i] ^ inputB[i]);
        }

        public static bool CompareBytes(byte[] value1, int offset1, byte[] value2, int offset2, int len)
        {
            bool result = true;
            for (int i = 0; i < len; i++)
            {
                if (value1[i + offset1] != value2[i + offset2])
                {
                    result = false;
                    break;
                }
            }
            return result;
        }
    }
}
