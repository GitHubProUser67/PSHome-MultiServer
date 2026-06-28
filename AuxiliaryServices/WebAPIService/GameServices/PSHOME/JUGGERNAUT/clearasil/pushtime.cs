using System.Xml;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.PSHOME.JUGGERNAUT.clearasil
{
    public class pushtime
    {
        public static string ProcessPushTime(
            IDictionary<string, string> QueryParameters,
            string apiPath
        )
        {
            if (QueryParameters != null)
            {
                var user = QueryParameters["user"];
                var time = QueryParameters["time"];

                if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(time))
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
                            var phase2 = phase2Element.InnerText != "0";
                            ClearasilScoreBoardData scoreboard;

                            lock (pushscore.Leaderboards)
                            {
                                scoreboard = pushscore.Leaderboards[phase2 ? 1 : 0];

                                if (scoreboard == null)
                                {
                                    scoreboard = new ClearasilScoreBoardData(
                                        LeaderboardDbContext.BuildOptions(
                                            0,
                                            $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}"
                                        ),
                                        phase2 ? "phase2" : "phase1"
                                    );
                                    pushscore.Leaderboards[phase2 ? 1 : 0] = scoreboard;
                                }
                            }

                            _ = scoreboard.AddTimeAsync(user, time);
                        }
                    }

                    return string.Empty;
                }
            }

            return null;
        }
    }
}
