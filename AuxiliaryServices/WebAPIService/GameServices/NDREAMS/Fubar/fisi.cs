using MultiServerLibrary.HTTP;
using CustomLogger;
using HttpMultipartParser;
using System.Text;
using System.IO;

namespace WebAPIService.GameServices.NDREAMS.Fubar
{
    public class fisi
    {
        public static string fisiProcess(byte[] PostData, string contentType) {

            LoggerAccessor.LogInfo($"FUBAR POSTDATA: {Encoding.UTF8.GetString(PostData)}");

            string boundary = HTTPProcessor.ExtractBoundary(contentType);
            using (MemoryStream ms = new MemoryStream(PostData))
            {
                var data = MultipartFormDataParser.Parse(ms, boundary);

                string key = data.GetParameterValue("key");
                string name = data.GetParameterValue("name");
                string func = data.GetParameterValue("func");
                string tick = data.GetParameterValue("tick");


                switch(func)
                {
                    case "getPlayer":
                        return $"<xmlParser>\r\n\t" +
                            $"<key>{key}</key>\r\n" +
                            $"</xmlParser>";
                    default:
                        LoggerAccessor.LogError($"func unhandled: UserName: {name} {func} | key {key} | tick {tick}");
                        break;
                }




                ms.Flush();
            }


            return "";
        }
    }
}
