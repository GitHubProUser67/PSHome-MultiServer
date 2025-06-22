using System;
using System.ComponentModel;
using System.IO;
using System.Numerics;
using CustomLogger;
using NetworkLibrary.Extension;

namespace HomeTools.PS3_Creator
{
    public class EDAT
    {
        public static sbyte COMPRESSION_UNSUPPORTED = sbyte.MinValue;
        public static sbyte STATUS_ERROR_OUPUTFILE_IO = -101;
        public static sbyte STATUS_ERROR_INPUTFILE_IO = -100;
        public static sbyte STATUS_ERROR_HASHTITLEIDNAME = -1;
        public static sbyte STATUS_ERROR_HASHDEVKLIC = -2;
        public static sbyte STATUS_ERROR_MISSINGKEY = -3;
        public static sbyte STATUS_ERROR_HEADERCHECK = -4;
        public static sbyte STATUS_ERROR_DECRYPTING = -5;
        public static sbyte STATUS_ERROR_INCORRECT_FLAGS = -6;
        public static sbyte STATUS_ERROR_INCORRECT_VERSION = -7;
        public static sbyte STATUS_OK = 0;
        public static long FLAG_COMPRESSED = 0x00000001L;
        public static long FLAG_0x02 = 0x00000002L;
        public static long FLAG_KEYENCRYPTED = 0x00000008L;
        public static long FLAG_0x10 = 0x00000010L;
        public static long FLAG_0x20 = 0x00000020L;
        public static long FLAG_SDAT = 0x01000000L;
        public static long FLAG_DEBUG = 0x80000000L;

        /* KDSBEST START */
        public int encryptFile(String inFile, String outFile, byte[] devKLic, byte[] keyFromRif, byte[] contentID, byte[] flags, byte[] type, byte[] version)
        {
            if (string.IsNullOrEmpty(inFile))
                return STATUS_ERROR_INPUTFILE_IO;
            if (string.IsNullOrEmpty(outFile))
                return STATUS_ERROR_OUPUTFILE_IO;

            FileStream fin = File.Open(inFile, FileMode.Open);
            //  MemoryMappedFile fin1 = MemoryMappedFile.CreateFromFile(inFile, FileMode.Open);
            NPD[] ptr = new NPD[1]; //Ptr to Ptr
            FileStream o = File.Open(outFile, FileMode.Create);
            string[] fn = o.Name.Split('\\');
            byte[] npd = writeValidNPD(fn[fn.Length - 1], devKLic, ptr, fin, contentID, flags, version, type);
            o.Write(npd, 0, npd.Length);
            // FLAGS
            byte[] buffer = new byte[4]
            {
                 1,
                 0,
                 0,
                 60
            };
            o.Write(buffer, 0, 4);

            // blocksize 0x00004000
            buffer[0] = 0;
            buffer[1] = 0;
            buffer[2] = 64;
            buffer[3] = 0;
            o.Write(buffer, 0, 4);
            long finlen = fin.Length;
            byte[] lenBuf = BitConverter.GetBytes(finlen);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(lenBuf);
            byte[] rLenBuf = new byte[8];
            for (int i = 0; i < 8; i++)
                rLenBuf[i] = 0x00;
            for (int i = 0; i < lenBuf.Length; i++)
            {
                rLenBuf[7 - i] = lenBuf[i];
            }
            o.Write(rLenBuf, 0, 8);

            // Fill the rest 0x10 bytes with dummy we generate the metasection hash later!
            // the bytes till 0x100 are unknown
            buffer[0] = 0x00;
            while (o.Length < 0x100)
                o.Write(buffer, 0, 1);

            EDATData data = new EDATData
            {
                flags = 0x0100003C,
                blockSize = 0x00004000,
                fileLen = new BigInteger(finlen)
            };
            byte[] rifkey = getKey(ptr[0], data, devKLic, keyFromRif); //Obtain the key for decryption (result of sc471 or sdatkey)
            int hashFlag = 0x10000002;

            encryptData(fin, o, ptr[0], data, rifkey);

            o.Seek(0x90, SeekOrigin.Begin);
            AppLoader aa = new AppLoader();
            aa.doInit(hashFlag, version[0] == 4, 0x00000001, new byte[0x10], new byte[0x10], rifkey);

            int startoffset = (data.getFlags() & FLAG_SDAT) != 0L ? 32 : 16;
            int numBlocks = (int)((data.getFileLen() + data.getBlockSize() - 1) / data.getBlockSize());
            long totallength = 0;
            int baseOffset = 0x100;
            int len;
            for (long remaining = startoffset * numBlocks; remaining > 0L; remaining -= len)
            {
                len = HEADER_MAX_BLOCKSIZE > remaining ? (int)remaining : HEADER_MAX_BLOCKSIZE;
                o.Seek(baseOffset + totallength, 0);
                byte[] content = new byte[len];
                byte[] ooo = new byte[len];
                o.Read(content, 0, content.Length);
                aa.doUpdate(content, 0, ooo, 0, len);
                totallength += len;
            }
            byte[] headerHash = new byte[16];
            aa.doFinalButGetHash(headerHash);
            o.Seek(144L, 0);
            o.Write(headerHash, 0, headerHash.Length);
            o.Seek(0L, 0);
            byte[] header = new byte[160];
            byte[] headerODummy = new byte[160];
            o.Read(header, 0, header.Length);
            AppLoaderReverse a = new AppLoaderReverse();
            byte[] generatedHash = new byte[16];
            a.doAll(hashFlag, version[0] == 4, 1, header, 0, headerODummy, 0, header.Length, new byte[16], new byte[16], rifkey, generatedHash, 0);
            o.Seek(160L, 0);
            o.Write(generatedHash, 0, generatedHash.Length);
            while (o.Length < 256L)
                o.Write(buffer, 0, 1);
            o.Close();
            fin.Close();
            return STATUS_OK;
        }

