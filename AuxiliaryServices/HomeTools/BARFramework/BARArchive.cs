using HomeTools.Crypto;
using EndianTools;
using CustomLogger;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System;
using MultiServerLibrary.Extension;
using HomeTools.AFS;
using MultiServerLibrary.HTTP;
using EndianTools.BinaryExtension;
using CastleLibrary.Sony.Edge;
#if NET6_0_OR_GREATER
using MultiServerLibrary.Upscalers;
#endif
namespace HomeTools.BARFramework
{
    public class BARArchive
    {
        private Stream GetDataWriterStream(string fileName)
        {
            if (m_outputStream != null)
                return m_outputStream;
            return File.Open(fileName, FileMode.Create);
        }

        private Stream GetDataReadStream()
        {
            if (m_sourceStream != null)
                return m_sourceStream;
            return File.OpenRead(m_sourceFile);
        }

        public Header GetHeader()
        {
            return m_header;
        }

        private bool encrypt = false;

        private bool optimizeassets = false;

        private ushort cdnMode = 0;

        private string version2key = string.Empty;

        private string convertersfolder = null;
#if NET6_0_OR_GREATER
        private string m_imparams = ImageOptimizer.defaultOptimizerParams;
#endif
        public BARArchive()
        {
            m_toc = new TOC(this);
            m_deletedFileSection = new Dictionary<HashedFileName, TOCEntry>();
            m_sourceFile = string.Empty;
            m_endian = Endianness.LittleEndian;
            m_allowWhitespaceInFilenames = true;
        }

        public BARArchive(string ConvertersFolder, string sourceFilePath, string resourceRoot, ushort cdnMode = 0, int UserData = 0, bool encrypt = false, bool bigendian = false, string version2key = "", bool optimizeassets = false) : this()
        {
            convertersfolder = ConvertersFolder;
            m_sourceFile = sourceFilePath;
            m_resourceRoot = resourceRoot;
            this.cdnMode = cdnMode;
            m_header.UserData = UserData;
            if (bigendian)
                m_endian = Endianness.BigEndian;
            if (encrypt)
                this.encrypt = true;
            if (!string.IsNullOrEmpty(version2key))
            {
                this.version2key = version2key;
                m_header.Version = 512;
            }
#if NET6_0_OR_GREATER
            this.optimizeassets = optimizeassets;
#endif
        }

        public bool Dirty
        {
            get
            {
                return m_dirty;
            }
            set
            {
                m_dirty = value;
            }
        }

        public bool AllowWhitespaceInFilenames
        {
            get
            {
                return m_allowWhitespaceInFilenames;
            }
            set
            {
                m_allowWhitespaceInFilenames = value;
            }
        }
#if NET6_0_OR_GREATER
        public string ImageMagickParams
        {
            get
            {
                return m_imparams;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    m_imparams = ImageOptimizer.defaultOptimizerParams;
                else
                    m_imparams = value;
            }
        }
#endif
        public string FileName
        {
            get
            {
                return m_sourceFile;
            }
            set
            {
                m_sourceFile = value;
            }
        }

        public Header BARHeader
        {
            get
            {
                return m_header;
            }
        }

        public TOC TableOfContents
        {
            get
            {
                return m_toc;
            }
        }

        public string ResourceRoot
        {
            get
            {
                return m_resourceRoot;
            }
            set
            {
                if (value.EndsWith("\\") || value.EndsWith("/"))
                {
                    m_resourceRoot = Path.GetDirectoryName(value);
                    return;
                }
                m_resourceRoot = value;
            }
        }

        public Endianness Endian
        {
            get
            {
                return m_endian;
            }
            set
            {
                m_endian = value;
            }
        }

        public CompressionMethod DefaultCompression
        {
            get
            {
                return m_defaultCompression;
            }
            set
            {
                m_defaultCompression = value;
            }
        }

        private static bool ShouldCompress(string filePath, BARAddFileOptions options)
        {
            bool result = true;
            if (options == BARAddFileOptions.ForceCompress)
                result = true;
            else if (options == BARAddFileOptions.ForceUncompress)
                result = false;
            else if (options == BARAddFileOptions.Default)
            {
                string a = Path.GetExtension(filePath).ToLower();
                if (!string.IsNullOrEmpty(a) && (a == ".mp3" || a == ".mp4" || a == ".bar" || a == ".sharc"))
                    result = false;
            }
            return result;
        }

        private string GetInBARPath(string filePath)
        {
            return filePath.Replace(Path.DirectorySeparatorChar, '/').Substring(m_resourceRoot.Length + 1);
        }

        private void RunDeleteOperation(HashedFileName fileName, BARFileOperationFlags flags)
        {
            TOCEntry value = m_toc[fileName];
            if ((flags & BARFileOperationFlags.Delete) == BARFileOperationFlags.Delete && !m_deletedFileSection.ContainsKey(fileName))
                m_deletedFileSection[fileName] = value;
            m_toc.Remove(fileName);
            Dirty = true;
            m_toc.ResortOffsets();
        }

