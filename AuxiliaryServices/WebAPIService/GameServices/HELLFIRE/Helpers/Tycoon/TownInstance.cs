using CustomLogger;
using HttpMultipartParser;
using MultiServerLibrary.Extension;
using NetHasher;
using NetHasher.CRC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using WebAPIService.GameServices.HELLFIRE.Entities.HomeTycoon;

namespace WebAPIService.GameServices.HELLFIRE.Helpers.Tycoon
{
    internal class TownInstance
    {
        private static object _Lock = new object();

        public const ushort gridSize = 256;

        public static string RequestDefaultTownInstance()
        {
            return $"<Response><InstanceID>{GenerateTycoonId(DotNetHasher.ComputeMD5String(Encoding.ASCII.GetBytes("WANAPLAY?!!!!m3TycoonN0?w*")), string.Empty)}</InstanceID></Response>";
        }

        public static string RequestTownInstance(string UserID, string DisplayName, string TownID, string WorkPath)
        {
            if (uint.TryParse(TownID, out uint intTownID))
                return $"<Response><InstanceID>{GenerateTownInstanceID(intTownID)}</InstanceID></Response>";
            // Read last used city or creates default city.
            else
            {
                string userName = string.IsNullOrEmpty(DisplayName) ? UserID : DisplayName;

                if (!string.IsNullOrEmpty(userName))
                {
                    string InstanceID = GetCurrentSuburpInstanceID(userName, WorkPath);

                    if (InstanceID != null)
                    {
                        if (InstanceID == "EMPTY") // No cities registered yet.
                            return $"<Response><InstanceID>{CreateDefaultSuburp(userName, WorkPath)}</InstanceID></Response>";
                        else
                            return $"<Response><InstanceID>{InstanceID}</InstanceID></Response>";
                    }
                }
            }

            return $"<Response></Response>";
        }

        public static string RequestTown(string InstanceID, string WorkPath)
        {
            (string, string)? TownParams = RequestTownNameByInstanceID(InstanceID, WorkPath);

            if (TownParams == null) // Failure (should not happen)
                return $"<Response></Response>";

            string TownID = TownNameToID(TownParams.Value.Item1).ToString();
            string UserID = TownParams.Value.Item2;

            string townsDirPath = $"{WorkPath}/HomeTycoon/TownsData/{UserID}";
            string townStorageFilePath = $"{townsDirPath}/{TownID}.xml";

            Directory.CreateDirectory(townsDirPath);

            if (File.Exists(townStorageFilePath))
                return $"<Response>{File.ReadAllText(townStorageFilePath)}</Response>";
            else
            {
                StringBuilder gridBuilder = new StringBuilder();

                for (int i = 1; i <= gridSize; i++)
                {
                    gridBuilder.Append($"<{i}.000000>0</{i}.000000>");
                }

                string xml = $"<UserID>{UserID}</UserID><DisplayName>{UserID}</DisplayName>" +
                    $"<TownID>{TownID}</TownID>" +
                    $"<InstanceID>{InstanceID}</InstanceID><LastVisited>{DateTimeUtils.GetUnixTime()}</LastVisited><NumPlayers>0</NumPlayers><Privacy>1</Privacy><Grid>{gridBuilder}</Grid>";

                File.WriteAllText(townStorageFilePath, xml);

                return $"<Response>{xml}</Response>";
            }
        }

        public static string GetCurrentSuburpInstanceID(string DisplayName, string WorkPath)
        {
            string xmlProfile = string.Empty;
            string userDataPath = $"{WorkPath}/HomeTycoon/User_Data/{DisplayName}";

            string profilePath = $"{userDataPath}/Profile.xml";
            xmlProfile = File.Exists(profilePath) ? File.ReadAllText(profilePath) : User.DefaultHomeTycoonProfile;

            try
            {
                var doc = new XmlDocument();
                doc.LoadXml("<root>" + xmlProfile + "</root>");

                var instanceNode = doc.SelectSingleNode("//InstanceID");

                return (instanceNode != null && !string.IsNullOrWhiteSpace(instanceNode.InnerText))
                                        ? instanceNode.InnerText
                                        : "EMPTY";
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[TownInstance] - GetCurrentSuburpInstanceID: Failed picking current InstanceID (Exception:{ex})");
            }

            return null;
        }

