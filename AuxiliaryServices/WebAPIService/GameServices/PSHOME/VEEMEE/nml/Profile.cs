using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using HttpMultipartParser;
using MultiServerLibrary.HTTP;

namespace WebAPIService.GameServices.PSHOME.VEEMEE.nml
{
    public class Profile
    {
        public static string Verify(byte[] PostData, string ContentType)
        {
            var boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (var ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    ms.Flush();
                }

                return "1,,0,0,0,0,1";
            }

            return null;
        }

        public static string Reward(byte[] PostData, string ContentType)
        {
            var boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (var ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

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
                using (var ms = new MemoryStream(PostData))
                {
                    var data = HTTPProcessor.ExtractAndSortUrlEncodedPOSTData(PostData);
                    var game = data["game"].First();
                    var psnid = data["psnid"].First();

                    Directory.CreateDirectory($"{apiPath}/VEEMEE/nml/User_Data");

                    var xmlProfile = string.Empty;

                    if (File.Exists($"{apiPath}/VEEMEE/nml/User_Data/{psnid}.xml"))
                    {
                        // Load the XML string into an XmlDocument
                        var xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(
                            $"{File.ReadAllText($"{apiPath}/VEEMEE/nml/User_Data/{psnid}.xml")}"
                        );

                        xmlProfile = xmlDoc.OuterXml;
                    }
                    else
                    {
                        var XmlData =
                            $"<profiles>\r\n\t<player psnid_id=\"{RandomNumberGenerator.Create(psnid)}\" />\r\n\t<game game_id=\"{game}\" /><variable name=\"init\" type=\"bool\">false</variable>\r\n</profiles>";
                        File.WriteAllText($"{apiPath}/VEEMEE/nml/User_Data/{psnid}.xml", XmlData);

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
                var data = HTTPProcessor.ExtractAndSortUrlEncodedPOSTData(PostData);

                var psnid = data["psnid"].First();
                var game = data["game"].First();

                var profilePath = $"{apiPath}/VEEMEE/nml/User_Data" + $"/{psnid}.xml";
                Directory.CreateDirectory(apiPath);

                if (File.Exists(apiPath))
                {
                    // Create an XDocument from the XML content
                    var xmlDoc = XDocument.Parse($"{File.ReadAllText(profilePath)}");

                    // Decode the URL-encoded string
                    var xmlContent = WebUtility.UrlDecode(Encoding.UTF8.GetString(PostData));

                    var doc = new XmlDocument();
                    doc.Load(apiPath);

                    var profilesNode = doc.SelectSingleNode("//profiles");

                    // Check for existing variable entries and overwrite or add new ones
                    var variableNodes = profilesNode.SelectNodes("//variable");
                    foreach (XmlNode variableNode in variableNodes)
                    {
                        var name = variableNode.Attributes["name"].Value;
                        var guidValue = string.Empty;
                        var XmlNodeList = variableNode.SelectNodes("///value");
                        foreach (XmlNode valueNode in variableNode)
                            guidValue = valueNode.Value;

                        if (name == xmlContent.Contains($"name={name}").ToString())
                            variableNode.Attributes["value"].Value = name;
                        else // Add new variable
                        {
                            var newVariable = doc.CreateElement("variable");
                            newVariable.SetAttribute("name", name);
                            newVariable.SetAttribute("type", "guid");
                            newVariable.SetAttribute("value", guidValue);
                            profilesNode.AppendChild(newVariable);
                        }
                    }

                    // Check for existing variable entries and overwrite or add new ones
                    var listNodes = profilesNode.SelectNodes("//list");
                    foreach (XmlNode listNode in listNodes)
                    {
                        var name = listNode.Attributes["name"].Value;
                        var guidValue = string.Empty;
                        var XmlNodeList = listNode.SelectNodes("///value");
                        foreach (XmlNode valueNode in listNode)
                            guidValue = valueNode.Value;

                        if (name == xmlContent.Contains($"name={name}").ToString())
                            listNode.Attributes["value"].Value = name;
                        else // Add new variable
                        {
                            var newVariable = doc.CreateElement("variable");
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
                    // Decode the URL-encoded string
                    var xmlContent = WebUtility.UrlDecode(Encoding.UTF8.GetString(PostData));

                    // Create an XDocument from the XML content
                    var xmlDoc = XDocument.Parse(xmlContent);

                    // Create the final XML
                    var profiles = new XElement(
                        "profiles",
                        new XElement("game", new XAttribute("game_id", "profile")),
                        new XElement(
                            "player",
                            new XAttribute("psnid_id", RandomNumberGenerator.Create(psnid))
                        ),
                        from var in xmlDoc.Descendants("variable")
                        select var,
                        from list in xmlDoc.Descendants("list")
                        select list
                    );

                    // Save the XML to a file
                    var outputPath = profilePath + $"/{psnid}.xml";
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
            var xmlDoc = new XmlDocument();
            var rootElement = xmlDoc.CreateElement("profile");
            xmlDoc.AppendChild(rootElement);

            // Parse POST data
            var postDataCollection = System.Web.HttpUtility.ParseQueryString(postData);

            // Create a new entry and append it to the root element
            var newEntry = xmlDoc.CreateElement("variable");
            rootElement.AppendChild(newEntry);

            // Add params and values to the new entry
            for (var i = 0; i < postDataCollection.Count; i++)
            {
                var paramElement = xmlDoc.CreateElement(postDataCollection.GetKey(i));
                paramElement.SetAttribute("type", "string"); // You may change this based on the actual type
                paramElement.InnerText = postDataCollection.Get(i);
                newEntry.AppendChild(paramElement);
            }
            // Save the XML to file
            xmlDoc.Save(fileName);
            return xmlDoc.OuterXml;
        }
    }
}
