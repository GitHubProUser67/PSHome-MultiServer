using System.Collections.Concurrent;
using System.Text;
using System.Xml;
using CastleLibrary.NetHasher;
using CastleLibrary.NetHasher.CRC;
using CustomLogger;
using HttpMultipartParser;
using MultiServerLibrary.Extension;
using WebAPIService.GameServices.PSHOME.HELLFIRE.Entities.HomeTycoon;

namespace WebAPIService.GameServices.PSHOME.HELLFIRE.Helpers.Tycoon
{
    internal class TownInstance
    {
        private static readonly object _Lock = new();

        // Instance index: InstanceID -> (TownName, UserID)
        private static readonly ConcurrentDictionary<
            string,
            (string townName, string userId)
        > InstanceIndex = new();

        private static int _indexInitialized = 0;

        public const ushort gridSize = 256;

        public static string RequestDefaultTownInstance()
        {
            return $"<Response><InstanceID>{GenerateTycoonId(DotNetHasher.ComputeMD5String(Encoding.ASCII.GetBytes("WANAPLAY?!!!!m3TycoonN0?w*")), string.Empty)}</InstanceID></Response>";
        }

        public static string RequestTownInstance(
            string UserID,
            string DisplayName,
            string TownID,
            string WorkPath
        )
        {
            if (uint.TryParse(TownID, out var intTownID))
                return $"<Response><InstanceID>{GenerateTownInstanceID(intTownID)}</InstanceID></Response>";
            // Read last used city or creates default city.
            else
            {
                var userName = string.IsNullOrEmpty(DisplayName) ? UserID : DisplayName;

                if (!string.IsNullOrEmpty(userName))
                {
                    var InstanceID = GetCurrentSuburpInstanceID(userName, WorkPath);

                    if (InstanceID != null)
                    {
                        return InstanceID == "EMPTY"
                            ? $"<Response><InstanceID>{CreateDefaultSuburp(userName, WorkPath)}</InstanceID></Response>"
                            : $"<Response><InstanceID>{InstanceID}</InstanceID></Response>";
                    }
                }
            }

            return $"<Response></Response>";
        }

        public static string RequestTown(string InstanceID, string WorkPath)
        {
            var TownParams = RequestTownNameByInstanceID(InstanceID, WorkPath);

            if (TownParams == null) // Failure (should not happen)
                return $"<Response></Response>";

            var TownID = TownNameToID(TownParams.Value.Item1).ToString();
            var UserID = TownParams.Value.Item2;

            var townsDirPath = $"{WorkPath}/HomeTycoon/TownsData/{UserID}";
            var townStorageFilePath = $"{townsDirPath}/{TownID}.xml";

            Directory.CreateDirectory(townsDirPath);

            if (File.Exists(townStorageFilePath))
                return $"<Response>{File.ReadAllText(townStorageFilePath)}</Response>";
            else
            {
                var gridBuilder = new StringBuilder();

                for (var i = 1; i <= gridSize; i++)
                {
                    gridBuilder.Append($"<{i}.000000>0</{i}.000000>");
                }

                var xml =
                    $"<UserID>{UserID}</UserID><DisplayName>{UserID}</DisplayName>"
                    + $"<TownID>{TownID}</TownID>"
                    + $"<InstanceID>{InstanceID}</InstanceID><LastVisited>{DateTimeUtils.GetUnixTime()}</LastVisited><NumPlayers>0</NumPlayers><Privacy>1</Privacy><Grid>{gridBuilder}</Grid>";

                File.WriteAllText(townStorageFilePath, xml);

                return $"<Response>{xml}</Response>";
            }
        }

        public static string GetCurrentSuburpInstanceID(string DisplayName, string WorkPath)
        {
            var userDataPath = $"{WorkPath}/HomeTycoon/User_Data/{DisplayName}";

            var profilePath = $"{userDataPath}/Profile.xml";
            var xmlProfile = File.Exists(profilePath)
                ? File.ReadAllText(profilePath)
                : User.DefaultHomeTycoonProfile;

            try
            {
                var doc = new XmlDocument();
                doc.LoadXml("<root>" + xmlProfile + "</root>");

                var instanceNode = doc.SelectSingleNode("//InstanceID");

                return instanceNode != null && !string.IsNullOrWhiteSpace(instanceNode.InnerText)
                    ? instanceNode.InnerText
                    : "EMPTY";
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(
                    $"[TownInstance] - GetCurrentSuburpInstanceID: Failed picking current InstanceID (Exception:{ex})"
                );
            }

            return null;
        }

