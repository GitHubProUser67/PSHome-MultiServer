using System.Globalization;
using CustomLogger;
using HttpMultipartParser;
using MultiServerLibrary.HTTP;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.PSHOME.COGS
{
    public class COGSClass(string method, string workpath)
    {
        private static COGSScoreBoardData _leaderboard = null;

        private readonly string workpath = workpath;
        private readonly string method = method;

        public string ProcessRequest(byte[] PostData = null, string ContentType = null)
        {
            _leaderboard ??= new COGSScoreBoardData(
                LeaderboardDbContext.BuildOptions(
                    0,
                    $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}"
                )
            );

            switch (method)
            {
                case "POST":
                    var boundary = HTTPProcessor.ExtractBoundary(ContentType);

                    if (PostData != null && !string.IsNullOrEmpty(boundary))
                    {
                        try
                        {
                            using (var copyStream = new MemoryStream(PostData))
                            {
                                var data = MultipartFormDataParser.Parse(copyStream, boundary);

                                _ = _leaderboard.UpdateScoreAsync(
                                    data.GetParameterValue("Name"),
                                    float.Parse(
                                        data.GetParameterValue("Points"),
                                        CultureInfo.InvariantCulture
                                    )
                                );
                                return _leaderboard.SerializeToString("xml").Result;
                            }
                        }
                        catch (Exception ex)
                        {
                            LoggerAccessor.LogError(
                                $"[COGSClass] - leaderboard add request thrown an assertion. (Exception: {ex})"
                            );
                        }
                    }
                    break;
                case "GET":
                    return _leaderboard.SerializeToString("xml").Result;
            }

            return null;
        }
    }
}
