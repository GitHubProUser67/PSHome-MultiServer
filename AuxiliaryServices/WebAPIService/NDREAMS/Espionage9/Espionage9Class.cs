using HttpMultipartParser;
using MultiServerLibrary.HTTP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Xml.Serialization;
using System.Linq;

namespace WebAPIService.NDREAMS.Espionage9
{
    public class Espionage9Class
    {
        public static string ProcessPhpRequest(DateTime CurrentDate, byte[] PostData, string ContentType, string apipath)
        {
            string func = null;
            string key = null;
            string ExpectedHash = null;
            string name = null;
            string finger = null;
            string score = null;
            string win = null;
            string flag1 = null;
            string flag2 = null;

            string boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    func = data.GetParameterValue("func");
                    key = data.GetParameterValue("key");
                    name = data.GetParameterValue("name");

                    if (!string.IsNullOrEmpty(func))
                    {
                        string directoryPath = apipath + $"/NDREAMS/Espionage9/PlayersInventory/{name}";
                        string profilePath = directoryPath + "/SecretAgentData.xml";

                        switch (func)
                        {
                            case "get":
                                ExpectedHash = NDREAMSServerUtils.Server_GetSignature(string.Empty, "espionage", func, CurrentDate);

                                if (ExpectedHash.Equals(key))
                                {
                                    Espionage9ProfileData profileData;

                                    if (File.Exists(profilePath))
                                        profileData = Espionage9ProfileData.DeserializeProfileData(profilePath);
                                    else
                                    {
                                        profileData = new Espionage9ProfileData() { score = 0, plays = 0, wins = 0, flag1 = false, flag2 = false };

                                        Directory.CreateDirectory(directoryPath);
                                        profileData.SerializeProfileData(profilePath);
                                    }

                                    return $"<xml><success>true</success><score>{profileData.score}</score><plays>{profileData.plays}</plays><wins>{profileData.wins}</wins><flag1>{profileData.flag1}</flag1>" +
                                        $"<flag2>{profileData.flag2}</flag2><confirm>{NDREAMSServerUtils.Server_GetSignature(string.Empty, name, $"{profileData.score}{profileData.plays}{profileData.wins}", CurrentDate)}</confirm></xml>";
                                }
                                else
                                {
                                    string errMsg = $"[Espionage9] - PhpRequest: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessPhpRequest</function></xml>";
                                }
                            case "set":
                                finger = data.GetParameterValue("finger");
                                score = data.GetParameterValue("score");
                                win = data.GetParameterValue("win");
                                flag1 = data.GetParameterValue("flag1");
                                flag2 = data.GetParameterValue("flag2");

                                ExpectedHash = NDREAMSServerUtils.Server_GetSignature(string.Empty, "espionage", func, CurrentDate);

                                if (ExpectedHash.Equals(key))
                                {
                                    string errMsg;

                                    if (File.Exists(profilePath))
                                    {
                                        if (!int.TryParse(score, out int scoreInt))
                                        {
                                            errMsg = $"[Espionage9] - PhpRequest: invalid score argument!";
                                            CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                            return $"<xml><success>false</success><error>Invalid score argument format</error><extra>{errMsg}</extra><function>ProcessPhpRequest</function></xml>";
                                        }

                                        Espionage9ProfileData profileData = Espionage9ProfileData.DeserializeProfileData(profilePath);
                                        profileData.score = scoreInt;
                                        if ("1".Equals(win))
                                            profileData.wins++;
                                        profileData.flag1 = "1".Equals(flag1);
                                        profileData.flag2 = "1".Equals(flag2);

                                        profileData.SerializeProfileData(profilePath);

                                        return $"<xml><success>true</success></xml>";
                                    }

                                    errMsg = $"[Espionage9] - PhpRequest: Profile doesn't exist!";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>No Profile available</error><extra>{errMsg}</extra><function>ProcessPhpRequest</function></xml>";
                                }
                                else
                                {
                                    string errMsg = $"[Espionage9] - PhpRequest: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessPhpRequest</function></xml>";
                                }
                            case "start":
                                finger = data.GetParameterValue("finger");

                                ExpectedHash = NDREAMSServerUtils.Server_GetSignature(string.Empty, "espionage", func, CurrentDate);

                                if (ExpectedHash.Equals(key))
                                {
                                    if (File.Exists(profilePath))
                                    {
                                        Espionage9ProfileData profileData = Espionage9ProfileData.DeserializeProfileData(profilePath);
                                        profileData.plays++;

                                        profileData.SerializeProfileData(profilePath);

                                        return $"<xml><success>true</success></xml>";
                                    }

                                    string errMsg = $"[Espionage9] - PhpRequest: Profile doesn't exist!";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>No Profile available</error><extra>{errMsg}</extra><function>ProcessPhpRequest</function></xml>";
                                }
                                else
                                {
                                    string errMsg = $"[Espionage9] - PhpRequest: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessPhpRequest</function></xml>";
                                }
                            case "high":
                                StringBuilder sb = new StringBuilder("<xml><success>true</success>");
                                List<(string, Espionage9ProfileData)> espionage9Profiles = ReadEspionage9Files(apipath + $"/NDREAMS/Espionage9/PlayersInventory");

                                if (espionage9Profiles.Count > 0)
                                {
                                    byte i = 1;
                                    string BattleContRegexPathern = @"[\\/]+([^\\/]+)[\\/]+SecretAgentData\.xml";

                                    foreach ((string, Espionage9ProfileData) ResultScore in espionage9Profiles.OrderByDescending(x => x.Item2.score).Take(10))
                                    {
                                        // Define the regular expression to capture the player name before "/SecretAgentData.xml"
                                        Match match = Regex.Match(ResultScore.Item1, BattleContRegexPathern);

                                        if (match.Success)
                                        {
                                            sb.Append($"<high name=\"{match.Groups[1].Value}\" pos=\"{i}\" score=\"{ResultScore.Item2.score}\"/>");
                                            i++;
                                        }
                                    }
                                }

                                return sb.ToString() + "</xml>";
                        }
                    }

                    ms.Flush();
                }
            }

