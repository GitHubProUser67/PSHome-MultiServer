using CustomLogger;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using EndianTools;
using System.Text;
using System;
using System.Threading.Tasks;
using NetworkLibrary.Extension;

namespace HomeTools.Crypto
{
    public class LIBSECURE
    {
        public static Task<byte[]> InitiateXTEABufferAsync(byte[] FileBytes, byte[] KeyBytes, byte[] m_iv, string mode, bool memxor = true, bool encrypt = false)
        {
            try
            {
                if (KeyBytes.Length == 16)
                {
                    // Create the cipher
                    IBufferedCipher cipher = CipherUtilities.GetCipher($"LIBSECUREXTEA/{mode}/NOPADDING");

                    if (mode == "CTR" || mode == "CBC")
                    {
                        if (m_iv == null || m_iv.Length != 8)
                        {
                            LoggerAccessor.LogError("[LIBSECURE] - InitiateXTEABuffer - Invalid IV!");
                            return Task.FromResult<byte[]>(null);
                        }

                        cipher.Init(encrypt, new ParametersWithIV(new KeyParameter(EndianUtils.EndianSwap(KeyBytes)), EndianUtils.EndianSwap(m_iv)));
                    }
                    else
                        cipher.Init(encrypt, new KeyParameter(EndianUtils.EndianSwap(KeyBytes)));

                    // Encrypt the plaintext
                    byte[] ciphertextBytes = new byte[cipher.GetOutputSize(FileBytes.Length)];
                    int ciphertextLength = cipher.ProcessBytes(memxor ? new byte[FileBytes.Length] : EndianUtils.EndianSwap(FileBytes), 0, FileBytes.Length, ciphertextBytes, 0); // Little optimization for nulled bytes array, no need to endian swap a bunch of nulls.
                    cipher.DoFinal(ciphertextBytes, ciphertextLength);

                    cipher = null;

                    return Task.FromResult(memxor ? Crypt_Decrypt(FileBytes, EndianUtils.EndianSwap(ciphertextBytes), 8) : EndianUtils.EndianSwap(ciphertextBytes));
                }
                else
                    LoggerAccessor.LogError("[LIBSECURE] - InitiateXTEABuffer - Invalid KeyByes!");

                return Task.FromResult<byte[]>(null);
            }
            finally
            {
                ToolsImplementation.libsecureSema.Release();
            }
        }

        public static Task<byte[]> InitiateBlowfishBufferAsync(byte[] FileBytes, byte[] KeyBytes, byte[] m_iv, string mode, bool memxor = true, bool encrypt = false)
        {
            try
            {
                if (KeyBytes.Length == 32)
                {
                    // Create the cipher
                    IBufferedCipher cipher = CipherUtilities.GetCipher($"Blowfish/{mode}/NOPADDING");

                    if (mode == "CTR" || mode == "CBC")
                    {
                        if (m_iv == null || m_iv.Length != 8)
                        {
                            LoggerAccessor.LogError("[LIBSECURE] - InitiateBlowfishBuffer - Invalid IV!");
                            return Task.FromResult<byte[]>(null);
                        }

                        cipher.Init(encrypt, new ParametersWithIV(new KeyParameter(KeyBytes), m_iv));
                    }
                    else
                        cipher.Init(encrypt, new KeyParameter(KeyBytes));

                    // Encrypt the plaintext
                    byte[] ciphertextBytes = new byte[cipher.GetOutputSize(FileBytes.Length)];
                    int ciphertextLength = cipher.ProcessBytes(memxor ? new byte[FileBytes.Length] : FileBytes, 0, FileBytes.Length, ciphertextBytes, 0);
                    cipher.DoFinal(ciphertextBytes, ciphertextLength);

                    cipher = null;

                    return Task.FromResult(memxor ? Crypt_Decrypt(FileBytes, ciphertextBytes, 8) : ciphertextBytes);
                }
                else
                    LoggerAccessor.LogError("[LIBSECURE] - InitiateBlowfishBuffer - Invalid KeyByes!");

                return Task.FromResult<byte[]>(null);
            }
            finally
            {
                ToolsImplementation.libsecureSema.Release();
            }
        }

