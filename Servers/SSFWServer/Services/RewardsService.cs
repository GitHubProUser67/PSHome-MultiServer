using System.Text;
using System.Text.Json;
using CustomLogger;
using MultiServerLibrary.Extension;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SSFWServer.Helpers.FileHelper;
using SSFWServer.Helpers.RegexHelper;

namespace SSFWServer.Services
{
    public class RewardsService(string? key)
    {
        private readonly string? key = key;

        public byte[] HandleRewardServicePOST(
            byte[] buffer,
            string directorypath,
            string filepath,
            string absolutepath
        )
        {
            Directory.CreateDirectory(directorypath);

            File.WriteAllBytes(
                $"{SSFWServerConfiguration.SSFWStaticFolder}/{absolutepath}.json",
                buffer
            );

            SSFWUpdateMini(filepath + "/mini.json", Encoding.UTF8.GetString(buffer), false);

            return buffer;
        }

        public static byte[] HandleRewardServiceInvPOST(
            byte[] buffer,
            string directorypath,
            string filepath,
            string absolutepath
        )
        {
            Directory.CreateDirectory(directorypath);

            return RewardServiceInventory(
                buffer,
                directorypath,
                filepath,
                absolutepath,
                false,
                false
            );
        }

        public byte[]? HandleRewardServiceInvCardTrackingDataDELETE(
            string directorypath,
            string filepath,
            string absolutepath,
            string userAgent,
            string sessionId
        )
        {
            AdminObjectService adminObjectService = new(sessionId, key);
            if (adminObjectService.IsAdminVerified(userAgent))
            {
                return RewardServiceInventory(
                    [],
                    directorypath,
                    filepath,
                    absolutepath,
                    false,
                    true
                );
            }
            else
            {
                LoggerAccessor.LogWarn(
                    $"[SSFW] - HandleRewardServiceInvCardTrackingDataDELETE : {SSFWUserSessionManager.GetIdBySessionId(sessionId)} Unauthorized to delete Card Tracking data!"
                );
                return null;
            }
        }

        public byte[]? HandleRewardServiceWipeInvDELETE(
            string directorypath,
            string filepath,
            string absolutepath,
            string userAgent,
            string sessionId
        )
        {
            AdminObjectService adminObjectService = new(sessionId, key);
            if (adminObjectService.IsAdminVerified(userAgent))
            {
                return RewardServiceInventory(
                    [],
                    directorypath,
                    filepath,
                    absolutepath,
                    true,
                    false
                );
            }
            else
            {
                LoggerAccessor.LogWarn(
                    $"[SSFW] - HandleRewardServiceWipeInvDELETE : {SSFWUserSessionManager.GetIdBySessionId(sessionId)} Unauthorized to wipe inventory data!"
                );
                return null;
            }
        }

        public void HandleRewardServiceTrunksPOST(
            byte[] buffer,
            string directorypath,
            string filepath,
            string absolutepath,
            string env,
            string? userId
        )
        {
            Directory.CreateDirectory(directorypath);

            File.WriteAllBytes(
                $"{SSFWServerConfiguration.SSFWStaticFolder}/{absolutepath}.json",
                buffer
            );

            TrunkServiceProcess(
                filepath.Replace("/setpartial", string.Empty) + ".json",
                Encoding.UTF8.GetString(buffer),
                env,
                userId
            );
        }

        public static void HandleRewardServiceTrunksEmergencyPOST(
            byte[] buffer,
            string directorypath,
            string absolutepath
        )
        {
            Directory.CreateDirectory(directorypath);

            File.WriteAllBytes(
                $"{SSFWServerConfiguration.SSFWStaticFolder}/{absolutepath}.json",
                buffer
            );
        }

        public void SSFWUpdateMini(string filePath, string postData, bool delete)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var json = FileHelper.ReadAllText(filePath, key);