        private byte[] createNPDHash1(String filename, byte[] npd)
        {
            byte[] fileBytes = ConversionUtils.charsToByte(filename.ToCharArray());
            byte[] data1 = new byte[0x30 + fileBytes.Length];
            ConversionUtils.arraycopy(npd, 0x10, data1, 0, 0x30);
            ConversionUtils.arraycopy(fileBytes, 0x00, data1, 0x30, fileBytes.Length);
            byte[] hash1 = ToolsImpl.CMAC128(EDATKeys.npdrm_omac_key3, data1, 0, data1.Length);
            ConversionUtils.arraycopy(hash1, 0, npd, 0x50, 0x10);
            bool result1 = compareBytes(hash1, 0, npd, 0x50, 0x10);
            if (result1)
            {
                return hash1;
            }
            return null;
        }

        private byte[] createNPDHash2(byte[] klicensee, byte[] npd)
        {
            byte[] xoredKey = new byte[0x10];
            ToolsImpl.XOR(xoredKey, klicensee, EDATKeys.npdrm_omac_key2);
            byte[] calculated = ToolsImpl.CMAC128(xoredKey, npd, 0, 0x60);
            ConversionUtils.arraycopy(calculated, 0, npd, 0x60, 0x10);
            bool result2 = compareBytes(calculated, 0, npd, 0x60, 0x10);
            if (result2)
            {
                return calculated;
            }
            return null;
        }

