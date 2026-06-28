using HttpMultipartParser;
using MultiServerLibrary.HTTP;
using Newtonsoft.Json.Linq;

namespace WebAPIService.GameServices.PSHOME.VEEMEE.accorn
{
    public static class Storage
    {
        public static string ReadConfig(byte[] PostData, string ContentType, string apiPath)
        {
            var config = string.Empty;
            var product = string.Empty;
            var boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (var ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    config = data.GetParameterValue("config");

                    product = data.GetParameterValue("product");

                    ms.Flush();
                }

                var configValue = "{}"; // Default response when config field doesn't exist

                if (!string.IsNullOrEmpty(config) && !string.IsNullOrEmpty(product))
                {
                    var jsonFilePath = Path.Combine($"{apiPath}/VEEMEE/Acorn_Medow/config.json");

                    if (File.Exists(jsonFilePath))
                    {
                        var jObject = JObject.Parse(File.ReadAllText(jsonFilePath));

                        if (jObject != null)
                        {
                            if (jObject.SelectToken(product) is JObject productToken)
                            {
                                var configToken = productToken.SelectToken(config);
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
            var psnid = string.Empty;
            var product = string.Empty;
            var hex = string.Empty;
            var __salt = string.Empty;
            var boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (var ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    psnid = data.GetParameterValue("psnid");

                    product = data.GetParameterValue("product");

                    hex = data.GetParameterValue("hex");

                    __salt = data.GetParameterValue("__salt");

                    ms.Flush();
                }

                var ProfileResult = ProfileManager.ReadProfile(
                    psnid,
                    product,
                    hex,
                    __salt,
                    apiPath
                );

                if (!string.IsNullOrEmpty(ProfileResult))
                    return ProfileResult;
            }

            return null;
        }

        public static string WriteTable(byte[] PostData, string ContentType, string apiPath)
        {
            var psnid = string.Empty;
            var profile = string.Empty;
            var boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (var ms = new MemoryStream(PostData))
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
        public static string ReadProfile(
            string psnid,
            string product,
            string hex,
            string salt,
            string apiPath
        )
        {
            return string.IsNullOrEmpty(hex) || string.IsNullOrEmpty(salt) ? null
                : File.Exists($"{apiPath}/VEEMEE/Acorn_Medow/User_Profiles/{psnid}.json")
                    ? Processor.Sign(
                        File.ReadAllText($"{apiPath}/VEEMEE/Acorn_Medow/User_Profiles/{psnid}.json")
                    )
                : Processor.Sign(
                    File.ReadAllText($"{apiPath}/VEEMEE/Acorn_Medow/default_profile.json")
                );
        }

        public static void WriteProfile(string psnid, string profile, string apiPath)
        {
            Directory.CreateDirectory($"{apiPath}/VEEMEE/Acorn_Medow/User_Profiles");

            File.WriteAllText($"{apiPath}/VEEMEE/Acorn_Medow/User_Profiles/{psnid}.json", profile);
        }
    }
}
