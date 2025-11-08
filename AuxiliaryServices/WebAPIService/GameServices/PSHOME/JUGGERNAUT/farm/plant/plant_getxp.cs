using System.IO;
using System.Collections.Generic;
namespace WebAPIService.GameServices.PSHOME.JUGGERNAUT.farm.plant
{
    public class plant_getxp
    {
        public static string ProcessGetXp(IDictionary<string, string> QueryParameters, string apiPath)
        {
            if (QueryParameters != null)
            {
                string user = QueryParameters["user"];

                if (!string.IsNullOrEmpty(user))
                {
                    Directory.CreateDirectory($"{apiPath}/juggernaut/farm/User_Data");

                    if (File.Exists($"{apiPath}/juggernaut/farm/User_Data/{user}.xml"))
                        return File.ReadAllText($"{apiPath}/juggernaut/farm/User_Data/{user}.xml");
                    else
                        return "<xml><found>0</found></xml>";
                }
            }

            return null;
        }
    }
}
