using CustomLogger;
using System;
using System.Text;

namespace WebAPIService.GameServices.OHS
{
    public class EncryptDecrypt
    {
        private static int Wrapped(int index, int max)
        {
            if (index > max)
                return 1 + (index % (max + 1));
            else
                return index;
        }

        public static string Encrypt(string str, int offset, int game)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            if (offset > StaticKeys.keyLength)
            {
                LoggerAccessor.LogError("[OHS - Encrypt] - Offset is too large.");
                return null;
            }

            StringBuilder chars = new StringBuilder();

            chars.Append((char)(((int)Math.Floor((double)((offset - 1) / 95))) + 32));
            chars.Append((char)(((offset - 1) % 95) + 32));

            for (int i = 0; i < str.Length; i++)
            {
                int srcbyte = str[i] - 32;
                if (srcbyte < 0 || srcbyte > 95)
                {
                    LoggerAccessor.LogError("[OHS - Encrypt] - Invalid character in input string");
                    return null;
                }

                int cipherbyte;

                if (game == 1)
                    cipherbyte = StaticKeys.version1cipher[Wrapped(i + 1 + offset, StaticKeys.version1cipher.Length) - 1];
                else if (game == 2)
                    cipherbyte = StaticKeys.version2cipher[Wrapped(i + 1 + offset, StaticKeys.version2cipher.Length) - 1];
                else
                    return null;

                chars.Append((char)((srcbyte + cipherbyte) % 95 + 32));
            }

            return chars.ToString();
        }

        public static string Decrypt(string str, int game)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            StringBuilder chars = new StringBuilder();

            int offset = ((str[0] - 32) * 95) + (str[1] - 32) + 1;

            for (int i = 3; i <= str.Length; i++)
            {
                int srcbyte = str[i - 1] - 32;
                if (srcbyte < 0 || srcbyte > 95)
                {
                    LoggerAccessor.LogError("[OHS - Decrypt] - Invalid character in input string");
                    return null;
                }

                int cipherbyte;

                if (game == 1)
                    cipherbyte = StaticKeys.version1cipher[Wrapped(i - 2 + offset, StaticKeys.version1cipher.Length) - 1];
                else if (game == 2)
                    cipherbyte = StaticKeys.version2cipher[Wrapped(i - 2 + offset, StaticKeys.version2cipher.Length) - 1];
                else
                    return null;

                chars.Append((char)(((srcbyte - cipherbyte) % 95 + 95) % 95 + 32));
            }

            return chars.ToString();
        }

        private static ushort Hash16(string str, ushort histart, ushort lostart)
        {
            ushort hi = histart, lo = lostart;
            for (int i = 0; i < str.Length; i++)
            {
                byte b = (byte)str[i];
                lo = (ushort)((b + lo) % 255);
                hi = (ushort)((255 - b + hi) % 255);

                ushort lolo = (ushort)(lo % 16);
                ushort lohi = (ushort)(lo / 16);
                ushort hilo = (ushort)(hi % 16);
                ushort hihi = (ushort)(hi / 16);

                lo = (ushort)(hilo * 16 + lolo);
                hi = (ushort)(hihi * 16 + lohi);

                ushort temp = lo;
                lo = hi;
                hi = temp;
            }

            return (ushort)(hi * 255 + lo);
        }

        private static (ushort, ushort) Hash32(string str)
        {
            return (Hash16(str, 170, 204), Hash16(str, 11, 252));
        }

        public static string Hash32Str(string str)
        {
            (ushort hi, ushort lo) = Hash32(str);
            return string.Format("{0:X4}{1:X4}", hi, lo);
        }

        public static string UnEscape(string str)
        {
            // Replace '&a' with '&' and '&m' with '%'
            return str.Replace("&a", "&").Replace("&m", "%");
        }
    }
}
