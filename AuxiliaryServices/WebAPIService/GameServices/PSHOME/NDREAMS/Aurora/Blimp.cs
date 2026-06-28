using HttpMultipartParser;
using MultiServerLibrary.HTTP;

namespace WebAPIService.GameServices.PSHOME.NDREAMS.Aurora
{
    public static class Blimp
    {
        public static string ProcessBlimps(
            DateTime CurrentDate,
            byte[] PostData,
            string ContentType
        )
        {
            var key = string.Empty;
            var func = string.Empty;
            var resdata = string.Empty;
            var user = string.Empty;
            var boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (var ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    func = data.GetParameterValue("func");

                    try
                    {
                        key = data.GetParameterValue("key");
                        resdata = data.GetParameterValue("data");
                        user = data.GetParameterValue("user");
                        var ship = data.GetParameterValue("ship");
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
                        var ExpectedHash = NDREAMSServerUtils.Server_GetSignature(
                            "Blimps",
                            user,
                            resdata,
                            CurrentDate
                        );

                        if (key == ExpectedHash)
                            return "<xml></xml>";
                        else
                            CustomLogger.LoggerAccessor.LogWarn(
                                $"[nDreams] - Blimps: invalid key sent! Received:{key} Expected:{ExpectedHash}"
                            );
                        break;
                }
            }

            return null;
        }
    }
}
