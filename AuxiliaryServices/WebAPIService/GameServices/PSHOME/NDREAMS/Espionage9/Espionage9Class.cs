using System.Xml.Serialization;
using HttpMultipartParser;
using MultiServerLibrary.HTTP;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.PSHOME.NDREAMS.Espionage9
{
    public class Espionage9Class
    {
        private static Espionage9ScoreBoardData _leaderboard = null;

        public static string ProcessPhpRequest(
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
                        var directoryPath =
                            apipath + $"/NDREAMS/Espionage9/PlayersInventory/{name}";
                        var profilePath = directoryPath + "/SecretAgentData.xml";
                        string ExpectedHash;
                        string finger;
                        switch (func)
                        {
                            case "get":
                                ExpectedHash = NDREAMSServerUtils.Server_GetSignature(
                                    string.Empty,
                                    "espionage",
                                    func,
                                    CurrentDate
                                );

                                if (ExpectedHash.Equals(key))
                                {
                                    Espionage9ProfileData profileData;

                                    if (File.Exists(profilePath))
                                        profileData = Espionage9ProfileData.DeserializeProfileData(
                                            profilePath
                                        );
                                    else
                                    {
                                        profileData = new Espionage9ProfileData()
                                        {
                                            score = 0,
                                            plays = 0,
                                            wins = 0,
                                            flag1 = false,
                                            flag2 = false,
                                        };

                                        Directory.CreateDirectory(directoryPath);
                                        profileData.SerializeProfileData(profilePath);
                                    }

                                    return $"<xml><success>true</success><score>{profileData.score}</score><plays>{profileData.plays}</plays><wins>{profileData.wins}</wins><flag1>{profileData.flag1}</flag1>"
                                        + $"<flag2>{profileData.flag2}</flag2><confirm>{NDREAMSServerUtils.Server_GetSignature(string.Empty, name, $"{profileData.score}{profileData.plays}{profileData.wins}", CurrentDate)}</confirm></xml>";
                                }
                                else
                                {
                                    var errMsg =
                                        $"[Espionage9] - PhpRequest: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessPhpRequest</function></xml>";
                                }
                            case "set":
                                finger = data.GetParameterValue("finger");
                                var score = data.GetParameterValue("score");
                                var win = data.GetParameterValue("win");
                                var flag1 = data.GetParameterValue("flag1");
                                var flag2 = data.GetParameterValue("flag2");
                                ExpectedHash = NDREAMSServerUtils.Server_GetSignature(
                                    string.Empty,
                                    "espionage",
                                    func,
                                    CurrentDate
                                );

                                if (ExpectedHash.Equals(key))
                                {
                                    string errMsg;

                                    if (File.Exists(profilePath))
                                    {
                                        if (!int.TryParse(score, out var scoreInt))
                                        {
                                            errMsg =
                                                $"[Espionage9] - PhpRequest: invalid score argument!";
                                            CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                            return $"<xml><success>false</success><error>Invalid score argument format</error><extra>{errMsg}</extra><function>ProcessPhpRequest</function></xml>";
                                        }

                                        var profileData =
                                            Espionage9ProfileData.DeserializeProfileData(
                                                profilePath
                                            );
                                        profileData.score = scoreInt;
                                        if ("1".Equals(win))
                                        {
                                            _leaderboard ??= new Espionage9ScoreBoardData(
                                                LeaderboardDbContext.BuildOptions(
                                                    0,
                                                    $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}"
                                                )
                                            );

                                            _ = _leaderboard.UpdateScoreAsync(name, scoreInt);

                                            profileData.wins++;
                                        }
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
                                    var errMsg =
                                        $"[Espionage9] - PhpRequest: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessPhpRequest</function></xml>";
                                }
                            case "start":
                                finger = data.GetParameterValue("finger");

                                ExpectedHash = NDREAMSServerUtils.Server_GetSignature(
                                    string.Empty,
                                    "espionage",
                                    func,
                                    CurrentDate
                                );

                                if (ExpectedHash.Equals(key))
                                {
                                    if (File.Exists(profilePath))
                                    {
                                        var profileData =
                                            Espionage9ProfileData.DeserializeProfileData(
                                                profilePath
                                            );
                                        profileData.plays++;

                                        profileData.SerializeProfileData(profilePath);

                                        return $"<xml><success>true</success></xml>";
                                    }

                                    var errMsg =
                                        $"[Espionage9] - PhpRequest: Profile doesn't exist!";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>No Profile available</error><extra>{errMsg}</extra><function>ProcessPhpRequest</function></xml>";
                                }
                                else
                                {
                                    var errMsg =
                                        $"[Espionage9] - PhpRequest: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessPhpRequest</function></xml>";
                                }
                            case "high":
                                _leaderboard ??= new Espionage9ScoreBoardData(
                                    LeaderboardDbContext.BuildOptions(
                                        0,
                                        $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}"
                                    )
                                );

                                return _leaderboard.SerializeToString(null, 10).Result;
                        }
                    }

                    ms.Flush();
                }
            }

            return null;
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
            var serializer = new XmlSerializer(typeof(Espionage9ProfileData));
            using (var writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, this);
            }
        }

        public static Espionage9ProfileData DeserializeProfileData(string filePath)
        {
            var serializer = new XmlSerializer(typeof(Espionage9ProfileData));
            using (var reader = new StreamReader(filePath))
            {
                return (Espionage9ProfileData)serializer.Deserialize(reader);
            }
        }
    }
}
