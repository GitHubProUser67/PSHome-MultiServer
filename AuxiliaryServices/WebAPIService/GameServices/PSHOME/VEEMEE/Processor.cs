using System;
using CustomLogger;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Collections.Generic;
using NetHasher;

namespace WebAPIService.GameServices.PSHOME.VEEMEE
{
    public static class Processor
    {
        private const string HashSalt = "veemeeHTTPRequ9R3UMWDAT8F3*#@&$^";

        public static string Sign(string jsonData)
        {
            try
            {
                string formattedJson = JToken.Parse(jsonData.Replace("\n", string.Empty)).ToString(Newtonsoft.Json.Formatting.None);

                string hash = DotNetHasher.ComputeSHA1String(Encoding.UTF8.GetBytes($"{HashSalt}{formattedJson}"));

                JToken token = JToken.Parse(formattedJson);

                if (token.Type == JTokenType.Object)
                {
                    JObject obj = (JObject)token;
                    obj["hash"] = hash;
                    formattedJson = obj.ToString(Newtonsoft.Json.Formatting.None);
                }
                else if (token.Type == JTokenType.Array)
                {
                    JArray array = (JArray)token;
                    JObject obj = new JObject
                    {
                        ["hash"] = hash
                    };
                    array.Add(obj);
                    formattedJson = array.ToString(Newtonsoft.Json.Formatting.None);
                }

                return formattedJson;
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[VEEMEE] : Exception in Sign, file might be incorrect : {ex}");
            }

            return null;
        }

        public static string GetVerificationSalt(string hex, Dictionary<string, string> PostDataKeyValuesDic = null)
        {
            string localSalt = HashSalt;

            if (PostDataKeyValuesDic != null)
            {
                foreach (KeyValuePair<string, string> KeyPair in PostDataKeyValuesDic)
                {
                    localSalt = localSalt + KeyPair.Key + KeyPair.Value;
                }
            }

            return DotNetHasher.ComputeSHA1String(Encoding.UTF8.GetBytes($"{localSalt}hex{hex}"));
        }
    }
}