        private byte[] writeValidNPD(String filename, byte[] devKLic, NPD[] npdPtr, FileStream fin, byte[] contentID, byte[] flags, byte[] version, byte[] type)
        {
            byte[] npd = new byte[0x80];
            //NPD Magic
            //ConversionUtils.arraycopy(npd, 0, result.magic, 0, 4);
            npd[0] = 0x4E;
            npd[1] = 0x50;
            npd[2] = 0x44;
            npd[3] = 0x00;
            //Version 3
            //result.version = ConversionUtils.be32(npd, 4);
            npd[4] = 0x00;
            npd[5] = 0x00;
            npd[6] = 0x00;
            npd[7] = version[0];
            //License 2 ref 3 klic /* 1 network, 2 local, 3 free */
            //result.license = ConversionUtils.be32(npd, 8);
            npd[8] = 0x00;
            npd[9] = 0x00;
            npd[10] = 0x00;
            npd[11] = 0x03;

            //Type /* 1 exec, 21 update */
            //result.type = ConversionUtils.be32(npd, 0xC);
            npd[12] = 0x00;
            npd[13] = 0x00;
            npd[14] = 0x00;
            npd[15] = type[0];

            //No Idea where I get the content_id
            //ConversionUtils.arraycopy(npd, 0x10, result.content_id, 0, 0x30
            for (int i = 0; i < 0x30; i++)
                npd[0x10 + i] = contentID[i];

            //Used to create IV
            //ConversionUtils.arraycopy(npd, 0x40, result.digest, 0, 0x10);
            byte[] iv = ConversionUtils.charsToByte("FixedLicenseEDAT".ToCharArray());
            ConversionUtils.arraycopy(iv, 0, npd, 0x40, 0x10);

            //I guess it's a full file hash
            //ConversionUtils.arraycopy(npd, 0x50, result.titleHash, 0, 0x10);
            byte[] hash = createNPDHash1(filename, npd);
            ConversionUtils.arraycopy(hash, 0x00, npd, 0x50, 0x10);

            //Used to create Blockkey
            //ConversionUtils.arraycopy(npd, 0x60, result.devHash, 0, 0x10);
            byte[] devHash = createNPDHash2(devKLic, npd);
            ConversionUtils.arraycopy(devHash, 0, npd, 0x60, 0x10);

            //NPD EOF?!?!?!
            //result.unknown3 = ConversionUtils.be64(npd, 0x70);
            //result.unknown4 = ConversionUtils.be64(npd, 0x78);
            for (int i = 0; i < 16; i++)
            {
                npd[0x70 + i] = 0x00;
            }

            npdPtr[0] = NPD.createNPD(npd);
            return npd;
        }

        /* KDSBEST END */

        public int decryptFile(String inFile, String outFile, byte[] devKLic, byte[] keyFromRif)
        {
            FileStream fin = File.Open(inFile, FileMode.Open);
            string[] fn = fin.Name.Split('\\');

            NPD[] ptr = new NPD[1]; //Ptr to Ptr
            int result = validateNPD(fn[fn.Length - 1], devKLic, ptr, fin); //Validate NPD hashes
            if (result < 0)
            {
                fin.Close();
                return result;
            }
            NPD npd = ptr[0];
            EDATData data = getEDATData(fin); //Get flags, blocksize and file len
            byte[] rifkey = getKey(npd, data, devKLic, keyFromRif); //Obtain the key for decryption (result of sc471 or sdatkey)
            if (rifkey == null)
            {
                LoggerAccessor.LogError("[PS3 Creator] - EDAT - ERROR: Key for decryption is missing");
                fin.Close();
                return STATUS_ERROR_MISSINGKEY;
            }
            else
            {
#if DEBUG
                LoggerAccessor.LogInfo("[PS3 Creator] - EDAT - DECRYPTION KEY: " + rifkey.ToHexString());
#endif
            }
            result = checkHeader(rifkey, data, npd, fin);
            if (result < 0)
            {
                fin.Close();
                return result;
            }
            FileStream o = File.Open(outFile, FileMode.Create);
            result = decryptData(fin, o, npd, data, rifkey);
            if (result < 0)
            {
                fin.Close();
                return result;
            }
            fin.Close();
            o.Close();
            LoggerAccessor.LogInfo("[PS3 Creator] - EDAT - COMPLETE: File Written to disk");
            return STATUS_OK;
        }
        private static int HEADER_MAX_BLOCKSIZE = 0x3C00;


