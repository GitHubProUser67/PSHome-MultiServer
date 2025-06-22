using CustomLogger;
using EndianTools;
using SevenZip;
using SevenZip.Compression.LZMA;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CompressionLibrary.Edge
{
    // Partially from https://github.com/AdmiralCurtiss/ToVPatcher/blob/master/Tales/tlzc/TLZC.cs
    public class LZMA
    {
        // Process Environment.ProcessorCount process at a time, removing the limit is not tolerable as CPU usage goes way too high.
        private static readonly SemaphoreSlim lzmaSema = new SemaphoreSlim(Environment.ProcessorCount);

        private static int dictionary = 1 << 23;

        // static Int32 posStateBits = 2;
        // static Int32 litContextBits = 3; // for normal files
        // UInt32 litContextBits = 0; // for 32-bit data
        // static Int32 litPosBits = 0;
        // UInt32 litPosBits = 2; // for 32-bit data
        // static Int32 algorithm = 2;
        // static Int32 numFastBytes = 128;

        private static bool eos = false;

        private static CoderPropID[] propIDs =
                {
                    CoderPropID.DictionarySize,
                    CoderPropID.PosStateBits,
                    CoderPropID.LitContextBits,
                    CoderPropID.LitPosBits,
                    CoderPropID.Algorithm,
                    CoderPropID.NumFastBytes,
                    CoderPropID.MatchFinder,
                    CoderPropID.EndMarker
                };

        // these are the default properties, keeping it simple for now:
        private static object[] properties =
                {
                    (int)(dictionary),
                    (int)(2),
                    (int)(3),
                    (int)(0),
                    (int)(2),
                    (int)(128),
                    "bt4",
                    eos
                };

        /// <summary>
        /// Decompress a EdgeLZMA byte array data.
        /// <para>Decompresser un tableau de byte en format EdgeLZMA.</para>
        /// </summary>
        /// <param name="CompressedData">The byte array to decompress.</param>
        /// <param name="SegsMode">Enables an alternative decompression mode.</param>
        /// <returns>A byte array.</returns>
        public static byte[] Decompress(byte[] CompressedData, bool SegsMode)
        {
            if (CompressedData == null || CompressedData.Length <= 12)
                throw new InvalidDataException("[Edge] - LZMA - Decompress: buffer is not a valid EdgeLZMA compressed data");

            if (SegsMode)
                return SegmentsDecompress(CompressedData);
            else if (BitConverter.ToInt32(!BitConverter.IsLittleEndian ? EndianUtils.ReverseArray(CompressedData) : CompressedData, 8) != CompressedData.Length)
                throw new InvalidDataException("[Edge] - LZMA - Decompress: buffer length does not match declared buffer length");
            else
            {
                switch (CompressedData[5])
                {
                    case 2:
                        return Decompress2(CompressedData);
                    case 4:
                        return Decompress4(CompressedData);
                }

                throw new InvalidDataException("[Edge] - LZMA - Decompress: unknown compression type");
            }
        }

        // magic value for Tales of Vesperia PS3: TLZC
        public static byte[] Compress(byte[] data, byte[] magic, byte compressionType, int numFastBytes = 64)
        {
            if (compressionType != 4)
                throw new ArgumentException("[Edge] - LZMA - Compress: only compressionType 4 is supported currently", "compressionType");

            switch (compressionType)
            {
                case 4:
                    return Compress4(data, magic, numFastBytes);
            }

            throw new Exception();
        }

        private static byte[] Decompress2(byte[] buffer)
        {
            throw new NotImplementedException();
        }

        private static byte[] Compress4(byte[] buffer, byte[] magic, int numFastBytes = 64, int litContextBits = 3, int litPosBits = 0, int posStateBits = 2, int blockSize = 0, int matchFinderCycles = 32)
        {
            const int dictionarySize = 0x10000;

            int inSize = buffer.Length;
            int streamCount = (inSize + ushort.MaxValue) >> 16;
            int offset = 0;

            using (MemoryStream result = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(result))
            {
                bw.Write(magic);
                bw.Write((byte)0x01);
                bw.Write((byte)0x04);
                bw.Write((byte)0x00);
                bw.Write((byte)0x00);
                bw.Write(0);   // compressed size - we'll fill this in once we know it
                bw.Write(buffer.Length);   // decompressed size
                bw.Write(0);   // unknown, 0
                bw.Write(0);   // unknown, 0
                               // next comes the coder properties (5 bytes), followed by stream lengths, followed by the streams themselves.

                Encoder encoder = new Encoder();
                Dictionary<CoderPropID, object> props = new Dictionary<CoderPropID, object>();
                props[CoderPropID.DictionarySize] = dictionarySize;
                props[CoderPropID.MatchFinder] = "BT4";
                props[CoderPropID.NumFastBytes] = numFastBytes;
                props[CoderPropID.LitContextBits] = litContextBits;
                props[CoderPropID.LitPosBits] = litPosBits;
                props[CoderPropID.PosStateBits] = posStateBits;
                //props[CoderPropID.BlockSize] = blockSize; // this always throws an exception when set
                //props[CoderPropID.MatchFinderCycles] = matchFinderCycles; // ^ same here
                //props[CoderPropID.DefaultProp] = 0;
                //props[CoderPropID.UsedMemorySize] = 100000;
                //props[CoderPropID.Order] = 1;
                //props[CoderPropID.NumPasses] = 10;
                //props[CoderPropID.Algorithm] = 0;
                //props[CoderPropID.NumThreads] = ;
                //props[CoderPropID.EndMarker] = ;

                encoder.SetCoderProperties(props.Keys.ToArray(), props.Values.ToArray());

                encoder.WriteCoderProperties(result);

                // reserve space for the stream lengths. we'll fill them in later after we know what they are.
                bw.Write(new byte[streamCount * 2]);

                List<int> streamSizes = new List<int>();

                for (int i = 0; i < streamCount; i++)
                {
                    int count = Math.Min(inSize, dictionarySize);
                    long preLength = result.Length;

                    encoder.Code(new MemoryStream(buffer, offset, count), result, count, -1, null);

                    int streamSize = (int)(result.Length - preLength);
                    if (streamSize >= dictionarySize)
                    {
                        LoggerAccessor.LogDebug("[Edge] - LZMA - Compress4: Warning! stream did not compress at all. This will cause a different code path to be executed on the PS3 whose operation is assumed and not tested!");
                        result.Position = preLength;
                        result.SetLength(preLength);
                        result.Write(buffer, offset, count);
                        streamSize = 0;
                    }

                    inSize -= dictionarySize;
                    offset += dictionarySize;
                    streamSizes.Add(streamSize);
                }

                // fill in compressed size
                result.Position = 8;
                bw.Write((int)result.Length);

                byte[] temp = result.ToArray();

                // fill in stream sizes
                for (int i = 0; i < streamSizes.Count; i++)
                {
                    temp[5 + 0x18 + i * 2] = (byte)streamSizes[i];
                    temp[6 + 0x18 + i * 2] = (byte)(streamSizes[i] >> 8);
                }

                return temp;
            }
        }

        private static byte[] Decompress4(byte[] buffer)
        {
            const int dictionarySize = 0x10000;

            using (MemoryStream result = new MemoryStream())
            {
                int outSize = BitConverter.ToInt32(!BitConverter.IsLittleEndian ? EndianUtils.ReverseArray(buffer) : buffer, 12);
                int streamCount = (outSize + ushort.MaxValue) >> 16;
                int offset = 0x18 + streamCount * 2 + 5;

                Decoder decoder = new Decoder();
                decoder.SetDecoderProperties(new MemoryStream(buffer, 0x18, 5).ToArray());

                for (int i = 0; i < streamCount; i++)
                {
                    int streamSize = buffer[5 + 0x18 + i * 2] + (buffer[6 + 0x18 + i * 2] << 8);
                    if (streamSize != 0)
                        decoder.Code(new MemoryStream(buffer, offset, streamSize), result, streamSize, Math.Min(outSize, dictionarySize), null);
                    else
                        result.Write(buffer, offset, streamSize = Math.Min(outSize, dictionarySize));
                    outSize -= dictionarySize;
                    offset += streamSize;
                }

                return result.ToArray();
            }
        }

        /// <summary>
        /// Decompress a given buffer compressed with EdgeLZMA segmented mode.
        /// <para>Décompresse un tableau de bytes avec le codec EdgeLZMA en mode s�gment�.</para>
        /// </summary>
        /// <param name="inbuffer">The byte array to decompress.</param>
        /// <returns>A byte array.</returns>
        private static byte[] SegmentsDecompress(byte[] inbuffer)
        {
            bool LittleEndian = BitConverter.IsLittleEndian;

            try
            {
                if (inbuffer.Length > 4 && inbuffer[0] == 0x73 && inbuffer[1] == 0x65 && inbuffer[2] == 0x67 && inbuffer[3] == 0x73)
                {
                    int numofsegments = BitConverter.ToInt16(!LittleEndian ? new byte[] { inbuffer[6], inbuffer[7] } : new byte[] { inbuffer[7], inbuffer[6] }, 0);
                    int OriginalSize = BitConverter.ToInt32(!LittleEndian ? new byte[] { inbuffer[8], inbuffer[9], inbuffer[10], inbuffer[11] } : new byte[] { inbuffer[11], inbuffer[10], inbuffer[9], inbuffer[8] }, 0);
                    //int CompressedSize = BitConverter.ToInt32(!LittleEndian ? new byte[] { inbuffer[12], inbuffer[13], inbuffer[14], inbuffer[15] } : new byte[] { inbuffer[15], inbuffer[14], inbuffer[13], inbuffer[12] }, 0); // Unused during decompression.

                    byte[] TOCData = new byte[8 * numofsegments]; // 8 being size of each TOC entry.

                    Buffer.BlockCopy(inbuffer, 16, TOCData, 0, TOCData.Length);

                    if (TOCData.Length % 8 == 0)
                    {
                        int chunkIndex = 0;
                        List<KeyValuePair<int, Task<byte[]>>> lzmaTasks = new List<KeyValuePair<int, Task<byte[]>>>();

                        for (int i = 0; i < TOCData.Length; i += 8)
                        {
                            int SegmentIndex = i;

                            byte[] SegmentCompressedSizeByte = new byte[2];
                            byte[] SegmentOriginalSizeByte = new byte[2];
                            byte[] SegmentOffsetByte = new byte[4];

                            Buffer.BlockCopy(TOCData, SegmentIndex, SegmentCompressedSizeByte, 0, SegmentCompressedSizeByte.Length);
                            Buffer.BlockCopy(TOCData, SegmentIndex + 2, SegmentOriginalSizeByte, 0, SegmentOriginalSizeByte.Length);
                            Buffer.BlockCopy(TOCData, SegmentIndex + 4, SegmentOffsetByte, 0, SegmentOffsetByte.Length);

                            lzmaSema.Wait();

                            lzmaTasks.Add(new KeyValuePair<int, Task<byte[]>>(chunkIndex, Task.Run(() =>
                            {
                                int SegmentOffset;
                                byte[] CompressedData, output;

                                try
                                {
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

                                    Buffer.BlockCopy(inbuffer, SegmentOffset, CompressedData, 0, CompressedData.Length);

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
                                        LoggerAccessor.LogError($"[Edge] - LZMA - Segs: Segment at position:{SegmentIndex} has a size that is different than the one indicated in TOC! (Got:{sizeOfSegment}, Expected:{SegmentOriginalSize}).");
                                        return null;
                                    }

                                    return output;
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
                            foreach (var result in lzmaTasks.OrderBy(kv => kv.Key))
                            {
                                try
                                {
                                    // Await each decompression task
                                    byte[] decompressedChunk = result.Value.Result;
                                    memoryStream.Write(decompressedChunk, 0, decompressedChunk.Length);
                                }
                                catch (Exception ex)
                                {
                                    throw new InvalidOperationException($"[Edge] - LZMA - Segs: Error during decompression at chunk {result.Key}", ex);
                                }
                            }
                            if (memoryStream.Length == OriginalSize)
                                return memoryStream.ToArray();
                        }

                        LoggerAccessor.LogError("[Edge] - LZMA - Segs: File size is different than the one indicated in TOC!.");
                    }
                    else
                        LoggerAccessor.LogError("[Edge] - LZMA - Segs: The byte array length is not evenly divisible by 8!");
                }
                else
                    LoggerAccessor.LogError("[Edge] - LZMA - Segs: File is not a valid segment based EdgeLzma compressed file!");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Edge] - LZMA - Segs: SegmentsDecompress thrown an assertion : {ex}");
            }

            return null;
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
            for (byte i = 0; i < 8; i++)
            {
                int v = inStream.ReadByte();
                outSize |= (long)(byte)v << 8 * i;
            }
            decoder.Code(inStream, outStream, inStream.Length - inStream.Position, outSize, null);
            outStream.Position = 0;
        }

        /// <summary>
        /// Compress a block of the segmented EdgeLZMA data.
        /// <para>Compresse un block provenant d'une matrice de donn�e encod�e avec le codec EdgeLZMA.</para>
        /// </summary>
        /// <param name="inStream">The input stream.</param>
        /// <param name="outStream">The output LZMA stream.</param>
        /// <returns>Nothing.</returns>
        private static void SegmentCompress(Stream inStream, Stream outStream)
        {
            Encoder encoder = new Encoder();
            encoder.SetCoderProperties(propIDs, properties);
            using (MemoryStream strmOutStream = new MemoryStream())
            {
                encoder.WriteCoderProperties(strmOutStream);
                long fileSize = inStream.Length;
                for (byte i = 0; i < 8; i++)
                    strmOutStream.WriteByte((byte)(fileSize >> (8 * i)));
                encoder.Code(inStream, strmOutStream, -1, -1, null);
                strmOutStream.Position = 0;
                strmOutStream.CopyTo(outStream);
                outStream.Position = 0;
            }
        }
    }
}
