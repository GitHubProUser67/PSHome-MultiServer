using System.Xml;
using HttpMultipartParser;
using MultiServerLibrary.HTTP;

namespace WebAPIService.GameServices.PSHOME.NDREAMS.Aurora
{
    public static class visit
    {
        public static string ProcessVisit(byte[] PostData, string ContentType, string apipath)
        {
            var name = string.Empty;
            var bonus = string.Empty;
            var boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (var ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    var friends = data.GetParameterValue("friends");
                    name = data.GetParameterValue("name");
                    var age = data.GetParameterValue("age");
                    bonus = data.GetParameterValue("bonus");

                    ms.Flush();
                }

                var CounterInfos = "<days>0</days><sessions>1</sessions>";

                var Prefix = "<new>false</new><first>false</first>";

                Directory.CreateDirectory(apipath + "/NDREAMS/Aurora/Profiles");

                var ProfilePath = apipath + $"/NDREAMS/Aurora/Profiles/{name}.xml";

                if (File.Exists(ProfilePath))
                {
                    try
                    {
                        // Load the XML string
                        var xmlDoc = new XmlDocument();
                        xmlDoc.Load(ProfilePath);

                        // Get the <days> and <sessions> nodes
                        var daysNode = xmlDoc.SelectSingleNode("//days");
                        var sessionsNode = xmlDoc.SelectSingleNode("//sessions");

                        if (
                            daysNode != null
                            && sessionsNode != null
                            && int.TryParse(daysNode.InnerText, out var days)
                            && int.TryParse(sessionsNode.InnerText, out var sessions)
                        )
                        {
                            // Compare file creation date with current date
                            if (File.GetCreationTime(ProfilePath).Date != DateTime.Today)
                                // If the creation date is not today, increment days counter
                                daysNode.InnerText = (days + 1).ToString();

                            sessionsNode.InnerText = (sessions + 1).ToString();
                        }

                        File.WriteAllText(ProfilePath, xmlDoc.OuterXml);

                        CounterInfos = xmlDoc
                            .OuterXml.Replace("<xml>", string.Empty)
                            .Replace("</xml>", string.Empty);
                    }
                    catch (Exception ex)
                    {
                        CustomLogger.LoggerAccessor.LogError(
                            $"[AURORA] - visit Errored out while reading profile:{ProfilePath} with exception:{ex}"
                        );
                    }
                }
                else
                {
                    Prefix = "<new>true</new><first>true</first>";
                    File.WriteAllText(ProfilePath, "<xml>" + CounterInfos + "</xml>");
                }

                return $"<xml><result>{Prefix}<bonus>{bonus}</bonus>{CounterInfos}</result></xml>";
            }

            return null;
        }
    }
}
