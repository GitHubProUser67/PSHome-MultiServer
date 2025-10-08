using Microsoft.EntityFrameworkCore;
using MultiServerLibrary.HTTP;
using System.Collections.Generic;
using System.Linq;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.VEEMEE.goalie_sfrgbt
{
    internal static class GSLeaderboard
    {
        public static Dictionary<string, GSScoreBoardData> Leaderboards = new Dictionary<string, GSScoreBoardData>();

        public static void InitializeLeaderboard(string gameName)
        {
            lock (Leaderboards)
            {
                if (!Leaderboards.ContainsKey(gameName))
                    Leaderboards.Add(gameName, new GSScoreBoardData(LeaderboardDbContext.OnContextBuilding(new DbContextOptionsBuilder<LeaderboardDbContext>(), 0, $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}").Options, gameName));
            }
        }

        public static string GetLeaderboardPOST(byte[] PostData, string ContentType, bool global, string apiPath)
        {
            string key = string.Empty;
            string psnid = string.Empty;
            string type = string.Empty;

            if (ContentType == "application/x-www-form-urlencoded" && PostData != null)
            {
                var data = HTTPProcessor.ExtractAndSortUrlEncodedPOSTData(PostData);
                key = data["key"].First();
                if (key != "d2us7A2EcU2PuBuz")
                {
                    CustomLogger.LoggerAccessor.LogError("[VEEMEE] - goalie_sfrgbt - Client tried to push invalid key! Invalidating request.");
                    return null;
                }
                psnid = data["psnid"].First();
                type = data["type"].First();

                string gameName = "sfrgbt";

                if (global)
                    gameName = "goalie";

                InitializeLeaderboard(gameName);

                switch (type)
                {
                    case "Today":
                        lock (Leaderboards)
                            return Leaderboards[gameName].SerializeToDailyString("leaderboard").Result;
                    case "Yesterday":
                        lock (Leaderboards)
                            return Leaderboards[gameName].SerializeToYesterdayString("leaderboard").Result;
                    case "All Time":
                        lock (Leaderboards)
                            return Leaderboards[gameName].SerializeToString("leaderboard").Result;
                }
            }

            return "<leaderboard></leaderboard>";
        }
    }
}
