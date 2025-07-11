using CustomLogger;
using HttpMultipartParser;
using MultiServerLibrary.HTTP;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace WebAPIService.CODEGLUE
{
    public class WipeoutShooterClass
    {
        private static Dictionary<string, WipeoutShooterScoreBoardData> _leaderboards = new Dictionary<string, WipeoutShooterScoreBoardData>();

        private string workpath;
        private string method;

        public WipeoutShooterClass(string method, string workpath)
        {
            this.method = method;
            this.workpath = workpath;
        }

        public string ProcessRequest(IDictionary<string, string> QueryParameters, byte[] PostData = null, string ContentType = null)
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

                                string GAME_TYPE = data.GetParameterValue("GAME_TYPE");
                                string TERRITORY = data.GetParameterValue("TERRITORY");
                                string REGION = data.GetParameterValue("REGION");

                                if (byte.TryParse(GAME_TYPE, out byte gameTypeIByte))
                                {
                                    switch (gameTypeIByte)
                                    {
                                        case 1:
                                            GAME_TYPE = "SINGLE";
                                            break;
                                        case 2:
                                            GAME_TYPE = "COOP";
                                            break;
                                        case 3:
                                            GAME_TYPE = "VERSUS";
                                            break;
                                    }

                                    lock (_leaderboards)
                                    {
                                        if (_leaderboards.ContainsKey(GAME_TYPE))
                                        {
                                            _leaderboards[GAME_TYPE].UpdateScoreBoard(data.GetParameterValue("NAME"), float.Parse(data.GetParameterValue("SCORE"), CultureInfo.InvariantCulture));
                                            return _leaderboards[GAME_TYPE].UpdateScoreboardXml(workpath, GAME_TYPE);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LoggerAccessor.LogError($"[WipeoutShooterClass] - leaderboard submit request thrown an assertion. (Exception: {ex})");
                        }
                    }
                    break;
                case "GET":
                    if (QueryParameters.ContainsKey("TERRITORY") && QueryParameters.ContainsKey("NAME"))
                    {
                        StringBuilder st = new StringBuilder("<XML><LEADERBOARD>");

                        try
                        {
                            string TERRITORY = QueryParameters["TERRITORY"];
                            string NAME = QueryParameters["NAME"];

                            for (byte i = 1; i < 4; i++)
                            {
                                string GAME_TYPE = string.Empty;
                                switch (i)
                                {
                                    case 1:
                                        GAME_TYPE = "SINGLE";
                                        break;
                                    case 2:
                                        GAME_TYPE = "COOP";
                                        break;
                                    case 3:
                                        GAME_TYPE = "VERSUS";
                                        break;
                                }

                                lock (_leaderboards)
                                {
                                    if (!_leaderboards.ContainsKey(GAME_TYPE))
                                        _leaderboards.Add(GAME_TYPE, new WipeoutShooterScoreBoardData());
                                    st.Append(_leaderboards[GAME_TYPE].UpdateScoreboardXml(workpath, GAME_TYPE));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LoggerAccessor.LogError($"[WipeoutShooterClass] - leaderboard list querying request thrown an assertion. (Exception: {ex})");
                        }

                        st.Append("</LEADERBOARD></XML>");

                        return st.ToString();
                    }
                    break;
            }

            return null;
        }
    }
}
