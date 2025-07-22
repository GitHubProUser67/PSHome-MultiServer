using System;
using System.IO;
using CustomLogger;
using Newtonsoft.Json.Linq;
using HttpMultipartParser;
using Newtonsoft.Json;
using MultiServerLibrary.HTTP;

namespace WebAPIService.GameServices.OHS
{
    public class Community
    {
        public static string Community_Getscore(byte[] PostData, string ContentType, string directorypath, string batchparams, int game)
        {
            string dataforohs = null;
            int output = 0;

            if (string.IsNullOrEmpty(batchparams))
            {
                string boundary = HTTPProcessor.ExtractBoundary(ContentType);

                if (!string.IsNullOrEmpty(boundary))
                {
                    using (MemoryStream ms = new MemoryStream(PostData))
                    {
                        var data = MultipartFormDataParser.Parse(ms, boundary);
                        LoggerAccessor.LogInfo($"[OHS] : Client Version - {data.GetParameterValue("version")}");
                        dataforohs = JaminProcessor.JaminDeFormat(data.GetParameterValue("data"), true, game);
                        ms.Flush();
                    }
                }
            }
            else
                dataforohs = batchparams;

            try
            {
                if (!string.IsNullOrEmpty(dataforohs))
                {
                    JToken Token = JToken.Parse(dataforohs);

                    if (Token != null)
                    {
                        object key = JtokenUtils.GetValueFromJToken(Token, "key");

                        if (key != null)
                        {
                            if (File.Exists(directorypath + $"/Community_Profiles/{key}.json"))
                            {
                                // Check if the key name already exists in the JSON
                                JToken existingKey = JObject.Parse(File.ReadAllText(directorypath + $"/Community_Profiles/{key}.json"))?.SelectToken($"$..{key}");

                                if (existingKey != null)
                                    // Get the value of the existing key
                                    output = existingKey.Value<int>();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Community] - Json Format Error - {ex}");
            }

            if (!string.IsNullOrEmpty(batchparams))
                return "{ [\"score\"] = " + output.ToString() + " }";

            return JaminProcessor.JaminFormat($"{{ [\"status\"] = \"success\", [\"value\"] = {{ [\"score\"] = {output} }} }}", game);
        }

        public static string Community_UpdateScore(byte[] PostData, string ContentType, string directorypath, string batchparams, int game)
        {
            string dataforohs = null;
            int output = 0;

            if (string.IsNullOrEmpty(batchparams))
            {
                string boundary = HTTPProcessor.ExtractBoundary(ContentType);

                if (!string.IsNullOrEmpty(boundary))
                {
                    using (MemoryStream ms = new MemoryStream(PostData))
                    {
                        var data = MultipartFormDataParser.Parse(ms, boundary);
                        LoggerAccessor.LogInfo($"[OHS] : Client Version - {data.GetParameterValue("version")}");
                        dataforohs = JaminProcessor.JaminDeFormat(data.GetParameterValue("data"), true, game);
                        ms.Flush();
                    }
                }
            }
            else
                dataforohs = batchparams;

            try
            {
                if (!string.IsNullOrEmpty(dataforohs))
                {
                    JToken Token = JToken.Parse(dataforohs);

                    if (Token != null)
                    {
                        object key = JtokenUtils.GetValueFromJToken(Token, "key");

                        int inc = Convert.ToInt32(JtokenUtils.GetValueFromJToken(Token, "inc"));

                        Directory.CreateDirectory(directorypath + $"/Community_Profiles");

                        if (File.Exists(directorypath + $"/Community_Profiles/{key}.json") && key != null)
                        {
                            string tempreader = File.ReadAllText(directorypath + $"/Community_Profiles/{key}.json");

                            if (!string.IsNullOrEmpty(tempreader))
                            {
                                JObject jObject = JObject.Parse(tempreader);

                                if (jObject != null)
                                {
                                    // Check if the key name already exists in the JSON
                                    JToken existingKey = jObject.SelectToken($"$..{key}");

                                    if (existingKey != null)
                                    {
                                        output = existingKey.Value<int>() + inc;
                                        existingKey.Replace(output);
                                    }
                                    else if (key != null)
                                    {
                                        string keyname = key.ToString();
                                        if (keyname != null)
                                        {
                                            output = inc;
                                            jObject.Add(new JProperty(keyname, output));
                                        }
                                    }
                                }

                                File.WriteAllText(directorypath + $"/Community_Profiles/{key}.json", JsonConvert.SerializeObject(jObject, Formatting.Indented));
                            }
                        }
                        else if (key != null)
                        {
                            output = inc;

                            Directory.CreateDirectory(directorypath + "/Community_Profiles");

                            File.WriteAllText(directorypath + $"/Community_Profiles/{key}.json", $"{{ \"{key}\":{inc} }}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Community] - Json Format Error - {ex}");
            }

            if (!string.IsNullOrEmpty(batchparams))
                return "{ [\"score\"] = " + output.ToString() + " }";

            return JaminProcessor.JaminFormat($"{{ [\"status\"] = \"success\", [\"value\"] = {{ [\"score\"] = {output} }} }}", game);
        }
    }
}
