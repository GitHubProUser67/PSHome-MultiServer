using System.Text;

namespace CastleLibrary.Sony.SVO
{
    public class WebSecurityUtils
    {
        public static string CalcuateSVOMac(string clientSVOMac)
        {
            if (string.IsNullOrEmpty(clientSVOMac) || clientSVOMac.Length != 32)
                return null;

            // Get SVOMac from client and combine with speaksId together for new MD5, converting to a byte array for MD5 rehashing
            return NetHasher.DotNetHasher.ComputeMD5String(Encoding.ASCII.GetBytes($"{clientSVOMac}sp9ck0348sld00000000000000000000")).ToLower();
        }

        // HMAC-MD5 as result is 16 in length as shown in eboot.
        // Known keys - "m4nT15" (profile) - "GHOST_PWD" (ghost replays)
        public static string CalcuateOTGSecuredHash(string keytohash)
        {
            return NetHasher.DotNetHasher.ComputeMD5String(Encoding.ASCII.GetBytes(keytohash), Encoding.ASCII.GetBytes("ca91f51f-a7f5-4b95-814a-5796cfff586c")).ToLower();
        }
    }
}