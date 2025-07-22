using System.Linq;
using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Linq;

namespace WebAPIService.GameServices.JUGGERNAUT.farm.plant
{
    public class plant_watered
    {
        public static string ProcessWatered(IDictionary<string, string> QueryParameters, string apiPath)
        {
            if (QueryParameters != null)
            {
                string user = QueryParameters["user"];
                string type = QueryParameters["type"];
                string id = QueryParameters["id"];
                string posix = QueryParameters["posix"];

                if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(posix))
                {
                    Directory.CreateDirectory($"{apiPath}/juggernaut/farm/User_Data");

                    if (File.Exists($"{apiPath}/juggernaut/farm/User_Data/{user}.xml"))
                        File.WriteAllText($"{apiPath}/juggernaut/farm/User_Data/{user}.xml",
                                UpdateWateredAttributes(File.ReadAllText($"{apiPath}/juggernaut/farm/User_Data/{user}.xml"), id, type, posix));

                    return string.Empty;
                }
            }

            return null;
        }

        private static string UpdateWateredAttributes(string xmlData, string id, string type, string posix)
        {
            try
            {
                XDocument xdoc = XDocument.Parse(xmlData);

                XElement plantToWatered = xdoc.Descendants("plant")
                    .FirstOrDefault(a => a.Element("id")?.Value == id && a.Element("t")?.Value == type);

                if (plantToWatered != null)
                    plantToWatered.Element("lw").Value = posix;

                return xdoc.ToString();
            }
            catch (Exception)
            {

            }

            return xmlData;
        }
    }
}
