using MultiServerLibrary.HTTP;
using HttpMultipartParser;
using System.IO;
using System;
using System.Xml.Serialization;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WebAPIService.GameServices.PSHOME.NDREAMS;

namespace WebAPIService.GameServices.PSHOME.NDREAMS.Xi2
{
    public class BattleCont
    {
        public static string ProcessBattleCont(DateTime CurrentDate, byte[] PostData, string ContentType, string apipath)
        {
            string func = null;
            string key = null;
            string ExpectedHash = null;
            string name = null;
            string blame = null;
            string win = null;
            string score = null;
            string hash = null;
            string com = null;

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
                        string directoryPath = apipath + $"/NDREAMS/Xi2/PlayersInventory/{name}";
                        string profilePath = directoryPath + "/BattleCont.xml";

                        switch (func)
                        {
                            case "load":
                                ExpectedHash = NDREAMSServerUtils.Server_GetSignatureCustom(Cont.ContSignature, name, func, CurrentDate);

                                if (ExpectedHash.Equals(key))
                                {
                                    BattleContProfileData profileData;

                                    if (File.Exists(profilePath))
                                        profileData = BattleContProfileData.DeserializeProfileData(profilePath);
                                    else
                                    {
                                        profileData = new BattleContProfileData() { SaveData = "NEW PLAYER", Hash = "NEW PLAYER", Completed = 0, Wins = 0, Losses = 0, Conn_Lost = 0, Quits = 0, Best = 0, Average = 0, Packs = 0 };

                                        Directory.CreateDirectory(directoryPath);
                                        profileData.SerializeProfileData(profilePath);
                                    }

                                    return $"<xml><success>true</success><result><Data>{profileData.SaveData}</Data><Hash>{profileData.Hash}</Hash><Missions>{profileData.Completed}</Missions><Wins>{profileData.Wins}</Wins><Lost>{profileData.Losses}</Lost>" +
                                        $"<Best>{profileData.Best}</Best><Avg>{profileData.Average}</Avg><Conn>{profileData.Conn_Lost}</Conn><Quits>{profileData.Quits}</Quits><Packs>{profileData.Packs}</Packs>" +
                                        $"<confirm>{NDREAMSServerUtils.Server_GetSignatureCustom(Cont.ContSignature, name, $"{profileData.Hash}{profileData.Wins}{profileData.Losses}{profileData.Completed}{profileData.Packs}", CurrentDate)}</confirm></result></xml>";
                                }
                                else
                                {
                                    string errMsg = $"[Xi2] - BattleCont: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessBattleCont</function></xml>";
                                }
                            case "report_quit":
                                blame = data.GetParameterValue("blame");
                                win = data.GetParameterValue("win");

                                ExpectedHash = NDREAMSServerUtils.Server_GetSignatureCustom(Cont.ContSignature, name, name + func + blame, CurrentDate);

                                if (ExpectedHash.Equals(key))
                                {
                                    if (File.Exists(profilePath))
                                    {
                                        BattleContProfileData profileData = BattleContProfileData.DeserializeProfileData(profilePath);
                                        if (win.Equals("true"))
                                            profileData.Wins++;
                                        profileData.Quits++;
                                        profileData.SerializeProfileData(profilePath);

                                        return $"<xml><success>true</success><result><Success>true</Success></result></xml>";
                                    }

                                    string errMsg = $"[Xi2] - BattleCont: Profile doesn't exist!";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>No Profile available</error><extra>{errMsg}</extra><function>ProcessBattleCont</function></xml>";
                                }
                                else
                                {
                                    string errMsg = $"[Xi2] - BattleCont: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessBattleCont</function></xml>";
                                }
                            case "report_lost":
                                blame = data.GetParameterValue("blame");

                                ExpectedHash = NDREAMSServerUtils.Server_GetSignatureCustom(Cont.ContSignature, name, name + func + blame, CurrentDate);

                                if (ExpectedHash.Equals(key))
                                {
                                    if (File.Exists(profilePath))
                                    {
                                        BattleContProfileData profileData = BattleContProfileData.DeserializeProfileData(profilePath);
                                        profileData.Conn_Lost++;
                                        profileData.SerializeProfileData(profilePath);

                                        return $"<xml><success>true</success><result><Success>true</Success></result></xml>";
                                    }

                                    string errMsg = $"[Xi2] - BattleCont: Profile doesn't exist!";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>No Profile available</error><extra>{errMsg}</extra><function>ProcessBattleCont</function></xml>";
                                }
                                else
                                {
                                    string errMsg = $"[Xi2] - BattleCont: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessBattleCont</function></xml>";
                                }
                            case "submit":
                                score = data.GetParameterValue("score");
                                win = data.GetParameterValue("win");

                                ExpectedHash = NDREAMSServerUtils.Server_GetSignatureCustom(Cont.ContSignature, name, func + score + win, CurrentDate);

                                if (ExpectedHash.Equals(key))
                                {
                                    if (File.Exists(profilePath))
                                    {
                                        BattleContProfileData profileData = BattleContProfileData.DeserializeProfileData(profilePath);
                                        if (win.Equals("true"))
                                            profileData.Wins++;
                                        if (int.TryParse(score, out int integerScore))
                                            profileData.Packs = integerScore;
                                        profileData.SerializeProfileData(profilePath);

                                        return $"<xml><success>true</success><result><Success>true</Success><Wins>{profileData.Wins}</Wins><Lost>{profileData.Losses}</Lost><Best>{profileData.Best}</Best><Avg>{profileData.Average}</Avg>" +
                                        $"<confirm>{NDREAMSServerUtils.Server_GetSignatureCustom(Cont.ContSignature, name, $"{profileData.Wins}{profileData.Losses}{profileData.Best}", CurrentDate)}</confirm></result></xml>";
                                    }

                                    string errMsg = $"[Xi2] - BattleCont: Profile doesn't exist!";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>No Profile available</error><extra>{errMsg}</extra><function>ProcessBattleCont</function></xml>";
                                }
                                else
                                {
                                    string errMsg = $"[Xi2] - BattleCont: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessBattleCont</function></xml>";
                                }
                            case "save":
                                string SaveData = data.GetParameterValue("data");
                                win = data.GetParameterValue("win");
                                hash = data.GetParameterValue("hash");
                                score = data.GetParameterValue("score");
                                try
                                {
                                    com = data.GetParameterValue("com");
                                }
                                catch
                                {
                                    com = string.Empty;
                                }

                                ExpectedHash = NDREAMSServerUtils.Server_GetSignatureCustom(Cont.ContSignature, name, win + func + hash + score + com, CurrentDate);

                                if (ExpectedHash.Equals(key))
                                {
                                    if (File.Exists(profilePath))
                                    {
                                        BattleContProfileData profileData = BattleContProfileData.DeserializeProfileData(profilePath);
                                        profileData.SaveData = SaveData;
                                        profileData.Hash = hash;
                                        if (win.Equals("true"))
                                            profileData.Wins++;
                                        if (int.TryParse(score, out int integerScore))
                                            profileData.Packs = integerScore;
                                        profileData.SerializeProfileData(profilePath);

                                        return $"<xml><success>true</success><result><Success>true</Success><Data>{profileData.SaveData}</Data><Hash>{profileData.Hash}</Hash><Missions>{profileData.Completed}</Missions><Packs>{profileData.Packs}</Packs>" +
                                        $"<confirm>{NDREAMSServerUtils.Server_GetSignatureCustom(Cont.ContSignature, name, $"{profileData.Hash}{profileData.Completed}", CurrentDate)}</confirm></result></xml>";
                                    }

                                    string errMsg = $"[Xi2] - BattleCont: Profile doesn't exist!";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>No Profile available</error><extra>{errMsg}</extra><function>ProcessBattleCont</function></xml>";
                                }
                                else
                                {
                                    string errMsg = $"[Xi2] - BattleCont: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessBattleCont</function></xml>";
                                }
                            case "stats":
                                ExpectedHash = NDREAMSServerUtils.Server_GetSignatureCustom(Cont.ContSignature, name, func, CurrentDate);

                                if (ExpectedHash.Equals(key))
                                {
                                    if (File.Exists(profilePath))
                                    {
                                        BattleContProfileData profileData = BattleContProfileData.DeserializeProfileData(profilePath);

                                        return $"<xml><success>true</success><result><Wins>{profileData.Wins}</Wins><Lost>{profileData.Losses}</Lost><Best>{profileData.Best}</Best><Avg>{profileData.Average}</Avg>" +
                                        $"<Conn>{profileData.Conn_Lost}</Conn><Quits>{profileData.Quits}</Quits><confirm>{NDREAMSServerUtils.Server_GetSignatureCustom(Cont.ContSignature, name, $"{profileData.Quits}{profileData.Wins}{profileData.Losses}{profileData.Average}{profileData.Best}{profileData.Conn_Lost}", CurrentDate)}" +
                                        $"</confirm></result></xml>";
                                    }

                                    string errMsg = $"[Xi2] - BattleCont: Profile doesn't exist!";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>No Profile available</error><extra>{errMsg}</extra><function>ProcessBattleCont</function></xml>";
                                }
                                else
                                {
                                    string errMsg = $"[Xi2] - BattleCont: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessBattleCont</function></xml>";
                                }
                            case "highscores":
                                ExpectedHash = NDREAMSServerUtils.Server_GetSignatureCustom(Cont.ContSignature, name, func, CurrentDate);

                                if (ExpectedHash.Equals(key))
                                {
                                    StringBuilder sb = new StringBuilder("<xml><success>true</success><result><Success>true</Success>");
                                    List<(string, BattleContProfileData)> battleContProfiles = ReadBattleContFiles(apipath + $"/NDREAMS/Xi2/PlayersInventory");

                                    if (battleContProfiles.Count > 0)
                                    {
                                        byte i = 1;
                                        string BattleContRegexPathern = @"[\\/]+([^\\/]+)[\\/]+BattleCont\.xml";

                                        foreach ((string, BattleContProfileData) ResultScore in battleContProfiles.OrderByDescending(x => x.Item2.Best).Take(10))
                                        {
                                            // Define the regular expression to capture the player name before "/BattleCont.xml"
                                            Match match = Regex.Match(ResultScore.Item1, BattleContRegexPathern);

                                            if (match.Success)
                                            {
                                                sb.Append($"<Scores name=\"{match.Groups[1].Value}\" rank=\"{i}\" score=\"{ResultScore.Item2.Best}\"/>");
                                                i++;
                                            }
                                        }

                                        i = 1;

                                        foreach ((string, BattleContProfileData) ResultScore in battleContProfiles.OrderByDescending(x => x.Item2.Wins).Take(10))
                                        {
                                            // Define the regular expression to capture the player name before "/BattleCont.xml"
                                            Match match = Regex.Match(ResultScore.Item1, BattleContRegexPathern);

                                            if (match.Success)
                                            {
                                                sb.Append($"<Wins name=\"{match.Groups[1].Value}\" rank=\"{i}\" wins=\"{ResultScore.Item2.Wins}\"/>");
                                                i++;
                                            }
                                        }
                                    }

                                    return sb.ToString() + "</result></xml>";
                                }
                                else
                                {
                                    string errMsg = $"[Xi2] - BattleCont: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessBattleCont</function></xml>";
                                }
                        }
                    }

