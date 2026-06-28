using System.Collections.Concurrent;
using CustomLogger;
using HttpMultipartParser;
using MultiServerLibrary.HTTP;

namespace WebAPIService.GameServices.PSHOME.VEEMEE.accorn
{
    public static class Slot
    {
        public static string GetObjectSpace(byte[] PostData, string ContentType)
        {
            var boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (var ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    var scene_id = data.GetParameterValue("scene_id");

                    scene_id = data.GetParameterValue("scene_id");
                    var region = data.GetParameterValue("region");
                    var instance_id = data.GetParameterValue("instance_id");
                    var psn_id = data.GetParameterValue("psn_id");
                    var object_id = data.GetParameterValue("object_id");
                    var session_key = data.GetParameterValue("session_key");
                    var space_name = data.GetParameterValue("space_name");
                    var hex = data.GetParameterValue("hex");
                    var __salt = data.GetParameterValue("__salt");

                    ms.Flush();
                }

                return Processor.Sign($"{{\"space\":1}}");
            }

            return null;
        }

        public static string GetObjectSlot(byte[] PostData, string ContentType)
        {
            var max_slot = 0;
            var slot_name = string.Empty;
            var psn_id = string.Empty;
            var instance_id = string.Empty;
            var boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (var ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    slot_name = data.GetParameterValue("slot_name");

                    var session_key = data.GetParameterValue("session_key");
                    var scene_id = data.GetParameterValue("scene_id");
                    var region = data.GetParameterValue("region");

                    try
                    {
                        max_slot = int.Parse(data.GetParameterValue("max_slot"));
                    }
                    catch (Exception)
                    {
                        // Not Important
                    }

                    var object_id = data.GetParameterValue("object_id");

                    psn_id = data.GetParameterValue("psn_id");

                    instance_id = data.GetParameterValue("instance_id");

                    var hex = data.GetParameterValue("hex");
                    var __salt = data.GetParameterValue("__salt");

                    ms.Flush();
                }

                return Processor.Sign(
                    $"{{\"slot\":{SlotManager.UpdateSlot($"{instance_id}_{slot_name}", 0, psn_id, false, max_slot)}}}"
                );
            }

            return null;
        }

        public static string RemoveSlot(byte[] PostData, string ContentType)
        {
            var slot_num = 0;
            var slot_name = string.Empty;
            var psn_id = string.Empty;
            var instance_id = string.Empty;
            var boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (var ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    slot_name = data.GetParameterValue("slot_name");

                    var session_key = data.GetParameterValue("session_key");
                    var scene_id = data.GetParameterValue("scene_id");
                    var region = data.GetParameterValue("region");
                    var object_id = data.GetParameterValue("object_id");

                    psn_id = data.GetParameterValue("psn_id");

                    instance_id = data.GetParameterValue("instance_id");

                    var hex = data.GetParameterValue("hex");
                    var __salt = data.GetParameterValue("__salt");

                    ms.Flush();
                }

                return Processor.Sign(
                    $"{{\"success\":{SlotManager.UpdateSlot($"{instance_id}_{slot_name}", slot_num, psn_id, true)}}}"
                );
            }

            return null;
        }

        public static string HeartBeat(byte[] PostData, string ContentType)
        {
            var boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (var ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);
                    var slot_name = data.GetParameterValue("slot_name");

                    var session_key = data.GetParameterValue("session_key");
                    var scene_id = data.GetParameterValue("scene_id");
                    var region = data.GetParameterValue("region");
                    var object_id = data.GetParameterValue("object_id");
                    var psn_id = data.GetParameterValue("psn_id");
                    var instance_id = data.GetParameterValue("instance_id");
                    var hex = data.GetParameterValue("hex");
                    var __salt = data.GetParameterValue("__salt");

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

        private static readonly ConcurrentDictionary<
            string,
            ConcurrentDictionary<int, string>
        > _instanceData = new();

        public static string UpdateSlot(
            string instance_id,
            int slot_num,
            string psn_id,
            bool removemode,
            int max_slot = 0
        )
        {
            var found = false;

            try
            {
                if (!_instanceData.TryGetValue(instance_id, out var data))
                {
                    data = new ConcurrentDictionary<int, string>();
                    _instanceData[instance_id] = data;

                    // Initialize the dictionary with max_slot number of slots.
                    for (var i = 1; i <= max_slot; i++)
                        _instanceData[instance_id][i] = EMPTY_SLOT;
                }

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
                LoggerAccessor.LogWarn(
                    $"[VEEMEESlotManager] - Failed to update or remove slot - {ex}"
                );

                found = false;
            }

            return !found
                ? !removemode
                    ? "0"
                    : "false"
                : "true";
        }
    }
}
