using System.Numerics;

namespace HomeTools.PS3_Creator
{
    public class ConversionUtils
    {
        public static BigInteger Be64(byte[] buffer, int initOffset)
        {
            BigInteger result = BigInteger.Zero;
            for (int i = initOffset; i < initOffset + 8; i++)
            {
                result *= new BigInteger(256);
                result += new BigInteger(buffer[i] & byte.MaxValue);
            }
            return result;
        }

        public static long Be32(byte[] buffer, int initOffset)
        {
            long result = 0;
            for (int i = initOffset; i < initOffset + 4; i++)
                result = result * 256 + (buffer[i] & byte.MaxValue);
            return result;
        }

        public static int Be16(byte[] buffer, int initOffset)
        {
            int result = 0;
            for (int i = initOffset; i < initOffset + 2; i++)
                result = result * 256 + (buffer[i] & byte.MaxValue);
            return result;
        }

        public static void Arraycopy(byte[] src, int srcPos, byte[] dest, long destPos, int length)
        {
            for (int i = 0; i < length; i++)
                dest[destPos + i] = src[srcPos + i];
        }

        public static char[] BytesToChar(byte[] b)
        {
            char[] c = new char[b.Length];
            for (int i = 0; i < b.Length; i++)
                c[i] = (char)b[i];
            return c;
        }

        public static byte[] ReverseByteWithSizeFIX(byte[] b)
        {

            byte[] b2;
            if(b[b.Length -1] == byte.MinValue)
                b2 = new byte[b.Length - 1];
            else
                b2 = new byte[b.Length];
            for (int i = 0; i < b2.Length; i++)
                b2[b2.Length - 1 - i] = b[i];
            return b2;
        }

        public static byte[] CharsToByte(char[] b)
        {
            byte[] c = new byte[b.Length];
            for (int i = 0; i < b.Length; i++)
                c[i] = (byte)b[i];
            return c;
        }
    }
}
