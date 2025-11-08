using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace WebAPIService.GameServices.PSHOME.JUGGERNAUT.farm
{
    public class remodel_bought
    {
        public static string ProcessBought(IDictionary<string, string> QueryParameters, string apiPath)
        {
            if (QueryParameters != null)
            {
                string user = QueryParameters["user"];
                string type = QueryParameters["type"];
                string amount = QueryParameters["amount"];
                string wood = QueryParameters["wood"];

                if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(amount) && !string.IsNullOrEmpty(wood))
                {
                    Directory.CreateDirectory($"{apiPath}/juggernaut/farm/User_Data");

                    if (File.Exists($"{apiPath}/juggernaut/farm/User_Data/{user}.xml"))
                    {
                        // Load the XML string into an XmlDocument
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(File.ReadAllText($"{apiPath}/juggernaut/farm/User_Data/{user}.xml"));

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
                        }

                        // Find the <wood> element
                        XmlElement woodElement = xmlDoc.SelectSingleNode("/xml/resources/wood") as XmlElement;

                        if (woodElement != null)
                        {
                            try
                            {
                                int remainingwood = int.Parse(woodElement.InnerText) - int.Parse(wood);

                                if (remainingwood < 0)
                                    remainingwood = 0;

                                // Replace the value of <wood> with a new value
                                woodElement.InnerText = remainingwood.ToString();
                            }
                            catch (Exception)
                            {
                                // Not Important
                            }
                        }

                        File.WriteAllText($"{apiPath}/juggernaut/farm/User_Data/{user}.xml", xmlDoc.OuterXml);
                    }

                    return string.Empty;
                }
            }

            return null;
        }
    }
}
