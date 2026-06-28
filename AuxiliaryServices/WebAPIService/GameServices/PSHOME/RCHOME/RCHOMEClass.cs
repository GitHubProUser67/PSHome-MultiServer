using System.Globalization;
using CastleLibrary.NetHasher;
using CustomLogger;
using HttpMultipartParser;
using MultiServerLibrary.HTTP;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.PSHOME.RCHOME
{
    public class RCHOMEClass(string method, string absolutepath, string workpath)
    {
        private static readonly Dictionary<string, FiringRangeScoreBoardData> _leaderboards = [];

        private readonly string absolutepath = absolutepath;
        private readonly string workpath = workpath;
        private readonly string method = method;

        public string ProcessRequest(byte[] PostData = null, string ContentType = null)
        {
            if (string.IsNullOrEmpty(absolutepath))
                return null;

            switch (method)
            {
                case "POST":
                    var boundary = HTTPProcessor.ExtractBoundary(ContentType);

                    switch (absolutepath)
                    {
                        case "/rchome/leaderboard.py/query":
                            if (PostData != null && !string.IsNullOrEmpty(boundary))
                            {
                                try
                                {
                                    using (var copyStream = new MemoryStream(PostData))
                                    {
                                        var data = MultipartFormDataParser.Parse(
                                            copyStream,
                                            boundary
                                        );

                                        var gameName = data.GetParameterValue("gameName");

                                        if (!string.IsNullOrEmpty(gameName))
                                        {
                                            lock (_leaderboards)
                                            {
                                                if (!_leaderboards.ContainsKey(gameName))
                                                    _leaderboards.Add(
                                                        gameName,
                                                        new FiringRangeScoreBoardData(
                                                            LeaderboardDbContext.BuildOptions(
                                                                0,
                                                                $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}"
                                                            ),
                                                            gameName
                                                        )
                                                    );

                                                return _leaderboards[gameName]
                                                    .SerializeToString("data")
                                                    .Result;
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LoggerAccessor.LogError(
                                        $"[RCHOMEClass] - leaderboard.py query request thrown an assertion. (Exception: {ex})"
                                    );
                                }
                            }
                            break;
                        case "/rchome/leaderboard.py/submit":
                            if (PostData != null && !string.IsNullOrEmpty(boundary))
                            {
                                try
                                {
                                    using (var copyStream = new MemoryStream(PostData))
                                    {
                                        var data = MultipartFormDataParser.Parse(
                                            copyStream,
                                            boundary
                                        );

                                        var gameName = data.GetParameterValue("game");
                                        var player = data.GetParameterValue("player");
                                        var score = data.GetParameterValue("score");
                                        var expectedHash = data.GetParameterValue("hash");
                                        var hash = DotNetHasher
                                            .ComputeSHA1String(
                                                gameName + player + score + "awethnloaovdslqeoc"
                                            )
                                            .ToLower();

                                        if (hash == expectedHash)
                                        {
                                            if (!string.IsNullOrEmpty(gameName))
                                            {
                                                lock (_leaderboards)
                                                {
                                                    if (!_leaderboards.ContainsKey(gameName))
                                                        _leaderboards.Add(
                                                            gameName,
                                                            new FiringRangeScoreBoardData(
                                                                LeaderboardDbContext.BuildOptions(
                                                                    0,
                                                                    $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}"
                                                                ),
                                                                gameName
                                                            )
                                                        );

                                                    _ = _leaderboards[gameName]
                                                        .UpdateScoreAsync(
                                                            player,
                                                            (int)
                                                                float.Parse(
                                                                    score,
                                                                    CultureInfo.InvariantCulture
                                                                )
                                                        );
                                                    return _leaderboards[gameName]
                                                        .SerializeToString("data")
                                                        .Result;
                                                }
                                            }
                                        }
                                        else
                                            LoggerAccessor.LogWarn(
                                                $"[RCHOMEClass] - leaderboard.py submit request: invalid hash sent! Received:{hash} Expected:{expectedHash}"
                                            );
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LoggerAccessor.LogError(
                                        $"[RCHOMEClass] - leaderboard.py submit request thrown an assertion. (Exception: {ex})"
                                    );
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
