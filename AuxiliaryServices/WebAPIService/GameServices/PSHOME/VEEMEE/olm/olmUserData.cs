using System.Globalization;
using MultiServerLibrary.HTTP;

namespace WebAPIService.GameServices.PSHOME.VEEMEE.olm
{
    public class OLMUserData
    {
        public static string SetUserDataPOST(byte[] PostData, string ContentType, string apiPath)
        {
            if (ContentType == "application/x-www-form-urlencoded" && PostData != null)
            {
                var data = HTTPProcessor.ExtractAndSortUrlEncodedPOSTData(PostData);
                var key = data["key"].First();
                if (key != "KEqZKh3At4Ev")
                {
                    CustomLogger.LoggerAccessor.LogError(
                        "[VEEMEE] - olm - Client tried to push invalid key! Invalidating request."
                    );
                    return null;
                }

                var psnid = data["psnid"].First();
                var score = data["score"].First();
                var throws = data["throws"].First();
                OLMLeaderboard.InitializeLeaderboard();

                _ = OLMLeaderboard.Leaderboard.UpdateScoreAsync(
                    psnid,
                    float.Parse(score, CultureInfo.InvariantCulture),
                    [throws]
                );

                return $"<psnid>{psnid}</psnid><score>{score}</score><throws>{throws}</throws>";
            }

            return null;
        }

        public static string GetUserDataPOST(byte[] PostData, string ContentType, string apiPath)
        {
            var psnid = string.Empty;

            if (ContentType == "application/x-www-form-urlencoded" && PostData != null)
            {
                var data = HTTPProcessor.ExtractAndSortUrlEncodedPOSTData(PostData);
                var key = data["key"].First();
                if (key != "KEqZKh3At4Ev")
                {
                    CustomLogger.LoggerAccessor.LogError(
                        "[VEEMEE] - olm - Client tried to push invalid key! Invalidating request."
                    );
                    return null;
                }
                psnid = data["psnid"].First();

                OLMLeaderboard.InitializeLeaderboard();

                var scoreData = OLMLeaderboard.Leaderboard.GetEntryForUser(psnid);

                if (scoreData != null)
                    return $"<psnid>{psnid}</psnid><score>{scoreData.Score.ToString().Replace(",", ".")}</score><throws>{scoreData.throws}</throws>";
            }

            return $"<psnid>{psnid}</psnid><score>0</score><throws>0</throws>";
        }
    }
}
