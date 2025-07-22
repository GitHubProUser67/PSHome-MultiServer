using System.Linq;
using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Linq;

namespace WebAPIService.GameServices.JUGGERNAUT.farm.animal
{
    public class animal_renewed
    {
        public static string ProcessRenewed(IDictionary<string, string> QueryParameters, string apiPath)
        {
            if (QueryParameters != null)
            {
                string user = QueryParameters["user"];
                string type = QueryParameters["type"];
                string id = QueryParameters["id"];

                if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(id))
                {
                    Directory.CreateDirectory($"{apiPath}/juggernaut/farm/User_Data");

                    if (File.Exists($"{apiPath}/juggernaut/farm/User_Data/{user}.xml"))
                        File.WriteAllText($"{apiPath}/juggernaut/farm/User_Data/{user}.xml",
                                UpdateTbu(File.ReadAllText($"{apiPath}/juggernaut/farm/User_Data/{user}.xml"), id, type));

                    return string.Empty;
                }
            }

            return null;
        }

        private static string UpdateTbu(string xmlData, string id, string type)
        {
            try
            {
                XDocument xdoc = XDocument.Parse(xmlData);

                XElement animalToUpdate = xdoc.Descendants("animal")
                    .FirstOrDefault(a => a.Element("id")?.Value == id && a.Element("t")?.Value == type);

                if (animalToUpdate != null)
                    animalToUpdate.Element("tbu").Value = "1";

                return xdoc.ToString();
            }
            catch (Exception)
            {

            }

            return xmlData;
        }
    }
}
