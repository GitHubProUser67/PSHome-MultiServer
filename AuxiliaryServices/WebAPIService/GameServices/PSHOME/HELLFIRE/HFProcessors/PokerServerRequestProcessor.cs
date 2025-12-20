using MultiServerLibrary.HTTP;
using CustomLogger;
using HttpMultipartParser;
using System.IO;
using System.Collections.Generic;
using System;
using WebAPIService.GameServices.PSHOME.HELLFIRE.Helpers;
using WebAPIService.GameServices.PSHOME.HELLFIRE.Helpers.Poker;

namespace WebAPIService.GameServices.PSHOME.HELLFIRE.HFProcessors
{
    public class PokerServerRequestProcessor
    {
        public List<PokerPlayer> pokerPlayers;

        public static string ProcessPokerMainPHP(byte[] PostData, string ContentType, string PHPSessionID, string WorkPath)
        {
            if (PostData == null || string.IsNullOrEmpty(ContentType))
                return null;

            string Command = string.Empty;
            string UserID = string.Empty;
            string DisplayName = string.Empty;
            string InstanceID = string.Empty;
            string Region = string.Empty;
            string boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (boundary != null)
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);
                    Command = data.GetParameterValue("Command");
                    UserID = data.GetParameterValue("UserID");

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
                    ms.Flush();
                }

                // Get the current UTC time
                DateTime currentTime = DateTime.UtcNow;
                // Add 5 minutes
                DateTime futureTime = currentTime.AddMinutes(3);
                // Convert to Unix timestamp
                long unixTimestamp = new DateTimeOffset(futureTime).ToUnixTimeSeconds();

