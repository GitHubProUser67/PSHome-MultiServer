using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WebAPIService.LeaderboardService;
namespace WebAPIService.GameServices.JUGGERNAUT.clearasil
{
    public class getscores
    {
        public static string ProcessGetScores(IDictionary<string, string> QueryParameters, string apiPath)
        {
            if (QueryParameters != null)
            {
                if (pushscore.Leaderboard == null)
                {
                    var retCtx = new LeaderboardDbContext(LeaderboardDbContext.OnContextBuilding(new DbContextOptionsBuilder<LeaderboardDbContext>(), 0, $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}").Options);

                    retCtx.Database.Migrate();

                    pushscore.Leaderboard = new ClearasilScoreBoardData(retCtx);
                }

                if (!string.IsNullOrEmpty(QueryParameters["phase"]))
                    return pushscore.Leaderboard.SerializeToString("xml").Result;
            }

            return "<xml></xml>";
        }
    }
}
