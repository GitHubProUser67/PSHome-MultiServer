using Microsoft.EntityFrameworkCore;
using MultiServerLibrary.HTTP;
using System.Linq;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.VEEMEE.olm
{
    internal static class OLMLeaderboard
    {
        public static OLMScoreBoardData Leaderboard = null;

        public static void InitializeLeaderboard()
        {
            if (Leaderboard == null)
            {
                var retCtx = new LeaderboardDbContext(LeaderboardDbContext.OnContextBuilding(new DbContextOptionsBuilder<LeaderboardDbContext>(), 0, $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}").Options);

                retCtx.Database.Migrate();

                Leaderboard = new OLMScoreBoardData(retCtx);
            }
        }

        public static string GetLeaderboardPOST(byte[] PostData, string ContentType, int mode, string apiPath)
        {
            string key;
            string psnid;

            if (ContentType == "application/x-www-form-urlencoded" && PostData != null)
            {
                var data = HTTPProcessor.ExtractAndSortUrlEncodedPOSTData(PostData);
                key = data["key"].First();
                if (key != "KEqZKh3At4Ev")
                {
                    CustomLogger.LoggerAccessor.LogError("[VEEMEE] - olm - Client tried to push invalid key! Invalidating request.");
                    return null;
                }
                psnid = data["psnid"].First();

                InitializeLeaderboard();

                switch (mode)
                {
                    case 0:
                        return Leaderboard.SerializeToDailyString("leaderboard").Result;
                    case 1:
                        return Leaderboard.SerializeToWeeklyString("leaderboard").Result;
                    default:
                        CustomLogger.LoggerAccessor.LogWarn($"[OLMLeaderboard] - Unknown mode:{mode} requested, sending empty data...");
                        break;
                }
            }

            return "<leaderboard></leaderboard>";
        }
    }
}
