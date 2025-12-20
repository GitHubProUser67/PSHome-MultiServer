using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;

namespace WebAPIService.GameServices.PSHOME.JUGGERNAUT.farm.furniture
{
    public class furniture_crafting_sold
    {
        public static string ProcessCraftingSold(IDictionary<string, string> QueryParameters, string apiPath)
        {
            if (QueryParameters != null)
            {
                string user = QueryParameters["user"];
                string amount = QueryParameters["amount"];

                if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(amount))
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
                            try
                            {
                                // Replace the value of <gold> with a new value
                                goldElement.InnerText = (int.Parse(goldElement.InnerText) + int.Parse(amount)).ToString();
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
    }
}
