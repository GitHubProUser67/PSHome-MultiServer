using System.Xml;
using System.Xml.Linq;

namespace WebAPIService.GameServices.PSHOME.JUGGERNAUT.farm.animal
{
    public class animal_sold
    {
        public static string ProcessSold(
            IDictionary<string, string> QueryParameters,
            string apiPath
        )
        {
            if (QueryParameters != null)
            {
                var user = QueryParameters["user"];
                var type = QueryParameters["type"];
                var id = QueryParameters["id"];
                var amount = QueryParameters["amount"];

                if (
                    !string.IsNullOrEmpty(user)
                    && !string.IsNullOrEmpty(type)
                    && !string.IsNullOrEmpty(id)
                    && !string.IsNullOrEmpty(amount)
                )
                {
                    Directory.CreateDirectory($"{apiPath}/juggernaut/farm/User_Data");

                    if (File.Exists($"{apiPath}/juggernaut/farm/User_Data/{user}.xml"))
                    {
                        // Load the XML string into an XmlDocument
                        var xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(
                            RemoveAnimalEntry(
                                File.ReadAllText($"{apiPath}/juggernaut/farm/User_Data/{user}.xml"),
                                type,
                                id
                            )
                        );

                        // Find the <gold> element

                        if (
                            xmlDoc.SelectSingleNode("/xml/resources/gold") is XmlElement goldElement
                        )
                        {
                            try
                            {
                                // Replace the value of <gold> with a new value
                                goldElement.InnerText = (
                                    int.Parse(goldElement.InnerText) + int.Parse(amount)
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

        private static string RemoveAnimalEntry(string xmlData, string type, string id)
        {
            var xdoc = XDocument.Parse(xmlData);

            var animalToRemove = xdoc.Descendants("animal")
                .FirstOrDefault(a => a.Element("t")?.Value == type && a.Element("id")?.Value == id);

            animalToRemove?.Remove();

            return xdoc.ToString();
        }
    }
}
