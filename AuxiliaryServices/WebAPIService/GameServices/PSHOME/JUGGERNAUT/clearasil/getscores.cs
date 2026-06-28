using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.PSHOME.JUGGERNAUT.clearasil
{
    public class getscores
    {
        public static string ProcessGetScores(
            IDictionary<string, string> QueryParameters,
            string apiPath
        )
        {
            if (QueryParameters != null && QueryParameters.TryGetValue("phase", out var value))
            {
                var phase2 = value == "2";
                ClearasilScoreBoardData scoreboard;

                lock (pushscore.Leaderboards)
                {
                    scoreboard = pushscore.Leaderboards[phase2 ? 1 : 0];

                    if (scoreboard == null)
                    {
                        scoreboard = new ClearasilScoreBoardData(
                            LeaderboardDbContext.BuildOptions(
                                0,
                                $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}"
                            ),
                            phase2 ? "phase2" : "phase1"
                        );
                        pushscore.Leaderboards[phase2 ? 1 : 0] = scoreboard;
                    }
                }

                return scoreboard.SerializeToString("xml").Result;
            }

            return "<xml></xml>";
        }
    }
}