        private int checkHeader(byte[] rifKey, EDATData data, NPD npd, FileStream i)
        {
            i.Seek(0, SeekOrigin.Begin);
            byte[] header = new byte[0xA0];
            byte[] o = new byte[0xA0];
            byte[] expectedHash = new byte[0x10];
            //Version check
#if DEBUG
            LoggerAccessor.LogInfo("[PS3 Creator] - EDAT - checking NPD Version:" + npd.getVersion());
#endif
            if ((npd.getVersion() == 0) || (npd.getVersion() == 1))
            {
                if ((data.getFlags() & 0x7FFFFFFE) != 0) { LoggerAccessor.LogError("[PS3 Creator] - EDAT - ERROR: Incorrect Header Flags"); return STATUS_ERROR_INCORRECT_FLAGS; }
            }
            else if (npd.getVersion() == 2)
            {
                if ((data.getFlags() & 0x7EFFFFE0) != 0) { LoggerAccessor.LogError("[PS3 Creator] - EDAT - ERROR: Incorrect Header Flags"); return STATUS_ERROR_INCORRECT_FLAGS; }
            }
            else if (npd.getVersion() == 3 || (npd.getVersion() == 4))
            {
                if ((data.getFlags() & 0x7EFFFFC0) != 0) { LoggerAccessor.LogError("[PS3 Creator] - EDAT - ERROR: Incorrect Header Flags"); return STATUS_ERROR_INCORRECT_FLAGS; }
            }
            else { LoggerAccessor.LogError("[PS3 Creator] - EDAT - ERROR: Unsupported EDAT version (need keys)"); return STATUS_ERROR_INCORRECT_VERSION; }
            {
                i.Read(header, 0, header.Length);
                i.Read(expectedHash, 0, expectedHash.Length);
#if DEBUG
                LoggerAccessor.LogInfo("[PS3 Creator] - EDAT - Checking header hash:");
#endif
                AppLoader a = new AppLoader();
                int hashFlag = ((data.getFlags() & FLAG_KEYENCRYPTED) == 0) ? 0x00000002 : 0x10000002;
                if ((data.getFlags() & FLAG_DEBUG) != 0) hashFlag |= 0x01000000;

                //Veryfing header
                bool result = a.doAll(hashFlag, npd.getVersion() == 4, 0x00000001, header, 0, o, 0, header.Length, new byte[0x10], new byte[0x10], rifKey, expectedHash, 0);
                if (!result)
                {
                    LoggerAccessor.LogError("[PS3 Creator] - EDAT - Error verifying header. Is rifKey valid?.");
                    return STATUS_ERROR_HEADERCHECK;
                }
                if ((data.getFlags() & FLAG_0x20) == 0L)
                {
#if DEBUG
                    LoggerAccessor.LogInfo("[PS3 Creator] - EDAT - Checking metadata hash:");
#endif
                    a = new AppLoader();
                    a.doInit(hashFlag, npd.getVersion() == 4, 0x00000001, new byte[0x10], new byte[0x10], rifKey);

                    int sectionSize = ((data.getFlags() & FLAG_COMPRESSED) != 0) ? 0x20 : 0x010;
                    int numBlocks = (int)((data.getFileLen() + data.getBlockSize() - 11) / data.getBlockSize()); //Determine the metadatasection total len

                    int readed = 0;
                    int baseOffset = 0x100;
                    //baseOffset +=  modifier; //There is an unknown offset to add to the metadatasection... value seen 0
                    long remaining = sectionSize * numBlocks;
                    while (remaining > 0)
                    {
                        int lenToRead = (HEADER_MAX_BLOCKSIZE > remaining) ? (int)remaining : HEADER_MAX_BLOCKSIZE;
                        i.Seek(baseOffset + readed, SeekOrigin.Begin);
                        byte[] content = new byte[lenToRead];
                        o = new byte[lenToRead];
                        i.Read(content, 0, content.Length);
                        a.doUpdate(content, 0, o, 0, lenToRead);
                        readed += lenToRead;
                        remaining -= lenToRead;
                    }
                    result = a.doFinal(header, 0x90);

                    if (!result)
                    {
                        LoggerAccessor.LogError("[PS3 Creator] - EDAT - Error verifying metadatasection. Data tampered");
                        return STATUS_ERROR_HEADERCHECK;
                    }
                }

                return STATUS_OK;
            }
        }

