using CustomLogger;
using HttpMultipartParser;
using Microsoft.EntityFrameworkCore;
using MultiServerLibrary.HTTP;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.PSHOME.OHS
{
    public class Leaderboard
    {
        private static Dictionary<string, OHSScoreBoardData> _leaderboards = new Dictionary<string, OHSScoreBoardData>();

        public static string Levelboard_GetAll(string project, int game, bool levelboard)
        {
            string dataforohs = GetAllBetterScores(project, levelboard);

            if (string.IsNullOrEmpty(dataforohs))
                return null;

            return dataforohs;
        }

        public static string Leaderboard_RequestByUsers(byte[] PostData, string ContentType, string project, string batchparams, int game)
        {
            string dataforohs = null;

            if (string.IsNullOrEmpty(batchparams))
            {
                string boundary = HTTPProcessor.ExtractBoundary(ContentType);

                if (!string.IsNullOrEmpty(boundary))
                {
                    using (MemoryStream ms = new MemoryStream(PostData))
                    {
                        var data = MultipartFormDataParser.Parse(ms, boundary);
                        LoggerAccessor.LogInfo($"[OHS] : Client Version - {data.GetParameterValue("version")}");
                        dataforohs = RequestByUsers(JaminProcessor.JaminDeFormat(data.GetParameterValue("data"), true, game), project, false);
                        ms.Flush();
                    }
                }
            }
            else
                dataforohs = RequestByUsers(batchparams, project, false);

            if (!string.IsNullOrEmpty(batchparams))
            {
                if (string.IsNullOrEmpty(dataforohs))
                    return null;
                else
                    return dataforohs;
            }
            else
            {
                if (string.IsNullOrEmpty(dataforohs))
                    dataforohs = JaminProcessor.JaminFormat("{ [\"status\"] = \"fail\" }", game);
                else
                    dataforohs = JaminProcessor.JaminFormat($"{{ [\"status\"] = \"success\", [\"value\"] = {dataforohs} }}", game);
            }

            return dataforohs;
        }

        public static string Leaderboard_RequestByRank(byte[] PostData, string ContentType, string project, string batchparams, int game)
        {
            string dataforohs = null;

            if (string.IsNullOrEmpty(batchparams))
            {
                string boundary = HTTPProcessor.ExtractBoundary(ContentType);

                if (!string.IsNullOrEmpty(boundary))
                {
                    using (MemoryStream ms = new MemoryStream(PostData))
                    {
                        var data = MultipartFormDataParser.Parse(ms, boundary);
                        LoggerAccessor.LogInfo($"[OHS] : Client Version - {data.GetParameterValue("version")}");
                        dataforohs = RequestByRank(JaminProcessor.JaminDeFormat(data.GetParameterValue("data"), true, game), project, false);
                        ms.Flush();
                    }
                }
            }
            else
                dataforohs = RequestByRank(batchparams, project, false);

            if (!string.IsNullOrEmpty(batchparams))
            {
                if (string.IsNullOrEmpty(dataforohs))
                    return null;
                else
                    return dataforohs;
            }
            else
            {
                if (string.IsNullOrEmpty(dataforohs))
                    dataforohs = JaminProcessor.JaminFormat("{ [\"status\"] = \"fail\" }", game);
                else
                    dataforohs = JaminProcessor.JaminFormat($"{{ [\"status\"] = \"success\", [\"value\"] = {dataforohs} }}", game);
            }

            return dataforohs;
        }

        public static string Leaderboard_Update(byte[] PostData, string ContentType, string project, string batchparams, int game, bool levelboard)
        {
            string dataforohs = null;
            string writekey = "11111111";

            if (string.IsNullOrEmpty(batchparams))
            {
                string boundary = HTTPProcessor.ExtractBoundary(ContentType);

                if (!string.IsNullOrEmpty(boundary))
                {
                    using (MemoryStream ms = new MemoryStream(PostData))
                    {
                        var data = MultipartFormDataParser.Parse(ms, boundary);
                        LoggerAccessor.LogInfo($"[OHS] : Client Version - {data.GetParameterValue("version")}");
                        (string, string) dualresult = JaminProcessor.JaminDeFormatWithWriteKey(data.GetParameterValue("data"), true, game);
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
                    ScoreBoardUpdate rootObject = JsonConvert.DeserializeObject<ScoreBoardUpdate>(dataforohs, new JsonSerializerSettings
                    {
                        Converters = { new ScoreBoardUpdateConverter() }
                    });

                    if (rootObject != null)
                    {
                        // Extract the values
                        string user = rootObject.user;
                        int score = rootObject.score;
                        string key = rootObject.key;

                        if (rootObject.value != null && rootObject.value.Length > 0 && rootObject.value[0] is string v)
                        {
                            extraData = JaminProcessor.JaminDeFormat(v, false, 0, false);
#if DEBUG
                            if (!string.IsNullOrEmpty(extraData))
                                LoggerAccessor.LogInfo($"[OHS] : {(levelboard ? "Levelboard" : "Leaderboard")} has extra data: {extraData}");
#endif
                        }

                        dataforohs = UpdateScoreboard(user, score, project, key, levelboard, extraData);
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
                if (string.IsNullOrEmpty(dataforohs))
                    return null;
                else
                    return $"{{ [\"writeKey\"] = \"{writekey}\", [\"entries\"] = {dataforohs} }}";
            }
            else
            {
                if (string.IsNullOrEmpty(dataforohs))
                    dataforohs = JaminProcessor.JaminFormat("{ [\"status\"] = \"fail\" }", game);
                else
                    dataforohs = JaminProcessor.JaminFormat($"{{ [\"status\"] = \"success\", [\"value\"] = {{ [\"writeKey\"] = \"{writekey}\", [\"entries\"] = {dataforohs} }} }}", game);
            }

            return dataforohs;
        }

        public static string Leaderboard_UpdatesSameEntry(byte[] PostData, string ContentType, string project, string batchparams, int game, bool levelboard)
        {
            string dataforohs = null;
            string writekey = "11111111";

            if (string.IsNullOrEmpty(batchparams))
            {
                string boundary = HTTPProcessor.ExtractBoundary(ContentType);

                if (!string.IsNullOrEmpty(boundary))
                {
                    using (MemoryStream ms = new MemoryStream(PostData))
                    {
                        var data = MultipartFormDataParser.Parse(ms, boundary);
                        LoggerAccessor.LogInfo($"[OHS] : Client Version - {data.GetParameterValue("version")}");
                        (string, string) dualresult = JaminProcessor.JaminDeFormatWithWriteKey(data.GetParameterValue("data"), true, game);
                        writekey = dualresult.Item1;
                        dataforohs = dualresult.Item2;
                        ms.Flush();
                    }
                }
            }
            else
                dataforohs = batchparams;
            // TODO! writekey must be somewhere.

            StringBuilder resultBuilder = new StringBuilder();

            string extraData = null;

            try
            {
                if (!string.IsNullOrEmpty(dataforohs))
                {
                    // Deserialize the JSON string
                    ScoreBoardUpdateSameEntry rootObject = JsonConvert.DeserializeObject<ScoreBoardUpdateSameEntry>(dataforohs, new JsonSerializerSettings
                    {
                        Converters = { new ScoreBoardUpdateSameEntryConverter() }
                    });

                    if (rootObject != null)
                    {
                        // Extract the values
                        string user = rootObject.user;
                        int score = rootObject.score;
                        string[] keys = rootObject.keys;

                        if (rootObject.value != null && rootObject.value.Length > 0 && rootObject.value[0] is string v)
                        {
                            extraData = JaminProcessor.JaminDeFormat(v, false, 0, false);
#if DEBUG
                            if (!string.IsNullOrEmpty(extraData))
                                LoggerAccessor.LogInfo($"[OHS] : {(levelboard ? "Levelboard" : "Leaderboard")} has extra data: {extraData}");
#endif
                        }

                        if (keys != null)
                        {
                            foreach (string key in keys)
                            {
                                if (resultBuilder.Length == 0)
                                    resultBuilder.Append(UpdateScoreboard(user, score, project, key, levelboard, extraData));
                                else
                                    resultBuilder.Append(", " + UpdateScoreboard(user, score, project, key, levelboard, extraData));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Leaderboard] - UpdatesSameEntry failed - {ex}");
            }

            string res = resultBuilder.ToString();

            resultBuilder = null;

            if (!string.IsNullOrEmpty(batchparams))
            {
                if (res.Length == 0)
                    return null;
                else
                    return $"{{ [\"writeKey\"] = \"{writekey}\", [\"entries\"] = {res} }}";
            }
            else
            {
                if (res.Length == 0)
                    dataforohs = JaminProcessor.JaminFormat("{ [\"status\"] = \"fail\" }", game);
                else
                    dataforohs = JaminProcessor.JaminFormat($"{{ [\"status\"] = \"success\", [\"value\"] = {{ [\"writeKey\"] = \"{writekey}\", [\"entries\"] = {res} }} }}", game);
            }

            return dataforohs;
        }

        public static void InitializeLeaderboard(string tablekey, bool fillResults = true)
        {
            if (!_leaderboards.ContainsKey(tablekey))
            {
                if (_leaderboards.TryAdd(tablekey, new OHSScoreBoardData(LeaderboardDbContext.OnContextBuilding(new DbContextOptionsBuilder<LeaderboardDbContext>(), 0, $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}").Options, tablekey)) && fillResults)
                {
                    for (int j = 1; j < 11; j++)
                    {
                        _ = _leaderboards[tablekey].UpdateScoreAsync(FrenchNameGenerator.GetRandomWord(), 0);
                    }
                }
            }
        }

        public static string UpdateScoreboard(string playerId, int newScore, string project, string key, bool levelboard, string extraData = null)
        {
            string scoreboarddata = string.Empty;
            string tablekey = levelboard ? project + $"|{key}" + "|levelboard" : project + $"|{key}";

            OHSScoreBoardData lb;
            lock (_leaderboards)
            {
                InitializeLeaderboard(tablekey);
                lb = _leaderboards[tablekey];
            }

            _ = lb.UpdateScoreAsync(playerId, newScore);
            if (!string.IsNullOrEmpty(extraData))
                _ = lb.SetJaminExtraData(playerId, extraData);
            return lb.SerializeToStringEx(null, playerId).Result ?? "{ }";
        }

        public static string GetAllBetterScores(string project, bool levelboard)
        {
            string returnvalue = string.Empty;

            IEnumerable<KeyValuePair<string, OHSScoreBoardData>> leaderboardsToProcess;

            lock (_leaderboards)
            {
                leaderboardsToProcess = _leaderboards
                    .Where(x => x.Key.Contains(project) && (!levelboard || x.Key.Contains("|levelboard")));
            }

            foreach (var kvp in leaderboardsToProcess)
            {
                var scoreEntries = kvp.Value.GetTopScoresAsync(1).Result;

                if (scoreEntries.Any())
                {
                    var scoreEntry = scoreEntries.First();

                    if (returnvalue.Length != 0)
                        returnvalue += $", [\"{kvp.Key.Split('|')[1]}\"] = {{ [\"score\"] = {(int)scoreEntry.Score}, [\"user\"] = \"{scoreEntry.PsnId}\" }}";
                    else
                        returnvalue = $"{{ [\"{kvp.Key.Split('|')[1]}\"] = {{ [\"score\"] = {(int)scoreEntry.Score}, [\"user\"] = \"{scoreEntry.PsnId}\" }}";
                }
            }

            if (returnvalue.Length != 0)
                returnvalue += " }";
            else
                returnvalue = "{ }";

            return returnvalue;
        }

        public static string RequestByUsers(string jsontable, string project, bool levelboard)
        {
            string returnvalue = "{ [\"entries\"] = { }, [\"user\"] = { [\"score\"] = 0 } }";

            try
            {
                ScoreBoardUsersRequest data = JsonConvert.DeserializeObject<ScoreBoardUsersRequest>(jsontable);

                if (data != null)
                {
                    string key = data.Key;
                    string tablekey = levelboard ? project + $"|{key}" + "|levelboard" : project + $"|{key}";
                    bool hasKey = false;

                    lock (_leaderboards)
                    {
                        InitializeLeaderboard(tablekey);
                        hasKey = _leaderboards.ContainsKey(tablekey);
                    }

                    if (hasKey)
                    {
                        List<Entities.OHSScoreboardEntry> scoreEntries;

                        bool isDaily = key.Contains("daily", StringComparison.InvariantCultureIgnoreCase);
                        bool isWeekly = key.Contains("weekly", StringComparison.InvariantCultureIgnoreCase);

                        if (isDaily)
                            scoreEntries = _leaderboards[tablekey].GetTodayScoresAsync(-1).Result;
                        else if (isWeekly)
                            scoreEntries = _leaderboards[tablekey].GetCurrentWeekScoresAsync(-1).Result;
                        else
                            scoreEntries = _leaderboards[tablekey].GetAllScoresAsync().Result;

                        if (scoreEntries.Any())
                        {
                            StringBuilder resultBuilder = new StringBuilder();

                            foreach (string user in data.Users)
                            {
                                foreach (var entry in scoreEntries)
                                {
                                    if (entry.PsnId == user)
                                    {
                                        if (resultBuilder.Length == 0)
                                            resultBuilder.Append($"[\"user\"] = {{ [\"score\"] = {(int)entry.Score} }}");
                                        else
                                            resultBuilder.Append($", [\"user\"] = {{ [\"score\"] = {(int)entry.Score} }}");
                                    }
                                }
                            }

                            if (resultBuilder.Length == 0)
                                resultBuilder.Append($"[\"user\"] = {{ [\"score\"] = 0 }}");

                            Dictionary<int, Dictionary<string, object>> luaTable = new Dictionary<int, Dictionary<string, object>>();

                            int i = 1;

                            foreach (var entry in scoreEntries.Where(entry => data.Users.Contains(entry.PsnId)).OrderByDescending(entry => entry.Score))
                            {
                                luaTable.Add(i, new Dictionary<string, object>
                                                        {
                                                            { "[\"user\"]", $"\"{entry.PsnId}\"" },
                                                            { "[\"score\"]", $"{(int)entry.Score}" }
                                                        });

                                i++;
                            }

                            returnvalue = "{ [\"entries\"] = " + OHSScoreBoardData.FormatScoreBoardLuaTable(luaTable) + ", " + resultBuilder.ToString() + " }";
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

        public static string RequestByRank(string jsontable, string project, bool levelboard)
        {
            try
            {
                int numEntries = 0;

                int start = 1;

                string user = null;

                string key = null;

                if (!string.IsNullOrEmpty(jsontable))
                {
                    JObject jsonDatainit = JObject.Parse(jsontable);

                    if (jsonDatainit != null)
                    {
                        JToken numEntriesToken = jsonDatainit["numEntries"];
                        if (numEntriesToken != null)
                            numEntries = (int)numEntriesToken;

                        JToken startToken = jsonDatainit["start"];
                        if (startToken != null)
                            start = (int)startToken;

                        user = (string)jsonDatainit["user"];
                        key = (string)jsonDatainit["key"];
                    }

                    if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(key))
                        return null;

                    bool hasKey = false;
                    bool isDaily = key.Contains("daily", StringComparison.InvariantCultureIgnoreCase);
                    bool isWeekly = key.Contains("weekly", StringComparison.InvariantCultureIgnoreCase);
                    string tablekey = levelboard ? project + $"|{key}" + "|levelboard" : project + $"|{key}";

                    lock (_leaderboards)
                    {
                        InitializeLeaderboard(tablekey, !(isDaily || isWeekly));
                        hasKey = _leaderboards.ContainsKey(tablekey);
                    }

                    if (hasKey)
                    {
                        if (isDaily)
                            return _leaderboards[tablekey].SerializeToStringDailyEx(null, user, start, numEntries).Result;
                        else if (isWeekly)
                            return _leaderboards[tablekey].SerializeToWeeklyStringEx(null, user, start, numEntries).Result;
                        else
                            return _leaderboards[tablekey].SerializeToStringEx(null, user, start, numEntries).Result;
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Leaderboard] - RequestByRank failed - {ex}");
            }

            return $"{{ [\"user\"] = {{ [\"score\"] = 0 }}, [\"entries\"] = {{ }} }}";
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
            public override ScoreBoardUpdateSameEntry ReadJson(JsonReader reader, Type objectType, ScoreBoardUpdateSameEntry existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                JObject jsonObject = JObject.Load(reader);

                ScoreBoardUpdateSameEntry entry = new ScoreBoardUpdateSameEntry
                {
                    user = jsonObject["user"]?.ToString(),
                    keys = jsonObject["keys"]?.ToObject<string[]>(),
                    score = jsonObject["score"]?.ToObject<int>() ?? 0
                };

                // Determine if "value" is a string or an array of objects
                JToken valueToken = jsonObject["value"];
                if (valueToken != null)
                {
                    if (valueToken.Type == JTokenType.String)
                        entry.value = new object[] { valueToken.ToObject<string>() ?? string.Empty };
                    else
                        entry.value = valueToken.ToObject<object[]>();
                }

                return entry;
            }

            public override void WriteJson(JsonWriter writer, ScoreBoardUpdateSameEntry value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }

        private class ScoreBoardUpdateConverter : JsonConverter<ScoreBoardUpdate>
        {
            public override ScoreBoardUpdate ReadJson(JsonReader reader, Type objectType, ScoreBoardUpdate existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                JObject jsonObject = JObject.Load(reader);

                ScoreBoardUpdate entry = new ScoreBoardUpdate
                {
                    user = jsonObject["user"]?.ToString(),
                    key = jsonObject["key"]?.ToObject<string>(),
                    score = jsonObject["score"]?.ToObject<int>() ?? 0
                };

                // Determine if "value" is a string or an array of objects
                JToken valueToken = jsonObject["value"];
                if (valueToken != null)
                {
                    if (valueToken.Type == JTokenType.String)
                        entry.value = new object[] { valueToken.ToObject<string>() ?? string.Empty };
                    else
                        entry.value = valueToken.ToObject<object[]>();
                }

                return entry;
            }

            public override void WriteJson(JsonWriter writer, ScoreBoardUpdate value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
}