                    // Parse the JSON string as a JArray
                    var jsonArray = string.IsNullOrEmpty(json) ? [] : JArray.Parse(json);

                    // Extract the rewards object from the POST data
                    var postDataObject = JObject.Parse(postData);
                    var rewardsObject = (JObject?)postDataObject["rewards"];

                    if (rewardsObject != null)
                    {
                        // Iterate over each reward in the POST data
                        foreach (var reward in rewardsObject)
                        {
                            var rewardKey = reward.Key;
                            var rewardValue = reward.Value;
                            if (string.IsNullOrEmpty(rewardKey) || rewardValue == null)
                                continue;

                            // Check if the reward exists in the JSON array
                            var existingReward = jsonArray.FirstOrDefault(r =>
                                r[rewardKey] != null
                            );
                            if (delete)
                            {
                                // If delete is true, remove the existing reward
                                if (existingReward != null)
                                    jsonArray.Remove(existingReward);
                            }
                            else
                            {
                                if (existingReward != null)
                                    // Update the value of the reward
                                    existingReward[rewardKey] = DateTime.UtcNow.ToUnixTime();
                                else
                                {
                                    // Add the new reward to the JSON array
                                    jsonArray.Add(
                                        new JObject { { rewardKey, DateTime.UtcNow.ToUnixTime() } }
                                    );
                                }
                            }
                        }

                        File.WriteAllText(filePath, jsonArray.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(
                    $"[SSFW] - SSFWUpdateMini errored out with this exception - {ex}"
                );
            }
        }

        public void TrunkServiceProcess(string filePath, string request, string env, string? userId)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var json = FileHelper.ReadAllText(filePath, key);