        public static Task<byte[]> InitiateAESBufferAsync(byte[] FileBytes, byte[] KeyBytes, byte[] m_iv, string mode, bool memxor = true, bool encrypt = false)
        {
            try
            {
                if (KeyBytes.Length >= 16)
                {
                    // Create the cipher
                    IBufferedCipher cipher = CipherUtilities.GetCipher($"AES/{mode}/NOPADDING");

                    if (mode == "CTR" || mode == "CBC")
                    {
                        if (m_iv == null || m_iv.Length != 16)
                        {
                            LoggerAccessor.LogError("[LIBSECURE] - InitiateAESBuffer - Invalid IV!");
                            return Task.FromResult<byte[]>(null);
                        }

                        cipher.Init(encrypt, new ParametersWithIV(new KeyParameter(KeyBytes), m_iv));
                    }
                    else
                        cipher.Init(encrypt, new KeyParameter(KeyBytes));

                    // Encrypt the plaintext
                    byte[] ciphertextBytes = new byte[cipher.GetOutputSize(FileBytes.Length)];
                    int ciphertextLength = cipher.ProcessBytes(memxor ? new byte[FileBytes.Length] : FileBytes, 0, FileBytes.Length, ciphertextBytes, 0);
                    cipher.DoFinal(ciphertextBytes, ciphertextLength);

                    cipher = null;

                    return Task.FromResult(memxor ? Crypt_Decrypt(FileBytes, ciphertextBytes, 16) : ciphertextBytes);
                }
                else
                    LoggerAccessor.LogError("[LIBSECURE] - InitiateAESBuffer - Invalid KeyByes!");

                return Task.FromResult<byte[]>(null);
            }
            finally
            {
                ToolsImplementation.libsecureSema.Release();
            }
        }

