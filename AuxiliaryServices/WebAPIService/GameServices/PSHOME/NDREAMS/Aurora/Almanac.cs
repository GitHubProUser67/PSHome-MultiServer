using System.Text;
using HttpMultipartParser;
using MultiServerLibrary.HTTP;
using Newtonsoft.Json;

namespace WebAPIService.GameServices.PSHOME.NDREAMS.Aurora
{
    public static class Almanac
    {
        public static string ProcessAlmanac(
            DateTime CurrentDate,
            byte[] PostData,
            string ContentType,
            string fullurl,
            string apipath
        )
        {
            var Weight = !string.IsNullOrEmpty(fullurl) && fullurl.Contains("Weights");
            var func = string.Empty;
            var name = string.Empty;
            var key = string.Empty;
            var element = string.Empty;
            var resdata = string.Empty;
            var boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (var ms = new MemoryStream(PostData))
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

                var SkyFishingProfilePath = apipath + $"/NDREAMS/Aurora/SkyFishing/{name}.json";

                switch (func)
                {
                    case "get":
                        var valuescount = 0;

                        if (File.Exists(SkyFishingProfilePath))
                        {
                            var st = new StringBuilder();

                            foreach (
                                var prop in JsonConvert
                                    .DeserializeObject<List<FishingProps>>(
                                        File.ReadAllText(SkyFishingProfilePath)
                                    )
                                    .OrderBy(x => int.Parse(x.a_id))
                            )
                            {
                                if (int.TryParse(prop.a_id, out var a_idInt))
                                {
                                    if (Weight)
                                    {
                                        st.Append(
                                            $"<element id=\"{a_idInt}\" value=\"{prop.weight}\" />"
                                        );
                                        if ("0".Equals(prop.weight)) { }
                                        else
                                            valuescount += a_idInt;
                                    }
                                    else
                                    {
                                        st.Append(
                                            $"<element id=\"{a_idInt}\" value=\"{prop.caught}\" />"
                                        );
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
                            var props = JsonConvert.DeserializeObject<List<FishingProps>>(
                                File.ReadAllText(SkyFishingProfilePath)
                            );

                            if (props == null)
                            {
                                CustomLogger.LoggerAccessor.LogWarn(
                                    $"[nDreams] - Almanac: Profile:{SkyFishingProfilePath} has an invalid format! Erroring out client..."
                                );
                                return null;
                            }

                            foreach (var prop in props)
                            {
                                if (prop.a_id == element)
                                {
                                    if (Weight)
                                        prop.weight = resdata;
                                    else
                                        prop.caught = resdata;
                                }
                            }

                            File.WriteAllText(
                                SkyFishingProfilePath,
                                JsonConvert.SerializeObject(props, Formatting.Indented)
                            );
                        }
                        else
                        {
                            List<FishingProps> newProfile = [];

                            for (byte i = 1; i <= 40; i++)
                            {
                                if (Weight)
                                {
                                    if (i.ToString() == element)
                                        newProfile.Add(
                                            new FishingProps() { a_id = element, weight = resdata }
                                        );
                                    else
                                        newProfile.Add(new FishingProps() { a_id = i.ToString() });
                                }
                                else
                                {
                                    if (i.ToString() == element)
                                        newProfile.Add(
                                            new FishingProps() { a_id = element, caught = resdata }
                                        );
                                    else
                                        newProfile.Add(new FishingProps() { a_id = i.ToString() });
                                }
                            }

                            File.WriteAllText(
                                SkyFishingProfilePath,
                                JsonConvert.SerializeObject(newProfile, Formatting.Indented)
                            );
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
