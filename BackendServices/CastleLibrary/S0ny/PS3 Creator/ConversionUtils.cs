using System.Numerics;

namespace CastleLibrary.S0ny.PS3_Creator
{
    public class ConversionUtils
    {
        public static BigInteger Be64(byte[] buffer, int initOffset)
        {
            var result = BigInteger.Zero;
            for (var i = initOffset; i < initOffset + 8; i++)
            {
                result *= new BigInteger(256);
                result += new BigInteger(buffer[i] & byte.MaxValue);
            }
            return result;
        }

        public static long Be32(byte[] buffer, int initOffset)
        {
            long result = 0;
            for (var i = initOffset; i < initOffset + 4; i++)
                result = (result * 256) + (buffer[i] & byte.MaxValue);
            return result;
        }

        public static int Be16(byte[] buffer, int initOffset)
        {
            var result = 0;
            for (var i = initOffset; i < initOffset + 2; i++)
                result = (result * 256) + (buffer[i] & byte.MaxValue);
            return result;
        }

        public static void Arraycopy(byte[] src, int srcPos, byte[] dest, long destPos, int length)
        {
            for (var i = 0; i < length; i++)
                dest[destPos + i] = src[srcPos + i];
        }

        public static char[] BytesToChar(byte[] b)
        {
            var c = new char[b.Length];
            for (var i = 0; i < b.Length; i++)
                c[i] = (char)b[i];
            return c;
        }

        public static byte[] ReverseByteWithSizeFIX(byte[] b)
        {
            var b2 =
                b[b.Length - 1] == byte.MinValue ? (new byte[b.Length - 1]) : (new byte[b.Length]);
            for (var i = 0; i < b2.Length; i++)
                b2[b2.Length - 1 - i] = b[i];
            return b2;
        }

        public static byte[] CharsToByte(char[] b)
        {
            var c = new byte[b.Length];
            for (var i = 0; i < b.Length; i++)
                c[i] = (byte)b[i];
            return c;
        }
    }
}
