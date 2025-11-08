using System.Linq;
using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml;

namespace WebAPIService.GameServices.PSHOME.JUGGERNAUT.farm.plant
{
    public class plant_bought
    {
        public static string ProcessBought(IDictionary<string, string> QueryParameters, string apiPath)
        {
            if (QueryParameters != null)
            {
                string user = QueryParameters["user"];
                string type = QueryParameters["type"];
                string id = QueryParameters["id"];
                string amount = QueryParameters["amount"];

                if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(amount))
                {
                    Directory.CreateDirectory($"{apiPath}/juggernaut/farm/User_Data");

                    if (File.Exists($"{apiPath}/juggernaut/farm/User_Data/{user}.xml"))
                    {
                        // Load the XML string into an XmlDocument
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(AddPlantEntry(File.ReadAllText($"{apiPath}/juggernaut/farm/User_Data/{user}.xml"), type, id));

                        // Find the <gold> element
                        XmlElement goldElement = xmlDoc.SelectSingleNode("/xml/resources/gold") as XmlElement;

                        if (goldElement != null)
                        {
                            try
                            {
                                int remaininggold = int.Parse(goldElement.InnerText) - int.Parse(amount);

                                if (remaininggold < 0)
                                    remaininggold = 0;

                                // Replace the value of <gold> with a new value
                                goldElement.InnerText = remaininggold.ToString();
                            }
                            catch (Exception)
                            {
                                // Not Important
                            }

                            File.WriteAllText($"{apiPath}/juggernaut/farm/User_Data/{user}.xml", xmlDoc.OuterXml);
                        }
                    }

                    return string.Empty;
                }
            }

            return null;
        }

        private static string AddPlantEntry(string xmlData, string type, string id)
        {
            XDocument xdoc = XDocument.Parse(xmlData);

            XElement newAnimal = new XElement("plant",
                new XElement("t", type),
                new XElement("l", 1),
                new XElement("id", id),
                new XElement("lw", 0),
                new XElement("pbu", 0),
                new XElement("tbu", 0)
            );

            xdoc.Descendants("plants").First().Add(newAnimal);

            return xdoc.ToString();
        }
    }
}
