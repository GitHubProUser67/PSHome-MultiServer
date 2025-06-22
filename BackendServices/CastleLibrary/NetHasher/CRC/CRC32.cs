using System;
#if NETCOREAPP3_0_OR_GREATER
using System.Runtime.Intrinsics.X86;
using System.Runtime.Intrinsics.Arm;
#endif
using EndianTools;

namespace NetHasher.CRC
{
    public static class CRC32
    {
        private const ushort crcArraySize = 256;

        private static readonly uint[] iso_hldc_hash = new uint[crcArraySize]
        {
            0x00000000, 0x77073096, 0xEE0E612C, 0x990951BA, 0x076DC419, 0x706AF48F, 0xE963A535, 0x9E6495A3,
            0x0EDB8832, 0x79DCB8A4, 0xE0D5E91E, 0x97D2D988, 0x09B64C2B, 0x7EB17CBD, 0xE7B82D07, 0x90BF1D91,
            0x1DB71064, 0x6AB020F2, 0xF3B97148, 0x84BE41DE, 0x1ADAD47D, 0x6DDDE4EB, 0xF4D4B551, 0x83D385C7,
            0x136C9856, 0x646BA8C0, 0xFD62F97A, 0x8A65C9EC, 0x14015C4F, 0x63066CD9, 0xFA0F3D63, 0x8D080DF5,
            0x3B6E20C8, 0x4C69105E, 0xD56041E4, 0xA2677172, 0x3C03E4D1, 0x4B04D447, 0xD20D85FD, 0xA50AB56B,
            0x35B5A8FA, 0x42B2986C, 0xDBBBC9D6, 0xACBCF940, 0x32D86CE3, 0x45DF5C75, 0xDCD60DCF, 0xABD13D59,
            0x26D930AC, 0x51DE003A, 0xC8D75180, 0xBFD06116, 0x21B4F4B5, 0x56B3C423, 0xCFBA9599, 0xB8BDA50F,
            0x2802B89E, 0x5F058808, 0xC60CD9B2, 0xB10BE924, 0x2F6F7C87, 0x58684C11, 0xC1611DAB, 0xB6662D3D,
            0x76DC4190, 0x01DB7106, 0x98D220BC, 0xEFD5102A, 0x71B18589, 0x06B6B51F, 0x9FBFE4A5, 0xE8B8D433,
            0x7807C9A2, 0x0F00F934, 0x9609A88E, 0xE10E9818, 0x7F6A0DBB, 0x086D3D2D, 0x91646C97, 0xE6635C01,
            0x6B6B51F4, 0x1C6C6162, 0x856530D8, 0xF262004E, 0x6C0695ED, 0x1B01A57B, 0x8208F4C1, 0xF50FC457,
            0x65B0D9C6, 0x12B7E950, 0x8BBEB8EA, 0xFCB9887C, 0x62DD1DDF, 0x15DA2D49, 0x8CD37CF3, 0xFBD44C65,
            0x4DB26158, 0x3AB551CE, 0xA3BC0074, 0xD4BB30E2, 0x4ADFA541, 0x3DD895D7, 0xA4D1C46D, 0xD3D6F4FB,
            0x4369E96A, 0x346ED9FC, 0xAD678846, 0xDA60B8D0, 0x44042D73, 0x33031DE5, 0xAA0A4C5F, 0xDD0D7CC9,
            0x5005713C, 0x270241AA, 0xBE0B1010, 0xC90C2086, 0x5768B525, 0x206F85B3, 0xB966D409, 0xCE61E49F,
            0x5EDEF90E, 0x29D9C998, 0xB0D09822, 0xC7D7A8B4, 0x59B33D17, 0x2EB40D81, 0xB7BD5C3B, 0xC0BA6CAD,
            0xEDB88320, 0x9ABFB3B6, 0x03B6E20C, 0x74B1D29A, 0xEAD54739, 0x9DD277AF, 0x04DB2615, 0x73DC1683,
            0xE3630B12, 0x94643B84, 0x0D6D6A3E, 0x7A6A5AA8, 0xE40ECF0B, 0x9309FF9D, 0x0A00AE27, 0x7D079EB1,
            0xF00F9344, 0x8708A3D2, 0x1E01F268, 0x6906C2FE, 0xF762575D, 0x806567CB, 0x196C3671, 0x6E6B06E7,
            0xFED41B76, 0x89D32BE0, 0x10DA7A5A, 0x67DD4ACC, 0xF9B9DF6F, 0x8EBEEFF9, 0x17B7BE43, 0x60B08ED5,
            0xD6D6A3E8, 0xA1D1937E, 0x38D8C2C4, 0x4FDFF252, 0xD1BB67F1, 0xA6BC5767, 0x3FB506DD, 0x48B2364B,
            0xD80D2BDA, 0xAF0A1B4C, 0x36034AF6, 0x41047A60, 0xDF60EFC3, 0xA867DF55, 0x316E8EEF, 0x4669BE79,
            0xCB61B38C, 0xBC66831A, 0x256FD2A0, 0x5268E236, 0xCC0C7795, 0xBB0B4703, 0x220216B9, 0x5505262F,
            0xC5BA3BBE, 0xB2BD0B28, 0x2BB45A92, 0x5CB36A04, 0xC2D7FFA7, 0xB5D0CF31, 0x2CD99E8B, 0x5BDEAE1D,
            0x9B64C2B0, 0xEC63F226, 0x756AA39C, 0x026D930A, 0x9C0906A9, 0xEB0E363F, 0x72076785, 0x05005713,
            0x95BF4A82, 0xE2B87A14, 0x7BB12BAE, 0x0CB61B38, 0x92D28E9B, 0xE5D5BE0D, 0x7CDCEFB7, 0x0BDBDF21,
            0x86D3D2D4, 0xF1D4E242, 0x68DDB3F8, 0x1FDA836E, 0x81BE16CD, 0xF6B9265B, 0x6FB077E1, 0x18B74777,
            0x88085AE6, 0xFF0F6A70, 0x66063BCA, 0x11010B5C, 0x8F659EFF, 0xF862AE69, 0x616BFFD3, 0x166CCF45,
            0xA00AE278, 0xD70DD2EE, 0x4E048354, 0x3903B3C2, 0xA7672661, 0xD06016F7, 0x4969474D, 0x3E6E77DB,
            0xAED16A4A, 0xD9D65ADC, 0x40DF0B66, 0x37D83BF0, 0xA9BCAE53, 0xDEBB9EC5, 0x47B2CF7F, 0x30B5FFE9,
            0xBDBDF21C, 0xCABAC28A, 0x53B39330, 0x24B4A3A6, 0xBAD03605, 0xCDD70693, 0x54DE5729, 0x23D967BF,
            0xB3667A2E, 0xC4614AB8, 0x5D681B02, 0x2A6F2B94, 0xB40BBE37, 0xC30C8EA1, 0x5A05DF1B, 0x2D02EF8D
        };

