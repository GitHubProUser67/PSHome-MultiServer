using System.Globalization;
using System.Linq;
using MultiServerLibrary.HTTP;

namespace WebAPIService.GameServices.PSHOME.VEEMEE.goalie_sfrgbt
{
    public class UserData
    {
        public static string SetUserDataPOST(byte[] PostData, string ContentType, bool global, string apiPath)
        {
            string key = string.Empty;
            string psnid = string.Empty;
            string guest = string.Empty;
            string goals = string.Empty;
            string duration = string.Empty;

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
                guest = data["guest"].First();
                goals = data["goals"].First();
                duration = data["duration"].First();

                string gameName = "sfrgbt";

                if (global)
                    gameName = "goalie";

                GSLeaderboard.InitializeLeaderboard(gameName);

                lock (GSLeaderboard.Leaderboards)
                    _ = GSLeaderboard.Leaderboards[gameName].UpdateScoreAsync(psnid, float.Parse(goals, CultureInfo.InvariantCulture), new System.Collections.Generic.List<object> { duration, guest });

                return $"<scores><entry><psnid>{psnid}</psnid><goals>{goals}</goals><duration>{duration}</duration><paid_goals></paid_goals></entry></scores>";
            }

            return null;
        }

        public static string GetUserDataPOST(byte[] PostData, string ContentType, bool global, string apiPath)
        {
            string key = string.Empty;
            string psnid = string.Empty;

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

                string gameName = "sfrgbt";

                if (global)
                    gameName = "goalie";

                GSLeaderboard.InitializeLeaderboard(gameName);

                lock (GSLeaderboard.Leaderboards)
                {
                    var scoreData = GSLeaderboard.Leaderboards[gameName].GetEntryForUser(psnid);

                    if (scoreData != null)
                        return $"<scores><entry><psnid>{psnid}</psnid><goals>{scoreData.Score.ToString().Replace(",", ".")}</goals><duration>{scoreData.duration}</duration><paid_goals></paid_goals></entry></scores>";
                }
            }

            return $"<scores><entry><psnid>{psnid}</psnid><goals>0</goals><duration>0</duration><paid_goals></paid_goals></entry></scores>";
        }
    }
}