        private byte[] decryptMetadataSection(byte[] metadata)
        {
            byte[] result = new byte[0x10];
            result[0x00] = (byte)(metadata[0xC] ^ metadata[0x8] ^ metadata[0x10]);
            result[0x01] = (byte)(metadata[0xD] ^ metadata[0x9] ^ metadata[0x11]);
            result[0x02] = (byte)(metadata[0xE] ^ metadata[0xA] ^ metadata[0x12]);
            result[0x03] = (byte)(metadata[0xF] ^ metadata[0xB] ^ metadata[0x13]);
            result[0x04] = (byte)(metadata[0x4] ^ metadata[0x8] ^ metadata[0x14]);
            result[0x05] = (byte)(metadata[0x5] ^ metadata[0x9] ^ metadata[0x15]);
            result[0x06] = (byte)(metadata[0x6] ^ metadata[0xA] ^ metadata[0x16]);
            result[0x07] = (byte)(metadata[0x7] ^ metadata[0xB] ^ metadata[0x17]);
            result[0x08] = (byte)(metadata[0xC] ^ metadata[0x0] ^ metadata[0x18]);
            result[0x09] = (byte)(metadata[0xD] ^ metadata[0x1] ^ metadata[0x19]);
            result[0x0A] = (byte)(metadata[0xE] ^ metadata[0x2] ^ metadata[0x1A]);
            result[0x0B] = (byte)(metadata[0xF] ^ metadata[0x3] ^ metadata[0x1B]);
            result[0x0C] = (byte)(metadata[0x4] ^ metadata[0x0] ^ metadata[0x1C]);
            result[0x0D] = (byte)(metadata[0x5] ^ metadata[0x1] ^ metadata[0x1D]);
            result[0x0E] = (byte)(metadata[0x6] ^ metadata[0x2] ^ metadata[0x1E]);
            result[0x0F] = (byte)(metadata[0x7] ^ metadata[0x3] ^ metadata[0x1F]);
            return result;
        }

        private EDATData getEDATData(FileStream i)
        {
            i.Seek(0x80, SeekOrigin.Begin);
            byte[] data = new byte[0x10];
            i.Read(data, 0, data.Length);
            return EDATData.createEDATData(data);
        }

        private bool compareBytes(byte[] value1, int offset1, byte[] value2, int offset2, int len)
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

        private int validateNPD(String filename, byte[] devKLic, NPD[] npdPtr, FileStream i)
        {
            i.Seek(0, SeekOrigin.Begin);
            byte[] npd = new byte[0x80];
            i.Read(npd, 0, npd.Length);
            byte[] extraData = new byte[0x04];
            i.Read(extraData, 0, extraData.Length);
            long flag = ConversionUtils.be32(extraData, 0);
            if ((flag & FLAG_SDAT) != 0)
            {
                LoggerAccessor.LogInfo("[PS3 Creator] - EDAT - SDAT detected. NPD header is not validated");
            }
            else if (!checkNPDHash1(filename, npd))
            {
                LoggerAccessor.LogError("[PS3 Creator] - EDAT - ERROR: Hashing Title ID Name");
                return STATUS_ERROR_HASHTITLEIDNAME;
            }
            else if (devKLic == null)
            {
                LoggerAccessor.LogWarn("[PS3 Creator] - EDAT - WARNING: Can not validate devklic header");
            }
            else if (!checkNPDHash2(devKLic, npd))
            {
                LoggerAccessor.LogError("[PS3 Creator] - EDAT - ERROR: Hashing devklic");
                return STATUS_ERROR_HASHDEVKLIC;
            }
            npdPtr[0] = NPD.createNPD(npd);
            return STATUS_OK;
        }

