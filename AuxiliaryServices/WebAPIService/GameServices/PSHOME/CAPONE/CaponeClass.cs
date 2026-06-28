namespace WebAPIService.GameServices.PSHOME.CAPONE
{
    public class CAPONEClass(string method, string absolutePath, string workPath)
    {
        private readonly string workPath = workPath;
        private readonly string absolutePath = absolutePath;
        private readonly string method = method;

        public string ProcessRequest(byte[] PostData, string ContentType, bool https)
        {
            if (string.IsNullOrEmpty(absolutePath))
                return null;

            var res = string.Empty;

            switch (method)
            {
                case "POST":
                    switch (absolutePath)
                    {
                        case "/capone/reportCollector/submit/":
                        {
                            res = GriefReporter.caponeReportCollectorSubmit(
                                PostData,
                                ContentType,
                                workPath
                            );
                            return res;
                        }

                        //Case statement won't handle dynamic changing strings
                        default:

                            res = GriefReporter.caponeContentStoreUpload(
                                PostData,
                                ContentType,
                                workPath,
                                absolutePath
                            );
                            return res;
                    }
                default:
                    break;
            }

            return res;
        }
    }
}
