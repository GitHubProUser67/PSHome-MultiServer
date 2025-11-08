using System.Numerics;

namespace HomeTools.PS3_Creator
{
    public class EDATData
    {
        public long flags;
        public long blockSize;
        public BigInteger fileLen;

        public EDATData()
        {
        }

        public static EDATData createEDATData(byte[] data)
        {
            EDATData result = new EDATData();
            result.flags = ConversionUtils.Be32(data, 0);
            result.blockSize = ConversionUtils.Be32(data, 4);
            result.fileLen = ConversionUtils.Be64(data, 0x8);
            return result;
        }

        public long getBlockSize()
        {
            return blockSize;
        }

        public BigInteger getFileLen()
        {
            return fileLen;
        }

        public long getFlags()
        {
            return flags;
        }
    }
}