        private bool LoadHeaderandTOC()
        {
            bool ok = false;
            Stream dataReadStream = GetDataReadStream();
            try
            {
                m_endian = GetEndianness(dataReadStream);
                if (ReadHeader(dataReadStream))
                {
                    uint num = 20U;
                    if (m_header.Version == 512)
                        num = 52U;

                    if ((ushort)(m_header.Flags & ArchiveFlags.Bar_Flag_ZTOC) == 1 && m_header.Version != 512)
                    {
                        EndianAwareBinaryReader endianAwareBinaryReader = EndianAwareBinaryReader.Create(dataReadStream, m_endian);
                        uint num2 = endianAwareBinaryReader.ReadUInt32();
                        EndianAwareBinaryReader endianAwareBinaryReader2 = EndianAwareBinaryReader.Create(dataReadStream, Endianness.LittleEndian);
                        byte[] inData = endianAwareBinaryReader2.ReadBytes((int)num2);
                        CompressionMethod method = CompressionMethod.ZLib;
                        byte[] buffer = CompressionFactory.Decompress(inData, method, m_header.Flags);
                        MemoryStream inStream = new MemoryStream(buffer);
                        if (ReadTOC(inStream, m_endian))
                        {
                            m_toc.CompressedSize = num2;
                            uint num3 = (uint)Utils.GetFourByteAligned((long)(ulong)num2) - num2;
                            uint num4 = num2 + 4U + num3;
                            num += num4;
                        }
                        else
                        {
                            dataReadStream.Close();
                            return false;
                        }
                    }
                    else
                    {
                        if (ReadTOC(dataReadStream, m_endian))
                        {
                            if (m_header.Version == 512)
                                num += m_toc.Version2Size;
                            else
                                num += m_toc.Version1Size;
                        }
                        else
                        {
                            dataReadStream.Close();
                            return false;
                        }
                    }
                    LoggerAccessor.LogDebug("Loading file data", Array.Empty<object>());
                    foreach (object obj in m_toc)
                    {
                        TOCEntry tocentry = (TOCEntry)obj;
                        dataReadStream.Seek((long)(ulong)(num + tocentry.DataOffset), SeekOrigin.Begin);
                        EndianAwareBinaryReader endianAwareBinaryReader3 = EndianAwareBinaryReader.Create(dataReadStream, Endianness.LittleEndian);
                        byte[] array = null;
                        if (tocentry.CompressedSize <= 4194304UL)
                            array = endianAwareBinaryReader3.ReadBytes((int)tocentry.CompressedSize);
                        else
                        {
                            array = new byte[tocentry.CompressedSize];
                            byte[] array2;
                            for (long num5 = 0L; num5 < (long)(ulong)tocentry.CompressedSize; num5 += array2.LongLength)
                            {
                                array2 = endianAwareBinaryReader3.ReadBytes((int)Math.Min(4194304L, (long)(tocentry.CompressedSize - (ulong)num5)));
                                Array.Copy(array2, 0L, array, num5, array2.Length);
                            }
                        }
                        tocentry.RawData = array;
                    }

                    ok = true;
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[BARArchive - An Error happened when processing data - {ex}]");
                ok = false;
            }

            dataReadStream.Close();

            if (ok)
                return true;
            else
                return false;
        }

        private void LoadBAR()
        {
            if (LoadHeaderandTOC())
            {
                LoggerAccessor.LogDebug("Analysing file contents", Array.Empty<object>());
                AnalyseFileTypes();
            }
        }

        private Endianness GetEndianness(Stream inStream)
        {
            Endianness result = Endianness.LittleEndian;
            if (inStream.ReadByte() == 173)
                result = Endianness.BigEndian;
            inStream.Seek(0L, SeekOrigin.Begin);
            return result;
        }

        private void AnalyseFileTypes()
        {
            foreach (object obj in m_toc)
            {
                try
                {
                    TOCEntry tocentry = (TOCEntry)obj;
                    using (MemoryStream memoryStream = new MemoryStream(tocentry.GetData(m_header.Flags)))
                    {
                        tocentry.FileType = FileTypeAnalyser.Instance.Analyse(memoryStream);
                        memoryStream.Close();
                    }
                }
                catch (Exception ex)
                {
                    LoggerAccessor.LogError($"[BARArchive] - Failed to Analyse File - {ex}");
                }
            }
        }

        private void LoadInternal()
        {
            LoadBAR();
            bool flag = false;
            string text = Path.ChangeExtension(m_sourceFile, ".map");
            if (!File.Exists(text))
            {
                text = m_sourceFile + ".map";
                if (File.Exists(text))
                    flag = true;
            }
            else
                flag = true;
            if (flag)
                LoadMap(text);
        }

        private int WriteDataAlignedSection(byte[] data, Stream outStream)
        {
            EndianAwareBinaryWriter endianAwareBinaryWriter = EndianAwareBinaryWriter.Create(outStream, Endianness.LittleEndian);
            endianAwareBinaryWriter.Write(data);

            long currentPosition = outStream.Position;
            long alignedPosition = currentPosition + 3 & ~3L; // Align the position to the nearest multiple of four

            if (alignedPosition > currentPosition)
            {
                int paddingBytes = (int)(alignedPosition - currentPosition);
                byte[] padding = new byte[paddingBytes];
                endianAwareBinaryWriter.Write(padding);
            }

            return data.Length + (int)(alignedPosition - currentPosition);
        }

        private void WriteMapAs(string mapFileName)
        {
            using (TextWriter textWriter = File.CreateText(mapFileName))
            {
                string value = Path.Combine(Path.GetDirectoryName(mapFileName), Path.GetFileNameWithoutExtension(mapFileName));
                textWriter.WriteLine(value);
                textWriter.WriteLine("\n== Header ==");
                if (m_header.Version == 512)
                    textWriter.WriteLine("size: {0}", 52U);
                else
                    textWriter.WriteLine("size: {0}", 20U);
                uint num = 0U;
                textWriter.WriteLine("{0:X8} Magic: {1:X4}", num, m_header.Magic);
                num += 4U;
                textWriter.WriteLine("{0:X8} Version: {1:X8}", num, m_header.Version);
                num += 4U;
                textWriter.WriteLine("{0:X8} Priority: {1:X8}", num, m_header.Priority);
                num += 4U;
                if (m_header.Version == 512)
                {
                    textWriter.WriteLine("{0:X8} IV: {1:X8}", num, m_header.IV.ToHexString());
                    num += 16U;
                }
                textWriter.WriteLine("{0:X8} User: {1}", num, m_header.UserData);
                num += 4U;
                textWriter.WriteLine("{0:X8} Number of files: {1}", num, m_header.NumFiles);
                num += 4U;
                if (m_header.Version == 512)
                {
                    textWriter.WriteLine("{0:X8} Key: {1:X8}", num, m_header.Key.ToHexString());
                    num += 16U;
                }
                textWriter.WriteLine("\n== Table of Contents ==");
                if ((ushort)(GetHeader().Flags & ArchiveFlags.Bar_Flag_ZTOC) == 1)
                    textWriter.WriteLine("size: {0} (compressed)", m_toc.CompressedSize);
                else
                {
                    if (m_header.Version == 512)
                        textWriter.WriteLine("size: {0}", m_toc.Version2Size);
                    else
                        textWriter.WriteLine("size: {0}", m_toc.Version1Size);
                }
                foreach (object obj in m_toc)
                {
                    TOCEntry tocentry = (TOCEntry)obj;
                    if (m_header.Version == 512)
                    {
                        textWriter.WriteLine("{0:X8} {1}[{2:X8}] size={3} IV={4}", new object[]
                        {
                            num,
                            tocentry.Path,
                            (uint)tocentry.FileName.Value,
                            tocentry.Size,
                            tocentry.IV.ToHexString()
                        });
                        num += 24U;
                    }
                    else
                    {
                        if ((ushort)(GetHeader().Flags & ArchiveFlags.Bar_Flag_ZTOC) == 1)
                        {
                            textWriter.WriteLine("{0:X8} {1}[{2:X8}] size={3}", new object[]
                            {
                            0,
                            tocentry.Path,
                            (uint)tocentry.FileName.Value,
                            tocentry.Size
                            });
                        }
                        else
                        {
                            textWriter.WriteLine("{0:X8} {1}[{2:X8}] size={3}", new object[]
                            {
                            num,
                            tocentry.Path,
                            (uint)tocentry.FileName.Value,
                            tocentry.Size
                            });
                            num += 16U;
                        }
                    }
                }
                textWriter.WriteLine("\n== File Data ==");
                long num2 = 0L;
                long num3 = 0L;
                foreach (TOCEntry tocentry2 in m_toc.SortByDataSectionOffset())
                {
                    float num4 = tocentry2.Size != 0U ? (1f - tocentry2.CompressedSize / tocentry2.Size) * 100f : 0f;
                    textWriter.WriteLine("{0:X8} {1:X8} {2}({3}%)", new object[]
                    {
                        num,
                        tocentry2.CompressedSize,
                        tocentry2.Path,
                        num4
                    });
                    num += tocentry2.CompressedSize + tocentry2.CompressedSize % 4U;
                    num2 += (long)(ulong)tocentry2.Size;
                    num3 += (long)(ulong)(tocentry2.CompressedSize + tocentry2.CompressedSize % 4U);
                }
                textWriter.WriteLine("\n== Stats ==");
                float num5 = num2 != 0L ? (1f - num3 / (float)num2) * 100f : 0f;
                textWriter.WriteLine("Total input data size = {0}", num2);
                textWriter.WriteLine("Total output data size = {0}", num3);
                textWriter.WriteLine("Compression = {0}%", num5);
            }
        }

        public void WriteMap(string BARFileName)
        {
            string mapFileName;
            if (KeepExtension)
                mapFileName = BARFileName + ".map";
            else
            {
                if (m_header.Version == 512)
                    mapFileName = Path.ChangeExtension(BARFileName, ".sharc") + ".map";
                else
                    mapFileName = Path.ChangeExtension(BARFileName, ".bar") + ".map";
            }
            WriteMapAs(mapFileName);
        }

        private bool ReadHeader(Stream inStream)
        {
            EndianAwareBinaryReader endianAwareBinaryReader = EndianAwareBinaryReader.Create(inStream, m_endian);
            m_header.Magic = endianAwareBinaryReader.ReadUInt32();
            if (m_header.Magic != 2918127585U)
            {
                LoggerAccessor.LogWarn("File is not a BAR File or the header is corrupt", m_sourceFile);
                return false;
            }
            uint num = endianAwareBinaryReader.ReadUInt32();
            ushort flags = (ushort)(65535U & num);
            ushort version = (ushort)(65535U & num >> 16);
            m_header.Flags = (ArchiveFlags)flags;
            m_header.Version = version;
            if (m_header.Version <= 256)
            {
                LoggerAccessor.LogInfo("BAR Version 1 Detected", m_sourceFile);
                m_header.Priority = endianAwareBinaryReader.ReadInt32();
                m_header.UserData = endianAwareBinaryReader.ReadInt32();
                m_header.NumFiles = endianAwareBinaryReader.ReadUInt32();
            }
            else if (m_header.Version == 512)
            {
                LoggerAccessor.LogInfo("BAR Version 2 Detected", m_sourceFile);
                if (m_endian == Endianness.BigEndian)
                    m_header.IV = EndianUtils.EndianSwap(endianAwareBinaryReader.ReadBytes(16));
                else
                    m_header.IV = endianAwareBinaryReader.ReadBytes(16);
                m_header.Priority = endianAwareBinaryReader.ReadInt32();
                m_header.UserData = endianAwareBinaryReader.ReadInt32();
                m_header.NumFiles = endianAwareBinaryReader.ReadUInt32();
                if (m_endian == Endianness.BigEndian)
                    m_header.Key = EndianUtils.EndianSwap(endianAwareBinaryReader.ReadBytes(16));
                else
                    m_header.Key = endianAwareBinaryReader.ReadBytes(16);
            }
            else if (m_header.Version > 512)
            {
                LoggerAccessor.LogWarn("Invalid BAR File Version", m_sourceFile);
                return false;
            }

            return true;
        }

        private bool ReadTOC(Stream inStream, Endianness endian)
        {
            bool isok = true;
            try
            {
                LoggerAccessor.LogDebug("Loading Table of Contents", Array.Empty<object>());
                EndianAwareBinaryReader endianAwareBinaryReader = EndianAwareBinaryReader.Create(inStream, endian);
                m_toc.Clear();
                int num = 0;
                while (num < (long)(ulong)m_header.NumFiles)
                {
                    int fileName = endianAwareBinaryReader.ReadInt32();
                    uint num2 = endianAwareBinaryReader.ReadUInt32();
                    uint offset = num2 & 4294967292U;
                    uint num3 = num2 & 3U;
                    CompressionMethod compression = (CompressionMethod)num3;
                    uint size = endianAwareBinaryReader.ReadUInt32();
                    uint compressedSize = endianAwareBinaryReader.ReadUInt32();
                    TOCEntry tocentry = null;
                    if (m_header.Version == 512)
                    {
                        byte[] IV = null;
                        if (endian == Endianness.BigEndian) // IV is always little endian.
                            IV = EndianUtils.EndianSwap(endianAwareBinaryReader.ReadBytes(8));
                        else
                            IV = endianAwareBinaryReader.ReadBytes(8);
                        tocentry = new TOCEntry(fileName, size, compressedSize, offset, IV);
                    }
                    else
                        tocentry = new TOCEntry(fileName, size, compressedSize, offset);
                    tocentry.Compression = compression;
                    tocentry.Index = num;
                    m_toc.Add(tocentry);
                    LoggerAccessor.LogDebug("Loaded file {0}", new object[]
                    {
                        ((int)tocentry.FileName).ToString("X")
                    });
                    num++;
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[BARArchive] - ReadTOC Failed with error - {ex}");
                isok = false;
            }

            return isok;
        }

        private void WriteHeader(EndianAwareBinaryWriter writer)
        {
            uint num = m_header.Version;
            num = num << 16 | (uint)m_header.Flags;
            writer.Write(m_header.Magic);
            writer.Write(num);
            if (m_header.Version == 512)
            {
                byte[] IV = new byte[16];
                byte[] CipheredHeaderData = new byte[28];
                byte[] PriorityBytes = BitConverter.GetBytes(m_header.Priority);
                byte[] UserDataBytes = BitConverter.GetBytes(m_header.UserData);
                byte[] NumFilesBytes = BitConverter.GetBytes(m_header.NumFiles);
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(PriorityBytes);
                    Array.Reverse(UserDataBytes);
                    Array.Reverse(NumFilesBytes);
                }
                if (m_endian == Endianness.LittleEndian) // By default data is in big endian.
                {
                    PriorityBytes = EndianUtils.EndianSwap(PriorityBytes);
                    UserDataBytes = EndianUtils.EndianSwap(UserDataBytes);
                    NumFilesBytes = EndianUtils.EndianSwap(NumFilesBytes);
                }
                Buffer.BlockCopy(m_header.IV, 0, IV, 0, m_header.IV.Length);
                Buffer.BlockCopy(PriorityBytes, 0, CipheredHeaderData, 0, PriorityBytes.Length);
                Buffer.BlockCopy(UserDataBytes, 0, CipheredHeaderData, PriorityBytes.Length, UserDataBytes.Length);
                Buffer.BlockCopy(NumFilesBytes, 0, CipheredHeaderData, PriorityBytes.Length + UserDataBytes.Length, NumFilesBytes.Length);
                Buffer.BlockCopy(m_header.Key, 0, CipheredHeaderData, PriorityBytes.Length + UserDataBytes.Length + NumFilesBytes.Length, m_header.Key.Length);
                if (m_endian == Endianness.BigEndian) // This data is always little endian.
                {
                    writer.Write(EndianUtils.EndianSwap(IV));
                    writer.Write(EndianUtils.EndianSwap(ToolsImplementation.ProcessCrypt_Decrypt(CipheredHeaderData, version2key.IsBase64().Item2, IV, 2)));
                }
                else
                {
                    writer.Write(IV);
                    writer.Write(ToolsImplementation.ProcessCrypt_Decrypt(CipheredHeaderData, version2key.IsBase64().Item2, IV, 2));
                }
            }
            else
            {
                writer.Write(m_header.Priority);
                writer.Write(m_header.UserData);
                writer.Write(m_header.NumFiles);
            }
        }

        private TOCEntry GetTocEntryFromFilepath(string filePath, uint dataLength)
        {
            string text = GetInBARPath(filePath).Trim();
            if (!m_allowWhitespaceInFilenames && text.Contains(" "))
            {
                LoggerAccessor.LogError(string.Format("File paths cannot contain whitespace: {0}", text));
                return null;
            }
            AFSHash afsHash = new AFSHash(text);
            bool flag = false;
            if (Path.GetExtension(filePath) == string.Empty && int.TryParse(Path.GetFileName(filePath), out _))
                flag = true;
            TOCEntry tocentry;
            if (flag)
            {
                int val = int.Parse(Path.GetFileName(filePath));
                tocentry = new TOCEntry((HashedFileName)val, dataLength)
                {
                    Path = string.Empty
                };
            }
            else
            {
                tocentry = new TOCEntry((HashedFileName)afsHash.Value, dataLength)
                {
                    Path = text
                };
            }
            return tocentry;
        }

        private void CompressAndAddFile(bool compress, Stream inStream, TOCEntry tocEntry)
        {
            if (m_header.Version == 512)
            {
                bool isvalid = true;
                if (inStream.Length == 0L)
                    isvalid = false;
                byte[] array = new byte[(int)inStream.Length];
                tocEntry.Size = (uint)inStream.Length;
                inStream.Read(array, 0, (int)inStream.Length);
                inStream.Close();
                byte[] array2 = null;
                if (isvalid)
                    array2 = Zlib.EdgeZlibCompress(array);
                if (array2 != null)
                {
                    tocEntry.CompressedSize = (uint)array2.Length;
                    tocEntry.Compression = CompressionMethod.Encrypted;
                    using (MemoryStream memoryStream = new MemoryStream(array))
                    {
                        tocEntry.FileType = FileTypeAnalyser.Instance.Analyse(memoryStream);
                        memoryStream.Close();
                    }
                    int count = (int)m_toc.Count;
                    tocEntry.Index = count;
                    byte[] IV = new byte[8];
                    Buffer.BlockCopy(tocEntry.IV, 0, IV, 0, tocEntry.IV.Length);
                    tocEntry.RawData = ToolsImplementation.ProcessCrypt_Decrypt(array2, m_header.Key, IV, 0);
                }
                else
                {
                    array2 = array;
                    tocEntry.CompressedSize = (uint)array2.Length;
                    tocEntry.Compression = CompressionMethod.Uncompressed;
                    using (MemoryStream memoryStream = new MemoryStream(array))
                    {
                        tocEntry.FileType = FileTypeAnalyser.Instance.Analyse(memoryStream);
                        memoryStream.Close();
                    }
                    int count = (int)m_toc.Count;
                    tocEntry.Index = count;
                    tocEntry.RawData = array2;
                }
            }
            else if (m_header.Version != 512 && encrypt)
            {
                bool isvalid = true;
                if (inStream.Length == 0L)
                    isvalid = false;
                byte[] array = new byte[(int)inStream.Length];
                tocEntry.Size = (uint)inStream.Length;
                inStream.Read(array, 0, (int)inStream.Length);
                inStream.Close();
                byte[] array2 = null;
                if (isvalid)
                    array2 = Zlib.EdgeZlibCompress(array);
                if (array2 != null)
                {
                    tocEntry.CompressedSize = (uint)array2.Length + 28;
                    tocEntry.Compression = CompressionMethod.Encrypted;
                    using (MemoryStream memoryStream = new MemoryStream(array))
                        tocEntry.FileType = FileTypeAnalyser.Instance.Analyse(memoryStream);
                    int count = (int)m_toc.Count;
                    tocEntry.Index = count;
                    if (m_endian == Endianness.BigEndian)
                        tocEntry.RawData = ByteUtils.CombineByteArrays(ToolsImplementation.ApplyPaddingPrefix(new byte[20], true), new byte[][]
                        {
                             EndianUtils.EndianSwap(BitConverter.GetBytes(!BitConverter.IsLittleEndian ? EndianUtils.ReverseInt(array2.Length) : array2.Length)),
                             array2
                        });
                    else
                        tocEntry.RawData = ByteUtils.CombineByteArrays(ToolsImplementation.ApplyPaddingPrefix(new byte[20], false), new byte[][]
                        {
                             BitConverter.GetBytes(!BitConverter.IsLittleEndian ? EndianUtils.ReverseInt(array2.Length) : array2.Length),
                             array2
                        });
                    }
                else
                {
                    array2 = array;
                    tocEntry.CompressedSize = (uint)array2.Length;
                    tocEntry.Compression = CompressionMethod.Uncompressed;
                    using (MemoryStream memoryStream = new MemoryStream(array))
                        tocEntry.FileType = FileTypeAnalyser.Instance.Analyse(memoryStream);
                    int count = (int)m_toc.Count;
                    tocEntry.Index = count;
                    tocEntry.RawData = array2;
                }
            }
            else
            {
                CompressionMethod compressionMethod = compress ? DefaultCompression : CompressionMethod.Uncompressed;
                byte[] array = new byte[(int)inStream.Length];
                byte[] array2 = array;
                tocEntry.Size = (uint)inStream.Length;
                if (inStream.Length == 0L)
                    compressionMethod = CompressionMethod.Uncompressed;
                else
                {
                    inStream.Read(array, 0, (int)inStream.Length);
                    inStream.Close();
                    array2 = array;
                    if (compress)
                    {
                        array2 = CompressionFactory.Compress(array, compressionMethod, m_header.Flags);
                        if (array2 == null || array2.Length >= array.Length)
                        {
                            compressionMethod = CompressionMethod.Uncompressed;
                            array2 = array;
                        }
                    }
                }

                tocEntry.CompressedSize = (uint)array2.Length;
                tocEntry.Compression = compressionMethod;
                using (MemoryStream memoryStream = new MemoryStream(array))
                {
                    tocEntry.FileType = FileTypeAnalyser.Instance.Analyse(memoryStream);
                    memoryStream.Close();
                }
                tocEntry.Index = (int)m_toc.Count;
                tocEntry.RawData = array2;
            }
        }

        public void VerifyFileIsInDirectory(DirectoryInfo directory, string filePath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(filePath));
            bool flag = false;
            DirectoryInfo directoryInfo2 = directoryInfo;
            string b = directory.FullName.ToLower().TrimEnd(new char[]
            {
                '/',
                Path.DirectorySeparatorChar
            });
            while (directoryInfo2 != null)
            {
                if (directoryInfo2.FullName.ToLower() == b)
                {
                    flag = true;
                    break;
                }
                directoryInfo2 = directoryInfo2.Parent;
            }
            if (!flag)
                LoggerAccessor.LogError(string.Format("{0} is not under the build root {1}.", filePath, directory.FullName));
        }