        public static string CreateDefaultSuburp(string UserID, string WorkPath)
        {
            var TownName = $"{UserID}_Town_1";
            var userDataPath = $"{WorkPath}/HomeTycoon/User_Data/{UserID}";

            Directory.CreateDirectory(userDataPath);

            var profilePath = $"{userDataPath}/Profile.xml";
            var xmlProfile = File.Exists(profilePath)
                ? File.ReadAllText(profilePath)
                : User.DefaultHomeTycoonProfile;

            try
            {
                var doc = new XmlDocument();
                doc.LoadXml("<root>" + xmlProfile + "</root>");

                var InstanceID = GenerateTownInstanceID(TownNameToID(TownName));

                var townsNode = doc.SelectSingleNode("//Towns");
                if (townsNode != null)
                {
                    var firstTown = doc.CreateElement(TownName);

                    var nameEl = doc.CreateElement("Name");
                    nameEl.InnerText = TownName;
                    firstTown.AppendChild(nameEl);

                    var instEl = doc.CreateElement("InstanceID");
                    instEl.InnerText = InstanceID;
                    firstTown.AppendChild(instEl);

                    townsNode.AppendChild(firstTown);
                }

                doc.SelectSingleNode("//InstanceID").InnerText = InstanceID;

                File.WriteAllText(
                    profilePath,
                    doc.DocumentElement.InnerXml.Replace("<root>", string.Empty)
                        .Replace("</root>", string.Empty)
                );

                InstanceIndex[InstanceID] = (TownName, UserID);

                return InstanceID;
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(
                    $"[TownInstance] - CreateDefaultSuburp: Failed creating default entry (Exception:{ex})"
                );
            }

            return null;
        }

        public static string CreateSuburp(string UserID, string WorkPath)
        {
            var userDataPath = $"{WorkPath}/HomeTycoon/User_Data/{UserID}";
            Directory.CreateDirectory(userDataPath);

            var profilePath = $"{userDataPath}/Profile.xml";
            var xmlProfile = File.Exists(profilePath)
                ? File.ReadAllText(profilePath)
                : User.DefaultHomeTycoonProfile;

            try
            {
                var doc = new XmlDocument();
                doc.LoadXml("<root>" + xmlProfile + "</root>");

                var townsNode = doc.SelectSingleNode("//Towns");
                if (townsNode != null)
                {
                    var maxId = 0;

                    // Find highest existing numeric town id
                    foreach (XmlNode node in townsNode.ChildNodes)
                    {
                        if (
                            int.TryParse(
                                node.Name.Replace("Town", string.Empty)
                                    .Replace(UserID, string.Empty)
                                    .Replace("_", string.Empty),
                                out var id
                            )
                        )
                        {
                            if (id > maxId)
                                maxId = id;
                        }
                    }

                    // new town id = next one
                    var nextId = maxId + 1;
                    var newTownName = $"{UserID}_Town_{nextId}";
                    var TownID = TownNameToID(newTownName);
                    var InstanceID = GenerateTownInstanceID(TownID);

                    // Only add if not already there
                    if (townsNode.SelectSingleNode(newTownName) == null)
                    {
                        var newTown = doc.CreateElement(newTownName);

                        var nameEl = doc.CreateElement("Name");
                        nameEl.InnerText = newTownName;
                        newTown.AppendChild(nameEl);

                        var instEl = doc.CreateElement("InstanceID");
                        instEl.InnerText = InstanceID;
                        newTown.AppendChild(instEl);

                        townsNode.AppendChild(newTown);
                    }

                    doc.SelectSingleNode("//InstanceID").InnerText = InstanceID;

                    File.WriteAllText(
                        profilePath,
                        doc.DocumentElement.InnerXml.Replace("<root>", string.Empty)
                            .Replace("</root>", string.Empty)
                    );

                    InstanceIndex[InstanceID] = (newTownName, UserID);

                    return $"<Response><TownID>{TownID}</TownID></Response>";
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(
                    $"[TownInstance] - CreateSuburp: Failed updating cities (Exception:{ex})"
                );
            }

            return "<Response></Response>";
        }

        public static List<string> RequestTownsName(string UserID, string WorkPath)
        {
            List<string> townNames = [];
            var userDataPath = $"{WorkPath}/HomeTycoon/User_Data/{UserID}";

            var profilePath = $"{userDataPath}/Profile.xml";

            var xmlProfile = File.Exists(profilePath)
                ? File.ReadAllText(profilePath)
                : User.DefaultHomeTycoonProfile;
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
                LoggerAccessor.LogError(
                    $"[TownInstance] - RequestTownsName: An assertion was thrown while grabbing user Cities name. (Exception:{ex})"
                );
            }

            return townNames;
        }