        public static string MemXOR(string IV, string block, byte blocksize)
        {
            StringBuilder CryptoBytes = new StringBuilder();

            try
            {
                switch (blocksize)
                {
                    case 2:
                        for (int i = 1; i != 0; --i)
                        {
                            string BlockIV = IV.Substring(0, 4);
                            string CipherBlock = block.Substring(0, 4);
                            IV = IV.Substring(4);
                            block = block.Substring(4);

                            CryptoBytes.Append(((ushort)(Convert.ToUInt16(BlockIV, 16) ^ Convert.ToUInt16(CipherBlock, 16))).ToString("X4").HexStringToByteArray().ToHexString());
                        }
                        break;
                    case 4:
                        for (int i = 2; i != 0; --i)
                        {
                            string BlockIV = IV.Substring(0, 4);
                            string CipherBlock = block.Substring(0, 4);
                            IV = IV.Substring(4);
                            block = block.Substring(4);

                            CryptoBytes.Append(((ushort)(Convert.ToUInt16(BlockIV, 16) ^ Convert.ToUInt16(CipherBlock, 16))).ToString("X4").HexStringToByteArray().ToHexString());
                        }
                        break;
                    case 8:
                        for (int i = 4; i != 0; --i)
                        {
                            string BlockIV = IV.Substring(0, 4);
                            string CipherBlock = block.Substring(0, 4);
                            IV = IV.Substring(4);
                            block = block.Substring(4);

                            CryptoBytes.Append(((ushort)(Convert.ToUInt16(BlockIV, 16) ^ Convert.ToUInt16(CipherBlock, 16))).ToString("X4").HexStringToByteArray().ToHexString());
                        }
                        break;
                    case 16:
                        for (int i = 8; i != 0; --i)
                        {
                            string BlockIV = IV.Substring(0, 4);
                            string CipherBlock = block.Substring(0, 4);
                            IV = IV.Substring(4);
                            block = block.Substring(4);

                            CryptoBytes.Append(((ushort)(Convert.ToUInt16(BlockIV, 16) ^ Convert.ToUInt16(CipherBlock, 16))).ToString("X4").HexStringToByteArray().ToHexString());
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[LIBSECURE] - Error In MemXOR: {ex}");
            }

            return CryptoBytes.ToString();
        }

        public static byte[] Crypt_Decrypt(byte[] fileBytes, byte[] IVA, byte blockSize)
        {
            StringBuilder hexStr = new StringBuilder();
            byte[] CipheredFileBytes = null;
            int totalProcessedBytes = 0;
            int totalBytes = fileBytes.Length;

            while (totalProcessedBytes <= totalBytes)
            {
                int Blksize = Math.Min(blockSize, totalBytes - totalProcessedBytes);

                byte[] ivBlk = new byte[blockSize];
                if (Blksize < blockSize)
                    Array.Copy(IVA, totalProcessedBytes, ivBlk, 0, Blksize);
                else
                    Array.Copy(IVA, totalProcessedBytes, ivBlk, 0, ivBlk.Length);

                byte[] block = new byte[blockSize];
                if (Blksize < blockSize)
                    Array.Copy(fileBytes, totalProcessedBytes, block, 0, Blksize);
                else
                    Array.Copy(fileBytes, totalProcessedBytes, block, 0, block.Length);

                int BytesToFill = blockSize - Blksize;

                if (BytesToFill != 0)
                {
                    byte[] ISO97971 = new byte[BytesToFill];

                    for (int j = 0; j < BytesToFill; j++)
                    {
                        if (j == 0)
                            ISO97971[j] = 0x80;
                        else if (j == BytesToFill - 1)
                            ISO97971[j] = 0x01;
                        else
                            ISO97971[j] = 0x00;
                    }

                    Array.Copy(ISO97971, 0, block, block.Length - BytesToFill, BytesToFill);

                    hexStr.Append(MemXOR(ivBlk.ToHexString(), block.ToHexString(), blockSize).Substring(0, BytesToFill * 2));
                }
                else
                    hexStr.Append(MemXOR(ivBlk.ToHexString(), block.ToHexString(), blockSize));

                totalProcessedBytes += blockSize;
            }

            CipheredFileBytes = hexStr.ToString().HexStringToByteArray();

            hexStr = null;

            if (CipheredFileBytes.Length > fileBytes.Length)
            {
                byte[] ResultTrimmedArray = new byte[fileBytes.Length];
                Buffer.BlockCopy(CipheredFileBytes, 0, ResultTrimmedArray, 0, ResultTrimmedArray.Length);
                return ResultTrimmedArray;
            }
            else if (CipheredFileBytes.Length < fileBytes.Length)
            {
                int difference = fileBytes.Length - CipheredFileBytes.Length;
                byte[] ResultAppendedArray = new byte[fileBytes.Length];

                byte[] ivBlk = new byte[blockSize];
                Array.Copy(IVA, IVA.Length - difference, ivBlk, 0, difference);

                byte[] block = new byte[blockSize];
                Array.Copy(fileBytes, fileBytes.Length - difference, block, 0, difference);

                int BytesToFill = blockSize - difference;

                byte[] ISO97971 = new byte[BytesToFill];

                for (int j = 0; j < BytesToFill; j++)
                {
                    if (j == 0)
                        ISO97971[j] = 0x80;
                    else if (j == BytesToFill - 1)
                        ISO97971[j] = 0x01;
                    else
                        ISO97971[j] = 0x00;
                }

                Array.Copy(ISO97971, 0, block, block.Length - BytesToFill, BytesToFill);

                Buffer.BlockCopy(CipheredFileBytes, 0, ResultAppendedArray, 0, CipheredFileBytes.Length);
                Buffer.BlockCopy(MemXOR(ivBlk.ToHexString(),
                    block.ToHexString(), blockSize).HexStringToByteArray(), 0, ResultAppendedArray, CipheredFileBytes.Length, difference);
                return ResultAppendedArray;
            }

            return CipheredFileBytes;
        }
    }
}
