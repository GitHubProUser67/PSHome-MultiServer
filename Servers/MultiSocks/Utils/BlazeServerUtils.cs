using System.Security.Cryptography;
using System.Text;

namespace MultiSocks.Utils
{
    public static class BlazeServerUtils
    {
        public static string GenerateSessionKey()
        {
            var rnd = new Random();
            var numberPart = rnd.Next(10000000, 100000000);

            var bytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(bytes);

            var hex = new StringBuilder(32);
            foreach (var b in bytes)
                hex.Append(b.ToString("x2")); // lowercase hex

            return $"{numberPart}_{hex}";
        }

        public static string GenerateTelemetrySessionId()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var result = new StringBuilder(11);
            var rnd = new Random();

            for (var i = 0; i < 11; i++)
            {
                result.Append(chars[rnd.Next(chars.Length)]);
            }

            return result.ToString();
        }
    }
}
