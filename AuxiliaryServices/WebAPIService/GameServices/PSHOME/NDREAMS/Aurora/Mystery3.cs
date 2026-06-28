using HttpMultipartParser;
using MultiServerLibrary.HTTP;

namespace WebAPIService.GameServices.PSHOME.NDREAMS.Aurora
{
    public static class Mystery3
    {
        public static string ProcessMystery3(
            DateTime CurrentDate,
            byte[] PostData,
            string ContentType,
            string fullurl,
            string apipath
        )
        {
            var key = string.Empty;
            var func = string.Empty;
            var name = string.Empty;
            var resdata = string.Empty;
            var finger = string.Empty;
            var boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                ushort min = 0;
                ushort max = 180;
                using (var ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    key = data.GetParameterValue("key");
                    func = data.GetParameterValue("func");
                    name = data.GetParameterValue("name");
                    try
                    {
                        resdata = data.GetParameterValue("data");
                        finger = data.GetParameterValue("finger");
                    }
                    catch
                    {
                        // Not Important.
                    }

                    ms.Flush();
                }

                int turns;
                // Check if today is April 4th or October 25th
                if (CurrentDate.Month == 4 && CurrentDate.Day == 4)
                {
                    // Increase the chance of getting 80
                    turns = new Random().Next(0, 100) < 70 ? 80 : new Random().Next(min, max);
                }
                else if (CurrentDate.Month == 10 && CurrentDate.Day == 25)
                {
                    // Increase the chance of getting 60
                    turns = new Random().Next(0, 100) < 80 ? 60 : new Random().Next(min, max);
                }
                else
                    // For other dates, use a uniform random distribution
                    turns = new Random().Next(min, max);

                Directory.CreateDirectory(apipath + "/NDREAMS/Aurora/Mystery3");
                var TimestampProfilePath = apipath + $"/NDREAMS/Aurora/Mystery3/{name}.txt";

                string ExpectedHash;
                switch (func)
                {
                    case "get":
                        if (File.Exists(TimestampProfilePath))
                        {
                            var timestamp = File.ReadAllText(TimestampProfilePath);
                            return $"<xml><sig>{NDREAMSServerUtils.Server_GetSignature(fullurl, name, "collect", CurrentDate)}</sig><confirm>{NDREAMSServerUtils.Server_KeyToHash(key, CurrentDate, timestamp)}</confirm><timestamp>{timestamp}</timestamp><Turns>{turns}</Turns></xml>";
                        }
                        else
                            return $"<xml><sig>{NDREAMSServerUtils.Server_GetSignature(fullurl, name, "collect", CurrentDate)}</sig><confirm>{NDREAMSServerUtils.Server_KeyToHash(key, CurrentDate, "nil")}</confirm><timestamp>nil</timestamp><Turns>{turns}</Turns></xml>";
                    case "giveExp":
                        ExpectedHash = NDREAMSServerUtils.Server_GetSignature(
                            fullurl,
                            name,
                            "collect" + resdata,
                            CurrentDate
                        );

                        if (finger == ExpectedHash)
                            return $"<xml><sig>{ExpectedHash}</sig><confirm>{NDREAMSServerUtils.Server_KeyToHash(key, CurrentDate, resdata)}</confirm></xml>";
                        else
                            CustomLogger.LoggerAccessor.LogWarn(
                                $"[nDreams] - Mystery3: invalid fingerprint sent! Received:{finger} Expected:{ExpectedHash}"
                            );

                        break;
                    case "set":
                        ExpectedHash = NDREAMSServerUtils.Server_GetSignature(
                            fullurl,
                            name,
                            "collect" + resdata,
                            CurrentDate
                        );

                        if (finger == ExpectedHash)
                        {
                            if (resdata == "nil" && File.Exists(TimestampProfilePath))
                                File.Delete(TimestampProfilePath);
                            else
                                File.WriteAllText(TimestampProfilePath, resdata);

                            return $"<xml><sig>{ExpectedHash}</sig><confirm>{NDREAMSServerUtils.Server_KeyToHash(key, CurrentDate, resdata)}</confirm></xml>";
                        }
                        else
                            CustomLogger.LoggerAccessor.LogWarn(
                                $"[nDreams] - Mystery3: invalid fingerprint sent! Received:{finger} Expected:{ExpectedHash}"
                            );

                        break;
                }
            }

            return null;
        }
    }
}