                if (!string.IsNullOrEmpty(Command))
                {
                    Directory.CreateDirectory($"{WorkPath}/Poker/User_Data");

                    switch (Command)
                    {
                        case "RequestNPTicket":
                            return Helpers.Poker.NPTicket.RequestNPTicket(PostData, boundary);
                        case "RequestRewards":
                            
                            return @"<Response>
	                                    <Rewards>
		                                    <Reward>
                                                <Male>F22BE91D-BCA343AD-88CBD940-4EFE3E76</Male>
                                                <How>Win a Sit-n-Go tournament</How>
		                                    </Reward>    
	                                    </Rewards>
                                    </Response>";
                        case "RequestUser":
                            return User.RequestUserPoker(PostData, boundary, UserID, WorkPath);
                        case "UpdateUser":
                            return User.UpdateUserPoker(PostData, boundary, UserID, WorkPath);
                        case "RequestDailyInstancePlayers":
                            //Request InstanceID

                            //Response
                            return @"
                            <Response>
                                <Players>
                                    <Player>
                                        <TableNum>1</TableNum>
                                        <SeatNum>2</SeatNum>
                                        <Stack>5000</Stack>
                                        <UserID>Dust514JumpDev</UserID>
                                        <DisplayName>Dust514JumpDev</DisplayName>
                                    </Player>
                                </Players>
                            </Response>";
                        case "RemoveTimedoutUsers":
                            return "<Response></Response>";
                        case "PlayerArrived":
                            return "<Response></Response>";
                        case "PlayerKeepalive":
                            return "<Response></Response>";
                        case "DeleteSitngoPlayer":
                            //DisplayName
                            return "<Response></Response>";
                        case "AwardTourneyPrize":
                            //Bankroll
                            return "<Response></Response>";
                        case "PlayerLeftGame":  
                            return "<Response></Response>";

                        case "UserTourneySeat":
                            //Request UserID

                            //Response
                            //TableNum
                            //SeatNum
                            //Buyin
                            //Stack
                            //StartTime

                            //InstanceID
                            //Type - SITNGO_TYPE, DAILY_TYPE, WEEKLY_TYPE
                            //FinalTable - true
                            //ResponseCode == SeatingNotOpen, UserNotInTourney

                            return $@"<Response>
                                        <ResponseCode>Success</ResponseCode>
                                        <InstanceID>1</InstanceID>
                                        <Type>SITNGO_TYPE</Type>
                                        <FinalTable>true</FinalTable>
                                        <TableNum>1</TableNum>
                                        <SeatNum>1</SeatNum>
                                        <Buyin>50</Buyin>
                                        <Stack>0</Stack>
                                        <StartTime>{unixTimestamp}</StartTime>
                                    </Response>";
                        case "UserDailyAssignedSeat":
                            //InstanceID or UserID
                            //Type - SITNGO_TYPE, DAILY_TYPE, WEEKLY_TYPE
                            //FinalTable - true
                            //ResponseCode == SeatingNotOpen, UserNotInTourney
                            return $@"<Response>
                                        <ResponseCode>Success</ResponseCode>
                                        <InstanceID>1</InstanceID>
                                        <Type>SITNGO_TYPE</Type>
                                        <FinalTable>true</FinalTable>
                                        <TableNum>1</TableNum>
                                        <SeatNum>1</SeatNum>
                                        <Buyin>50</Buyin>
                                        <Stack>0</Stack>
                                        <StartTime>{unixTimestamp}</StartTime>
                                    </Response>";
                        case "UserTourneyQueue":
                            //Request UserID

                            //Response StartTime
                            //Type - DAILY_TYPE, WEEKLY_TYPE
                            return @"<Response>
                                        <StartTime>15</StartTime>
                                        <Type>DAILY_TYPE</Type>
                                    </Response>";

                        case "SetInstanceUUID":
                            //Request 
                            //InstanceID
                            //InstanceUUID

                            //Response 
                            //StartupValue - SITNGO_STARTUP_VALUE = -1337,  DAILY_STARTUP_VALUE = -4331, FINALTABLE_STARTUP_VALUE = -74513
                            return "<Response><StartupValue>-4331</StartupValue></Response>";
                        case "GetStartupValue":
                            //Request 
                            //InstanceID
                            //InstanceUUID

                            //Response 
                            //StartupValue - SITNGO_STARTUP_VALUE = -1337,  DAILY_STARTUP_VALUE = -4331, FINALTABLE_STARTUP_VALUE = -74513
                            return "<Response><StartupValue>-4331</StartupValue></Response>";

                        case "RequestConfig":
                            return @"<Response>
                                        <SITNGO_LEVEL_MINS>25</SITNGO_LEVEL_MINS>
                                        <SITNGO_PRESEATING_MINS>50</SITNGO_PRESEATING_MINS>
                                        <SITNGO_MIN_PLAYERS>4</SITNGO_MIN_PLAYERS>
                                        <SITNGO_BB_FRACT>0.5</SITNGO_BB_FRACT>
                                        <DAILY_BB_FRACT>0.5</DAILY_BB_FRACT>
                                        <DAILY_START_STACK>50</DAILY_START_STACK>
                                        <DAILY_DURATION_MINS>50</DAILY_DURATION_MINS>
                                        <DAILY_LEVEL_MINS>50</DAILY_LEVEL_MINS>
                                        <DAILY_PRESEATING_MINS>50</DAILY_PRESEATING_MINS>
                                    </Response>";

                        case "TourneyGameStarted":
                            //InstanceID
                            //TableNum
                            return "<Response></Response>";

                        case "TourneyChipstackSavepoint":
                            //Request DisplayNameChipstacks 
                            //InstanceID
                            //TableNum

                            //Response
                            //

                            return "<Response></Response>";

                        case "GetDailyPlayerStacks":
                            //Request TransitDisplaynames

                            //Response 
                            // Players -> Player 
                            // Player -> DisplayName, InstanceID, TableNum, SeatNum, Stack

                            return "<Response></Response>";

                        case "DeleteDailyPlayer":
                            //Request DisplayName

                            //Response Rank integer
                            return "<Response></Response>";

                        case "RequestLeaderboard":
                            return "<Response></Response>";//Leaderboards.GetLeaderboardsClearasil(PostData, boundary, UserID, WorkPath);
                        case "LogMetric":
                            return "<Response></Response>"; // We don't really care about Metrics just yet

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