        public static BARArchive CreateEmpty(string resourceRoot)
        {
            return new BARArchive
            {
                ResourceRoot = resourceRoot
            };
        }

        public void AddFile(string filePath)
        {
            BARAddFileOptions options = BARAddFileOptions.Default;
            AddFile(filePath, options);
        }

        public void AddFile(string filePath, BARAddFileOptions options)
        {
            VerifyFileIsInDirectory(new DirectoryInfo(m_resourceRoot), filePath);
            if (ContainsFile(filePath))
            {
                ReplaceFile(filePath, options);
                return;
            }
            string extension = null;
            try
            {
                extension = Path.GetExtension(filePath);
            }
            catch
            {
            }
            string ContentType = HTTPProcessor.GetMimeType(extension);
#if NET6_0_OR_GREATER
            if (optimizeassets && (ContentType.StartsWith("image/") || (!string.IsNullOrEmpty(extension) && extension.Equals(".dds", StringComparison.InvariantCultureIgnoreCase))))
            {
                using (Stream stream = ImageOptimizer.OptimizeImage(convertersfolder, filePath, extension, m_imparams))
                {
                    try
                    {
                        AddFile(filePath, stream, options);
                    }
                    catch
                    {

                    }
                }
            }
            else
            {
                using (FileStream fileStream = File.OpenRead(filePath))
                {
                    try
                    {
                        AddFile(filePath, fileStream, options);
                    }
                    catch
                    {

                    }
                }
            }
#else
            using (FileStream fileStream = File.OpenRead(filePath))
            {
                try
                {
                    AddFile(filePath, fileStream, options);
                }
                catch
                {

                }
            }
#endif
        }

