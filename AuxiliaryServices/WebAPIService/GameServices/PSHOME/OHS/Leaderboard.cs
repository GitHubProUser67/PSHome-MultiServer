using System.Text;
using CustomLogger;
using HttpMultipartParser;
using MultiServerLibrary.HTTP;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.PSHOME.OHS
{
    public class Leaderboard
    {
        // Skip the filler on said project as they can mess-up the leaderboards (some relies on negative scores).
        private static readonly string[] _fillerProjectSkip = ["sodium_racer"];

        private static readonly Dictionary<string, OHSScoreBoardData> _leaderboards = [];

        public static string Levelboard_GetAll(string project, int game, bool levelboard)
        {
            var dataforohs = GetAllBetterScores(project, levelboard);

            return string.IsNullOrEmpty(dataforohs) ? null : dataforohs;
        }

        public static string Leaderboard_RequestByUsers(
            string directorypath,
            byte[] PostData,
            string ContentType,
            string project,
            string batchparams,
            int game
        )
        {
            string dataforohs = null;

            if (string.IsNullOrEmpty(batchparams))
            {
                var boundary = HTTPProcessor.ExtractBoundary(ContentType);

                if (!string.IsNullOrEmpty(boundary))
                {
                    using (var ms = new MemoryStream(PostData))
                    {
                        var data = MultipartFormDataParser.Parse(ms, boundary);
                        LoggerAccessor.LogInfo(
                            $"[OHS] : Client Version - {data.GetParameterValue("version")}"
                        );
                        dataforohs = RequestByUsers(
                            directorypath,
                            JaminProcessor.JaminDeFormat(
                                data.GetParameterValue("data"),
                                true,
                                game
                            ),
                            project,
                            false
                        );
                        ms.Flush();
                    }
                }
            }
            else
                dataforohs = RequestByUsers(directorypath, batchparams, project, false);

            if (!string.IsNullOrEmpty(batchparams))
            {
                return string.IsNullOrEmpty(dataforohs) ? null : dataforohs;
            }
            else
            {
                dataforohs = string.IsNullOrEmpty(dataforohs)
                    ? JaminProcessor.JaminFormat("{ [\"status\"] = \"fail\" }", game)
                    : JaminProcessor.JaminFormat(
                        $"{{ [\"status\"] = \"success\", [\"value\"] = {dataforohs} }}",
                        game
                    );
            }

            return dataforohs;
        }

        public static string Leaderboard_RequestByRank(
            string directorypath,
            byte[] PostData,
            string ContentType,
            string project,
            string batchparams,
            int game
        )
        {
            string dataforohs = null;

            if (string.IsNullOrEmpty(batchparams))
            {
                var boundary = HTTPProcessor.ExtractBoundary(ContentType);

                if (!string.IsNullOrEmpty(boundary))
                {
                    using (var ms = new MemoryStream(PostData))
                    {
                        var data = MultipartFormDataParser.Parse(ms, boundary);
                        LoggerAccessor.LogInfo(
                            $"[OHS] : Client Version - {data.GetParameterValue("version")}"
                        );
                        dataforohs = RequestByRank(
                            directorypath,
                            JaminProcessor.JaminDeFormat(
                                data.GetParameterValue("data"),
                                true,
                                game
                            ),
                            project,
                            false
                        );
                        ms.Flush();
                    }
                }
            }
            else
                dataforohs = RequestByRank(directorypath, batchparams, project, false);

            if (!string.IsNullOrEmpty(batchparams))
            {
                return string.IsNullOrEmpty(dataforohs) ? null : dataforohs;
            }
            else
            {
                dataforohs = string.IsNullOrEmpty(dataforohs)
                    ? JaminProcessor.JaminFormat("{ [\"status\"] = \"fail\" }", game)
                    : JaminProcessor.JaminFormat(
                        $"{{ [\"status\"] = \"success\", [\"value\"] = {dataforohs} }}",
                        game
                    );
            }

            return dataforohs;
        }

        public static string Leaderboard_Update(
            string directorypath,
            byte[] PostData,
            string ContentType,
            string project,
            string batchparams,
            int game,
            bool levelboard
        )
        {
            string dataforohs = null;
            var writekey = "11111111";

            if (string.IsNullOrEmpty(batchparams))
            {
                var boundary = HTTPProcessor.ExtractBoundary(ContentType);

                if (!string.IsNullOrEmpty(boundary))
                {
                    using (var ms = new MemoryStream(PostData))
                    {
                        var data = MultipartFormDataParser.Parse(ms, boundary);
                        LoggerAccessor.LogInfo(
                            $"[OHS] : Client Version - {data.GetParameterValue("version")}"
                        );
                        var dualresult = JaminProcessor.JaminDeFormatWithWriteKey(
                            data.GetParameterValue("data"),
                            true,
                            game
                        );
                        writekey = dualresult.Item1;
                        dataforohs = dualresult.Item2;
                        ms.Flush();
                    }
                }
            }
            else
                dataforohs = batchparams;
            // TODO! writekey must be somewhere.

            string extraData = null;

            try
            {
                if (!string.IsNullOrEmpty(dataforohs))
                {
                    // Deserialize the JSON string
                    var rootObject = JsonConvert.DeserializeObject<ScoreBoardUpdate>(
                        dataforohs,
                        new JsonSerializerSettings
                        {
                            Converters = { new ScoreBoardUpdateConverter() },
                        }
                    );

                    if (rootObject != null)
                    {
                        // Extract the values
                        var user = rootObject.user;
                        var score = rootObject.score;
                        var key = rootObject.key;

                        if (
                            rootObject.value != null
                            && rootObject.value.Length > 0
                            && rootObject.value[0] is string v
                        )
                        {
                            extraData = JaminProcessor.JaminDeFormat(v, false, 0, false);
#if DEBUG
                            if (!string.IsNullOrEmpty(extraData))
                                LoggerAccessor.LogInfo(
                                    $"[OHS] : {(levelboard ? "Levelboard" : "Leaderboard")} has extra data: {extraData}"
                                );
#endif
                        }

                        dataforohs = UpdateScoreboard(
                            directorypath,
                            user,
                            score,
                            project,
                            key,
                            levelboard,
                            extraData
                        );
                    }
                    else
                        dataforohs = null;
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Leaderboard] - Update failed - {ex}");
            }

            if (!string.IsNullOrEmpty(batchparams))
            {
                return string.IsNullOrEmpty(dataforohs)
                    ? null
                    : $"{{ [\"writeKey\"] = \"{writekey}\", [\"entries\"] = {dataforohs} }}";
            }
            else
            {
                dataforohs = string.IsNullOrEmpty(dataforohs)
                    ? JaminProcessor.JaminFormat("{ [\"status\"] = \"fail\" }", game)
                    : JaminProcessor.JaminFormat(
                        $"{{ [\"status\"] = \"success\", [\"value\"] = {{ [\"writeKey\"] = \"{writekey}\", [\"entries\"] = {dataforohs} }} }}",
                        game
                    );
            }

            return dataforohs;
        }

        public static string Leaderboard_UpdatesSameEntry(
            string directorypath,
            byte[] PostData,
            string ContentType,
            string project,
            string batchparams,
            int game,
            bool levelboard
        )
        {
            string dataforohs = null;
            var writekey = "11111111";

            if (string.IsNullOrEmpty(batchparams))
            {
                var boundary = HTTPProcessor.ExtractBoundary(ContentType);

                if (!string.IsNullOrEmpty(boundary))
                {
                    using (var ms = new MemoryStream(PostData))
                    {
                        var data = MultipartFormDataParser.Parse(ms, boundary);
                        LoggerAccessor.LogInfo(
                            $"[OHS] : Client Version - {data.GetParameterValue("version")}"
                        );
                        var dualresult = JaminProcessor.JaminDeFormatWithWriteKey(
                            data.GetParameterValue("data"),
                            true,
                            game
                        );
                        writekey = dualresult.Item1;
                        dataforohs = dualresult.Item2;
                        ms.Flush();
                    }
                }
            }
            else
                dataforohs = batchparams;
            // TODO! writekey must be somewhere.

            var resultBuilder = new StringBuilder();

            string extraData = null;

            try
            {
                if (!string.IsNullOrEmpty(dataforohs))
                {
                    // Deserialize the JSON string
                    var rootObject = JsonConvert.DeserializeObject<ScoreBoardUpdateSameEntry>(
                        dataforohs,
                        new JsonSerializerSettings
                        {
                            Converters = { new ScoreBoardUpdateSameEntryConverter() },
                        }
                    );

                    if (rootObject != null)
                    {
                        // Extract the values
                        var user = rootObject.user;
                        var score = rootObject.score;
                        var keys = rootObject.keys;

                        if (
                            rootObject.value != null
                            && rootObject.value.Length > 0
                            && rootObject.value[0] is string v
                        )
                        {
                            extraData = JaminProcessor.JaminDeFormat(v, false, 0, false);
#if DEBUG
                            if (!string.IsNullOrEmpty(extraData))
                                LoggerAccessor.LogInfo(
                                    $"[OHS] : {(levelboard ? "Levelboard" : "Leaderboard")} has extra data: {extraData}"
                                );
#endif
                        }

                        if (keys != null)
                        {
                            foreach (var key in keys)
                            {
                                if (resultBuilder.Length == 0)
                                    resultBuilder.Append(
                                        UpdateScoreboard(
                                            directorypath,
                                            user,
                                            score,
                                            project,
                                            key,
                                            levelboard,
                                            extraData
                                        )
                                    );
                                else
                                    resultBuilder.Append(
                                        ", "
                                            + UpdateScoreboard(
                                                directorypath,
                                                user,
                                                score,
                                                project,
                                                key,
                                                levelboard,
                                                extraData
                                            )
                                    );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Leaderboard] - UpdatesSameEntry failed - {ex}");
            }

            var res = resultBuilder.ToString();

            resultBuilder = null;

            if (!string.IsNullOrEmpty(batchparams))
            {
                return res.Length == 0
                    ? null
                    : $"{{ [\"writeKey\"] = \"{writekey}\", [\"entries\"] = {res} }}";
            }
            else
            {
                dataforohs =
                    res.Length == 0
                        ? JaminProcessor.JaminFormat("{ [\"status\"] = \"fail\" }", game)
                        : JaminProcessor.JaminFormat(
                            $"{{ [\"status\"] = \"success\", [\"value\"] = {{ [\"writeKey\"] = \"{writekey}\", [\"entries\"] = {res} }} }}",
                            game
                        );
            }

            return dataforohs;
        }

        public static void InitializeLeaderboard(
            string directorypath,
            string tablekey,
            bool fillResults = true
        )
        {
            if (!_leaderboards.ContainsKey(tablekey))
            {
                var scoreBoard = new OHSScoreBoardData(
                    LeaderboardDbContext.BuildOptions(
                        0,
                        $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}"
                    ),
                    tablekey
                );
                if (_leaderboards.TryAdd(tablekey, scoreBoard))
                {
                    var tableParams = tablekey.Split('|');
                    var key = tableParams[1];

                    _ = scoreBoard.PerformMigrationAsync(
                        directorypath
                            + $"/{tableParams[0]}/{(tableParams.Last().EndsWith("levelboard") ? $"Levelboard_Data/levelboard_{key}.json" : $"Leaderboard_Data/scoreboard_{key}.json")}"
                    );

                    if (fillResults)
                    {
                        for (var j = 1; j < 11; j++)
                        {
                            _ = _leaderboards[tablekey]
                                .UpdateScoreAsync(FrenchNameGenerator.GetRandomWord(), 0);
                        }
                    }
                }
            }
        }

        public static string UpdateScoreboard(
            string directorypath,
            string playerId,
            int newScore,
            string project,
            string key,
            bool levelboard,
            string extraData = null
        )
        {
            var tablekey = levelboard ? project + $"|{key}" + "|levelboard" : project + $"|{key}";

            OHSScoreBoardData lb;
            lock (_leaderboards)
            {
                InitializeLeaderboard(
                    directorypath,
                    tablekey,
                    !_fillerProjectSkip.Contains(project)
                );
                lb = _leaderboards[tablekey];
            }

            _ = lb.UpdateScoreAsync(playerId, newScore);
            if (!string.IsNullOrEmpty(extraData))
                _ = lb.SetJaminExtraData(playerId, extraData);
            return lb.SerializeToStringEx(null, playerId).Result ?? "{ }";
        }

        public static string GetAllBetterScores(string project, bool levelboard)
        {
            var returnvalue = string.Empty;

            IEnumerable<KeyValuePair<string, OHSScoreBoardData>> leaderboardsToProcess;

            lock (_leaderboards)
            {
                leaderboardsToProcess = _leaderboards.Where(x =>
                    x.Key.Contains(project) && (!levelboard || x.Key.Contains("|levelboard"))
                );
            }

            foreach (var kvp in leaderboardsToProcess)
            {
                var scoreEntries = kvp.Value.GetTopScoresAsync(1).Result;

                if (scoreEntries.Count != 0)
                {
                    var scoreEntry = scoreEntries.First();

                    if (returnvalue.Length != 0)
                        returnvalue +=
                            $", [\"{kvp.Key.Split('|')[1]}\"] = {{ [\"score\"] = {(int)scoreEntry.Score}, [\"user\"] = \"{scoreEntry.PsnId}\", [\"rank\"] = 1 }}";
                    else
                        returnvalue =
                            $"{{ [\"{kvp.Key.Split('|')[1]}\"] = {{ [\"score\"] = {(int)scoreEntry.Score}, [\"user\"] = \"{scoreEntry.PsnId}\", [\"rank\"] = 1 }}";
                }
            }

            if (returnvalue.Length != 0)
                returnvalue += " }";
            else
                returnvalue = "{ }";

            return returnvalue;
        }

        public static string RequestByUsers(
            string directorypath,
            string jsontable,
            string project,
            bool levelboard
        )
        {
            var returnvalue = "{ [\"entries\"] = { }, [\"user\"] = { [\"score\"] = 0 } }";

            try
            {
                var data = JsonConvert.DeserializeObject<ScoreBoardUsersRequest>(jsontable);

                if (data != null)
                {
                    var key = data.Key;
                    var tablekey = levelboard
                        ? project + $"|{key}" + "|levelboard"
                        : project + $"|{key}";
                    var hasKey = false;

                    lock (_leaderboards)
                    {
                        InitializeLeaderboard(
                            directorypath,
                            tablekey,
                            !_fillerProjectSkip.Contains(project)
                        );
                        hasKey = _leaderboards.ContainsKey(tablekey);
                    }

                    if (hasKey)
                    {
                        List<Entities.OHSScoreboardEntry> scoreEntries;

                        var isDaily = key.Contains(
                            "daily",
                            StringComparison.InvariantCultureIgnoreCase
                        );
                        var isWeekly = key.Contains(
                            "weekly",
                            StringComparison.InvariantCultureIgnoreCase
                        );

                        scoreEntries =
                            isDaily ? _leaderboards[tablekey].GetTodayScoresAsync(-1).Result
                            : isWeekly
                                ? _leaderboards[tablekey].GetCurrentWeekScoresAsync(-1).Result
                            : _leaderboards[tablekey].GetAllScoresAsync().Result;

                        if (scoreEntries.Count != 0)
                        {
                            Dictionary<string, int> ranks = [];
                            Dictionary<int, Dictionary<string, object>> luaTable = [];

                            var i = 1;

                            foreach (
                                var entry in scoreEntries
                                    .Where(entry => data.Users.Contains(entry.PsnId))
                                    .OrderByDescending(entry => entry.Score)
                            )
                            {
                                ranks.TryAdd(entry.PsnId, i);

                                luaTable.Add(
                                    i,
                                    new Dictionary<string, object>
                                    {
                                        { "[\"user\"]", $"\"{entry.PsnId}\"" },
                                        { "[\"score\"]", $"{(int)entry.Score}" },
                                        { "[\"rank\"]", $"{i}" },
                                    }
                                );

                                i++;
                            }

                            var resultBuilder = new StringBuilder();

                            foreach (var user in data.Users)
                            {
                                foreach (var entry in scoreEntries)
                                {
                                    if (entry.PsnId == user)
                                    {
                                        if (resultBuilder.Length == 0)
                                            resultBuilder.Append(
                                                $"[\"user\"] = {{ [\"score\"] = {(int)entry.Score}, [\"rank\"] = {ranks[user]} }}"
                                            );
                                        else
                                            resultBuilder.Append(
                                                $", [\"user\"] = {{ [\"score\"] = {(int)entry.Score}, [\"rank\"] = {ranks[user]} }}"
                                            );
                                    }
                                }
                            }

                            if (resultBuilder.Length == 0)
                                resultBuilder.Append(
                                    $"[\"user\"] = {{ [\"score\"] = 0, [\"rank\"] = 0 }}"
                                );

                            returnvalue =
                                "{ [\"entries\"] = "
                                + OHSScoreBoardData.FormatScoreBoardLuaTable(luaTable)
                                + ", "
                                + resultBuilder.ToString()
                                + " }";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Leaderboard] - RequestByUsers failed - {ex}");
            }

            return returnvalue;
        }

        public static string RequestByRank(
            string directorypath,
            string jsontable,
            string project,
            bool levelboard
        )
        {
            try
            {
                var numEntries = 0;

                var start = 1;

                string user = null;

                string key = null;

                if (!string.IsNullOrEmpty(jsontable))
                {
                    var jsonDatainit = JObject.Parse(jsontable);

                    if (jsonDatainit != null)
                    {
                        var numEntriesToken = jsonDatainit["numEntries"];
                        if (numEntriesToken != null)
                            numEntries = (int)numEntriesToken;

                        var startToken = jsonDatainit["start"];
                        if (startToken != null)
                            start = (int)startToken;

                        user = (string)jsonDatainit["user"];
                        key = (string)jsonDatainit["key"];
                    }

                    if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(key))
                        return null;

                    var hasKey = false;
                    var isDaily = key.Contains(
                        "daily",
                        StringComparison.InvariantCultureIgnoreCase
                    );
                    var isWeekly = key.Contains(
                        "weekly",
                        StringComparison.InvariantCultureIgnoreCase
                    );
                    var tablekey = levelboard
                        ? project + $"|{key}" + "|levelboard"
                        : project + $"|{key}";

                    lock (_leaderboards)
                    {
                        InitializeLeaderboard(
                            directorypath,
                            tablekey,
                            !_fillerProjectSkip.Contains(project) && !(isDaily || isWeekly)
                        );
                        hasKey = _leaderboards.ContainsKey(tablekey);
                    }

                    if (hasKey)
                    {
                        return isDaily
                                ? _leaderboards[tablekey]
                                    .SerializeToStringDailyEx(null, user, start, numEntries)
                                    .Result
                            : isWeekly
                                ? _leaderboards[tablekey]
                                    .SerializeToWeeklyStringEx(null, user, start, numEntries)
                                    .Result
                            : _leaderboards[tablekey]
                                .SerializeToStringEx(null, user, start, numEntries)
                                .Result;
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Leaderboard] - RequestByRank failed - {ex}");
            }

            return $"{{ [\"user\"] = {{ [\"score\"] = 0, [\"rank\"] = 0 }}, [\"entries\"] = {{ }} }}";
        }

        public class Scoreboard
        {
            public List<ScoreboardEntry> Entries { get; set; }
        }

        public class ScoreboardEntry
        {
            public string Name { get; set; }
            public int Score { get; set; }
            public int Rank { get; set; }
        }

        public class ScoreBoardUpdateSameEntry
        {
            public string user { get; set; }
            public string[] keys { get; set; }
            public int score { get; set; }
            public object[] value { get; set; }
        }

        public class ScoreBoardUpdate
        {
            public string user { get; set; }
            public string key { get; set; }
            public int score { get; set; }
            public object[] value { get; set; }
        }

        public class ScoreBoardUsersRequest
        {
            public string[] Users { get; set; }
            public string Key { get; set; }
        }

        private class ScoreBoardUpdateSameEntryConverter : JsonConverter<ScoreBoardUpdateSameEntry>
        {
            public override ScoreBoardUpdateSameEntry ReadJson(
                JsonReader reader,
                Type objectType,
                ScoreBoardUpdateSameEntry existingValue,
                bool hasExistingValue,
                JsonSerializer serializer
            )
            {
                var jsonObject = JObject.Load(reader);

                var entry = new ScoreBoardUpdateSameEntry
                {
                    user = jsonObject["user"]?.ToString(),
                    keys = jsonObject["keys"]?.ToObject<string[]>(),
                    score = jsonObject["score"]?.ToObject<int>() ?? 0,
                };

                // Determine if "value" is a string or an array of objects
                var valueToken = jsonObject["value"];
                if (valueToken != null)
                {
                    entry.value =
                        valueToken.Type == JTokenType.String
                            ? [valueToken.ToObject<string>() ?? string.Empty]
                            : valueToken.ToObject<object[]>();
                }

                return entry;
            }

            public override void WriteJson(
                JsonWriter writer,
                ScoreBoardUpdateSameEntry value,
                JsonSerializer serializer
            )
            {
                throw new NotImplementedException();
            }
        }

        private class ScoreBoardUpdateConverter : JsonConverter<ScoreBoardUpdate>
        {
            public override ScoreBoardUpdate ReadJson(
                JsonReader reader,
                Type objectType,
                ScoreBoardUpdate existingValue,
                bool hasExistingValue,
                JsonSerializer serializer
            )
            {
                var jsonObject = JObject.Load(reader);

                var entry = new ScoreBoardUpdate
                {
                    user = jsonObject["user"]?.ToString(),
                    key = jsonObject["key"]?.ToObject<string>(),
                    score = jsonObject["score"]?.ToObject<int>() ?? 0,
                };

                // Determine if "value" is a string or an array of objects
                var valueToken = jsonObject["value"];
                if (valueToken != null)
                {
                    entry.value =
                        valueToken.Type == JTokenType.String
                            ? [valueToken.ToObject<string>() ?? string.Empty]
                            : valueToken.ToObject<object[]>();
                }

                return entry;
            }

            public override void WriteJson(
                JsonWriter writer,
                ScoreBoardUpdate value,
                JsonSerializer serializer
            )
            {
                throw new NotImplementedException();
            }
        }
    }
}