                    if (!string.IsNullOrEmpty(json))
                    {
                        // Parse the request
                        var requestObject = JsonConvert.DeserializeObject<JObject>(request);

                        if (requestObject != null)
                        {
                            var mainFile = JObject.Parse(json);

                            // Check if 'add' operation is requested
                            if (
                                requestObject.ContainsKey("add")
                                && requestObject["add"]?["objects"] is JArray addArray
                            )
                            {
                                var mainArray = (JArray?)mainFile["objects"];
                                if (mainArray != null)
                                {
                                    Dictionary<string, string> entriesToAddInMini = [];

                                    foreach (var addObject in addArray.Cast<JObject>())
                                    {
                                        mainArray.Add(addObject);
                                        if (
                                            addObject.TryGetValue("objectId", out var objectIdToken)
                                            && objectIdToken != null
                                            && addObject.TryGetValue("type", out var typeToken)
                                            && typeToken != null
                                            && int.TryParse(
                                                typeToken.ToString(),
                                                out var typeTokenInt
                                            )
                                            && typeTokenInt != 0
                                        )
                                            entriesToAddInMini.TryAdd(
                                                objectIdToken.ToString(),
                                                typeToken.ToString()
                                            );
                                    }

                                    // Update the mini file accordingly.
                                    if (!string.IsNullOrEmpty(env) && !string.IsNullOrEmpty(userId))
                                    {
                                        var miniPath =
                                            $"{SSFWServerConfiguration.SSFWStaticFolder}/RewardsService/{env}/rewards/{userId}/mini.json";

                                        if (!File.Exists(miniPath))
                                            File.WriteAllText(miniPath, "[]");

                                        foreach (var entry in entriesToAddInMini)
                                        {
                                            SSFWUpdateMini(
                                                miniPath,
                                                $"{{ \"rewards\": {{ \"{entry.Key}\": {entry.Value} }} }}",
                                                false
                                            );
                                        }
                                    }
                                }
                            }

                            // Check if 'update' operation is requested
                            if (
                                requestObject.ContainsKey("update")
                                && requestObject["update"]?["objects"] is JArray updateArray
                            )
                            {
                                var mainArray = (JArray?)mainFile["objects"];
                                if (mainArray != null)
                                {
                                    foreach (var updateObj in updateArray.Cast<JObject>())
                                    {
                                        if (
                                            updateObj.TryGetValue("objectId", out var objectIdToken)
                                            && objectIdToken is JValue objectIdValue
                                        )
                                        {
                                            var objectId = objectIdValue.ToString();
                                            var existingObj =
                                                mainArray.FirstOrDefault(obj =>
                                                    obj["objectId"]?.ToString() == objectId
                                                ) as JObject;
                                            existingObj?.Merge(
                                                updateObj,
                                                new JsonMergeSettings
                                                {
                                                    MergeArrayHandling = MergeArrayHandling.Replace,
                                                }
                                            );
                                        }
                                    }
                                }
                            }

                            // Check if 'delete' operation is requested
                            if (
                                requestObject.ContainsKey("delete")
                                && requestObject["delete"]?["objects"] is JArray deleteArray
                            )
                            {
                                var mainArray = (JArray?)mainFile["objects"];
                                if (mainArray != null)
                                {
                                    List<string> entriesToRemoveInMini = [];

                                    foreach (var deleteObj in deleteArray.Cast<JObject>())
                                    {
                                        if (
                                            deleteObj.TryGetValue("objectId", out var objectIdToken)
                                            && objectIdToken is JValue objectIdValue
                                        )
                                        {
                                            var objectId = objectIdValue.ToString();
                                            var existingObj =
                                                mainArray.FirstOrDefault(obj =>
                                                    obj["objectId"]?.ToString() == objectId
                                                ) as JObject;
                                            existingObj?.Remove();
                                            if (
                                                deleteObj.TryGetValue("type", out var typeToken)
                                                && typeToken != null
                                                && int.TryParse(
                                                    typeToken.ToString(),
                                                    out var typeTokenInt
                                                )
                                                && typeTokenInt != 0
                                            )
                                                entriesToRemoveInMini.Add(objectId);
                                        }
                                    }

                                    // Update the mini file accordingly.
                                    if (!string.IsNullOrEmpty(env) && !string.IsNullOrEmpty(userId))
                                    {
                                        var miniPath =
                                            $"{SSFWServerConfiguration.SSFWStaticFolder}/RewardsService/{env}/rewards/{userId}/mini.json";

                                        if (File.Exists(miniPath))
                                        {
                                            foreach (var entry in entriesToRemoveInMini)
                                            {
                                                SSFWUpdateMini(
                                                    miniPath,
                                                    $"{{ \"rewards\": {{ \"{entry}\": -1 }} }}",
                                                    true
                                                );
                                            }
                                        }
                                    }
                                }
                            }

                            File.WriteAllText(filePath, mainFile.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(
                    $"[SSFW] - TrunkServiceProcess errored out with this exception - {ex}"
                );
            }
        }

        public static byte[] RewardServiceInventory(
            byte[] buffer,
            string directorypath,
            string filepath,
            string absolutePath,
            bool deleteInv,
            bool deleteOnlyTracking
        )
        {
            //Tracking Inventory GUID
            const string trackingGuid = "00000000-00000000-00000000-00000001"; // fallback/hardcoded tracking GUID

            //Only return trackingGuid on error
            var errorPayload = Encoding.UTF8.GetBytes($"{{\"idList\": [\"{trackingGuid}\"] }}");

            // File paths based on the provided format
            var countsStoreDir = $"{SSFWServerConfiguration.SSFWStaticFolder}/{absolutePath}";
            var countsStore = $"{countsStoreDir}/counts.json";

            var trackingFileDir =
                $"{SSFWServerConfiguration.SSFWStaticFolder}/{absolutePath}/object";
            var trackingFile = $"{trackingFileDir}/{trackingGuid}.json";

            if (!string.IsNullOrEmpty(countsStoreDir) && !string.IsNullOrEmpty(trackingFileDir))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(countsStoreDir));
                Directory.CreateDirectory(Path.GetDirectoryName(trackingFileDir));
            }
            else
            {
                LoggerAccessor.LogError(
                    "[SSFW] - RewardServiceInventoryPOST: Fatal error in RewardService Inventory System! CountsStoreDir or TrackingFileDir should NOT be null!"
                );
                return errorPayload;
            }

