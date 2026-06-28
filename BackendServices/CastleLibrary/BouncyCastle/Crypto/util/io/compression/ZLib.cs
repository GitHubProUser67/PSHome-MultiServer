using System.IO;
using System.IO.Compression;

namespace Org.BouncyCastle.Utilities.IO.Compression
{
    internal static class ZLib
    {
        internal static Stream CompressOutput(
            Stream stream,
            int zlibCompressionLevel,
            bool leaveOpen = false
        )
        {
            return new ZLibStream(stream, GetCompressionLevel(zlibCompressionLevel), leaveOpen);
        }

        internal static Stream DecompressInput(Stream stream, bool leaveOpen = false)
        {
            return new ZLibStream(stream, CompressionMode.Decompress, leaveOpen);
        }

        internal static CompressionLevel GetCompressionLevel(int zlibCompressionLevel)
        {
            return zlibCompressionLevel switch
            {
                0 => CompressionLevel.NoCompression,
                1 or 2 or 3 => CompressionLevel.Fastest,
                7 or 8 or 9 => CompressionLevel.SmallestSize,
                _ => CompressionLevel.Optimal,
            };
        }
    }
}
