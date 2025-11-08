using Microsoft.EntityFrameworkCore;
using MultiServerLibrary.HTTP;
using System.Linq;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.PSHOME.VEEMEE.gofish
{
    internal static class GFLeaderboard
    {
        public static GFScoreBoardData Leaderboard = null;

        public static void InitializeLeaderboard()
        {
            if (Leaderboard == null)
                Leaderboard = new GFScoreBoardData(LeaderboardDbContext.OnContextBuilding(new DbContextOptionsBuilder<LeaderboardDbContext>(), 0, $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}").Options);
        }

        public static string GetLeaderboardPOST(byte[] PostData, string ContentType, int mode, string apiPath)
        {
            string key = string.Empty;
            string psnid = string.Empty;

            if (ContentType == "application/x-www-form-urlencoded" && PostData != null)
            {
                var data = HTTPProcessor.ExtractAndSortUrlEncodedPOSTData(PostData);
                key = data["key"].First();
                if (key != "tHeHuYUmuDa54qur")
                {
                    CustomLogger.LoggerAccessor.LogError("[VEEMEE] - gofish - Client tried to push invalid key! Invalidating request.");
                    return null;
                }
                psnid = data["psnid"].First();

                InitializeLeaderboard();

                switch (mode)
                {
                    case 0:
                        return Leaderboard.SerializeToDailyString("leaderboard").Result;
                    case 1:
                        return Leaderboard.SerializeToYesterdayString("leaderboard").Result;
                    case 2:
                        return Leaderboard.SerializeToString("leaderboard").Result;
                }
            }

            return "<leaderboard></leaderboard>";
        }
    }
}
