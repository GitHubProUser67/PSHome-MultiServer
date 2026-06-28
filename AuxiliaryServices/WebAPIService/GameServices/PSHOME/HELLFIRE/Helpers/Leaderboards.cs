using System.Xml;
using CustomLogger;
using HttpMultipartParser;
using WebAPIService.GameServices.PSHOME.HELLFIRE.Helpers.NovusPrime;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.PSHOME.HELLFIRE.Helpers
{
    public class Leaderboards
    {
        public static InterGalacticScoreBoardData NovusLeaderboard = null;

        public static string GetLeaderboardsClearasil(
            byte[] PostData,
            string boundary,
            string UserID,
            string WorkPath
        )
        {
            var path = $"{WorkPath}/ClearasilSkater/User_Data";

            var playerDataFiles = Directory.GetFiles(path);

            // Create an XmlDocument
            var doc = new XmlDocument();
            doc.LoadXml(
                "<Response><table type=\"table\" classname=\"ClearasilLeaderboards\"></table></Response>"
            );

            foreach (var playerData in playerDataFiles)
            {
                if (!File.Exists(playerData))
                {
                    // If file doesn't exist continue foreach
                    continue;
                }

                // Load the XML file
                var doc2 = new XmlDocument();
                var xmlProfile = File.ReadAllText(playerData);
                doc2.LoadXml("<root>" + xmlProfile + "</root>");

                // Get all LeaderboardScore elements
                var leaderboardScoreNodeList = doc2.GetElementsByTagName("LeaderboardScore");

                foreach (XmlNode lbScoreNode in leaderboardScoreNodeList)
                {
                    if (lbScoreNode != null && float.TryParse(lbScoreNode.InnerText, out var score))
                        // Use the score value here to display
                        doc.SelectSingleNode("//table").InnerXml +=
                            $"<DisplayName>{Path.GetFileNameWithoutExtension(playerData)}</DisplayName><LeaderboardScore>{score}</LeaderboardScore>";
                    else
                        LoggerAccessor.LogError(
                            $"[HFGAMEs] - LeaderboardScore element is incorrect: {lbScoreNode?.InnerText}."
                        );
                }
            }

            return doc.OuterXml;
        }

        public static string GetLeaderboardsSlimJim(
            byte[] PostData,
            string boundary,
            string UserID,
            string WorkPath
        )
        {
            var path = $"{WorkPath}/SlimJim/User_Data";

            var playerDataFiles = Directory.GetFiles(path);

            // Create an XmlDocument
            var doc = new XmlDocument();
            doc.LoadXml(
                "<Response><table type=\"table\" classname=\"SlimJimLeaderboards\"></table></Response>"
            );

            foreach (var playerData in playerDataFiles)
            {
                if (!File.Exists(playerData))
                {
                    //If file doesn't exist continue foreach
                    continue;
                }

                // Load the XML file
                var doc2 = new XmlDocument();
                doc2.LoadXml("<root>" + File.ReadAllText(playerData) + "</root>");

                // Get all LeaderboardScore elements
                var leaderboardScoreNodeList = doc2.GetElementsByTagName("LeaderboardScore");

                foreach (XmlNode lbScoreNode in leaderboardScoreNodeList)
                {
                    if (lbScoreNode != null && float.TryParse(lbScoreNode.InnerText, out var score))
                        // Use the score value here to display
                        doc.SelectSingleNode("//table").InnerXml +=
                            $"<DisplayName>{Path.GetFileNameWithoutExtension(playerData)}</DisplayName><LeaderboardScore>{score}</LeaderboardScore>";
                    else
                        LoggerAccessor.LogError(
                            $"[HFGAMEs] - LeaderboardScore element is incorrect: {lbScoreNode?.InnerText}."
                        );
                }
            }

            return doc.OuterXml;
        }

        public static string GetLeaderboardsNovusPrime(
            byte[] PostData,
            string boundary,
            string UserID,
            string WorkPath
        )
        {
            using (var ms = new MemoryStream(PostData))
            {
                var data = MultipartFormDataParser.Parse(ms, boundary);

                var UserNovusPrimeID = data.GetParameterValue("UserID");

                NovusLeaderboard ??= new InterGalacticScoreBoardData(
                    LeaderboardDbContext.BuildOptions(
                        0,
                        $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}"
                    )
                );

                return "<Response>" + NovusLeaderboard.SerializeToString("Root").Result
                    ?? string.Empty + "</Response>";
            }
        }

        public static string GetGlobalPopulationLeaderboard(
            byte[] PostData,
            string boundary,
            string UserID,
            string WorkPath
        )
        {
            // TODO
            return @"<Response>
                    <1><DisplayName>Not Implemented yet!</DisplayName><GlobalPop>0</GlobalPop></1>
                    </Response>";
        }

        public static string GetGlobalRevenueCollectedLeaderboard(
            byte[] PostData,
            string boundary,
            string UserID,
            string WorkPath
        )
        {
            // TODO
            return @"<Response>
                    <1><DisplayName>Not Implemented yet!</DisplayName><TotalCollected>0</TotalCollected></1>
                    </Response>";
        }
    }
}
