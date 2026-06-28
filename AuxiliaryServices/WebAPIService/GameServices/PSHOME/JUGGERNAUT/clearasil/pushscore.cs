using System.Globalization;
using System.Xml;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.PSHOME.JUGGERNAUT.clearasil
{
    public class pushscore
    {
        public static readonly ClearasilScoreBoardData[] Leaderboards = [null, null];

        public static string ProcessPushScore(
            IDictionary<string, string> QueryParameters,
            string apiPath
        )
        {
            if (QueryParameters != null)
            {
                var user = QueryParameters["user"];
                var score = QueryParameters["score"];

                if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(score))
                {
                    Directory.CreateDirectory($"{apiPath}/juggernaut/clearasil/space_access");

                    var profilePath = $"{apiPath}/juggernaut/clearasil/space_access/{user}.xml";

                    if (File.Exists(profilePath))
                    {
                        // Load the XML string into an XmlDocument
                        var xmlDoc = new XmlDocument();
                        xmlDoc.Load(profilePath);

                        // Find the <score> element

                        if (xmlDoc.SelectSingleNode("/xml/score") is XmlElement scoreElement)
                        {
                            // Find the <phase2> element

                            if (xmlDoc.SelectSingleNode("/xml/phase2") is XmlElement phase2Element)
                            {
                                var phase2 = phase2Element.InnerText != "0";
                                try
                                {
                                    var increment = (int)
                                        double.Parse(score, CultureInfo.InvariantCulture);
                                    var existingscore = int.Parse(scoreElement.InnerText);
                                    var combinedscore = existingscore + increment;
                                    ClearasilScoreBoardData scoreboard;

                                    lock (Leaderboards)
                                    {
                                        scoreboard = Leaderboards[phase2 ? 1 : 0];

                                        if (scoreboard == null)
                                        {
                                            scoreboard = new ClearasilScoreBoardData(
                                                LeaderboardDbContext.BuildOptions(
                                                    0,
                                                    $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}"
                                                ),
                                                phase2 ? "phase2" : "phase1"
                                            );
                                            Leaderboards[phase2 ? 1 : 0] = scoreboard;
                                        }
                                    }

                                    _ = scoreboard.UpdateScoreAsync(user, combinedscore);

                                    // Replace the value of <score> with a new value
                                    scoreElement.InnerText = combinedscore.ToString();
                                }
                                catch (Exception ex)
                                {
                                    CustomLogger.LoggerAccessor.LogError(
                                        $"[pushscore] - Failed to update the user profile:{profilePath} with score:{scoreElement.InnerText}. (Exception:{ex})"
                                    );
                                }
                            }

                            File.WriteAllText(
                                $"{apiPath}/juggernaut/clearasil/space_access/{user}.xml",
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
