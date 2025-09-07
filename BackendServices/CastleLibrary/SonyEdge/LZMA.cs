using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CustomLogger;
using EndianTools;
using SharpCompress.Compressors.LZMA;

namespace SonyEdge
{
    public class LZMA
    {
        private static readonly bool LittleEndian = BitConverter.IsLittleEndian;

        private static readonly SemaphoreSlim lzmaSema = new SemaphoreSlim(Environment.ProcessorCount);

        /// <summary>
        /// Decompress a EdgeLZMA segs byte array data.
        /// <para>Decompresser un tableau de byte en format EdgeLZMA segs.</para>
        /// </summary>
        /// <param name="InBuffer">The byte array to decompress.</param>
        /// <returns>A byte array.</returns>
        public static byte[] SegmentsDecompress(byte[] InBuffer, bool PrintErrors = true)
        {
            return Task.Run(async() => {
                try
                {
                    if (InBuffer.Length > 4 && InBuffer[0] == 0x73 && InBuffer[1] == 0x65 && InBuffer[2] == 0x67 && InBuffer[3] == 0x73)
                    {
                        int numofsegments = BitConverter.ToInt16(!LittleEndian ? new byte[] { InBuffer[6], InBuffer[7] } : new byte[] { InBuffer[7], InBuffer[6] }, 0);
                        int OriginalSize = BitConverter.ToInt32(!LittleEndian ? new byte[] { InBuffer[8], InBuffer[9], InBuffer[10], InBuffer[11] } : new byte[] { InBuffer[11], InBuffer[10], InBuffer[9], InBuffer[8] }, 0);
                        //int CompressedSize = BitConverter.ToInt32(!LittleEndian ? new byte[] { inbuffer[12], inbuffer[13], inbuffer[14], inbuffer[15] } : new byte[] { inbuffer[15], inbuffer[14], inbuffer[13], inbuffer[12] }, 0); // Unused during decompression.

                        byte[] TOCData = new byte[8 * numofsegments]; // 8 being size of each TOC entry.

                        Buffer.BlockCopy(InBuffer, 16, TOCData, 0, TOCData.Length);

                        if (TOCData.Length % 8 == 0)
                        {
                            int chunkIndex = 0;
                            List<KeyValuePair<int, Task<byte[]>>> lzmaResults = new List<KeyValuePair<int, Task<byte[]>>>();

                            for (int i = 0; i < TOCData.Length; i += 8)
                            {
                                int SegmentIndex = i;

                                lzmaResults.Add(new KeyValuePair<int, Task<byte[]>>(chunkIndex, Task.Run(async () => {
                                    await lzmaSema.WaitAsync().ConfigureAwait(false);

                                    try
                                    {
                                        byte[] SegmentCompressedSizeByte = new byte[2];
                                        byte[] SegmentOriginalSizeByte = new byte[2];
                                        byte[] SegmentOffsetByte = new byte[4];

                                        Buffer.BlockCopy(TOCData, SegmentIndex, SegmentCompressedSizeByte, 0, SegmentCompressedSizeByte.Length);
                                        Buffer.BlockCopy(TOCData, SegmentIndex + 2, SegmentOriginalSizeByte, 0, SegmentOriginalSizeByte.Length);
                                        Buffer.BlockCopy(TOCData, SegmentIndex + 4, SegmentOffsetByte, 0, SegmentOffsetByte.Length);

                                        int SegmentOffset;
                                        byte[] CompressedData, output;

                                        if (LittleEndian)
                                        {
                                            Array.Reverse(SegmentCompressedSizeByte);
                                            Array.Reverse(SegmentOriginalSizeByte);
                                            Array.Reverse(SegmentOffsetByte);
                                        }

                                        int SegmentCompressedSize = BitConverter.ToUInt16(SegmentCompressedSizeByte, 0);
                                        bool hasCompressedData = SegmentCompressedSize > 0;
                                        int SegmentOriginalSize = BitConverter.ToUInt16(SegmentOriginalSizeByte, 0);

                                        if (!hasCompressedData)
                                        {
                                            SegmentOffset = BitConverter.ToInt32(SegmentOffsetByte, 0);
                                            CompressedData = new byte[65536];
                                        }
                                        else
                                        {
                                            SegmentOffset = BitConverter.ToInt32(SegmentOffsetByte, 0) - 1; // -1 cause there is an offset for compressed content... sdk bug?
                                            CompressedData = new byte[SegmentCompressedSize];
                                        }

                                        Buffer.BlockCopy(InBuffer, SegmentOffset, CompressedData, 0, CompressedData.Length);

                                        if (hasCompressedData && CompressedData.Length > 3 && CompressedData[0] == 0x5D && CompressedData[1] == 0x00 && CompressedData[2] == 0x00)
                                        {
                                            using (MemoryStream compressedStream = new MemoryStream(CompressedData))
                                            using (MemoryStream decompressedStream = new MemoryStream())
                                            {
                                                try
                                                {
                                                    SegmentDecompress(compressedStream, decompressedStream);

                                                    // Find the number of bytes in the stream
                                                    int contentLength = (int)decompressedStream.Length;

                                                    // Create a byte array
                                                    byte[] buffer = new byte[contentLength];

                                                    // Read the contents of the memory stream into the byte array
                                                    decompressedStream.Read(buffer, 0, contentLength);

                                                    output = buffer;
                                                }
                                                catch // Not a LZMA stream. Can in theory happen with file data being uncompressed and starting with 0x5D,NULL,NULL bytes (haven't seen any for now).
                                                {
                                                    output = CompressedData;
                                                }
                                            }
                                        }
                                        else
                                            output = CompressedData; // Can happen, just means segment is not compressed.

                                        int sizeOfSegment = output.Length;

                                        if (SegmentOriginalSize != 0 && sizeOfSegment != SegmentOriginalSize)
                                        {
                                            if (PrintErrors)
                                                LoggerAccessor.LogError($"[SonyEdge] - LZMA - Segs: Segment at position:{SegmentIndex} has a size that is different than the one indicated in TOC! (Got:{sizeOfSegment}, Expected:{SegmentOriginalSize}).");
                                            return null;
                                        }

                                        return output;
                                    }
                                    catch (Exception ex)
                                    {
                                        if (PrintErrors)
                                            LoggerAccessor.LogError($"[SonyEdge] - LZMA - Segs: SegmentsDecompress task for segment index:{SegmentIndex} thrown an assertion : {ex}");
                                        return null;
                                    }
                                    finally
                                    {
                                        lzmaSema.Release();
                                    }
                                })));

                                chunkIndex++;
                            }

                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                foreach (var result in lzmaResults.OrderBy(kv => kv.Key))
                                {
                                    byte[] decompressedChunk = await result.Value.ConfigureAwait(false);
                                    if (decompressedChunk == null) // We failed.
                                        return null;
                                    memoryStream.Write(decompressedChunk, 0, decompressedChunk.Length);
                                }
                                if (memoryStream.Length == OriginalSize)
                                    return memoryStream.ToArray();
                            }

                            if (PrintErrors)
                                LoggerAccessor.LogError("[SonyEdge] - LZMA - Segs: File size is different than the one indicated in TOC!.");
                        }
                        else if (PrintErrors)
                            LoggerAccessor.LogError("[SonyEdge] - LZMA - Segs: The byte array length is not evenly divisible by 8!");
                    }
                    else if (PrintErrors)
                        LoggerAccessor.LogError("[SonyEdge] - LZMA - Segs: File is not a valid segment based EdgeLzma compressed file!");
                }
                catch (Exception ex)
                {
                    if (PrintErrors)
                        LoggerAccessor.LogError($"[SonyEdge] - LZMA - Segs: SegmentsDecompress thrown an assertion : {ex}");
                }

                return null;
            }).Result;
        }

