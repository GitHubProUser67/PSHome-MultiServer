using System.Xml;
using System.Xml.Linq;

namespace WebAPIService.GameServices.PSHOME.JUGGERNAUT.farm.animal
{
    public class animal_bought
    {
        public static string ProcessBought(
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
                            AddAnimalEntry(
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
                                var remaininggold =
                                    int.Parse(goldElement.InnerText) - int.Parse(amount);

                                if (remaininggold < 0)
                                    remaininggold = 0;

                                // Replace the value of <gold> with a new value
                                goldElement.InnerText = remaininggold.ToString();
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

        private static string AddAnimalEntry(string xmlData, string type, string id)
        {
            var xdoc = XDocument.Parse(xmlData);

            var newAnimal = new XElement(
                "animal",
                new XElement("t", type),
                new XElement("l", 1),
                new XElement("lf", 0),
                new XElement("id", id),
                new XElement("lastFeedTime", 0),
                new XElement("pbu", 0),
                new XElement("tbu", 0)
            );

            xdoc.Descendants("animals").First().Add(newAnimal);

            return xdoc.ToString();
        }
    }
}