            return null;
        }

        private static List<(string, Espionage9ProfileData)> ReadEspionage9Files(string directoryPath)
        {
            List<(string, Espionage9ProfileData)> espionage9ProfilesList = new List<(string, Espionage9ProfileData)>();

            try
            {
                foreach (string filePath in Directory.GetFiles(directoryPath, "SecretAgentData.xml", SearchOption.AllDirectories))
                {
                    espionage9ProfilesList.Add((filePath, Espionage9ProfileData.DeserializeProfileData(filePath)));
                }
            }
            catch (Exception ex)
            {
                CustomLogger.LoggerAccessor.LogError($"[Espionage9] - PhpRequest - ReadEspionage9Files: An error occurred while reading profiles in directory: {directoryPath} ({ex})");
                espionage9ProfilesList.Clear();
            }

            return espionage9ProfilesList;
        }
    }

    public class Espionage9ProfileData
    {
        [XmlElement(ElementName = "score")]
        public int score { get; set; }

        [XmlElement(ElementName = "plays")]
        public int plays { get; set; }

        [XmlElement(ElementName = "wins")]
        public int wins { get; set; }

        [XmlElement(ElementName = "flag1")]
        public bool flag1 { get; set; }

        [XmlElement(ElementName = "flag2")]
        public bool flag2 { get; set; }

        public void SerializeProfileData(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Espionage9ProfileData));
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, this);
            }
        }

        public static Espionage9ProfileData DeserializeProfileData(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Espionage9ProfileData));
            using (StreamReader reader = new StreamReader(filePath))
            {
                return (Espionage9ProfileData)serializer.Deserialize(reader);
            }
        }
    }
}