        public bool ContainsFile(string filePath)
        {
            string inBARPath = GetInBARPath(filePath);
            AFSHash afsHash = new AFSHash(inBARPath);
            HashedFileName filename = new HashedFileName(afsHash.Value);
            TOCEntry tocentry = m_toc[filename];
            return tocentry != null;
        }

        public void ReplaceFile(string filePath, BARAddFileOptions options)
        {
            string inBARPath = GetInBARPath(filePath);
            AFSHash afsHash = new AFSHash(inBARPath);
            HashedFileName filename = (HashedFileName)afsHash.Value;
            TOCEntry tocEntry = m_toc[filename];
            Stream inStream;
            string extension = null;
            try
            {
                extension = Path.GetExtension(filePath);
            }
            catch
            {
            }
            string ContentType = HTTPProcessor.GetMimeType(extension);
#if NET6_0_OR_GREATER
            if (optimizeassets && (ContentType.StartsWith("image/") || (!string.IsNullOrEmpty(extension) && extension.Equals(".dds", StringComparison.InvariantCultureIgnoreCase))))
                inStream = ImageOptimizer.OptimizeImage(convertersfolder, filePath, extension, m_imparams);
            else
#endif
                inStream = File.OpenRead(filePath);
            bool compress = ShouldCompress(filePath, options);
            CompressAndAddFile(compress, inStream, tocEntry);
            Dirty = true;
            m_toc.ResortOffsets();
            LoggerAccessor.LogDebug("Replaced file {0}", new object[]
            {
                filePath
            });
        }

