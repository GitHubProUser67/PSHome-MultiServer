using EndianTools;

namespace HomeTools.Crypto
{
    public class ProfanityFilterSecurity
    {
        private static readonly uint[] ProfanityFilterKey = new uint[4]
        {
            0xF512A417, 0x485EF87A, 0xB3D85E90, 0xC4923F75
        };

        /// <summary>
        /// Decryption using custom Corrected Block TEA (xxtea) algorithm
        /// </summary>
        /// <param name="ciphertext">byte[] to be decrypted</param>
        /// <returns></returns>
        public static byte[] PF_DeCipher(byte[] ciphertext)
        {
            if (ciphertext.Length <= 1) { return null; }

            uint[] v = EndianAwareConverter.ToUInt32(ciphertext, Endianness.BigEndian);

            uint n = (uint)v.Length,
                   z,
                   y = v[0],
                   y0,
                   y1;

            for (uint sum = (uint)((52 / (int)n + 6) * -0x61c88647); (int)sum != 0;
                sum += 0x61c88647)
            {
                uint e = sum >> 2 & 3;
                uint p = n;
                uint offset = n - 2;

                while (true)
                {
                    p--;
                    y0 = y << 2;
                    y1 = y >> 3;
                    if (p == 0) break;
                    z = v[offset];
                    y = v[p] -= (z >> 5 ^ y0) + (y1 ^ z << 4) ^ (sum ^ y) + (z ^ ProfanityFilterKey[p & 3 ^ e]);
                    offset--;
                }

                z = v[n - 1];
                y = v[0] -= (z >> 5 ^ y0) + (y1 ^ z << 4) ^ (sum ^ y) + (z ^ ProfanityFilterKey[e]);
            }

            return EndianAwareConverter.GetBytes(v, Endianness.LittleEndian);
        }
    }
}
