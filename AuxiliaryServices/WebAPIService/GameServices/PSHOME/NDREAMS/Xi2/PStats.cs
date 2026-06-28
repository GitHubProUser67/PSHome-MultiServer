using HttpMultipartParser;
using MultiServerLibrary.HTTP;

namespace WebAPIService.GameServices.PSHOME.NDREAMS.Xi2
{
    public class PStats
    {
        public static string ProcessPStats(byte[] PostData, string ContentType)
        {
            var boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (var ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    var func = data.GetParameterValue("func");
                    var name = data.GetParameterValue("name");

                    ms.Flush();
                }

                return "<xml><success>true</success><result></result></xml>";
            }

            return null;
        }
    }
}