        public void AddFile(string filePath, Stream inStream, BARAddFileOptions options)
        {
            TOCEntry tocEntryFromFilepath = GetTocEntryFromFilepath(filePath, (uint)inStream.Length);
			if (tocEntryFromFilepath == null)
				return;
            bool compress = ShouldCompress(filePath, options);
            CompressAndAddFile(compress, inStream, tocEntryFromFilepath);
            m_toc.Add(tocEntryFromFilepath);
            TOCEntry lastEntry = m_toc.GetLastEntry();
            if (lastEntry == null)
                tocEntryFromFilepath.DataOffset = 0U;
            else
                tocEntryFromFilepath.DataOffset = lastEntry.DataOffset + lastEntry.AlignedSize;
            LoggerAccessor.LogDebug("Added file {0}", new object[]
            {
                filePath
            });
            Dirty = true;
        }

        public void DeleteFile(HashedFileName fileName)
        {
            RunDeleteOperation(fileName, BARFileOperationFlags.Delete);
        }

        public void ObliterateFile(HashedFileName fileName)
        {
            RunDeleteOperation(fileName, BARFileOperationFlags.Obliterate);
        }

        public void UndeleteFile(HashedFileName fileName)
        {
            if (m_deletedFileSection.ContainsKey(fileName))
            {
                TOCEntry newEntry = m_deletedFileSection[fileName];
                m_toc.Add(newEntry);
                Dirty = true;
                return;
            }
            LoggerAccessor.LogError("Undelete failed - cannot find specified file");
        }

