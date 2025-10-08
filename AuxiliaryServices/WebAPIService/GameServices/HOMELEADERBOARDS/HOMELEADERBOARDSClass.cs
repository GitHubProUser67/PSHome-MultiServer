using CustomLogger;
using HttpMultipartParser;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.HOMELEADERBOARDS
{
    public static class HOMELEADERBOARDSClass
    {
        private static Dictionary<string, HomeScoreBoardData> _leaderboards = new Dictionary<string, HomeScoreBoardData>();

        public static string ProcessEntryBare(byte[] postdata, string boundary, string apiPath)
        {
            if (postdata != null && !string.IsNullOrEmpty(boundary))
            {
                try
                {
                    using (MemoryStream copyStream = new MemoryStream(postdata))
                    {
                        var data = MultipartFormDataParser.Parse(copyStream, boundary);

                        string postType = data.GetParameterValue("postType");
                        string game = data.GetParameterValue("game");

                        switch (postType)
                        {
                            case "getHighScore":
                                if (!string.IsNullOrEmpty(game))
                                {
                                    lock (_leaderboards)
                                    {
                                        if (!_leaderboards.ContainsKey(game))
                                            _leaderboards.Add(game, new HomeScoreBoardData(LeaderboardDbContext.OnContextBuilding(new DbContextOptionsBuilder<LeaderboardDbContext>(), 0, $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}").Options, game));

                                        return $"<MsRoot>{_leaderboards[game].SerializeToString("PAGE").Result}</MsRoot>";
                                    }
                                }
                                break;
                            case "postScore":
                                float score = float.Parse(data.GetParameterValue("score"), CultureInfo.InvariantCulture);
                                string player = data.GetParameterValue("player");

                                lock (_leaderboards)
                                {
                                    if (!string.IsNullOrEmpty(game))
                                    {
                                        if (!_leaderboards.ContainsKey(game))
                                            _leaderboards.Add(game, new HomeScoreBoardData(LeaderboardDbContext.OnContextBuilding(new DbContextOptionsBuilder<LeaderboardDbContext>(), 0, $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}").Options, game));

                                        _ = _leaderboards[game].UpdateScoreAsync(player, score);
                                        return $"<MsRoot>{_leaderboards[game].SerializeToString("PAGE").Result}</MsRoot>";
                                    }
                                }
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LoggerAccessor.LogError($"[HOMELEADERBOARDSClass] - entryBare request thrown an assertion. (Exception: {ex})");
                }
            }

            return null;
        }
    }
}
