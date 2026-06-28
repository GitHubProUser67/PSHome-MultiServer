using System.Xml;

namespace WebAPIService.GameServices.PSHOME.JUGGERNAUT.clearasil
{
    public class pushrewards
    {
        public const ushort TargetTime = 280;

        public static string ProcessPushRewards(
            IDictionary<string, string> QueryParameters,
            string apiPath
        )
        {
            if (QueryParameters != null)
            {
                var user = QueryParameters["user"];
                var reward1 = QueryParameters["reward1"];

                if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(reward1))
                {
                    Directory.CreateDirectory($"{apiPath}/juggernaut/clearasil/space_access");

                    if (File.Exists($"{apiPath}/juggernaut/clearasil/space_access/{user}.xml"))
                    {
                        // Load the XML string into an XmlDocument
                        var xmlDoc = new XmlDocument();
                        xmlDoc.Load($"{apiPath}/juggernaut/clearasil/space_access/{user}.xml");

                        // Find the <phase2> element

                        if (xmlDoc.SelectSingleNode("/xml/phase2") is XmlElement phase2Element)
                        {
                            // Replace the value of <phase2> with a new value
                            phase2Element.InnerText = reward1;
                            File.WriteAllText(
                                $"{apiPath}/juggernaut/clearasil/space_access/{user}.xml",
                                xmlDoc.OuterXml
                            );
                        }
                    }
                    else
                        File.WriteAllText(
                            $"{apiPath}/juggernaut/clearasil/space_access/{user}.xml",
                            $"<xml><seconds>{TargetTime}</seconds><phase2>{reward1}</phase2><score>0</score></xml>"
                        );

                    return string.Empty;
                }
            }

            return null;
        }
    }
}
