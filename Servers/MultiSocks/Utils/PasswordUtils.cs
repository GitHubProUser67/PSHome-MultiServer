using MultiServerLibrary.Extension;
using System.Text;

namespace MultiSocks.Utils
{
    public class PasswordUtils
    {
        public string? ssc2Decode(string? encodedPassword, string ssc2Key)
        {
            encodedPassword = SanitizeInput(encodedPassword);

            if (!string.IsNullOrEmpty(encodedPassword))
            {
                byte[] decodeHexKey = ssc2Key.HexStringToByteArray();
                byte[] decodeBuffer = new byte[32];
                CryptSSC2.cryptSSC2StringDecrypt(decodeBuffer, decodeBuffer.Length, Encoding.UTF8.GetBytes(encodedPassword), decodeHexKey, decodeHexKey.Length, decodeHexKey.Length);
                return TruncateAtNull(Encoding.UTF8.GetString(decodeBuffer));
            }

            return null;
        }

        public string TruncateAtNull(string input)
        {
            int nullPos = input.IndexOf('\0');
            return (nullPos != -1) ? input[..nullPos] : input;
        }

        public string? SanitizeInput(string? input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Remove enclosing quotes
            if (input[0] == 0x22 && input[^1] == 0x22)
                input = input[1..];

            // Remove leading tilde
            if (input[0] == 0x7E)
                input = input[1..];

            return LobbyTagField.decodeString(input.ToHexString()).HexStringToString();
        }
    }
}
