using MultiServerLibrary.HTTP;
using HttpMultipartParser;
using System.IO;

namespace WebAPIService.NDREAMS.Xi2
{
    public class PStats
    {
        public static string ProcessPStats(byte[] PostData, string ContentType)
        {
            string func = null;
            string name = null;
            string boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    func = data.GetParameterValue("func");
                    name = data.GetParameterValue("name");

                    ms.Flush();
                }

                return "<xml><success>true</success><result></result></xml>";
            }

            return null;
        }
    }
}
