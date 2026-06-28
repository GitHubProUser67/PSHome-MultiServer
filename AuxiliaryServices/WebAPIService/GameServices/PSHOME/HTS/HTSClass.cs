using MultiServerLibrary.HTTP;
using WebAPIService.GameServices.PSHOME.HTS.Helpers;

namespace WebAPIService.GameServices.PSHOME.HTS
{
    public class HTSClass(string method, string absolutepath, string workpath)
    {
        private readonly string workpath = workpath;
        private readonly string absolutepath = absolutepath;
        private readonly string method = method;

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
                            return NPTicketSample.RequestNPTicket(
                                PostData,
                                HTTPProcessor.ExtractBoundary(ContentType)
                            );
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