            //Parse Buffer
            var fixedJsonPayload = GUIDValidator.FixJsonValues(Encoding.UTF8.GetString(buffer));
            try
            {
                using var document = JsonDocument.Parse(fixedJsonPayload);
                var root = document.RootElement;

                if (
                    !root.TryGetProperty("rewards", out var rewardsElement)
                    || rewardsElement.ValueKind != JsonValueKind.Array
                )
                {
                    LoggerAccessor.LogError(
                        "[SSFW] - RewardServiceInventoryPOST: Invalid payload - 'rewards' must be an array."
                    );
                    return errorPayload;
                }

                var rewards = rewardsElement.EnumerateArray();
                if (!rewards.MoveNext())
                {
                    LoggerAccessor.LogError(
                        "[SSFW] - RewardServiceInventoryPOST: Invalid payload - 'rewards' array is empty."
                    );
                    return errorPayload;
                }

                Dictionary<string, int> counts = [];
                if (File.Exists(countsStore))
                {
                    if (deleteInv)
                    {
                        File.Delete(countsStore);
#if DEBUG
                        LoggerAccessor.LogInfo(
                            $"[SSFW] - RewardServiceInventory: Successfully deleted Inventory counts at {countsStore}"
                        );
#endif
                    }
                    else
                    {
                        var countsJson = File.ReadAllText(countsStore);
                        counts =
                            System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int>>(
                                countsJson
                            ) ?? [];
                    }
                }
                else
                {
                    counts = [];
                }

                Dictionary<string, Dictionary<string, object>>? existingTrackingData = null;
                if (File.Exists(trackingFile))
                {
                    if (deleteInv || deleteOnlyTracking)
                    {
                        File.Delete(trackingFile);
#if DEBUG
                        LoggerAccessor.LogInfo(
                            $"[SSFW] - RewardServiceInventory: Deleting Tracking file at {trackingFile}"
                        );
#endif
                        return Encoding.UTF8.GetBytes("");
                    }
                    else
                    {
                        var existingTrackingJson = File.ReadAllText(trackingFile);
                        using var trackingDoc = JsonDocument.Parse(existingTrackingJson);
                        var trackingRoot = trackingDoc.RootElement;
                        if (
                            trackingRoot.TryGetProperty("rewards", out var trackingRewardsElement)
                            && trackingRewardsElement.ValueKind == JsonValueKind.Object
                        )
                        {
                            existingTrackingData = System.Text.Json.JsonSerializer.Deserialize<
                                Dictionary<string, Dictionary<string, object>>
                            >(trackingRewardsElement.GetRawText());
                        }

                        foreach (var reward in rewards)
                        {
                            if (
                                !reward.TryGetProperty("objectId", out var objectIdElement)
                                || objectIdElement.ValueKind != JsonValueKind.String
                            )
                            {
                                LoggerAccessor.LogError(
                                    "[SSFW] - RewardServiceInventoryPOST: Invalid reward - 'objectId' missing or not a string."
                                );
                                continue;
                            }

                            if (objectIdElement.ValueKind != JsonValueKind.String)
                            {
                                LoggerAccessor.LogError(
                                    $"[SSFW] - RewardServiceInventoryPOST: 'objectId' must be a string, got {objectIdElement.ValueKind}."
                                );
                                continue;
                            }

                            var objectId = objectIdElement.GetString();
                            if (!string.IsNullOrEmpty(objectId))
                            {
                                // Update counts
                                counts[objectId] = counts.TryGetValue(objectId, out var value)
                                    ? ++value
                                    : 1;
                            }

                            // Check if this is a tracking object (has metadata or matches tracking GUID)
                            var hasMetadata =
                                reward.TryGetProperty("_id", out _)
                                || reward.TryGetProperty("scene", out _)
                                || reward.TryGetProperty("boost", out _)
                                || reward.TryGetProperty("game", out _)
                                || reward.TryGetProperty("migrated", out _);
                            if (hasMetadata || objectId == trackingGuid || objectId != string.Empty)
                            {
                                var trackingRewards = existingTrackingData ?? [];
                                var metadata = new Dictionary<string, object>();

                                foreach (var prop in reward.EnumerateObject())
                                {
                                    if (prop.Name != "objectId")
                                    {
                                        metadata[prop.Name] = prop.Value.ValueKind switch
                                        {
                                            JsonValueKind.String => prop.Value.GetString() ?? "",
                                            JsonValueKind.Number => prop.Value.GetInt32(),
                                            JsonValueKind.True or JsonValueKind.False =>
                                                prop.Value.GetBoolean(),
                                            _ => prop.Value.ToString(),
                                        };
                                    }
                                }

                                if (!string.IsNullOrEmpty(objectId))
                                {
                                    trackingRewards[objectId] = metadata;
                                }

                                // Write tracking data
                                var trackingData = new Dictionary<string, object>
                                {
                                    { "result", 0 },
                                    { "rewards", trackingRewards },
                                };
                                var trackingJson = System.Text.Json.JsonSerializer.Serialize(
                                    trackingData,
                                    new JsonSerializerOptions { WriteIndented = true }
                                );
                                File.WriteAllText(trackingFile, trackingJson);
#if DEBUG
                                LoggerAccessor.LogInfo(
                                    $"[SSFW] - RewardServiceInventoryPOST: Updated tracking file: {trackingFile}"
                                );
#endif
                            }
                        }

                        var updatedCountsJson = System.Text.Json.JsonSerializer.Serialize(
                            counts,
                            new JsonSerializerOptions { WriteIndented = true }
                        );
                        File.WriteAllText(countsStore, updatedCountsJson);
#if DEBUG
                        LoggerAccessor.LogInfo(
                            $"[SSFW] - RewardServiceInventoryPOST: Updated counts file: {countsStore}"
                        );
#endif
                    }
                }
            }
            catch (System.Text.Json.JsonException ex)
            {
                LoggerAccessor.LogError(
                    $"[SSFW] - RewardServiceInventoryPOST: Error parsing JSON payload: {ex.Message}"
                );
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(
                    $"[SSFW] - RewardServiceInventoryPOST: Error processing POST request: {ex.Message}"
                );
            }

