using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using MultiServerLibrary.HTTP;

namespace WebAPIService.GameServices.VEEMEE.olm
{
    public class OLMLeaderboard
    {
        public static string GetLeaderboardPOST(byte[] PostData, string ContentType, int mode, string apiPath)
        {
            string key = string.Empty;
            string psnid = string.Empty;

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

                DateTime refdate = DateTime.Now; // We avoid race conditions by calculating it one time.

                switch (mode)
                {
                    case 0:
                        if (File.Exists($"{apiPath}/VEEMEE/olm/leaderboard_{refdate:yyyy_MM_dd}.xml"))
                            return File.ReadAllText($"{apiPath}/VEEMEE/olm/leaderboard_{refdate:yyyy_MM_dd}.xml");
                        break;
                    case 1:
                        if (Directory.Exists($"{apiPath}/VEEMEE/olm"))
                        {
                            // Get all XML files in the scoreboard folder
                            foreach (string file in Directory.GetFiles($"{apiPath}/VEEMEE/olm", "leaderboard_*.xml"))
                            {
                                // Extract date from the filename
                                Match match = Regex.Match(file, @"leaderboard_(\d{4}_\d{2}_\d{2}).xml");
                                if (match.Success)
                                {
                                    string fileDate = match.Groups[1].Value;

                                    // Parse the file date
                                    if (DateTime.TryParse(fileDate, out DateTime fileDateTime))
                                    {
                                        // Check if the file is newer than 7 days
                                        double diff = (refdate - fileDateTime).TotalDays;
                                        if (diff <= 7)
                                        {
                                            string weekScoreBoardPath = $"{apiPath}/VEEMEE/olm/leaderboard_{refdate.AddDays(-diff):yyyy_MM_dd}.xml";
                                            if (File.Exists(weekScoreBoardPath))
                                                return File.ReadAllText(weekScoreBoardPath);
                                        }
                                    }
                                }
                            }
                        }
                        break;
                }
            }

            return "<leaderboard></leaderboard>";
        }
    }
}
