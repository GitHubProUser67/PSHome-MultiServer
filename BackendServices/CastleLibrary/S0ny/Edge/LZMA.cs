using CastleLibrary.SharpCompress.LZMA;
using CustomLogger;
using EndianTools;

namespace CastleLibrary.S0ny.Edge
{
    public class LZMA
    {
        /// <summary>
        /// Decompress a EdgeLZMA segs byte array data.
        /// <para>Decompresser un tableau de byte en format EdgeLZMA segs.</para>
        /// </summary>
        /// <param name="InBuffer">The byte array to decompress.</param>
        /// <returns>A byte array.</returns>
        public static byte[] SegmentsDecompress(byte[] InBuffer, bool PrintErrors = true)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (
                        InBuffer.Length > 4
                        && InBuffer[0] == 0x73
                        && InBuffer[1] == 0x65
                        && InBuffer[2] == 0x67
                        && InBuffer[3] == 0x73
                    ) // segs
                    {
                        int numofsegments = BitConverter.ToUInt16(
                            !EndianAwareConverter.isLittleEndianSystem
                                ? new byte[] { InBuffer[6], InBuffer[7] }
                                : new byte[] { InBuffer[7], InBuffer[6] },
                            0
                        );
                        var OriginalSize = BitConverter.ToUInt32(
                            !EndianAwareConverter.isLittleEndianSystem
                                ? new byte[]
                                {
                                    InBuffer[8],
                                    InBuffer[9],
                                    InBuffer[10],
                                    InBuffer[11],
                                }
                                : new byte[]
                                {
                                    InBuffer[11],
                                    InBuffer[10],
                                    InBuffer[9],
                                    InBuffer[8],
                                },
                            0
                        );
                        //var CompressedSize = BitConverter.ToUInt32(!EndianAwareConverter.isLittleEndianSystem ? new byte[] { inbuffer[12], inbuffer[13], inbuffer[14], inbuffer[15] } : new byte[] { inbuffer[15], inbuffer[14], inbuffer[13], inbuffer[12] }, 0); // Unused during decompression.

                        var TOCData = new byte[numofsegments * 8]; // 8 being size of each TOC entry.

                        Buffer.BlockCopy(InBuffer, 16, TOCData, 0, TOCData.Length);

                        if (TOCData.Length % 8 == 0)
                        {
                            var chunkIndex = 0;
                            var lzmaResults = new List<KeyValuePair<int, Task<byte[]>>>();

                            for (var i = 0; i < TOCData.Length; i += 8)
                            {
                                var SegmentIndex = i;

                                lzmaResults.Add(
                                    new KeyValuePair<int, Task<byte[]>>(
                                        chunkIndex,
                                        Task.Run(() =>
                                        {
                                            try
                                            {
                                                var SegmentCompressedSizeByte = new byte[2];
                                                var SegmentOriginalSizeByte = new byte[2];
                                                var SegmentOffsetByte = new byte[4];

                                                Buffer.BlockCopy(
                                                    TOCData,
                                                    SegmentIndex,
                                                    SegmentCompressedSizeByte,
                                                    0,
                                                    SegmentCompressedSizeByte.Length
                                                );
                                                Buffer.BlockCopy(
                                                    TOCData,
                                                    SegmentIndex + 2,
                                                    SegmentOriginalSizeByte,
                                                    0,
                                                    SegmentOriginalSizeByte.Length
                                                );
                                                Buffer.BlockCopy(
                                                    TOCData,
                                                    SegmentIndex + 4,
                                                    SegmentOffsetByte,
                                                    0,
                                                    SegmentOffsetByte.Length
                                                );

                                                byte[] CompressedData,
                                                    output;

                                                if (EndianAwareConverter.isLittleEndianSystem)
                                                {
                                                    Array.Reverse(SegmentCompressedSizeByte);
                                                    Array.Reverse(SegmentOriginalSizeByte);
                                                    Array.Reverse(SegmentOffsetByte);
                                                }

                                                uint SegmentOffset = BitConverter.ToUInt32(
                                                        SegmentOffsetByte,
                                                        0
                                                    ),
                                                    SegmentCompressedSize = BitConverter.ToUInt16(
                                                        SegmentCompressedSizeByte,
                                                        0
                                                    );
                                                bool hasCompressedSize = SegmentCompressedSize > 0,
                                                    hasLZMAData = false;
                                                var SegmentOriginalSize = BitConverter.ToUInt16(
                                                    SegmentOriginalSizeByte,
                                                    0
                                                );

                                                if (!hasCompressedSize)
                                                    CompressedData = new byte[65536]; // Overflow logic.
                                                else
                                                {
                                                    if (
                                                        SegmentCompressedSize != SegmentOriginalSize
                                                    )
                                                    {
                                                        hasLZMAData = true;
                                                        SegmentOffset--; // -1 cause there is an offset for compressed content... sdk bug?
                                                    }

                                                    CompressedData = new byte[
                                                        SegmentCompressedSize
                                                    ];
                                                }

                                                Buffer.BlockCopy(
                                                    InBuffer,
                                                    (int)SegmentOffset,
                                                    CompressedData,
                                                    0,
                                                    CompressedData.Length
                                                );

                                                if (hasLZMAData)
                                                {
                                                    using (
                                                        var compressedStream = new MemoryStream(
                                                            CompressedData
                                                        )
                                                    )
                                                    using (
                                                        var decompressedStream = new MemoryStream()
                                                    )
                                                    {
                                                        try
                                                        {
                                                            SegmentDecompress(
                                                                compressedStream,
                                                                decompressedStream
                                                            );

                                                            decompressedStream.Position = 0;

                                                            // Find the number of bytes in the stream
                                                            var contentLength = (int)
                                                                decompressedStream.Length;

                                                            // Create a byte array
                                                            var buffer = new byte[contentLength];

                                                            // Read the contents of the memory stream into the byte array
                                                            decompressedStream.Read(
                                                                buffer,
                                                                0,
                                                                contentLength
                                                            );

                                                            output = buffer;
                                                        }
                                                        catch // Not a LZMA stream. Can in theory happen with file data being uncompressed and starting with 0x5D,NULL,NULL bytes (haven't seen any for now).
                                                        {
                                                            if (PrintErrors)
                                                                LoggerAccessor.LogError(
                                                                    $"[SonyEdge] - LZMA - Segs: Segment at position:{SegmentIndex} has a LZMA flag but is not a valid LZMA stream."
                                                                );
                                                            return null;
                                                        }
                                                    }
                                                }
                                                else
                                                    output = CompressedData; // Can happen, just means segment is not compressed.

                                                var sizeOfSegment = output.Length;

                                                if (
                                                    SegmentOriginalSize != 0
                                                    && sizeOfSegment != SegmentOriginalSize
                                                )
                                                {
                                                    if (PrintErrors)
                                                        LoggerAccessor.LogError(
                                                            $"[SonyEdge] - LZMA - Segs: Segment at position:{SegmentIndex} has a size that is different than the one indicated in TOC! (Got:{sizeOfSegment}, Expected:{SegmentOriginalSize})."
                                                        );
                                                    return null;
                                                }

                                                return output;
                                            }
                                            catch (Exception ex)
                                            {
                                                if (PrintErrors)
                                                    LoggerAccessor.LogError(
                                                        $"[SonyEdge] - LZMA - Segs: SegmentsDecompress task for segment index:{SegmentIndex} thrown an assertion : {ex}"
                                                    );
                                            }

                                            return null;
                                        })
                                    )
                                );

                                chunkIndex++;
                            }

                            using (var memoryStream = new MemoryStream())
                            {
                                foreach (var result in lzmaResults.OrderBy(kv => kv.Key))
                                {
                                    var decompressedChunk = await result.Value.ConfigureAwait(
                                        false
                                    );
                                    if (decompressedChunk == null) // We failed.
                                        return null;
                                    memoryStream.Write(
                                        decompressedChunk,
                                        0,
                                        decompressedChunk.Length
                                    );
                                }
                                if (memoryStream.Length == OriginalSize)
                                    return memoryStream.ToArray();
                            }

                            if (PrintErrors)
                                LoggerAccessor.LogError(
                                    "[SonyEdge] - LZMA - Segs: File size is different than the one indicated in TOC!."
                                );
                        }
                        else if (PrintErrors)
                            LoggerAccessor.LogError(
                                "[SonyEdge] - LZMA - Segs: The byte array length is not evenly divisible by 8!"
                            );
                    }
                    else if (PrintErrors)
                        LoggerAccessor.LogError(
                            "[SonyEdge] - LZMA - Segs: File is not a valid segment based EdgeLzma compressed file!"
                        );
                }
                catch (Exception ex)
                {
                    if (PrintErrors)
                        LoggerAccessor.LogError(
                            $"[SonyEdge] - LZMA - Segs: SegmentsDecompress thrown an assertion : {ex}"
                        );
                }

