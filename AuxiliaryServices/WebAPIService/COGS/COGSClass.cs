using CustomLogger;
using HttpMultipartParser;
using NetworkLibrary.HTTP;
using System;
using System.Globalization;
using System.IO;

namespace WebAPIService.COGS
{
    public class COGSClass
    {
        private static COGSScoreBoardData _leaderboard = new COGSScoreBoardData();

        private string workpath;
        private string method;

        public COGSClass(string method, string workpath)
        {
            this.method = method;
            this.workpath = workpath;
        }

        public string ProcessRequest(byte[] PostData = null, string ContentType = null)
        {
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

                                lock (_leaderboard)
                                {
                                    _leaderboard.UpdateScoreBoard(data.GetParameterValue("Name"), float.Parse(data.GetParameterValue("Points"), CultureInfo.InvariantCulture));
                                    return _leaderboard.UpdateScoreboardXml(workpath);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LoggerAccessor.LogError($"[COGSClass] - leaderboard add request thrown an assertion. (Exception: {ex})");
                        }
                    }
                    break;
                case "GET":
                    return _leaderboard.UpdateScoreboardXml(workpath);
            }

            return null;
        }
    }
}
