using System.Linq;
using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Linq;

namespace WebAPIService.GameServices.PSHOME.JUGGERNAUT.farm.animal
{
    public class animal_fed
    {
        public static string ProcessFed(IDictionary<string, string> QueryParameters, string apiPath)
        {
            if (QueryParameters != null)
            {
                string permBoost = null;
                string user = QueryParameters["user"];
                string type = QueryParameters["type"];
                string id = QueryParameters["id"];
                string posix = QueryParameters["posix"];
                try
                {
                    permBoost = QueryParameters["permBoost"];
                }
                catch
                {
                }

                if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(posix))
                {
                    Directory.CreateDirectory($"{apiPath}/juggernaut/farm/User_Data");

                    if (string.IsNullOrEmpty(permBoost) && posix.Contains("="))
                    {
                        const string permDelimiter = "permBoost=";
                        int permBoostIndex = posix.IndexOf(permDelimiter);
                        if (permBoostIndex != -1)
                        {
                            permBoost = posix.Substring(permBoostIndex + permDelimiter.Length);
                            posix = posix.Substring(0, permBoostIndex);
                        }
                    }

                    if (File.Exists($"{apiPath}/juggernaut/farm/User_Data/{user}.xml"))
                        File.WriteAllText($"{apiPath}/juggernaut/farm/User_Data/{user}.xml",
                                UpdateFedAttributes(File.ReadAllText($"{apiPath}/juggernaut/farm/User_Data/{user}.xml"), id, type, posix, permBoost));

                    return string.Empty;
                }
            }

            return null;
        }

        private static string UpdateFedAttributes(string xmlData, string id, string type, string posix, string permBoost)
        {
            try
            {
                XDocument xdoc = XDocument.Parse(xmlData);

                XElement animalToUpdate = xdoc.Descendants("animal")
                    .FirstOrDefault(a => a.Element("id")?.Value == id && a.Element("t")?.Value == type);

                if (animalToUpdate != null)
                {
                    animalToUpdate.Element("lf").Value = posix;
                    animalToUpdate.Element("pbu").Value = permBoost;
                }

                return xdoc.ToString();
            }
            catch (Exception)
            {

            }

            return xmlData;
        }
    }
}
