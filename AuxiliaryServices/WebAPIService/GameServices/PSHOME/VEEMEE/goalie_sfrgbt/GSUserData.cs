using System.Globalization;
using MultiServerLibrary.HTTP;

namespace WebAPIService.GameServices.PSHOME.VEEMEE.goalie_sfrgbt
{
    public class UserData
    {
        public static string SetUserDataPOST(
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
                var guest = data["guest"].First();
                var goals = data["goals"].First();
                var duration = data["duration"].First();
                var gameName = "sfrgbt";

                if (global)
                    gameName = "goalie";

                GSLeaderboard.InitializeLeaderboard(gameName);

                lock (GSLeaderboard.Leaderboards)
                    _ = GSLeaderboard
                        .Leaderboards[gameName]
                        .UpdateScoreAsync(
                            psnid,
                            float.Parse(goals, CultureInfo.InvariantCulture),
                            [duration, guest]
                        );

                return $"<scores><entry><psnid>{psnid}</psnid><goals>{goals}</goals><duration>{duration}</duration><paid_goals></paid_goals></entry></scores>";
            }

            return null;
        }

        public static string GetUserDataPOST(
            byte[] PostData,
            string ContentType,
            bool global,
            string apiPath
        )
        {
            var psnid = string.Empty;

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
                psnid = data["psnid"].First();

                var gameName = "sfrgbt";

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
