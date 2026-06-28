using System.Xml.Linq;

namespace WebAPIService.GameServices.PSHOME.JUGGERNAUT.farm.animal
{
    public class animal_renewed
    {
        public static string ProcessRenewed(
            IDictionary<string, string> QueryParameters,
            string apiPath
        )
        {
            if (QueryParameters != null)
            {
                var user = QueryParameters["user"];
                var type = QueryParameters["type"];
                var id = QueryParameters["id"];

                if (
                    !string.IsNullOrEmpty(user)
                    && !string.IsNullOrEmpty(type)
                    && !string.IsNullOrEmpty(id)
                )
                {
                    Directory.CreateDirectory($"{apiPath}/juggernaut/farm/User_Data");

                    if (File.Exists($"{apiPath}/juggernaut/farm/User_Data/{user}.xml"))
                        File.WriteAllText(
                            $"{apiPath}/juggernaut/farm/User_Data/{user}.xml",
                            UpdateTbu(
                                File.ReadAllText($"{apiPath}/juggernaut/farm/User_Data/{user}.xml"),
                                id,
                                type
                            )
                        );

                    return string.Empty;
                }
            }

            return null;
        }

        private static string UpdateTbu(string xmlData, string id, string type)
        {
            try
            {
                var xdoc = XDocument.Parse(xmlData);

                var animalToUpdate = xdoc.Descendants("animal")
                    .FirstOrDefault(a =>
                        a.Element("id")?.Value == id && a.Element("t")?.Value == type
                    );

                animalToUpdate?.Element("tbu").Value = "1";

                return xdoc.ToString();
            }
            catch (Exception) { }

            return xmlData;
        }
    }
}
