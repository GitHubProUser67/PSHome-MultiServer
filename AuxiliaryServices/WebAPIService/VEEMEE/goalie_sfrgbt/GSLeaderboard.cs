using System;
using System.IO;
using System.Linq;
using MultiServerLibrary.HTTP;

namespace WebAPIService.VEEMEE.goalie_sfrgbt
{
    public class GSLeaderboard
    {
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

                string directoryPath = string.Empty;

                if (global)
                    directoryPath = $"{apiPath}/VEEMEE/goalie";
                else
                    directoryPath = $"{apiPath}/VEEMEE/sfrgbt";

                DateTime refdate = DateTime.Now; // We avoid race conditions by calculating it one time.

                switch (type)
                {
                    case "Today":
                        if (File.Exists($"{directoryPath}/leaderboard_{refdate:yyyy_MM_dd}.xml"))
                            return File.ReadAllText($"{directoryPath}/leaderboard_{refdate:yyyy_MM_dd}.xml");
                        break;
                    case "Yesterday":
                        if (File.Exists($"{directoryPath}/leaderboard_{refdate.AddDays(-1):yyyy_MM_dd}.xml"))
                            return File.ReadAllText($"{directoryPath}/leaderboard_{refdate.AddDays(-1):yyyy_MM_dd}.xml");
                        break;
                    case "All Time":
                        if (File.Exists($"{directoryPath}/leaderboard_alltime.xml"))
                            return File.ReadAllText($"{directoryPath}/leaderboard_alltime.xml");
                        break;
                }
            }

            return "<leaderboard></leaderboard>";
        }
    }
}
