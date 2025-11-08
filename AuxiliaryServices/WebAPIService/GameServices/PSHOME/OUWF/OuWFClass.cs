using CustomLogger;
using System;
using System.Text;

namespace WebAPIService.GameServices.PSHOME.OUWF
{
    public class OuWFClass
    {
        string workpath;
        string absolutepath;
        string method;

        public OuWFClass(string method, string absolutepath, string workpath)
        {
            this.workpath = workpath;
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

                        case "/list/":
                            return OuWFList.List(PostData, ContentType);
                        case "/scrape/":
                            return OuWFScrape.Scrape(PostData, ContentType);
                        case "/set/":
                            return OuWFSet.Set(PostData, ContentType);
                        case "/execute/":
                            return OuWFExecute.Execute(PostData, ContentType);
                        default:
                            {
                                LoggerAccessor.LogError($"[OuWF] - Unhandled server request discovered: {absolutepath} | DETAILS: \n{Encoding.UTF8.GetString(PostData)}");
                            }
                            break;
                    }
                    break;
                default:
                    {
                        LoggerAccessor.LogError($"[OuWF] - Method unhandled {method}");
                    }
                    break;
            }

            return null;
        }
    }
}