        public void Load()
        {
            LoadInternal();
        }

        public void FastLoad()
        {
            LoadHeaderandTOC();
        }

        public void Save()
        {
            SaveAs(FileName);
        }

        public void SaveAs(string fileName)
        {
            Stream dataWriterStream = GetDataWriterStream(fileName);
            m_header.NumFiles = m_toc.Count;
            if (Dirty)
                m_toc.ResortOffsets();
            EndianAwareBinaryWriter endianAwareBinaryWriter = EndianAwareBinaryWriter.Create(dataWriterStream, m_endian);
            EndianAwareBinaryWriter endianAwareBinaryWriter2 = EndianAwareBinaryWriter.Create(dataWriterStream, Endianness.LittleEndian);
            byte[] array = Array.Empty<byte>();
            if (m_header.Version == 512)
            {
                byte[] IV = new byte[16];
                Buffer.BlockCopy(m_header.IV, 0, IV, 0, m_header.IV.Length);
                ToolsImplementation.IncrementIVBytes(IV, 1); // Increment IV by one (supposed to be the continuation of the header cypher context).
                array = m_toc.GetBytesVersion2(version2key, IV, m_endian);
            }
            else
                array = m_toc.GetBytesVersion1();
            if (m_endian == Endianness.BigEndian && m_header.Version != 512)
                array = EndianUtils.EndianSwap(array);
            if ((ushort)(m_header.Flags & ArchiveFlags.Bar_Flag_ZTOC) == 1 && m_header.Version != 512)
            {
                CompressionMethod method = CompressionMethod.ZLib;
                byte[] array2 = CompressionFactory.Compress(array, method, m_header.Flags);
                if (array2 != null)
                {
                    uint num = (uint)Utils.GetFourByteAligned(array2.Length);
                    if (num + 4U < m_toc.Version1Size)
                    {
                        WriteHeader(endianAwareBinaryWriter);
                        endianAwareBinaryWriter.Write(num);
                        byte[] array3 = new byte[num];
                        array2.CopyTo(array3, 0);
                        endianAwareBinaryWriter2.Write(array3);
                    }
                    else
                    {
                        Header header = m_header;
                        header.Flags &= ~ArchiveFlags.Bar_Flag_ZTOC;
                        WriteHeader(endianAwareBinaryWriter);
                        endianAwareBinaryWriter2.Write(array);
                    }
                }
                else
                {
                    Header header = m_header;
                    header.Flags &= ~ArchiveFlags.Bar_Flag_ZTOC;
                    WriteHeader(endianAwareBinaryWriter);
                    endianAwareBinaryWriter2.Write(array);
                }
            }
            else
            {
                WriteHeader(endianAwareBinaryWriter);
                endianAwareBinaryWriter2.Write(array);
            }
            foreach (TOCEntry tocentry in m_toc.SortByDataSectionOffset())
            {
                if (m_header.Version != 512 && tocentry.Compression == CompressionMethod.Encrypted)
                {
                    byte[] SignatureIV = BitConverter.GetBytes(ToolsImplementation.BuildSignatureIv((int)tocentry.Size, (int)tocentry.CompressedSize, (int)dataWriterStream.Position, m_header.UserData));
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(SignatureIV);
                    byte[] OriginalSigntureIV = new byte[SignatureIV.Length];
                    Buffer.BlockCopy(SignatureIV, 0, OriginalSigntureIV, 0, OriginalSigntureIV.Length);
                    ToolsImplementation.IncrementIVBytes(SignatureIV, 3);
                    byte[] FileBytes = new byte[(int)tocentry.CompressedSize - 28];
                    Buffer.BlockCopy(tocentry.RawData, 28, FileBytes, 0, FileBytes.Length);
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
                        byte[] SignatureHeader = new byte[24];
                        byte[] SHA1Data = NetHasher.DotNetHasher.ComputeSHA1(FileBytes);
                        Buffer.BlockCopy(SHA1Data, 0, tocentry.RawData, 4, SHA1Data.Length);
                        Buffer.BlockCopy(FileBytes, 0, tocentry.RawData, 28, FileBytes.Length);
                        Buffer.BlockCopy(tocentry.RawData, 4, SignatureHeader, 0, SignatureHeader.Length);
                        switch (cdnMode)
                        {
                            case 2:
                                SignatureHeader = ToolsImplementation.ProcessCrypt_Decrypt(SignatureHeader, ToolsImplementation.HDKSignatureKey, OriginalSigntureIV, 1);
                                break;
                            case 1:
                                SignatureHeader = ToolsImplementation.ProcessCrypt_Decrypt(SignatureHeader, ToolsImplementation.BetaSignatureKey, OriginalSigntureIV, 1);
                                break;
                            default:
                                SignatureHeader = ToolsImplementation.ProcessCrypt_Decrypt(SignatureHeader, ToolsImplementation.SignatureKey, OriginalSigntureIV, 1);
                                break;
                        }
                        if (SignatureHeader != null)
                            Buffer.BlockCopy(SignatureHeader, 0, tocentry.RawData, 4, SignatureHeader.Length);
                        else
                            LoggerAccessor.LogError("[Encryption Proxy] - Encryption of SignatureHeader failed! Entry will be corrupted!");
                    }
                    else
                        LoggerAccessor.LogError("[Encryption Proxy] - Encryption of FileBytes failed! Entry will be corrupted!");
                }
                WriteDataAlignedSection(tocentry.RawData, dataWriterStream);
            }
            if (m_outputStreamCopy != null)
            {
                dataWriterStream.Position = 0L;
                dataWriterStream.CopyTo(m_outputStreamCopy);
                dataWriterStream.Position = m_outputStreamCopy.Position = 0L;
            }
            endianAwareBinaryWriter2.Close();
            endianAwareBinaryWriter.Close();
            WriteMap(fileName);
            if (m_header.Version == 512)
                LoggerAccessor.LogDebug("SHARC Archive complete", Array.Empty<object>());
            else
                LoggerAccessor.LogDebug("BAR Archive complete", Array.Empty<object>());
            Dirty = false;
            FileName = fileName;
        }

