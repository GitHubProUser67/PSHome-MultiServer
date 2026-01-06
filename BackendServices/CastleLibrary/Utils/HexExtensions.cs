using System;
using System.Runtime.InteropServices;
using System.Text;
#if !NET5_0_OR_GREATER
using System.Linq;
#endif
namespace CastleLibrary.Utils
{
    public static class HexExtensions
    {
        private static readonly uint[] _lookup32Unsafe = CreateLookup32Unsafe();
        private unsafe static readonly uint* _lookup32UnsafeP = (uint*)GCHandle.Alloc(_lookup32Unsafe, GCHandleType.Pinned).AddrOfPinnedObject();

        private static uint[] CreateLookup32Unsafe()
        {
            uint[] result = new uint[256];
            for (int i = 0; i < 256; i++)
            {
                string s = i.ToString("X2");
                if (BitConverter.IsLittleEndian)
                    result[i] = ((uint)s[0]) + ((uint)s[1] << 16);
                else
                    result[i] = ((uint)s[1]) + ((uint)s[0] << 16);
            }
            return result;
        }

        /// <summary>
        /// Transform a byte array to it's hexadecimal representation.
        /// <para>Obtenir un tableau de bytes dans sa représentation hexadecimale.</para>
        /// <param name="bytes">The byte array to transform.</param>
        /// </summary>
        /// <returns>A string.</returns>
        public static unsafe string BytesToHexStr(this byte[] bytes)
        {
            uint* lookupP = _lookup32UnsafeP;
            char[] result = new char[bytes.Length * 2];
            fixed (byte* bytesP = bytes)
            fixed (char* resultP = result)
            {
                uint* resultP2 = (uint*)resultP;
                for (int i = 0; i < bytes.Length; i++)
                    resultP2[i] = lookupP[bytesP[i]];
            }
            return new string(result);
        }

        /// <summary>
        /// Transform a string to it's hexadecimal representation.
        /// <para>Obtenir un string dans sa représentation hexadecimale.</para>
        /// <param name="str">The string to transform.</param>
        /// </summary>
        /// <returns>A string.</returns>
        public static string StrToHexStr(this string str, Encoding enc = null)
        {
            if (enc == null)
                enc = Encoding.UTF8;

            return enc.GetBytes(str).BytesToHexStr();
        }

        /// <summary>
        /// Convert a hex-formatted string to byte array.
        /// <para>Convertir une représentation hexadécimal en tableau de bytes.</para>
        /// </summary>
        /// <param name="hex">A string looking like "300D06092A864886F70D0101050500".</param>
        /// <returns>A byte array.</returns>
        public static byte[] HexStrToBytes(this string hex)
        {
            string cleanedRequest = hex.Replace(" ", string.Empty)
                .Replace("\t", string.Empty).Replace("\r", string.Empty)
                .Replace("\n", string.Empty);

            if (cleanedRequest.Length % 2 == 1)
                throw new ArgumentException("[HexExtensions] - HexStrToBytes - The binary key cannot have an odd number of digits");

            try
            {
#if NET5_0_OR_GREATER
                return Convert.FromHexString(cleanedRequest);
#else
                return Enumerable.Range(0, cleanedRequest.Length)
                         .Where(x => x % 2 == 0)
                         .Select(x => Convert.ToByte(cleanedRequest.Substring(x, 2), 16))
                         .ToArray();
#endif
            }
            catch (Exception ex) when (ex is FormatException || ex is OverflowException)
            {
                throw new ArgumentException(
                    "[HexExtensions] - HexStrToBytes - Invalid hex string",
                    ex);
            }
        }

        public static string HexStrToStr(this string hex, Encoding enc = null)
        {
            if (enc == null)
                enc = Encoding.UTF8;

            return enc.GetString(hex.HexStrToBytes());
        }
    }
}
