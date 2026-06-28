using MultiServerLibrary.HTTP;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.PSHOME.VEEMEE.goalie_sfrgbt
{
    internal static class GSLeaderboard
    {
        public static Dictionary<string, GSScoreBoardData> Leaderboards = [];

        public static void InitializeLeaderboard(string gameName)
        {
            lock (Leaderboards)
            {
                if (!Leaderboards.ContainsKey(gameName))
                    Leaderboards.Add(
                        gameName,
                        new GSScoreBoardData(
                            LeaderboardDbContext.BuildOptions(
                                0,
                                $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}"
                            ),
                            gameName
                        )
                    );
            }
        }

        public static string GetLeaderboardPOST(
            byte[] PostData,
            string ContentType,
            bool global,
            string apiPath
        )
        {
            if (ContentType == "application/x-www-form-urlencoded" && PostData != null)
            {
                var data = HTTPProcessor.ExtractAndSortUrlEncodedPOSTData(PostData);
                var key = data["key"].First();
                if (key != "d2us7A2EcU2PuBuz")
                {
                    CustomLogger.LoggerAccessor.LogError(
                        "[VEEMEE] - goalie_sfrgbt - Client tried to push invalid key! Invalidating request."
                    );
                    return null;
                }

                var psnid = data["psnid"].First();
                var type = data["type"].First();
                var gameName = "sfrgbt";

                if (global)
                    gameName = "goalie";

                InitializeLeaderboard(gameName);

                switch (type)
                {
                    case "Today":
                        lock (Leaderboards)
                            return Leaderboards[gameName]
                                .SerializeToDailyString("leaderboard")
                                .Result;
                    case "Yesterday":
                        lock (Leaderboards)
                            return Leaderboards[gameName]
                                .SerializeToYesterdayString("leaderboard")
                                .Result;
                    case "All Time":
                        lock (Leaderboards)
                            return Leaderboards[gameName].SerializeToString("leaderboard").Result;
                }
            }

            return "<leaderboard></leaderboard>";
        }
    }
}
