using System.Xml;

namespace WebAPIService.GameServices.PSHOME.JUGGERNAUT.farm.furniture
{
    public class furniture_slot_saved
    {
        public static string ProcessSlotSaved(
            IDictionary<string, string> QueryParameters,
            string apiPath
        )
        {
            if (QueryParameters != null)
            {
                var user = QueryParameters["user"];
                var slot = QueryParameters["slot"];

                if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(slot))
                {
                    Directory.CreateDirectory($"{apiPath}/juggernaut/farm/User_Data");

                    if (File.Exists($"{apiPath}/juggernaut/farm/User_Data/{user}.xml"))
                    {
                        // Load the XML string into an XmlDocument
                        var xmlDoc = new XmlDocument();
                        xmlDoc.Load($"{apiPath}/juggernaut/farm/User_Data/{user}.xml");

                        // Find the <lastLayout> element

                        if (
                            xmlDoc.SelectSingleNode("/xml/resources/lastLayout")
                            is XmlElement lastLayoutElement
                        )
                        {
                            // Replace the value of <lastLayout> with a new value
                            lastLayoutElement.InnerText = slot;

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
