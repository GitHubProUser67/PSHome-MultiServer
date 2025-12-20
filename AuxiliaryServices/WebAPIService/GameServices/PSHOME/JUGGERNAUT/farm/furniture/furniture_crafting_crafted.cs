using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;

namespace WebAPIService.GameServices.PSHOME.JUGGERNAUT.farm.furniture
{
    public class furniture_crafting_crafted
    {
        public static string ProcessCraftingCrafted(IDictionary<string, string> QueryParameters, string apiPath)
        {
            if (QueryParameters != null)
            {
                string user = QueryParameters["user"];
                string gold = QueryParameters["gold"];
                string wood = QueryParameters["wood"];

                if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(gold) && !string.IsNullOrEmpty(wood))
                {
                    Directory.CreateDirectory($"{apiPath}/juggernaut/farm/User_Data");

                    if (File.Exists($"{apiPath}/juggernaut/farm/User_Data/{user}.xml"))
                    {
                        // Load the XML string into an XmlDocument
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load($"{apiPath}/juggernaut/farm/User_Data/{user}.xml");

                        // Find the <gold> element
                        XmlElement goldElement = xmlDoc.SelectSingleNode("/xml/resources/gold") as XmlElement;

                        if (goldElement != null)
                        {
                            // Replace the value of <gold> with a new value
                            goldElement.InnerText = gold;

                            // Find the <wood> element
                            XmlElement woodElement = xmlDoc.SelectSingleNode("/xml/resources/wood") as XmlElement;

                            if (woodElement != null)
                            {
                                try
                                {
                                    int woodtoremove = int.Parse(woodElement.InnerText) - int.Parse(wood);

                                    if (woodtoremove < 0 )
                                        woodtoremove = 0;

                                    // Replace the value of <wood> with a new value
                                    woodElement.InnerText = woodtoremove.ToString();
                                }
                                catch (Exception)
                                {

                                }

                                File.WriteAllText($"{apiPath}/juggernaut/farm/User_Data/{user}.xml", xmlDoc.OuterXml);
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
