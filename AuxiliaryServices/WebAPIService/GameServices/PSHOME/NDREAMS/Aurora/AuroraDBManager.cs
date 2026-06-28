using System.Text.RegularExpressions;
using CastleLibrary.NetHasher;
using CastleLibrary.Utils;
using HttpMultipartParser;
using MultiServerLibrary.Extension;
using MultiServerLibrary.HTTP;
using Newtonsoft.Json.Linq;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.PSHOME.NDREAMS.Aurora
{
    public static partial class AuroraDBManager
    {
        private static OrbrunnerScoreBoardData _leaderboard = null;

        public static string ProcessVisitCounter2(
            DateTime CurrentDate,
            byte[] PostData,
            string ContentType,
            string apipath
        )
        {
            var func = string.Empty;
            var name = string.Empty;
            var game = string.Empty;
            var key = string.Empty;
            var boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (var ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    func = data.GetParameterValue("func");
                    name = data.GetParameterValue("name");
                    game = data.GetParameterValue("game");
                    var territory = data.GetParameterValue("territory");
                    key = data.GetParameterValue("key");

                    ms.Flush();
                }

                var ExpectedHash = NDREAMSServerUtils.Server_GetSignatureCustom(
                    "nDreamsAuroraCont",
                    name,
                    func + game,
                    CurrentDate
                );

                if (key == ExpectedHash)
                {
                    Directory.CreateDirectory(apipath + $"/NDREAMS/Aurora/PlayersInventory/{name}");

                    var PlayerVisitProfilePath =
                        apipath + $"/NDREAMS/Aurora/PlayersInventory/{name}/visit_counter.json";
                    var Hash = DotNetHasher.ComputeMD5(Array.Empty<byte>()).BytesToHexStr(); // Seems to not make a difference.

                    if (File.Exists(PlayerVisitProfilePath))
                    {
                        // Parse the JSON string
                        var jsonObject = JObject.Parse(File.ReadAllText(PlayerVisitProfilePath));

                        // Check if the game exists in the JSON object
                        if (jsonObject.ContainsKey(game))
                        {
                            // Increment the counter associated with the game
                            var currentValue = jsonObject[game]?.Value<int?>();

                            jsonObject[game] =
                                currentValue == null
                                    ? (JToken)currentValue
                                    : (JToken)(currentValue + 1);
                        }
                        else
                            // If the game doesn't exist, add it with the value 1
                            jsonObject.Add(game, 1);

                        // Convert the updated JObject back to JSON string
                        File.WriteAllText(PlayerVisitProfilePath, jsonObject.ToString());
                    }
                    else
                        File.WriteAllText(PlayerVisitProfilePath, $"{{\"{game}\":1}}");

                    return $"<xml><success>true</success><result><Success>true</Success><Hash>{Hash}</Hash><game>{game}</game><confirm>{NDREAMSServerUtils.Server_GetSignatureCustom("nDreamsAuroraCont", name, $"{Hash}{game}", CurrentDate)}</confirm></result></xml>";
                }
                else
                {
                    var errMsg =
                        $"[nDreams] - VisitCounter2: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                    return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessVisitCounter2</function></xml>";
                }
            }

            return null;
        }

        public static string ProcessTheEnd(
            DateTime CurrentDate,
            byte[] PostData,
            string ContentType,
            string apipath
        )
        {
            var func = string.Empty;
            var name = string.Empty;
            var doom = string.Empty;
            var key = string.Empty;
            var boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (var ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    func = data.GetParameterValue("func");
                    name = data.GetParameterValue("name");
                    key = data.GetParameterValue("key");

                    try
                    {
                        doom = data.GetParameterValue("doom");
                    }
                    catch
                    {
                        // Not Important.
                    }

                    ms.Flush();
                }

                var ExpectedHash = NDREAMSServerUtils.Server_GetSignatureCustom(
                    "nDreamsAuroraTheEnd",
                    name,
                    !string.IsNullOrEmpty(doom) ? func + doom : func,
                    CurrentDate
                );

                if (key == ExpectedHash)
                {
                    Directory.CreateDirectory(apipath + "/NDREAMS/Aurora/TheEnd");

                    var DayProfilePath = apipath + $"/NDREAMS/Aurora/TheEnd/{name}.txt";
                    var qa = "false";
                    ushort state = 6;
                    var message = 10; // Seems unused.

                    // Get the current day of the week
                    switch (DateTime.Today.DayOfWeek)
                    {
                        case DayOfWeek.Monday:
                            state = 0;
                            break;
                        case DayOfWeek.Tuesday:
                            state = 1;
                            break;
                        case DayOfWeek.Wednesday:
                            state = 2;
                            break;
                        case DayOfWeek.Thursday:
                            state = 3;
                            break;
                        case DayOfWeek.Friday:
                            state = 4;
                            break;
                        case DayOfWeek.Saturday:
                            state = 5;
                            break;
                    }

                    if (File.Exists(DayProfilePath))
                    {
                        var ProfileContent = File.ReadAllText(DayProfilePath);

                        var qaMatch = MyRegex().Match(ProfileContent);
                        var stateMatch = new Regex(@"state=(\d+)", RegexOptions.IgnoreCase).Match(
                            ProfileContent
                        );

                        // Check if both matches are successful
                        if (qaMatch.Success && stateMatch.Success)
                        {
                            // Extract the values
                            qa = qaMatch.Groups[1].Value.ToLower();

                            if (qa == "true")
                                state = ushort.Parse(stateMatch.Groups[1].Value);
                        }
                        else
                            CustomLogger.LoggerAccessor.LogWarn(
                                $"[nDreams] - TheEnd: Profile:{DayProfilePath} has an invalid format! Using default..."
                            );
                    }
                    else
                        File.WriteAllText(DayProfilePath, $"qa={qa},state=0");

                    if (
                        func == "ChangeDoomLevel"
                        && !string.IsNullOrEmpty(doom)
                        && ushort.TryParse(doom, out var resstate)
                    )
                    {
                        state = resstate;
                        File.WriteAllText(DayProfilePath, $"qa={qa},state={state}");
                    }

                    return $"<xml><success>true</success><result><state>{state}</state><message>{message}</message><qa>{qa}</qa><confirm>{NDREAMSServerUtils.Server_GetSignatureCustom("nDreamsAuroraTheEnd", name, $"{state}{message}{qa}", CurrentDate)}</confirm></result></xml>";
                }
                else
                {
                    var errMsg =
                        $"[nDreams] - TheEnd: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                    return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessTheEnd</function></xml>";
                }
            }

            return null;
        }

        public static string ProcessComplexABTest(
            DateTime CurrentDate,
            byte[] PostData,
            string ContentType
        )
        {
            var func = string.Empty;
            var name = string.Empty;
            var key = string.Empty;
            var boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (var ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    func = data.GetParameterValue("func");
                    name = data.GetParameterValue("name");
                    key = data.GetParameterValue("key");

                    ms.Flush();
                }

                var ExpectedHash = NDREAMSServerUtils.Server_GetSignatureCustom(
                    "nDreamsCommiePlexCont",
                    name,
                    func,
                    CurrentDate
                );

                if (key == ExpectedHash)
                    return $"<xml><success>true</success><result><Success>true</Success></result></xml>";
                else
                {
                    var errMsg =
                        $"[nDreams] - ComplexABTest: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                    return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessComplexABTest</function></xml>";
                }
            }

            return null;
        }

        public static string ProcessOrbrunnerScores(
            DateTime CurrentDate,
            byte[] PostData,
            string ContentType,
            string apipath
        )
        {
            var func = string.Empty;
            var name = string.Empty;
            var score = string.Empty;
            var xp = string.Empty;
            var orbs = string.Empty;
            var key = string.Empty;
            var boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (var ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    func = data.GetParameterValue("func");
                    try
                    {
                        name = data.GetParameterValue("name");
                        score = data.GetParameterValue("score");
                        xp = data.GetParameterValue("xp");
                        orbs = data.GetParameterValue("orbs");
                        key = data.GetParameterValue("key");
                    }
                    catch
                    {
                        // Not Important.
                    }

                    ms.Flush();
                }

                var high = string.Empty;
                switch (func)
                {
                    case "submit":
                        var ExpectedHash = NDREAMSServerUtils.Server_GetSignatureCustom(
                            "nDreamsAuroraCont",
                            name,
                            func + score + orbs,
                            CurrentDate
                        );

                        if (key == ExpectedHash)
                        {
                            var best = 0;
                            var Hash = DotNetHasher.ComputeMD5(Array.Empty<byte>()).BytesToHexStr();

                            if (int.TryParse(score, out var resscore))
                            {
                                if (_leaderboard == null)
                                {
                                    _leaderboard = new OrbrunnerScoreBoardData(
                                        LeaderboardDbContext.BuildOptions(
                                            0,
                                            $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}"
                                        )
                                    );

                                    _ = _leaderboard.UpdateScoreAsync("EatFlammingDeath", 50000);
                                }

                                _ = _leaderboard.UpdateScoreAsync(name, resscore);

                                var scoreExtraction = _leaderboard.GetTopScoresAsync(1).Result;
                                if (scoreExtraction.Count != 0)
                                {
                                    (string, int)? HighestScore = (
                                        scoreExtraction.First().PsnId,
                                        (int)scoreExtraction.First().Score
                                    );
                                    best = HighestScore.Value.Item2;
                                    high = $"{HighestScore.Value.Item1},{best}";
                                }
                            }

                            return $"<xml><success>true</success><result><Success>true</Success><Hash>{Hash}</Hash><high>{high}</high><best>{best}</best>"
                                + $"<score>{score}</score><exp>{(File.Exists(apipath + $"/NDREAMS/Aurora/PlayersInventory/{name}/inventory.json") ? NDREAMSProfilesUtils.ExtractProfileProperties(File.ReadAllText(apipath + $"/NDREAMS/Aurora/PlayersInventory/{name}/inventory.json")).Item1.ToString() : xp)}"
                                + $"</exp><confirm>{NDREAMSServerUtils.Server_GetSignatureCustom("nDreamsAuroraCont", name, $"{Hash}{high}", CurrentDate)}</confirm></result></xml>";
                        }
                        else
                        {
                            var errMsg =
                                $"[nDreams] - OrbrunnerScores: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                            CustomLogger.LoggerAccessor.LogWarn(errMsg);
                            return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessOrbrunnerScores</function></xml>";
                        }
                    case "high":
                        if (_leaderboard == null)
                        {
                            _leaderboard = new OrbrunnerScoreBoardData(
                                LeaderboardDbContext.BuildOptions(
                                    0,
                                    $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}"
                                )
                            );

                            _ = _leaderboard.UpdateScoreAsync("EatFlammingDeath", 50000);
                        }
                        return $"<xml><success>true</success><result><high>{_leaderboard.SerializeToString(null, 20).Result ?? string.Empty}</high></result></xml>";
                }
            }

            return null;
        }

        public static string ProcessConsumables(
            DateTime CurrentDate,
            byte[] PostData,
            string ContentType,
            string apipath
        )
        {
            var func = string.Empty;
            var name = string.Empty;
            var everything = string.Empty;
            var consumable = string.Empty;
            var count = string.Empty;
            var key = string.Empty;
            var boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (var ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    func = data.GetParameterValue("func");
                    name = data.GetParameterValue("name");
                    var territory = data.GetParameterValue("territory");
                    key = data.GetParameterValue("key");
                    try
                    {
                        everything = data.GetParameterValue("everything");
                    }
                    catch
                    {
                        // Not Important.
                    }
                    try
                    {
                        consumable = data.GetParameterValue("consumable");
                    }
                    catch
                    {
                        // Not Important.
                    }
                    try
                    {
                        count = data.GetParameterValue("count");
                    }
                    catch
                    {
                        // Not Important.
                    }

                    ms.Flush();
                }

                var directoryPath =
                    apipath + $"/NDREAMS/Aurora/PlayersInventory/{name}/Consumables";

                Directory.CreateDirectory(directoryPath);

                string ExpectedHash;
                switch (func)
                {
                    case "update":
                        ExpectedHash = NDREAMSServerUtils.Server_GetSignatureCustom(
                            "nDreamsAuroraCont",
                            name,
                            func + everything,
                            CurrentDate
                        );

                        if (key == ExpectedHash)
                        {
                            var Hash = DotNetHasher.ComputeMD5(Array.Empty<byte>()).BytesToHexStr();
                            if (!string.IsNullOrEmpty(everything))
                            {
                                var parts = everything.Split(',');

                                for (var i = 0; i < parts.Length; i += 2)
                                {
                                    File.WriteAllText(directoryPath + $"/{parts[i]}", parts[i + 1]);
                                }
                            }

                            return $"<xml><success>true</success><result><Success>true</Success><Hash>{Hash}</Hash><everything>{everything}</everything><confirm>{NDREAMSServerUtils.Server_GetSignatureCustom("nDreamsAuroraCont", name, $"{Hash}{everything}", CurrentDate)}</confirm></result></xml>";
                        }
                        else
                        {
                            var errMsg =
                                $"[nDreams] - Consumables: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                            CustomLogger.LoggerAccessor.LogWarn(errMsg);
                            return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessConsumables</function></xml>";
                        }
                    case "set":
                        ExpectedHash = NDREAMSServerUtils.Server_GetSignatureCustom(
                            "nDreamsAuroraCont",
                            name,
                            func + count,
                            CurrentDate
                        );

                        if (key == ExpectedHash)
                        {
                            var Hash = DotNetHasher.ComputeMD5(Array.Empty<byte>()).BytesToHexStr();

                            if (!string.IsNullOrEmpty(consumable))
                                File.WriteAllText(directoryPath + $"/{consumable}", count);

                            return $"<xml><success>true</success><result><Success>true</Success><Hash>{Hash}</Hash><count>{count}</count><confirm>{NDREAMSServerUtils.Server_GetSignatureCustom("nDreamsAuroraCont", name, $"{Hash}{count}", CurrentDate)}</confirm></result></xml>";
                        }
                        else
                        {
                            var errMsg =
                                $"[nDreams] - Consumables: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                            CustomLogger.LoggerAccessor.LogWarn(errMsg);
                            return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessConsumables</function></xml>";
                        }
                    case "get":
                        ExpectedHash = NDREAMSServerUtils.Server_GetSignatureCustom(
                            "nDreamsAuroraCont",
                            name,
                            func + consumable,
                            CurrentDate
                        );

                        if (key == ExpectedHash)
                        {
                            var Hash = DotNetHasher.ComputeMD5(Array.Empty<byte>()).BytesToHexStr();
                            var rescount = 0;

                            if (
                                !string.IsNullOrEmpty(consumable)
                                && File.Exists(directoryPath + $"/{consumable}")
                            )
                            {
                                try
                                {
                                    rescount = int.Parse(
                                        File.ReadAllText(directoryPath + $"/{consumable}")
                                    );
                                }
                                catch
                                {
                                    // Not Important.
                                }
                            }

                            return $"<xml><success>true</success><result><Success>true</Success><Hash>{Hash}</Hash><count>{rescount}</count><confirm>{NDREAMSServerUtils.Server_GetSignatureCustom("nDreamsAuroraCont", name, $"{Hash}{rescount}", CurrentDate)}</confirm></result></xml>";
                        }
                        else
                        {
                            var errMsg =
                                $"[nDreams] - Consumables: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                            CustomLogger.LoggerAccessor.LogWarn(errMsg);
                            return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessConsumables</function></xml>";
                        }
                }
            }

            return null;
        }

        public static string ProcessPStats(byte[] PostData, string ContentType)
        {
            var boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (var ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    var func = data.GetParameterValue("func");
                    var scene = data.GetParameterValue("scene");
                    var resdata = data.GetParameterValue("data");
                    var counts = data.GetParameterValue("counts");
                    var rgn = data.GetParameterValue("rgn");

                    ms.Flush();
                }

                return "<xml><success>true</success><result></result></xml>";
            }

            return null;
        }

        public static string ProcessReleaseInfo(
            DateTime CurrentDate,
            byte[] PostData,
            string ContentType,
            string apipath
        )
        {
            var func = string.Empty;
            var name = string.Empty;
            var version = string.Empty;
            var key = string.Empty;
            var boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (var ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    func = data.GetParameterValue("func");
                    name = data.GetParameterValue("name");
                    key = data.GetParameterValue("key");
                    try
                    {
                        version = data.GetParameterValue("version");
                    }
                    catch
                    {
                        // Not Important.
                    }

                    ms.Flush();
                }

                var ExpectedHash = NDREAMSServerUtils.Server_GetSignatureCustom(
                    "nDreamsAuroraWelcome",
                    name,
                    !string.IsNullOrEmpty(version) ? func + version : func,
                    CurrentDate
                );

                if (key == ExpectedHash)
                {
                    Directory.CreateDirectory(apipath + $"/NDREAMS/Aurora/Welcome");

                    var introProfilePath = apipath + $"/NDREAMS/Aurora/Welcome/intro_{name}.txt";
                    var infopointsProfilePath =
                        apipath + $"/NDREAMS/Aurora/Welcome/infopoints_{name}.txt";

                    switch (func)
                    {
                        case "get":
                            return $"<xml><success>true</success><result><intro>{(File.Exists(introProfilePath) ? File.ReadAllText(introProfilePath.Replace("intro=", string.Empty)) : "true")}</intro><infopoint>{(File.Exists(infopointsProfilePath) ? File.ReadAllText(infopointsProfilePath.Replace("infopoints=", string.Empty)) : "true")}</infopoint></result></xml>";
                        case "setIntro":
                            File.WriteAllText(introProfilePath, "intro=false");
                            return "<xml><success>true</success><result></result></xml>";
                        case "setInfo":
                            File.WriteAllText(infopointsProfilePath, "infopoints=false");
                            return "<xml><success>true</success><result></result></xml>";
                    }
                }
                else
                {
                    var errMsg =
                        $"[nDreams] - ReleaseInfo: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                    return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessReleaseInfo</function></xml>";
                }
            }

            return null;
        }

        public static string ProcessAuroraXP(
            DateTime CurrentDate,
            byte[] PostData,
            string ContentType,
            string apipath
        )
        {
            var func = string.Empty;
            var name = string.Empty;
            var locale = string.Empty;
            var ticket = string.Empty;
            var key = string.Empty;
            var boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (var ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    func = data.GetParameterValue("func");
                    name = data.GetParameterValue("name");
                    locale = data.GetParameterValue("locale");
                    key = data.GetParameterValue("key");
                    try
                    {
                        ticket = data.GetParameterValue("ticket");
                    }
                    catch
                    {
                        // Not Important.
                    }

                    ms.Flush();
                }

                var ExpectedHash = NDREAMSServerUtils.Server_GetSignatureCustom(
                    "nDreamsAuroraXP",
                    name,
                    !string.IsNullOrEmpty(ticket) ? func + locale + ticket : func + locale,
                    CurrentDate
                );

                if (key == ExpectedHash)
                {
                    Directory.CreateDirectory(apipath + $"/NDREAMS/Aurora/PlayersInventory/{name}");

                    var PlayerInventoryPath =
                        apipath + $"/NDREAMS/Aurora/PlayersInventory/{name}/inventory.json";

                    var Level = 1;
                    var AwaredLevels = 0;
                    var XP = 0;
                    var NextXP = 0;
                    var LastRewardXP = 0;
                    var NextRewardXP = 0;
                    var NextRewardProgress = 0F;

                    switch (func)
                    {
                        case "GetInitData":
                            var GUIDS = NDREAMSServerUtils.CreateBase64StringFromGuids([
                                "1A645C1F-91FA47C0-833EA523-4E491A8B",
                                "711733DB-785A4753-B997EF54-5E8A7D36",
                                "81F209BC-5ED54D71-AAE4EC2A-BD38AFF3",
                                "D66B9F33-FEDB4F58-87BB9715-17BAB056",
                                "2F1DDA81-AD6A4098-9CFFFC7E-19A727C8",
                                "3D45C776-5D674C77-AA050D7B-0A2DF449",
                                "41FBFF75-289F4BB3-B6BFFFA1-7B674E5F",
                                "7BDC421C-A9254314-B3B2678F-62FDFB50",
                                "5A72D2DC-FF73460F-B819B3B8-1C99049F",
                                "B73BCE6C-2E594445-912F50A3-D4D42AD8",
                                "B6A6A14D-8DCD4A90-881C8411-14A0EEF7",
                                "17A05DC3-DFF343E9-AD5E07CA-47676219",
                                "E2F0C65A-49494C96-A74305BA-D42EBACF",
                                "03773B9F-C10B402A-BA2CD677-9631A58E",
                                "F10BEF73-93624D94-BE145C8A-80059C34",
                                "40A47CD1-D7D14EBC-8265C6D9-D2C49518",
                                "2008FF15-60CA4ED4-8180591D-4E9EE7FF",
                                "9FCE7142-4F914227-8551E923-19C20570",
                                "42D2B6D6-3DAE4E88-997C86EB-797671D2",
                                "7B0528DC-E14A4CEE-BD495333-E21635BA",
                                "7CC31C56-17D746B8-97E3A262-7CB6B627",
                                "6F0DAACA-033C4DBB-864CFC93-5D8CB134",
                                "ACD2D926-8C604F26-9A32F1A0-38F8DD50",
                                "667086C2-EA664231-9D71BCCC-FEE9BE00",
                                "EC67D1F0-124E4405-9E3AB646-BD3D131C",
                                "C7A0DCF5-1D5E4F8E-B1EB4D3A-A88490EF",
                                "8F48A72C-605E4FC1-AF512ACC-7570A986",
                                "D7DEF370-B98D4E07-B97DA353-258B41E8",
                                "CD6F7653-54B74B48-AE468BD0-379032CF",
                                "9325DE25-5BEF45A9-B9BF750C-0496871F",
                                "57421054-BED04C84-AA1DAF30-497CF607",
                                "0F758E3C-9C3C4D21-80991302-5125AB00",
                                "4B2B989E-CC0B4AA8-858B1FE3-859FA45B",
                                "CC3B9E04-A0DE4733-95B7BFBB-0F865CFE",
                            ]);

                            if (string.IsNullOrEmpty(GUIDS))
                                CustomLogger.LoggerAccessor.LogError(
                                    $"[nDreams] - AuroraXP: GUIDS list produced a null result, issues will happen!!"
                                );
                            else
                            {
#if DEBUG
                                CustomLogger.LoggerAccessor.LogInfo(
                                    $"[nDreams] - AuroraXP: Debug GUIDS list:{GUIDS}"
                                );
#endif
                            }

                            if (File.Exists(PlayerInventoryPath))
                            {
                                var PlayerStats = NDREAMSProfilesUtils.ExtractProfileProperties(
                                    File.ReadAllText(PlayerInventoryPath)
                                );
                                XP = PlayerStats.Item1;
                                Level = PlayerStats.Item2;
                            }
                            else
                                File.WriteAllText(PlayerInventoryPath, $"{{\"XP\":0,\"level\":1}}");

                            NextXP = AuroraXPTable.FindClosestHigherXP(XP) - XP;
                            LastRewardXP = AuroraXPTable.GetClosestLowerRewardXP(XP);
                            NextRewardXP = AuroraXPTable.GetClosestHigherRewardXP(XP);

                            if (NextRewardXP != 0)
                            {
                                NextRewardProgress = (float)XP / NextRewardXP;
                                NextRewardProgress = Math.Clamp(NextRewardProgress, 0.0F, 1.0F);
                            }

                            return $"<xml><success>true</success><result><Level>{Level}</Level><XP>{XP}</XP><NextXP>{NextXP}</NextXP><GUIDS>{GUIDS}</GUIDS><LastRewardXP>{LastRewardXP}</LastRewardXP>"
                                + $"<NextRewardXP>{NextRewardXP}</NextRewardXP><NextRewardProgress>{NextRewardProgress.ToString().Replace(',', '.')}</NextRewardProgress><Notifications></Notifications><History></History></result></xml>";
                        case "AddTicket":
                            var XPAwarded = 0;

                            (bool, byte[]) base64Data = ticket.IsBase64();

                            if (base64Data.Item1)
                            {
                                var DecodedTicket = base64Data.Item2;

                                if (DecodedTicket[0] == 0x00 && DecodedTicket[1] == 0x01)
                                    XPAwarded = BitConverter.ToInt16(
                                        EndianTools.EndianAwareConverter.isLittleEndianSystem
                                            ? [DecodedTicket[22], DecodedTicket[21]]
                                            : [DecodedTicket[21], DecodedTicket[22]],
                                        0
                                    );
                                else
                                    CustomLogger.LoggerAccessor.LogWarn(
                                        $"[nDreams] - AuroraXP: Unknown Base64 Ticket format detected, generated by:{name}. Ignoring..."
                                    );
                            }
                            else
                                CustomLogger.LoggerAccessor.LogWarn(
                                    $"[nDreams] - AuroraXP: Unknown Ticket format detected, generated by:{name}. Ignoring..."
                                );

                            if (File.Exists(PlayerInventoryPath))
                            {
                                var profile = JObject.Parse(File.ReadAllText(PlayerInventoryPath));

                                XP = NDREAMSProfilesUtils.UpdateXP(profile, XPAwarded);

                                var LVLProperties = NDREAMSProfilesUtils.UpdateLevel(
                                    profile,
                                    AuroraXPTable.FindClosestPreviousLevel(XP)
                                );

                                Level = LVLProperties.Item2;

                                AwaredLevels = Level - LVLProperties.Item1;

                                File.WriteAllText(PlayerInventoryPath, profile.ToString());
                            }
                            else
                            {
                                XP = XPAwarded;
                                File.WriteAllText(
                                    PlayerInventoryPath,
                                    $"{{\"XP\":{XPAwarded},\"level\":1}}"
                                );
                            }

                            NextXP = AuroraXPTable.FindClosestHigherXP(XP) - XP;
                            LastRewardXP = AuroraXPTable.GetClosestLowerRewardXP(XP);
                            NextRewardXP = AuroraXPTable.GetClosestHigherRewardXP(XP);

                            if (NextRewardXP != 0)
                            {
                                NextRewardProgress = (float)XP / NextRewardXP;
                                NextRewardProgress = Math.Clamp(NextRewardProgress, 0.0F, 1.0F);
                            }

                            return $"<xml><success>true</success><result><Level>{Level}</Level><Awarded>{XPAwarded}</Awarded><LvlChange>{AwaredLevels}</LvlChange><LvlChang>{(AwaredLevels != 0 ? "true" : "false")}</LvlChang><XP>{XP}</XP><NextXP>{NextXP}</NextXP><LastRewardXP>{LastRewardXP}</LastRewardXP>"
                                + $"<NextRewardXP>{NextRewardXP}</NextRewardXP><NextRewardProgress>{NextRewardProgress.ToString().Replace(',', '.')}</NextRewardProgress><Success>true</Success><XPAwarded>{XPAwarded}</XPAwarded><Notifications></Notifications></result></xml>";
                    }
                }
                else
                {
                    var errMsg =
                        $"[nDreams] - AuroraXP: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                    return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessAuroraXP</function></xml>";
                }
            }

            return null;
        }

        [GeneratedRegex(@"qa=(true|false)", RegexOptions.IgnoreCase, "fr-FR")]
        private static partial Regex MyRegex();
    }
}
