using System.Xml.Serialization;
using HttpMultipartParser;
using MultiServerLibrary.HTTP;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.PSHOME.NDREAMS.Xi2
{
    public class BattleCont
    {
        private static BattleContScoreBoardData _leaderboard = null;

        public static string ProcessBattleCont(
            DateTime CurrentDate,
            byte[] PostData,
            string ContentType,
            string apipath
        )
        {
            var boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (var ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    var func = data.GetParameterValue("func");
                    var key = data.GetParameterValue("key");
                    var name = data.GetParameterValue("name");

                    if (!string.IsNullOrEmpty(func))
                    {
                        var directoryPath = apipath + $"/NDREAMS/Xi2/PlayersInventory/{name}";
                        var profilePath = directoryPath + "/BattleCont.xml";
                        string ExpectedHash;
                        string blame;
                        string win;
                        string score;
                        switch (func)
                        {
                            case "load":
                                ExpectedHash = NDREAMSServerUtils.Server_GetSignatureCustom(
                                    Cont.ContSignature,
                                    name,
                                    func,
                                    CurrentDate
                                );

                                if (ExpectedHash.Equals(key))
                                {
                                    BattleContProfileData profileData;

                                    if (File.Exists(profilePath))
                                        profileData = BattleContProfileData.DeserializeProfileData(
                                            profilePath
                                        );
                                    else
                                    {
                                        profileData = new BattleContProfileData()
                                        {
                                            SaveData = "NEW PLAYER",
                                            Hash = "NEW PLAYER",
                                            Completed = 0,
                                            Wins = 0,
                                            Losses = 0,
                                            Conn_Lost = 0,
                                            Quits = 0,
                                            Best = 0,
                                            Average = 0,
                                            Packs = 0,
                                        };

                                        Directory.CreateDirectory(directoryPath);
                                        profileData.SerializeProfileData(profilePath);
                                    }

                                    return $"<xml><success>true</success><result><Data>{profileData.SaveData}</Data><Hash>{profileData.Hash}</Hash><Missions>{profileData.Completed}</Missions><Wins>{profileData.Wins}</Wins><Lost>{profileData.Losses}</Lost>"
                                        + $"<Best>{profileData.Best}</Best><Avg>{profileData.Average}</Avg><Conn>{profileData.Conn_Lost}</Conn><Quits>{profileData.Quits}</Quits><Packs>{profileData.Packs}</Packs>"
                                        + $"<confirm>{NDREAMSServerUtils.Server_GetSignatureCustom(Cont.ContSignature, name, $"{profileData.Hash}{profileData.Wins}{profileData.Losses}{profileData.Completed}{profileData.Packs}", CurrentDate)}</confirm></result></xml>";
                                }
                                else
                                {
                                    var errMsg =
                                        $"[Xi2] - BattleCont: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessBattleCont</function></xml>";
                                }
                            case "report_quit":
                                blame = data.GetParameterValue("blame");
                                win = data.GetParameterValue("win");

                                ExpectedHash = NDREAMSServerUtils.Server_GetSignatureCustom(
                                    Cont.ContSignature,
                                    name,
                                    name + func + blame,
                                    CurrentDate
                                );

                                if (ExpectedHash.Equals(key))
                                {
                                    if (File.Exists(profilePath))
                                    {
                                        var profileData =
                                            BattleContProfileData.DeserializeProfileData(
                                                profilePath
                                            );
                                        if (win.Equals("true"))
                                        {
                                            profileData.Wins++;

                                            _leaderboard ??= new BattleContScoreBoardData(
                                                LeaderboardDbContext.BuildOptions(
                                                    0,
                                                    $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}"
                                                )
                                            );

                                            _ = _leaderboard.UpdateWinsAsync(
                                                name,
                                                profileData.Wins
                                            );
                                        }
                                        profileData.Quits++;
                                        profileData.SerializeProfileData(profilePath);

                                        return $"<xml><success>true</success><result><Success>true</Success></result></xml>";
                                    }

                                    var errMsg = $"[Xi2] - BattleCont: Profile doesn't exist!";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>No Profile available</error><extra>{errMsg}</extra><function>ProcessBattleCont</function></xml>";
                                }
                                else
                                {
                                    var errMsg =
                                        $"[Xi2] - BattleCont: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessBattleCont</function></xml>";
                                }
                            case "report_lost":
                                blame = data.GetParameterValue("blame");

                                ExpectedHash = NDREAMSServerUtils.Server_GetSignatureCustom(
                                    Cont.ContSignature,
                                    name,
                                    name + func + blame,
                                    CurrentDate
                                );

                                if (ExpectedHash.Equals(key))
                                {
                                    if (File.Exists(profilePath))
                                    {
                                        var profileData =
                                            BattleContProfileData.DeserializeProfileData(
                                                profilePath
                                            );
                                        profileData.Conn_Lost++;
                                        profileData.SerializeProfileData(profilePath);

                                        return $"<xml><success>true</success><result><Success>true</Success></result></xml>";
                                    }

                                    var errMsg = $"[Xi2] - BattleCont: Profile doesn't exist!";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>No Profile available</error><extra>{errMsg}</extra><function>ProcessBattleCont</function></xml>";
                                }
                                else
                                {
                                    var errMsg =
                                        $"[Xi2] - BattleCont: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessBattleCont</function></xml>";
                                }
                            case "submit":
                                score = data.GetParameterValue("score");
                                win = data.GetParameterValue("win");

                                ExpectedHash = NDREAMSServerUtils.Server_GetSignatureCustom(
                                    Cont.ContSignature,
                                    name,
                                    func + score + win,
                                    CurrentDate
                                );

                                if (ExpectedHash.Equals(key))
                                {
                                    if (File.Exists(profilePath))
                                    {
                                        var profileData =
                                            BattleContProfileData.DeserializeProfileData(
                                                profilePath
                                            );
                                        if (win.Equals("true"))
                                        {
                                            profileData.Wins++;

                                            _leaderboard ??= new BattleContScoreBoardData(
                                                LeaderboardDbContext.BuildOptions(
                                                    0,
                                                    $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}"
                                                )
                                            );

                                            _ = _leaderboard.UpdateWinsAsync(
                                                name,
                                                profileData.Wins
                                            );
                                        }
                                        if (int.TryParse(score, out var integerScore))
                                        {
                                            profileData.Packs = integerScore;

                                            _leaderboard ??= new BattleContScoreBoardData(
                                                LeaderboardDbContext.BuildOptions(
                                                    0,
                                                    $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}"
                                                )
                                            );

                                            _ = _leaderboard.UpdateScoreAsync(
                                                name,
                                                profileData.Packs
                                            );
                                        }
                                        profileData.SerializeProfileData(profilePath);

                                        return $"<xml><success>true</success><result><Success>true</Success><Wins>{profileData.Wins}</Wins><Lost>{profileData.Losses}</Lost><Best>{profileData.Best}</Best><Avg>{profileData.Average}</Avg>"
                                            + $"<confirm>{NDREAMSServerUtils.Server_GetSignatureCustom(Cont.ContSignature, name, $"{profileData.Wins}{profileData.Losses}{profileData.Best}", CurrentDate)}</confirm></result></xml>";
                                    }

                                    var errMsg = $"[Xi2] - BattleCont: Profile doesn't exist!";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>No Profile available</error><extra>{errMsg}</extra><function>ProcessBattleCont</function></xml>";
                                }
                                else
                                {
                                    var errMsg =
                                        $"[Xi2] - BattleCont: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessBattleCont</function></xml>";
                                }
                            case "save":
                                var SaveData = data.GetParameterValue("data");
                                win = data.GetParameterValue("win");
                                var hash = data.GetParameterValue("hash");
                                score = data.GetParameterValue("score");
                                string com;
                                try
                                {
                                    com = data.GetParameterValue("com");
                                }
                                catch
                                {
                                    com = string.Empty;
                                }

                                ExpectedHash = NDREAMSServerUtils.Server_GetSignatureCustom(
                                    Cont.ContSignature,
                                    name,
                                    win + func + hash + score + com,
                                    CurrentDate
                                );

                                if (ExpectedHash.Equals(key))
                                {
                                    if (File.Exists(profilePath))
                                    {
                                        var profileData =
                                            BattleContProfileData.DeserializeProfileData(
                                                profilePath
                                            );
                                        profileData.SaveData = SaveData;
                                        profileData.Hash = hash;
                                        if (win.Equals("true"))
                                        {
                                            profileData.Wins++;

                                            _leaderboard ??= new BattleContScoreBoardData(
                                                LeaderboardDbContext.BuildOptions(
                                                    0,
                                                    $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}"
                                                )
                                            );

                                            _ = _leaderboard.UpdateWinsAsync(
                                                name,
                                                profileData.Wins
                                            );
                                        }
                                        if (int.TryParse(score, out var integerScore))
                                        {
                                            profileData.Packs = integerScore;

                                            _leaderboard ??= new BattleContScoreBoardData(
                                                LeaderboardDbContext.BuildOptions(
                                                    0,
                                                    $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}"
                                                )
                                            );

                                            _ = _leaderboard.UpdateScoreAsync(
                                                name,
                                                profileData.Packs
                                            );
                                        }
                                        profileData.SerializeProfileData(profilePath);

                                        return $"<xml><success>true</success><result><Success>true</Success><Data>{profileData.SaveData}</Data><Hash>{profileData.Hash}</Hash><Missions>{profileData.Completed}</Missions><Packs>{profileData.Packs}</Packs>"
                                            + $"<confirm>{NDREAMSServerUtils.Server_GetSignatureCustom(Cont.ContSignature, name, $"{profileData.Hash}{profileData.Completed}", CurrentDate)}</confirm></result></xml>";
                                    }

                                    var errMsg = $"[Xi2] - BattleCont: Profile doesn't exist!";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>No Profile available</error><extra>{errMsg}</extra><function>ProcessBattleCont</function></xml>";
                                }
                                else
                                {
                                    var errMsg =
                                        $"[Xi2] - BattleCont: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessBattleCont</function></xml>";
                                }
                            case "stats":
                                ExpectedHash = NDREAMSServerUtils.Server_GetSignatureCustom(
                                    Cont.ContSignature,
                                    name,
                                    func,
                                    CurrentDate
                                );

                                if (ExpectedHash.Equals(key))
                                {
                                    if (File.Exists(profilePath))
                                    {
                                        var profileData =
                                            BattleContProfileData.DeserializeProfileData(
                                                profilePath
                                            );

                                        return $"<xml><success>true</success><result><Wins>{profileData.Wins}</Wins><Lost>{profileData.Losses}</Lost><Best>{profileData.Best}</Best><Avg>{profileData.Average}</Avg>"
                                            + $"<Conn>{profileData.Conn_Lost}</Conn><Quits>{profileData.Quits}</Quits><confirm>{NDREAMSServerUtils.Server_GetSignatureCustom(Cont.ContSignature, name, $"{profileData.Quits}{profileData.Wins}{profileData.Losses}{profileData.Average}{profileData.Best}{profileData.Conn_Lost}", CurrentDate)}"
                                            + $"</confirm></result></xml>";
                                    }

                                    var errMsg = $"[Xi2] - BattleCont: Profile doesn't exist!";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>No Profile available</error><extra>{errMsg}</extra><function>ProcessBattleCont</function></xml>";
                                }
                                else
                                {
                                    var errMsg =
                                        $"[Xi2] - BattleCont: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessBattleCont</function></xml>";
                                }
                            case "highscores":
                                ExpectedHash = NDREAMSServerUtils.Server_GetSignatureCustom(
                                    Cont.ContSignature,
                                    name,
                                    func,
                                    CurrentDate
                                );

                                if (ExpectedHash.Equals(key))
                                {
                                    _leaderboard ??= new BattleContScoreBoardData(
                                        LeaderboardDbContext.BuildOptions(
                                            0,
                                            $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}"
                                        )
                                    );

                                    return _leaderboard.SerializeToString(null, 10).Result;
                                }
                                else
                                {
                                    var errMsg =
                                        $"[Xi2] - BattleCont: invalid key sent! Received:{key} Expected:{ExpectedHash}";
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
            var serializer = new XmlSerializer(typeof(BattleContProfileData));
            using (var writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, this);
            }
        }

        public static BattleContProfileData DeserializeProfileData(string filePath)
        {
            var serializer = new XmlSerializer(typeof(BattleContProfileData));
            using (var reader = new StreamReader(filePath))
            {
                return (BattleContProfileData)serializer.Deserialize(reader);
            }
        }
    }
}
