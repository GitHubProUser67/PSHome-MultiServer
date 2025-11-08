using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WebAPIService.LeaderboardService;
namespace WebAPIService.GameServices.PSHOME.JUGGERNAUT.clearasil
{
    public class getscores
    {
        public static string ProcessGetScores(IDictionary<string, string> QueryParameters, string apiPath)
        {
            if (QueryParameters != null)
            {
                if (pushscore.Leaderboard == null)
                    pushscore.Leaderboard = new ClearasilScoreBoardData(LeaderboardDbContext.OnContextBuilding(new DbContextOptionsBuilder<LeaderboardDbContext>(), 0, $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}").Options);

                if (!string.IsNullOrEmpty(QueryParameters["phase"]) && QueryParameters["phase"] == "2")
                    return pushscore.Leaderboard.SerializeToString("xml").Result;
            }

            return "<xml></xml>";
        }
    }
}
