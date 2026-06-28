using System.Xml;

namespace WebAPIService.GameServices.PSHOME.JUGGERNAUT.farm.animal
{
    public class animal_collect_renew
    {
        public static string ProcessCollectRenew(
            IDictionary<string, string> QueryParameters,
            string apiPath
        )
        {
            if (QueryParameters != null)
            {
                var user = QueryParameters["user"];
                var amount = QueryParameters["amount"];

                if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(amount))
                {
                    Directory.CreateDirectory($"{apiPath}/juggernaut/farm/User_Data");

                    if (File.Exists($"{apiPath}/juggernaut/farm/User_Data/{user}.xml"))
                    {
                        // Load the XML string into an XmlDocument
                        var xmlDoc = new XmlDocument();
                        xmlDoc.Load($"{apiPath}/juggernaut/farm/User_Data/{user}.xml");

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

                            File.WriteAllText(
                                $"{apiPath}/juggernaut/farm/User_Data/{user}.xml",
                                xmlDoc.OuterXml
                            );
                        }
                    }

                    return string.Empty;
                }
            }

            return null;
        }
    }
}
