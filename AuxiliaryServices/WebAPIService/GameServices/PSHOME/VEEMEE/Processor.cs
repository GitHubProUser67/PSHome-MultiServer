using System.Text;
using CastleLibrary.NetHasher;
using CustomLogger;
using Newtonsoft.Json.Linq;

namespace WebAPIService.GameServices.PSHOME.VEEMEE
{
    public static class Processor
    {
        private const string HashSalt = "veemeeHTTPRequ9R3UMWDAT8F3*#@&$^";

        public static string Sign(string jsonData)
        {
            try
            {
                var formattedJson = JToken
                    .Parse(jsonData.Replace("\n", string.Empty))
                    .ToString(Newtonsoft.Json.Formatting.None);

                var hash = DotNetHasher.ComputeSHA1String(
                    Encoding.UTF8.GetBytes($"{HashSalt}{formattedJson}")
                );

                var token = JToken.Parse(formattedJson);

                if (token.Type == JTokenType.Object)
                {
                    var obj = (JObject)token;
                    obj["hash"] = hash;
                    formattedJson = obj.ToString(Newtonsoft.Json.Formatting.None);
                }
                else if (token.Type == JTokenType.Array)
                {
                    var array = (JArray)token;
                    var obj = new JObject { ["hash"] = hash };
                    array.Add(obj);
                    formattedJson = array.ToString(Newtonsoft.Json.Formatting.None);
                }

                return formattedJson;
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(
                    $"[VEEMEE] : Exception in Sign, file might be incorrect : {ex}"
                );
            }

            return null;
        }

        public static string GetVerificationSalt(
            string hex,
            Dictionary<string, string> PostDataKeyValuesDic = null
        )
        {
            var localSalt = HashSalt;

            if (PostDataKeyValuesDic != null)
            {
                foreach (var KeyPair in PostDataKeyValuesDic)
                {
                    localSalt = localSalt + KeyPair.Key + KeyPair.Value;
                }
            }

            return DotNetHasher.ComputeSHA1String(Encoding.UTF8.GetBytes($"{localSalt}hex{hex}"));
        }
    }
}
