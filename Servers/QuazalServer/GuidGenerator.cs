using NetHasher;
using System.Text;

namespace QuazalServer
{
    public class GuidGenerator
    {
        public static string UBISOFTGenerateGuid(string input1, string input2)
        {
            string md5hash = DotNetHasher.ComputeMD5String(Encoding.UTF8.GetBytes(input1 + "!*JAd3!!!!!!!!!!*!!!"));
            string sha512hash = DotNetHasher.ComputeSHA512String(Encoding.UTF8.GetBytes("*!*!*!*!*!*!*!*!Qn3TZZZ" + input2));
            return (md5hash.Substring(1, 8) + "-" + sha512hash.Substring(2, 4) + "-" + md5hash.Substring(10, 4) + "-" + sha512hash.Substring(16, 4) + "-" + sha512hash.Substring(19, 12)).ToLower();
        }
    }
}
