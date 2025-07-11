using MultiServerLibrary.Extension;
using System.Text.Json;

namespace MultiServerLibrary.HTTP
{
    public static class PlayerData
    {
        public static double? GetVBitRate(string jsonInfoData)
        {
            using (JsonDocument doc = JsonDocument.Parse(jsonInfoData))
            {
                var root = doc.RootElement;

                // Navigate to "media_info" -> "streams"
                if (root.TryGetProperty("media_info", out JsonElement mediaInfo) &&
                    mediaInfo.TryGetProperty("streams", out JsonElement streams) &&
                    streams.ValueKind == JsonValueKind.Array)
                {
                    foreach (JsonElement stream in streams.EnumerateArray())
                    {
                        // Check for "codec_type" == "video"
                        if (stream.TryGetProperty("codec_type", out JsonElement codecType) && codecType.GetString() == "video")
                        {
                            // Get "bit_rate" value and convert it
                            if (stream.TryGetProperty("bit_rate", out JsonElement bitRateElement))
                            {
                                string bitRateStr = bitRateElement.GetString();
                                if (!string.IsNullOrEmpty(bitRateStr) && double.TryParse(bitRateStr, out double bitRate))
                                    return bitRate / 1024; // Convert from bps to kbps
                            }
                        }
                    }
                }
            }

            return null;
        }

        public static double? GetVFrameRate(string jsonInfoData)
        {
            using (JsonDocument doc = JsonDocument.Parse(jsonInfoData))
            {
                var root = doc.RootElement;

                // Navigate to "media_info" -> "streams"
                if (root.TryGetProperty("media_info", out JsonElement mediaInfo) &&
                    mediaInfo.TryGetProperty("streams", out JsonElement streams) &&
                    streams.ValueKind == JsonValueKind.Array)
                {
                    foreach (JsonElement stream in streams.EnumerateArray())
                    {
                        // Check for "codec_type" == "video"
                        if (stream.TryGetProperty("codec_type", out JsonElement codecType) && codecType.GetString() == "video")
                        {
                            // Get "r_frame_rate" value
                            if (stream.TryGetProperty("r_frame_rate", out JsonElement frameRateElement))
                            {
                                string rFrameRateStr = frameRateElement.GetString();
                                if (!string.IsNullOrEmpty(rFrameRateStr))
                                    return rFrameRateStr.Eval();
                            }
                        }
                    }
                }
            }

            return null;
        }
    }
}
