using System.Text;
using CastleLibrary.Utils;
using CustomLogger;
using EndianTools;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace CastleLibrary.S0ny.LIBSECURE
{
    public static class SymmetricCrypto
    {
        public static void IncrementIVBytes(byte[] byteArray, int increment)
        {
            for (var i = byteArray.Length - 1; i > -1; i--)
            {
                var newValue = byteArray[i] + (byte)increment;
                byteArray[i] = (byte)newValue;
                increment = newValue >> 8; // Carry over the overflow to the next byte
                if (increment == 0)
                    break; // No more overflow, we're done
            }
        }

        public static byte[] ProcessCrypt_Decrypt(
            byte[] inData,
            byte[] KeyBytes,
            byte[] IV,
            byte mode
        )
        {
            return Task.Run(async () =>
            {
                int BlockSize;
                var chunkIndex = 0;
                var inputLength = inData.Length;
                var libsecureResults = new List<KeyValuePair<(int, int), Task<byte[]>>>();

                switch (mode)
                {
                    case 0: // Xtea
                    case 1: // Blowfish
                        BlockSize = 8;
                        break;
                    case 2: // AES
                        BlockSize = 16;
                        break;
                    default:
                        LoggerAccessor.LogError(
                            $"[ToolsImplementation] - ProcessCrypt_Decrypt: unknown crypto mode selected:{mode}."
                        );
                        return null;
                }

                using (var memoryStream = new MemoryStream(inData))
                {
                    while (memoryStream.Position < memoryStream.Length)
                    {
                        Task<byte[]> libsecureResult;
                        var block = new byte[BlockSize];
                        var blockIV = (byte[])IV.Clone();
                        var currentBlockSize = Math.Min(BlockSize, inputLength - chunkIndex);
                        if (currentBlockSize < BlockSize)
                        {
                            var difference = BlockSize - currentBlockSize;
                            Buffer.BlockCopy(
                                new byte[difference],
                                0,
                                block,
                                block.Length - difference,
                                difference
                            );
                        }
                        await memoryStream
                            .ReadAsync(block.AsMemory(0, currentBlockSize))
                            .ConfigureAwait(false);
                        switch (mode)
                        {
                            case 0: // Xtea
                                libsecureResult = InitiateXTEABufferAsync(
                                    block,
                                    KeyBytes,
                                    blockIV,
                                    "CTR"
                                );
                                break;
                            case 1: // Blowfish
                                libsecureResult = InitiateBlowfishBufferAsync(
                                    block,
                                    KeyBytes,
                                    blockIV,
                                    "CTR"
                                );
                                break;
                            default: // AES
                                libsecureResult = InitiateAESBufferAsync(
                                    block,
                                    KeyBytes,
                                    blockIV,
                                    "CTR"
                                );
                                break;
                        }
                        libsecureResults.Add(
                            new KeyValuePair<(int, int), Task<byte[]>>(
                                (chunkIndex, currentBlockSize),
                                libsecureResult
                            )
                        );
                        IncrementIVBytes(IV, 1);
                        chunkIndex += currentBlockSize;
                    }
                }

                using (var memoryStream = new MemoryStream(inData.Length))
                {
                    foreach (var result in libsecureResults.OrderBy(kv => kv.Key.Item1))
                    {
                        var decryptedChunk = await result.Value.ConfigureAwait(false);
                        if (decryptedChunk == null) // We failed.
                            return null;
                        var buffSize = result.Key.Item2;
                        if (decryptedChunk.Length < buffSize)
                            memoryStream.Write(decryptedChunk, 0, decryptedChunk.Length);
                        else
                            memoryStream.Write(decryptedChunk, 0, buffSize);
                    }

                    return memoryStream.ToArray();
                }
            }).Result;
        }

        public static Task<byte[]> InitiateXTEABufferAsync(
            byte[] FileBytes,
            byte[] KeyBytes,
            byte[] m_iv,
            string mode,
            bool memxor = true,
            bool encrypt = false
        )
        {
            if (KeyBytes.Length == 16)
            {
                // Create the cipher
                var cipher = CipherUtilities.GetCipher($"LIBSECUREXTEA/{mode}/NOPADDING");

                if (mode == "CTR" || mode == "CBC")
                {
                    if (m_iv == null || m_iv.Length != 8)
                    {
                        LoggerAccessor.LogError(
                            "[SymmetricCrypto] - InitiateXTEABuffer - Invalid IV!"
                        );
                        return Task.FromResult<byte[]>(null);
                    }

                    cipher.Init(
                        encrypt,
                        new ParametersWithIV(
                            new KeyParameter(EndianUtils.EndianSwap(KeyBytes)),
                            EndianUtils.EndianSwap(m_iv)
                        )
                    );
                }
                else
                    cipher.Init(encrypt, new KeyParameter(EndianUtils.EndianSwap(KeyBytes)));

                // Encrypt the plaintext
                var ciphertextBytes = new byte[cipher.GetOutputSize(FileBytes.Length)];
                var ciphertextLength = cipher.ProcessBytes(
                    memxor ? new byte[FileBytes.Length] : EndianUtils.EndianSwap(FileBytes),
                    0,
                    FileBytes.Length,
                    ciphertextBytes,
                    0
                );
                cipher.DoFinal(ciphertextBytes, ciphertextLength);

                return Task.FromResult(
                    memxor
                        ? Crypt_Decrypt(FileBytes, EndianUtils.EndianSwap(ciphertextBytes), 8)
                        : EndianUtils.EndianSwap(ciphertextBytes)
                );
            }
            else
                LoggerAccessor.LogError(
                    "[SymmetricCrypto] - InitiateXTEABuffer - Invalid KeyByes!"
                );

            return Task.FromResult<byte[]>(null);
        }

        public static Task<byte[]> InitiateBlowfishBufferAsync(
            byte[] FileBytes,
            byte[] KeyBytes,
            byte[] m_iv,
            string mode,
            bool memxor = true,
            bool encrypt = false
        )
        {
            if (KeyBytes.Length == 32)
            {
                // Create the cipher
                var cipher = CipherUtilities.GetCipher($"Blowfish/{mode}/NOPADDING");

                if (mode == "CTR" || mode == "CBC")
                {
                    if (m_iv == null || m_iv.Length != 8)
                    {
                        LoggerAccessor.LogError(
                            "[SymmetricCrypto] - InitiateBlowfishBuffer - Invalid IV!"
                        );
                        return Task.FromResult<byte[]>(null);
                    }

                    cipher.Init(encrypt, new ParametersWithIV(new KeyParameter(KeyBytes), m_iv));
                }
                else
                    cipher.Init(encrypt, new KeyParameter(KeyBytes));

                // Encrypt the plaintext
                var ciphertextBytes = new byte[cipher.GetOutputSize(FileBytes.Length)];
                var ciphertextLength = cipher.ProcessBytes(
                    memxor ? new byte[FileBytes.Length] : FileBytes,
                    0,
                    FileBytes.Length,
                    ciphertextBytes,
                    0
                );
                cipher.DoFinal(ciphertextBytes, ciphertextLength);

                return Task.FromResult(
                    memxor ? Crypt_Decrypt(FileBytes, ciphertextBytes, 8) : ciphertextBytes
                );
            }
            else
                LoggerAccessor.LogError(
                    "[SymmetricCrypto] - InitiateBlowfishBuffer - Invalid KeyByes!"
                );

            return Task.FromResult<byte[]>(null);
        }

        public static Task<byte[]> InitiateAESBufferAsync(
            byte[] FileBytes,
            byte[] KeyBytes,
            byte[] m_iv,
            string mode,
            bool memxor = true,
            bool encrypt = false
        )
        {
            if (KeyBytes.Length >= 16)
            {
                // Create the cipher
                var cipher = CipherUtilities.GetCipher($"AES/{mode}/NOPADDING");

                if (mode == "CTR" || mode == "CBC")
                {
                    if (m_iv == null || m_iv.Length != 16)
                    {
                        LoggerAccessor.LogError(
                            "[SymmetricCrypto] - InitiateAESBuffer - Invalid IV!"
                        );
                        return Task.FromResult<byte[]>(null);
                    }

                    cipher.Init(encrypt, new ParametersWithIV(new KeyParameter(KeyBytes), m_iv));
                }
                else
                    cipher.Init(encrypt, new KeyParameter(KeyBytes));

                // Encrypt the plaintext
                var ciphertextBytes = new byte[cipher.GetOutputSize(FileBytes.Length)];
                var ciphertextLength = cipher.ProcessBytes(
                    memxor ? new byte[FileBytes.Length] : FileBytes,
                    0,
                    FileBytes.Length,
                    ciphertextBytes,
                    0
                );
                cipher.DoFinal(ciphertextBytes, ciphertextLength);

                return Task.FromResult(
                    memxor ? Crypt_Decrypt(FileBytes, ciphertextBytes, 16) : ciphertextBytes
                );
            }
            else
                LoggerAccessor.LogError("[SymmetricCrypto] - InitiateAESBuffer - Invalid KeyByes!");

            return Task.FromResult<byte[]>(null);
        }

        public static byte[] Crypt_Decrypt(byte[] fileBytes, byte[] IVA, int blockSize)
        {
            var hexStr = new StringBuilder();
            byte[] CipheredFileBytes = null;
            var totalProcessedBytes = 0;
            var totalBytes = fileBytes.Length;

            while (totalProcessedBytes <= totalBytes)
            {
                var Blksize = Math.Min(blockSize, totalBytes - totalProcessedBytes);

                var ivBlk = new byte[blockSize];
                if (Blksize < blockSize)
                    Array.Copy(IVA, totalProcessedBytes, ivBlk, 0, Blksize);
                else
                    Array.Copy(IVA, totalProcessedBytes, ivBlk, 0, ivBlk.Length);

                var block = new byte[blockSize];
                if (Blksize < blockSize)
                    Array.Copy(fileBytes, totalProcessedBytes, block, 0, Blksize);
                else
                    Array.Copy(fileBytes, totalProcessedBytes, block, 0, block.Length);

                var BytesToFill = blockSize - Blksize;

                if (BytesToFill != 0)
                {
                    var ISO97971 = new byte[BytesToFill];

                    for (var j = 0; j < BytesToFill; j++)
                    {
                        ISO97971[j] =
                            j == 0 ? (byte)0x80
                            : j == BytesToFill - 1 ? (byte)0x01
                            : (byte)0x00;
                    }

                    Array.Copy(ISO97971, 0, block, block.Length - BytesToFill, BytesToFill);

                    hexStr.Append(
                        MemXOR(ivBlk.BytesToHexStr(), block.BytesToHexStr(), blockSize)
                            .AsSpan(0, BytesToFill * 2)
                    );
                }
                else
                    hexStr.Append(MemXOR(ivBlk.BytesToHexStr(), block.BytesToHexStr(), blockSize));

                totalProcessedBytes += blockSize;
            }

            CipheredFileBytes = hexStr.ToString().HexStrToBytes();

            if (CipheredFileBytes.Length > fileBytes.Length)
            {
                var ResultTrimmedArray = new byte[fileBytes.Length];
                Array.Copy(CipheredFileBytes, 0, ResultTrimmedArray, 0, ResultTrimmedArray.Length);
                return ResultTrimmedArray;
            }
            else if (CipheredFileBytes.Length < fileBytes.Length)
            {
                var difference = fileBytes.Length - CipheredFileBytes.Length;
                var ResultAppendedArray = new byte[fileBytes.Length];

                var ivBlk = new byte[blockSize];
                Array.Copy(IVA, IVA.Length - difference, ivBlk, 0, difference);

                var block = new byte[blockSize];
                Array.Copy(fileBytes, fileBytes.Length - difference, block, 0, difference);

                var BytesToFill = blockSize - difference;

                var ISO97971 = new byte[BytesToFill];

                for (var j = 0; j < BytesToFill; j++)
                {
                    ISO97971[j] =
                        j == 0 ? (byte)0x80
                        : j == BytesToFill - 1 ? (byte)0x01
                        : (byte)0x00;
                }

                Array.Copy(ISO97971, 0, block, block.Length - BytesToFill, BytesToFill);
                Array.Copy(CipheredFileBytes, 0, ResultAppendedArray, 0, CipheredFileBytes.Length);
                Array.Copy(
                    MemXOR(ivBlk.BytesToHexStr(), block.BytesToHexStr(), blockSize).HexStrToBytes(),
                    0,
                    ResultAppendedArray,
                    CipheredFileBytes.Length,
                    difference
                );

                return ResultAppendedArray;
            }

            return CipheredFileBytes;
        }

        private static string MemXOR(string IV, string block, int blocksize)
        {
            var CryptoBytes = new StringBuilder();

            try
            {
                for (var i = blocksize / 2; i != 0; --i)
                {
                    var BlockIV = IV.Substring(0, 4);
                    var CipherBlock = block.Substring(0, 4);
                    IV = IV.Substring(4);
                    block = block.Substring(4);

                    CryptoBytes.Append(
                        (
                            (ushort)(
                                Convert.ToUInt16(BlockIV, 16) ^ Convert.ToUInt16(CipherBlock, 16)
                            )
                        ).ToString("X4")
                    );
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[SymmetricCrypto] - Error In MemXOR: {ex}");
            }

            return CryptoBytes.ToString();
        }
    }
}
