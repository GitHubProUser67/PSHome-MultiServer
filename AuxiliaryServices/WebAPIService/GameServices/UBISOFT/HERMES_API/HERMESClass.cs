using System;
using WebAPIService.GameServices.UBISOFT.HERMES_API.v1;
using WebAPIService.GameServices.UBISOFT.HERMES_API.v2;

namespace WebAPIService.GameServices.UBISOFT.HERMES_API
{
    public class HERMESClass
    {
        private string absolutepath;
        private string method;
        private string UbiAppId;
        private string UbiRequestedPlatformType;
        private string ubiappbuildid;
        private string clientip;
        private string regioncode;
        private string ticket;
        private string apipath;

        public HERMESClass(string method, string absolutepath, string UbiAppId, string UbiRequestedPlatformType, string ubiappbuildid, string clientip, string regioncode, string ticket, string apipath)
        {
            this.absolutepath = absolutepath;
            this.method = method;
            this.UbiAppId = UbiAppId;
            this.UbiRequestedPlatformType = UbiRequestedPlatformType;
            this.ubiappbuildid = ubiappbuildid;
            this.clientip = clientip;
            this.regioncode = regioncode;
            this.ticket = ticket;
            this.apipath = apipath;
        }

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
                            return V1SessionsClass.HandleSessionPOST(PostData, UbiAppId, clientip, regioncode);
                        default:
                            break;
                    }
                    break;
                case "GET":
                    switch (absolutepath)
                    {
                        default:
                            if (absolutepath.StartsWith("/v1/applications/") && absolutepath.EndsWith("configuration"))
                                return V2ConfigurationClass.HandleConfigurationGET(apipath, UbiAppId);
                            else if (absolutepath.StartsWith("/v2/applications/") && absolutepath.EndsWith("configuration"))
                                return V2ConfigurationClass.HandleConfigurationGET(apipath, UbiAppId);
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
