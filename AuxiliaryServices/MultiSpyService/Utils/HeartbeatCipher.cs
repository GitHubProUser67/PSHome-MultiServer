using System;
using System.Text;

namespace MultiSpyService.Utils
{
    // From: https://github.com/teknogods/eaEmu/blob/master/eaEmu/gamespy/cipher.py#L21
    public class HeartbeatCipher
    {
        private static readonly string Alphabet;
        public string Salt { get; private set; }

        static HeartbeatCipher()
        {
            var builder = new StringBuilder();
            for (int i = 0x21; i < 0x7f; i++) // From 33 to 126 (inclusive)
            {
                builder.Append((char)i);
            }
            Alphabet = builder.ToString();
        }

        public HeartbeatCipher(string salt = null)
        {
            if (salt == null)
            {
                Salt = GenerateRandomSalt(6);
            }
            else
            {
                Salt = salt;
            }
        }

        private string GenerateRandomSalt(int length)
        {
            var random = new Random();
            var result = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                int index = random.Next(Alphabet.Length);
                result.Append(Alphabet[index]);
            }

            return result.ToString();
        }
    }
}
