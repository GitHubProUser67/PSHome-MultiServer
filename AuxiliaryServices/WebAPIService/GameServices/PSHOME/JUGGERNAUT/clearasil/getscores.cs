using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WebAPIService.LeaderboardService;
namespace WebAPIService.GameServices.PSHOME.JUGGERNAUT.clearasil
{
    public class getscores
    {
        public static string ProcessGetScores(IDictionary<string, string> QueryParameters, string apiPath)
        {
            if (QueryParameters != null && QueryParameters.ContainsKey("phase"))
            {
                bool phase2 = QueryParameters["phase"] == "2";
                ClearasilScoreBoardData scoreboard;

                lock (pushscore.Leaderboards)
                {
                    scoreboard = pushscore.Leaderboards[phase2 ? 1 : 0];

                    if (scoreboard == null)
                    {
                        scoreboard = new ClearasilScoreBoardData(LeaderboardDbContext.OnContextBuilding(new DbContextOptionsBuilder<LeaderboardDbContext>(), 0, $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}").Options, phase2 ? "phase2" : "phase1");
                        pushscore.Leaderboards[phase2 ? 1 : 0] = scoreboard;
                    }
                }

                return scoreboard.SerializeToString("xml").Result;
            }

            return "<xml></xml>";
        }
    }
}
