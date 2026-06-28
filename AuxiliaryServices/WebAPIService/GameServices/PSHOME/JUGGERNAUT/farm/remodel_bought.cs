using System.Xml;

namespace WebAPIService.GameServices.PSHOME.JUGGERNAUT.farm
{
    public class remodel_bought
    {
        public static string ProcessBought(
            IDictionary<string, string> QueryParameters,
            string apiPath
        )
        {
            if (QueryParameters != null)
            {
                var user = QueryParameters["user"];
                var type = QueryParameters["type"];
                var amount = QueryParameters["amount"];
                var wood = QueryParameters["wood"];

                if (
                    !string.IsNullOrEmpty(user)
                    && !string.IsNullOrEmpty(type)
                    && !string.IsNullOrEmpty(amount)
                    && !string.IsNullOrEmpty(wood)
                )
                {
                    Directory.CreateDirectory($"{apiPath}/juggernaut/farm/User_Data");

                    if (File.Exists($"{apiPath}/juggernaut/farm/User_Data/{user}.xml"))
                    {
                        // Load the XML string into an XmlDocument
                        var xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(
                            File.ReadAllText($"{apiPath}/juggernaut/farm/User_Data/{user}.xml")
                        );

                        // Find the <gold> element

                        if (
                            xmlDoc.SelectSingleNode("/xml/resources/gold") is XmlElement goldElement
                        )
                        {
                            try
                            {
                                var remaininggold =
                                    int.Parse(goldElement.InnerText) - int.Parse(amount);

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

                        if (
                            xmlDoc.SelectSingleNode("/xml/resources/wood") is XmlElement woodElement
                        )
                        {
                            try
                            {
                                var remainingwood =
                                    int.Parse(woodElement.InnerText) - int.Parse(wood);

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

                        File.WriteAllText(
                            $"{apiPath}/juggernaut/farm/User_Data/{user}.xml",
                            xmlDoc.OuterXml
                        );
                    }

                    return string.Empty;
                }
            }

            return null;
        }
    }
}
