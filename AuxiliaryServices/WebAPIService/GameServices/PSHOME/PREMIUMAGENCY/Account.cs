using CustomLogger;
using HttpMultipartParser;
using MultiServerLibrary.HTTP;

namespace WebAPIService.GameServices.PSHOME.PREMIUMAGENCY
{
    public class Account
    {
        public static string checkAccount(byte[] PostData, string ContentType, string workpath)
        {
            var boundary = HTTPProcessor.ExtractBoundary(ContentType);
            using (var ms = new MemoryStream(PostData))
            {
                var data = MultipartFormDataParser.Parse(ms, boundary);

                var nid = data.GetParameterValue("nid");
                var lang = data.GetParameterValue("lang");
                var regcd = data.GetParameterValue("regcd");
                ms.Flush();
            }

            LoggerAccessor.LogInfo("Check Account successful");

            //ACCOUNT_ENTRY_NONE = 1 / ACCOUNT_ENTRY_DONE = 2
            return @"<xml>\r\n\t"
                + "<result type=\"int\">1</result>\r\n\t"
                + "<description type=\"text\">CHECK_ACCOUNT</description>\r\n\t"
                + "<error_no type=\"int\">0</error_no>\r\n\t"
                + "<error_message type=\"text\">None</error_message>\r\n\r\n\t"
                + "</xml>";
            ;
        }

        public static string entryAccount(byte[] PostData, string ContentType, string workpath)
        {
            var boundary = HTTPProcessor.ExtractBoundary(ContentType);
            using (var ms = new MemoryStream(PostData))
            {
                var data = MultipartFormDataParser.Parse(ms, boundary);

                var nid = data.GetParameterValue("nid");
                var lang = data.GetParameterValue("lang");
                var regcd = data.GetParameterValue("regcd");
                ms.Flush();
            }

            LoggerAccessor.LogInfo("Check Account successful");

            //ACCOUNT_ENTRY_NONE = 1 / ACCOUNT_ENTRY_DONE = 2
            return @"<xml>\r\n\t"
                + "<result type=\"int\">1</result>\r\n\t"
                + "<description type=\"text\">CHECK_ACCOUNT</description>\r\n\t"
                + "<error_no type=\"int\">0</error_no>\r\n\t"
                + "<error_message type=\"text\">None</error_message>\r\n\r\n\t"
                + "</xml>";
            ;
        }
    }
}
