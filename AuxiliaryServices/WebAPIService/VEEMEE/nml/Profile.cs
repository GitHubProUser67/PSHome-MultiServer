using System.IO;
using NetworkLibrary.HTTP;
using CustomLogger;
using HttpMultipartParser;
using System.Collections.Specialized;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Linq;

namespace WebAPIService.VEEMEE.nml
{
    public class Profile
    {
        public static string Verify(byte[] PostData, string ContentType)
        {
            string config = string.Empty;
            string product = string.Empty;
            string boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);
					
					LoggerAccessor.LogInfo($"[VEEMEE] - Verify Details: POSTDATA: \n{Encoding.UTF8.GetString(PostData)}");


                    ms.Flush();
                }

                return "1,,0,0,0,0,1";
            }

            return null;
        }

        public static string Reward(byte[] PostData, string ContentType)
        {
            string config = string.Empty;
            string product = string.Empty;
            string boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    LoggerAccessor.LogInfo($"[VEEMEE] - Reward POSTDATA: {Encoding.UTF8.GetString(PostData)}");

                    ms.Flush();
                }

                return "1,1,1,1,1,1,1,1,1,1";
            }

            return null;
        }

        public static string Get(byte[] PostData, string ContentType, string apiPath)
        {

            if (PostData != null && ContentType == "application/x-www-form-urlencoded")
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var data = HTTPProcessor.ExtractAndSortUrlEncodedPOSTData(PostData);
                    string game = data["game"].First();
                    string psnid = data["psnid"].First();

                    Directory.CreateDirectory($"{apiPath}/VEEMEE/nml/User_Data");

                    string xmlProfile = string.Empty;

                    if (File.Exists($"{apiPath}/VEEMEE/nml/User_Data/{psnid}.xml"))
                    {

                        // Load the XML string into an XmlDocument
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml($"{File.ReadAllText($"{apiPath}/VEEMEE/nml/User_Data/{psnid}.xml")}");

                        ms.Flush();
                        xmlProfile = xmlDoc.OuterXml;
                    }
                    else
                    {
                        string XmlData = $"<profiles>\r\n\t<player psnid_id=\"{RandomNumberGenerator.Create(psnid)}\" />\r\n\t<game game_id=\"{game}\" /><variable name=\"init\" type=\"bool\">false</variable>\r\n</profiles>";
                        File.WriteAllText($"{apiPath}/VEEMEE/nml/User_Data/{psnid}.xml", XmlData);



                        ms.Flush();
                        xmlProfile = XmlData;
                    }

                    return xmlProfile;
                }
            }

            return null;
        }

        public static string Set(byte[] PostData, string ContentType, string apiPath)
        {

            if (ContentType == "application/x-www-form-urlencoded" && PostData != null)
            {
                LoggerAccessor.LogInfo($"[VEEMEE] SetProfile - PSOTDATA: \n{Encoding.UTF8.GetString(PostData)}");

                var data = HTTPProcessor.ExtractAndSortUrlEncodedPOSTData(PostData);

                string psnid = data["psnid"].First();
                string game = data["game"].First();

                string profilePath = $"{apiPath}/VEEMEE/nml/User_Data" + $"/{psnid}.xml";
                Directory.CreateDirectory(apiPath);

                if (File.Exists(apiPath))
                {
                    LoggerAccessor.LogInfo("Attempting to load profile xml from API...");

                    // Create an XDocument from the XML content
                    XDocument xmlDoc = XDocument.Parse($"{File.ReadAllText(profilePath)}");


                    // Decode the URL-encoded string
                    string xmlContent = WebUtility.UrlDecode(Encoding.UTF8.GetString(PostData));

                    XmlDocument doc = new XmlDocument();
                    doc.Load(apiPath);

                    XmlNode profilesNode = doc.SelectSingleNode("//profiles");

                    // Check for existing variable entries and overwrite or add new ones
                    XmlNodeList variableNodes = profilesNode.SelectNodes("//variable");
                    foreach (XmlNode variableNode in variableNodes)
                    {
                        string name = variableNode.Attributes["name"].Value;
                        string guidValue = string.Empty;
                        XmlNodeList XmlNodeList = variableNode.SelectNodes("///value");
                        foreach (XmlNode valueNode in variableNode)
                        {
                            guidValue = valueNode.Value;
                        }

                        if (name == xmlContent.Contains($"name={name}").ToString())
                        {
                            variableNode.Attributes["value"].Value = name;
                        }
                        else // Add new variable
                        {
                            XmlElement newVariable = doc.CreateElement("variable");
                            newVariable.SetAttribute("name", name);
                            newVariable.SetAttribute("type", "guid");
                            newVariable.SetAttribute("value", guidValue);
                            profilesNode.AppendChild(newVariable);
                        }
                    }

                    // Check for existing variable entries and overwrite or add new ones
                    XmlNodeList listNodes = profilesNode.SelectNodes("//list");
                    foreach (XmlNode listNode in listNodes)
                    {
                        string name = listNode.Attributes["name"].Value;
                        string guidValue = string.Empty;
                        XmlNodeList XmlNodeList = listNode.SelectNodes("///value");
                        foreach(XmlNode valueNode in listNode)
                        {
                            guidValue = valueNode.Value;
                        }

                        if (name == xmlContent.Contains($"name={name}").ToString())
                        {
                            listNode.Attributes["value"].Value = name;
                        }
                        else // Add new variable
                        {
                            XmlElement newVariable = doc.CreateElement("variable");
                            newVariable.SetAttribute("name", name);
                            newVariable.SetAttribute("type", "guid");
                            newVariable.SetAttribute("value", guidValue);
                            profilesNode.AppendChild(newVariable);
                        }
                    }

                    doc.Save(profilePath);
                    return doc.OuterXml;
                }
                else
                {

                    LoggerAccessor.LogInfo("File does not exist. Creating a new file...");

                    // Decode the URL-encoded string
                    string xmlContent = WebUtility.UrlDecode(Encoding.UTF8.GetString(PostData));

                    // Create an XDocument from the XML content
                    XDocument xmlDoc = XDocument.Parse(xmlContent);

                    // Create the final XML
                    XElement profiles =
                        new XElement("profiles",
                            new XElement("game", new XAttribute("game_id", "profile")),
                            new XElement("player", new XAttribute("psnid_id", RandomNumberGenerator.Create(psnid))),
                            from var in xmlDoc.Descendants("variable")
                            select var,
                            from list in xmlDoc.Descendants("list")
                            select list
                        );

                    // Save the XML to a file
                    string outputPath = profilePath + $"/{psnid}.xml";
                    profiles.Save(outputPath);
                    
                    return xmlDoc.ToString();
                }
            }

            return null;
        }

        static bool VariableExists(XmlElement rootElement, string nameValue)
        {
            return rootElement.SelectNodes($"//variable[@name='{nameValue}']").Count > 0;
        }

        static string CreateNewProfileFile(string postData, string fileName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement rootElement = xmlDoc.CreateElement("profile");
            xmlDoc.AppendChild(rootElement);

            // Parse POST data
#if NETFRAMEWORK
            var postDataCollection = HTTPProcessor.GetQueryParameters(postData);

            // Create a new entry and append it to the root element
            XmlElement newEntry = xmlDoc.CreateElement("variable");
            rootElement.AppendChild(newEntry);

            // Add params and values to the new entry
            foreach (var entry in postDataCollection)
            {
                XmlElement paramElement = xmlDoc.CreateElement(entry.Key);
                paramElement.SetAttribute("type", "string"); // You may change this based on the actual type
                paramElement.InnerText = entry.Value;
                newEntry.AppendChild(paramElement);
            }
#else
            NameValueCollection postDataCollection = System.Web.HttpUtility.ParseQueryString(postData);

            // Create a new entry and append it to the root element
            XmlElement newEntry = xmlDoc.CreateElement("variable");
            rootElement.AppendChild(newEntry);

            // Add params and values to the new entry
            for (int i = 0; i < postDataCollection.Count; i++)
            {
                XmlElement paramElement = xmlDoc.CreateElement(postDataCollection.GetKey(i));
                paramElement.SetAttribute("type", "string"); // You may change this based on the actual type
                paramElement.InnerText = postDataCollection.Get(i);
                newEntry.AppendChild(paramElement);
            }
#endif
            // Save the XML to file
            xmlDoc.Save(fileName);
            LoggerAccessor.LogInfo("New XML file created with the entry.");
            return xmlDoc.OuterXml;
        }
    }
}
