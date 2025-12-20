using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;

namespace WebAPIService.GameServices.PSHOME.JUGGERNAUT.farm.plant
{
    public class plant_pushxp
    {
        public static string ProcessPushXp(IDictionary<string, string> QueryParameters, string apiPath)
        {
            if (QueryParameters != null)
            {
                string user = QueryParameters["user"];
                string level = QueryParameters["level"];
                string xp = QueryParameters["xp"];

                if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(level) && !string.IsNullOrEmpty(xp))
                {
                    Directory.CreateDirectory($"{apiPath}/juggernaut/farm/User_Data");

                    if (File.Exists($"{apiPath}/juggernaut/farm/User_Data/{user}.xml"))
                    {
                        // Load the XML string into an XmlDocument
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load($"{apiPath}/juggernaut/farm/User_Data/{user}.xml");

                        // Find the <level> element
                        XmlElement levelElement = xmlDoc.SelectSingleNode("/xml/resources/level") as XmlElement;

                        if (levelElement != null)
                        {
                            try
                            {
                                // Replace the value of <level> with a new value
                                levelElement.InnerText = level;
                            }
                            catch (Exception)
                            {
                                // Not Important
                            }

                            // Find the <xp> element
                            XmlElement xpElement = xmlDoc.SelectSingleNode("/xml/resources/xp") as XmlElement;

                            if (xpElement != null)
                            {
                                try
                                {
                                    // Replace the value of <xp> with a new value
                                    xpElement.InnerText = xp;
                                }
                                catch (Exception)
                                {
                                    // Not Important
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