        private bool checkNPDHash1(String filename, byte[] npd)
        {
            byte[] fileBytes = ConversionUtils.charsToByte(filename.ToCharArray());
            byte[] data1 = new byte[0x30 + fileBytes.Length];
            ConversionUtils.arraycopy(npd, 0x10, data1, 0, 0x30);
            ConversionUtils.arraycopy(fileBytes, 0x00, data1, 0x30, fileBytes.Length);
            byte[] hash1 = ToolsImpl.CMAC128(EDATKeys.npdrm_omac_key3, data1, 0, data1.Length);
            bool result1 = compareBytes(hash1, 0, npd, 0x50, 0x10);
            if (result1)
            {
#if DEBUG
                LoggerAccessor.LogInfo("[PS3 Creator] - EDAT - NPD hash 1 is valid (" + hash1.ToHexString() + ")");
#endif
            }
            return result1;
        }

        private bool checkNPDHash2(byte[] klicensee, byte[] npd)
        {
            byte[] xoredKey = new byte[0x10];
            ToolsImpl.XOR(xoredKey, klicensee, EDATKeys.npdrm_omac_key2);
            byte[] calculated = ToolsImpl.CMAC128(xoredKey, npd, 0, 0x60);
            bool result2 = compareBytes(calculated, 0, npd, 0x60, 0x10);
            if (result2)
            {
#if DEBUG
                LoggerAccessor.LogInfo("[PS3 Creator] - EDAT - NPD hash 2 is valid (" + calculated.ToHexString() + ")");
#endif
            }
            return result2;
        }

        private byte[] getKey(NPD npd, EDATData data, byte[] devKLic, byte[] keyFromRif)
        {
            byte[] result = null;
            if ((data.getFlags() & FLAG_SDAT) != 0)
            {
                //Case SDAT
                result = new byte[0x10];
                ToolsImpl.XOR(result, npd.getDevHash(), EDATKeys.SDATKEY);
            }
            else
            {
                //Case EDAT
                if (npd.getLicense() == 0x03)
                {
                    result = devKLic;
                }
                else if (npd.getLicense() == 0x02)
                {
                    result = keyFromRif;
                }
            }
            return result;
        }

