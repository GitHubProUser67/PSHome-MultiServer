using System.IO;
using System.IO.Compression;

namespace Org.BouncyCastle.Utilities.IO.Compression
{
    internal static class Zip
    {
        internal static Stream CompressOutput(
            Stream stream,
            int zlibCompressionLevel,
            bool leaveOpen = false
        )
        {
            return new DeflateStream(
                stream,
                ZLib.GetCompressionLevel(zlibCompressionLevel),
                leaveOpen
            );
        }

        internal static Stream DecompressInput(Stream stream, bool leaveOpen = false)
        {
            return new DeflateStream(stream, CompressionMode.Decompress, leaveOpen);
        }
    }
}
