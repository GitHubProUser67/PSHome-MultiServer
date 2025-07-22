using System.IO;
using System.Collections.Generic;
using MultiServerLibrary.HTTP;
using CustomLogger;
using HttpMultipartParser;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System;
using System.Linq;

namespace WebAPIService.GameServices.OHS
{
    public class Leaderboard
    {
        public static string Levelboard_GetAll(string directorypath, int game, bool levelboard)
        {
            string dataforohs = GetAllLeaderboards(directorypath, levelboard);

            if (string.IsNullOrEmpty(dataforohs))
                return null;

            return dataforohs;
        }

        public static string Leaderboard_RequestByUsers(byte[] PostData, string ContentType, string directorypath, string batchparams, int game)
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
                        dataforohs = RequestByUsers(JaminProcessor.JaminDeFormat(data.GetParameterValue("data"), true, game), directorypath);
                        ms.Flush();
                    }
                }
            }
            else
                dataforohs = RequestByUsers(batchparams, directorypath);

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

        public static string Leaderboard_RequestByRank(byte[] PostData, string ContentType, string directorypath, string batchparams, int game)
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
                        dataforohs = RequestByRank(JaminProcessor.JaminDeFormat(data.GetParameterValue("data"), true, game), directorypath);
                        ms.Flush();
                    }
                }
            }
            else
                dataforohs = RequestByRank(batchparams, directorypath);

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

        public static string Leaderboard_Update(byte[] PostData, string ContentType, string directorypath, string batchparams, int game, bool levelboard)
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

            string value = null;

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
                            value = JaminProcessor.JaminDeFormat(v, false, 0, false);

                            if (!string.IsNullOrEmpty(value))
                                LoggerAccessor.LogInfo($"[OHS] : {(levelboard ? "Levelboard" : "Leaderboard")} has extra data: {value}");
                        }

                        string scoreboardfile = directorypath + $"/{(levelboard ? $"Levelboard_Data/levelboard_{key}.json" : $"Leaderboard_Data/scoreboard_{key}.json")}";

                        if (!string.IsNullOrEmpty(user))
                        {
                            if (File.Exists(scoreboardfile))
                            {
                                string tempreader = File.ReadAllText(scoreboardfile);
                                if (tempreader != null)
                                    dataforohs = UpdateScoreboard(tempreader, user, score, scoreboardfile);
                            }
                            else // Apparently update can be used to generate the scoreboard.
                            {
                                string boardDirectoryPath = Path.GetDirectoryName(scoreboardfile);

                                if (!string.IsNullOrEmpty(boardDirectoryPath))
                                {
                                    string JsonSerializedData = JsonConvert.SerializeObject(GenerateSampleScoreboard(10 /* Just because it is most common value */), Formatting.Indented);

                                    Directory.CreateDirectory(boardDirectoryPath);

                                    File.WriteAllText(scoreboardfile, JsonSerializedData);

                                    dataforohs = UpdateScoreboard(JsonSerializedData, user, score, scoreboardfile);
                                }
                                else
                                    dataforohs = null;
                            }
                        }
                        else
                            dataforohs = null;
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

        public static string Leaderboard_UpdatesSameEntry(byte[] PostData, string ContentType, string directorypath, string batchparams, int game)
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

            string value = null;

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
                            value = JaminProcessor.JaminDeFormat(v, false, 0, false);

                            if (!string.IsNullOrEmpty(value))
                                LoggerAccessor.LogInfo($"[OHS] : Leaderboard has extra data: {value}");
                        }

                        if (keys != null)
                        {
                            foreach (string key in keys)
                            {
                                string scoreboardfile = directorypath + $"/Leaderboard_Data/scoreboard_{key}.json";

                                if (File.Exists(scoreboardfile))
                                {
                                    string tempreader = File.ReadAllText(scoreboardfile);

                                    if (tempreader != null && user != null)
                                    {
                                        if (resultBuilder.Length == 0)
                                            resultBuilder.Append(UpdateScoreboard(tempreader, user, score, scoreboardfile));
                                        else
                                            resultBuilder.Append(", " + UpdateScoreboard(tempreader, user, score, scoreboardfile));
                                    }
                                }
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

        public static string UpdateScoreboard(string json, string nameToUpdate, int newScore, string scoreboardfile)
        {
            bool noedits = false;
            int newIndex = -1;
            string scoreboarddata = string.Empty;

            try
            {
                // Step 1: Deserialize JSON string into a C# object
                Scoreboard scoreboard = JsonConvert.DeserializeObject<Scoreboard>(json);

                if (scoreboard != null && scoreboard.Entries != null)
                {
                    for (int i = 0; i < scoreboard.Entries.Count; i++)
                    {
                        ScoreboardEntry entry = scoreboard.Entries[i];

                        if (newScore > entry.Score)
                        {
                            newIndex = i;
                            break;
                        }
                    }

                    // Step 2: Add the new entry at the appropriate position
                    if (newIndex >= 0)
                        scoreboard.Entries[newIndex] = new ScoreboardEntry
                        {
                            Name = nameToUpdate,
                            Score = newScore
                        };
                    else
                        noedits = true;

                    if (!noedits)
                    {
                        // Step 3: Sort the entries based on the new scores
                        scoreboard.Entries.Sort((a, b) => b.Score.CompareTo(a.Score));

                        // Step 4: Adjust the ranks accordingly
                        for (int i = 0; i < scoreboard.Entries.Count; i++)
                        {
                            scoreboard.Entries[i].Rank = i + 1;
                        }

                        // Step 5: Serialize the updated object back to a JSON string
                        string updatedscoreboard = JsonConvert.SerializeObject(scoreboard, Formatting.Indented);

                        if (!string.IsNullOrEmpty(updatedscoreboard))
                            File.WriteAllText(scoreboardfile, updatedscoreboard);
                    }
                }

                if (noedits)
                    scoreboarddata = json;
                else
                    scoreboarddata = File.ReadAllText(scoreboardfile);

                // Step 1: Parse JSON to C# objects
                List<ScoreboardEntry> entries = JsonConvert.DeserializeObject<JObject>(scoreboarddata)?["Entries"]?.ToObject<List<ScoreboardEntry>>();

                if (entries != null)
                {
                    // Step 2: Convert to Lua table structure
                    Dictionary<int, Dictionary<string, object>> luaTable = new Dictionary<int, Dictionary<string, object>>();

                    foreach (ScoreboardEntry entry in entries)
                    {
                        if (!string.IsNullOrEmpty(entry.Name))
                            luaTable.Add(entry.Rank, new Dictionary<string, object>
                                    {
                                        { "[\"user\"]", $"\"{entry.Name}\"" }, // Enclose string in double quotes and put it inside the brackets
                                        { "[\"score\"]", $"{entry.Score}" } // For numbers, no need to enclose in quotes and put it inside the brackets
                                    });
                    }

                    // Step 3: Format the Lua table as a string using regex
                    return FormatScoreBoardLuaTable(luaTable);
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Leaderboard] - UpdateScoreboard failed - {ex}");
            }

            return "{ }";
        }

        public static string GetAllLeaderboards(string scoreboardpath, bool levelboard)
        {
            string returnvalue = string.Empty;

            scoreboardpath += $"/{(levelboard ? "Levelboard_Data/" : "Leaderboard_Data/")}";

            try
            {
                if (Directory.Exists(scoreboardpath))
                {
                    foreach (string scoreboardfile in Directory.GetFiles(scoreboardpath, "*.json"))
                    {
                        // Split the filename by '_'
                        string[] parts = scoreboardfile.Substring(scoreboardfile.LastIndexOf('/') + 1).Split('_');

                        // Check if there are enough parts to get the second one
                        if (parts.Length > 1)
                        {
                            string boardName = RemoveAfterDot(parts[1]);

                            // Find the entry with the highest score
                            ScoreboardEntry highestScoreEntry = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(scoreboardfile))?["Entries"]?
                                .ToObject<List<ScoreboardEntry>>()?.OrderByDescending(e => e.Score).FirstOrDefault();

                            if (highestScoreEntry != null)
                            {
                                if (returnvalue.Length != 0)
                                    returnvalue += $", [\"{boardName}\"] = {{ [\"score\"] = {highestScoreEntry.Score}, [\"user\"] = \"{highestScoreEntry.Name}\" }}";
                                else
                                    returnvalue = $"{{ [\"{boardName}\"] = {{ [\"score\"] = {highestScoreEntry.Score}, [\"user\"] = \"{highestScoreEntry.Name}\" }}";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Leaderboard] - GetAllLeaderboards failed - {ex}");
            }

            if (returnvalue.Length != 0)
                returnvalue += " }";
            else
                returnvalue = "{ }";

            return returnvalue;
        }

        public static string RequestByUsers(string jsontable, string scoreboardpath)
        {
            string returnvalue = "{ [\"entries\"] = { }, [\"user\"] = { [\"score\"] = 0 } }";

            try
            {
                string scoreboardfile = string.Empty;

                if (Directory.Exists(scoreboardpath))
                {
                    if (!string.IsNullOrEmpty(jsontable))
                    {
                        ScoreBoardUsersRequest data = JsonConvert.DeserializeObject<ScoreBoardUsersRequest>(jsontable);

                        if (data != null && data.Users != null)
                        {
                            scoreboardfile = scoreboardpath + $"/Leaderboard_Data/scoreboard_{data.Key}.json";

                            if (File.Exists(scoreboardfile))
                            {
                                List<ScoreboardEntry> entries = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(scoreboardfile))?["Entries"]?.ToObject<List<ScoreboardEntry>>();

                                if (entries != null)
                                {
                                    StringBuilder resultBuilder = new StringBuilder();

                                    foreach (string user in data.Users.Where(user => !string.IsNullOrEmpty(user)))
                                    {
                                        foreach (ScoreboardEntry entry in entries)
                                        {
                                            if (!string.IsNullOrEmpty(entry.Name) && entry.Name.Equals(user))
                                            {
                                                if (entry.Score != 0)
                                                {
                                                    if (resultBuilder.Length == 0)
                                                        resultBuilder.Append($"[\"user\"] = {{ [\"score\"] = {entry.Score.ToString()} }}");
                                                    else
                                                        resultBuilder.Append($", [\"user\"] = {{ [\"score\"] = {entry.Score.ToString()} }}");
                                                }
                                                else
                                                {
                                                    if (resultBuilder.Length == 0)
                                                        resultBuilder.Append($"[\"user\"] = {{ [\"score\"] = 0 }}");
                                                    else
                                                        resultBuilder.Append($", [\"user\"] = {{ [\"score\"] = 0 }}");
                                                }
                                            }
                                        }
                                    }

                                    if (resultBuilder.Length == 0)
                                        resultBuilder.Append($"[\"user\"] = {{ [\"score\"] = 0 }}");

                                    // Step 2: Convert to Lua table structure
                                    Dictionary<int, Dictionary<string, object>> luaTable = new Dictionary<int, Dictionary<string, object>>();

                                    int i = 1;

                                    foreach (ScoreboardEntry entry in entries)
                                    {
                                        if (i >= 1 && !string.IsNullOrEmpty(entry.Name))
                                        {
                                            Dictionary<string, object> rankData = new Dictionary<string, object>
                                                        {
                                                            { "[\"user\"]", $"\"{entry.Name}\"" },
                                                            { "[\"score\"]", $"{entry.Score}" }
                                                        };

                                            luaTable.Add(entry.Rank, rankData);
                                        }
                                    }

                                    // Step 3: Format the Lua table as a string using regex
                                    returnvalue = "{ [\"entries\"] = " + FormatScoreBoardLuaTable(luaTable) + ", " + resultBuilder.ToString() + " }";

                                    resultBuilder = null;
                                }
                            }
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

        public static string RequestByRank(string jsontable, string scoreboardpath)
        {
            // Sometimes requestbyrank was used to create the scoreboard.

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

                    Directory.CreateDirectory(scoreboardpath + "/Leaderboard_Data");

                    string scoreboardfile = scoreboardpath + $"/Leaderboard_Data/scoreboard_{key}.json";

                    // Step 1: Parse JSON to C# objects
                    List<ScoreboardEntry> entries = null;

                    if (!File.Exists(scoreboardfile))
                    {
                        string JsonSerializedData = JsonConvert.SerializeObject(GenerateSampleScoreboard(numEntries), Formatting.Indented);

                        File.WriteAllText(scoreboardfile, JsonSerializedData);

                        entries = JsonConvert.DeserializeObject<JObject>(JsonSerializedData)?["Entries"]?.ToObject<List<ScoreboardEntry>>();
                    }
                    else
                        entries = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(scoreboardfile))?["Entries"]?.ToObject<List<ScoreboardEntry>>();

                    if (entries != null)
                    {
                        int scoreforuser = 0;
                        int i = 1;

                        // Step 2: Convert to Lua table structure
                        Dictionary<int, Dictionary<string, object>> luaTable = new Dictionary<int, Dictionary<string, object>>();

                        foreach (ScoreboardEntry entry in entries)
                        {
                            if (i >= start && !string.IsNullOrEmpty(entry.Name))
                            {
                                Dictionary<string, object> rankData = new Dictionary<string, object>
                                            {
                                                { "[\"user\"]", $"\"{entry.Name}\"" }, // Enclose string in double quotes and put it inside the brackets
                                                { "[\"score\"]", $"{entry.Score}" } // For numbers, no need to enclose in quotes and put it inside the brackets
                                            };

                                luaTable.Add(entry.Rank, rankData);

                                if (entry.Name == user)
                                    scoreforuser = entry.Score;
                            }
                        }

                        // Step 3: Format the Lua table as a string using regex
                        return $"{{ [\"user\"] = {{ [\"score\"] = {scoreforuser} }}, [\"entries\"] = {FormatScoreBoardLuaTable(luaTable)} }}";
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Leaderboard] - RequestByRank failed - {ex}");
            }

            return $"{{ [\"user\"] = {{ [\"score\"] = 0 }}, [\"entries\"] = {{ }} }}";
        }

        private static Scoreboard GenerateSampleScoreboard(int numEntries)
        {
            Scoreboard scoreboard = new Scoreboard();

            scoreboard.Entries = new List<ScoreboardEntry>(numEntries);

            for (int i = 0; i < numEntries; i++)
            {
                scoreboard.Entries.Add(new ScoreboardEntry { Name = string.Empty, Score = 0 });
            }

            // Sort the entries by score in descending order
            scoreboard.Entries.Sort((entry1, entry2) => entry2.Score.CompareTo(entry1.Score));

            // Assign ranks based on the sorted order
            for (int i = 0; i < scoreboard.Entries.Count; i++)
            {
                scoreboard.Entries[i].Rank = i + 1;
            }

            return scoreboard;
        }

        // Helper method to format the Lua table as a string
        private static string FormatScoreBoardLuaTable(Dictionary<int, Dictionary<string, object>> luaTable)
        {
            string luaString = "{ ";
            foreach (var rankData in luaTable)
            {
                luaString += $"[{rankData.Key}] = {{ ";
                foreach (var kvp in rankData.Value)
                {
                    luaString += $"{kvp.Key} = {kvp.Value}, "; // We already formatted the keys and values accordingly
                }
                luaString = RemoveTrailingComma(luaString); // Remove the trailing comma for the last element in each number category
                luaString += " }, ";
            }
            luaString += " }";

            // Remove trailing commas
            return RemoveTrailingComma(luaString);
        }

        // Helper method to remove the trailing comma from the Lua table string
        private static string RemoveTrailingComma(string input)
        {
            return Regex.Replace(input, @",(\s*})|(\s*]\s*})", "$1$2");
        }

        private static string RemoveAfterDot(string input)
        {
            int dotIndex = input.IndexOf('.');
            if (dotIndex != -1)
                return input.Substring(0, dotIndex);
            return input; // Return the original string if there's no dot
        }

        public class Scoreboard
        {
            public List<ScoreboardEntry> Entries { get; set; }
        }

        public class ScoreboardEntry
        {
            public string Name { get; set; }
            public int Score { get; set; }
            public int Rank { get; set; } // Add this property to hold the rank
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
