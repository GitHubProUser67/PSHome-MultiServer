using System.IO;
using MultiServerLibrary.HTTP;
using HttpMultipartParser;
using Newtonsoft.Json.Linq;

namespace WebAPIService.GameServices.VEEMEE.accorn
{
    public static class Storage
    {
        public static string ReadConfig(byte[] PostData, string ContentType, string apiPath)
        {
            string config = string.Empty;
            string product = string.Empty;
            string boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    config = data.GetParameterValue("config");

                    product = data.GetParameterValue("product");

                    ms.Flush();
                }

                string configValue = "{}"; // Default response when config field doesn't exist

                if (!string.IsNullOrEmpty(config) && !string.IsNullOrEmpty(product))
                {
                    string jsonFilePath = Path.Combine($"{apiPath}/VEEMEE/Acorn_Medow/config.json");

                    if (File.Exists(jsonFilePath))
                    {
                        JObject jObject = JObject.Parse(File.ReadAllText(jsonFilePath));

                        if (jObject != null)
                        {
                            if (jObject.SelectToken(product) is JObject productToken)
                            {
                                JToken configToken = productToken.SelectToken(config);
                                if (configToken != null)
                                    configValue = configToken.ToString();
                            }
                        }
                    }
                }

                return Processor.Sign(configValue);
            }

            return null;
        }

        public static string ReadTable(byte[] PostData, string ContentType, string apiPath)
        {
            string psnid = string.Empty;
            string product = string.Empty;
            string hex = string.Empty;
            string __salt = string.Empty;
            string boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    psnid = data.GetParameterValue("psnid");

                    product = data.GetParameterValue("product");

                    hex = data.GetParameterValue("hex");

                    __salt = data.GetParameterValue("__salt");

                    ms.Flush();
                }

                string ProfileResult = ProfileManager.ReadProfile(psnid, product, hex, __salt, apiPath);

                if (!string.IsNullOrEmpty(ProfileResult))
                    return ProfileResult;
            }

            return null;
        }

        public static string WriteTable(byte[] PostData, string ContentType, string apiPath)
        {
            string psnid = string.Empty;
            string profile = string.Empty;
            string boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    psnid = data.GetParameterValue("psnid");

                    profile = data.GetParameterValue("profile");

                    ms.Flush();
                }

                ProfileManager.WriteProfile(psnid, profile, apiPath);

                return Processor.Sign(profile);
            }

            return null;
        }
    }

    public class ProfileManager
    {
        public static string ReadProfile(string psnid, string product, string hex, string salt, string apiPath)
        {
            if (string.IsNullOrEmpty(hex) || string.IsNullOrEmpty(salt))
                return null;

            if (File.Exists($"{apiPath}/VEEMEE/Acorn_Medow/User_Profiles/{psnid}.json"))
                return Processor.Sign(File.ReadAllText($"{apiPath}/VEEMEE/Acorn_Medow/User_Profiles/{psnid}.json"));
            else
                return Processor.Sign(File.ReadAllText($"{apiPath}/VEEMEE/Acorn_Medow/default_profile.json"));
        }

        public static void WriteProfile(string psnid, string profile, string apiPath)
        {
            Directory.CreateDirectory($"{apiPath}/VEEMEE/Acorn_Medow/User_Profiles");

            File.WriteAllText($"{apiPath}/VEEMEE/Acorn_Medow/User_Profiles/{psnid}.json", profile);
        }
    }
}
