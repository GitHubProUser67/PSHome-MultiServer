using System;
using System.IO;
using MultiServerLibrary.HTTP;
using HttpMultipartParser;
using System.Xml;

namespace WebAPIService.GameServices.PSHOME.NDREAMS.Aurora
{
    public static class visit
    {
        public static string ProcessVisit(byte[] PostData, string ContentType, string apipath)
        {
            string friends = string.Empty;
            string name = string.Empty;
            string age = string.Empty;
            string bonus = string.Empty;
            string boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    friends = data.GetParameterValue("friends");
                    name = data.GetParameterValue("name");
                    age = data.GetParameterValue("age");
                    bonus = data.GetParameterValue("bonus");

                    ms.Flush();
                }

                string CounterInfos = "<days>0</days><sessions>1</sessions>";

                string Prefix = "<new>false</new><first>false</first>";

                Directory.CreateDirectory(apipath + "/NDREAMS/Aurora/Profiles");

                string ProfilePath = apipath + $"/NDREAMS/Aurora/Profiles/{name}.xml";

                if (File.Exists(ProfilePath))
                {
                    try
                    {
                        // Load the XML string
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(ProfilePath);

                        // Get the <days> and <sessions> nodes
                        XmlNode daysNode = xmlDoc.SelectSingleNode("//days");
                        XmlNode sessionsNode = xmlDoc.SelectSingleNode("//sessions");

                        if (daysNode != null && sessionsNode != null && int.TryParse(daysNode.InnerText, out int days) && int.TryParse(sessionsNode.InnerText, out int sessions))
                        {
                            // Compare file creation date with current date
                            if (File.GetCreationTime(ProfilePath).Date != DateTime.Today)
                                // If the creation date is not today, increment days counter
                                daysNode.InnerText = (days + 1).ToString();

                            sessionsNode.InnerText = (sessions + 1).ToString();
                        }

                        File.WriteAllText(ProfilePath, xmlDoc.OuterXml);

                        CounterInfos = xmlDoc.OuterXml.Replace("<xml>", string.Empty).Replace("</xml>", string.Empty);
                    }
                    catch (Exception ex)
                    {
                        CustomLogger.LoggerAccessor.LogError($"[AURORA] - visit Errored out while reading profile:{ProfilePath} with exception:{ex}");
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