        public void LoadMap(string MAPFile)
        {
            LoggerAccessor.LogDebug("Loading map file {0}", new object[]
            {
                MAPFile
            });
            FileAttributes attributes = File.GetAttributes(MAPFile);
            FileAttributes fileAttributes = attributes;
            fileAttributes &= ~FileAttributes.ReadOnly;
            File.SetAttributes(MAPFile, fileAttributes);
            TextReader textReader = File.OpenText(MAPFile);
            string text = textReader.ReadToEnd();
            textReader.Close();
            File.SetAttributes(MAPFile, attributes);
            string[] array = text.Split(new char[]
            {
                '\n'
            });
            Hashtable hashtable = new Hashtable();
            Regex regex = new Regex("([\\w\\d]+)(\\s+)(Number of files:\\s+)(?<filecount>[\\d]+)");
            Regex regex2 = new Regex("([\\w\\d]+)\\s+(?<path>[:\\w\\d\\s-_$/\\.+]+)\\[(?<hash>[\\w\\d]+)\\]");
            foreach (string input in array)
            {
                Match match = regex.Match(input);
                if (match.Success)
                    int.Parse(match.Groups["filecount"].Value);
                else
                {
                    match = regex2.Match(input);
                    if (match.Success)
                    {
                        string value = match.Groups["path"].Value;
                        HashedFileName hashedFileName = (HashedFileName)int.Parse(match.Groups["hash"].Value, NumberStyles.HexNumber);
                        hashtable[hashedFileName] = value;
                    }
                }
            }
            foreach (object obj in hashtable.Keys)
            {
                HashedFileName hashedFileName2 = (HashedFileName)obj;
                TOCEntry tocentry = m_toc[hashedFileName2];
                if (tocentry != null)
                {
                    tocentry.Path = (string)hashtable[hashedFileName2];
                    tocentry.FileType = FileTypeAnalyser.GetFileType(Path.GetExtension(tocentry.Path));
                }
            }
            textReader.Close();
        }

        public byte[] GetFileData(string FileName)
        {
            return GetFileData(new HashedFileName(new AFSHash(FileName).Value));
        }

        public byte[] GetFileData(HashedFileName FileName)
        {
            return m_toc[FileName]?.GetData(m_header.Flags);
        }

        public void UpdateFile(HashedFileName FileName, byte[] inData)
        {
            TOCEntry tocentry = m_toc[FileName];
            byte[] array = inData;
            byte[] array2 = CompressionFactory.Compress(inData, tocentry.Compression, m_header.Flags);
            if (array2 != null)
            {
                if (array2.Length < inData.Length)
                    array = array2;
                else
                    tocentry.Compression = CompressionMethod.Uncompressed;
            }
            tocentry.Size = (uint)inData.Length;
            tocentry.CompressedSize = (uint)array.Length;
            tocentry.RawData = array;
            Dirty = true;
            m_toc.ResortOffsets();
            LoggerAccessor.LogDebug("Updated file {0}", new object[]
            {
                tocentry.Path
            });
        }

