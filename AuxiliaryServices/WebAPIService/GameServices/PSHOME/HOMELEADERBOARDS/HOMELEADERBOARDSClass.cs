using System.Globalization;
using CustomLogger;
using HttpMultipartParser;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.PSHOME.HOMELEADERBOARDS
{
    public static class HOMELEADERBOARDSClass
    {
        private static readonly Dictionary<string, HomeScoreBoardData> _leaderboards = [];

        public static string ProcessEntryBare(byte[] postdata, string boundary, string apiPath)
        {
            if (postdata != null && !string.IsNullOrEmpty(boundary))
            {
                try
                {
                    using (var copyStream = new MemoryStream(postdata))
                    {
                        var data = MultipartFormDataParser.Parse(copyStream, boundary);

                        var postType = data.GetParameterValue("postType");
                        var game = data.GetParameterValue("game");

                        switch (postType)
                        {
                            case "getHighScore":
                                if (!string.IsNullOrEmpty(game))
                                {
                                    lock (_leaderboards)
                                    {
                                        if (!_leaderboards.ContainsKey(game))
                                            _leaderboards.Add(
                                                game,
                                                new HomeScoreBoardData(
                                                    LeaderboardDbContext.BuildOptions(
                                                        0,
                                                        $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}"
                                                    ),
                                                    game
                                                )
                                            );

                                        return $"<MsRoot>{_leaderboards[game].SerializeToString("PAGE").Result}</MsRoot>";
                                    }
                                }
                                break;
                            case "postScore":
                                var score = float.Parse(
                                    data.GetParameterValue("score"),
                                    CultureInfo.InvariantCulture
                                );
                                var player = data.GetParameterValue("player");

                                lock (_leaderboards)
                                {
                                    if (!string.IsNullOrEmpty(game))
                                    {
                                        if (!_leaderboards.ContainsKey(game))
                                            _leaderboards.Add(
                                                game,
                                                new HomeScoreBoardData(
                                                    LeaderboardDbContext.BuildOptions(
                                                        0,
                                                        $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}"
                                                    ),
                                                    game
                                                )
                                            );

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
                    LoggerAccessor.LogError(
                        $"[HOMELEADERBOARDSClass] - entryBare request thrown an assertion. (Exception: {ex})"
                    );
                }
            }

            return null;
        }
    }
}
