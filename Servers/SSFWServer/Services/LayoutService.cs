using CustomLogger;
using Newtonsoft.Json.Linq;
using SSFWServer.Helpers.FileHelper;
using SSFWServer.Helpers.RegexHelper;
using System.Text;

namespace SSFWServer.Services
{
    public class LayoutService
    {
        private string? key;

        public LayoutService(string? key)
        {
            this.key = key;
        }

        public bool HandleLayoutServicePOST(byte[] buffer, string directorypath, string absolutepath)
        {
            Directory.CreateDirectory(directorypath);

            string sceneid = absolutepath;
            string[] words = sceneid.Split('/');

            if (words.Length > 0)
                sceneid = words[^1];

            if (sceneid != absolutepath) // If ends with UUID Ok.
            {
                if (File.Exists(SSFWServerConfiguration.ScenelistFile))
                {
                    bool handled = false;

                    IDictionary<string, string> scenemap = ScenelistParser.sceneDictionary;

                    if (File.Exists(directorypath + "/mylayout.json")) // Migrate data.
                    {
                        // Parsing each value in the dictionary
                        foreach (var kvp in SSFWGetLegacyFurnitureLayouts(directorypath + "/mylayout.json"))
                        {
                            if (kvp.Key == "00000000-00000000-00000000-00000004")
                            {
                                File.WriteAllText(directorypath + "/HarborStudio.json", kvp.Value);
                                handled = true;
                            }
                            else
                            {
                                string scenename = scenemap.FirstOrDefault(x => x.Value == Program.ExtractPortion(kvp.Key, 13, 18)).Key;
                                if (!string.IsNullOrEmpty(scenename))
                                {
                                    if (File.Exists(directorypath + $"/{kvp.Key}.json")) // SceneID now mapped, so SceneID based file has become obsolete.
                                        File.Delete(directorypath + $"/{kvp.Key}.json");

                                    File.WriteAllText(directorypath + $"/{scenename}.json", kvp.Value);
                                    handled = true;
                                }
                            }

                            if (!handled)
                                File.WriteAllText(directorypath + $"/{kvp.Key}.json", kvp.Value);

                            handled = false;
                        }

                        File.Delete(directorypath + "/mylayout.json");
                    }
                    else
                    {
                        if (sceneid == "00000000-00000000-00000000-00000004")
                        {
                            File.WriteAllText(directorypath + "/HarborStudio.json", Encoding.UTF8.GetString(buffer));
                            handled = true;
                        } else {
                            string scenename = scenemap.FirstOrDefault(x => x.Value == Program.ExtractPortion(sceneid, 13, 18)).Key;
                            if (!string.IsNullOrEmpty(scenename))
                            {
                                if (File.Exists(directorypath + $"/{sceneid}.json")) // SceneID now mapped, so SceneID based file has become obsolete.
                                    File.Delete(directorypath + $"/{sceneid}.json");

                                File.WriteAllText(directorypath + $"/{scenename}.json", Encoding.UTF8.GetString(buffer));
                                handled = true;
                            }
                        }

                        if (!handled)
                            File.WriteAllText(directorypath + $"/{sceneid}.json", Encoding.UTF8.GetString(buffer));
                    }
                }
                else
                    SSFWLegacyFurnitureLayoutProcess(directorypath + "/mylayout.json", buffer, sceneid);

                return true;
            }

            return false;
        }

