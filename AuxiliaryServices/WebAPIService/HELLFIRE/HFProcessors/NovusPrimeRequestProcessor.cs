using CustomLogger;
using HttpMultipartParser;
using MultiServerLibrary.HTTP;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using WebAPIService.HELLFIRE.Helpers;
using XI5;

namespace WebAPIService.HELLFIRE.HFProcessors
{
    public class NovusPrimeRequestProcessor
    {
        public static string ProcessMainPHP(byte[] PostData, string ContentType, string PHPSessionID, string WorkPath)
        {
            if (PostData == null || string.IsNullOrEmpty(ContentType))
                return null;

            string Command = string.Empty;
            string UserID = string.Empty;
            string DisplayName = string.Empty;
            string InstanceID = string.Empty;
            string Region = string.Empty;
            string Achievements = string.Empty;
            string Type = string.Empty;
            string Str = string.Empty;
            string Amount = string.Empty;

            string boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (boundary != null)
            {
                byte[] ticketData = null;

                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    foreach (var file in data.Files)
                    {
                        using (Stream filedata = file.Data)
                        {
                            filedata.Position = 0;

                            // Find the number of bytes in the stream
                            int contentLength = (int)filedata.Length;

                            // Create a byte array
                            byte[] buffer = new byte[contentLength];

                            // Read the contents of the memory stream into the byte array
                            filedata.Read(buffer, 0, contentLength);

                            if (file.FileName == "ticket.bin")
                                ticketData = buffer;

                            filedata.Flush();
                        }
                    }

                    if (ticketData != null && ticketData.Length > 188)
                    {
                        const string RPCNSigner = "RPCN";

                        // get ticket
                        XI5Ticket ticket = XI5Ticket.ReadFromBytes(ticketData);

                        // setup username
                        string username = ticket.Username;

                        // invalid ticket
                        if (!ticket.Valid)
                        {
                            // log to console
                            LoggerAccessor.LogWarn($"[HFGames] - NovusPrime : User {username} tried to alter their ticket data");

                            return null;
                        }

                        // RPCN
                        if (ticket.SignatureIdentifier == RPCNSigner)
                            LoggerAccessor.LogInfo($"[HFGames] - NovusPrime : User {username} connected at: {DateTime.Now} and is on RPCN");
                        else if (username.EndsWith($"@{RPCNSigner}"))
                        {
                            LoggerAccessor.LogError($"[HFGames] - NovusPrime : User {username} was caught using a RPCN suffix while not on it!");

                            return null;
                        }
                        else
                            LoggerAccessor.LogInfo($"[HFGames] - NovusPrime : User {username} connected at: {DateTime.Now} and is on PSN");
                    }

                    Command = data.GetParameterValue("Command");
                    UserID = data.GetParameterValue("UserId");

                    LoggerAccessor.LogInfo($"[HFGAMES] Command detected as {Command}");

                    try
                    {
                        DisplayName = data.GetParameterValue("DisplayName");
                    }
                    catch
                    {
                        // Not Important.
                    }
                    try
                    {
                        InstanceID = data.GetParameterValue("InstanceID");
                    }
                    catch
                    {
                        // Not Important.
                    }
                    try
                    {
                        Region = data.GetParameterValue("Region");
                    }
                    catch
                    {
                        // Not Important.
                    }
                    try
                    {
                        Achievements = data.GetParameterValue("Achievements");
                    }
                    catch
                    {
                        // Not Important.
                    }

                    try
                    {
                        //for Log cmd
                        Type = data.GetParameterValue("Type");
                        Str = data.GetParameterValue("Str");
                        Amount = data.GetParameterValue("Amount");
                    }
                    catch
                    {
                        // Not Important.
                    }

                    ms.Flush();
                }

                if (!string.IsNullOrEmpty(Command))
                {
                    Directory.CreateDirectory($"{WorkPath}/SlimJim/User_Data");
                    Directory.CreateDirectory($"{WorkPath}/NovusPrime/User_Data");
                    Directory.CreateDirectory($"{WorkPath}/NovusPrime/galactic_scores");

                    switch (Command)
                    {
                        case "RequestNPTicket":
                            return NPTicket.RequestNPTicket(PostData, boundary);
                        case "RequestInitialData":
                            return User.RequestInitialDataNovusPrime(PostData, boundary, UserID, WorkPath);
                        case "UpdateUser":
                            return User.UpdateUserSlimJim(PostData, boundary, UserID, WorkPath);
                        case "RequestScoreLeaderboard":
                            return Leaderboards.GetLeaderboardsNovusPrime(PostData, boundary, UserID, WorkPath);
                        case "RequestSpecificLeaderboard":
                            DateTime refdate = DateTime.Now; // We avoid race conditions by calculating it one time.

                            switch (Type)
                            {
                                case "Day":
                                    if (File.Exists($"{WorkPath}/NovusPrime/galactic_scores/leaderboard_{refdate:yyyy_MM_dd}.xml"))
                                        return File.ReadAllText($"{WorkPath}/NovusPrime/galactic_scores/leaderboard_{refdate:yyyy_MM_dd}.xml");
                                    break;
                                case "Week":
                                    if (Directory.Exists($"{WorkPath}/NovusPrime/galactic_scores"))
                                    {
                                        // Get all XML files in the scoreboard folder
                                        foreach (string file in Directory.GetFiles($"{WorkPath}/NovusPrime/galactic_scores", "leaderboard_weekly_*.xml"))
                                        {
                                            // Extract date from the filename
                                            Match match = Regex.Match(file, @"leaderboard_weekly_(\d{4}_\d{2}_\d{2}).xml");
                                            if (match.Success)
                                            {
                                                string fileDate = match.Groups[1].Value;
                                                // Parse the file date
                                                if (DateTime.TryParseExact(fileDate, "yyyy_MM_dd",
                                                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fileDateTime))
                                                {
                                                    // Check if the file is newer than 7 days
                                                    double diff = (refdate - fileDateTime).TotalDays;
                                                    if (diff <= 7)
                                                    {
                                                        string weekScoreBoardPath = $"{WorkPath}/NovusPrime/galactic_scores/leaderboard_weekly_{refdate.AddDays(-diff):yyyy_MM_dd}.xml";
                                                        if (File.Exists(weekScoreBoardPath))
                                                            return File.ReadAllText(weekScoreBoardPath);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    break;
                                case "Month":
                                    if (File.Exists($"{WorkPath}/NovusPrime/galactic_scores/leaderboard_monthly_{refdate:yyyy_MM}.xml"))
                                        return File.ReadAllText($"{WorkPath}/NovusPrime/galactic_scores/leaderboard_monthly_{refdate:yyyy_MM}.xml");
                                    break;
                            }
                            return "<Response></Response>";
                        case "CompleteMission":
                            return User.NovusCompleteMission(PostData, boundary, UserID, WorkPath);
                        case "RequestCharacter":
                            return User.UpdateCharacter(PostData, boundary, UserID, WorkPath, Command);
                        case "UpdateCharacter":
                            return User.UpdateCharacter(PostData, boundary, UserID, WorkPath, Command);
                        case "ReadShipConfigData":
                            return "<Response></Response>";
                        case "ConfigureShip":
                            return User.UpdateCharacter(PostData, boundary, UserID, WorkPath, Command);
                        case "QueryServerGlobals":
                        case "RequestStore":
                            return "<Response></Response>";
                        case "QueryBoosterItems":
                            return "<Response><Booster><BoostedStat>2</BoostedStat><ObjectID>BCAA547B-5908430F-AF823FFA-74247EE3</ObjectID><Percent>10</Percent></Booster>" +
                                "<Booster><BoostedStat>2</BoostedStat><ObjectID>E81B2444-121A49D3-8D9CC3F0-37222CCE</ObjectID><Percent>10</Percent></Booster>" +
                                "<Booster><BoostedStat>2</BoostedStat><ObjectID>32AA025A-AB73423C-A08821E3-FB2D6064</ObjectID><Percent>5</Percent></Booster>" +
                                "<Booster><BoostedStat>2</BoostedStat><ObjectID>A6A6D7DB-FE7D45E0-8136A766-69AC32E7</ObjectID><Percent>5</Percent></Booster>" +
                                "<Booster><BoostedStat>2</BoostedStat><ObjectID>1DAAF0AB-76AA44F9-A99D220F-F905C4FB</ObjectID><Percent>5</Percent></Booster>" +
                                "<Booster><BoostedStat>2</BoostedStat><ObjectID>3D670654-672A4186-B9C6293D-DEEFD0DD</ObjectID><Percent>5</Percent></Booster>" +
                                "<Booster><BoostedStat>2</BoostedStat><ObjectID>61D5310E-6A5F41EE-850E3AAC-E7F18903</ObjectID><Percent>5</Percent></Booster>" +
                                "<Booster><BoostedStat>2</BoostedStat><ObjectID>6368E019-07544E1D-9F1355C3-D89C965A</ObjectID><Percent>5</Percent></Booster>" +
                                "<Booster><BoostedStat>2</BoostedStat><ObjectID>7A1B6AFE-17E0415C-AC55E187-6D4D2009</ObjectID><Percent>5</Percent></Booster>" +
                                "<Booster><BoostedStat>2</BoostedStat><ObjectID>8213F77B-83E744D8-951A452F-67A4C4D0</ObjectID><Percent>5</Percent></Booster>" +
                                "<Booster><BoostedStat>2</BoostedStat><ObjectID>860D2539-CAB44560-8A98ECA4-4EA4B183</ObjectID><Percent>5</Percent></Booster>" +
                                "<Booster><BoostedStat>2</BoostedStat><ObjectID>AF9ACCEF-D41042B7-8582771C-D89AD2D5</ObjectID><Percent>5</Percent></Booster>" +
                                "<Booster><BoostedStat>2</BoostedStat><ObjectID>D715417B-EBD04C14-9064B5FD-C5642AF8</ObjectID><Percent>5</Percent></Booster>" +
                                "<Booster><BoostedStat>2</BoostedStat><ObjectID>D7387B09-B1FB41E6-A0FDF8CB-151C2B15</ObjectID><Percent>5</Percent></Booster>" +
                                "<Booster><BoostedStat>1</BoostedStat><ObjectID>21AA481D-571F46DF-A55A4B0D-CDFB1BB9</ObjectID><Percent>5</Percent></Booster>" +
                                "<Booster><BoostedStat>1</BoostedStat><ObjectID>A05D2C30-E7E74C1C-BFB7D1B5-4876EB94</ObjectID><Percent>5</Percent></Booster>" +
                                "<Booster><BoostedStat>1</BoostedStat><ObjectID>4054DC7E-14654310-8D312A09-7A18619A</ObjectID><Percent>5</Percent></Booster>" +
                                "<Booster><BoostedStat>1</BoostedStat><ObjectID>C8107A83-437F4DA7-81B5C459-865A4259</ObjectID><Percent>5</Percent></Booster>" +
                                "<Booster><BoostedStat>1</BoostedStat><ObjectID>6E6143AB-C89A4B50-8BC9B3A1-CC601F90</ObjectID><Percent>5</Percent></Booster>" +
                                "</Response>";
                        case "QueryAchievements":
                            if (File.Exists($"{WorkPath}/NovusPrime/User_Data/{UserID}_Achievements.xml"))
                                return $"<Response>{File.ReadAllText($"{WorkPath}/NovusPrime/User_Data/{UserID}_Achievements.xml")}</Response>";
                            return "<Response></Response>";
                        case "CompleteAchievements":
                            StringBuilder achievementsSt = new StringBuilder();
                            foreach (string achievement in Achievements.Split(','))
                            {
                                if (!string.IsNullOrEmpty(achievement))
                                    achievementsSt.Append($"<Achievement><AchievementID>{achievement}</AchievementID></Achievement>");
                            }
                            File.WriteAllText($"{WorkPath}/NovusPrime/User_Data/{UserID}_Achievements.xml", achievementsSt.ToString());
                            return "<Response></Response>";
                        case "QueryAchievementData":
                            StringBuilder achievementDataSt = new StringBuilder("<Response>");
                            for (byte missionId = 0; missionId < 25; missionId++)
                            {
                                achievementDataSt.Append($"<Achievement><TriggerType>1</TriggerType><IntParam>{missionId}</IntParam><AchievementID>MISSION_{missionId}_WON</AchievementID></Achievement>");
                            }
                            achievementDataSt.Append("</Response>");
                            return achievementDataSt.ToString();
                        case "QueryLevelRewards":
                            return @"<Response>
                                      <Item>
                                        <Level>2</Level>
                                        <Rewards>FDB5983B-4DA44581-8D0E29EB-6B7F908A,89DB3FAA-511B4641-B2941C02-25297E1D</Rewards>
                                      </Item>
                                      <Item>
                                        <Level>4</Level>
                                        <Rewards>993D934A-48A142FB-A9C80D52-0BC4B446</Rewards>
                                      </Item>
                                      <Item>
                                        <Level>6</Level>
                                        <Rewards>44196AF1-D92A42F9-9F276D28-77390F0A,EB0A944F-037B4623-9A9E6986-A0BAE584</Rewards>
                                      </Item>
                                      <Item>
                                        <Level>8</Level>
                                        <Rewards>AF8EA713-C9854143-9190735E-D1B496AC</Rewards>
                                      </Item>
                                      <Item>
                                        <Level>10</Level>
                                        <Rewards>BE40021F-26BE4554-8BA62B02-E7DA8268</Rewards>
                                      </Item>
                                      <Item>
                                        <Level>12</Level>
                                        <Rewards>320CC5A5-F4414CB0-966AF8FD-EEF7D704,386B537E-770E4FEC-8E4DBE51-65328F11</Rewards>
                                      </Item>
                                      <Item>
                                        <Level>14</Level>
                                        <Rewards>DE8E2772-0DB24A2F-B41C9F81-757624ED</Rewards>
                                      </Item>
                                      <Item>
                                        <Level>16</Level>
                                        <Rewards>6C06981F-41324F71-857E6395-823189C9</Rewards>
                                      </Item>
                                      <Item>
                                        <Level>18</Level>
                                        <Rewards>8ACA6ACA-93194386-8A5AD137-A4D79973</Rewards>
                                      </Item>
                                      <Item>
                                        <Level>20</Level>
                                        <Rewards>AF0DE7A4-51954D71-BBE9418A-6DC050C8,E80009A1-CC0C4A8A-84856F21-E38A8D22</Rewards>
                                      </Item>
                                      <Item>
                                        <Level>22</Level>
                                        <Rewards>54C0B2EF-EE0D4BE1-BF256495-51B8F6CC,C0A1E986-65764AC5-9E4E1520-11C0AE42,D80CBD7C-B29B47E4-87D6C690-BF5D780A</Rewards>
                                      </Item>
                                      <Item>
                                        <Level>24</Level>
                                        <Rewards>12737FD2-B9424C06-88268881-E1A85E9E,C3E035AA-B23B49D5-8D889DC0-4981B547,6F61F08B-C13D4945-B3C3A27E-C18880F5</Rewards>
                                      </Item>
                                      <Item>
                                        <Level>26</Level>
                                        <Rewards>B055C14B-5F34405A-AF20977A-2C09642A,BCB117F4-EBE44057-A203711F-C58C3D38,947C8577-55EC4CFA-81B079D0-AD12693D</Rewards>
                                      </Item>
                                      <Item>
                                        <Level>28</Level>
                                        <Rewards>0BCB1FDE-97BF4F34-881EF7D2-053BB98F,20AC9513-1943494C-86FCDB7E-11920882,7E106EA6-34494F40-860FF6EF-81720CD6</Rewards>
                                      </Item>
                                      <Item>
                                        <Level>30</Level>
                                        <Rewards>912DC677-92BF40AA-9A2F9515-786B8358,4C15092F-6115434D-AD24CFA7-FA77A0E3,CA89389A-8A3545F4-8E2BE943-DFA595D2</Rewards>
                                      </Item>
                                    </Response>";
                        case "RequestInventory":
                            return User.UpdateCharacter(PostData, boundary, UserID, WorkPath, Command);
                        case "AddInventory":
                            return User.UpdateCharacter(PostData, boundary, UserID, WorkPath, Command);
                        case "UseDaily":
                            return User.UpdateCharacter(PostData, boundary, UserID, WorkPath, Command);
                        case "QueryDaily":
                            if (File.Exists($"{WorkPath}/NovusPrime/User_Data/{UserID}_Rewards.xml"))
                                return $"<Response>{File.ReadAllText($"{WorkPath}/NovusPrime/User_Data/{UserID}_Rewards.xml")}</Response>";
                            return "<Response></Response>";
                        case "Log":
#if DEBUG
                            LoggerAccessor.LogInfo($"[HFGAMES] - Novus Log: {Type} {Str} {Amount}");
#endif
                            return "<Response></Response>";
                        default:
                            LoggerAccessor.LogWarn($"[HFGAMES] - Client Request a Command I don't know about, please post the message on GITHUB : {Command}");
                            return "<Response></Response>";
                    }
                }
            }

            return null;
        }
    }
}
