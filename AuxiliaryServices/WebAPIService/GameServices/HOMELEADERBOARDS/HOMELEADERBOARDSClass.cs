using CustomLogger;
using HttpMultipartParser;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace WebAPIService.GameServices.HOMELEADERBOARDS
{
    public static class HOMELEADERBOARDSClass
    {
        private static Dictionary<string, HomeLeaderboards> _leaderboards = new Dictionary<string, HomeLeaderboards>();

        public static string ProcessEntryBare(byte[] postdata, string boundary, string apiPath)
        {
            if (postdata != null && !string.IsNullOrEmpty(boundary))
            {
                try
                {
                    using (MemoryStream copyStream = new MemoryStream(postdata))
                    {
                        var data = MultipartFormDataParser.Parse(copyStream, boundary);

                        string postType = data.GetParameterValue("postType");
                        string game = data.GetParameterValue("game");

                        switch (postType)
                        {
                            case "getHighScore":
                                if (!string.IsNullOrEmpty(game))
                                {
                                    lock (_leaderboards)
                                    {
                                        if (!_leaderboards.ContainsKey(game))
                                            _leaderboards.Add(game, new HomeLeaderboards());
                                        return $"<MsRoot>{_leaderboards[game].UpdateScoreboardXml(apiPath, game)}</MsRoot>";
                                    }
                                }
                                break;
                            case "postScore":
                                double score = double.Parse(data.GetParameterValue("score"), CultureInfo.InvariantCulture);
                                string player = data.GetParameterValue("player");

                                lock (_leaderboards)
                                {
                                    if (!string.IsNullOrEmpty(game) && _leaderboards.ContainsKey(game))
                                    {
                                        _leaderboards[game].UpdateScoreBoard(player, score);
                                        return $"<MsRoot>{_leaderboards[game].UpdateScoreboardXml(apiPath, game)}</MsRoot>";
                                    }
                                }
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LoggerAccessor.LogError($"[HOMELEADERBOARDSClass] - entryBare request thrown an assertion. (Exception: {ex})");
                }
            }

            return null;
        }
    }
}
