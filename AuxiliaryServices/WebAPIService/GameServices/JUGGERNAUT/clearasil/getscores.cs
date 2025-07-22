using System.IO;
using System.Collections.Generic;
namespace WebAPIService.GameServices.JUGGERNAUT.clearasil
{
    public class getscores
    {
        public static string ProcessGetScores(IDictionary<string, string> QueryParameters, string apiPath)
        {
            if (QueryParameters != null)
            {
                string phase = QueryParameters["phase"];

                if (!string.IsNullOrEmpty(phase))
                {
                    Directory.CreateDirectory($"{apiPath}/juggernaut/clearasil");

                    if (File.Exists($"{apiPath}/juggernaut/clearasil/scoreboard.xml"))
                        return File.ReadAllText($"{apiPath}/juggernaut/clearasil/scoreboard.xml");
					
                    return "<xml></xml>";
                }
            }

            return null;
        }
    }
}
