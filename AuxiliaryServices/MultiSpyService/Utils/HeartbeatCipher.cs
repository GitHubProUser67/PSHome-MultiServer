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
            for (var i = 0x21; i < 0x7f; i++) // From 33 to 126 (inclusive)
            {
                builder.Append((char)i);
            }
            Alphabet = builder.ToString();
        }

        public HeartbeatCipher(string salt = null)
        {
            Salt = salt ?? GenerateRandomSalt(6);
        }

        private static string GenerateRandomSalt(int length)
        {
            var random = new Random();
            var result = new StringBuilder();

            for (var i = 0; i < length; i++)
            {
                var index = random.Next(Alphabet.Length);
                result.Append(Alphabet[index]);
            }

            return result.ToString();
        }
    }
}
