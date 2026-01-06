using System.Numerics;

namespace CastleLibrary.S0ny.PS3_Creator
{
    public class EDATData
    {
        public long flags;
        public long blockSize;
        public BigInteger fileLen;

        public EDATData()
        {
        }

        public static EDATData CreateEDATData(byte[] data)
        {
            return new EDATData
            {
                flags = ConversionUtils.Be32(data, 0),
                blockSize = ConversionUtils.Be32(data, 4),
                fileLen = ConversionUtils.Be64(data, 0x8)
            };
        }

        public long GetBlockSize()
        {
            return blockSize;
        }

        public BigInteger GetFileLen()
        {
            return fileLen;
        }

        public long GetFlags()
        {
            return flags;
        }
    }
}
