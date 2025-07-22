using System;
using System.Collections.Generic;
using System.IO;
using MultiServerLibrary.HTTP;
using HttpMultipartParser;
using Newtonsoft.Json;
using System.Text;
using System.Linq;
using WebAPIService.GameServices.NDREAMS;

namespace WebAPIService.GameServices.NDREAMS.Aurora
{
    public static class Almanac
    {
        public static string ProcessAlmanac(DateTime CurrentDate, byte[] PostData, string ContentType, string fullurl, string apipath)
        {
            bool Weight = !string.IsNullOrEmpty(fullurl) && fullurl.Contains("Weights");
            string func = string.Empty;
            string name = string.Empty;
            string key = string.Empty;
            string element = string.Empty;
            string resdata = string.Empty;
            string boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    func = data.GetParameterValue("func");
                    name = data.GetParameterValue("name");
                    try
                    {
                        element = data.GetParameterValue("element");
                        resdata = data.GetParameterValue("data");
                    }
                    catch
                    {
                        // Not Important.
                    }
                    try
                    {
                        key = data.GetParameterValue("key");
                    }
                    catch
                    {
                        // Not Important.
                    }

                    ms.Flush();
                }

                Directory.CreateDirectory(apipath + "/NDREAMS/Aurora/SkyFishing");

                string SkyFishingProfilePath = apipath + $"/NDREAMS/Aurora/SkyFishing/{name}.json";

                switch (func)
                {
                    case "get":
                        int valuescount = 0;

                        if (File.Exists(SkyFishingProfilePath))
                        {
                            StringBuilder st = new StringBuilder();

                            foreach (FishingProps prop in JsonConvert.DeserializeObject<List<FishingProps>>(File.ReadAllText(SkyFishingProfilePath)).OrderBy(x => int.Parse(x.a_id)))
                            {
                                if (int.TryParse(prop.a_id, out int a_idInt))
                                {
                                    if (Weight)
                                    {
                                        st.Append($"<element id=\"{a_idInt}\" value=\"{prop.weight}\" />");
                                        if ("0".Equals(prop.weight))
                                        {

                                        }
                                        else
                                            valuescount += a_idInt;
                                    }
                                    else
                                    {
                                        st.Append($"<element id=\"{a_idInt}\" value=\"{prop.caught}\" />");
                                        if ("1".Equals(prop.caught))
                                            valuescount += a_idInt;
                                    }
                                }
                            }

                            return $"<xml>{st}<sig>{NDREAMSServerUtils.Server_GetSignature(fullurl, name, "SkyFishingGet", CurrentDate)}</sig><confirm>{NDREAMSServerUtils.Server_KeyToHash(key, CurrentDate, valuescount.ToString())}</confirm></xml>";
                        }
                        else
                            return $"<xml><sig>{NDREAMSServerUtils.Server_GetSignature(fullurl, name, "SkyFishingGet", CurrentDate)}</sig><confirm>{NDREAMSServerUtils.Server_KeyToHash(key, CurrentDate, valuescount.ToString())}</confirm></xml>";
                    case "set":
                        if (File.Exists(SkyFishingProfilePath))
                        {
                            List<FishingProps> props = JsonConvert.DeserializeObject<List<FishingProps>>(File.ReadAllText(SkyFishingProfilePath));

                            if (props == null)
                            {
                                CustomLogger.LoggerAccessor.LogWarn($"[nDreams] - Almanac: Profile:{SkyFishingProfilePath} has an invalid format! Erroring out client...");
                                return null;
                            }

                            foreach (FishingProps prop in props)
                            {
                                if (prop.a_id == element)
                                {
                                    if (Weight)
                                        prop.weight = resdata;
                                    else
                                        prop.caught = resdata;
                                }
                            }

                            File.WriteAllText(SkyFishingProfilePath, JsonConvert.SerializeObject(props, Formatting.Indented));
                        }
                        else
                        {
                            List<FishingProps> newProfile = new List<FishingProps>();

                            for (byte i = 1; i <= 40; i++)
                            {
                                if (Weight)
                                {
                                    if (i.ToString() == element)
                                        newProfile.Add(new FishingProps() { a_id = element, weight = resdata });
                                    else
                                        newProfile.Add(new FishingProps() { a_id = i.ToString() });
                                }
                                else
                                {
                                    if (i.ToString() == element)
                                        newProfile.Add(new FishingProps() { a_id = element, caught = resdata });
                                    else
                                        newProfile.Add(new FishingProps() { a_id = i.ToString() });
                                }
                            }

                            File.WriteAllText(SkyFishingProfilePath, JsonConvert.SerializeObject(newProfile, Formatting.Indented));
                        }
                        return $"<xml></xml>";
                }
            }

            return null;
        }

        public class FishingProps
        {
            public string a_id { get; set; }
            public string caught { get; set; } = "0";
            public string weight { get; set; } = "0";
        }
    }
}
