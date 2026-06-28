using CustomLogger;

namespace WebAPIService.GameServices.PSHOME.CDM
{
    public class CDMClass(string method, string absolutePath, string workPath)
    {
        private readonly string workPath = workPath;
        private readonly string absolutePath = absolutePath;
        private readonly string method = method;

        public string ProcessRequest(byte[] PostData, string ContentType)
        {
            if (string.IsNullOrEmpty(absolutePath))
                return null;

            var res = string.Empty;
            var endPointURI = string.Empty;

            List<string> endPoints =
            [
                "/user/game/",
                "/user/sync/",
                "/user/event/",
                "/user/quest/",
                "/user/space/",
                "/userevent/list/date/",
                "/userevent/list/friend/",
                "/quest/list/date/",
                "/leaderboard/",
            ];

            // Dedicated endpoint trimmer for sanity checks!
            foreach (var endPoint in endPoints)
            {
                if (absolutePath.StartsWith(endPoint))
                {
                    endPointURI = absolutePath[..endPoint.Length];
                    break;
                }
            }

            // If no endpoint is found, use the full absolute path
            if (string.IsNullOrEmpty(endPointURI))
            {
                endPointURI = absolutePath;
            }

            switch (method)
            {
                case "GET":
                    switch (endPointURI)
                    {
                        ///<summary>
                        /// Primary endpoint for any CDM supported minigame, returns the company publisherID, password, and name.
                        /// If this publisher list does not contain a valid token and pubID, the minigame will consider the server unavailable.
                        ///</summary>
                        case "/publisher/list/":
                            return Publisher.handlePublisherList(
                                PostData,
                                ContentType,
                                workPath,
                                absolutePath
                            );
                        case "/user/game/":
                            return User.HandleGame(PostData, ContentType, workPath, absolutePath);
                        case "/user/space/":
                            return User.HandleSpace(PostData, ContentType, workPath, absolutePath);
                        case "/leaderboard/":
                            return Leaderboards.handleLeaderboards(
                                PostData,
                                ContentType,
                                workPath,
                                absolutePath
                            );
                        default:
                            LoggerAccessor.LogWarn(
                                $"[CDM] - Unhandled GET endpoint for {endPointURI}"
                            );
                            break;
                    }
                    break;
                case "POST":
                    switch (endPointURI)
                    {
                        case "/user/sync/":
                            return User.HandleUserSync(
                                PostData,
                                ContentType,
                                workPath,
                                absolutePath
                            );
                        default:
                            LoggerAccessor.LogWarn(
                                $"[CDM] - Unhandled POST endpoint for {endPointURI}"
                            );
                            break;
                    }
                    break;
                default:
                    LoggerAccessor.LogWarn(
                        $"[CDM] - Unhandled {method} endpoint for {absolutePath}"
                    );
                    break;
            }

            return res;
        }
    }
}
