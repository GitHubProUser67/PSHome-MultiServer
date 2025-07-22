using HttpMultipartParser;
using MultiServerLibrary.HTTP;
using System;
using System.IO;
using WebAPIService.GameServices.NDREAMS;

namespace WebAPIService.GameServices.NDREAMS.Aurora
{
    public static class Blimp
    {
        public static string ProcessBlimps(DateTime CurrentDate, byte[] PostData, string ContentType)
        {
            string key = string.Empty;
            string func = string.Empty;
            string resdata = string.Empty;
            string user = string.Empty;
            string ship = string.Empty;
            string boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    func = data.GetParameterValue("func");

                    try
                    {
                        key = data.GetParameterValue("key");
                        resdata = data.GetParameterValue("data");
                        user = data.GetParameterValue("user");
                        ship = data.GetParameterValue("ship");
                    }
                    catch
                    {
                        // Not Important.
                    }

                    ms.Flush();
                }

                switch (func)
                {
                    case "play":
                        return "<xml></xml>";
                    case "ships":
                        string ExpectedHash = NDREAMSServerUtils.Server_GetSignature("Blimps", user, resdata, CurrentDate);

                        if (key == ExpectedHash)
                            return "<xml></xml>";
                        else
                            CustomLogger.LoggerAccessor.LogWarn($"[nDreams] - Blimps: invalid key sent! Received:{key} Expected:{ExpectedHash}");
                        break;
                }
            }

            return null;
        }
    }
}
