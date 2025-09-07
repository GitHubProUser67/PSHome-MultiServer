using CustomLogger;
using HttpMultipartParser;
using Microsoft.EntityFrameworkCore;
using MultiServerLibrary.Extension;
using MultiServerLibrary.HTTP;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using WebAPIService.GameServices.HEAVYWATER.Entities;
using WebAPIService.GameServices.SSFW;
using WebAPIService.LeaderboardService;
using XI5;

namespace WebAPIService.GameServices.HEAVYWATER
{
    public class HeavyWaterClass
    {
        private static ScoreboardService<HexxScoreboardEntry> _hexx_leaderboard = null;

        private string absolutepath;
        private string method;
        private string apipath;

        public HeavyWaterClass(string method, string absolutepath, string apipath)
        {
            this.absolutepath = absolutepath;
            this.method = method;
            this.apipath = apipath;
        }

        public string ProcessRequest(IDictionary<string, string> QueryParameters, byte[] PostData, string ContentType)
        {
            const string playerPathern = @"/player/([^/]+)";

            try
            {
                Match match = Regex.Match(absolutepath, playerPathern);

                switch (method)
                {
                    case "GET":
                        if (match.Success)
                        {
                            string id = match.Groups[1].Value;

                            if (absolutepath.Contains("/D2O/Avalon/"))
                            {
                                string avalonProfilePath = apipath + $"/HEAVYWATER/Avalon_keep/{id}";

                                if (absolutepath.EndsWith("/data/HouseData"))
                                {
                                    string house = "{\"House\":{\"AVALON\":true}}";

                                    if (File.Exists(avalonProfilePath + "/House.json"))
                                        house = File.ReadAllText(avalonProfilePath + "/House.json");

                                    return $@"{{
                                    ""STATUS"": ""SUCCESS"",
                                    ""result"":
                                      {house}
                                  }}";
                                }
                                else if (absolutepath.EndsWith("/data/MyAvalonKeepData"))
                                {
                                    string AvalonKeepData = "{}";

                                    if (File.Exists(avalonProfilePath + "/AvalonKeepData.json"))
                                        AvalonKeepData = File.ReadAllText(avalonProfilePath + "/AvalonKeepData.json");

                                    return $@"{{
                                    ""STATUS"": ""SUCCESS"",
                                    ""result"":
                                      {AvalonKeepData}
                                  }}";
                                }
                            }
                            else if (absolutepath.Contains("/D2O/AvalonHexx/"))
                            {
                                string hexxProfilePath = apipath + $"/HEAVYWATER/Avalon_hexx/{id}";

                                if (absolutepath.EndsWith("/data/MyAvalonHexxData"))
                                {
                                    string AvalonHexxData = "{}";

                                    if (File.Exists(hexxProfilePath + "/HexxData.json"))
                                        AvalonHexxData = File.ReadAllText(hexxProfilePath + "/HexxData.json");

                                    return $@"{{
                                        ""STATUS"": ""SUCCESS"",
                                        ""result"":
                                          {AvalonHexxData}
                                      }}";
                                }
                            }
                            else if (absolutepath.Contains("/D2O/EmoRay/"))
                            {
                                string emorayProfilePath = apipath + $"/HEAVYWATER/EmoRay/{id}";

                                if (absolutepath.EndsWith("/data/ProgressionData"))
                                {
                                    string emorayProgData = "{}";

                                    if (File.Exists(emorayProfilePath + "/ProgressionData.json"))
                                        emorayProgData = File.ReadAllText(emorayProfilePath + "/ProgressionData.json");

                                    return $@"{{
                                        ""STATUS"": ""SUCCESS"",
                                        ""result"":
                                          {emorayProgData}
                                      }}";
                                }
                                else if (absolutepath.EndsWith("/data/EquippedData"))
                                {
                                    string emorayEquippedData = "{}";

                                    if (File.Exists(emorayProfilePath + "/EquippedData.json"))
                                        emorayEquippedData = File.ReadAllText(emorayProfilePath + "/EquippedData.json");

                                    return $@"{{
                                        ""STATUS"": ""SUCCESS"",
                                        ""result"":
                                          {emorayEquippedData}
                                      }}";
                                }
                                else if (absolutepath.EndsWith("/data/ScoresData"))
                                {
                                    string emorayScoresData = "{}";

                                    if (File.Exists(emorayProfilePath + "/ScoresData.json"))
                                        emorayScoresData = File.ReadAllText(emorayProfilePath + "/ScoresData.json");

                                    return $@"{{
                                        ""STATUS"": ""SUCCESS"",
                                        ""result"":
                                          {emorayScoresData}
                                      }}";
                                }
                                else if (absolutepath.EndsWith("/data/ControllerData"))
                                {
                                    string emorayControllerData = "{}";

                                    if (File.Exists(emorayProfilePath + "/ControllerData.json"))
                                        emorayControllerData = File.ReadAllText(emorayProfilePath + "/ControllerData.json");

                                    return $@"{{
                                        ""STATUS"": ""SUCCESS"",
                                        ""result"":
                                          {emorayControllerData}
                                      }}";
                                }
                                else if (absolutepath.EndsWith("/data/StoreProgressData"))
                                {
                                    string emorayStoreProgressData = "{}";

                                    if (File.Exists(emorayProfilePath + "/StoreProgressData.json"))
                                        emorayStoreProgressData = File.ReadAllText(emorayProfilePath + "/StoreProgressData.json");

                                    return $@"{{
                                        ""STATUS"": ""SUCCESS"",
                                        ""result"":
                                          {emorayStoreProgressData}
                                      }}";
                                }
                            }
                            else if (absolutepath.Contains("/D2O/D2OUniverse/"))
                            {
                                string ProfilePath = apipath + $"/HEAVYWATER/D2OUniverse/{id}";

                                if (absolutepath.EndsWith("/data/D2OData"))
                                {
                                    string D2OData = "{}";

                                    if (File.Exists(ProfilePath + "/D2OData.json"))
                                        D2OData = File.ReadAllText(ProfilePath + "/D2OData.json");

                                    return $@"{{
                                    ""STATUS"": ""SUCCESS"",
                                    ""result"":
                                      {D2OData}
                                  }}";
                                }
                            }
                        }
                        else if (absolutepath.Contains("/D2O/Avalon/"))
                        {
                            if (absolutepath.EndsWith("/contributions"))
                            {
                                string ContribData = "{\"Contribution\":{}}";
                                string ContribPath = apipath + $"/HEAVYWATER/Avalon_keep/contributions.json";

                                if (File.Exists(ContribPath))
                                    ContribData = File.ReadAllText(ContribPath);

                                return $@"{{
                                    ""STATUS"": ""SUCCESS"",
                                    ""result"":
                                      {ContribData}
                                  }}";
                            }
                        }
                        else if (absolutepath.Contains("/D2O/AvalonHexx/"))
                        {
                            const string d2oidPathern = @"/d2oid/([^/]+)";

                            match = Regex.Match(absolutepath, d2oidPathern);

                            if (match.Success)
                                return $@"{{
                                    ""STATUS"": ""SUCCESS"",
                                    ""result"":
                                      ""{GenerateD2OGuid(match.Groups[1].Value)}""
                                  }}";
                            else if (absolutepath.EndsWith("Scores/") && QueryParameters.ContainsKey("limit") && int.TryParse(QueryParameters["limit"], out int limit))
                            {
                                int i = 1;
                                StringBuilder scoreboardData = new StringBuilder("{\"Scores\":{");

                                var scoreData = _hexx_leaderboard.GetTopScoresAsync(limit).Result;
                                int scoreDataCount = scoreData.Count();

                                foreach (var scoreKeyPair in scoreData)
                                {
                                    if (i == scoreDataCount)
                                        scoreboardData.Append($"\"{scoreKeyPair.PsnId}\":{(int)scoreKeyPair.Score}");
                                    else
                                        scoreboardData.Append($"\"{scoreKeyPair.PsnId}\":{(int)scoreKeyPair.Score},");

                                    i++;
                                }

                                return $@"{{
                                    ""STATUS"": ""SUCCESS"",
                                    ""result"":
                                      {scoreboardData}}}}}
                                  }}";
                            }
                        }
                        else if (absolutepath.Contains("/D2O/EmoRay/"))
                        {
                            if (absolutepath.EndsWith("Scores/") && QueryParameters.ContainsKey("limit") && QueryParameters.ContainsKey("range")
                                && !string.IsNullOrEmpty(QueryParameters["range"]) && int.TryParse(QueryParameters["limit"], out int limit))
                            {
                                // TODO, figure out leaderboards.

                                return $@"{{
                                    ""STATUS"": ""SUCCESS"",
                                    ""result"": {{ }}
                                  }}";
                            }
                        }
                        break;
                    case "POST":
                        if (absolutepath.Contains("/D2O/Ticket/") && (absolutepath.Contains("validate/") || absolutepath.Contains("verify/")))
                        {
                            using (MemoryStream copyStream = new MemoryStream(PostData))
                            {
                                foreach (FilePart file in MultipartFormDataParser.Parse(copyStream, HTTPProcessor.ExtractBoundary(ContentType))
                                    .Files.Where(x => x.FileName == "ticket.bin" && x.Name == "base64-ticket"))
                                {
                                    using (Stream filedata = file.Data)
                                    {
                                        filedata.Position = 0;

                                        // Find the number of bytes in the stream
                                        int contentLength = (int)filedata.Length;

                                        // Create a byte array
                                        byte[] ticketData = new byte[contentLength];

                                        // Read the contents of the memory stream into the byte array
                                        filedata.Read(ticketData, 0, contentLength);

                                        (bool, byte[]) isValidBase64Data = Encoding.ASCII.GetString(ticketData).IsBase64();

                                        if (isValidBase64Data.Item1)
                                        {
                                            const string RPCNSigner = "RPCN";

                                            // get ticket
                                            XI5Ticket ticket = XI5Ticket.ReadFromBytes(isValidBase64Data.Item2);

                                            // setup username
                                            string username = ticket.Username;

                                            // invalid ticket
                                            if (!ticket.Valid)
                                            {
                                                // log to console
                                                LoggerAccessor.LogWarn($"[HeavyWaterClass] : User {username} tried to alter their ticket data");

                                                return null;
                                            }

                                            // RPCN
                                            if (ticket.SignatureIdentifier == RPCNSigner)
                                                LoggerAccessor.LogInfo($"[HeavyWaterClass] : User {username} connected at: {DateTime.Now} and is on RPCN");
                                            else if (username.EndsWith($"@{RPCNSigner}"))
                                            {
                                                LoggerAccessor.LogError($"[HeavyWaterClass] : User {username} was caught using a RPCN suffix while not on it!");

                                                return null;
                                            }
                                            else
                                                LoggerAccessor.LogInfo($"[HeavyWaterClass] : User {username} connected at: {DateTime.Now} and is on PSN");

                                            return $@"{{
                                                    ""STATUS"": ""SUCCESS"",
                                                    ""result"": {{
                                                      ""d2oID"": ""{GenerateD2OGuid(username)}""
                                                    }}
                                                  }}";
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case "PUT":
#if NET6_0_OR_GREATER
                        if (PostData.Length > 0 && PostData.Length <= Array.MaxLength)
#else
                        if (PostData.Length > 0 && PostData.Length <= 0x7FFFFFC7)
#endif
                        {
                            if (match.Success)
                            {
                                string id = match.Groups[1].Value;

                                if (absolutepath.Contains("/D2O/Avalon/"))
                                {
                                    string avalonProfilePath = apipath + $"/HEAVYWATER/Avalon_keep/{id}";

                                    Directory.CreateDirectory(avalonProfilePath);

                                    if (absolutepath.EndsWith("/data/HouseData"))
                                    {
                                        string house = Encoding.UTF8.GetString(PostData);
                                        File.WriteAllText(avalonProfilePath + "/House.json", house);

                                        return $@"{{
                                            ""STATUS"": ""SUCCESS"",
                                            ""result"":
                                              {house}
                                          }}";
                                    }
                                    else if (absolutepath.EndsWith("/data/MyAvalonKeepData"))
                                    {
                                        string AvalonKeepData = Encoding.UTF8.GetString(PostData);
                                        File.WriteAllText(avalonProfilePath + "/AvalonKeepData.json", AvalonKeepData);

                                        return $@"{{
                                            ""STATUS"": ""SUCCESS"",
                                            ""result"":
                                              {AvalonKeepData}
                                          }}";
                                    }
                                }
                                else if (absolutepath.Contains("/D2O/AvalonHexx/"))
                                {
                                    string hexxProfilePath = apipath + $"/HEAVYWATER/Avalon_hexx/{id}";

                                    Directory.CreateDirectory(hexxProfilePath);

                                    if (absolutepath.EndsWith("/data/MyAvalonHexxData"))
                                    {
                                        string AvalonHexxData = Encoding.UTF8.GetString(PostData);
                                        File.WriteAllText(hexxProfilePath + "/AvalonHexxData.json", AvalonHexxData);

                                        return $@"{{
                                            ""STATUS"": ""SUCCESS"",
                                            ""result"":
                                              {AvalonHexxData}
                                          }}";
                                    }
                                }
                                else if (absolutepath.Contains("/D2O/EmoRay/"))
                                {
                                    string emorayProfilePath = apipath + $"/HEAVYWATER/EmoRay/{id}";

                                    Directory.CreateDirectory(emorayProfilePath);

                                    if (absolutepath.EndsWith("/data/ProgressionData"))
                                    {
                                        string emorayProgData = Encoding.UTF8.GetString(PostData);
                                        File.WriteAllText(emorayProfilePath + "/ProgressionData.json", emorayProgData);

                                        return $@"{{
                                            ""STATUS"": ""SUCCESS"",
                                            ""result"":
                                              {emorayProgData}
                                          }}";
                                    }
                                    else if (absolutepath.EndsWith("/data/EquippedData"))
                                    {
                                        string emorayEquippedData = Encoding.UTF8.GetString(PostData);
                                        File.WriteAllText(emorayProfilePath + "/EquippedData.json", emorayEquippedData);

                                        return $@"{{
                                            ""STATUS"": ""SUCCESS"",
                                            ""result"":
                                              {emorayEquippedData}
                                          }}";
                                    }
                                    else if (absolutepath.EndsWith("/data/ScoresData"))
                                    {
                                        string emorayScoresData = Encoding.UTF8.GetString(PostData);
                                        File.WriteAllText(emorayProfilePath + "/ScoresData.json", emorayScoresData);

                                        return $@"{{
                                            ""STATUS"": ""SUCCESS"",
                                            ""result"":
                                              {emorayScoresData}
                                          }}";
                                    }
                                    else if (absolutepath.EndsWith("/data/ControllerData"))
                                    {
                                        string emorayControllerData = Encoding.UTF8.GetString(PostData);
                                        File.WriteAllText(emorayProfilePath + "/ControllerData.json", emorayControllerData);

                                        return $@"{{
                                            ""STATUS"": ""SUCCESS"",
                                            ""result"":
                                              {emorayControllerData}
                                          }}";
                                    }
                                    else if (absolutepath.EndsWith("/data/StoreProgressData"))
                                    {
                                        string emorayStoreProgressData = Encoding.UTF8.GetString(PostData);
                                        File.WriteAllText(emorayProfilePath + "/StoreProgressData.json", emorayStoreProgressData);

                                        return $@"{{
                                            ""STATUS"": ""SUCCESS"",
                                            ""result"":
                                              {emorayStoreProgressData}
                                          }}";
                                    }
                                }
                                else if (absolutepath.Contains("/D2O/D2OUniverse/"))
                                {
                                    string ProfilePath = apipath + $"/HEAVYWATER/D2OUniverse/{id}";

                                    Directory.CreateDirectory(ProfilePath);

                                    if (absolutepath.EndsWith("/data/D2OData"))
                                    {
                                        string D2OData = Encoding.UTF8.GetString(PostData);
                                        File.WriteAllText(ProfilePath + "/D2OData.json", D2OData);

                                        return $@"{{
                                            ""STATUS"": ""SUCCESS"",
                                            ""result"": 
                                              {D2OData}
                                          }}";
                                    }
                                }
                            }
                            else if (absolutepath.Contains("/D2O/Avalon/"))
                            {
                                if (absolutepath.EndsWith("/metrics"))
                                {
                                    // TODO: process metrics data?
                                    return @"{
                                        ""STATUS"": ""SUCCESS"",
                                      }";
                                }
                                else if (absolutepath.EndsWith("/contributions"))
                                {
                                    Directory.CreateDirectory(apipath + $"/HEAVYWATER/Avalon_keep");

                                    string ContribPath = apipath + $"/HEAVYWATER/Avalon_keep/contributions.json";

                                    using (JsonDocument requestDoc = JsonDocument.Parse(Encoding.UTF8.GetString(PostData)))
                                    {
                                        JsonElement contribution = requestDoc.RootElement.GetProperty("Contribution");
                                        string house = contribution.GetProperty("House").GetString();
                                        int amount = contribution.GetProperty("Amount").GetInt32();

                                        if (!File.Exists(ContribPath))
                                            File.WriteAllText(ContribPath, $"{{\"Contribution\":{{\"{house}\":{amount}}}");
                                        else
                                        {
                                            JObject existingData = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(ContribPath));

                                            JObject publicContribution = existingData["Contribution"] as JObject;
                                            if (publicContribution != null)
                                            {
                                                if (publicContribution[house] != null)
                                                    amount += publicContribution[house].ToObject<int>();

                                                publicContribution[house] = amount;

                                                File.WriteAllText(ContribPath, existingData.ToString());
                                            }
                                        }

                                        return "{\"STATUS\": \"SUCCESS\"}";
                                    }
                                }
                            }
                            else if (absolutepath.Contains("/D2O/AvalonPublicHexx/"))
                            {
                                if (absolutepath.EndsWith("/metrics"))
                                {
                                    // TODO: process metrics data?
                                    return @"{
                                        ""STATUS"": ""SUCCESS"",
                                      }";
                                }
                            }
                            else if (absolutepath.Contains("/D2O/AvalonHexx/"))
                            {
                                if (absolutepath.Contains("Scores/"))
                                {
                                    string hexxDataPath = apipath + $"/HEAVYWATER/Avalon_hexx";
                                    string[] parts = absolutepath.Split('/');

                                    Directory.CreateDirectory(hexxDataPath);

                                    if (_hexx_leaderboard == null)
                                    {
                                        var retCtx = new LeaderboardDbContext(LeaderboardDbContext.OnContextBuilding(new DbContextOptionsBuilder<LeaderboardDbContext>(), 0, $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}").Options);

                                        retCtx.Database.Migrate();

                                        _hexx_leaderboard = new ScoreboardService<HexxScoreboardEntry>(retCtx);
                                    }

                                    _ = _hexx_leaderboard.UpdateScoreAsync(parts[parts.Length - 2], int.Parse(parts[parts.Length - 1]));

                                    return @"{
                                        ""STATUS"": ""SUCCESS"",
                                      }";
                                }
                            }
                            else if (absolutepath.Contains("/D2O/HeavyWaterPublic/"))
                            {
                                if (absolutepath.EndsWith("/metrics"))
                                {
                                    // TODO: process metrics data?
                                    return @"{
                                        ""STATUS"": ""SUCCESS"",
                                      }";
                                }
                            }
                            else if (absolutepath.Contains("/D2O/EmoRay/"))
                            {
                                if (absolutepath.EndsWith("/metrics"))
                                {
                                    // TODO: process metrics data?
                                    return @"{
                                        ""STATUS"": ""SUCCESS"",
                                      }";
                                }
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[HeavyWaterClass] - ProcessRequest thrown an assertion. (Exception: {ex})");
            }

            return null;
        }

        private static string GenerateD2OGuid(string input)
        {
            return GuidGenerator.SSFWGenerateGuid(input, "1amAH3vyFan?!0yY3ahhhhhhhh!!!!!");
        }
    }
}
