using CustomLogger;
using HttpMultipartParser;
using System.IO;
using System.Xml;

namespace WebAPIService.GameServices.HELLFIRE.Helpers
{
    public class Leaderboards
    {
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
            string LBPath = $"{WorkPath}/NovusPrime/User_Data/Leaderboards.xml";

            #region Template LB to start with
            /*string templateLB = @"<Root><GalaxyRider42>
                                    <DisplayName>GalaxyRider42</DisplayName>
                                    <Score>0</Score>
                                </GalaxyRider42>
                                <ShadowHunterX>
                                    <DisplayName>ShadowHunterX</DisplayName>
                                    <Score>0</Score>
                                </ShadowHunterX>
                                <NeonNova77>
                                    <DisplayName>NeonNova77</DisplayName>
                                    <Score>0</Score>
                                </NeonNova77>
                                <IronWraith89>
                                    <DisplayName>IronWraith89</DisplayName>
                                    <Score>0</Score>
                                </IronWraith89>
                                <PixelPhantom5>
                                    <DisplayName>PixelPhantom5</DisplayName>
                                    <Score>0</Score>
                                </PixelPhantom5>
                                <BlazeFalconer>
                                    <DisplayName>BlazeFalconer</DisplayName>
                                    <Score>0</Score>
                                </BlazeFalconer>
                                <QuantumLynx92>
                                    <DisplayName>QuantumLynx92</DisplayName>
                                    <Score>0</Score>
                                </QuantumLynx92>
                                <HyperBolt13>
                                    <DisplayName>HyperBolt13</DisplayName>
                                    <Score>0</Score>
                                </HyperBolt13>
                                <FrostEcho99>
                                    <DisplayName>FrostEcho99</DisplayName>
                                    <Score>0</Score>
                                </FrostEcho99>
                                <VoidTrekker11>
                                    <DisplayName>VoidTrekker11</DisplayName>
                                    <Score>0</Score>
                                </VoidTrekker11></Root>";*/
            #endregion

            using (MemoryStream ms = new MemoryStream(PostData))
            {
                MultipartFormDataParser data = MultipartFormDataParser.Parse(ms, boundary);

                string UserNovusPrimeID = data.GetParameterValue("UserID");

                // Create an XmlDocument
                XmlDocument doc = new XmlDocument();

                if (File.Exists(LBPath))
                    doc.LoadXml($"<Response>{File.ReadAllText(LBPath)}</Response>");
                else
                    doc.LoadXml($"<Response></Response>");

                return doc.InnerXml;
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
