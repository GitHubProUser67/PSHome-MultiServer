using CastleLibrary.SharpCompress.LZMA;
using EndianTools;

namespace CastleLibrary.S0ny.Edge
{
    public class LZMASegsCompressor(int maxSegmentSize)
    {
        private const uint _segsMagic = 0x73656773; // "segs"
        private const int _defaultDictSize = 65536;
        private const byte _edgeLzmaType = 1;
        private const byte _fileVersion = 5;

        private const bool _eos = false;

        private const string _matchFinder = "BT4";

        private static readonly CoderPropId[] _propIDs =
        [
            CoderPropId.Algorithm,
            CoderPropId.DictionarySize,
            CoderPropId.NumFastBytes,
            CoderPropId.LitContextBits,
            CoderPropId.LitPosBits,
            CoderPropId.PosStateBits,
            CoderPropId.MatchFinder,
            CoderPropId.EndMarker,
        ];

        private int MaxSegmentSize { get; set; } = maxSegmentSize;

        private sealed class SegmentResult
        {
            public int UncompressedSize;
            public byte[] StoredBytes = [];
            public bool IsCompressed;
        }

        public byte[] CompressToSegs(byte[] input, int level = 9)
        {
            return CompressToSegs(input, level, Environment.ProcessorCount * 2);
        }

        public byte[] CompressToSegs(byte[] input, int level, int maxDegreeOfParallelism)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            else if (level < 0 || level > 9)
                throw new ArgumentOutOfRangeException(
                    nameof(level),
                    "[LZMASegsCompressor] - Compression level must be 0 through 9."
                );
            else if (maxDegreeOfParallelism <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(maxDegreeOfParallelism),
                    "[LZMASegsCompressor] - maxDegreeOfParallelism must be greater than 0."
                );

            const int headerSizeOf = 16;
            int inputLength = input.Length;
            int numSegments = (inputLength + (MaxSegmentSize - 1)) / MaxSegmentSize;
            int headerAndTocSize = headerSizeOf + (numSegments * 8);

            SegmentResult[] results = new SegmentResult[numSegments];

            Parallel.For(
                0,
                numSegments,
                new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism },
                segNo =>
                {
                    int srcOffset = segNo * MaxSegmentSize;
                    int uncompSegSize = Math.Min(MaxSegmentSize, inputLength - srcOffset);
                    using (var inputStream = new MemoryStream())
                    using (var outputStream = new MemoryStream())
                    {
                        inputStream.Write(input, srcOffset, uncompSegSize);

                        inputStream.Position = 0;

                        SegmentCompress(inputStream, outputStream, level);

                        byte[] compressed = outputStream.ToArray();
                        bool storeCompressed = compressed.Length < uncompSegSize;

                        if (storeCompressed)
                            results[segNo] = new SegmentResult
                            {
                                UncompressedSize = uncompSegSize,
                                StoredBytes = compressed,
                                IsCompressed = true,
                            };
                        else
                        {
                            byte[] rawSegment = new byte[uncompSegSize];
                            Array.Copy(input, srcOffset, rawSegment, 0, uncompSegSize);

                            results[segNo] = new SegmentResult
                            {
                                UncompressedSize = uncompSegSize,
                                StoredBytes = rawSegment,
                                IsCompressed = false,
                            };
                        }
                    }
                }
            );

            byte[] toc = new byte[numSegments * 8];
            using MemoryStream payload = new MemoryStream();

            int fileOffset = headerAndTocSize;

            for (int segNo = 0; segNo < numSegments; segNo++)
            {
                SegmentResult result = results[segNo];
                int storedSize = result.StoredBytes.Length;

                WriteBE16(toc, segNo * 8 + 0, storedSize);
                WriteBE16(toc, segNo * 8 + 2, result.UncompressedSize);
                EndianAwareConverter.WriteUInt32(
                    toc,
                    Endianness.BigEndian,
                    (uint)(segNo * 8 + 4),
                    (uint)fileOffset
                        + ((storedSize != result.UncompressedSize && result.IsCompressed) ? 1u : 0u)
                ); // Simulate the -1 SDK bug (according to test data).

                payload.Write(result.StoredBytes, 0, storedSize);

                fileOffset += storedSize;

                int remainder = fileOffset % headerSizeOf;
                if (remainder != 0)
                {
                    remainder = headerSizeOf - remainder;
                    byte[] padding = new byte[remainder];
                    payload.Write(padding, 0, padding.Length);

                    fileOffset += remainder;
                }
            }

            using MemoryStream output = new MemoryStream(fileOffset);