        private static readonly uint[] cdrom_edc_hash = new uint[crcArraySize]
        {
            0x00000000, 0x90910101,  0x91210201, 0x01B00300,   0x92410401, 0x02D00500,  0x03600600, 0x93F10701,
            0x94810801, 0x04100900,  0x05A00A00, 0x95310B01,   0x06C00C00, 0x96510D01,  0x97E10E01, 0x07700F00,
            0x99011001, 0x09901100,  0x08201200, 0x98B11301,   0x0B401400, 0x9BD11501,  0x9A611601, 0x0AF01700,
            0x0D801800, 0x9D111901,  0x9CA11A01, 0x0C301B00,   0x9FC11C01, 0x0F501D00,  0x0EE01E00, 0x9E711F01,
            0x82012001, 0x12902100,  0x13202200, 0x83B12301,   0x10402400, 0x80D12501,  0x81612601, 0x11F02700,
            0x16802800, 0x86112901,  0x87A12A01, 0x17302B00,   0x84C12C01, 0x14502D00,  0x15E02E00, 0x85712F01,
            0x1B003000, 0x8B913101,  0x8A213201, 0x1AB03300,   0x89413401, 0x19D03500,  0x18603600, 0x88F13701,
            0x8F813801, 0x1F103900,  0x1EA03A00, 0x8E313B01,   0x1DC03C00, 0x8D513D01,  0x8CE13E01, 0x1C703F00,
            0xB4014001, 0x24904100,  0x25204200, 0xB5B14301,   0x26404400, 0xB6D14501,  0xB7614601, 0x27F04700,
            0x20804800, 0xB0114901,  0xB1A14A01, 0x21304B00,   0xB2C14C01, 0x22504D00,  0x23E04E00, 0xB3714F01,
            0x2D005000, 0xBD915101,  0xBC215201, 0x2CB05300,   0xBF415401, 0x2FD05500,  0x2E605600, 0xBEF15701,
            0xB9815801, 0x29105900,  0x28A05A00, 0xB8315B01,   0x2BC05C00, 0xBB515D01,  0xBAE15E01, 0x2A705F00,
            0x36006000, 0xA6916101,  0xA7216201, 0x37B06300,   0xA4416401, 0x34D06500,  0x35606600, 0xA5F16701,
            0xA2816801, 0x32106900,  0x33A06A00, 0xA3316B01,   0x30C06C00, 0xA0516D01,  0xA1E16E01, 0x31706F00,
            0xAF017001, 0x3F907100,  0x3E207200, 0xAEB17301,   0x3D407400, 0xADD17501,  0xAC617601, 0x3CF07700,
            0x3B807800, 0xAB117901,  0xAAA17A01, 0x3A307B00,   0xA9C17C01, 0x39507D00,  0x38E07E00, 0xA8717F01,
            0xD8018001, 0x48908100,  0x49208200, 0xD9B18301,   0x4A408400, 0xDAD18501,  0xDB618601, 0x4BF08700,
            0x4C808800, 0xDC118901,  0xDDA18A01, 0x4D308B00,   0xDEC18C01, 0x4E508D00,  0x4FE08E00, 0xDF718F01,
            0x41009000, 0xD1919101,  0xD0219201, 0x40B09300,   0xD3419401, 0x43D09500,  0x42609600, 0xD2F19701,
            0xD5819801, 0x45109900,  0x44A09A00, 0xD4319B01,   0x47C09C00, 0xD7519D01,  0xD6E19E01, 0x46709F00,
            0x5A00A000, 0xCA91A101,  0xCB21A201, 0x5BB0A300,   0xC841A401, 0x58D0A500,  0x5960A600, 0xC9F1A701,
            0xCE81A801, 0x5E10A900,  0x5FA0AA00, 0xCF31AB01,   0x5CC0AC00, 0xCC51AD01,  0xCDE1AE01, 0x5D70AF00,
            0xC301B001, 0x5390B100,  0x5220B200, 0xC2B1B301,   0x5140B400, 0xC1D1B501,  0xC061B601, 0x50F0B700,
            0x5780B800, 0xC711B901,  0xC6A1BA01, 0x5630BB00,   0xC5C1BC01, 0x5550BD00,  0x54E0BE00, 0xC471BF01,
            0x6C00C000, 0xFC91C101,  0xFD21C201, 0x6DB0C300,   0xFE41C401, 0x6ED0C500,  0x6F60C600, 0xFFF1C701,
            0xF881C801, 0x6810C900,  0x69A0CA00, 0xF931CB01,   0x6AC0CC00, 0xFA51CD01,  0xFBE1CE01, 0x6B70CF00,
            0xF501D001, 0x6590D100,  0x6420D200, 0xF4B1D301,   0x6740D400, 0xF7D1D501,  0xF661D601, 0x66F0D700,
            0x6180D800, 0xF111D901,  0xF0A1DA01, 0x6030DB00,   0xF3C1DC01, 0x6350DD00,  0x62E0DE00, 0xF271DF01,
            0xEE01E001, 0x7E90E100,  0x7F20E200, 0xEFB1E301,   0x7C40E400, 0xECD1E501,  0xED61E601, 0x7DF0E700,
            0x7A80E800, 0xEA11E901,  0xEBA1EA01, 0x7B30EB00,   0xE8C1EC01, 0x7850ED00,  0x79E0EE00, 0xE971EF01,
            0x7700F000, 0xE791F101,  0xE621F201, 0x76B0F300,   0xE541F401, 0x75D0F500,  0x7460F600, 0xE4F1F701,
            0xE381F801, 0x7310F900,  0x72A0FA00, 0xE231FB01,   0x71C0FC00, 0xE151FD01,  0xE0E1FE01, 0x7070FF00
        };

