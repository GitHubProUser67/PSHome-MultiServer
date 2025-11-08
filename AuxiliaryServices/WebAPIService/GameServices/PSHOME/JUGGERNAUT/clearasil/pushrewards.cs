using System.IO;
using System.Collections.Generic;
using System.Xml;

namespace WebAPIService.GameServices.PSHOME.JUGGERNAUT.clearasil
{
    public class pushrewards
    {
        public static string ProcessPushRewards(IDictionary<string, string> QueryParameters, string apiPath)
        {
            if (QueryParameters != null)
            {
                string user = QueryParameters["user"];
                string reward1 = QueryParameters["reward1"];

                if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(reward1))
                {
                    Directory.CreateDirectory($"{apiPath}/juggernaut/clearasil/space_access");

                    if (File.Exists($"{apiPath}/juggernaut/clearasil/space_access/{user}.xml"))
                    {
                        // Load the XML string into an XmlDocument
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load($"{apiPath}/juggernaut/clearasil/space_access/{user}.xml");

                        // Find the <phase2> element
                        XmlElement phase2Element = xmlDoc.SelectSingleNode("/xml/phase2") as XmlElement;

                        if (phase2Element != null)
                        {
                            // Replace the value of <phase2> with a new value
                            phase2Element.InnerText = reward1;
                            File.WriteAllText($"{apiPath}/juggernaut/clearasil/space_access/{user}.xml", xmlDoc.OuterXml);
                        }
                    }
                    else
                        File.WriteAllText($"{apiPath}/juggernaut/clearasil/space_access/{user}.xml"
                                , $"<xml><seconds>500</seconds><phase2>{reward1}</phase2><score>0</score></xml>");

                    return string.Empty;
                }
            }

            return null;
        }
    }
}
