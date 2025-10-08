using CustomLogger;
using HttpMultipartParser;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using WebAPIService.GameServices.HELLFIRE.Helpers.NovusPrime;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.HELLFIRE.Helpers
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
            <Inventory></Inventory>";

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

        public static string GetUserHomeTycoon(byte[] PostData, string boundary, string UserID, string WorkPath)
        {
            string profilePath = $"{WorkPath}/TYCOON/User_Data/{UserID}/Profile.xml";

            string xmlProfile;

            if (File.Exists(profilePath))
                xmlProfile = File.ReadAllText(profilePath);
            else
                xmlProfile = DefaultHomeTycoonProfile;

            return $"<Response>{xmlProfile}</Response>";
        }


        public static string UpdateUserHomeTycoon(byte[] PostData, string boundary, string UserID, string WorkPath, string cmd)
        {
            string xmlProfile = string.Empty;
            string updatedXMLProfile = string.Empty;

            // Retrieve the user's JSON profile
            string profilePath = $"{WorkPath}/TYCOON/User_Data/{UserID}/Profile.xml";

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
                                    var userProfileUnlockNode = doc.SelectSingleNode("//Unlocked");

                                    //We check if HomeTycoon sends multiple AddUnlockeds for same building.. we don't need dupe entries.
                                    if (userProfileUnlockNode.SelectSingleNode(BuildingName) != null)
                                        return "<Response></Response>";

                                    XmlElement BuildingToAddEntry = doc.CreateElement(BuildingName);
                                    BuildingToAddEntry.InnerText = BuildingName;

                                    userProfileUnlockNode.AppendChild(BuildingToAddEntry);
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
                                            doc.DocumentElement.SelectSingleNode("//Unlocked").RemoveChild(buildingNode);
                                        else
                                            LoggerAccessor.LogWarn($"[HELLFIRE] - User - Building not found: {buildingName}");
                                    }
                                    else
                                        LoggerAccessor.LogWarn($"[HELLFIRE] - User - Unlocked node not found in the XML");
                                }
                                break;
                            case "AddDialog":
                                {
                                    string DialogName = data.GetParameterValue("DialogName");

                                    var userProfileDialogNode = doc.SelectSingleNode("//Dialogs");
                                    XmlElement DialogToAdd = doc.CreateElement(DialogName);
                                    DialogToAdd.InnerText = DialogName;
                                    userProfileDialogNode.AppendChild(DialogToAdd);
                                }
                                break;
                            case "CompleteDialog":
                                {
                                    doc.DocumentElement.SelectSingleNode("//Dialogs").RemoveChild(
                                        doc.DocumentElement.SelectSingleNode("//Dialogs").SelectSingleNode(data.GetParameterValue("DialogName")));
                                }
                                break;
                            case "AddVehicle":
                                {
                                    string VehicleName = data.GetParameterValue("VehicleName");

                                    // Update the profile values from the provided data
                                    var userProfileVehicleNode = doc.SelectSingleNode("//Vehicles");
                                    XmlElement VeicleToAdd = doc.CreateElement(VehicleName);
                                    VeicleToAdd.InnerText = VehicleName;
                                    userProfileVehicleNode.AppendChild(VeicleToAdd);
                                }
                                break;
                            case "RemoveVehicle":
                                {
                                    var userProfileVehicleNode = doc.DocumentElement.SelectSingleNode("//Vehicles");
                                    var VeicleToRemove = userProfileVehicleNode.SelectSingleNode(data.GetParameterValue("VehicleName"));
                                    userProfileVehicleNode.RemoveChild(userProfileVehicleNode);
                                }
                                break;
                            case "AddInventory":
                                {
                                    string BuildingID = data.GetParameterValue("BuildingID");

                                    var userProfileInvNode = doc.SelectSingleNode("//Inventory");
                                    XmlElement BuildingToAdd = doc.CreateElement(BuildingID);
                                    BuildingToAdd.InnerText = BuildingID;
                                    userProfileInvNode.AppendChild(BuildingToAdd);
                                }
                                break;
                            case "AddActivity":
                                {
                                    string ActivityName = data.GetParameterValue("ActivityName");
                                    var userProfileFlagNode = doc.SelectSingleNode("//Activities");

                                    XmlElement BuildingToAddEntry = doc.CreateElement(ActivityName);
                                    BuildingToAddEntry.InnerText = ActivityName;

                                    userProfileFlagNode.AppendChild(BuildingToAddEntry);
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
                                        {
                                            doc.DocumentElement.SelectSingleNode("//Activities").RemoveChild(buildingNode);

                                            // Save the updated XML back to the file
                                            File.WriteAllText(profilePath, doc.InnerXml);
                                        }
                                        else
                                            LoggerAccessor.LogWarn($"[HELLFIRE] - User - Activity not found: {ActivityName}");
                                    }
                                    else
                                        LoggerAccessor.LogWarn("[HELLFIRE] - User - Activities node not found in the XML.");
                                }
                                break;
                            case "AddMission":
                                {
                                    string MissionName = data.GetParameterValue("MissionName");

                                    // Update the profile values from the provided data
                                    var userProfileMissionsNode = doc.SelectSingleNode("//Missions");
                                    XmlElement MissionToAddEntry = doc.CreateElement(MissionName);
                                    MissionToAddEntry.InnerText = MissionName;
                                    userProfileMissionsNode.AppendChild(MissionToAddEntry);
                                }
                                break;
                            case "CompleteMission":
                                {
                                    try
                                    {
                                        var MissionsNode = doc.DocumentElement.SelectSingleNode("//Missions");
                                        var userProfileMissionsNode = MissionsNode.SelectSingleNode(data.GetParameterValue("MissionName"));
                                        MissionsNode.RemoveChild(userProfileMissionsNode);
                                    }
                                    catch (Exception ex)
                                    {
                                        LoggerAccessor.LogError($"[HELLFIRE] - User - CompleteMission: Exception caught: {ex}");
                                    }
                                }
                                break;
                            case "AddMissionToJournal":
                                {
                                    // Update the profile values from the provided data
                                    var userProfileMissionsNode = doc.SelectSingleNode("//Journal");
                                    XmlElement MissionToAddEntry = doc.CreateElement(data.GetParameterValue("MissionName"));
                                    MissionToAddEntry.InnerText = data.GetParameterValue("MissionName");
                                    userProfileMissionsNode.AppendChild(MissionToAddEntry);
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
                                        {
                                            doc.DocumentElement.SelectSingleNode("//Journal").RemoveChild(buildingNode);

                                            // Save the updated XML back to the file
                                            File.WriteAllText(profilePath, doc.InnerXml);
                                        }
                                        else
                                            LoggerAccessor.LogWarn($"[HELLFIRE] - User - Mission not found: {MissionName}");
                                    }
                                    else
                                        LoggerAccessor.LogWarn("[HELLFIRE] - User - Journal node not found in the XML.");
                                }
                                break;
                            case "AddFlag":
                                {
                                    var userProfileFlagNode = doc.SelectSingleNode("//Flags");

                                    XmlElement FlagEntry = doc.CreateElement(data.GetParameterValue("Flag"));
                                    FlagEntry.InnerText = data.GetParameterValue("Flag");

                                    userProfileFlagNode.AppendChild(FlagEntry);
                                }
                                break;
                            case "SpendCoins":
                                {
                                    string coinType = data.GetParameterValue("CoinType");
                                    int NumCoins = Convert.ToInt32(data.GetParameterValue("NumCoins"));
                                    string TransType = data.GetParameterValue("TransType");
                                    string TransParam = data.GetParameterValue("TransParam");

                                    switch (TransType)
                                    {
                                        case "CollectAllRevenue":
                                            {
                                                int profileGoldCoins = Convert.ToInt32(doc.SelectSingleNode("//GoldCoins").InnerText);
                                                doc.SelectSingleNode("//GoldCoins").InnerText = Convert.ToString(profileGoldCoins - NumCoins);
                                                doc.SelectSingleNode("//SilverCoins").InnerText = data.GetParameterValue("SilverCoins") ?? "0";

                                                return $@"<Response>
                                                <ResponseCode>Success</ResponseCode>
                                                <TotalSilver>{doc.SelectSingleNode("//SilverCoins").InnerText}</TotalSilver>
                                                <TotalGold>{doc.SelectSingleNode("//GoldCoins").InnerText}</TotalGold>
                                                <SilverSpent>0</SilverSpent>
                                                <GoldSpent>{NumCoins}</GoldSpent>
                                                </Response>";
                                            }
                                        case "BuyBuilding":
                                            {
                                                return $@"<Response>
                                                <ResponseCode>Success</ResponseCode>
                                                <TotalSilver>{doc.SelectSingleNode("//SilverCoins").InnerText}</TotalSilver>
                                                <TotalGold>{doc.SelectSingleNode("//GoldCoins").InnerText}</TotalGold>
                                                <SilverSpent>0</SilverSpent>
                                                <GoldSpent>{NumCoins}</GoldSpent>
                                                </Response>";
                                            }
                                        case "BuyWorkers":
                                            {
                                                return $@"<Response>
                                                <ResponseCode>Success</ResponseCode>
                                                <TotalSilver>{doc.SelectSingleNode("//SilverCoins").InnerText}</TotalSilver>
                                                <TotalGold>{doc.SelectSingleNode("//GoldCoins").InnerText}</TotalGold>
                                                <SilverSpent>0</SilverSpent>
                                                <GoldSpent>{NumCoins}</GoldSpent>
                                                </Response>";
                                            }
                                        case "BuyVehicles":
                                            {
                                                return $@"<Response>
                                                <ResponseCode>Success</ResponseCode>
                                                <TotalSilver>{doc.SelectSingleNode("//SilverCoins").InnerText}</TotalSilver>
                                                <TotalGold>{doc.SelectSingleNode("//GoldCoins").InnerText}</TotalGold>
                                                <SilverSpent>0</SilverSpent>
                                                <GoldSpent>{NumCoins}</GoldSpent>
                                                </Response>";
                                            }
                                        case "BuyExpansion":
                                            {
                                                return $@"<Response>
                                                <ResponseCode>Success</ResponseCode>
                                                <TotalSilver>{doc.SelectSingleNode("//SilverCoins").InnerText}</TotalSilver>
                                                <TotalGold>{doc.SelectSingleNode("//GoldCoins").InnerText}</TotalGold>
                                                <SilverSpent>0</SilverSpent>
                                                <GoldSpent>{NumCoins}</GoldSpent>
                                                </Response>";
                                            }
                                        case "BuyDollars":
                                            {
                                                return $@"<Response>
                                                <ResponseCode>Success</ResponseCode>
                                                <TotalSilver>{doc.SelectSingleNode("//SilverCoins").InnerText}</TotalSilver>
                                                <TotalGold>{doc.SelectSingleNode("//GoldCoins").InnerText}</TotalGold>
                                                <SilverSpent>0</SilverSpent>
                                                <GoldSpent>{NumCoins}</GoldSpent>
                                                </Response>";
                                            }
                                        case "BuySuburb":
                                            {
                                                return $@"<Response>
                                                <ResponseCode>Success</ResponseCode>
                                                <TotalSilver>{doc.SelectSingleNode("//SilverCoins").InnerText}</TotalSilver>
                                                <TotalGold>{doc.SelectSingleNode("//GoldCoins").InnerText}</TotalGold>
                                                <SilverSpent>0</SilverSpent>
                                                <GoldSpent>{NumCoins}</GoldSpent>
                                                </Response>";
                                            }
                                        case "BuyTimeOfDay":
                                            {
                                                return $@"<Response>
                                                <ResponseCode>Success</ResponseCode>
                                                <TotalSilver>{doc.SelectSingleNode("//SilverCoins").InnerText}</TotalSilver>
                                                <TotalGold>{doc.SelectSingleNode("//GoldCoins").InnerText}</TotalGold>
                                                <SilverSpent>0</SilverSpent>
                                                <GoldSpent>{NumCoins}</GoldSpent>
                                                </Response>";
                                            }
                                    }


                                }
                                break;

                            case "SetPrivacy":
                                {
                                    string ownID = data.GetParameterValue("TownID");
                                    string NewSetting = data.GetParameterValue("NewSetting");

                                    var userProfileOptionsNode = doc.DocumentElement.SelectSingleNode("//Options");

                                    userProfileOptionsNode.SelectSingleNode("PrivacySetting").InnerText = NewSetting;

                                    //Write a server txt for player names we can ban?
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
                                    var jsonObject = JToken.Parse(data.GetParameterValue("Options"));
                                    var fieldValues = jsonObject.ToObject<Dictionary<string, object>>();

                                    if (fieldValues != null)
                                    {
                                        foreach (var fieldValue in fieldValues)
                                        {
                                            if (fieldValue.Key == "MusicVolume" && fieldValue.Value != null)
                                                doc.SelectSingleNode("//Options/MusicVolume").InnerText = fieldValue.Value.ToString() ?? "1.0";
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
                LoggerAccessor.LogError($"[HFGAMES] User - An assertion was thrown in UpdateUser : {ex}");
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
                LoggerAccessor.LogError($"[HELLFIRE] - User - An assertion was thrown in UpdateUser : {ex}");
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
                LoggerAccessor.LogError($"[HELLFIRE] - User - An assertion was thrown in UpdateUser : {ex}");
            }

            return $"<Response>{updatedXMLProfile}</Response>";
        }

        public static string RequestInitialDataNovusPrime(byte[] PostData, string boundary, string UserID, string WorkPath)
        {
            string profilePath = $"{WorkPath}/NovusPrime/User_Data/{UserID}.xml";

            string xmlProfile;
            if (File.Exists(profilePath))
            {
                LoggerAccessor.LogInfo($"[HELLFIRE] - User - Detected existing player data, sending!");
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
            {
                LoggerAccessor.LogInfo($"[[HELLFIRE] - User - New player with no player data! Using default!");
                xmlProfile = DefaultNovusPrimeProfile;
            }

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

            return "<Response></Response>";
        }

        public static string UpdateCharacter(byte[] PostData, string boundary, string UserID, string WorkPath, string cmd)
        {
            //userId
            //Experience
            //Level
            //Nebulon
            //TotalNebulonEver

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
                var lbDoc = new XmlDocument();

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

                                    XmlNode userNameExistEntry = lbDoc.SelectSingleNode($"//{UserID}");

                                    if (userNameExistEntry != null)
                                    {
                                        XmlNode scoreNode = userNameExistEntry.SelectSingleNode("Score");
                                        if (scoreNode == null)
                                        {
                                            scoreNode = lbDoc.CreateElement("Score");
                                            userNameExistEntry.AppendChild(scoreNode);
                                        }
                                        scoreNode.InnerText = totalNebulonEver.ToString();
                                    }
                                    else
                                    {
                                        XmlElement userNameElement = lbDoc.CreateElement(UserID);
                                        XmlElement displayNameElement = lbDoc.CreateElement("DisplayName");
                                        XmlElement scoreElement = lbDoc.CreateElement("Score");

                                        displayNameElement.InnerText = UserID;
                                        scoreElement.InnerText = totalNebulonEver.ToString();

                                        userNameElement.AppendChild(displayNameElement);
                                        userNameElement.AppendChild(scoreElement);

                                        lbDoc.DocumentElement.AppendChild(userNameElement);
                                    }
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

                        ms.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[HELLFIRE] - User - An assertion was thrown in UpdateUser : {ex}");
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

                        ms.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[HELLFIRE] - User - An assertion was thrown in UpdateUser : {ex}");
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
                LoggerAccessor.LogError($"[HELLFIRE] - User - An assertion was thrown in UpdateUser : {ex}");
            }

            return $"<Response>{updatedXMLProfile}</Response>";
        }
    }
}
