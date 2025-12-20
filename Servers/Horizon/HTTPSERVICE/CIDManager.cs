using Newtonsoft.Json;
using System.Text;

namespace Horizon.HTTPSERVICE
{
    public class CIDManager
    {
        private static readonly ConcurrentList<CIDPair> cids = new();

        // Create a CIDPair based on the provided parameters
        public static void CreateCIDPair(string? UserName, string? MachineID)
        {
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(MachineID))
                return;

            CIDPair? cidpairToUpdate = cids.FirstOrDefault(cidpair => cidpair.UserName == UserName && cidpair.MachineID == MachineID);

            if (cidpairToUpdate == null)
            {
                cidpairToUpdate = new CIDPair { UserName = UserName, MachineID = MachineID };
                cids.Add(cidpairToUpdate);
            }
        }

        // Get a list of all CIDPair
        public static List<CIDPair> GetAllCIDPair()
        {
            return cids.ToList();
        }

        // Serialize the CIDPair list to JSON
        public static string ToJson(bool encrypt)
        {
            string JsonData = JsonConvert.SerializeObject(GetAllCIDPair());
            return encrypt ? XORString(JsonData, HorizonServerConfiguration.MediusAPIKey) : JsonData;
        }

        private static string XORString(string input, string? key)
        {
            if (string.IsNullOrEmpty(key))
                key = "@00000000000!00000000000!";

            StringBuilder result = new();

            for (int i = 0; i < input.Length; i++)
            {
                result.Append((char)(input[i] ^ key[i % key.Length]));
            }

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(result.ToString()));
        }
    }

    public class CIDPair
    {
        public string? UserName { get; set; }
        public string? MachineID { get; set; }
    }
}
