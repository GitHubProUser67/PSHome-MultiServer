using MultiServerLibrary.HTTP;
using CustomLogger;
using HttpMultipartParser;
using System.IO;
using System.Threading;
using WebAPIService.GameServices.HELLFIRE.Helpers;
using WebAPIService.GameServices.HELLFIRE.Helpers.Tycoon;

namespace WebAPIService.GameServices.HELLFIRE.HFProcessors
{
    public class TycoonRequestProcessor
    {
        public static string ProcessMainPHP(byte[] PostData, string ContentType, string PHPSessionID, string WorkPath, bool https)
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
                    try
                    {
                        UserID = data.GetParameterValue("UserID");
                    }
                    catch
                    {
                        // Not Important.
                    }
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

                if (!string.IsNullOrEmpty(Command))
                {
                    string ServerFilesPath = $"{WorkPath}/TYCOON/Server_Data";
                    string userDataPath = $"{WorkPath}/TYCOON/User_Data";
                    Directory.CreateDirectory(userDataPath);
#if DEBUG
                    LoggerAccessor.LogInfo(Command);
#endif
                    switch (Command)
                    {
                        case "VersionCheck":
                            return $"<Response><URL>{(https ? "https" : "http")}://game2.hellfiregames.com/HomeTycoon</URL></Response>";
                        case "RequestNPTicket":
                            return NPTicket.RequestNPTicket(PostData, boundary);
                        case "RequestDefaultTownInstance":
                            //DisplayName
                            return "<Response></Response>";
                        case "RequestTownInstance":
                            return TownInstance.RequestTownInstance(UserID, DisplayName, PHPSessionID);
                        case "RequestTown":
                            Thread.Sleep(3000); // Why is that in here? Because the game is so bugged that responding too fast makes it crash.
                            return TownInstance.RequestTown(UserID, InstanceID, DisplayName, WorkPath);
                        case "RequestUser":
                            if (File.Exists(userDataPath + $"/{UserID}.xml"))
                                return $"<Response>{File.ReadAllText(userDataPath + $"/{UserID}.xml")}</Response>";
                            else
                                return $"<Response>{User.DefaultHomeTycoonProfile}</Response>";
                        case "RequestVisitingUser":
                            if (File.Exists(userDataPath + $"/{UserID}.xml"))
                                return $"<Response>{File.ReadAllText(userDataPath + $"/{UserID}.xml")}</Response>";
                            else
                                return $"<Response>{User.DefaultHomeTycoonProfile}</Response>";
                        case "RequestUserTowns":
                            return "<Response><MyTown></MyTown></Response>";
                        case "QueryMotd":
                            LoggerAccessor.LogInfo($"Checking if {ServerFilesPath + "/MOTD.xml"} exists!");
                            if (File.Exists(ServerFilesPath + "/MOTD.xml"))
                            {
                                return $"<Response>{File.ReadAllText(ServerFilesPath + "/MOTD.xml")}</Response>";
                            }
                            else
                            {
                                return @"<Response>
                                            <Motd>
                                            <en>
                                            Welcome to Home Tycoon! \n \n

                                            Ready to begin building your new city? Your assistant will get you started, and you can
                                            access everything you need from your Mayor's Desk by pressing the button. \n \n

                                            Earn free rewards by visiting the Train Station from your Mayor's Desk, where you'll be
                                            able to visit other Home Tycoon cities, hang out with friends, and maybe even get your
                                            Town city onto the leaderboards! \n \n

                                            Have fun building your dream city, Mayor!
                                            </en>
                                            </Motd>
                                        </Response>";
                            };
                        case "QueryServerGlobals":
                            //Enables Debug
                            return "<Response><GlobalHard>0</GlobalHard><GlobalWrinkles>0</GlobalWrinkles></Response>";
                        case "QueryPrices":
                            return "<Response>" +
                                "<Buildings><GlobosynTower>10</GlobosynTower></Buildings><Services>" +
                                "<GlobosynService>10</GlobosynService></Services>" +
                                "<Vehicles><GlobosynVehicle>10</GlobosynVehicle>" +
                                "</Vehicles><Expansions><GlobosynExpansion>10</GlobosynExpansion>" +
                                "</Expansions><WorkerPackages><GlobosynWorker>10</GlobosynWorker></WorkerPackages>" +
                                "</Response>";
                        case "QueryBoosters":
                            return "<Response>" +
                                "<Booster>" +
                                "<Type>1</Type><Value>1</Value><Param>1</Param>" +
                                "<UUID>7A8BC3DB-399F4457-8117F099-D9F5D132</UUID><Reward>true</Reward>" +
                                "</Booster>" +
                                "</Response>";
                        case "QueryHoldbacks":
                            //Holdback some items
                            return "<Response></Response>";
                        case "QueryRewards":
                            if (File.Exists(ServerFilesPath + $"/Server_Rewards.xml"))
                                return $"<Response>{File.ReadAllText(userDataPath + $"/Server_Rewards.xml")}</Response>";
                            else
                                return @"<Response>
                                <Reward>
                                    <Name>HT_T_Shirt</Name>
                                    <SCEA type=""table"">
                                       <UUID>3EC5EAB2-5C5D4379-AE88BD53-A2B26938</UUID>
                                       <UUID>ADED875A-724E40C7-BF874795-6ACEF9E3</UUID>
                                    </SCEA>
                                </Reward>
                                <Reward>
                                    <Name>Construction_Hat</Name>
                                    <SCEA type=""table"">
                                       <UUID>E13282E0-662243D5-9FFE2923-DE784629</UUID>
                                       <UUID>B5BBC99A-C47C45AF-8E9E394C-571098A5</UUID>
                                    </SCEA>
                                </Reward>
                                <Reward>
                                    <Name>TU_Shirt</Name>
                                    <SCEA type=""table"">
                                       <UUID>5A8D5748-86974730-AFAEDD07-1762E91D</UUID>
                                    </SCEA>
                                </Reward>
                                <Reward>
                                    <Name>GS_Shirt</Name>
                                    <SCEA type=""table"">
                                       <UUID>5A8D5748-86974730-AFAEDD07-1762E91D</UUID>
                                       <UUID>4E24376E-2C82472E-94762AFE-92684A8C</UUID>
                                    </SCEA>
                                </Reward>
                                <Reward>
                                    <Name>Fire_Helmet</Name>
                                    <SCEA type=""table"">
                                       <UUID>2F0AAC71-78844481-81D0CA67-D45825BF</UUID>
                                       <UUID>160F4886-9A1243E8-855E6DBA-19F47246</UUID>
                                    </SCEA>
                                </Reward> 
                                <Reward>
                                    <Name>Stop_Sign</Name>
                                    <SCEA type=""table"">
                                        <UUID>00455F91-55174086-B2B98B2A-B1F3644D</UUID>
                                    </SCEA>
                                </Reward>
                                <Reward>
                                    <Name>Police_Hat</Name>
                                    <SCEA type=""table"">
                                        <UUID>A553749A-297849B0-9FB73F2D-65A8372C</UUID>
                                        <UUID>1731F810-F13948C5-B4BD0B99-4776B29C</UUID>
                                    </SCEA>
                                </Reward>
                                <Reward>
                                    <Name>Police_Shirt</Name>
                                    <SCEA type=""table"">
                                        <UUID>6F73F579-68914063-8CEC298A-6229F961</UUID>
                                        <UUID>C2864B65-7ED74C96-950ACFA6-956FBAC8</UUID>
                                    </SCEA>
                                </Reward>
                                <Reward>
                                    <Name>Head_Mirror</Name>
                                    <SCEA type=""table"">
                                        <UUID>87D4951A-2A8C4139-B310F037-FA75D803</UUID>
                                        <UUID>0A25BFC8-872C44F9-A9EB507D-6CCED420</UUID>
                                    </SCEA>
                                </Reward>
                                <Reward>
                                    <Name>Zombie_Briefcase</Name>
                                    <SCEA type=""table"">
                                        <UUID>51630718-06794A57-B1C07AAB-351A9450</UUID>
                                        <UUID>B1377361-FD3B458F-82137B59-4668A41D</UUID>
                                    </SCEA>
                                </Reward>
                                <Reward>
                                    <Name>Money_Bag</Name>
                                    <SCEA type=""table"">
                                        <UUID>3752C3E6-8C0B49BB-B56C9CEA-8EF72704</UUID>
                                        <UUID>665048D1-B4234C01-A55EC3B0-9A6A24FC</UUID>
                                    </SCEA>
                                </Reward>
                                <Reward>
                                    <Name>Classy_Cane</Name>
                                    <SCEA type=""table"">
                                        <UUID>07BF333A-5B5A4C94-970F8AA6-9AA52BB3</UUID>
                                        <UUID>6DF457F3-28824F1D-95711D87-2DBE5953</UUID>
                                    </SCEA>
                                </Reward>
                                <Reward>
                                    <Name>Pith_Helmet</Name>
                                    <SCEA type=""table"">
                                        <UUID>C7FAD181-CFE14883-B558AC79-529DF0C1</UUID>
                                        <UUID>1CBC11C1-2AC74BC7-ADA4ABFB-5BF1BF4E</UUID>
                                    </SCEA>
                                </Reward>
                                <Reward>
                                    <Name>Bear_Companion</Name>
                                    <SCEA type=""table"">
                                        <UUID>0F251E61-13774374-8B028F67-73FF8D4D</UUID>
                                    </SCEA>
                                </Reward>
                                <Reward>
                                    <Name>Science_Gloves</Name>
                                    <SCEA type=""table"">
                                        <UUID>A99BAA9D-1E734B87-947DBE36-87CA6471</UUID>
                                        <UUID>9AE73E6B-962A4BF0-8EC5C0C6-0FFE28DC</UUID>
                                    </SCEA>
                                </Reward>
                                <Reward>
                                    <Name>Sonja_Portrait</Name>
                                    <SCEA type=""table"">
                                        <UUID>8866F55D-77E94B79-931173AF-C859F433</UUID>
                                    </SCEA>
                                </Reward>
                                <Reward>
                                    <Name>Science_Goggles</Name>
                                    <SCEA type=""table"">
                                        <UUID>23022989-4CFE4808-BCFCABEE-44F6239E</UUID>
                                        <UUID>87DC7757-835B46CB-B552E3E8-E9ED1D70</UUID>
                                    </SCEA>
                                </Reward>
                                <Reward>
                                    <Name>Geiger_Counter</Name>
                                    <SCEA type=""table"">
                                        <UUID>FBAA938E-2ECF4B0A-B489E637-4D35E100</UUID>
                                        <UUID>E6D20C76-FC87402E-9FA05F49-C64183AE</UUID>
                                    </SCEA>
                                </Reward>
                                <Reward>
                                    <Name>Hazmat_Suit</Name>
                                    <SCEA type=""table"">
                                        <UUID>F79F3FE5-CB6048C4-AA60F7AC-2DCB2284</UUID>
                                        <UUID>CF8233A4-9D0A42C8-A31C7E91-8283E369</UUID>
                                    </SCEA>
                                </Reward>
                                <Reward>
                                    <Name>Briefcase</Name>
                                    <SCEA type=""table"">
                                        <UUID>AB63CC1E-27994EA0-AABD000B-E913153C</UUID>
                                    </SCEA>
                                </Reward>
                                <Reward>
                                    <Name>Magnus_Portrait</Name>
                                    <SCEA type=""table"">
                                        <UUID>2D79FF0B-E8984797-AD501B97-60FABECB</UUID>
                                    </SCEA>
                                </Reward>
                                <Reward>
                                    <Name>Mayor_Pants</Name>
                                    <SCEA type=""table"">
                                        <UUID>D0A3B7EB-6E4441B1-AC500603-549B08D7</UUID>
                                    </SCEA>
                                </Reward>
                                <Reward>
                                    <Name>Medic_Helmet</Name>
                                    <SCEA type=""table"">
                                        <UUID>83B0054B-94854708-93F79807-B2C6E822</UUID>
                                        <UUID>D3D58063-C1E94046-BA95179D-664FB50A</UUID>
                                    </SCEA>
                                </Reward>
                                <Reward>
                                    <Name>Police_Helmet</Name>
                                    <SCEA type=""table"">
                                        <UUID>FBD8A0B0-C66F451C-8B292AF3-786E2138</UUID>
                                        <UUID>00DC914D-3F054B62-A9ED25DE-0B81E05D</UUID>
                                    </SCEA>
                                </Reward>
                                <Reward>
                                    <Name>Mayor_Vest</Name>
                                    <SCEA type=""table"">
                                        <UUID>FBD8A0B0-C66F451C-8B292AF3-786E2138</UUID>
                                        <UUID>70A97492-2B0A420F-A8838A9E-877F8D9D</UUID>
                                    </SCEA>
                                </Reward>
                                <Reward>
                                    <Name>Balloon_Cart</Name>
                                    <SCEA type=""table"">
                                        <UUID>A39BF49F-5C944139-8D2C36A9-5A9B86CD</UUID>
                                    </SCEA>
                                </Reward>
                                <Reward>
                                    <Name>News_Helmet</Name>
                                    <SCEA type=""table"">
                                        <UUID>725040AC-812C4C79-86CBB69C-1CC0C512</UUID>
                                        <UUID>C06A2E46-A34D4467-9CB460F4-732E394A</UUID>
                                    </SCEA>
                                </Reward>
                                <Reward>
                                    <Name>Carnival_Tent</Name>
                                    <SCEA type=""table"">
                                        <UUID>2B17E6D9-BF104CF5-94DDF655-C28209FE</UUID>
                                    </SCEA>
                                </Reward>
                                <Reward>
                                    <Name>Popcorn_Cart</Name>
                                    <SCEA type=""table"">
                                        <UUID>80D47F2A-CFC44160-886AF5B7-0515292A</UUID>
                                    </SCEA>
                                </Reward>
                                <Reward>
                                    <Name>Fortune_Teller</Name>
                                    <SCEA type=""table"">
                                        <UUID>D1C6E9B7-A8824EEF-85A63FA8-6B06A318</UUID>
                                    </SCEA>
                                </Reward>
                                <Reward>
                                    <Name>Floating_Balloons</Name>
                                    <SCEA type=""table"">
                                        <UUID>F7C99A22-80904B69-95754823-CC2051F8</UUID>
                                    </SCEA>
                                </Reward>
                                </Response>";
                        case "QueryGifts":
                            if (File.Exists(userDataPath + $"/{UserID}_Gifts.xml"))
                                return $"<Response>{File.ReadAllText(userDataPath + $"/{UserID}_Gifts.xml")}</Response>";
                            else
                                return "<Response><Gift>5</Gift></Response>";
                        case "UpdateTownTime":
                            //-- This one updates the timestamp on the "LastVisited" row every few minutes
                            return "<Response></Response>";
                        case "UpdateTownPlayers":
                            return "<Response></Response>";
                        case "UpdateInstance":
                            //-- This one updates the timestamp on the "LastVisited" row every few minutes
                            //--and also keeps the number of people in the city updated
                            return "<Response></Response>";
                        case "AddVisitor":
                            return TownProcessor.HandleVisitors(PostData, boundary, UserID, WorkPath, Command);
                        case "GetVisitors":
                            return TownProcessor.HandleVisitors(PostData, boundary, UserID, WorkPath, Command);
                        case "ClearVisitors":
                            return TownProcessor.HandleVisitors(PostData, boundary, UserID, WorkPath, Command);
                            //Create multiple towns unsupported for NOW
                        case "CreateSuburb":
                            return "<Response></Response>";
                        case "SpendCoins":
                            return User.UpdateUserHomeTycoon(PostData, boundary, UserID, WorkPath, Command);
                        case "SavePostcard":
                            return "<Response></Response>";
                        case "UpdateUser":
                            return User.UpdateUserHomeTycoon(PostData, boundary, UserID, WorkPath, Command);
                        case "HasUser":
                            return User.GetUserHomeTycoon(PostData, boundary, UserID, WorkPath);
                        case "SetPrivacy":
                            //Set Town Privacy for User
                            return User.UpdateUserHomeTycoon(PostData, boundary, UserID, WorkPath, Command);
                        case "AddActivity":
                            return User.UpdateUserHomeTycoon(PostData, boundary, UserID, WorkPath, Command);
                        case "RemoveActivity":
                            return User.UpdateUserHomeTycoon(PostData, boundary, UserID, WorkPath, Command);
                        case "AddUnlocked":
                            return User.UpdateUserHomeTycoon(PostData, boundary, UserID, WorkPath, Command);
                        case "RemoveUnlocked":
                            return User.UpdateUserHomeTycoon(PostData, boundary, UserID, WorkPath, Command);
                        case "AddVehicle":
                            return User.UpdateUserHomeTycoon(PostData, boundary, UserID, WorkPath, Command);
                        case "RemoveVehicle":
                            return User.UpdateUserHomeTycoon(PostData, boundary, UserID, WorkPath, Command);
                        case "AddInventory":
                            return User.UpdateUserHomeTycoon(PostData, boundary, UserID, WorkPath, Command);
                        case "AddFlag":
                            return User.UpdateUserHomeTycoon(PostData, boundary, UserID, WorkPath, Command);
                        case "AddMission":
                            return User.UpdateUserHomeTycoon(PostData, boundary, UserID, WorkPath, Command);
                        case "CompleteMission":
                            return User.UpdateUserHomeTycoon(PostData, boundary, UserID, WorkPath, Command);
                        case "AddMissionToJournal":
                            return User.UpdateUserHomeTycoon(PostData, boundary, UserID, WorkPath, Command);
                        case "RemoveMissionFromJournal":
                            return User.UpdateUserHomeTycoon(PostData, boundary, UserID, WorkPath, Command);
                        case "AddDialog":
                            return User.UpdateUserHomeTycoon(PostData, boundary, UserID, WorkPath, Command);
                        case "CompleteDialog":
                            return User.UpdateUserHomeTycoon(PostData, boundary, UserID, WorkPath, Command);
                        case "UnlockDefault":
                            return @"<Response><COM_GD_HighRiseBusiness_B>COM_GD_HighRiseBusiness_B</COM_GD_HighRiseBusiness_B>
                                      <COM_NE_BusinessTower_A>COM_NE_BusinessTower_A</COM_NE_BusinessTower_A>
                                      <COM_NE_HighRiseBusiness_A>COM_NE_HighRiseBusiness_A</COM_NE_HighRiseBusiness_A>
                                      <COM_NE_HighRiseBusiness_B>COM_NE_HighRiseBusiness_B</COM_NE_HighRiseBusiness_B>
                                      <COM_NE_Mall>COM_NE_Mall</COM_NE_Mall>
                                      <COM_NE_Factory>COM_NE_Factory</COM_NE_Factory>
                                      <RES_NE_Estate_A>RES_NE_Estate_A</RES_NE_Estate_A>
                                      <RES_NE_Estate_B>RES_NE_Estate_B</RES_NE_Estate_B>
                                      <RES_NE_HighRiseCondo_A>RES_NE_HighRiseCondo_A</RES_NE_HighRiseCondo_A>
                                      <RES_NE_HighRiseCondo_B>RES_NE_HighRiseCondo_B</RES_NE_HighRiseCondo_B>
                                      <RES_NE_SmApartment_B>RES_NE_SmApartment_B</RES_NE_SmApartment_B>
                                      <RES_NE_LgApartment_B>RES_NE_LgApartment_B</RES_NE_LgApartment_B>
                                      <UTL_NE_ElectricTower>UTL_NE_ElectricTower</UTL_NE_ElectricTower>
                                      <UTL_NE_Highway_A>UTL_NE_Highway_A</UTL_NE_Highway_A>
                                      <UTL_NE_Lake_A>UTL_NE_Lake_A</UTL_NE_Lake_A>
                                      <UTL_NE_Lake_B>UTL_NE_Lake_B</UTL_NE_Lake_B>
                                      <UTL_NE_NovusRecruitStation>UTL_NE_NovusRecruitStation</UTL_NE_NovusRecruitStation>
                                      <UTL_NE_Road_Straight_TreeCover>UTL_NE_Road_Straight_TreeCover</UTL_NE_Road_Straight_TreeCover>
                                      <Park2>Park2</Park2>
                                      <RES_GD_LgApartment_A>RES_GD_LgApartment_A</RES_GD_LgApartment_A>
                                    </Response>";
                        case "CreateBuilding":
                            return TownProcessor.CreateBuilding(PostData, boundary, UserID, WorkPath);
                        case "UpdateBuildings":
                            return TownProcessor.UpdateBuildings(PostData, boundary, UserID, WorkPath);
                        case "RemoveBuilding":
                            return TownProcessor.RemoveBuilding(PostData, boundary, UserID, WorkPath);
                        case "GlobalPopulationLeaderboard":
                            return Leaderboards.GetGlobalPopulationLeaderboard(PostData, boundary, UserID, WorkPath);
                        case "GlobalRevenueCollectedLeaderboard":
                            return Leaderboards.GetGlobalRevenueCollectedLeaderboard(PostData, boundary, UserID, WorkPath);
                        //Debug functions in lua commented out
                        case "DeleteCity":
                            return "<Response></Response>";
                        case "DeleteUser":
                            return "<Response></Response>";
                        case "ClearMissionRevenueCollected":
                            //return User.ClearMissionRevenueCollected(PostData, boundary, UserID, WorkPath);
                            return "<Response></Response>";
                        default:
                            LoggerAccessor.LogWarn($"[HFGAMES] - Client Requested an unknown Home Tycoon Command, please report as issue on GITHUB : {Command}");
                            return "<Response></Response>";
                    }
                }
            }

            return null;
        }

        public static string ProcessPostCards(byte[] PostData, string ContentType) // Not yet handled.
        {
            if (PostData == null || string.IsNullOrEmpty(ContentType))
                return null;

            return null;
        }
    }
}