            return Encoding.UTF8.GetBytes(
                @"{ ""idList"": [""00000000-00000000-00000000-00000001""]}"
            );
        }

        public void AddMiniEntry(
            string uuid,
            byte invtype,
            string trunkFilePath,
            string env,
            string? userId
        )
        {
            ProcessTrunkObjectUpdate(
                trunkFilePath,
                new Dictionary<string, byte> { { uuid, invtype } },
                env,
                userId,
                true
            );
        }

        public void RemoveMiniEntry(
            string uuid,
            byte invtype,
            string trunkFilePath,
            string env,
            string? userId
        )
        {
            ProcessTrunkObjectUpdate(
                trunkFilePath,
                new Dictionary<string, byte> { { uuid, invtype } },
                env,
                userId,
                false
            );
        }

        public void AddMiniEntries(
            Dictionary<string, byte> entriesToAdd,
            string trunkFilePath,
            string env,
            string? userId
        )
        {
            ProcessTrunkObjectUpdate(trunkFilePath, entriesToAdd, env, userId, true);
        }

        public void RemoveMiniEntries(
            Dictionary<string, byte> entriesToRemove,
            string trunkFilePath,
            string env,
            string? userId
        )
        {
            ProcessTrunkObjectUpdate(trunkFilePath, entriesToRemove, env, userId, false);
        }