        private static readonly uint[] castagnoli_hash = GenerateCRCTable(0x82F63B78);
        private static readonly uint[] koopman_hash = GenerateCRCTable(0xEB31D82E);
        private static readonly uint[] crc_32q_hash = GenerateCRCTable(0xD5828281);

        private static uint[] GenerateCRCTable(uint a_poly32)
        {
            uint[] m_crc_tab = new uint[crcArraySize];

            for (uint i = 0; i < crcArraySize; ++i)
            {
                uint crc = i;

                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 1) == 1)
                        crc = (crc >> 1) ^ a_poly32;
                    else
                        crc >>= 1;
                }

                m_crc_tab[i] = crc;
            }

            return m_crc_tab;
        }

        public static uint Create(byte[] data)
        {
            return Create(data, 0, data.Length);
        }

        public static uint Create(byte[] data, int offset, int length)
        {
            // Ensure the length doesn't exceed the array's length
            length = Math.Min(length, data.Length);

            uint crc = uint.MaxValue;

            for (int i = offset; i < length; i++)
            {
                byte b = data[i];
                crc = crc >> 8 ^ iso_hldc_hash[(crc ^ b) & byte.MaxValue];
            }

            return ~crc;
        }

        public static uint CreateEdc(byte[] data)
        {
            return CreateEdc(data, 0, data.Length);
        }

        public static uint CreateEdc(byte[] data, int offset, int length)
        {
            // Ensure the length doesn't exceed the array's length
            length = Math.Min(length, data.Length);

            uint crc = 0;

            for (int i = offset; i < length; i++)
            {
                byte b = data[i];
                crc = crc >> 8 ^ cdrom_edc_hash[(crc ^ b) & byte.MaxValue];
            }

            return crc;
        }

        public static uint CreateCastagnoli(byte[] data)
        {
            return CreateCastagnoli(data, 0, data.Length);
        }

        public static uint CreateCastagnoli(byte[] data, int offset, int length)
        {
            // Ensure the length doesn't exceed the array's length
            length = Math.Min(length, data.Length);

            uint crc = uint.MaxValue;

            // X86 SIMD uses the Castagnoli method only, ARM supports booth this and the IEEE compilant one.
#if NETCOREAPP3_0_OR_GREATER
            if (Crc32.IsSupported)
            {
                if (Crc32.Arm64.IsSupported)
                {
                    while (length >= 8)
                    {
                        // ARM64 is always little-endian, no need to check for that info while using the BitConverter.
                        crc = Crc32.Arm64.ComputeCrc32C(crc, BitConverter.ToUInt64(data, offset));
                        offset += 8;
                        length -= 8;
                    }
                }

                while (length >= 4)
                {
                    crc = Crc32.ComputeCrc32C(crc, BitConverter.ToUInt32(!BitConverter.IsLittleEndian ? EndianUtils.EndianSwap(data) : data, offset));
                    offset += 4;
                    length -= 4;
                }

                while (length >= 2)
                {
                    crc = Crc32.ComputeCrc32C(crc, BitConverter.ToUInt16(!BitConverter.IsLittleEndian ? EndianUtils.EndianSwap(data) : data, offset));
                    offset += 2;
                    length -= 2;
                }

                while (length > 0)
                {
                    crc = Crc32.ComputeCrc32C(crc, data[offset]);
                    offset++;
                    length--;
                }

                return ~crc;
            }
            else if (Sse42.IsSupported)
            {
                if (Sse42.X64.IsSupported)
                {
                    while (length >= 8)
                    {
                        // X86_64 is always little-endian, no need to check for that info while using the BitConverter.
                        crc = (uint)Sse42.X64.Crc32(crc, BitConverter.ToUInt64(data, offset));
                        offset += 8;
                        length -= 8;
                    }
                }

                while (length >= 4)
                {
                    // X86 is always little-endian, no need to check for that info while using the BitConverter.
                    crc = (uint)Sse42.X64.Crc32(crc, BitConverter.ToUInt32(data, offset));
                    offset += 4;
                    length -= 4;
                }

                while (length >= 2)
                {
                    // X86 is always little-endian, no need to check for that info while using the BitConverter.
                    crc = (uint)Sse42.X64.Crc32(crc, BitConverter.ToUInt16(data, offset));
                    offset += 2;
                    length -= 2;
                }

                while (length > 0)
                {
                    crc = Sse42.Crc32(crc, data[offset]);
                    offset++;
                    length--;
                }

                return ~crc;
            }
#endif
            for (int i = offset; length > 0; i++, length--)
                crc = crc >> 8 ^ castagnoli_hash[(byte)crc ^ data[i]];

            return crc ^ uint.MaxValue;
        }

        public static uint CreateKoopman(byte[] data)
        {
            return CreateKoopman(data, 0, data.Length);
        }

        public static uint CreateKoopman(byte[] data, int offset, int length)
        {
            // Ensure the length doesn't exceed the array's length
            length = Math.Min(length, data.Length);

            uint crc = uint.MaxValue;

            for (int i = offset; length > 0; i++, length--)
                crc = crc >> 8 ^ koopman_hash[(byte)crc ^ data[i]];

            return crc ^ uint.MaxValue;
        }

        public static uint CreateCRC32q(byte[] data)
        {
            return CreateCRC32q(data, 0, data.Length);
        }

        public static uint CreateCRC32q(byte[] data, int offset, int length)
        {
            // Ensure the length doesn't exceed the array's length
            length = Math.Min(length, data.Length);

            uint crc = uint.MaxValue;

            for (int i = offset; length > 0; i++, length--)
                crc = crc >> 8 ^ crc_32q_hash[(byte)crc ^ data[i]];

            return crc ^ uint.MaxValue;
        }
    }
}
