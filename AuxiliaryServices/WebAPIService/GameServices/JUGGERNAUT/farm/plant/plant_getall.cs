using System.IO;
using System.Collections.Generic;
namespace WebAPIService.GameServices.JUGGERNAUT.farm.plant
{
    public class plant_getall
    {
        public static string ProcessGetAll(IDictionary<string, string> QueryParameters, string apiPath)
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
                    {
                        File.WriteAllText($"{apiPath}/juggernaut/farm/User_Data/{user}.xml",
                                $"<xml><found>1</found><resources><wood>0</wood><gold>0</gold><weather>0</weather><level>0</level>" +
                                "<xp>0</xp><lastLayout>0</lastLayout><remodel>1</remodel></resources><animals></animals><plants></plants></xml>");

                        return "<xml><found>0</found></xml>";
                    }
                }
            }

            return null;
        }
    }
}
