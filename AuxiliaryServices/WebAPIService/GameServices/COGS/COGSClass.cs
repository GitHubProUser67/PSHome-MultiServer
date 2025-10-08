using CustomLogger;
using HttpMultipartParser;
using Microsoft.EntityFrameworkCore;
using MultiServerLibrary.HTTP;
using System;
using System.Globalization;
using System.IO;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.COGS
{
    public class COGSClass
    {
        private static COGSScoreBoardData _leaderboard = null;

        private string workpath;
        private string method;

        public COGSClass(string method, string workpath)
        {
            this.method = method;
            this.workpath = workpath;
        }

        public string ProcessRequest(byte[] PostData = null, string ContentType = null)
        {
            if (_leaderboard == null)
                _leaderboard = new COGSScoreBoardData(LeaderboardDbContext.OnContextBuilding(new DbContextOptionsBuilder<LeaderboardDbContext>(), 0, $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}").Options);

            switch (method)
            {
                case "POST":
                    string boundary = HTTPProcessor.ExtractBoundary(ContentType);

                    if (PostData != null && !string.IsNullOrEmpty(boundary))
                    {
                        try
                        {
                            using (MemoryStream copyStream = new MemoryStream(PostData))
                            {
                                var data = MultipartFormDataParser.Parse(copyStream, boundary);

                                _ = _leaderboard.UpdateScoreAsync(data.GetParameterValue("Name"), float.Parse(data.GetParameterValue("Points"), CultureInfo.InvariantCulture));
                                return _leaderboard.SerializeToString("xml").Result;
                            }
                        }
                        catch (Exception ex)
                        {
                            LoggerAccessor.LogError($"[COGSClass] - leaderboard add request thrown an assertion. (Exception: {ex})");
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
