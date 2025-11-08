using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.PSHOME.JUGGERNAUT.clearasil
{
    public class pushscore
    {
        public static readonly ClearasilScoreBoardData[] Leaderboards = new ClearasilScoreBoardData[2] { null, null };

        public static string ProcessPushScore(IDictionary<string, string> QueryParameters, string apiPath)
        {
            if (QueryParameters != null)
            {
                string user = QueryParameters["user"];
                string score = QueryParameters["score"];

                if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(score))
                {
                    Directory.CreateDirectory($"{apiPath}/juggernaut/clearasil/space_access");

                    string profilePath = $"{apiPath}/juggernaut/clearasil/space_access/{user}.xml";

                    if (File.Exists(profilePath))
                    {
                        // Load the XML string into an XmlDocument
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(profilePath);

                        // Find the <score> element
                        XmlElement scoreElement = xmlDoc.SelectSingleNode("/xml/score") as XmlElement;

                        if (scoreElement != null)
                        {
                            // Find the <phase2> element
                            XmlElement phase2Element = xmlDoc.SelectSingleNode("/xml/phase2") as XmlElement;

                            if (phase2Element != null)
                            {
                                bool phase2 = phase2Element.InnerText != "0";
                                try
                                {
                                    int increment = (int)double.Parse(score, CultureInfo.InvariantCulture);
                                    int existingscore = int.Parse(scoreElement.InnerText);
                                    int combinedscore = existingscore + increment;
                                    ClearasilScoreBoardData scoreboard;

                                    lock (Leaderboards)
                                    {
                                        scoreboard = Leaderboards[phase2 ? 1 : 0];

                                        if (scoreboard == null)
                                        {
                                            scoreboard = new ClearasilScoreBoardData(LeaderboardDbContext.OnContextBuilding(new DbContextOptionsBuilder<LeaderboardDbContext>(), 0, $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}").Options, phase2 ? "phase2" : "phase1");
                                            Leaderboards[phase2 ? 1 : 0] = scoreboard;
                                        }
                                    }

                                    _ = scoreboard.UpdateScoreAsync(user, combinedscore);

                                    // Replace the value of <score> with a new value
                                    scoreElement.InnerText = combinedscore.ToString();
                                }
                                catch (Exception ex)
                                {
                                    CustomLogger.LoggerAccessor.LogError($"[pushscore] - Failed to update the user profile:{profilePath} with score:{scoreElement.InnerText}. (Exception:{ex})");
                                }
                            }

                            File.WriteAllText($"{apiPath}/juggernaut/clearasil/space_access/{user}.xml", xmlDoc.OuterXml);
                        }
                    }

                    return string.Empty;
                }
            }

            return null;
        }
    }
}
