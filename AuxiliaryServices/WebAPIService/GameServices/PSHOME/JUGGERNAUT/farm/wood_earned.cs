using System.Xml;

namespace WebAPIService.GameServices.PSHOME.JUGGERNAUT.farm
{
    public class wood_earned
    {
        public static string ProcessWoodEarned(
            IDictionary<string, string> QueryParameters,
            string apiPath
        )
        {
            if (QueryParameters != null)
            {
                var user = QueryParameters["user"];
                var amount = QueryParameters["amount"];

                if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(amount))
                {
                    Directory.CreateDirectory($"{apiPath}/juggernaut/farm/User_Data");

                    if (File.Exists($"{apiPath}/juggernaut/farm/User_Data/{user}.xml"))
                    {
                        // Load the XML string into an XmlDocument
                        var xmlDoc = new XmlDocument();
                        xmlDoc.Load($"{apiPath}/juggernaut/farm/User_Data/{user}.xml");

                        // Find the <wood> element

                        if (
                            xmlDoc.SelectSingleNode("/xml/resources/wood") is XmlElement woodElement
                        )
                        {
                            try
                            {
                                // Replace the value of <wood> with a new value
                                woodElement.InnerText = (
                                    int.Parse(woodElement.InnerText) + int.Parse(amount)
                                ).ToString();
                            }
                            catch (Exception)
                            {
                                // Not Important
                            }

                            File.WriteAllText(
                                $"{apiPath}/juggernaut/farm/User_Data/{user}.xml",
                                xmlDoc.OuterXml
                            );
                        }
                    }

                    return string.Empty;
                }
            }

            return null;
        }
    }
}
