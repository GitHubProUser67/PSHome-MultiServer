using System.Runtime.InteropServices;
using System.Text;

namespace CastleLibrary.Utils
{
    public static class HexExtensions
    {
        private static readonly uint[] _lookup32Unsafe = CreateLookup32Unsafe();
        private static readonly unsafe uint* _lookup32UnsafeP = (uint*)
            GCHandle.Alloc(_lookup32Unsafe, GCHandleType.Pinned).AddrOfPinnedObject();

        private static uint[] CreateLookup32Unsafe()
        {
            var result = new uint[256];
            for (var i = 0; i < 256; i++)
            {
                var s = i.ToString("X2");
                result[i] = EndianTools.EndianAwareConverter.isLittleEndianSystem
                    ? ((uint)s[0]) + ((uint)s[1] << 16)
                    : ((uint)s[1]) + ((uint)s[0] << 16);
            }
            return result;
        }

        extension(byte[] bytes)
        {
            /// <summary>
            /// Transform a byte array to it's hexadecimal representation.
            /// <para>Obtenir un tableau de bytes dans sa représentation hexadecimale.</para>
            /// <param name="bytes">The byte array to transform.</param>
            /// </summary>
            /// <returns>A string.</returns>
            public unsafe string BytesToHexStr()
            {
                var lookupP = _lookup32UnsafeP;
                var result = new char[bytes.Length * 2];
                fixed (byte* bytesP = bytes)
                fixed (char* resultP = result)
                {
                    var resultP2 = (uint*)resultP;
                    for (var i = 0; i < bytes.Length; i++)
                        resultP2[i] = lookupP[bytesP[i]];
                }
                return new string(result);
            }
        }

        extension(string hex)
        {
            /// <summary>
            /// Transform a string to it's hexadecimal representation.
            /// <para>Obtenir un string dans sa représentation hexadecimale.</para>
            /// <param name="hex">The string to transform.</param>
            /// </summary>
            /// <returns>A string.</returns>
            public string StrToHexStr(Encoding enc = null)
            {
                enc ??= Encoding.UTF8;

                return enc.GetBytes(hex).BytesToHexStr();
            }

            /// <summary>
            /// Convert a hex-formatted string to byte array.
            /// <para>Convertir une représentation hexadécimal en tableau de bytes.</para>
            /// </summary>
            /// <param name="hex">A string looking like "300D06092A864886F70D0101050500".</param>
            /// <returns>A byte array.</returns>
            public byte[] HexStrToBytes()
            {
                var cleanedRequest = hex.Replace(" ", string.Empty)
                    .Replace("\t", string.Empty)
                    .Replace("\r", string.Empty)
                    .Replace("\n", string.Empty);

                if (cleanedRequest.Length % 2 == 1)
                    throw new ArgumentException(
                        "[HexExtensions] - HexStrToBytes - The binary key cannot have an odd number of digits"
                    );

                try
                {
                    return Convert.FromHexString(cleanedRequest);
                }
                catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                {
                    throw new ArgumentException(
                        "[HexExtensions] - HexStrToBytes - Invalid hex string",
                        ex
                    );
                }
            }

            public string HexStrToStr(Encoding enc = null)
            {
                enc ??= Encoding.UTF8;

                return enc.GetString(hex.HexStrToBytes());
            }
        }
    }
}