        private void ProcessTrunkObjectUpdate(
            string trunkFilePath,
            Dictionary<string, byte> entries,
            string env,
            string? userId,
            bool add
        )
        {
            var trunkJsonData = FileHelper.ReadAllText(trunkFilePath, key);

            if (!string.IsNullOrEmpty(trunkJsonData))
            {
                string setpartialRequest;

                try
                {
                    var setPartialDirectory = trunkFilePath[..^5];
                    using var doc = JsonDocument.Parse(trunkJsonData);

                    List<int> indexList = [];
                    Dictionary<int, (string, byte)> indexToItem = [];

                    foreach (var obj in doc.RootElement.GetProperty("objects").EnumerateArray())
                    {
                        if (
                            obj.TryGetProperty("index", out var indexProp)
                            && obj.TryGetProperty("objectId", out var idProp)
                            && obj.TryGetProperty("type", out var idType)
                            && int.TryParse(indexProp.GetString(), out var index)
                        )
                        {
                            indexList.Add(index);
                            var idPropStr = idProp.GetString();
                            var idTypeStr = idType.GetString();
                            if (
                                !string.IsNullOrEmpty(idTypeStr)
                                && !string.IsNullOrEmpty(idPropStr)
                                && byte.TryParse(idTypeStr, out var typeOfEntry)
                            )
                                indexToItem[index] = (idPropStr, typeOfEntry);
                        }
                    }

                    var lastIndex = indexList.Count > 0 ? indexList.Max() + 1 : 0;

                    if (add)
                    {
                        // Make sure we don't add a given uuid twice (causes inventory errors at boot)
                        foreach (var key in entries.Keys.Where(key => trunkJsonData.Contains(key)))
                        {
                            entries.Remove(key);
                        }
                        setpartialRequest = BuildAddSetPartialJson(entries, lastIndex);
                    }
                    else
                        setpartialRequest = BuildDeleteSetPartialJson(entries, indexToItem);

                    Directory.CreateDirectory(setPartialDirectory);

                    File.WriteAllText(setPartialDirectory + "/setpartial.json", setpartialRequest);

                    TrunkServiceProcess(trunkFilePath, setpartialRequest, env, userId);
                }
                catch (Exception ex)
                {
                    LoggerAccessor.LogError(
                        $"[SSFW] - ProcessTrunkObjectUpdate: setpartial update errored out with this exception - {ex}"
                    );
                }
            }
        }

        private static string BuildAddSetPartialJson(
            Dictionary<string, byte> entries,
            int startIndex
        )
        {
            // Create the object to build the JSON structure
            var jsonObject = new { add = new { objects = new List<object>() } };

            // Loop through the dictionary and add each item to the objects list
            foreach (var item in entries)
            {
                jsonObject.add.objects.Add(
                    new
                    {
                        objectId = item.Key,
                        type = item.Value.ToString(),
                        trunk = "0",
                        index = startIndex.ToString(),
                    }
                );

                startIndex++;
            }

            // Serialize the object to JSON string
            return JsonConvert.SerializeObject(jsonObject);
        }

        private static string BuildDeleteSetPartialJson(
            Dictionary<string, byte> entries,
            Dictionary<int, (string, byte)> indexToItem
        )
        {
            // Create the object to build the JSON structure
            var jsonObject = new { delete = new { objects = new List<object>() } };

            // Loop through the dictionary and add each item to the objects list
            foreach (var item in entries)
            {
                jsonObject.delete.objects.Add(
                    new
                    {
                        objectId = item.Key,
                        type = item.Value.ToString(),
                        trunk = "0",
                        index = indexToItem
                            .Where(x => x.Value.Item2 == item.Value && x.Value.Item1 == item.Key)
                            .FirstOrDefault()
                            .Key.ToString(),
                    }
                );
            }

            // Serialize the object to JSON string
            return JsonConvert.SerializeObject(jsonObject);
        }
    }
}
