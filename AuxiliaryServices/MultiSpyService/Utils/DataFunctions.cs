using MultiServerLibrary.Extension;
using System;
using System.Text;

namespace MultiSpyService.Utils
{
    public static class DataFunctions
    {
        private static readonly object _InternalLock = new object();

        public static string GetString(this Random rand, int length)
        {
            char[] array = new char[length];
            lock (_InternalLock)
            {
                for (int i = 0; i < length; i++)
                {
                    array[i] = FileSystemUtils.ASCIIChars[rand.Next(62)];
                }
            }
            return new string(array);
        }

        public static string GetString(this Random rand, int length, string chars)
        {
            char[] array = new char[length];
            lock (_InternalLock)
            {
                for (int i = 0; i < length; i++)
                {
                    array[i] = chars[rand.Next(chars.Length)];
                }
            }
            return new string(array);
        }

        public static byte[] StringToBytes(string data)
        {
            return Encoding.GetEncoding("ISO-8859-1").GetBytes(data);
        }

        public static string BytesToString(byte[] data)
        {
            return Encoding.GetEncoding("ISO-8859-1").GetString(data);
        }
    }
}
