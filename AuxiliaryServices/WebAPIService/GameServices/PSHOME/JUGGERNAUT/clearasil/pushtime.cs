using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using WebAPIService.LeaderboardService;
namespace WebAPIService.GameServices.PSHOME.JUGGERNAUT.clearasil
{
    public class pushtime
    {
        public static string ProcessPushTime(IDictionary<string, string> QueryParameters, string apiPath)
        {
            if (QueryParameters != null)
            {
                string user = QueryParameters["user"];
                string time = QueryParameters["time"];

                if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(time))
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
                            bool phase2 = phase2Element.InnerText != "0";
                            ClearasilScoreBoardData scoreboard;

                            lock (pushscore.Leaderboards)
                            {
                                scoreboard = pushscore.Leaderboards[phase2 ? 1 : 0];

                                if (scoreboard == null)
                                {
                                    scoreboard = new ClearasilScoreBoardData(LeaderboardDbContext.OnContextBuilding(new DbContextOptionsBuilder<LeaderboardDbContext>(), 0, $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}").Options, phase2 ? "phase2" : "phase1");
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
