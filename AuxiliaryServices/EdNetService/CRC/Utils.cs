using System.Linq;

namespace EdNetService.CRC
{
    public class Utils
    {
        private static readonly object _lock = new object();

        private static bool IsCRCTableInitiated = false;

        private static readonly uint[] CRCTable = new uint[256];

        public static string GetCRCFromStringHexadecimal(string s)
        {
            return "0x" + GetCRCFromString(s).ToString("X").PadLeft(4, '0');
        }

        public static ushort GetCRCFromString(string s)
        {
            return GetCRCFromBuffer(s.ToCharArray());
        }

        public static ushort GetCRCFromBuffer(char[] b)
        {
            uint CRCValue = GetCRCFromBuffer32(b);
            return (ushort)((CRCValue ^ uint.MaxValue) & 65535U ^ ((CRCValue ^ uint.MaxValue) & 4294901760U) >> 16);
        }

        public static ushort GetCRCFromBuffer(byte[] b)
        {
            uint CRCValue = GetCRCFromBuffer32(b);
            return (ushort)((CRCValue ^ uint.MaxValue) & 65535U ^ ((CRCValue ^ uint.MaxValue) & 4294901760U) >> 16);
        }

        private static uint GetCRCFromBuffer32(char[] b)
        {
            uint CRCValue = uint.MaxValue;

            if (!IsCRCTableInitiated)
                InitializeCRCTable();

            foreach (byte byteValue in b.Select(v => (byte)v))
                CRCValue = CRCValue >> 8 ^ CRCTable[(CRCValue ^ byteValue) & byte.MaxValue];

            return ~CRCValue;
        }

        private static uint GetCRCFromBuffer32(byte[] b)
        {
            uint CRCValue = uint.MaxValue;

            if (!IsCRCTableInitiated)
                InitializeCRCTable();

            foreach (byte byteValue in b)
                CRCValue = CRCValue >> 8 ^ CRCTable[(CRCValue ^ byteValue) & byte.MaxValue];

            return ~CRCValue;
        }

        private static void InitializeCRCTable()
        {
            uint uVar2 = 0;

            lock (_lock)
            {
                do
                {
                    int iVar1 = 8;
                    uint uVar3 = uVar2;

                    do
                    {
                        if ((uVar3 & 1) == 0)
                            uVar3 >>= 1;
                        else
                            uVar3 = uVar3 >> 1 ^ 0xEDB88320;

                        iVar1--;

                    } while (iVar1 > 0);

                    CRCTable[uVar2] = uVar3;

                    uVar2++;

                } while (uVar2 < 256);

                IsCRCTableInitiated = true;
            }
        }
    }
}
