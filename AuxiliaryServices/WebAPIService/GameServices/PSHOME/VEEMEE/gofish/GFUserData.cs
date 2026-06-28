using System.Globalization;
using MultiServerLibrary.HTTP;

namespace WebAPIService.GameServices.PSHOME.VEEMEE.gofish
{
    public class GFUserData
    {
        public static string SetUserDataPOST(byte[] PostData, string ContentType, string apiPath)
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
                var score = data["score"].First();
                var fishcount = data["fishcount"].First();
                var biggestfishweight = data["biggestfishweight"].First();
                var totalfishweight = data["totalfishweight"].First();
                GFLeaderboard.InitializeLeaderboard();

                _ = GFLeaderboard.Leaderboard.UpdateScoreAsync(
                    psnid,
                    float.Parse(score, CultureInfo.InvariantCulture),
                    [fishcount, biggestfishweight, totalfishweight]
                );

                return $"<psnid>{psnid}</psnid><score>{score}</score><fishcount>{fishcount}</fishcount><psnid>{psnid}</psnid><biggestfishweight>{biggestfishweight}</biggestfishweight><totalfishweight>{totalfishweight}</totalfishweight>";
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
                if (key != "tHeHuYUmuDa54qur")
                {
                    CustomLogger.LoggerAccessor.LogError(
                        "[VEEMEE] - gofish - Client tried to push invalid key! Invalidating request."
                    );
                    return null;
                }
                psnid = data["psnid"].First();

                GFLeaderboard.InitializeLeaderboard();

                var scoreData = GFLeaderboard.Leaderboard.GetEntryForUser(psnid);

                if (scoreData != null)
                    return $"<psnid>{psnid}</psnid><score>{scoreData.Score.ToString().Replace(",", ".")}</score><fishcount>{scoreData.fishcount}</fishcount><psnid>{psnid}</psnid><biggestfishweight>{scoreData.biggestfishweight}</biggestfishweight><totalfishweight>{scoreData.totalfishweight}</totalfishweight>";
            }

            return $"<psnid>{psnid}</psnid><score>0</score><fishcount>0</fishcount><psnid>{psnid}</psnid><biggestfishweight>0</biggestfishweight><totalfishweight>0</totalfishweight>";
        }
    }
}