                return null;
            }).Result;
        }

        public static byte[] Decompress(byte[] CompressedData)
        {
            if (CompressedData == null || CompressedData.Length <= 12)
                throw new InvalidDataException(
                    "[SonyEdge] - LZMA - Decompress: buffer is not a valid EdgeLZMA compressed data"
                );

            if (
                BitConverter.ToInt32(
                    !EndianTools.EndianAwareConverter.isLittleEndianSystem
                        ? EndianUtils.ReverseArray(CompressedData)
                        : CompressedData,
                    8
                ) != CompressedData.Length
            )
                throw new InvalidDataException(
                    "[SonyEdge] - LZMA - Decompress: buffer length does not match declared buffer length"
                );
            else
            {
                switch (CompressedData[5])
                {
                    case 2:
                        return Decompress2(CompressedData);
                    case 4:
                        return Decompress4(CompressedData);
                }

                throw new InvalidDataException(
                    "[SonyEdge] - LZMA - Decompress: unknown compression type"
                );
            }
        }

        private static byte[] Decompress2(byte[] buffer)
        {
            throw new NotImplementedException();
        }

        private static byte[] Decompress4(byte[] buffer)
        {
            var chunkIndex = 0;
            var outSize = BitConverter.ToInt32(
                !EndianTools.EndianAwareConverter.isLittleEndianSystem
                    ? EndianUtils.ReverseArray(buffer)
                    : buffer,
                12
            );
            var streamCount = (outSize + ushort.MaxValue) >> 16;
            var offset = 0x18 + (streamCount * 2) + 5;

            var chunkBytes = new byte[LZMAChunkHeader.sizeOf];
            var properties = buffer.AsSpan(0x18, 5).ToArray();
            var dictionarySize = EndianAwareConverter.ToInt32(
                properties,
                Endianness.LittleEndian,
                1
            );

            var lzmaResults = new List<KeyValuePair<int, Task<byte[]>>>();

            for (var i = 0; i < streamCount; i++)
            {
                int currentOffset = offset;

                Array.Copy(buffer, 5 + 0x18 + (i * 2), chunkBytes, 0, chunkBytes.Length);

                int decompressedSize = Math.Min(outSize, dictionarySize);
                int compressedSize = LZMAChunkHeader.FromBytes(chunkBytes).CompressedSize;
                bool hasCompressedData = compressedSize != 0;

                if (!hasCompressedData)
                    compressedSize = decompressedSize;

                lzmaResults.Add(
                    new KeyValuePair<int, Task<byte[]>>(
                        chunkIndex,
                        Task.Run(() =>
                        {
                            using (
                                MemoryStream input = new MemoryStream(
                                    buffer,
                                    currentOffset,
                                    compressedSize
                                )
                            )
                            {
                                if (hasCompressedData)
                                {
                                    var decoder = new Decoder();
                                    decoder.SetDecoderProperties(properties);

                                    using (MemoryStream output = new MemoryStream(decompressedSize))
                                    {
                                        decoder.Code(
                                            input,
                                            output,
                                            compressedSize,
                                            decompressedSize,
                                            null
                                        );
                                        return output.ToArray();
                                    }
                                }
                                else
                                    return input.ToArray();
                            }
                        })
                    )
                );
                chunkIndex++;

                outSize -= dictionarySize;
                offset += compressedSize;
            }

            using (var memoryStream = new MemoryStream())
            {
                foreach (var result in lzmaResults.OrderBy(kv => kv.Key))
                {
                    var decompressedChunk = result.Value.GetAwaiter().GetResult(); // Preserve compatibility with assertion propagation.
                    memoryStream.Write(decompressedChunk, 0, decompressedChunk.Length);
                }

                return memoryStream.ToArray();
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
            var properties = new byte[5];
            inStream.Read(properties, 0, 5);
            var decoder = new Decoder();
            decoder.SetDecoderProperties(properties);
            long outSize = 0;
            for (var i = 0; i < 8; i++)
                outSize |= (long)(byte)inStream.ReadByte() << (8 * i);
            decoder.Code(inStream, outStream, inStream.Length - inStream.Position, outSize, null);
        }
    }
}
