using CustomLogger;
using HttpMultipartParser;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Xml;
using WebAPIService.GameServices.HELLFIRE.Helpers.NovusPrime;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.HELLFIRE.Helpers
{
    public class Leaderboards
    {
        public static InterGalacticScoreBoardData NovusLeaderboard = null;

        public static string GetLeaderboardsClearasil(byte[] PostData, string boundary, string UserID, string WorkPath)
        {
            string path = $"{WorkPath}/ClearasilSkater/User_Data";

            string[] playerDataFiles = Directory.GetFiles(path);

            // Create an XmlDocument
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<Response><table type=\"table\" classname=\"ClearasilLeaderboards\"></table></Response>");

            foreach (var playerData in playerDataFiles)
            {
                if (!File.Exists(playerData))
                {
                    // If file doesn't exist continue foreach
                    continue;
                }

                // Load the XML file
                XmlDocument doc2 = new XmlDocument();
                string xmlProfile = File.ReadAllText(playerData);
                doc2.LoadXml("<root>" + xmlProfile + "</root>");

                // Get all LeaderboardScore elements
                XmlNodeList leaderboardScoreNodeList = doc2.GetElementsByTagName("LeaderboardScore");

                foreach(XmlNode lbScoreNode in leaderboardScoreNodeList)
                {
                    if (lbScoreNode != null && float.TryParse(lbScoreNode.InnerText, out float score))
                        // Use the score value here to display
                        doc.SelectSingleNode("//table").InnerXml += $"<DisplayName>{Path.GetFileNameWithoutExtension(playerData)}</DisplayName><LeaderboardScore>{score}</LeaderboardScore>";
                    else
                        LoggerAccessor.LogError($"[HFGAMEs] - LeaderboardScore element is incorrect: {lbScoreNode?.InnerText}.");
                }
            }

            return doc.OuterXml;
        }
        public static string GetLeaderboardsSlimJim(byte[] PostData, string boundary, string UserID, string WorkPath)
        {
            string path = $"{WorkPath}/SlimJim/User_Data";

            string[] playerDataFiles = Directory.GetFiles(path);

            // Create an XmlDocument
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<Response><table type=\"table\" classname=\"SlimJimLeaderboards\"></table></Response>");

            foreach (var playerData in playerDataFiles)
            {
                if (!File.Exists(playerData))
                {
                    //If file doesn't exist continue foreach
                    continue;
                }

                // Load the XML file
                XmlDocument doc2 = new XmlDocument();
                doc2.LoadXml("<root>" + File.ReadAllText(playerData) + "</root>");

                // Get all LeaderboardScore elements
                XmlNodeList leaderboardScoreNodeList = doc2.GetElementsByTagName("LeaderboardScore");

                foreach (XmlNode lbScoreNode in leaderboardScoreNodeList)
                {
                    if (lbScoreNode != null && float.TryParse(lbScoreNode.InnerText, out float score))
                        // Use the score value here to display
                        doc.SelectSingleNode("//table").InnerXml += $"<DisplayName>{Path.GetFileNameWithoutExtension(playerData)}</DisplayName><LeaderboardScore>{score}</LeaderboardScore>";
                    else
                        LoggerAccessor.LogError($"[HFGAMEs] - LeaderboardScore element is incorrect: {lbScoreNode?.InnerText}.");
                }
            }

            return doc.OuterXml;
        }

        public static string GetLeaderboardsNovusPrime(byte[] PostData, string boundary, string UserID, string WorkPath)
        {
            using (MemoryStream ms = new MemoryStream(PostData))
            {
                MultipartFormDataParser data = MultipartFormDataParser.Parse(ms, boundary);

                string UserNovusPrimeID = data.GetParameterValue("UserID");

                if (NovusLeaderboard == null)
                {
                    var retCtx = new LeaderboardDbContext(LeaderboardDbContext.OnContextBuilding(new DbContextOptionsBuilder<LeaderboardDbContext>(), 0, $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}").Options);

                    retCtx.Database.Migrate();

                    NovusLeaderboard = new InterGalacticScoreBoardData(retCtx);
                }

                return "<Response>" + NovusLeaderboard.SerializeToString("Root").Result ?? string.Empty + "</Response>";
            }
        }

        public static string GetGlobalPopulationLeaderboard(byte[] PostData, string boundary, string UserID, string WorkPath)
        {
            return @"<Response>
                            <Entry>
                                <DisplayName>AgentDark447</DisplayName>
                                <GlobalPop>10000</GlobalPop>
                            </Entry>
                            <Entry>
                                <DisplayName>JumpSuitDev</DisplayName>
                                <GlobalPop>9500</GlobalPop>
                            </Entry>
                    </Response>";
        }

        public static string GetGlobalRevenueCollectedLeaderboard(byte[] PostData, string boundary, string UserID, string WorkPath)
        {
            return @"<Response>
                            <Entry>
                                <DisplayName>AgentDark447</DisplayName>
                                <TotalCollected>10000</TotalCollected>
                            </Entry>
                            <Entry>
                                <DisplayName>JumpSuitDev</DisplayName>
                                <TotalCollected>9500</TotalCollected>
                            </Entry>
                    </Response>";
        }
    }
}
