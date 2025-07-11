using CustomLogger;
using HttpMultipartParser;
using NetHasher;
using MultiServerLibrary.HTTP;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace WebAPIService.RCHOME
{
    public class RCHOMEClass
    {
        private static Dictionary<string, FiringRangeScoreBoardData> _leaderboards = new Dictionary<string, FiringRangeScoreBoardData>();

        private string absolutepath;
        private string workpath;
        private string method;

        public RCHOMEClass(string method, string absolutepath, string workpath)
        {
            this.absolutepath = absolutepath;
            this.method = method;
            this.workpath = workpath;
        }

        public string ProcessRequest(byte[] PostData = null, string ContentType = null)
        {
            if (string.IsNullOrEmpty(absolutepath))
                return null;

            switch (method)
            {
                case "POST":
                    string boundary = HTTPProcessor.ExtractBoundary(ContentType);

                    switch (absolutepath)
                    {
                        case "/rchome/leaderboard.py/query":
                            if (PostData != null && !string.IsNullOrEmpty(boundary))
                            {
                                try
                                {
                                    using (MemoryStream copyStream = new MemoryStream(PostData))
                                    {
                                        var data = MultipartFormDataParser.Parse(copyStream, boundary);

                                        string gameName = data.GetParameterValue("gameName");

                                        if (!string.IsNullOrEmpty(gameName))
                                        {
                                            lock (_leaderboards)
                                            {
                                                if (!_leaderboards.ContainsKey(gameName))
                                                    _leaderboards.Add(gameName, new FiringRangeScoreBoardData());
                                                return _leaderboards[gameName].UpdateScoreboardXml(workpath, gameName);
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LoggerAccessor.LogError($"[RCHOMEClass] - leaderboard.py query request thrown an assertion. (Exception: {ex})");
                                }
                            }
                            break;
                        case "/rchome/leaderboard.py/submit":
                            if (PostData != null && !string.IsNullOrEmpty(boundary))
                            {
                                try
                                {
                                    using (MemoryStream copyStream = new MemoryStream(PostData))
                                    {
                                        var data = MultipartFormDataParser.Parse(copyStream, boundary);

                                        string gameName = data.GetParameterValue("game");
                                        string player = data.GetParameterValue("player");
                                        string score = data.GetParameterValue("score");
                                        string expectedHash = data.GetParameterValue("hash");
                                        string hash = DotNetHasher.ComputeSHA1String(gameName + player + score + "awethnloaovdslqeoc").ToLower();

                                        if (hash == expectedHash)
                                        {
                                            lock (_leaderboards)
                                            {
                                                if (!string.IsNullOrEmpty(gameName) && _leaderboards.ContainsKey(gameName))
                                                {
                                                    _leaderboards[gameName].UpdateScoreBoard(player, (int)double.Parse(score, CultureInfo.InvariantCulture));
                                                    return _leaderboards[gameName].UpdateScoreboardXml(workpath, gameName);
                                                }
                                            }
                                        }
                                        else
                                            LoggerAccessor.LogWarn($"[RCHOMEClass] - leaderboard.py submit request: invalid hash sent! Received:{hash} Expected:{expectedHash}");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LoggerAccessor.LogError($"[RCHOMEClass] - leaderboard.py submit request thrown an assertion. (Exception: {ex})");
                                }
                            }
                            break;
                    }
                    break;
            }

            return null;
        }
    }
}