        private int encryptData(FileStream ii, FileStream o, NPD npd, EDATData data, byte[] rifkey)
        {
            int numBlocks = (int)((data.getFileLen() + (BigInteger)data.getBlockSize() - (BigInteger)1) / (BigInteger)data.getBlockSize());
            int metadataSectionSize = (data.getFlags() & FLAG_COMPRESSED) != 0L || (data.getFlags() & FLAG_0x20) != 0L ? 32 : 16;
            int baseOffset = 0x100;
            for (int i = 0; i < numBlocks; ++i)
            {
                if (i > 4043309056L / data.getBlockSize())
                    i = i + 1 - 1;
                long offset = i * data.getBlockSize();
                ii.Seek(offset, 0);
                int len = (int)data.getBlockSize();
                if (i == numBlocks - 1)
                {
                    len = (int)(data.getFileLen() % new BigInteger(data.getBlockSize()));
                    if (len == 0)
                        len = (int)data.getBlockSize();
                }
                int realLen = len;
                len = len + 15 & -16;
                byte[] encryptedData = new byte[len];
                byte[] decryptedData = new byte[len];
                int toRead = realLen;
                while (toRead > 0)
                    toRead -= ii.Read(decryptedData, realLen - toRead, toRead);
                for (int ai = realLen; ai < len; ++ai)
                    decryptedData[ai] = 0;
                byte[] key = new byte[16];
                byte[] hash = new byte[16];
                byte[] blockKey = calculateBlockKey(i, npd);
                ToolsImpl.aesecbEncrypt(rifkey, blockKey, 0, key, 0, blockKey.Length);
                if ((data.getFlags() & FLAG_0x10) != 0L)
                    ToolsImpl.aesecbEncrypt(rifkey, key, 0, hash, 0, key.Length);
                else
                    ConversionUtils.arraycopy(key, 0, hash, 0L, key.Length);
                int cryptoFlag = 0x10000002;
                int hashFlag = 0x10000001;
                AppLoaderReverse a = new AppLoaderReverse();
                byte[] iv = npd.getDigest();
                byte[] generatedHash = new byte[20];
                a.doAll(hashFlag, npd.getVersion() == 4, cryptoFlag, decryptedData, 0, encryptedData, 0, decryptedData.Length, key, iv, hash, generatedHash, 0);
                o.Seek(baseOffset + i * (metadataSectionSize + data.getBlockSize()), 0);
                byte[] encryptedDataForFile = "555555555555555555555555".HexStringToByteArray();
                byte[] expectedKeyForFile = new byte[16];
                byte[] expectedHashForFile = new byte[16];
                ConversionUtils.arraycopy(generatedHash, 16, expectedHashForFile, 0L, 4);
                ConversionUtils.arraycopy(encryptedDataForFile, 0, expectedHashForFile, 4L, encryptedDataForFile.Length);
                ToolsImpl.XOR(expectedKeyForFile, expectedHashForFile, generatedHash);
                o.Write(expectedKeyForFile, 0, expectedKeyForFile.Length);
                o.Write(expectedHashForFile, 0, expectedHashForFile.Length);
                o.Write(encryptedData, 0, encryptedData.Length);
            }
            byte[] EDATAVersion = "534441544120332E332E302E57000000".HexStringToByteArray();
            o.Write(EDATAVersion, 0, EDATAVersion.Length);
            return STATUS_OK;
        }

