using System.Globalization;
using System.Text;
using CustomLogger;
using HttpMultipartParser;
using MultiServerLibrary.HTTP;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.PSHOME.CODEGLUE
{
    public class WipeoutShooterClass(string method, string workpath)
    {
        private static readonly Dictionary<string, WipeoutShooterScoreBoardData> _leaderboards = [];

        private readonly string workpath = workpath;
        private readonly string method = method;

        public string ProcessRequest(
            IDictionary<string, string> QueryParameters,
            byte[] PostData = null,
            string ContentType = null
        )
        {
            string TERRITORY;
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

                                var GAME_TYPE = data.GetParameterValue("GAME_TYPE");
                                TERRITORY = data.GetParameterValue("TERRITORY");
                                var REGION = data.GetParameterValue("REGION");

                                if (byte.TryParse(GAME_TYPE, out var gameTypeIByte))
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
                                        if (!_leaderboards.ContainsKey(GAME_TYPE))
                                            _leaderboards.Add(
                                                GAME_TYPE,
                                                new WipeoutShooterScoreBoardData(
                                                    LeaderboardDbContext.BuildOptions(
                                                        0,
                                                        $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}"
                                                    ),
                                                    GAME_TYPE
                                                )
                                            );

                                        _ = _leaderboards[GAME_TYPE]
                                            .UpdateScoreAsync(
                                                data.GetParameterValue("NAME"),
                                                float.Parse(
                                                    data.GetParameterValue("SCORE"),
                                                    CultureInfo.InvariantCulture
                                                )
                                            );
                                        return _leaderboards[GAME_TYPE]
                                            .SerializeToString(GAME_TYPE)
                                            .Result;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LoggerAccessor.LogError(
                                $"[WipeoutShooterClass] - leaderboard submit request thrown an assertion. (Exception: {ex})"
                            );
                        }
                    }
                    break;
                case "GET":
                    if (
                        QueryParameters.TryGetValue("TERRITORY", out TERRITORY)
                        && QueryParameters.ContainsKey("NAME")
                    )
                    {
                        var st = new StringBuilder("<XML><LEADERBOARD>");

                        try
                        {
                            var NAME = QueryParameters["NAME"];

                            for (byte i = 1; i < 4; i++)
                            {
                                var GAME_TYPE = string.Empty;
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
                                        _leaderboards.Add(
                                            GAME_TYPE,
                                            new WipeoutShooterScoreBoardData(
                                                LeaderboardDbContext.BuildOptions(
                                                    0,
                                                    $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}"
                                                ),
                                                GAME_TYPE
                                            )
                                        );

                                    st.Append(
                                        _leaderboards[GAME_TYPE].SerializeToString(GAME_TYPE).Result
                                    );
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LoggerAccessor.LogError(
                                $"[WipeoutShooterClass] - leaderboard list querying request thrown an assertion. (Exception: {ex})"
                            );
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
