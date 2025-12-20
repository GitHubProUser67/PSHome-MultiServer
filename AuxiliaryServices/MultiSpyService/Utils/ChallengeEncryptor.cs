using System;
using System.Linq;

namespace MultiSpyService.Utils
{
    /// <summary>
    /// This code was started because I mistakenly thought that the NatNeg protocol used the Gamespy challenge encryption scheme. It doesn't, but this code remains, in case someone needs it for other uses.
    /// The code was manually converted to C# from http://aluigi.altervista.org/papers/gsmsalg.h
    /// </summary>
    public class ChallengeEncryptor
    {
        private static byte[] enctype1_data {
            get
            {
                if (_enctype1_data != null) return _enctype1_data;
                _enctype1_data = enctype1_data_string.Split(' ').Select((b) => { return (byte)Convert.ToInt32(b, 16); }).ToArray();
                return _enctype1_data;
            }
        }
        private static byte[] _enctype1_data = null;
        private static string enctype1_data_string =
             "01 ba fa b2 51 00 54 80 75 16 8e 8e 02 08 36 a5" +
            " 2d 05 0d 16 52 07 b4 22 8c e9 09 d6 b9 26 00 04" +
            " 06 05 00 13 18 c4 1e 5b 1d 76 74 fc 50 51 06 16" +
            " 00 51 28 00 04 0a 29 78 51 00 01 11 52 16 06 4a" +
            " 20 84 01 a2 1e 16 47 16 32 51 9a c4 03 2a 73 e1" +
            " 2d 4f 18 4b 93 4c 0f 39 0a 00 04 c0 12 0c 9a 5e" +
            " 02 b3 18 b8 07 0c cd 21 05 c0 a9 41 43 04 3c 52" +
            " 75 ec 98 80 1d 08 02 1d 58 84 01 4e 3b 6a 53 7a" +
            " 55 56 57 1e 7f ec b8 ad 00 70 1f 82 d8 fc 97 8b" +
            " f0 83 fe 0e 76 03 be 39 29 77 30 e0 2b ff b7 9e" +
            " 01 04 f8 01 0e e8 53 ff 94 0c b2 45 9e 0a c7 06" +
            " 18 01 64 b0 03 98 01 eb 02 b0 01 b4 12 49 07 1f" +
            " 5f 5e 5d a0 4f 5b a0 5a 59 58 cf 52 54 d0 b8 34" +
            " 02 fc 0e 42 29 b8 da 00 ba b1 f0 12 fd 23 ae b6" +
            " 45 a9 bb 06 b8 88 14 24 a9 00 14 cb 24 12 ae cc" +
            " 57 56 ee fd 08 30 d9 fd 8b 3e 0a 84 46 fa 77 b8";

        private static byte gsvalfunc(int reg) {
            if (reg < 26) return (byte)(reg + 'A');
            if (reg < 52) return (byte)(reg + 'G');
            if (reg < 62) return (byte)(reg - 4);
            if (reg == 62) return (byte)'+';
            if (reg == 63) return (byte)'/';
            return 0;
        }

        private static byte[] gsseckey(byte[] src, byte[] key, int enctype)
        {
            byte x, y, z, a, b;
            int i, size, keysz;
            byte[] enctmp = new byte[256];
            byte[] tmp = new byte[66];

            size = src.Length;
            byte[] dst = new byte[((size * 4) / 3) + 3];
            if ((size < 1) || (size > 65))
            {
                dst[0] = 0;
                return dst;
            }
            keysz = key.Length;

            for (i = 0; i < 256; i++)
            {
                enctmp[i] = (byte)i;
            }

            a = 0;
            for (i = 0; i < 256; i++)
            {
                a += (byte)(enctmp[i] + (byte)key[i % keysz]);
                x = enctmp[a];
                enctmp[a] = enctmp[i];
                enctmp[i] = x;
            }

            a = 0;
            b = 0;
            for (i = 0; src[i] > 0; i++)
            {
                a += (byte)(src[i] + 1);
                x = enctmp[a];
                b += x;
                y = enctmp[b];
                enctmp[b] = x;
                enctmp[a] = y;
                tmp[i] = (byte)(src[i] ^ enctmp[(x + y) & 0xff]);
            }
            for (size = i; size % 3 > 0; size++)
            {
                tmp[size] = 0;
            }

            if (enctype == 1)
            {
                for (i = 0; i < size; i++)
                {
                    tmp[i] = enctype1_data[tmp[i]];
                }
            }
            else if (enctype == 2)
            {
                for (i = 0; i < size; i++)
                {
                    tmp[i] ^= key[i % keysz];
                }
            }

            int p = 0;
            for (i = 0; i < size; i += 3)
            {
                x = tmp[i];
                y = tmp[i + 1];
                z = tmp[i + 2];
                dst[p++] = gsvalfunc(x >> 2);
                dst[p++] = gsvalfunc(((x & 3) << 4) | (y >> 4));
                dst[p++] = gsvalfunc(((y & 15) << 2) | (z >> 6));
                dst[p++] = gsvalfunc(z & 63);
            }
            return dst;
        }

    }
}
