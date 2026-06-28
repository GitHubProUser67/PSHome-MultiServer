using WebAPIService.GameServices.UBISOFT.HERMES_API.v1;
using WebAPIService.GameServices.UBISOFT.HERMES_API.v2;

namespace WebAPIService.GameServices.UBISOFT.HERMES_API
{
    public class HERMESClass(
        string method,
        string absolutepath,
        string UbiAppId,
        string UbiRequestedPlatformType,
        string ubiappbuildid,
        string clientip,
        string regioncode,
        string ticket,
        string apipath
    )
    {
        private readonly string absolutepath = absolutepath;
        private readonly string method = method;
        private readonly string UbiAppId = UbiAppId;
        private readonly string UbiRequestedPlatformType = UbiRequestedPlatformType;
        private readonly string ubiappbuildid = ubiappbuildid;
        private readonly string clientip = clientip;
        private readonly string regioncode = regioncode;
        private readonly string ticket = ticket;
        private readonly string apipath = apipath;

        public (string, string) ProcessRequest(byte[] PostData, string ContentType)
        {
            if (string.IsNullOrEmpty(absolutepath))
                return (null, null);

            switch (method)
            {
                case "POST":
                    switch (absolutepath)
                    {
                        case "/v1/profiles/sessions":
                            return V1SessionsClass.HandleSessionPOST(
                                PostData,
                                UbiAppId,
                                clientip,
                                regioncode
                            );
                        default:
                            break;
                    }
                    break;
                case "GET":
                    switch (absolutepath)
                    {
                        default:
                            if (
                                absolutepath.StartsWith("/v1/applications/")
                                && absolutepath.EndsWith("configuration")
                            )
                                return V2ConfigurationClass.HandleConfigurationGET(
                                    apipath,
                                    UbiAppId
                                );
                            else if (
                                absolutepath.StartsWith("/v2/applications/")
                                && absolutepath.EndsWith("configuration")
                            )
                                return V2ConfigurationClass.HandleConfigurationGET(
                                    apipath,
                                    UbiAppId
                                );
                            break;
                    }
                    break;
                default:
                    break;
            }

            return (null, null);
        }
    }
}