        public static string CreateDefaultSuburp(string UserID, string WorkPath)
        {
            string xmlProfile = string.Empty;
            string TownName = $"{UserID}_Town_1";
            string userDataPath = $"{WorkPath}/HomeTycoon/User_Data/{UserID}";

            Directory.CreateDirectory(userDataPath);

            string profilePath = $"{userDataPath}/Profile.xml";
            xmlProfile = File.Exists(profilePath) ? File.ReadAllText(profilePath) : User.DefaultHomeTycoonProfile;

            try
            {
                var doc = new XmlDocument();
                doc.LoadXml("<root>" + xmlProfile + "</root>");

                string InstanceID = GenerateTownInstanceID(TownNameToID(TownName));

                var townsNode = doc.SelectSingleNode("//Towns");
                if (townsNode != null)
                {
                    XmlElement firstTown = doc.CreateElement(TownName);

                    XmlElement nameEl = doc.CreateElement("Name");
                    nameEl.InnerText = TownName;
                    firstTown.AppendChild(nameEl);

                    XmlElement instEl = doc.CreateElement("InstanceID");
                    instEl.InnerText = InstanceID;
                    firstTown.AppendChild(instEl);

                    townsNode.AppendChild(firstTown);
                }

                doc.SelectSingleNode("//InstanceID").InnerText = InstanceID;

                File.WriteAllText(profilePath,
                    doc.DocumentElement.InnerXml.Replace("<root>", string.Empty).Replace("</root>", string.Empty));

                return InstanceID;
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[TownInstance] - CreateDefaultSuburp: Failed creating default entry (Exception:{ex})");
            }

            return null;
        }

