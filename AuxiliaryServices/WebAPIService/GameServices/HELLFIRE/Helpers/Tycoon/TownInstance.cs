using NetHasher;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WebAPIService.GameServices.HELLFIRE.Helpers.Tycoon
{
    public class TownInstance
    {
        public static string RequestTownInstance(string UserID, string DisplayName, string PHPSessionId)
        {
            string hash = DotNetHasher.ComputeMD5String(Encoding.ASCII.GetBytes(UserID + "G0TOH00000!!!!m3TycoonN0?w*" + DisplayName));

            if (string.IsNullOrEmpty(PHPSessionId))
                return $"<Response><InstanceID>{GenerateTycoonguid(hash, UserID + hash)}</InstanceID></Response>";
            else
                return $"<Response><InstanceID>{GenerateTycoonguid(hash, UserID + hash + PHPSessionId)}</InstanceID></Response>";
        }

        public static string RequestTown(string UserID, string InstanceID, string DisplayName, string WorkPath)
        {
            Directory.CreateDirectory($"{WorkPath}/TYCOON/User_Data/{UserID}");
            if (File.Exists($"{WorkPath}/TYCOON/User_Data/{UserID}/Town_{GenerateCityguid(InstanceID, UserID)}.xml"))
                return $"<Response>{File.ReadAllText($"{WorkPath}/TYCOON/User_Data/{UserID}/Town_{GenerateCityguid(InstanceID, UserID)}.xml")}</Response>";
            else
            {
                StringBuilder gridBuilder = new StringBuilder();

                for (int i = 1; i <= 256; i++)
                {
                    gridBuilder.Append($"<{i}.000000><TimeBuilt></TimeBuilt><Orientation></Orientation><Index>{i}.000000</Index><Type></Type></{i}.000000>");
                }

                string xml = $"<UserID>{UserID}</UserID><DisplayName>{DisplayName}</DisplayName>" +
                    $"<TownID>{GenerateCityguid(InstanceID, UserID)}</TownID>" +
                    $"<InstanceID>{InstanceID}</InstanceID><LastVisited>0</LastVisited><Grid>{gridBuilder.ToString()}</Grid>";

                gridBuilder = null;

                File.WriteAllText($"{WorkPath}/TYCOON/User_Data/{UserID}/Town_{GenerateCityguid(InstanceID, UserID)}.xml", xml);
                return $"<Response>{xml}</Response>";
            }
        }

        private static string GenerateTycoonguid(string input1, string input2)
        {
            string md5hash = DotNetHasher.ComputeMD5String(Encoding.UTF8.GetBytes(input1 + "**H0mEIsG3reATW1tHH0meTYC000N!!!!!!!!!!!!!!"));
            string sha512hash = DotNetHasher.ComputeSHA512String(Encoding.UTF8.GetBytes("C0MeW1tHH0meTYC111NBaCKHOm3*!*!*!*!*!*!*!*!" + input2));
            string result = (md5hash.Substring(1, 8) + sha512hash.Substring(2, 4) + md5hash.Substring(10, 4) + sha512hash.Substring(16, 4) + sha512hash.Substring(19, 16)).ToLower();

            // Use a dictionary to map characters 'a' to 'f' to specific numbers
            Dictionary<char, char> charMapping = new Dictionary<char, char>
            {
                { 'a', '6' },
                { 'b', '1' },
                { 'c', '2' },
                { 'd', '3' },
                { 'e', '4' },
                { 'f', '5' }
            };

            // Replace characters in the result based on the mapping
            StringBuilder stringBuilder = new StringBuilder();

            foreach (char c in result)
            {
                if (charMapping.TryGetValue(c, out char mappedChar))
                    stringBuilder.Append(mappedChar);
                else
                    stringBuilder.Append(c);
            }

            result = stringBuilder.ToString();

            stringBuilder = null;

            return result;
        }

        public static string GenerateCityguid(string input1, string input2)
        {
            string md5hash = DotNetHasher.ComputeMD5String(Encoding.UTF8.GetBytes(input1 + "**MyC1TY1sTH3be5T!!!!!!!!!!!!!!"));
            string sha512hash = DotNetHasher.ComputeSHA512String(Encoding.UTF8.GetBytes("1L0veH0mmmmeT1c000000nnnnn!!!!!" + input2));
            string result = (md5hash.Substring(1, 8) + sha512hash.Substring(2, 4) + md5hash.Substring(10, 4) + sha512hash.Substring(16, 4) + sha512hash.Substring(19, 16)).ToLower();
            StringBuilder stringBuilder = new StringBuilder();

            // Use a dictionary to map characters 'a' to 'f' to specific numbers
            Dictionary<char, char> charMapping = new Dictionary<char, char>
            {
                { 'a', '3' },
                { 'b', '5' },
                { 'c', '6' },
                { 'd', '8' },
                { 'e', '4' },
                { 'f', '9' }
            };

            // Replace characters in the result based on the mapping

            foreach (char c in result)
            {
                if (charMapping.TryGetValue(c, out char mappedChar))
                    stringBuilder.Append(mappedChar);
                else
                    stringBuilder.Append(c);
            }

            return stringBuilder.ToString();
        }
    }
}
