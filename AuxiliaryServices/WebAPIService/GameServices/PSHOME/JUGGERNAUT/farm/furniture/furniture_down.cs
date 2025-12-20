using System.IO;
using System.Collections.Generic;

namespace WebAPIService.GameServices.PSHOME.JUGGERNAUT.farm.furniture
{
    public class furniture_down
    {
        public static string ProcessDown(IDictionary<string, string> QueryParameters, string apiPath)
        {
            if (QueryParameters != null)
            {
                string user = QueryParameters["user"];
                string layout = QueryParameters["layout"];

                if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(layout))
                {
                    if (File.Exists($"{apiPath}/juggernaut/farm/User_Data/{user}/{layout}.xml"))
                        return File.ReadAllText($"{apiPath}/juggernaut/farm/User_Data/{user}/{layout}.xml");

                    return string.Empty;
                }
            }

            return null;
        }
    }
}
