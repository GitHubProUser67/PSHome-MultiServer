using System.Linq;
using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Linq;

namespace WebAPIService.GameServices.PSHOME.JUGGERNAUT.farm.plant
{
    public class plant_leveled
    {
        public static string ProcessLeveled(IDictionary<string, string> QueryParameters, string apiPath)
        {
            if (QueryParameters != null)
            {
                string user = QueryParameters["user"];
                string type = QueryParameters["type"];
                string id = QueryParameters["id"];
                string level = QueryParameters["level"];

                if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(level))
                {
                    Directory.CreateDirectory($"{apiPath}/juggernaut/farm/User_Data");

                    if (File.Exists($"{apiPath}/juggernaut/farm/User_Data/{user}.xml"))
                        File.WriteAllText($"{apiPath}/juggernaut/farm/User_Data/{user}.xml",
                                UpdateLevel(File.ReadAllText($"{apiPath}/juggernaut/farm/User_Data/{user}.xml"), id, type, level));

                    return string.Empty;
                }
            }

            return null;
        }

        private static string UpdateLevel(string xmlData, string id, string type, string newLevel)
        {
            try
            {
                XDocument xdoc = XDocument.Parse(xmlData);

                XElement plantToUpdate = xdoc.Descendants("plant")
                    .FirstOrDefault(a => a.Element("id")?.Value == id && a.Element("t")?.Value == type);

                if (plantToUpdate != null)
                    plantToUpdate.Element("l").Value = newLevel.ToString();

                return xdoc.ToString();
            }
            catch (Exception)
            {

            }

            return xmlData;
        }
    }
}
