using System.Globalization;
using HttpMultipartParser;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.PSHOME.VEEMEE.audi_sled
{
    internal static class SledScoreProcessor
    {
        private static SledScoreBoardData _leaderboard = null;

        public static void InitializeLeaderboard()
        {
            _leaderboard ??= new SledScoreBoardData(
                LeaderboardDbContext.BuildOptions(
                    0,
                    $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}"
                )
            );
        }

        public static string SetUserDataPOST(byte[] PostData, string boundary, string apiPath)
        {
            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                try
                {
                    using (var copyStream = new MemoryStream(PostData))
                    {
                        var data = MultipartFormDataParser.Parse(copyStream, boundary);
                        var key = data.GetParameterValue("key");
                        if (key != "k7dEUsKF3YvrfAxg")
                        {
                            CustomLogger.LoggerAccessor.LogError(
                                "[VEEMEE] - audi_sled - Client tried to push invalid key! Invalidating request."
                            );
                            return null;
                        }
                        var psnid = data.GetParameterValue("psnid");
                        var score = (float)
                            double.Parse(
                                data.GetParameterValue("score"),
                                CultureInfo.InvariantCulture
                            );

                        InitializeLeaderboard();

                        var numOfRaces = _leaderboard.GetNumOfRacesForUser(psnid);

                        _ = _leaderboard.UpdateScoreAsync(psnid, score, [numOfRaces++]);

                        return $"<scores><entry><psnid>{psnid}</psnid><races>{numOfRaces}</races><score>{score.ToString().Replace(",", ".")}</score></entry></scores>";
                    }
                }
                catch (Exception ex)
                {
                    CustomLogger.LoggerAccessor.LogError(
                        $"[SledScoreProcessor] - SetUserDataPOST thrown an assertion. (Exception: {ex})"
                    );
                }
            }

            return null;
        }

        public static string GetUserDataPOST(byte[] PostData, string boundary, string apiPath)
        {
            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                try
                {
                    using (var copyStream = new MemoryStream(PostData))
                    {
                        var data = MultipartFormDataParser.Parse(copyStream, boundary);
                        var key = data.GetParameterValue("key");
                        if (key != "k7dEUsKF3YvrfAxg")
                        {
                            CustomLogger.LoggerAccessor.LogError(
                                "[VEEMEE] - audi_sled - Client tried to push invalid key! Invalidating request."
                            );
                            return null;
                        }
                        var psnid = data.GetParameterValue("psnid");

                        InitializeLeaderboard();

                        return _leaderboard != null
                            ? $"<scores><entry><psnid>{psnid}</psnid><races>{_leaderboard.GetNumOfRacesForUser(psnid)}</races><score>{_leaderboard.GetScoreForUser(psnid).ToString().Replace(",", ".")}</score></entry></scores>"
                            : $"<scores><entry><psnid>{psnid}</psnid><races>0</races><score>0</score></entry></scores>";
                    }
                }
                catch (Exception ex)
                {
                    CustomLogger.LoggerAccessor.LogError(
                        $"[SledScoreProcessor] - GetUserDataPOST thrown an assertion. (Exception: {ex})"
                    );
                }
            }

            return null;
        }

        public static string GetHigherUserScorePOST(
            byte[] PostData,
            string boundary,
            string apiPath
        )
        {
            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                try
                {
                    using (var copyStream = new MemoryStream(PostData))
                    {
                        var data = MultipartFormDataParser.Parse(copyStream, boundary);
                        var key = data.GetParameterValue("key");
                        if (key != "k7dEUsKF3YvrfAxg")
                        {
                            CustomLogger.LoggerAccessor.LogError(
                                "[VEEMEE] - audi_sled - Client tried to push invalid key! Invalidating request."
                            );
                            return null;
                        }
                        var psnid = data.GetParameterValue("psnid");

                        InitializeLeaderboard();

                        if (_leaderboard != null)
                        {
                            var entries = _leaderboard.GetTopScoresAsync(1).Result;

                            if (entries.Count != 0)
                            {
                                var entry = entries.First();
                                return $"<scores><entry><psnid>{psnid}</psnid><races>{entry.numOfRaces}</races><score>{entry.Score.ToString().Replace(",", ".")}</score></entry></scores>";
                            }
                        }

                        return $"<scores><entry><psnid>{psnid}</psnid><races>0</races><score>0</score></entry></scores>";
                    }
                }
                catch (Exception ex)
                {
                    CustomLogger.LoggerAccessor.LogError(
                        $"[SledScoreProcessor] - GetHigherUserScorePOST thrown an assertion. (Exception: {ex})"
                    );
                }
            }

            return null;
        }

        public static string GetGlobalTablePOST(byte[] PostData, string boundary, string apiPath)
        {
            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                try
                {
                    using (var copyStream = new MemoryStream(PostData))
                    {
                        var data = MultipartFormDataParser.Parse(copyStream, boundary);
                        var key = data.GetParameterValue("key");
                        if (key != "k7dEUsKF3YvrfAxg")
                        {
                            CustomLogger.LoggerAccessor.LogError(
                                "[VEEMEE] - audi_sled - Client tried to push invalid key! Invalidating request."
                            );
                            return null;
                        }
                        var psnid = data.GetParameterValue("psnid");
                        var title = data.GetParameterValue("title");

                        InitializeLeaderboard();

                        return _leaderboard?.SerializeToString(title).Result
                            ?? $"<XML><PAGE><TEXT X=\"100\" Y=\"70\" col=\"#FFFFFF\" size=\"4\">{title}</TEXT></PAGE></XML>";
                    }
                }
                catch (Exception ex)
                {
                    CustomLogger.LoggerAccessor.LogError(
                        $"[SledScoreProcessor] - GetHigherUserScorePOST thrown an assertion. (Exception: {ex})"
                    );
                }
            }

            return null;
        }
    }
}
