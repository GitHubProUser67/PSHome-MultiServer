using MultiServerLibrary.HTTP;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.PSHOME.VEEMEE.gofish
{
    internal static class GFLeaderboard
    {
        public static GFScoreBoardData Leaderboard = null;

        public static void InitializeLeaderboard()
        {
            Leaderboard ??= new GFScoreBoardData(
                LeaderboardDbContext.BuildOptions(
                    0,
                    $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}"
                )
            );
        }

        public static string GetLeaderboardPOST(
            byte[] PostData,
            string ContentType,
            int mode,
            string apiPath
        )
        {
            if (ContentType == "application/x-www-form-urlencoded" && PostData != null)
            {
                var data = HTTPProcessor.ExtractAndSortUrlEncodedPOSTData(PostData);
                var key = data["key"].First();
                if (key != "tHeHuYUmuDa54qur")
                {
                    CustomLogger.LoggerAccessor.LogError(
                        "[VEEMEE] - gofish - Client tried to push invalid key! Invalidating request."
                    );
                    return null;
                }

                var psnid = data["psnid"].First();
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