        public static byte[] Decompress(byte[] CompressedData)
        {
            if (CompressedData == null || CompressedData.Length <= 12)
                throw new InvalidDataException("[SonyEdge] - LZMA - Decompress: buffer is not a valid EdgeLZMA compressed data");

            if (BitConverter.ToInt32(!BitConverter.IsLittleEndian ? EndianUtils.ReverseArray(CompressedData) : CompressedData, 8) != CompressedData.Length)
                throw new InvalidDataException("[SonyEdge] - LZMA - Decompress: buffer length does not match declared buffer length");
            else
            {
                switch (CompressedData[5])
                {
                    case 2:
                        return Decompress2(CompressedData);
                    case 4:
                        return Decompress4(CompressedData);
                }

                throw new InvalidDataException("[SonyEdge] - LZMA - Decompress: unknown compression type");
            }
        }

        private static byte[] Decompress2(byte[] buffer)
        {
            throw new NotImplementedException();
        }

        private static byte[] Decompress4(byte[] buffer)
        {
            using (MemoryStream result = new MemoryStream())
            {
                int outSize = BitConverter.ToInt32(!BitConverter.IsLittleEndian ? EndianUtils.ReverseArray(buffer) : buffer, 12);
                int streamCount = (outSize + 0xFFFF) >> 16;
                int offset = 0x18 + streamCount * 2 + 5;

                Decoder decoder = new Decoder();
                decoder.SetDecoderProperties(new MemoryStream(buffer, 0x18, 5).ToArray());

                for (int i = 0; i < streamCount; i++)
                {
                    int streamSize = buffer[5 + 0x18 + i * 2] + (buffer[6 + 0x18 + i * 2] << 8);
                    if (streamSize != 0)
                        decoder.Code(new MemoryStream(buffer, offset, streamSize), result, streamSize, Math.Min(outSize, 0x10000), null);
                    else
                        result.Write(buffer, offset, streamSize = Math.Min(outSize, 0x10000));
                    outSize -= 0x10000;
                    offset += streamSize;
                }

                return result.ToArray();
            }
        }

        /// <summary>
        /// Decompress a block of the segmented EdgeLZMA data.
        /// <para>D�compresse un block provenant d'une matrice de donn�e encod�e avec le codec EdgeLZMA.</para>
        /// </summary>
        /// <param name="inStream">The input LZMA stream.</param>
        /// <param name="outStream">The output stream.</param>
        /// <returns>Nothing.</returns>
        private static void SegmentDecompress(Stream inStream, Stream outStream)
        {
            byte[] properties = new byte[5];
            inStream.Read(properties, 0, 5);
            Decoder decoder = new Decoder();
            decoder.SetDecoderProperties(properties);
            long outSize = 0;
            for (int i = 0; i < 8; i++)
            {
                outSize |= (long)(byte)inStream.ReadByte() << 8 * i;
            }
            decoder.Code(inStream, outStream, inStream.Length - inStream.Position, outSize, null);
            outStream.Position = 0;
        }
    }
}
