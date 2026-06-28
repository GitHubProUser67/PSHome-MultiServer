using System.Text;

namespace MultiSpyService.Utils
{
    public class XorEncoding
    {
        /// <summary>
        /// simple xor encoding for Gstats,GPSP,GPCM
        /// </summary>
        /// <param name="plaintext"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static byte[] Xor(byte[] plaintext, byte type = 0)
        {
            var index = 0;
            var length = plaintext.Length;
            var KeyData = type switch
            {
                1 => Encoding.UTF8.GetBytes("GameSpy3D"),
                2 => Encoding.UTF8.GetBytes("Industries"),
                3 => Encoding.UTF8.GetBytes("ProjectAphex"),
                _ => Encoding.UTF8.GetBytes("gamespy"),
            };
            for (var i = 0; length > 0; length--)
            {
                if (i >= KeyData.Length)
                    i = 0;

                plaintext[index++] ^= KeyData[i++];
            }

            return plaintext;
        }

        public static string Xor(string plaintext, byte type = 0)
        {
            var index = 0;
            var length = plaintext.Length;
            var data = plaintext.ToCharArray();
            var KeyData = type switch
            {
                1 => "GameSpy3D",
                2 => "Industries",
                3 => "ProjectAphex",
                _ => "gamespy",
            };
            for (var i = 0; length > 0; length--)
            {
                if (i >= KeyData.Length)
                    i = 0;

                data[index++] ^= KeyData[i++];
            }

            return new string(data);
        }
    }
}
