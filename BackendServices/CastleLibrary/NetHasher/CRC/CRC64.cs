using System;

namespace NetHasher.CRC
{
    public static class CRC64
    {
        private static readonly ulong[] iso_hash = GenerateCRCTable(0xD800000000000000);
        private static readonly ulong[] ecma_182_hash = GenerateCRCTable(0xC96C5795D7870F42);

        private static ulong[] GenerateCRCTable(ulong a_poly64)
        {
            ulong[] m_crc_tab = new ulong[256];

            for (uint i = 0; i < 256; ++i)
            {
                ulong crc = i;

                for (uint j = 0; j < 8; ++j)
                {
                    if ((crc & 1) == 1)
                        crc = (crc >> 1) ^ a_poly64;
                    else
                        crc >>= 1;
                }

                m_crc_tab[i] = crc;
            }

            return m_crc_tab;
        }

        public static ulong Create(byte[] data)
        {
            return Create(data, 0, data.Length);
        }

        public static ulong Create(byte[] data, int offset, int length)
        {
            // Ensure the length doesn't exceed the array's length
            length = Math.Min(length, data.Length);

            ulong crc = ulong.MaxValue;

            for (int i = offset; length > 0; i++, length--)
                crc = crc >> 8 ^ iso_hash[(byte)crc ^ data[i]];

            return crc ^ ulong.MaxValue;
        }

        public static ulong CreateECMA128(byte[] data)
        {
            return CreateECMA128(data, 0, data.Length);
        }

        public static ulong CreateECMA128(byte[] data, int offset, int length)
        {
            // Ensure the length doesn't exceed the array's length
            length = Math.Min(length, data.Length);

            ulong crc = ulong.MaxValue;

            for (int i = offset; length > 0; i++, length--)
                crc = crc >> 8 ^ ecma_182_hash[(byte)crc ^ data[i]];

            return crc ^ ulong.MaxValue;
        }
    }
}
