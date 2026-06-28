using CustomLogger;

namespace WebAPIService.GameServices.PSHOME.LOOT
{
    public class LOOTClass(string method, string absolutepath, string workpath)
    {
        private readonly string absolutepath = absolutepath;
        private readonly string workpath = workpath;
        private readonly string method = method;

        public string ProcessRequest(
            IDictionary<string, string> QueryParameters,
            byte[] PostData = null,
            string ContentType = null
        )
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
                                return LOOTDatabase.ProcessDatabaseRequest(
                                    PostData,
                                    ContentType,
                                    workpath
                                );
                            break;
                        default:
                            LoggerAccessor.LogWarn(
                                $"[LOOT] Unhandled POST request {absolutepath} please report to GITHUB"
                            );
                            break;
                    }
                    break;
                case "GET":
                    switch (absolutepath)
                    {
                        case "/moviedb/settings/":
                        {
                            return LOOTMovieDb.FetchDBInfo(workpath, QueryParameters["id"]);
                        }
                        default:
                            LoggerAccessor.LogWarn(
                                $"[LOOT] Unhandled GET request {absolutepath} please report to GITHUB"
                            );
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