        public static (string, string)? RequestTownNameByInstanceID(
            string InstanceID,
            string WorkPath
        )
        {
            return InstanceIndex.TryGetValue(InstanceID, out var value) ? value : null;
        }

        private static void PrepareInitialTownNameByInstanceIDLookup(string WorkPath)
        {
            var searchDir = $"{WorkPath}/HomeTycoon/User_Data";

            try
            {
                Directory.CreateDirectory(searchDir);

                var doc = new XmlDocument();

                lock (_Lock)
                {
                    foreach (
                        var profilePath in Directory.GetFiles(
                            searchDir,
                            "*.*",
                            SearchOption.AllDirectories
                        )
                    )
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
                                    if (instanceNode != null)
                                    {
                                        var nameNode = townNode.SelectSingleNode("Name");
                                        if (nameNode != null)
                                            InstanceIndex[instanceNode.InnerText] = (
                                                nameNode.InnerText,
                                                Path.GetFileName(Path.GetDirectoryName(profilePath))
                                            );
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(
                    $"[TownInstance] - PrepareInitialTownNameByInstanceIDLookup: An assertion was thrown while grabbing user Cities name. (Exception:{ex})"
                );
            }
        }

        public static async Task EnsureIndexLoadedAsync(string WorkPath)
        {
            if (Interlocked.CompareExchange(ref _indexInitialized, 1, 0) == 0)
                await Task.Run(() => PrepareInitialTownNameByInstanceIDLookup(WorkPath))
                    .ConfigureAwait(false);
        }

        public static string RequestTowns(
            byte[] PostData,
            string boundary,
            string UserID,
            string DisplayName,
            string WorkPath
        )
        {
            var Query = string.Empty;
            string[] Friends = [];

            if (PostData != null && !string.IsNullOrEmpty(boundary))
            {
                using (var ms = new MemoryStream(PostData))
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

            var i = 0;
            var menuBuilder = new StringBuilder("<Response>");

            // TODO, implement the other modes.
            switch (Query)
            {
                case "Mine":
                    foreach (var townName in RequestTownsName(UserID, WorkPath))
                    {
                        var TownID = TownNameToID(townName).ToString();

                        menuBuilder.Append(
                            $"<{i}><DisplayName>{DisplayName}</DisplayName><TownID>{TownID}</TownID><ExtraData>{TownProcessor.GetTownPlayers(UserID, TownID, WorkPath)}</ExtraData></{i}>"
                        );

                        i++;
                    }
                    break;
                case "Friends":
                    foreach (var friend in Friends)
                    {
                        foreach (var townName in RequestTownsName(friend, WorkPath))
                        {
                            var TownID = TownNameToID(townName).ToString();
                            var privacySetting = TownProcessor.GetTownPrivacy(
                                friend,
                                TownID,
                                WorkPath
                            );

                            if (
                                privacySetting == TycoonPrivacySetting.Public
                                || privacySetting == TycoonPrivacySetting.FriendsOnly
                            )
                            {
                                menuBuilder.Append(
                                    $"<{i}><DisplayName>{DisplayName}</DisplayName><TownID>{TownID}</TownID><ExtraData>{TownProcessor.GetTownPlayers(friend, TownID, WorkPath)}</ExtraData></{i}>"
                                );

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
            var hash = DotNetHasher.ComputeMD5String(
                Encoding.ASCII.GetBytes(TownID + "G0TOH00000!!!!m3TycoonN0?w*")
            );
            return GenerateTycoonId(hash, TownID + hash);
        }

        private static string GenerateTycoonId(string input1, string input2)
        {
            // We must repect a number limit of 65535, so we use this CRC method to not get out of bounds.
            return CRC16.Create(Encoding.ASCII.GetBytes(input1 + "|" + input2)).ToString();
        }
    }
}
