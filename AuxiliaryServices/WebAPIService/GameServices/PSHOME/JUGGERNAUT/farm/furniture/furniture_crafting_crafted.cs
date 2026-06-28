using System.Xml;

namespace WebAPIService.GameServices.PSHOME.JUGGERNAUT.farm.furniture
{
    public class furniture_crafting_crafted
    {
        public static string ProcessCraftingCrafted(
            IDictionary<string, string> QueryParameters,
            string apiPath
        )
        {
            if (QueryParameters != null)
            {
                var user = QueryParameters["user"];
                var gold = QueryParameters["gold"];
                var wood = QueryParameters["wood"];

                if (
                    !string.IsNullOrEmpty(user)
                    && !string.IsNullOrEmpty(gold)
                    && !string.IsNullOrEmpty(wood)
                )
                {
                    Directory.CreateDirectory($"{apiPath}/juggernaut/farm/User_Data");

                    if (File.Exists($"{apiPath}/juggernaut/farm/User_Data/{user}.xml"))
                    {
                        // Load the XML string into an XmlDocument
                        var xmlDoc = new XmlDocument();
                        xmlDoc.Load($"{apiPath}/juggernaut/farm/User_Data/{user}.xml");

                        // Find the <gold> element

                        if (
                            xmlDoc.SelectSingleNode("/xml/resources/gold") is XmlElement goldElement
                        )
                        {
                            // Replace the value of <gold> with a new value
                            goldElement.InnerText = gold;

                            // Find the <wood> element

                            if (
                                xmlDoc.SelectSingleNode("/xml/resources/wood")
                                is XmlElement woodElement
                            )
                            {
                                try
                                {
                                    var woodtoremove =
                                        int.Parse(woodElement.InnerText) - int.Parse(wood);

                                    if (woodtoremove < 0)
                                        woodtoremove = 0;

                                    // Replace the value of <wood> with a new value
                                    woodElement.InnerText = woodtoremove.ToString();
                                }
                                catch (Exception) { }

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
