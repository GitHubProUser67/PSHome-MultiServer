using CustomLogger;
using System;
using System.Collections.Generic;

namespace WebAPIService.GameServices.LOOT
{
    public class LOOTClass
    {
        private string absolutepath;
        private string workpath;
        private string method;

        public LOOTClass(string method, string absolutepath, string workpath)
        {
            this.absolutepath = absolutepath;
            this.method = method;
            this.workpath = workpath;
        }

        public string ProcessRequest(IDictionary<string, string> QueryParameters, byte[] PostData = null, string ContentType = null)
        {
            if (string.IsNullOrEmpty(absolutepath))
                return null;

            switch (method)
            {
                case "POST":
                    switch (absolutepath)
                    {
                        case "/index.action.php":
                            if (PostData != null && !string.IsNullOrEmpty(ContentType))
                                return LOOTDatabase.ProcessDatabaseRequest(PostData, ContentType, workpath);
                            break;
                        default:
                            LoggerAccessor.LogWarn($"[LOOT] Unhandled POST request {absolutepath} please report to GITHUB");
                            break;
                    }
                    break;
                case "GET":
                    switch(absolutepath)
                    {
                        case "/moviedb/settings/":
                            {
                                return LOOTMovieDb.FetchDBInfo(workpath, QueryParameters["id"]);
                            }
                        default:
                            LoggerAccessor.LogWarn($"[LOOT] Unhandled GET request {absolutepath} please report to GITHUB");
                            break;
                    }
                    break;
                default:
                    break;
            }

            return null;
        }
    }
}
