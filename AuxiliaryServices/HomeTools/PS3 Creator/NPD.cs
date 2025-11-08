using System.Numerics;

namespace HomeTools.PS3_Creator
{
    class NPD
    {
        private byte[] magic;
        private long version;
        private long license; /* 1 network, 2 local, 3 free */
        private long type; /* 1 exec, 21 update */
        private byte[] content_id;
        private byte[] digest;
        private byte[] titleHash;
        private byte[] devHash;
        private BigInteger unknown3;
        private BigInteger unknown4;
        private NPD()
        {
            magic = new byte[4];
            content_id = new byte[0x30];
            digest = new byte[0x10];
            titleHash = new byte[0x10];
            devHash = new byte[0x10];
        }

        public static NPD createNPD(byte[] npd)
        {
            NPD result = new NPD();
            ConversionUtils.Arraycopy(npd, 0, result.magic, 0, 4);
            result.version = ConversionUtils.Be32(npd, 4);
            result.license = ConversionUtils.Be32(npd, 8);
            result.type = ConversionUtils.Be32(npd, 0xC);
            ConversionUtils.Arraycopy(npd, 0x10, result.content_id, 0, 0x30);
            ConversionUtils.Arraycopy(npd, 0x40, result.digest, 0, 0x10);
            ConversionUtils.Arraycopy(npd, 0x50, result.titleHash, 0, 0x10);
            ConversionUtils.Arraycopy(npd, 0x60, result.devHash, 0, 0x10);
            result.unknown3 = ConversionUtils.Be64(npd, 0x70);
            result.unknown4 = ConversionUtils.Be64(npd, 0x78);
            if (!result.validate()) result = null;
            return result;
        }

        public byte[] getDevHash()
        {
            return devHash;
        }

        public byte[] getDigest()
        {
            return digest;
        }

        public long getLicense()
        {
            return license;
        }

        public long getVersion()
        {
            return version;
        }

        private bool validate()
        {
            if (magic[0] != 0x4E || magic[1] != 0x50 || magic[2] != 0x44 || magic[3] != 0x00) return false;
            if (unknown3.CompareTo(BigInteger.Zero) != 0 || unknown4.CompareTo(BigInteger.Zero) != 0) return false;
            return true;
        }
    }
}
