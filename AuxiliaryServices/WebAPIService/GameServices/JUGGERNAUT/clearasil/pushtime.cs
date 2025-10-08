using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using WebAPIService.LeaderboardService;
namespace WebAPIService.GameServices.JUGGERNAUT.clearasil
{
    public class pushtime
    {
        public static string ProcessPushTime(IDictionary<string, string> QueryParameters, string apiPath)
        {
            if (QueryParameters != null)
            {
                string user = QueryParameters["user"];
                string time = QueryParameters["time"];

                if (pushscore.Leaderboard == null)
                    pushscore.Leaderboard = new ClearasilScoreBoardData(LeaderboardDbContext.OnContextBuilding(new DbContextOptionsBuilder<LeaderboardDbContext>(), 0, $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}").Options);

                if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(time))
                {
                    _ = pushscore.Leaderboard.AddTimeAsync(user, time);

                    return string.Empty;
                }
            }

            return null;
        }
    }
}
