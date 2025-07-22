using CustomLogger;
using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Collections.Generic;

namespace WebAPIService.GameServices.CDM
{
    public class CDMClass
    {
        private string workPath;
        private string absolutePath;
        private string method;

        public CDMClass(string method, string absolutePath, string workPath)
        {
            this.workPath = workPath;
            this.absolutePath = absolutePath;
            this.method = method;
        }

        public string ProcessRequest(byte[] PostData, string ContentType, string directoryPath)
        {
            if (string.IsNullOrEmpty(absolutePath) || string.IsNullOrEmpty(directoryPath))
                return null;

            string res = string.Empty;
            string endPointURI = string.Empty;

            List<string> endPoints = new List<string>() { "/user/game/", "/user/sync/", "/user/event/", "/user/quest/", "/user/space/",
                "/userevent/list/date/", "/userevent/list/friend/", "/quest/list/date/", "/leaderboard/" };

            // Dedicated endpoint trimmer for sanity checks!
            foreach (string endPoint in endPoints)
            {
                if (absolutePath.StartsWith(endPoint))
                {
                    endPointURI = absolutePath.Substring(0, endPoint.Length);
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
                            return Publisher.handlePublisherList(PostData, ContentType, workPath, absolutePath);
                        case "/user/game/":
                            return User.handleGame(PostData, ContentType, workPath, absolutePath);
                        case "/user/space/":
                            return User.handleSpace(PostData, ContentType, workPath, absolutePath);
                        case "/leaderboard/":
                            return Leaderboards.handleLeaderboards(PostData, ContentType, workPath, absolutePath);
                        default:
                            LoggerAccessor.LogError($"Unhandled GET endpoint for {endPointURI}");
                            break;
                    }
                    break;
                case "POST":
                   switch (endPointURI)
                    {
                        case "/user/sync/":
                            return User.handleUserSync(PostData, ContentType, workPath, absolutePath);
                        default:
                            LoggerAccessor.LogError($"Unhandled POST endpoint for {endPointURI}");
                            break;
                    }
                    break;
                default:
                    LoggerAccessor.LogError($"Unhandled {method} endpoint for {absolutePath}");
                    break;
            }

            return res;
        }
    }
}
