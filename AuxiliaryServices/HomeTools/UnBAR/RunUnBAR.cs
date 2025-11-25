using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using MultiServerLibrary.Extension;
using HomeTools.BARFramework;
using HomeTools.Crypto;
using HomeTools.PS3_Creator;
using EndianTools;
using CustomLogger;
using System.Collections.Generic;
using System.Text;
using CastleLibrary.Sony.Edge;

namespace HomeTools.UnBAR
{
    public static class RunUnBAR
    {
        public static async Task Run(string converterPath, string filePath, string outputpath, bool edat, ushort cdnMode)
        {
            if (edat)
                await RunDecrypt(converterPath, filePath, outputpath, cdnMode);
            else
                await RunExtract(filePath, outputpath, cdnMode);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect(); // We have no choice and it's not possible to remove them, HomeTools create a BUNCH of necessary objects.
        }

        public static void RunEncrypt(string filePath, string sdatfilePath)
        {
            try
            {
                int ExitCode = new EDAT().encryptFile(filePath, sdatfilePath, new byte[16], null, new byte[48], "0C".HexStringToByteArray(), "00".HexStringToByteArray(), "03".HexStringToByteArray());

                if (ExitCode != 0)
                    LoggerAccessor.LogError($"[RunUnBAR] - RunEncrypt failed with status code : {ExitCode}");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[RunUnBAR] - RunEncrypt failed with assertion : {ex}");
            }
        }

        private static async Task RunDecrypt(string converterPath, string sdatfilePath, string outDir, ushort cdnMode)
        {
            string datfilePath = Path.Combine(outDir, Path.GetFileNameWithoutExtension(sdatfilePath) + ".dat");

            try
            {
                int ExitCode = new EDAT().decryptFile(sdatfilePath, datfilePath, new byte[16], null);

                if (ExitCode != 0)
                    LoggerAccessor.LogError($"[RunUnBAR] - RunDecrypt failed with status code : {ExitCode}");
                else
                    await RunExtract(datfilePath, outDir, cdnMode);
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[RunUnBAR] - RunDecrypt failed with assertion : {ex}");
            }
        }