            WriteBE32(output, _segsMagic);
            output.WriteByte(_edgeLzmaType);
            output.WriteByte(_fileVersion);
            WriteBE16(output, numSegments);
            WriteBE32(output, (uint)inputLength);
            WriteBE32(output, (uint)fileOffset);

            output.Write(toc, 0, toc.Length);

            payload.Position = 0;
            payload.CopyTo(output);

            return output.ToArray();
        }

        /// <summary>
        /// Compress a block of the segmented EdgeLZMA data.
        /// <para>Compresse un block provenant d'une matrice de donn�e encod�e avec le codec EdgeLZMA.</para>
        /// </summary>
        /// <param name="inStream">The input stream.</param>
        /// <param name="outStream">The output LZMA stream.</param>
        /// <param name="level">The compression level.</param>
        /// <returns>Nothing.</returns>
        public static void SegmentCompress(Stream inStream, Stream outStream, int level)
        {
            Encoder encoder = new Encoder();

            GetEncoderProps(
                level,
                out int algorithm,
                out int dictSize,
                out int fastBytes,
                out int lc,
                out int lp,
                out int pb
            );

            encoder.SetCoderProperties(
                _propIDs,
                [algorithm, dictSize, fastBytes, lc, lp, pb, _matchFinder, _eos]
            );
            encoder.WriteCoderProperties(outStream);

            long fileSize = inStream.Length;

            for (int i = 0; i < 8; i++)
                outStream.WriteByte((byte)(fileSize >> (8 * i)));

            encoder.Code(inStream, outStream, -1, -1, null);
        }

        private static void GetEncoderProps(
            int level,
            out int algorithm,
            out int dictSize,
            out int fastBytes,
            out int lc,
            out int lp,
            out int pb
        )
        {
            switch (level)
            {
                case 0:
                    algorithm = 0;
                    dictSize = 1024;
                    fastBytes = 32;
                    lc = 3;
                    lp = 0;
                    pb = 2;
                    break;
                case 1:
                    algorithm = 0;
                    dictSize = _defaultDictSize / 16;
                    fastBytes = 32;
                    lc = 0;
                    lp = 0;
                    pb = 0;
                    break;
                case 2:
                    algorithm = 1;
                    dictSize = _defaultDictSize / 16;
                    fastBytes = 32;
                    lc = 0;
                    lp = 0;
                    pb = 0;
                    break;
                case 3:
                    algorithm = 1;
                    dictSize = _defaultDictSize / 8;
                    fastBytes = 32;
                    lc = 0;
                    lp = 0;
                    pb = 1;
                    break;
                case 4:
                    algorithm = 1;
                    dictSize = _defaultDictSize / 8;
                    fastBytes = 64;
                    lc = 1;
                    lp = 0;
                    pb = 1;
                    break;
                case 5:
                    algorithm = 1;
                    dictSize = _defaultDictSize / 4;
                    fastBytes = 64;
                    lc = 1;
                    lp = 0;
                    pb = 1;
                    break;
                case 6:
                    algorithm = 1;
                    dictSize = _defaultDictSize / 4;
                    fastBytes = 64;
                    lc = 1;
                    lp = 0;
                    pb = 2;
                    break;
                case 7:
                    algorithm = 1;
                    dictSize = _defaultDictSize / 2;
                    fastBytes = 64;
                    lc = 1;
                    lp = 0;
                    pb = 2;
                    break;
                case 8:
                    algorithm = 1;
                    dictSize = _defaultDictSize;
                    fastBytes = 64;
                    lc = 3;
                    lp = 0;
                    pb = 1;
                    break;
                default:
                    algorithm = 1;
                    dictSize = _defaultDictSize;
                    fastBytes = 64;
                    lc = 3;
                    lp = 0;
                    pb = 2;
                    break;
            }
        }

        private static void WriteBE16(byte[] buffer, int offset, int value)
        {
            EndianAwareConverter.WriteUInt16(
                buffer,
                Endianness.BigEndian,
                (uint)offset,
                (ushort)(value == _defaultDictSize ? 0 : value)
            );
        }

        private static void WriteBE16(Stream stream, int value)
        {
            ushort v = (ushort)(value == _defaultDictSize ? 0 : value);
            stream.WriteByte((byte)(v >> 8));
            stream.WriteByte((byte)v);
        }

        private static void WriteBE32(Stream stream, uint value)
        {
            stream.WriteByte((byte)(value >> 24));
            stream.WriteByte((byte)(value >> 16));
            stream.WriteByte((byte)(value >> 8));
            stream.WriteByte((byte)value);
        }
    }
}