                    ms.Flush();
                }
            }

            return null;
        }

        private static List<(string, BattleContProfileData)> ReadBattleContFiles(string directoryPath)
        {
            List<(string, BattleContProfileData)> battleContList = new List<(string, BattleContProfileData)>();

            try
            {
                foreach (string filePath in Directory.GetFiles(directoryPath, "BattleCont.xml", SearchOption.AllDirectories))
                {
                    battleContList.Add((filePath, BattleContProfileData.DeserializeProfileData(filePath)));
                }
            }
            catch (Exception ex)
            {
                CustomLogger.LoggerAccessor.LogError($"[Xi2] - BattleCont - ReadBattleContFiles: An error occurred while reading profiles in directory: {directoryPath} ({ex})");
                battleContList.Clear();
            }

            return battleContList;
        }
    }

    public class BattleContProfileData
    {
        [XmlElement(ElementName = "SaveData")]
        public string SaveData { get; set; }

        [XmlElement(ElementName = "Hash")]
        public string Hash { get; set; }

        [XmlElement(ElementName = "Completed")]
        public int Completed { get; set; }

        [XmlElement(ElementName = "Wins")]
        public int Wins { get; set; }

        [XmlElement(ElementName = "Losses")]
        public int Losses { get; set; }

        [XmlElement(ElementName = "Conn_Lost")]
        public int Conn_Lost { get; set; }

        [XmlElement(ElementName = "Quits")]
        public int Quits { get; set; }

        [XmlElement(ElementName = "Best")]
        public int Best { get; set; }

        [XmlElement(ElementName = "Average")]
        public int Average { get; set; }

        [XmlElement(ElementName = "Packs")]
        public int Packs { get; set; }

        public void SerializeProfileData(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(BattleContProfileData));
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, this);
            }
        }

        public static BattleContProfileData DeserializeProfileData(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(BattleContProfileData));
            using (StreamReader reader = new StreamReader(filePath))
            {
                return (BattleContProfileData)serializer.Deserialize(reader);
            }
        }
    }
}
