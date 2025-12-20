
using CustomLogger;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text;

namespace WebAPIService.GameServices.PSHOME.CAPONE
{
    public class CAPONEClass
    {
        private string workPath;
        private string absolutePath;
        private string method;

        public CAPONEClass(string method, string absolutePath, string workPath)
        {
            this.workPath = workPath;
            this.absolutePath = absolutePath;
            this.method = method;
        }


        public string ProcessRequest(byte[] PostData, string ContentType, bool https)
        {
            if (string.IsNullOrEmpty(absolutePath))
                return null;

            string res = string.Empty;

            switch (method)
            {
                case "POST":
                    switch (absolutePath)
                    {
                        case "/capone/reportCollector/submit/":
                            {

                                res = GriefReporter.caponeReportCollectorSubmit(PostData, ContentType, workPath);
                                return res;
                            }

                            //Case statement won't handle dynamic changing strings
                        default:

                            res = GriefReporter.caponeContentStoreUpload(PostData, ContentType, workPath, absolutePath);
                            return res;

                            
                    }
                default:
                    break;
            }

            return res;
        }
    }
}