        private int decryptData(FileStream ii, FileStream o, NPD npd, EDATData data, byte[] rifkey)
        {
            int numBlocks = (int)((data.getFileLen() + data.getBlockSize() - 1) / data.getBlockSize());
            int metadataSectionSize = ((data.getFlags() & FLAG_COMPRESSED) != 0 || (data.getFlags() & FLAG_0x20) != 0) ? 0x20 : 0x10;
            int baseOffset = 0x100;
            for (int i = 0; i < numBlocks; i++)
            {
                ii.Seek(baseOffset + i * metadataSectionSize, SeekOrigin.Begin);
                byte[] expectedHash = new byte[20];
                long offset;
                int len;
                int compressionEndBlock = 0;
                if ((data.getFlags() & FLAG_COMPRESSED) != 0)
                {
                    byte[] result = new byte[0x20];
                    byte[] metadata = new byte[0x20];
                    ii.Read(metadata, 0, metadata.Length);
                    if (npd.getVersion() <= 1L)
                        Array.Copy(metadata, result, metadata.Length);
                    else
                        result = decryptMetadataSection(metadata);
                    offset = (int)ConversionUtils.be64(result, 0);
                    len = (int)ConversionUtils.be32(result, 8);
                    compressionEndBlock = (int)ConversionUtils.be32(result, 0xC);
                    ConversionUtils.arraycopy(metadata, 0, expectedHash, 0, 0x10);
                }
                else if ((data.getFlags() & FLAG_0x20) != 0)
                {
                    byte[] metadata = new byte[0x20];
                    ii.Seek(baseOffset + i * (metadataSectionSize + data.getBlockSize()), 0);
                    ii.Read(metadata, 0, metadata.Length);
                    for (int j = 0; j < 0x10; j++)
                    {
                        expectedHash[j] = (byte)(metadata[j] ^ metadata[j + 0x10]);
                    }
                    ConversionUtils.arraycopy(metadata, 16, expectedHash, 16L, 4);
                    offset = baseOffset + i * (metadataSectionSize + data.getBlockSize()) + metadataSectionSize;
                    len = (int)data.getBlockSize();
                    if (i == numBlocks - 1)
                    {
                        int calclen = (int)(data.getFileLen() % new BigInteger(data.getBlockSize()));
                        len = calclen > 0 ? calclen : len;
                    }
                }
                else
                {
                    ii.Read(expectedHash, 0, expectedHash.Length);
                    offset = baseOffset + i * data.getBlockSize() + numBlocks * metadataSectionSize;
                    len = (int)data.getBlockSize();
                    if (i == numBlocks - 1)
                    {
                        len = (int)(data.getFileLen() % (new BigInteger(data.getBlockSize())));
                    }
                }
                int realLen = len + 15 & -16;
                LoggerAccessor.LogDebug("[PS3 Creator] - EDAT - Offset: %016X, len: %08X, realLen: %08X, endCompress: %d\r\n", offset, len, realLen, compressionEndBlock);
                ii.Seek(offset, SeekOrigin.Begin);
                byte[] encryptedData = new byte[realLen];
                byte[] decryptedData = new byte[realLen];
                ii.Read(encryptedData, 0, encryptedData.Length);
                byte[] key = new byte[0x10];
                byte[] hash = new byte[0x10];
                byte[] blockKey = calculateBlockKey(i, npd);

                ToolsImpl.aesecbEncrypt(rifkey, blockKey, 0, key, 0, blockKey.Length);
                if ((data.getFlags() & FLAG_0x10) != 0)
                {
                    ToolsImpl.aesecbEncrypt(rifkey, key, 0, hash, 0, key.Length);
                }
                else
                {
                    ConversionUtils.arraycopy(key, 0, hash, 0, key.Length);
                }
                int cryptoFlag = ((data.getFlags() & FLAG_0x02) == 0) ? 0x2 : 0x1;
                int hashFlag = (data.getFlags() & FLAG_0x10) != 0L ? (data.getFlags() & FLAG_0x20) != 0L ? 1 : 4 : 2;
                if ((data.getFlags() & FLAG_KEYENCRYPTED) != 0)
                {
                    cryptoFlag |= 0x10000000;
                    hashFlag |= 0x10000000;
                }
                if ((data.getFlags() & FLAG_DEBUG) != 0)
                {
                    cryptoFlag |= 0x01000000;
                    hashFlag |= 0x01000000;

                    if ((data.getFlags() & FLAG_COMPRESSED) != 0L && compressionEndBlock != 0)
                        return COMPRESSION_UNSUPPORTED;
                    else
                        o.Write(encryptedData, 0, realLen);
                }
                else
                {
                    AppLoader a = new AppLoader();
                    byte[] iv = (npd.getVersion() <= 1) ? (new byte[0x10]) : npd.getDigest();

                    a.doAll(hashFlag, npd.getVersion() == 4, cryptoFlag, encryptedData, 0, decryptedData, 0, encryptedData.Length, key, iv, hash, expectedHash, 0);

                    if ((data.getFlags() & FLAG_COMPRESSED) != 0 && compressionEndBlock != 0)
                        return COMPRESSION_UNSUPPORTED;
                    else
                        o.Write(decryptedData, 0, realLen);
                }
            }
            return STATUS_OK;
        }

        private byte[] calculateBlockKey(int blk, NPD npd)
        {
            byte[] baseKey = (npd.getVersion() <= 1) ? (new byte[0x10]) : npd.getDevHash();
            byte[] result = new byte[0x10];
            ConversionUtils.arraycopy(baseKey, 0, result, 0, 0xC);
            result[0xC] = (byte)(blk >> 24 & 0xFF);
            result[0xD] = (byte)(blk >> 16 & 0xFF);
            result[0xE] = (byte)(blk >> 8 & 0xFF);
            result[0xF] = (byte)(blk & 0xFF);
            return result;
        }
    }
}