        private static async Task RunExtract(string filePath, string outDir, ushort cdnMode)
        {
            bool isSharc = false;
            bool isLittleEndian = false;
            string options = ToolsImplementation.base64CDNKey2;
            byte[] RawBarData = null;

            if (File.Exists(filePath))
            {
                string barDirectoryPath = Path.Combine(outDir, Path.GetFileNameWithoutExtension(filePath));

                try
                {
                    RawBarData = File.ReadAllBytes(filePath);

                    if (RawBarData.Length < 12)
                        return; // File not a BAR.
                    else
                    {
                        if (RawBarData[0] == 0xAD && RawBarData[1] == 0xEF && RawBarData[2] == 0x17 && RawBarData[3] == 0xE1)
                        {

                        }
                        else if (RawBarData[0] == 0xE1 && RawBarData[1] == 0x17 && RawBarData[2] == 0xEF && RawBarData[3] == 0xAD)
                            isLittleEndian = true;
                        else
                            return; // File not a BAR.

                        switch (isLittleEndian)
                        {
                            case true:
                                if (RawBarData[4] == 0x00 && RawBarData[5] == 0x00 && RawBarData[6] == 0x00 && RawBarData[7] == 0x02)
                                    isSharc = true;
                                break;
                            default:
                                if (RawBarData[4] == 0x02 && RawBarData[5] == 0x00 && RawBarData[6] == 0x00 && RawBarData[7] == 0x00)
                                    isSharc = true;
                                break;
                        }
                    }

                    if (isSharc && RawBarData.Length > 52)
                    {
                        try
                        {
                            var base64Data = options.IsBase64();

                            if (!base64Data.Item1)
                                throw new InvalidDataException("[RunUnBAR] - options is expected to be of base64 type.");

                            byte[] HeaderIV = new byte[16];

                            Buffer.BlockCopy(RawBarData, 8, HeaderIV, 0, HeaderIV.Length);

                            if (HeaderIV != null)
                            {
                                byte[] EmptyArray = new byte[4];
                                byte[] SharcHeader = new byte[28];

                                Buffer.BlockCopy(RawBarData, 24, SharcHeader, 0, SharcHeader.Length);

                                SharcHeader = ToolsImplementation.ProcessCrypt_Decrypt(SharcHeader,
                                 base64Data.Item2, HeaderIV.ShadowCopy(), 2);

                                if (SharcHeader == null)
                                    return; // Sharc Header failed to decrypt.
                                else if (!(new byte[] { SharcHeader[0], SharcHeader[1], SharcHeader[2], SharcHeader[3] }).EqualsTo(EmptyArray))
                                {
                                    options = ToolsImplementation.base64CDNKey1;

                                    Buffer.BlockCopy(RawBarData, 24, SharcHeader, 0, SharcHeader.Length);

                                    SharcHeader = ToolsImplementation.ProcessCrypt_Decrypt(SharcHeader,
                                     base64Data.Item2, HeaderIV.ShadowCopy(), 2);

                                    if (SharcHeader == null)
                                        return; // Sharc Header failed to decrypt.
                                    else if (!(new byte[] { SharcHeader[0], SharcHeader[1], SharcHeader[2], SharcHeader[3] }).EqualsTo(EmptyArray))
                                    {
                                        options = ToolsImplementation.base64DefaultSharcKey;

                                        Buffer.BlockCopy(RawBarData, 24, SharcHeader, 0, SharcHeader.Length);

                                        SharcHeader = ToolsImplementation.ProcessCrypt_Decrypt(SharcHeader,
                                         base64Data.Item2, HeaderIV.ShadowCopy(), 2);

                                        if (SharcHeader == null)
                                            return; // Sharc Header failed to decrypt.
                                        else if (!(new byte[] { SharcHeader[0], SharcHeader[1], SharcHeader[2], SharcHeader[3] }).EqualsTo(EmptyArray))
                                            return; // All keys failed to decrypt.
                                    }
                                }

                                byte[] NumOfFiles = new byte[4];

                                if (isLittleEndian)
                                    Buffer.BlockCopy(SharcHeader, SharcHeader.Length - 20, NumOfFiles, 0, NumOfFiles.Length);
                                else
                                    Buffer.BlockCopy(EndianUtils.EndianSwap(SharcHeader), SharcHeader.Length - 20, NumOfFiles, 0, NumOfFiles.Length);

                                if (!BitConverter.IsLittleEndian)
                                    Array.Reverse(NumOfFiles);

                                byte[] SharcTOC = new byte[24 * BitConverter.ToUInt32(NumOfFiles, 0)];

                                Buffer.BlockCopy(RawBarData, 52, SharcTOC, 0, SharcTOC.Length);

                                if (SharcTOC != null)
                                {
                                    byte[] OriginalIV = new byte[HeaderIV.Length];

                                    Buffer.BlockCopy(HeaderIV, 0, OriginalIV, 0, OriginalIV.Length);

                                    ToolsImplementation.IncrementIVBytes(HeaderIV, 1); // Increment IV by one (supposed to be the continuation of the header cypher context).

                                    SharcTOC = ToolsImplementation.ProcessCrypt_Decrypt(SharcTOC, base64Data.Item2, HeaderIV, 2);

                                    if (SharcTOC != null)
                                    {
                                        byte[] SharcData = new byte[RawBarData.Length - (52 + SharcTOC.Length)];

                                        Buffer.BlockCopy(RawBarData, 52 + SharcTOC.Length, SharcData, 0, SharcData.Length);

                                        byte[] FileBytes = Array.Empty<byte>();

                                        if (isLittleEndian)
                                        {
                                            FileBytes = ByteUtils.CombineByteArrays(new byte[] { 0xE1, 0x17, 0xEF, 0xAD, 0x00, 0x00, 0x00, 0x02 }, new byte[][]
                                            {
                                                    OriginalIV,
                                                    SharcHeader,
                                                    SharcTOC,
                                                    SharcData
                                            });
                                        }
                                        else
                                        {
                                            FileBytes = ByteUtils.CombineByteArrays(new byte[] { 0xAD, 0xEF, 0x17, 0xE1, 0x02, 0x00, 0x00, 0x00 }, new byte[][]
                                            {
                                                    OriginalIV,
                                                    SharcHeader,
                                                    SharcTOC,
                                                    SharcData
                                            });
                                        }

                                        Directory.CreateDirectory(barDirectoryPath);

                                        File.WriteAllBytes(filePath, FileBytes);

                                        LoggerAccessor.LogInfo("Loading SHARC/dat: {0}", filePath);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LoggerAccessor.LogError($"[RunUnBAR] - SHARC Decryption failed! with error - {ex}");
                        }
                    }
                    else
                    {
                        Directory.CreateDirectory(barDirectoryPath);

                        LoggerAccessor.LogInfo("Loading BAR/dat: {0}", filePath);
                    }
                }
                catch (Exception ex)
                {
                    LoggerAccessor.LogError($"[RunUnBAR] - Initial archive loading failed with error - {ex}");
                }

                if (Directory.Exists(barDirectoryPath))
                {
                    try
                    {
                        BARArchive archive = new BARArchive(filePath, outDir);
                        archive.Load();
                        //archive.WriteMap(filePath);
                        File.WriteAllText(barDirectoryPath + "/timestamp.txt", archive.BARHeader.UserData.ToString("X"));
#if NET6_0_OR_GREATER
                        await Parallel.ForEachAsync(
                        archive.TableOfContents.Cast<TOCEntry>(),
                        new ParallelOptions { MaxDegreeOfParallelism = Utils.ThreadLimiter.NumOfThreadsAvailable },
                        async (tableOfContent, cancellationToken) =>
                        {
                            byte[] FileData = tableOfContent.GetData(archive.GetHeader().Flags);

                            try
                            {
                                if (archive.GetHeader().Version == 512)
                                    await ExtractToFileBarVersion2(archive.GetHeader().Key, FileData, archive, tableOfContent.FileName, barDirectoryPath).ConfigureAwait(false);
                                else
                                    await ExtractToFileBarVersion1(RawBarData, FileData, archive, tableOfContent.FileName, barDirectoryPath, cdnMode).ConfigureAwait(false);
                            }
                            catch (Exception ex)
                            {
                                LoggerAccessor.LogError($"[RunUnBAR] - RunExtract Errored out on file:{tableOfContent.FileName} (Exception: {ex})");
                            }
                        }).ConfigureAwait(false);
#elif NETCOREAPP || NETSTANDARD1_0_OR_GREATER || NET40_OR_GREATER
                        // Process Environment.ProcessorCount patherns at a time, removing the limit is not tolerable as CPU usage can go high.
                        Parallel.ForEach(archive.TableOfContents.Cast<TOCEntry>(), new ParallelOptions { MaxDegreeOfParallelism = Utils.ThreadLimiter.NumOfThreadsAvailable }, async tableOfContent =>
                        {
                            byte[] FileData = tableOfContent.GetData(archive.GetHeader().Flags);

                            try
                            {
                                if (archive.GetHeader().Version == 512)
                                    await ExtractToFileBarVersion2(archive.GetHeader().Key, FileData, archive, tableOfContent.FileName, barDirectoryPath).ConfigureAwait(false);
                                else
                                    await ExtractToFileBarVersion1(RawBarData, FileData, archive, tableOfContent.FileName, barDirectoryPath, cdnMode).ConfigureAwait(false);
                            }
                            catch (Exception ex)
                            {
                                LoggerAccessor.LogError($"[RunUnBAR] - RunExtract Errored out on file:{tableOfContent.FileName} (Exception: {ex})");
                            }
                        });
#else
                        foreach (TOCEntry tableOfContent in archive.TableOfContents)
                        {
                            byte[] FileData = tableOfContent.GetData(archive.GetHeader().Flags);

                            try
                            {
                                if (archive.GetHeader().Version == 512)
                                    await ExtractToFileBarVersion2(archive.GetHeader().Key, FileData, archive, tableOfContent.FileName, barDirectoryPath).ConfigureAwait(false);
                                else
                                    await ExtractToFileBarVersion1(RawBarData, FileData, archive, tableOfContent.FileName, barDirectoryPath, cdnMode).ConfigureAwait(false);
                            }
                            catch (Exception ex)
                            {
                                LoggerAccessor.LogError($"[RunUnBAR] - RunExtract Errored out on file:{tableOfContent.FileName} (Exception: {ex})");
                            }
                        }
#endif
                        if (File.Exists(filePath + ".map"))
                            File.Move(filePath + ".map", barDirectoryPath + $"/{Path.GetFileName(filePath)}.map");
                        else if (filePath.Length > 4 && File.Exists(string.Concat(filePath.AsSpan(0, filePath.Length - 4), ".sharc.map")))
                            File.Move(string.Concat(filePath.AsSpan(0, filePath.Length - 4), ".sharc.map"), barDirectoryPath + $"/{Path.GetFileName(filePath)}.map");
                        else if (filePath.Length > 4 && File.Exists(string.Concat(filePath.AsSpan(0, filePath.Length - 4), ".bar.map")))
                            File.Move(string.Concat(filePath.AsSpan(0, filePath.Length - 4), ".bar.map"), barDirectoryPath + $"/{Path.GetFileName(filePath)}.map");
                    }
                    catch (Exception ex)
                    {
                        LoggerAccessor.LogError($"[RunUnBAR] - RunExtract Errored out - {ex}");
                    }
                }
            }

            return;
        }

        private static Task ExtractToFileBarVersion1(byte[] RawBarData, byte[] data, BARArchive archive, HashedFileName FileName, string outDir, int cdnMode)
        {
            TOCEntry tableOfContent = archive.TableOfContents[FileName];
            string path = null;
            if (tableOfContent.Compression == CompressionMethod.Encrypted)
            {
                if (data.Length > ToolsImplementation.sizeOfVersionHeader)
                {
                    byte[] cryptoVersion = new byte[ToolsImplementation.sizeOfVersionHeader];

                    Array.Copy(data, 0, cryptoVersion, 0, cryptoVersion.Length);

                    if (cryptoVersion.EqualsTo(ToolsImplementation.CryptoVersionBytesBE) || cryptoVersion.EqualsTo(EndianUtils.EndianSwap(ToolsImplementation.CryptoVersionBytesBE)))
                    {
                        int dataStart = FindDataPositionInBinary(RawBarData, data);

                        if (dataStart != -1)
                        {
                            uint compressedSize = tableOfContent.CompressedSize;
                            uint fileSize = tableOfContent.Size;
                            int userData = archive.BARHeader.UserData;
                            byte[] EncryptedSignatureHeader = new byte[24];
#if DEBUG
                            LoggerAccessor.LogInfo("[RunUnBAR] - Encrypted Content Detected!, Running Decryption.");
                            LoggerAccessor.LogInfo($"CompressedSize - {compressedSize}");
                            LoggerAccessor.LogInfo($"Size - {fileSize}");
                            LoggerAccessor.LogInfo($"dataStart - 0x{dataStart:X}");
                            LoggerAccessor.LogInfo($"UserData - 0x{userData:X}");
#endif
                            byte[] SignatureIV = BitConverter.GetBytes(ToolsImplementation.BuildSignatureIv((int)fileSize, (int)compressedSize, dataStart, userData));

                            if (BitConverter.IsLittleEndian)
                                Array.Reverse(SignatureIV);

                            // Copy the first 24 bytes from the source array to the destination array
                            Buffer.BlockCopy(data, 4, EncryptedSignatureHeader, 0, EncryptedSignatureHeader.Length);

                            byte[] DecryptedSignatureHeader;

                            switch (cdnMode)
                            {
                                case 2:
                                    DecryptedSignatureHeader = ToolsImplementation.ProcessCrypt_Decrypt(EncryptedSignatureHeader, ToolsImplementation.HDKSignatureKey, SignatureIV, 1);
                                    break;
                                case 1:
                                    DecryptedSignatureHeader = ToolsImplementation.ProcessCrypt_Decrypt(EncryptedSignatureHeader, ToolsImplementation.BetaSignatureKey, SignatureIV, 1);
                                    break;
                                default:
                                    DecryptedSignatureHeader = ToolsImplementation.ProcessCrypt_Decrypt(EncryptedSignatureHeader, ToolsImplementation.SignatureKey, SignatureIV, 1);
                                    break;
                            }

                            if (DecryptedSignatureHeader != null)
                            {
                                string SignatureHeaderHexString = DecryptedSignatureHeader.ToHexString();

                                // Create a new byte array to store the remaining content
                                byte[] FileBytes = new byte[data.Length - 28];

                                // Copy the content after the first 28 bytes to the new array
                                Array.Copy(data, 28, FileBytes, 0, FileBytes.Length);

                                string SHA1HexString = NetHasher.DotNetHasher.ComputeSHA1String(FileBytes);

                                if (string.Equals(SHA1HexString, SignatureHeaderHexString.Substring(0, SignatureHeaderHexString.Length - 8))) // We strip the original file Compression size.
                                {
                                    if (tableOfContent.Size == 0) // The original Encryption Proxy seemed to only check for "lua" or "scene" file types, regardless if empty or not.
                                    {
                                        path = string.Format("{0}{1}{2:X8}{3}", outDir, Path.DirectorySeparatorChar, FileName.Value, ".unknown").ToUpper();

                                        string outdirectory = Path.GetDirectoryName(path);
                                        if (!string.IsNullOrEmpty(outdirectory))
                                        {
                                            Directory.CreateDirectory(outdirectory);

                                            using (FileStream fileStream = File.Open(path, (FileMode)2))
                                            {
                                                fileStream.Write(FileBytes, 0, FileBytes.Length);
                                                fileStream.Close();
                                            }
                                        }
#if DEBUG
                                        LoggerAccessor.LogInfo("Extracted file {0}", new object[1]
                                        {
                                    Path.GetFileName(path)
                                        });
#endif
                                        tableOfContent = null;

                                        return Task.CompletedTask;
                                    }
                                    else
                                    {
                                        switch (cdnMode)
                                        {
                                            case 2:
                                                FileBytes = ToolsImplementation.ProcessCrypt_Decrypt(FileBytes, ToolsImplementation.HDKBlowfishKey, SignatureIV, 1);
                                                break;
                                            case 1:
                                                FileBytes = ToolsImplementation.ProcessCrypt_Decrypt(FileBytes, ToolsImplementation.BetaBlowfishKey, SignatureIV, 1);
                                                break;
                                            default:
                                                FileBytes = ToolsImplementation.ProcessCrypt_Decrypt(FileBytes, ToolsImplementation.BlowfishKey, SignatureIV, 1);
                                                break;
                                        }

                                        if (FileBytes != null)
                                        {
                                            try
                                            {
                                                FileBytes = Zlib.EdgeZlibDecompress(FileBytes);
                                            }
                                            catch (Exception ex)
                                            {
                                                LoggerAccessor.LogError($"[RunUnBar] - Errored out when processing Encryption Proxy encrypted content - {ex}");

                                                FileBytes = data;
                                            }

                                            using (MemoryStream memoryStream = new MemoryStream(FileBytes))
                                            {
                                                string registeredExtension = string.Empty;

                                                try
                                                {
                                                    registeredExtension = FileTypeAnalyser.Instance.GetRegisteredExtension(FileTypeAnalyser.Instance.Analyse(memoryStream));
                                                }
                                                catch
                                                {
                                                    registeredExtension = ".unknown";
                                                }

                                                path = string.Format("{0}{1}{2:X8}{3}", outDir, Path.DirectorySeparatorChar, FileName.Value, registeredExtension).ToUpper();

                                                string outdirectory = Path.GetDirectoryName(path);
                                                if (!string.IsNullOrEmpty(outdirectory))
                                                {
                                                    Directory.CreateDirectory(outdirectory);

                                                    using (FileStream fileStream = File.Open(path, (FileMode)2))
                                                    {
                                                        fileStream.Write(FileBytes, 0, FileBytes.Length);
                                                        fileStream.Close();
                                                    }
                                                }

                                                memoryStream.Flush();
                                            }
#if DEBUG
                                            LoggerAccessor.LogInfo("Extracted file {0}", new object[1]
                                            {
                                        Path.GetFileName(path)
                                            });
#endif
                                            tableOfContent = null;

                                            return Task.CompletedTask;
                                        }
                                        else
                                            LoggerAccessor.LogError($"[RunUnBAR] - Encrypted file failed to decrypt, Writing original data.");
                                    }
                                }
                                else
                                    LoggerAccessor.LogError($"[RunUnBAR] - Encrypted file (SHA1 - {SHA1HexString}) has been tempered with! (Reference SHA1 - {SignatureHeaderHexString.Substring(0, SignatureHeaderHexString.Length - 8)}), Aborting decryption.");
                            }
                            else
                                LoggerAccessor.LogError("[RunUnBAR] - Encrypted data SignatureHeader Decryption has failed.");
                        }
                        else
                            LoggerAccessor.LogError("[RunUnBAR] - Encrypted data not found in BAR or false positive! Decryption has failed.");
                    }
                    else
                        LoggerAccessor.LogWarn($"[RunUnBAR] - Unknown crypto version being used ({BitConverter.ToString(cryptoVersion)}). Decryption has failed.");
                }
                else
                    LoggerAccessor.LogError("[RunUnBAR] - Encrypted data is malformed! Decryption has failed.");
            }

            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                string registeredExtension = string.Empty;

                try
                {
                    registeredExtension = FileTypeAnalyser.Instance.GetRegisteredExtension(FileTypeAnalyser.Instance.Analyse(memoryStream));
                }
                catch
                {
                    registeredExtension = ".unknown";
                }

                path = string.Format("{0}{1}{2:X8}{3}", outDir, Path.DirectorySeparatorChar, FileName.Value, registeredExtension).ToUpper();

                string outdirectory = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(outdirectory))
                {
                    Directory.CreateDirectory(outdirectory);

                    using (FileStream fileStream = File.Open(path, (FileMode)2))
                    {
                        fileStream.Write(data, 0, data.Length);
                        fileStream.Close();
                    }
                }

                memoryStream.Flush();
            }
#if DEBUG
            LoggerAccessor.LogInfo("Extracted file {0}", new object[1]
            {
                    Path.GetFileName(path)
            });
#endif
            tableOfContent = null;

            return Task.CompletedTask;
        }

        private static Task ExtractToFileBarVersion2(byte[] Key, byte[] data, BARArchive archive, HashedFileName FileName, string outDir)
        {
            TOCEntry tableOfContent = archive.TableOfContents[FileName];
            string path = null;
            if (tableOfContent.Compression == CompressionMethod.Encrypted)
            {
#if DEBUG
                LoggerAccessor.LogInfo("[RunUnBAR] - Encrypted Content Detected!, Running Decryption.");
                LoggerAccessor.LogInfo($"Key - {Key.ToHexString()}");
                LoggerAccessor.LogInfo($"IV - {tableOfContent.IV.ToHexString()}");
#endif

                byte[] FileBytes = ToolsImplementation.ProcessCrypt_Decrypt(data, Key, tableOfContent.IV.ShadowCopy(), 0);

                try
                {
                    FileBytes = Zlib.EdgeZlibDecompress(FileBytes);
                }
                catch (Exception ex)
                {
                    LoggerAccessor.LogError($"[RunUnBar] - Errored out when processing XTEA Proxy encrypted content - {ex}");

                    FileBytes = data;
                }

                using (MemoryStream memoryStream = new MemoryStream(FileBytes))
                {
                    string registeredExtension = string.Empty;

                    try
                    {
                        registeredExtension = FileTypeAnalyser.Instance.GetRegisteredExtension(FileTypeAnalyser.Instance.Analyse(memoryStream));
                    }
                    catch
                    {
                        registeredExtension = ".unknown";
                    }

                    path = string.Format("{0}{1}{2:X8}{3}", outDir, Path.DirectorySeparatorChar, FileName.Value, registeredExtension).ToUpper();

                    string outdirectory = Path.GetDirectoryName(path);
                    if (!string.IsNullOrEmpty(outdirectory))
                    {
                        Directory.CreateDirectory(outdirectory);

                        using (FileStream fileStream = File.Open(path, (FileMode)2))
                        {
                            fileStream.Write(FileBytes, 0, FileBytes.Length);
                            fileStream.Close();
                        }
                    }

                    memoryStream.Flush();
                }
            }
            else
            {
                using (MemoryStream memoryStream = new MemoryStream(data))
                {
                    string registeredExtension = string.Empty;

                    try
                    {
                        registeredExtension = FileTypeAnalyser.Instance.GetRegisteredExtension(FileTypeAnalyser.Instance.Analyse(memoryStream));
                    }
                    catch
                    {
                        registeredExtension = ".unknown";
                    }

                    path = string.Format("{0}{1}{2:X8}{3}", outDir, Path.DirectorySeparatorChar, FileName.Value, registeredExtension).ToUpper();

                    string outdirectory = Path.GetDirectoryName(path);
                    if (!string.IsNullOrEmpty(outdirectory))
                    {
                        Directory.CreateDirectory(outdirectory);

                        using (FileStream fileStream = File.Open(path, (FileMode)2))
                        {
                            fileStream.Write(data, 0, data.Length);
                            fileStream.Close();
                        }
                    }

                    memoryStream.Flush();
                }
            }
#if DEBUG
            LoggerAccessor.LogInfo("Extracted file {0}", new object[1]
            {
                Path.GetFileName(path)
            });
#endif
            tableOfContent = null;

            return Task.CompletedTask;
        }


        /// <summary>
        /// Finds a matching byte array within an other byte array.
        /// <para>Trouve un tableau de bytes correspondant dans un autre tableau de bytes.</para>
        /// </summary>
        /// <param name="data1">The data to search for.</param>
        /// <param name="data2">The data to search into for the data1.</param>
        /// <returns>A int (-1 if not found).</returns>
        private static int FindDataPositionInBinary(byte[] data1, byte[] data2)
        {
            if (data1 == null || data2 == null)
                return -1;

            IEnumerator<int> matches = new BoyerMoore(data2).BCLMatch(data1, 0).GetEnumerator();
            if (matches.MoveNext())
                return matches.Current;

            return -1; // Data2 not found in Data1
        }
    }
}