        public static string CreateSuburp(string UserID, string WorkPath)
        {
            string xmlProfile = string.Empty;
            string userDataPath = $"{WorkPath}/HomeTycoon/User_Data/{UserID}";
            Directory.CreateDirectory(userDataPath);

            string profilePath = $"{userDataPath}/Profile.xml";
            xmlProfile = File.Exists(profilePath) ? File.ReadAllText(profilePath) : User.DefaultHomeTycoonProfile;

            try
            {
                var doc = new XmlDocument();
                doc.LoadXml("<root>" + xmlProfile + "</root>");

                var townsNode = doc.SelectSingleNode("//Towns");
                if (townsNode != null)
                {
                    int maxId = 0;

                    // Find highest existing numeric town id
                    foreach (XmlNode node in townsNode.ChildNodes)
                    {
                        if (int.TryParse(node.Name.Replace("Town", string.Empty).Replace(UserID, string.Empty).Replace("_", string.Empty), out int id))
                        {
                            if (id > maxId)
                                maxId = id;
                        }
                    }

                    // new town id = next one
                    int nextId = maxId + 1;
                    string newTownName = $"{UserID}_Town_{nextId}";
                    uint TownID = TownNameToID(newTownName);
                    string InstanceID = GenerateTownInstanceID(TownID);

                    // Only add if not already there
                    if (townsNode.SelectSingleNode(newTownName) == null)
                    {
                        XmlElement newTown = doc.CreateElement(newTownName);

                        XmlElement nameEl = doc.CreateElement("Name");
                        nameEl.InnerText = newTownName;
                        newTown.AppendChild(nameEl);

                        XmlElement instEl = doc.CreateElement("InstanceID");
                        instEl.InnerText = InstanceID;
                        newTown.AppendChild(instEl);

                        townsNode.AppendChild(newTown);
                    }

                    doc.SelectSingleNode("//InstanceID").InnerText = InstanceID;

                    File.WriteAllText(profilePath,
                    doc.DocumentElement.InnerXml.Replace("<root>", string.Empty).Replace("</root>", string.Empty));

                    return $"<Response><TownID>{TownID}</TownID></Response>";
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[TownInstance] - CreateSuburp: Failed updating cities (Exception:{ex})");
            }

            return "<Response></Response>";
        }

        public static List<string> RequestTownsName(string UserID, string WorkPath)
        {
            List<string> townNames = new List<string>();

            string xmlProfile = string.Empty;
            string userDataPath = $"{WorkPath}/HomeTycoon/User_Data/{UserID}";

            string profilePath = $"{userDataPath}/Profile.xml";

            if (File.Exists(profilePath))
                xmlProfile = File.ReadAllText(profilePath);
            else
                xmlProfile = User.DefaultHomeTycoonProfile;

            try
            {
                var doc = new XmlDocument();
                doc.LoadXml("<xml>" + xmlProfile + "</xml>");

                if (doc != null)
                {
                    var townsNode = doc.SelectSingleNode("//Towns");
                    if (townsNode != null)
                    {
                        foreach (XmlNode townNode in townsNode.ChildNodes)
                        {
                            var nameNode = townNode.SelectSingleNode("Name");
                            if (nameNode != null)
                                townNames.Add(nameNode.InnerText);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[TownInstance] - RequestTownsName: An assertion was thrown while grabbing user Cities name. (Exception:{ex})");
            }

            return townNames;
        }

        public static (string, string)? RequestTownNameByInstanceID(string InstanceID, string WorkPath)
        {
            string searchDir = $"{WorkPath}/HomeTycoon/User_Data";

            try
            {
                var doc = new XmlDocument();

                lock (_Lock)
                {
                    foreach (var profilePath in Directory.GetFiles(searchDir, "*.*", SearchOption.AllDirectories))
                    {
                        doc.LoadXml("<xml>" + File.ReadAllText(profilePath) + "</xml>");

                        if (doc != null)
                        {
                            var townsNode = doc.SelectSingleNode("//Towns");
                            if (townsNode != null)
                            {
                                foreach (XmlNode townNode in townsNode.ChildNodes)
                                {
                                    var instanceNode = townNode.SelectSingleNode("InstanceID");
                                    if (instanceNode != null && instanceNode.InnerText == InstanceID)
                                    {
                                        var nameNode = townNode.SelectSingleNode("Name");
                                        if (nameNode != null)
                                            return (nameNode.InnerText, Path.GetFileName(Path.GetDirectoryName(profilePath)));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[TownInstance] - RequestTownName: An assertion was thrown while grabbing user Cities name. (Exception:{ex})");
            }

            return null;
        }

        public static string RequestTowns(byte[] PostData, string boundary, string UserID, string DisplayName, string WorkPath)
        {
            string Query = string.Empty;
            string[] Friends = Array.Empty<string>();

            if (PostData != null && !string.IsNullOrEmpty(boundary))
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);
                    try
                    {
                        Query = data.GetParameterValue("Query");
                        if (Query == "Friends")
                            Friends = data.GetParameterValue("Friends").Split('+');
                    }
                    catch
                    {
                        // Not Important.
                    }
                }
            }

            int i = 0;
            StringBuilder menuBuilder = new StringBuilder("<Response>");

            // TODO, implement the other modes.
            switch (Query)
            {
                case "Mine":
                    foreach (string townName in RequestTownsName(UserID, WorkPath))
                    {
                        string TownID = TownNameToID(townName).ToString();

                        menuBuilder.Append($"<{i}><DisplayName>{DisplayName}</DisplayName><TownID>{TownID}</TownID><ExtraData>{TownProcessor.GetTownPlayers(UserID, TownID, WorkPath)}</ExtraData></{i}>");

                        i++;
                    }
                    break;
                case "Friends":
                    foreach (string friend in Friends)
                    {
                        foreach (string townName in RequestTownsName(friend, WorkPath))
                        {
                            string TownID = TownNameToID(townName).ToString();
                            TycoonPrivacySetting privacySetting = TownProcessor.GetTownPrivacy(friend, TownID, WorkPath);

                            if (privacySetting == TycoonPrivacySetting.Public || privacySetting == TycoonPrivacySetting.FriendsOnly)
                            {
                                menuBuilder.Append($"<{i}><DisplayName>{DisplayName}</DisplayName><TownID>{TownID}</TownID><ExtraData>{TownProcessor.GetTownPlayers(friend, TownID, WorkPath)}</ExtraData></{i}>");

                                i++;
                            }
                        }
                    }
                    break;
                case "Hellfire":
                    break;
                case "Popular":
                    break;
                case "Biggest":
                    break;
                case "Active":
                    break;
            }

            menuBuilder.Append("</Response>");

            return menuBuilder.ToString();
        }

        public static uint TownNameToID(string TownName)
        {
            return CRC32.Create(Encoding.ASCII.GetBytes(TownName));
        }

        private static string GenerateTownInstanceID(uint TownID)
        {
            string hash = DotNetHasher.ComputeMD5String(Encoding.ASCII.GetBytes(TownID + "G0TOH00000!!!!m3TycoonN0?w*"));
            return GenerateTycoonId(hash, TownID + hash);
        }

        private static string GenerateTycoonId(string input1, string input2)
        {
            // We must repect a number limit, so we use this CRC method to not get out of bounds.
            return CRC16.Create(Encoding.ASCII.GetBytes(input1 + "|" + input2)).ToString() + CRC8.Create(Encoding.ASCII.GetBytes(input1));
        }
    }
}
