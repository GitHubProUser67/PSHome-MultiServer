using System.Xml.Linq;

namespace WebAPIService.GameServices.PSHOME.JUGGERNAUT.farm.plant
{
    public class plant_watered
    {
        public static string ProcessWatered(
            IDictionary<string, string> QueryParameters,
            string apiPath
        )
        {
            if (QueryParameters != null)
            {
                var user = QueryParameters["user"];
                var type = QueryParameters["type"];
                var id = QueryParameters["id"];
                var posix = QueryParameters["posix"];

                if (
                    !string.IsNullOrEmpty(user)
                    && !string.IsNullOrEmpty(type)
                    && !string.IsNullOrEmpty(id)
                    && !string.IsNullOrEmpty(posix)
                )
                {
                    Directory.CreateDirectory($"{apiPath}/juggernaut/farm/User_Data");

                    if (File.Exists($"{apiPath}/juggernaut/farm/User_Data/{user}.xml"))
                        File.WriteAllText(
                            $"{apiPath}/juggernaut/farm/User_Data/{user}.xml",
                            UpdateWateredAttributes(
                                File.ReadAllText($"{apiPath}/juggernaut/farm/User_Data/{user}.xml"),
                                id,
                                type,
                                posix
                            )
                        );

                    return string.Empty;
                }
            }

            return null;
        }

        private static string UpdateWateredAttributes(
            string xmlData,
            string id,
            string type,
            string posix
        )
        {
            try
            {
                var xdoc = XDocument.Parse(xmlData);

                var plantToWatered = xdoc.Descendants("plant")
                    .FirstOrDefault(a =>
                        a.Element("id")?.Value == id && a.Element("t")?.Value == type
                    );

                plantToWatered?.Element("lw").Value = posix;

                return xdoc.ToString();
            }
            catch (Exception) { }

            return xmlData;
        }
    }
}
