using SonyEdge;

namespace HomeTools.BARFramework
{
    public class EdgeZLibCompression : CompressionBase
    {
        public override byte[] Compress(TOCEntry te)
        {
            return Compress(te.RawData);
        }

        public override byte[] Decrypt(TOCEntry te)
        {
            return Decompress(te.RawData);
        }

        public override byte[] Compress(byte[] inData)
        {
            return Zlib.EdgeZlibCompress(inData);
        }

        public override byte[] Decompress(byte[] inData)
        {
            return Zlib.EdgeZlibDecompress(inData);
        }

        public override CompressionMethod Method
        {
            get
            {
                return CompressionMethod.EdgeZLib;
            }
        }
    }
}