        public void Recompress(HashedFileName[] fileNames, CompressionMethod newMethod, ArchiveFlags newFlags)
        {
            foreach (HashedFileName hashedFileName in fileNames)
            {
                TOCEntry tocentry = m_toc[hashedFileName];
                if (newMethod == tocentry?.Compression && BARHeader.Flags == newFlags)
                    LoggerAccessor.LogDebug("Skipped " + hashedFileName, Array.Empty<object>());
                else if (newMethod != CompressionMethod.Uncompressed && !ShouldCompress(tocentry.Path, BARAddFileOptions.Default))
                    LoggerAccessor.LogDebug("Skipped " + hashedFileName, Array.Empty<object>());
                else
                {
                    byte[] array = CompressionFactory.Decompress(tocentry, tocentry.Compression, m_header.Flags);
                    tocentry.RawData = array ?? Array.Empty<byte>();
                    byte[] array2 = CompressionFactory.Compress(tocentry, newMethod, newFlags);
                    if (newMethod != CompressionMethod.Uncompressed && array2?.Length >= array?.Length && newMethod != CompressionMethod.Encrypted)
                    {
                        tocentry.Compression = CompressionMethod.Uncompressed;
                        tocentry.CompressedSize = (uint)array.Length;
                        tocentry.RawData = array;
                        Dirty = true;
                        m_toc.ResortOffsets();
                        LoggerAccessor.LogDebug(string.Format("File {0} not recompressed because the size of the recompressed data is larger than the size of the uncompressed data", hashedFileName), Array.Empty<object>());
                    }
                    else
                    {
                        tocentry.Compression = newMethod;
                        tocentry.CompressedSize = (uint)array2.Length;
                        tocentry.RawData = array2;
                        Dirty = true;
                        m_toc.ResortOffsets();
                        LoggerAccessor.LogDebug("Recompressed " + hashedFileName, Array.Empty<object>());
                    }
                }
            }
            BARHeader.Flags = newFlags;
        }

        public byte[] GetRawFileData(string FileName)
        {
            return GetRawFileData(new HashedFileName(new AFSHash(GetInBARPath(FileName)).Value));
        }

        public byte[] GetRawFileData(HashedFileName FileName)
        {
            TOCEntry tocentry = m_toc[FileName];
            if (tocentry != null)
                return tocentry.RawData;
            return null;
        }

        public void ExtractToFile(HashedFileName FileName, string outDir)
        {
            TOCEntry tocentry = m_toc[FileName];
            string path = string.Empty;
            if (string.IsNullOrEmpty(tocentry.Path))
                path = string.Format("{0}{1}{2}", outDir, Path.DirectorySeparatorChar, FileName);
            else
                path = string.Format("{0}{1}{2}", outDir, Path.DirectorySeparatorChar, tocentry.Path.Replace('/', Path.DirectorySeparatorChar));
            string directoryName = Path.GetDirectoryName(path);
            Directory.CreateDirectory(directoryName);
            FileStream fileStream = File.Open(path, FileMode.Create);
            byte[] data = tocentry.GetData(m_header.Flags);
            fileStream.Write(data, 0, data.Length);
            fileStream.Close();
            LoggerAccessor.LogDebug("Extracted file {0}", new object[]
            {
                Path.GetFileName(path)
            });
        }

        public void ExtractAll(string outDir)
        {
            foreach (object obj in m_toc)
            {
                TOCEntry tocentry = (TOCEntry)obj;
                ExtractToFile(tocentry.FileName, outDir);
            }
        }

        public Hashtable GetFileTree()
        {
            Hashtable hashtable = new Hashtable();
            foreach (object obj in m_toc)
            {
                TOCEntry tocentry = (TOCEntry)obj;
                string key = Path.GetDirectoryName(tocentry.Path)?.Replace(Path.DirectorySeparatorChar, '/');
                if (key == null)
                    continue;
                string fileName = Path.GetFileName(tocentry.Path);
                List<string> list;
                if (hashtable.ContainsKey(key))
                    list = (List<string>)hashtable[key];
                else
                {
                    list = new List<string>();
                    hashtable[key] = list;
                }
                list?.Add(fileName);
            }
            return hashtable;
        }

        public void CreateManifest()
        {
            Hashtable fileTree = GetFileTree();
            StringBuilder stringBuilder = new StringBuilder();
            XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings()
            {
                Indent = true,
                Encoding = Encoding.UTF8,
                OmitXmlDeclaration = true
            });
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("manifest");
            foreach (object obj in fileTree.Keys)
            {
                string text = (string)obj;
                xmlWriter.WriteStartElement("dir");
                xmlWriter.WriteAttributeString("name", text);
                List<string> list = (List<string>)fileTree[text];
                foreach (string value in list)
                {
                    xmlWriter.WriteStartElement("file");
                    xmlWriter.WriteAttributeString("name", value);
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
            byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
            MemoryStream inStream = new MemoryStream(bytes);
            string filePath = m_resourceRoot + "\\__$manifest$__";
            AddFile(filePath, inStream, BARAddFileOptions.ForceCompress);
        }

        public bool KeepExtension
        {
            get
            {
                return m_keepExtension;
            }
            set
            {
                m_keepExtension = value;
            }
        }

        internal const long TE_DATA_CHUNKSIZE = 4194304L;

        protected string m_sourceFile = string.Empty;

        private string m_resourceRoot = string.Empty;

        private readonly Header m_header = new Header();

        private readonly TOC m_toc;

        private readonly Dictionary<HashedFileName, TOCEntry> m_deletedFileSection;

        private bool m_dirty;

        private bool m_allowWhitespaceInFilenames;

        private CompressionMethod m_defaultCompression = CompressionMethod.EdgeZLib;

        private Endianness m_endian;

        private bool m_keepExtension;

        public bool IsEditMode;

        protected Stream m_outputStream;

        protected Stream m_outputStreamCopy;

        protected Stream m_sourceStream;

        private delegate void LoadBARDelegate();

        private delegate bool BARFileMatchDelegate(TOCEntry te);

        private delegate void BarExtractAllDelegate(string outDir);

        private delegate void BarRecompressDelegate(HashedFileName[] fileNames, CompressionMethod newMethod, ArchiveFlags newFlags);
    }
}
