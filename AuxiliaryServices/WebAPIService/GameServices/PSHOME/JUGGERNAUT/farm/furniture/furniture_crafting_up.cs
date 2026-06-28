using System.Xml;

namespace WebAPIService.GameServices.PSHOME.JUGGERNAUT.farm.furniture
{
    public class furniture_crafting_up
    {
        public static string ProcessCraftingUp(
            IDictionary<string, string> QueryParameters,
            string apiPath
        )
        {
            if (QueryParameters != null)
            {
                var user = QueryParameters["user"];
                var level = QueryParameters["level"];
                var xp = QueryParameters["xp"];

                if (
                    !string.IsNullOrEmpty(user)
                    && !string.IsNullOrEmpty(level)
                    && !string.IsNullOrEmpty(xp)
                )
                {
                    Directory.CreateDirectory($"{apiPath}/juggernaut/farm/User_Data");

                    if (File.Exists($"{apiPath}/juggernaut/farm/User_Data/{user}.xml"))
                    {
                        // Load the XML string into an XmlDocument
                        var xmlDoc = new XmlDocument();
                        xmlDoc.Load($"{apiPath}/juggernaut/farm/User_Data/{user}.xml");

                        // Find the <level> element

                        if (
                            xmlDoc.SelectSingleNode("/xml/resources/level")
                            is XmlElement levelElement
                        )
                        {
                            // Replace the value of <level> with a new value
                            levelElement.InnerText = level;

                            // Find the <xp> element

                            if (
                                xmlDoc.SelectSingleNode("/xml/resources/xp") is XmlElement xpElement
                            )
                            {
                                // Replace the value of <xp> with a new value
                                xpElement.InnerText = xp;

                                File.WriteAllText(
                                    $"{apiPath}/juggernaut/farm/User_Data/{user}.xml",
                                    xmlDoc.OuterXml
                                );
                            }
                        }
                    }

                    return string.Empty;
                }
            }

            return null;
        }
    }
}
