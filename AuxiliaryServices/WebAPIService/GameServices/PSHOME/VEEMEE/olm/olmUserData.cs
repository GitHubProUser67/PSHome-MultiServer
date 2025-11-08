using System.Globalization;
using System.Linq;
using MultiServerLibrary.HTTP;

namespace WebAPIService.GameServices.PSHOME.VEEMEE.olm
{
    public  class OLMUserData
    {
        public static string SetUserDataPOST(byte[] PostData, string ContentType, string apiPath)
        {
            string key = string.Empty;
            string psnid = string.Empty;
            string score = string.Empty;
            string throws = string.Empty;

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
                score = data["score"].First();
                throws = data["throws"].First();

                OLMLeaderboard.InitializeLeaderboard();

                _ = OLMLeaderboard.Leaderboard.UpdateScoreAsync(psnid, float.Parse(score, CultureInfo.InvariantCulture), new System.Collections.Generic.List<object> { throws });

                return $"<psnid>{psnid}</psnid><score>{score}</score><throws>{throws}</throws>";
            }

            return null;
        }

        public static string GetUserDataPOST(byte[] PostData, string ContentType, string apiPath)
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

                OLMLeaderboard.InitializeLeaderboard();

                var scoreData = OLMLeaderboard.Leaderboard.GetEntryForUser(psnid);

                if (scoreData != null)
                    return $"<psnid>{psnid}</psnid><score>{scoreData.Score.ToString().Replace(",", ".")}</score><throws>{scoreData.throws}</throws>";
            }

            return $"<psnid>{psnid}</psnid><score>0</score><throws>0</throws>";
        }
    }
}
