using System.Text;
using CastleLibrary.NetHasher;
using HttpMultipartParser;
using MultiServerLibrary.HTTP;

namespace WebAPIService.GameServices.PSHOME.NDREAMS.Aurora
{
    public static class Teaser
    {
        public static string ProcessBeans(byte[] PostData, string ContentType)
        {
            var key = string.Empty;
            var territory = string.Empty;
            var day = string.Empty;
            var boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (var ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    var func = data.GetParameterValue("func");
                    key = data.GetParameterValue("key");
                    territory = data.GetParameterValue("territory");
                    day = data.GetParameterValue("day");

                    ms.Flush();
                }

                var ExpectedHash = Xoff_VerifyKey(territory, day);

                if (key == ExpectedHash)
                {
                    byte MockedDay = 0;

                    // Get the current day of the week
                    switch (DateTime.Today.DayOfWeek)
                    {
                        case DayOfWeek.Monday:
                            MockedDay = 5;
                            break;
                        case DayOfWeek.Tuesday:
                            MockedDay = 4;
                            break;
                        case DayOfWeek.Wednesday:
                            MockedDay = 3;
                            break;
                        case DayOfWeek.Thursday:
                            MockedDay = 2;
                            break;
                        case DayOfWeek.Friday:
                            MockedDay = 1;
                            break;
                    }

                    return $"<xml><success>true</success><result><day>{MockedDay}</day><hash>{Xoff_GetSignature(int.Parse(day), MockedDay)}</hash></result></xml>";
                }
                else
                {
                    var errMsg =
                        $"[nDreams] - Teaser: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                    return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessBeans</function></xml>";
                }
            }

            return null;
        }

        public static string Xoff_VerifyKey(string playerregion, string day)
        {
            return DotNetHasher
                .ComputeSHA1String(Encoding.UTF8.GetBytes("xoff" + playerregion + day + "done!"))
                .ToLower();
        }

        public static string Xoff_GetSignature(int day, int ResultDay)
        {
            return DotNetHasher
                .ComputeSHA1String(
                    Encoding.UTF8.GetBytes(
                        string.Format("Yum!Salted{0}", ((day + 3) * 1239) - (day * 6) + day)
                            + ResultDay
                    )
                )
                .ToLower();
        }
    }
}
