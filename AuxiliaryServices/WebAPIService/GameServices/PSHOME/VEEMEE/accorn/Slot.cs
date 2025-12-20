using System;
using MultiServerLibrary.HTTP;
using CustomLogger;
using HttpMultipartParser;
using System.IO;
using System.Collections.Concurrent;
using WebAPIService.GameServices.PSHOME.VEEMEE;

namespace WebAPIService.GameServices.PSHOME.VEEMEE.accorn
{
    public static class Slot
    {
        public static string GetObjectSpace(byte[] PostData, string ContentType)
        {
            string scene_id = string.Empty;
            string region = string.Empty;
            string instance_id = string.Empty;
            string psn_id = string.Empty;
            string object_id = string.Empty;
            string session_key = string.Empty;
            string space_name = string.Empty;
            string hex = string.Empty;
            string __salt = string.Empty;
            string boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    scene_id = data.GetParameterValue("scene_id");

                    scene_id = data.GetParameterValue("scene_id");

                    region = data.GetParameterValue("region");

                    instance_id = data.GetParameterValue("instance_id");

                    psn_id = data.GetParameterValue("psn_id");

                    object_id = data.GetParameterValue("object_id");

                    session_key = data.GetParameterValue("session_key");

                    space_name = data.GetParameterValue("space_name");

                    hex = data.GetParameterValue("hex");

                    __salt = data.GetParameterValue("__salt");

                    ms.Flush();
                }

                return Processor.Sign($"{{\"space\":1}}");
            }

            return null;
        }

        public static string GetObjectSlot(byte[] PostData, string ContentType)
        {
            int max_slot = 0;
            string slot_name = string.Empty;
            string session_key = string.Empty;
            string scene_id = string.Empty;
            string region = string.Empty;
            string object_id = string.Empty;
            string psn_id = string.Empty;
            string instance_id = string.Empty;
            string hex = string.Empty;
            string __salt = string.Empty;
            string boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    slot_name = data.GetParameterValue("slot_name");

                    session_key = data.GetParameterValue("session_key");

                    scene_id = data.GetParameterValue("scene_id");

                    region = data.GetParameterValue("region");

                    try
                    {
                        max_slot = int.Parse(data.GetParameterValue("max_slot"));
                    }
                    catch (Exception)
                    {
                        // Not Important
                    }

                    object_id = data.GetParameterValue("object_id");

                    psn_id = data.GetParameterValue("psn_id");

                    instance_id = data.GetParameterValue("instance_id");

                    hex = data.GetParameterValue("hex");

                    __salt = data.GetParameterValue("__salt");

                    ms.Flush();
                }

                return Processor.Sign($"{{\"slot\":{SlotManager.UpdateSlot($"{instance_id}_{slot_name}", 0, psn_id, false, max_slot)}}}");
            }

            return null;
        }

        public static string RemoveSlot(byte[] PostData, string ContentType)
        {
            int slot_num = 0;
            string slot_name = string.Empty;
            string session_key = string.Empty;
            string scene_id = string.Empty;
            string region = string.Empty;
            string object_id = string.Empty;
            string psn_id = string.Empty;
            string instance_id = string.Empty;
            string hex = string.Empty;
            string __salt = string.Empty;
            string boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {

                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    slot_name = data.GetParameterValue("slot_name");

                    session_key = data.GetParameterValue("session_key");

                    scene_id = data.GetParameterValue("scene_id");

                    region = data.GetParameterValue("region");

                    object_id = data.GetParameterValue("object_id");

                    psn_id = data.GetParameterValue("psn_id");

                    instance_id = data.GetParameterValue("instance_id");

                    hex = data.GetParameterValue("hex");

                    __salt = data.GetParameterValue("__salt");

                    ms.Flush();
                }

                return Processor.Sign($"{{\"success\":{SlotManager.UpdateSlot($"{instance_id}_{slot_name}", slot_num, psn_id, true)}}}");
            }

            return null;
        }

        public static string HeartBeat(byte[] PostData, string ContentType)
        {
            string session_key = string.Empty;
            string scene_id = string.Empty;
            string region = string.Empty;
            string slot_name = string.Empty;
            string object_id = string.Empty;
            string psn_id = string.Empty;
            string instance_id = string.Empty;
            string hex = string.Empty;
            string __salt = string.Empty;
            string boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    slot_name = data.GetParameterValue("slot_name");

                    session_key = data.GetParameterValue("session_key");

                    scene_id = data.GetParameterValue("scene_id");

                    region = data.GetParameterValue("region");

                    object_id = data.GetParameterValue("object_id");

                    psn_id = data.GetParameterValue("psn_id");

                    instance_id = data.GetParameterValue("instance_id");

                    hex = data.GetParameterValue("hex");

                    __salt = data.GetParameterValue("__salt");

                    ms.Flush();
                }

                return Processor.Sign("{ \"heartbeat\": true }");
            }

            return null;
        }
    }

    public static class SlotManager
    {
        private const string EMPTY_SLOT = "<EMPTY/>";

        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<int, string>> _instanceData = new ConcurrentDictionary<string, ConcurrentDictionary<int, string>>();

        public static string UpdateSlot(string instance_id, int slot_num, string psn_id, bool removemode, int max_slot = 0)
        {
            bool found = false;

            try
            {
                if (!_instanceData.ContainsKey(instance_id))
                {
                    _instanceData[instance_id] = new ConcurrentDictionary<int, string>();

                    // Initialize the dictionary with max_slot number of slots.
                    for (int i = 1; i <= max_slot; i++)
                        _instanceData[instance_id][i] = EMPTY_SLOT;
                }

                var data = _instanceData[instance_id];

                if (slot_num != 0 && removemode)
                {
                    if (data.ContainsKey(slot_num))
                    {
                        if (data[slot_num] == psn_id)
                        {
                            data[slot_num] = EMPTY_SLOT;
                            found = true;
                        }
                    }
                }

                if (slot_num == 0)
                {
                    foreach (var kvp in data)
                    {
                        if (!removemode)
                        {
                            if (kvp.Value == psn_id)
                                return kvp.Key.ToString();

                            if (kvp.Value == EMPTY_SLOT)
                            {
                                data[kvp.Key] = psn_id;
                                return kvp.Key.ToString();
                            }
                        }
                        else
                        {
                            if (kvp.Value == psn_id)
                            {
                                data[kvp.Key] = EMPTY_SLOT;
                                found = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogWarn($"[VEEMEESlotManager] - Failed to update or remove slot - {ex}");

                found = false;
            }

            if (!found)
            {
                if (!removemode)
                    return "0";
                else
                    return "false";
            }
            else
                return "true";
        }
    }
}
