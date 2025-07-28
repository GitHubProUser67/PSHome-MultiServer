using CustomLogger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPIService.GameServices.UBISOFT.BuildAPI.BuildDBPullService;
using WebAPIService.GameServices.OUWF;

namespace WebAPIService.GameServices.UBISOFT.BuildAPI
{

    public class SoapBuildAPIClass
    {
        string absolutepath;
        string method;

        public SoapBuildAPIClass(string method, string absolutepath)
        {
            this.absolutepath = absolutepath;
            this.method = method;
        }

        public string ProcessRequest(byte[] PostData, string ContentType)
        {
            if (string.IsNullOrEmpty(absolutepath))
                return null;

            switch (method)
            {
                case "POST":
                    switch (absolutepath)
                    {

                        case "/BuildDBPullService.asmx":
                            return BuildDBPullServiceHandler.buildDBRequestParser(PostData, ContentType);
                        default:
                            {
#if DEBUG
                                LoggerAccessor.LogWarn($"[BuildDBPullService] - Unhandled server request discovered: {absolutepath} | DETAILS: \n{Encoding.UTF8.GetString(PostData)}");
#else
                                LoggerAccessor.LogWarn($"[BuildDBPullService] - Unhandled server request discovered: {absolutepath}");
#endif
                            }
                            break;
                    }
                    break;
                default:
                    {
                        LoggerAccessor.LogWarn($"[BuildDBPullService] - Method unhandled {method}");
                    }
                    break;
            }

            return null;
        }
    }
}