        public string? HandleLayoutServiceGET(string directorypath, string sceneIdString)
        {
            string sceneid = sceneIdString;
            string[] words = sceneid.Split('/');

            if (words.Length > 0)
                sceneid = words[^1];

            if (GUIDValidator.RegexSceneIdValidMatch(sceneid).Success) // If is UUID Ok.
            {
                if (File.Exists(SSFWServerConfiguration.ScenelistFile))
                {
                    if (sceneid == "00000000-00000000-00000000-00000004")
                    {
                        if (File.Exists(directorypath + "/HarborStudio.json"))
                            return $"[{{\"{sceneid}\":{FileHelper.ReadAllText(directorypath + "/HarborStudio.json", key)}}}]";
                    }
                    else
                    {
                        string scenename = ScenelistParser.sceneDictionary.FirstOrDefault(x => x.Value == Program.ExtractPortion(sceneid, 13, 18)).Key;
                        if (!string.IsNullOrEmpty(scenename))
                        {
                            string filepath = directorypath + $"/{scenename}.json";
                            if (File.Exists(filepath))
                                return $"[{{\"{sceneid}\":{FileHelper.ReadAllText(filepath, key)}}}]";
                        }
                    }

                    if (File.Exists(directorypath + $"/{sceneid}.json"))
                        return $"[{{\"{sceneid}\":{FileHelper.ReadAllText(directorypath + $"/{sceneid}.json", key)}}}]";
                }

                string? stringlayout = SSFWGetLegacyFurnitureLayoutProcess(directorypath + "/mylayout.json", sceneid);

                if (stringlayout != null)
                    return $"[{{\"{sceneid}\":{stringlayout}}}]";
                else
                    return string.Empty;
            }

            return null;
        }

        public void SSFWLegacyFurnitureLayoutProcess(string filePath, byte[] postData, string sceneid)
        {
            try
            {
                string? json = null;

                if (File.Exists(filePath))
                    json = FileHelper.ReadAllText(filePath, key);

                // Parse the JSON string as a JArray
                JArray jsonArray;
                if (string.IsNullOrEmpty(json))
                    jsonArray = new JArray();
                else
                    jsonArray = JArray.Parse(json);

                // Find the index of the existing object with the given objectId
                int existingIndex = -1;
                for (int i = 0; i < jsonArray.Count; i++)
                {
                    if (jsonArray[i] is JObject obj && obj.Properties().Any(p => p.Name == sceneid))
                    {
                        existingIndex = i;
                        break;
                    }
                }

                if (existingIndex >= 0)
                {
                    // Update the existing object with the new POST data
                    if (jsonArray[existingIndex] is JObject existingObject)
                        existingObject[sceneid] = JObject.Parse(Encoding.UTF8.GetString(postData));
                }
                else
                    // Add the new object to the JSON array
                    jsonArray.Add(new JObject()
                    {
                        { sceneid, JObject.Parse(Encoding.UTF8.GetString(postData)) }
                    });

                File.WriteAllText(filePath, jsonArray.ToString());
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[SSFW] - SSFWLegacyFurnitureLayoutProcess errored out with this exception - {ex}");
            }
        }

        public string? SSFWGetLegacyFurnitureLayoutProcess(string filePath, string sceneid)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string? json = FileHelper.ReadAllText(filePath, key);
                    if (!string.IsNullOrEmpty(json))
                    {
                        JArray jsonArray = JArray.Parse(json);
                        JObject? existingObject = jsonArray
                            .OfType<JObject>()
                            .FirstOrDefault(obj => obj[sceneid] != null);

                        if (existingObject != null && existingObject.TryGetValue(sceneid, out JToken? value))
                            return value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[SSFW] - SSFWGetLegacyFurnitureLayoutProcess errored out with this exception - {ex}");
            }

            return null;
        }

        public Dictionary<string, string> SSFWGetLegacyFurnitureLayouts(string filePath)
        {
            Dictionary<string, string> outputDictionary = new();
            try
            {
                if (File.Exists(filePath))
                {
                    string? json = FileHelper.ReadAllText(filePath, key);
                    if (!string.IsNullOrEmpty(json))
                    {
                        foreach (JObject obj in JArray.Parse(json).OfType<JObject>())
                        {
                            foreach (JProperty? property in obj.Properties())
                            {
                                outputDictionary.Add(property.Name, property.Value.ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[SSFW] - SSFWGetLegacyFurnitureLayouts errored out with this exception - {ex}");
            }

            return outputDictionary;
        }

    }
}
