using SSFWServer.ChannelID;
using System.Collections.Concurrent;
using System.Xml;

namespace SSFWServer.Helpers.FileHelper
{
    public class ScenelistParser
    {
        public static ConcurrentDictionary<string, string> sceneDictionary = new();

        public static KeyValuePair<string, string>? GetSceneNameLike(string nameToFind)
        {
            return sceneDictionary.FirstOrDefault(x => x.Key.Contains(nameToFind));
        }

        public static void UpdateSceneDictionary(object? state)
        {
            try
            {
                if (File.Exists(SSFWServerConfiguration.ScenelistFile))
                {
                    CustomLogger.LoggerAccessor.LogInfo($"[ScenelistParser] - Dictionary refresh started at: {DateTime.Now}");

                    lock (sceneDictionary)
                    {
                        sceneDictionary.Clear();

                        XmlDocument xmlDoc = new();
                        xmlDoc.Load(SSFWServerConfiguration.ScenelistFile);

                        // Select all SCENE elements in the XML
                        XmlNodeList? sceneNodes = xmlDoc.SelectNodes("//REGION/SCENE[@Type='Home' or @Type='Clubhouse' or @Type='PrivateNoVideo'] | //SCENE[@Type='Home' or @Type='Clubhouse' or @Type='PrivateNoVideo']");

                        if (sceneNodes != null)
                        {
                            // Extract ChannelID and SCENE ID from each SCENE element
                            Parallel.ForEach(sceneNodes.Cast<XmlNode>(), sceneNode =>
                            {
                                if (sceneNode.Attributes != null)
                                {
                                    string? ID = sceneNode.Attributes["ID"]?.Value;

                                    if (!string.IsNullOrEmpty(ID))
                                    {
                                        string? channelID = sceneNode.Attributes["ChannelID"]?.Value;

                                        if (string.IsNullOrEmpty(channelID))
                                            channelID = sceneNode.Attributes["SceneID"]?.Value;

                                        if (!string.IsNullOrEmpty(channelID))
                                        {
                                            try
                                            {
                                                if (channelID.Contains('-'))
                                                {
                                                    SceneKey? sceneKey = new(channelID);

                                                    SIDKeyGenerator.VerifyNewerKey(sceneKey);
                                                    string SceneID = SIDKeyGenerator.Instance.ExtractSceneIDNewerType(sceneKey).ToString("X").PadLeft(4, '0'); // SSFW was introduced in modern Home only, safe to assume only newer type.
                                                    if (!sceneDictionary.ContainsKey(ID))
                                                    {
#if DEBUG
                                                        CustomLogger.LoggerAccessor.LogInfo($"[SSFW] - ScenelistParser: Added entry: ID:{ID}|SceneID:{SceneID}");
#endif

                                                        sceneDictionary.TryAdd(ID, SceneID);
                                                    }
                                                }
                                                else
                                                {
                                                    if (!sceneDictionary.ContainsKey(ID))
                                                    {
#if DEBUG
                                                        CustomLogger.LoggerAccessor.LogInfo($"[SSFW] - ScenelistParser: Added entry: ID:{ID}|SceneID:{channelID}");
#endif

                                                        sceneDictionary.TryAdd(ID, channelID);
                                                    }
                                                }
                                            }
                                            catch
                                            {
                                                // Not Important
                                            }
                                        }
                                    }
                                }
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CustomLogger.LoggerAccessor.LogError($"[ScenelistParser] - Errored out while updating scene list Dictionary: {ex}");
            }
        }
    }
}
