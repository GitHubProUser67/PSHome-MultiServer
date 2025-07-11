using MultiServerLibrary.HTTP;
using System;
using WebAPIService.HTS.Helpers;

namespace WebAPIService.HTS
{
    public class HTSClass
    {
        private string workpath;
        private string absolutepath;
        private string method;

        public HTSClass(string method, string absolutepath, string workpath)
        {
            this.absolutepath = absolutepath;
            this.workpath = workpath;
            this.method = method;
        }

        public string ProcessRequest(byte[] PostData, string ContentType, bool https)
        {
            if (string.IsNullOrEmpty(absolutepath))
                return null;

            switch (method)
            {
                case "POST":
                    switch (absolutepath)
                    {
                        #region NPTicket Sample
                        case "/NPTicketing/get_ticket_data.xml":
                        case "/NPTicketing/get_ticket_data.json":
                        case "/NPTicketing/get_ticket_data_base64.xml":
                        case "/NPTicketing/get_ticket_data_base64.json":
                            return NPTicketSample.RequestNPTicket(PostData, HTTPProcessor.ExtractBoundary(ContentType));
                        #endregion

                        default:    
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
