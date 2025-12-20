using CustomLogger;
using HttpMultipartParser;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using WebAPIService.GameServices.PSHOME.HELLFIRE.Helpers.NovusPrime;
using WebAPIService.GameServices.PSHOME.HELLFIRE.Entities.HomeTycoon;
using WebAPIService.GameServices.PSHOME.HELLFIRE.Helpers.Tycoon;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.PSHOME.HELLFIRE.Helpers
{
    public class User
    {
        public const string DefaultHomeTycoonProfile = @"<NewPlayer>1</NewPlayer>
            <TotalCollected>0.000000</TotalCollected>
            <Wallet>5000.000000</Wallet>
            <Workers>99.000000</Workers>
            <GoldCoins>9999.000000</GoldCoins>
            <SilverCoins>0.000000</SilverCoins>
            <Options><MusicVolume>1.0</MusicVolume><PrivacySetting>3</PrivacySetting></Options>
            <Missions></Missions>
            <Journal></Journal>
            <Dialogs><Dialog_S01M00>Dialog_S01M00</Dialog_S01M00></Dialogs>
            <Unlocked></Unlocked>
            <Activities></Activities>
            <Expansions></Expansions>
            <Vehicles></Vehicles>
            <Flags></Flags>
            <Inventory></Inventory>
            <InstanceID></InstanceID>
            <Towns></Towns>";

        public const string DefaultNovusPrimeProfile = @"
            <CharData>
                <Nebulon>0</Nebulon>
                <TotalNebulonEver>0</TotalNebulonEver>
                <Experience>0</Experience>
                <Level>1</Level>
            </CharData>
            <ShipConfig>
                <Chassis>0</Chassis>
                <Front1>0</Front1>
                <Front2>0</Front2>
                <Turret1>0</Turret1>
                <Turret2>0</Turret2>
                <Special>0</Special>
                <Maneuver>0</Maneuver>
                <Upgrade1>0</Upgrade1>
                <Upgrade2>0</Upgrade2>
                <Upgrade3>0</Upgrade3>
                <Upgrade4>0</Upgrade4>
                <PaintJob>0</PaintJob>
            </ShipConfig>
            <Inventory></Inventory>
            <Missions>
            </Missions>
            <DailyAvailable>
            </DailyAvailable>";

        public const string DefaultClearasilSkaterAndSlimJimProfile = "<BestScoreStage1>0</BestScoreStage1><BestScoreStage2>0</BestScoreStage2><LeaderboardScore>0</LeaderboardScore>";
        public const string DefaultPokerProfile = "<Bankroll>1000</Bankroll><NewPlayer>1</NewPlayer>";

        public static string UpdateUserHomeTycoon(byte[] PostData, string boundary, string UserID, string WorkPath, string cmd)
        {
            string xmlProfile = string.Empty;
            string updatedXMLProfile = string.Empty;
            string userDataPath = $"{WorkPath}/HomeTycoon/User_Data/{UserID}";

            Directory.CreateDirectory(userDataPath);

            // Retrieve the user's XML profile
            string profilePath = $"{userDataPath}/Profile.xml";

            if (File.Exists(profilePath))
                xmlProfile = File.ReadAllText(profilePath);
            else
                xmlProfile = DefaultHomeTycoonProfile;

            try
            {
                // Create an XmlDocument
                var doc = new XmlDocument();
                doc.LoadXml("<xml>" + xmlProfile + "</xml>"); // Wrap the XML string in a root element
                if (doc != null && PostData != null && !string.IsNullOrEmpty(boundary))
                {
                    using (MemoryStream ms = new MemoryStream(PostData))
                    {
                        var data = MultipartFormDataParser.Parse(ms, boundary);
                        // Update the profile values from the provided data
                        // Update the values based on IDs
#pragma warning disable 8602
                        switch (cmd)
                        {
                            case "AddUnlocked":
                                {
                                    string BuildingName = data.GetParameterValue("BuildingName");

                                    var UnlockedNode = doc.SelectSingleNode("//Unlocked");
                                    if (UnlockedNode != null)
                                    {
                                        var existingBuilding = UnlockedNode.SelectSingleNode(BuildingName);

                                        if (existingBuilding != null)
                                            existingBuilding.InnerText = BuildingName;
                                        else
                                        {
                                            XmlElement newBuilding = doc.CreateElement(BuildingName);
                                            newBuilding.InnerText = BuildingName;
                                            UnlockedNode.AppendChild(newBuilding);
                                        }
                                    }
                                }
                                break;
                            case "RemoveUnlocked":
                                {
                                    var userProfileUnlockNode = doc.DocumentElement.SelectSingleNode("//Unlocked");

                                    if (userProfileUnlockNode != null)
                                    {
                                        string buildingName = data.GetParameterValue("BuildingName");

                                        var buildingNode = userProfileUnlockNode.SelectSingleNode(buildingName);
                                        if (buildingNode != null)
                                            userProfileUnlockNode.RemoveChild(buildingNode);
                                        else
                                            LoggerAccessor.LogWarn($"[User] - UpdateUserHomeTycoon: Building not found: {buildingName}");
                                    }
                                    else
                                        LoggerAccessor.LogWarn($"[User] - UpdateUserHomeTycoon: Unlocked node not found in the XML");
                                }
                                break;
                            case "AddDialog":
                                {
                                    string dialogName = data.GetParameterValue("DialogName");

                                    var dialogsNode = doc.SelectSingleNode("//Dialogs");
                                    if (dialogsNode != null)
                                    {
                                        var existingDialog = dialogsNode.SelectSingleNode(dialogName);

                                        if (existingDialog != null)
                                            existingDialog.InnerText = dialogName;
                                        else
                                        {
                                            XmlElement newDialog = doc.CreateElement(dialogName);
                                            newDialog.InnerText = dialogName;
                                            dialogsNode.AppendChild(newDialog);
                                        }
                                    }
                                }
                                break;
                            case "CompleteDialog":
                                {
                                    var userProfileDialogs = doc.DocumentElement.SelectSingleNode("//Dialogs");
                                    var dialogNode = userProfileDialogs.SelectSingleNode(data.GetParameterValue("DialogName"));
                                    if (dialogNode != null)
                                        userProfileDialogs.RemoveChild(dialogNode);
                                }
                                break;
                            case "AddVehicle":
                                {
                                    string vehicleName = data.GetParameterValue("VehicleName");

                                    var vehiclesNode = doc.SelectSingleNode("//Vehicles");
                                    if (vehiclesNode != null)
                                    {
                                        var existingVehicle = vehiclesNode.SelectSingleNode(vehicleName);

                                        if (existingVehicle != null)
                                            existingVehicle.InnerText = vehicleName;
                                        else
                                        {
                                            XmlElement newVehicle = doc.CreateElement(vehicleName);
                                            newVehicle.InnerText = vehicleName;
                                            vehiclesNode.AppendChild(newVehicle);
                                        }
                                    }
                                }
                                break;
                            case "RemoveVehicle":
                                {
                                    var userProfileVehicleNode = doc.DocumentElement.SelectSingleNode("//Vehicles");
                                    var vehicleNode = userProfileVehicleNode.SelectSingleNode(data.GetParameterValue("VehicleName"));
                                    if (vehicleNode != null)
                                        userProfileVehicleNode.RemoveChild(vehicleNode);
                                }
                                break;
                            case "AddInventory":
                                {
                                    string buildingID = data.GetParameterValue("BuildingID");

                                    var inventoryNode = doc.SelectSingleNode("//Inventory");
                                    if (inventoryNode != null)
                                    {
                                        var existingBuilding = inventoryNode.SelectSingleNode(buildingID);
                                        if (existingBuilding != null)
                                            existingBuilding.InnerText = buildingID;
                                        else
                                        {
                                            XmlElement newBuilding = doc.CreateElement(buildingID);
                                            newBuilding.InnerText = buildingID;
                                            inventoryNode.AppendChild(newBuilding);
                                        }
                                    }
                                }
                                break;
                            case "AddActivity":
                                {
                                    string activityName = data.GetParameterValue("ActivityName");

                                    var activitiesNode = doc.SelectSingleNode("//Activities");
                                    if (activitiesNode != null)
                                    {
                                        var existingActivity = activitiesNode.SelectSingleNode(activityName);
                                        if (existingActivity != null)
                                            existingActivity.InnerText = activityName;
                                        else
                                        {
                                            XmlElement newActivity = doc.CreateElement(activityName);
                                            newActivity.InnerText = activityName;
                                            activitiesNode.AppendChild(newActivity);
                                        }
                                    }
                                }
                                break;
                            case "RemoveActivity":
                                {
                                    var userProfileActivitiesNode = doc.DocumentElement.SelectSingleNode("//Activities");
                                    if (userProfileActivitiesNode != null)
                                    {
                                        string ActivityName = data.GetParameterValue("ActivityName");

                                        var buildingNode = userProfileActivitiesNode.SelectSingleNode(ActivityName);
                                        if (buildingNode != null)
                                            userProfileActivitiesNode.RemoveChild(buildingNode);
                                        else
                                            LoggerAccessor.LogWarn($"[User] - UpdateUserHomeTycoon: Activity not found: {ActivityName}");
                                    }
                                    else
                                        LoggerAccessor.LogWarn("[User] - UpdateUserHomeTycoon: Activities node not found in the XML.");
                                }
                                break;
                            case "AddMission":
                                {
                                    string missionName = data.GetParameterValue("MissionName");

                                    var missionsNode = doc.SelectSingleNode("//Missions");
                                    if (missionsNode != null)
                                    {
                                        var existingMission = missionsNode.SelectSingleNode(missionName);

                                        if (existingMission != null)
                                            existingMission.InnerText = missionName;
                                        else
                                        {
                                            XmlElement newMission = doc.CreateElement(missionName);
                                            newMission.InnerText = missionName;
                                            missionsNode.AppendChild(newMission);
                                        }
                                    }
                                }
                                break;
                            case "CompleteMission":
                                {
                                    var missionsNode = doc.DocumentElement.SelectSingleNode("//Missions");
                                    var missionNode = missionsNode.SelectSingleNode(data.GetParameterValue("MissionName"));
                                    if (missionNode != null)
                                        missionsNode.RemoveChild(missionNode);
                                }
                                break;
                            case "AddMissionToJournal":
                                {
                                    string missionName = data.GetParameterValue("MissionName");

                                    var journalNode = doc.SelectSingleNode("//Journal");
                                    if (journalNode != null)
                                    {
                                        var existingMission = journalNode.SelectSingleNode(missionName);

                                        if (existingMission != null)
                                            existingMission.InnerText = missionName;
                                        else
                                        {
                                            XmlElement newMission = doc.CreateElement(missionName);
                                            newMission.InnerText = missionName;
                                            journalNode.AppendChild(newMission);
                                        }
                                    }
                                }
                                break;
                            case "RemoveMissionFromJournal":
                                {
                                    var userProfileActivitiesNode = doc.DocumentElement.SelectSingleNode("//Journal");
                                    if (userProfileActivitiesNode != null)
                                    {
                                        string MissionName = data.GetParameterValue("MissionName");

                                        var buildingNode = userProfileActivitiesNode.SelectSingleNode(MissionName);
                                        if (buildingNode != null)
                                            userProfileActivitiesNode.RemoveChild(buildingNode);
                                        else
                                            LoggerAccessor.LogWarn($"[User] - UpdateUserHomeTycoon: Mission not found: {MissionName}");
                                    }
                                    else
                                        LoggerAccessor.LogWarn("[User] - UpdateUserHomeTycoon: Journal node not found in the XML.");
                                }
                                break;
                            case "AddExpansion":
                                {
                                    string expansionName = data.GetParameterValue("ExpansionName");

                                    var expansionNode = doc.SelectSingleNode("//Expansions");
                                    if (expansionNode != null)
                                    {
                                        var existingMission = expansionNode.SelectSingleNode(expansionName);

                                        if (existingMission != null)
                                            existingMission.InnerText = expansionName;
                                        else
                                        {
                                            XmlElement newMission = doc.CreateElement(expansionName);
                                            newMission.InnerText = expansionName;
                                            expansionNode.AppendChild(newMission);
                                        }
                                    }
                                }
                                break;
                            case "AddFlag":
                                {
                                    string flag = data.GetParameterValue("Flag");

                                    var flagsNode = doc.SelectSingleNode("//Flags");
                                    if (flagsNode != null)
                                    {
                                        var existingFlag = flagsNode.SelectSingleNode(flag);

                                        if (existingFlag != null)
                                            existingFlag.InnerText = flag;
                                        else
                                        {
                                            XmlElement newFlag = doc.CreateElement(flag);
                                            newFlag.InnerText = flag;
                                            flagsNode.AppendChild(newFlag);
                                        }
                                    }
                                }
                                break;
                            case "SpendCoins":
                                {
                                    bool isSilverCoin = int.Parse(data.GetParameterValue("CoinType")) == 1;
                                    double NumCoinsDouble = double.Parse(data.GetParameterValue("NumCoins"), CultureInfo.InvariantCulture);
                                    int NumCoins = (int)NumCoinsDouble;
                                    string TransParam = data.GetParameterValue("TransParam");

                                    if (isSilverCoin)
                                    {
                                        var node = doc.SelectSingleNode("//SilverCoins");
                                        node.InnerText = Math.Max(0, double.Parse(node.InnerText, CultureInfo.InvariantCulture) - NumCoinsDouble).ToString(CultureInfo.InvariantCulture);
                                    }
                                    else
                                    {
                                        var node = doc.SelectSingleNode("//GoldCoins");
                                        node.InnerText = Math.Max(0, double.Parse(node.InnerText, CultureInfo.InvariantCulture) - NumCoinsDouble).ToString(CultureInfo.InvariantCulture);
                                    }

                                    switch (data.GetParameterValue("TransType"))
                                    {
                                        case "CollectAllRevenue":
                                            {
                                                return $@"<Response>
                                                <ResponseCode>Success</ResponseCode>
                                                <TotalSilver>{doc.SelectSingleNode("//SilverCoins").InnerText}</TotalSilver>
                                                <TotalGold>{doc.SelectSingleNode("//GoldCoins").InnerText}</TotalGold>
                                                <SilverSpent>{(isSilverCoin ? NumCoins : 0)}</SilverSpent>
                                                <GoldSpent>{(!isSilverCoin ? NumCoins : 0)}</GoldSpent>
                                                </Response>";
                                            }
                                        case "BuyBuilding":
                                            {
                                                return $@"<Response>
                                                <ResponseCode>Success</ResponseCode>
                                                <TotalSilver>{doc.SelectSingleNode("//SilverCoins").InnerText}</TotalSilver>
                                                <TotalGold>{doc.SelectSingleNode("//GoldCoins").InnerText}</TotalGold>
                                                <SilverSpent>{(isSilverCoin ? NumCoins : 0)}</SilverSpent>
                                                <GoldSpent>{(!isSilverCoin ? NumCoins : 0)}</GoldSpent>
                                                </Response>";
                                            }
                                        case "BuyWorkers":
                                            {
                                                return $@"<Response>
                                                <ResponseCode>Success</ResponseCode>
                                                <TotalSilver>{doc.SelectSingleNode("//SilverCoins").InnerText}</TotalSilver>
                                                <TotalGold>{doc.SelectSingleNode("//GoldCoins").InnerText}</TotalGold>
                                                <SilverSpent>{(isSilverCoin ? NumCoins : 0)}</SilverSpent>
                                                <GoldSpent>{(!isSilverCoin ? NumCoins : 0)}</GoldSpent>
                                                </Response>";
                                            }
                                        case "BuyVehicle":
                                        case "BuyVehicles":
                                            {
                                                return $@"<Response>
                                                <ResponseCode>Success</ResponseCode>
                                                <TotalSilver>{doc.SelectSingleNode("//SilverCoins").InnerText}</TotalSilver>
                                                <TotalGold>{doc.SelectSingleNode("//GoldCoins").InnerText}</TotalGold>
                                                <SilverSpent>{(isSilverCoin ? NumCoins : 0)}</SilverSpent>
                                                <GoldSpent>{(!isSilverCoin ? NumCoins : 0)}</GoldSpent>
                                                </Response>";
                                            }
                                        case "BuyExpansion":
                                            {
                                                return $@"<Response>
                                                <ResponseCode>Success</ResponseCode>
                                                <TotalSilver>{doc.SelectSingleNode("//SilverCoins").InnerText}</TotalSilver>
                                                <TotalGold>{doc.SelectSingleNode("//GoldCoins").InnerText}</TotalGold>
                                                <SilverSpent>{(isSilverCoin ? NumCoins : 0)}</SilverSpent>
                                                <GoldSpent>{(!isSilverCoin ? NumCoins : 0)}</GoldSpent>
                                                </Response>";
                                            }
                                        case "BuyDollars":
                                            {
                                                return $@"<Response>
                                                <ResponseCode>Success</ResponseCode>
                                                <TotalSilver>{doc.SelectSingleNode("//SilverCoins").InnerText}</TotalSilver>
                                                <TotalGold>{doc.SelectSingleNode("//GoldCoins").InnerText}</TotalGold>
                                                <SilverSpent>{(isSilverCoin ? NumCoins : 0)}</SilverSpent>
                                                <GoldSpent>{(!isSilverCoin ? NumCoins : 0)}</GoldSpent>
                                                </Response>";
                                            }
                                        case "BuySuburb":
                                            {
                                                return $@"<Response>
                                                <ResponseCode>Success</ResponseCode>
                                                <TotalSilver>{doc.SelectSingleNode("//SilverCoins").InnerText}</TotalSilver>
                                                <TotalGold>{doc.SelectSingleNode("//GoldCoins").InnerText}</TotalGold>
                                                <SilverSpent>{(isSilverCoin ? NumCoins : 0)}</SilverSpent>
                                                <GoldSpent>{(!isSilverCoin ? NumCoins : 0)}</GoldSpent>
                                                </Response>";
                                            }
                                        case "BuyTimeOfDay":
                                            {
                                                return $@"<Response>
                                                <ResponseCode>Success</ResponseCode>
                                                <TotalSilver>{doc.SelectSingleNode("//SilverCoins").InnerText}</TotalSilver>
                                                <TotalGold>{doc.SelectSingleNode("//GoldCoins").InnerText}</TotalGold>
                                                <SilverSpent>{(isSilverCoin ? NumCoins : 0)}</SilverSpent>
                                                <GoldSpent>{(!isSilverCoin ? NumCoins : 0)}</GoldSpent>
                                                </Response>";
                                            }
                                    }
                                }
                                break;
                            case "SetPrivacy":
                                {
                                    string TownID = data.GetParameterValue("TownID");
                                    TycoonPrivacySetting NewSetting = (TycoonPrivacySetting)(int)double.Parse(data.GetParameterValue("NewSetting"), CultureInfo.InvariantCulture);

                                    var userProfileOptionsNode = doc.DocumentElement.SelectSingleNode("//Options");

                                    userProfileOptionsNode.SelectSingleNode("PrivacySetting").InnerText = ((int)NewSetting).ToString();

                                    TownProcessor.UpdateTownPrivacy(UserID, TownID, NewSetting, WorkPath);

                                    // Clear visitors (also done locally)
                                    if (NewSetting == TycoonPrivacySetting.FriendsOnly || NewSetting == TycoonPrivacySetting.Private)
                                    {
                                        string townProfile = string.Empty;
                                        string townVisitorsPath = $"{WorkPath}/HomeTycoon/TownsData/{UserID}/TownVisitors_{TownID}.xml";

                                        if (File.Exists(profilePath))
                                            townProfile = File.ReadAllText(profilePath);

                                        try
                                        {
                                            // Create an XmlDocument
                                            var visitorsDoc = new XmlDocument();
                                            visitorsDoc.LoadXml("<xml>" + townProfile + "</xml>");
                                            if (doc != null)
                                            {
                                                doc.DocumentElement.RemoveAll();

                                                // Save the updated profile back to the file
                                                File.WriteAllText(townVisitorsPath, doc.DocumentElement.InnerXml.Replace("<xml>", string.Empty).Replace("</xml>", string.Empty));
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            LoggerAccessor.LogError($"[User] - UpdateUserHomeTycoon: An assertion was thrown while removing all visitors. (Exception:{ex})");
                                        }
                                    }

                                    // Write a server txt for player names we can ban?
                                    return "<Response><Banned>0</Banned></Response>";
                                }
                            case "UpdateUser":
                                {
                                    doc.SelectSingleNode("//TotalCollected").InnerText = data.GetParameterValue("TotalCollected");
                                    doc.SelectSingleNode("//Wallet").InnerText = data.GetParameterValue("Wallet");
                                    doc.SelectSingleNode("//Workers").InnerText = data.GetParameterValue("Workers");
                                    doc.SelectSingleNode("//GoldCoins").InnerText = data.GetParameterValue("GoldCoins");
                                    doc.SelectSingleNode("//SilverCoins").InnerText = data.GetParameterValue("SilverCoins") ?? "0";
                                    doc.SelectSingleNode("//NewPlayer").InnerText = data.GetParameterValue("NewPlayer") ?? "0";
                                    var fieldValues = JToken.Parse(data.GetParameterValue("Options")).ToObject<Dictionary<string, object>>();

                                    if (fieldValues != null)
                                    {
                                        foreach (var fieldValue in fieldValues)
                                        {
                                            if (fieldValue.Key == "MusicVolume" && fieldValue.Value != null)
                                                doc.SelectSingleNode("//Options/MusicVolume").InnerText = fieldValue.Value.ToString() ?? "1.0";
                                            else if (fieldValue.Key == "PrivacySetting" && fieldValue.Value != null)
                                                doc.SelectSingleNode("//Options/PrivacySetting").InnerText = fieldValue.Value.ToString();
                                        }
                                    }
                                }
                                break;
                        }

                        updatedXMLProfile = doc.DocumentElement.InnerXml.Replace("<root>", string.Empty).Replace("</root>", string.Empty);
#pragma warning restore 8602
                        // Save the updated profile back to the file
                        File.WriteAllText(profilePath, updatedXMLProfile);

                        ms.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[User] - UpdateUserHomeTycoon: An assertion was thrown. (Exception:{ex})");
            }

            return $"<Response>{updatedXMLProfile}</Response>";
        }

        public static string UpdateUserClearasilSkater(byte[] PostData, string boundary, string UserID, string WorkPath)
        {
            string xmlProfile = string.Empty;
            string updatedXMLProfile = string.Empty;

            string profilePath = $"{WorkPath}/ClearasilSkater/User_Data/{UserID}.xml";

            if (File.Exists(profilePath))
                xmlProfile = File.ReadAllText(profilePath);
            else
                xmlProfile = DefaultClearasilSkaterAndSlimJimProfile;

            try
            {
                // Create an XmlDocument
                var doc = new XmlDocument();
                doc.LoadXml("<root>" + xmlProfile + "</root>"); // Wrap the XML string in a root element
                if (doc != null && PostData != null && !string.IsNullOrEmpty(boundary))
                {
                    using (MemoryStream ms = new MemoryStream(PostData))
                    {
                        var data = MultipartFormDataParser.Parse(ms, boundary);
                        // Update the profile values from the provided data
                        // Update the values based on IDs
#pragma warning disable 8602
                        doc.SelectSingleNode("//BestScoreStage1").InnerText = data.GetParameterValue("BestScoreStage1");
                        doc.SelectSingleNode("//BestScoreStage2").InnerText = data.GetParameterValue("BestScoreStage2");
                        doc.SelectSingleNode("//LeaderboardScore").InnerText = data.GetParameterValue("LeaderboardScore");
                        
                        updatedXMLProfile = doc.DocumentElement.InnerXml.Replace("<root>", string.Empty).Replace("</root>", string.Empty);
#pragma warning restore 8602
                        File.WriteAllText(profilePath, updatedXMLProfile);

                        ms.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[User] - UpdateUserClearasilSkater: An assertion was thrown. (Exception:{ex})");
            }

            return $"<Response>{updatedXMLProfile}</Response>";
        }

        public static string UpdateUserSlimJim(byte[] PostData, string boundary, string UserID, string WorkPath)
        {
            string xmlProfile = string.Empty;
            string updatedXMLProfile = string.Empty;

            // Retrieve the user's JSON profile
            string profilePath = $"{WorkPath}/SlimJim/User_Data/{UserID}.xml";

            if (File.Exists(profilePath))
                xmlProfile = File.ReadAllText(profilePath);
            else
                xmlProfile = DefaultClearasilSkaterAndSlimJimProfile;

            try
            {
                // Create an XmlDocument
                var doc = new XmlDocument();
                doc.LoadXml("<root>" + xmlProfile + "</root>"); // Wrap the XML string in a root element
                if (doc != null && PostData != null && !string.IsNullOrEmpty(boundary))
                {
                    using (MemoryStream ms = new MemoryStream(PostData))
                    {
                        var data = MultipartFormDataParser.Parse(ms, boundary);
                        // Update the profile values from the provided data
                        // Update the values based on IDs
#pragma warning disable 8602
                        doc.SelectSingleNode("//BestScoreStage1").InnerText = data.GetParameterValue("BestScoreStage1");
                        doc.SelectSingleNode("//BestScoreStage2").InnerText = data.GetParameterValue("BestScoreStage2");
                        doc.SelectSingleNode("//LeaderboardScore").InnerText = data.GetParameterValue("LeaderboardScore");

                        // Get the updated XML string
                        updatedXMLProfile = doc.DocumentElement.InnerXml.Replace("<root>", string.Empty).Replace("</root>", string.Empty);
#pragma warning restore 8602
                        // Save the updated profile back to the file
                        File.WriteAllText(profilePath, updatedXMLProfile);

                        ms.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[User] - UpdateUserSlimJim: An assertion was thrown. (Exception:{ex})");
            }

            return $"<Response>{updatedXMLProfile}</Response>";
        }

        public static string RequestInitialDataNovusPrime(byte[] PostData, string boundary, string UserID, string WorkPath)
        {
            string profilePath = $"{WorkPath}/NovusPrime/User_Data/{UserID}.xml";

            string xmlProfile;
            if (File.Exists(profilePath))
            {
                xmlProfile = File.ReadAllText(profilePath);

                var doc = new XmlDocument();

                doc.LoadXml("<root>" + xmlProfile + "</root>"); // Wrap the XML string in a root element
                if (doc != null)
                {
                    XmlNode DailyAvailable = doc.SelectSingleNode("//DailyAvailable");

                    int currentUnixTime = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                    int lastUsedTime = 0;

                    XmlNode lastUsedNode = DailyAvailable.SelectSingleNode("LastUsedUnixTime");
                    if (lastUsedNode != null)
                        int.TryParse(lastUsedNode.InnerText, out lastUsedTime);

                    int elapsedTime = currentUnixTime - lastUsedTime;

                    if (elapsedTime < 0)
                        elapsedTime = 0; // in case system clock changes backward

                    XmlNode timeElapsedNode = DailyAvailable.SelectSingleNode("TimeElapsed");
                    if (timeElapsedNode != null)
                        timeElapsedNode.InnerText = elapsedTime.ToString();
                    else
                    {
                        XmlElement timeElapsedElem = doc.CreateElement("TimeElapsed");
                        timeElapsedElem.InnerText = elapsedTime.ToString();
                        DailyAvailable.AppendChild(timeElapsedElem);
                    }

                    // Get the updated XML string
                    xmlProfile = doc.DocumentElement.InnerXml.Replace("<root>", string.Empty).Replace("</root>", string.Empty);

                    // Save the updated profile back to the file
                    File.WriteAllText(profilePath, xmlProfile);
                }
            }
            else
                xmlProfile = DefaultNovusPrimeProfile;

            return $"<Response>{xmlProfile}</Response>";
        }

        public static string NovusCompleteMission(byte[] PostData, string boundary, string UserID, string WorkPath)
        {
            string profilePath = $"{WorkPath}/NovusPrime/User_Data/{UserID}.xml";
            string xmlProfile = string.Empty;

            if (File.Exists(profilePath))
                xmlProfile = File.ReadAllText(profilePath);
            else
                xmlProfile = DefaultNovusPrimeProfile;

            try
            {
                var doc = new XmlDocument();

                doc.LoadXml($"<xml>{xmlProfile}</xml>");

                var userProfileMissionsNode = doc.DocumentElement.SelectSingleNode("//Missions");

                if (userProfileMissionsNode != null)
                {
                    using (MemoryStream ms = new MemoryStream(PostData))
                    {
                        var data = MultipartFormDataParser.Parse(ms, boundary);

                        // Retrieve the new MissionId from the parsed data
                        string newMissionId = data.GetParameterValue("MissionId");

                        // Check if the MissionId already exists
                        var MissionNodesList = userProfileMissionsNode.SelectNodes("Mission");
                        bool missionExists = false;

                        foreach (XmlNode MissionNode in MissionNodesList)
                        {
                            if (MissionNode.SelectSingleNode("MissionId").InnerText == newMissionId)
                            {
                                missionExists = true;
                                break;
                            }
                        }

                        // If the mission doesn't exist, add a new entry
                        if (!missionExists)
                        {
                            XmlElement newMissionNode = doc.CreateElement("Mission");

                            XmlElement missionIdNode = doc.CreateElement("MissionId");
                            missionIdNode.InnerText = newMissionId;

                            newMissionNode.AppendChild(missionIdNode);

                            userProfileMissionsNode.AppendChild(newMissionNode);

                            // Save the updated XML to file
                            File.WriteAllText(profilePath, doc.DocumentElement.InnerXml);
                        }
                    }
                }
                else
                    LoggerAccessor.LogWarn($"[HELLFIRE] - User - Missions node not found in the XML: {profilePath}.");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[User] - NovusCompleteMission: An assertion was thrown. (Exception:{ex})");
            }

            return "<Response></Response>";
        }

        public static string UpdateNovusPrimeCharacter(byte[] PostData, string boundary, string UserID, string WorkPath, string cmd)
        {
            string xmlProfile = string.Empty;
            string updatedXMLProfile = string.Empty;

            string cooldownPath = $"{WorkPath}/NovusPrime/User_Data/{UserID}_cooldown.txt";
            string profilePath = $"{WorkPath}/NovusPrime/User_Data/{UserID}.xml";

            if (File.Exists(profilePath))
                xmlProfile = File.ReadAllText(profilePath);
            else
                xmlProfile = DefaultNovusPrimeProfile;

            try
            {
                var doc = new XmlDocument();

                doc.LoadXml("<root>" + xmlProfile + "</root>"); // Wrap the XML string in a root element
                if (doc != null && PostData != null && !string.IsNullOrEmpty(boundary))
                {
                    using (MemoryStream ms = new MemoryStream(PostData))
                    {
                        var data = MultipartFormDataParser.Parse(ms, boundary);
                        // Update the profile values from the provided data
                        // Update the values based on IDs
#pragma warning disable 8602

                        switch (cmd)
                        {
                            case "RequestCharacter":
                                {
                                    string Experience = doc.SelectSingleNode("//Experience").InnerText;
                                    string Level = doc.SelectSingleNode("//Level").InnerText;
                                    string Nebulon = doc.SelectSingleNode("//Nebulon").InnerText;
                                    string TotalNebulonEver = doc.SelectSingleNode("//TotalNebulonEver").InnerText;
                                    return $"<Response><Nebulon>{Nebulon}</Nebulon><TotalNebulonEver>{TotalNebulonEver}</TotalNebulonEver><Level>{Level}</Level><Experience>{Experience}</Experience></Response>";
                                }
                            case "UpdateCharacter":
                                {
                                    doc.SelectSingleNode("//Experience").InnerText = data.GetParameterValue("Experience");
                                    doc.SelectSingleNode("//Level").InnerText = data.GetParameterValue("Level");
                                    doc.SelectSingleNode("//Nebulon").InnerText = data.GetParameterValue("Nebulon");
                                    doc.SelectSingleNode("//TotalNebulonEver").InnerText = data.GetParameterValue("TotalNebulonEver");

                                    #region Leaderboard entry update

                                    if (Leaderboards.NovusLeaderboard == null)
                                        Leaderboards.NovusLeaderboard = new InterGalacticScoreBoardData(LeaderboardDbContext.OnContextBuilding(new DbContextOptionsBuilder<LeaderboardDbContext>(), 0, $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}").Options);

                                    int totalNebulonEver = (int)double.Parse(data.GetParameterValue("TotalNebulonEver"), CultureInfo.InvariantCulture);

                                    _ = Leaderboards.NovusLeaderboard.UpdateScoreAsync(UserID, totalNebulonEver);
                                    #endregion
                                }
                                break;
                            case "RequestInventory":
                                {
                                    var inventoryNode = doc.SelectSingleNode("//Inventory");
                                    return $"<Response>{inventoryNode}</Response>";
                                }
                            case "AddInventory":
                                {
                                    string baseName = "Item";
                                    int index = 1;
                                    string newObjectId = $"{baseName}{index}";

                                    var inventoryNode = doc.SelectSingleNode("//Inventory");

                                    var inventoryItems = inventoryNode.SelectNodes("*");
                                    var existingIds = new HashSet<string>();

                                    foreach (XmlNode item in inventoryItems)
                                    {
                                        existingIds.Add(item.Name);
                                    }

                                    while (existingIds.Contains(newObjectId))
                                    {
                                        index++;
                                        newObjectId = $"{baseName}{index}";
                                    }

                                    XmlElement objectNodeEntry = doc.CreateElement(newObjectId);
                                    XmlElement ObjectIdEntry = doc.CreateElement("ObjectId");
                                    XmlElement QuantityEntry = doc.CreateElement("Quantity");

                                    ObjectIdEntry.InnerText = data.GetParameterValue("ObjectId");
                                    QuantityEntry.InnerText = "1";

                                    objectNodeEntry.AppendChild(ObjectIdEntry);
                                    objectNodeEntry.AppendChild(QuantityEntry);

                                    inventoryNode.AppendChild(objectNodeEntry);

                                    doc.SelectSingleNode("//Inventory").AppendChild(objectNodeEntry);
                                }
                                break;

                            case "UseDaily":
                                {
                                    const int ElapsedTime = 0;
                                    int currentUnixTime = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

                                    XmlNode DailyAvailable = doc.SelectSingleNode("//DailyAvailable");
                                    XmlNode lastUsedNode = DailyAvailable.SelectSingleNode("LastUsedUnixTime");
                                    if (DailyAvailable.SelectSingleNode("TimeElapsed") != null)
                                    {
                                        XmlNode timeElapsedExisting = DailyAvailable.SelectSingleNode("TimeElapsed");
                                        timeElapsedExisting.InnerText = ElapsedTime.ToString();
                                        DailyAvailable.AppendChild(timeElapsedExisting);
                                    }
                                    else
                                    {
                                        XmlElement timeElapsed = doc.CreateElement("TimeElapsed");
                                        timeElapsed.InnerText = ElapsedTime.ToString();
                                        DailyAvailable.AppendChild(timeElapsed);
                                    }
                                    if (lastUsedNode != null)
                                        lastUsedNode.InnerText = currentUnixTime.ToString();
                                    else
                                    {
                                        XmlElement lastUsedElem = doc.CreateElement("LastUsedUnixTime");
                                        lastUsedElem.InnerText = currentUnixTime.ToString();
                                        DailyAvailable.AppendChild(lastUsedElem);
                                    }
                                }
                                break;
                            case "RequestShipSlots":
                                {
                                    return $"<Response>{doc.SelectSingleNode("//ShipConfig")}</Response>";
                                }
                            case "ConfigureShip":
                                {
                                    XmlNode shipConfig = doc.SelectSingleNode("//ShipConfig");
                                    shipConfig.SelectSingleNode("Chassis").InnerText = data.GetParameterValue("Chassis");
                                    shipConfig.SelectSingleNode("Front1").InnerText = data.GetParameterValue("Front1");
                                    shipConfig.SelectSingleNode("Front2").InnerText = data.GetParameterValue("Front2");
                                    shipConfig.SelectSingleNode("Turret1").InnerText = data.GetParameterValue("Turret1");
                                    shipConfig.SelectSingleNode("Turret2").InnerText = data.GetParameterValue("Turret2");
                                    shipConfig.SelectSingleNode("Special").InnerText = data.GetParameterValue("Special");
                                    shipConfig.SelectSingleNode("Maneuver").InnerText = data.GetParameterValue("Maneuver");
                                    shipConfig.SelectSingleNode("Upgrade1").InnerText = data.GetParameterValue("Upgrade1");
                                    shipConfig.SelectSingleNode("Upgrade2").InnerText = data.GetParameterValue("Upgrade2");
                                    shipConfig.SelectSingleNode("Upgrade3").InnerText = data.GetParameterValue("Upgrade3");
                                    shipConfig.SelectSingleNode("Upgrade4").InnerText = data.GetParameterValue("Upgrade4");
                                    shipConfig.SelectSingleNode("PaintJob").InnerText = data.GetParameterValue("PaintJob");
                                }
                                break;
                        }

                        // Get the updated XML string
                        updatedXMLProfile = doc.DocumentElement.InnerXml.Replace("<root>", string.Empty).Replace("</root>", string.Empty);
#pragma warning restore 8602
                        // Save the updated profile back to the file
                        File.WriteAllText(profilePath, updatedXMLProfile);
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[User] - UpdateNovusPrimeCharacter: An assertion was thrown. (Exception:{ex})");
            }

            return $"<Response></Response>";
        }

        public static string RequestShipSlots(byte[] PostData, string boundary, string UserID, string WorkPath)
        {
            string xmlProfile = string.Empty;
            string updatedXMLProfile = string.Empty;

            string profilePath = $"{WorkPath}/ClearasilSkater/User_Data/{UserID}.xml";

            if (File.Exists(profilePath))
                xmlProfile = File.ReadAllText(profilePath);
            else
                xmlProfile = DefaultClearasilSkaterAndSlimJimProfile;

            try
            {
                // Create an XmlDocument
                var doc = new XmlDocument();
                doc.LoadXml("<root>" + xmlProfile + "</root>"); // Wrap the XML string in a root element
                if (doc != null && PostData != null && !string.IsNullOrEmpty(boundary))
                {
                    using (MemoryStream ms = new MemoryStream(PostData))
                    {
                        var data = MultipartFormDataParser.Parse(ms, boundary);
                        // Update the profile values from the provided data
                        // Update the values based on IDs
#pragma warning disable 8602
                        doc.SelectSingleNode("//BestScoreStage1").InnerText = data.GetParameterValue("BestScoreStage1");

                        updatedXMLProfile = doc.DocumentElement.InnerXml.Replace("<root>", string.Empty).Replace("</root>", string.Empty);
#pragma warning restore 8602
                        File.WriteAllText(profilePath, updatedXMLProfile);
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[User] - RequestShipSlots: An assertion was thrown. (Exception:{ex})");
            }

            return $"<Response>{updatedXMLProfile}</Response>";
        }

        public static string RequestUserClearasilSkater(byte[] PostData, string boundary, string UserID, string WorkPath)
        {
            string profilePath = $"{WorkPath}/ClearasilSkater/User_Data/{UserID}.xml";

            string xmlProfile;
            if (File.Exists(profilePath))
            {
                LoggerAccessor.LogInfo($"[HELLFIRE] - User - Detected existing player data, sending!");
                xmlProfile = File.ReadAllText(profilePath);
            }
            else
            {
                LoggerAccessor.LogInfo($"[HELLFIRE] - User - New player with no player data! Using default!");
                xmlProfile = DefaultClearasilSkaterAndSlimJimProfile;
            }

            return $"<Response>{xmlProfile}</Response>";
        }

        public static string RequestUserSlimJim(byte[] PostData, string boundary, string UserID, string WorkPath)
        {
            string profilePath = $"{WorkPath}/SlimJim/User_Data/{UserID}.xml";

            string xmlProfile;

            if (File.Exists(profilePath))
                xmlProfile = File.ReadAllText(profilePath);
            else
                xmlProfile = DefaultClearasilSkaterAndSlimJimProfile;

            return $"<Response>{xmlProfile}</Response>";
        }

        public static string RequestUserPoker(byte[] PostData, string boundary, string UserID, string WorkPath)
        {
            string profilePath = $"{WorkPath}/Poker/User_Data/{UserID}.xml";
            string DisplayName = string.Empty;
            string HomeRegion = string.Empty;

            if (PostData != null && !string.IsNullOrEmpty(boundary))
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);
                    DisplayName = data.GetParameterValue("DisplayName");
                    HomeRegion = data.GetParameterValue("Region");
                    ms.Flush();
                }
            }
            
            string xmlProfile;

            if (File.Exists(profilePath))
                xmlProfile = File.ReadAllText(profilePath);
            else
                xmlProfile = DefaultPokerProfile;

            return $"<Response>{xmlProfile}</Response>";
        }

        public static string UpdateUserPoker(byte[] PostData, string boundary, string UserID, string WorkPath)
        {
            string Bankroll = string.Empty;

            if (PostData != null && !string.IsNullOrEmpty(boundary))
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);
                    Bankroll = data.GetParameterValue("Bankroll");
                    ms.Flush();
                }
            }

            string xmlProfile = string.Empty;
            string updatedXMLProfile = string.Empty;

            // Retrieve the user's JSON profile
            string profilePath = $"{WorkPath}/Poker/User_Data/{UserID}.xml";

            if (File.Exists(profilePath))
                xmlProfile = File.ReadAllText(profilePath);
            else
                xmlProfile = DefaultPokerProfile;

            try
            {
                // Create an XmlDocument
                var doc = new XmlDocument();
                doc.LoadXml("<xml>" + xmlProfile + "</xml>"); // Wrap the XML string in a root element
                if (doc != null && PostData != null && !string.IsNullOrEmpty(boundary))
                {
                    using (MemoryStream ms = new MemoryStream(PostData))
                    {
                        var data = MultipartFormDataParser.Parse(ms, boundary);
                        // Update the profile values from the provided data
                        // Update the values based on IDs
#pragma warning disable 8602
                        doc.SelectSingleNode("//Bankroll").InnerText = Bankroll;

                        // Get the updated XML string
                        updatedXMLProfile = doc.DocumentElement.InnerXml.Replace("<xml>", string.Empty).Replace("</xml>", string.Empty);
#pragma warning restore 8602
                        // Save the updated profile back to the file
                        File.WriteAllText(profilePath, updatedXMLProfile);

                        ms.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[User] - UpdateUserPoker: An assertion was thrown. (Exception:{ex})");
            }

            return $"<Response>{updatedXMLProfile}</Response>";
        }
    }
}
