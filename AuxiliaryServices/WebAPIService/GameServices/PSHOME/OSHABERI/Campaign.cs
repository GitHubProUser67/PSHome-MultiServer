using System;
using System.IO;
using System.Text;
using CustomLogger;

namespace WebAPIService.GameServices.PSHOME.OSHABERI
{
    public class Campaign
    {
        private const string HardcodedDefault =
            "{" +
                "\"ver_\":1," +
                "\"updateTime_\":300," +
                "\"conditionCheckTime_\":60," +
                "\"infoLifeTime_\":10," +
                "\"campaigns_\":[]," +
                "\"events_\":[]" +
            "}";

        public static string loadCampaign(string workPath, string fulluripath)
        {
            string overridePath = Path.Combine(workPath, "oshaberi", "config", "Campaign.json");

            if (File.Exists(overridePath))
            {
                try
                {
                    string json = File.ReadAllText(overridePath, Encoding.UTF8);
                    LoggerAccessor.LogInfo($"[OSHABERI] - Campaign: serving override ({json.Length} bytes)");
                    return json;
                }
                catch (Exception ex)
                {
                    LoggerAccessor.LogWarn($"[OSHABERI] - Campaign: override read failed, using defaults: {ex}");
                }
            }

            LoggerAccessor.LogInfo("[OSHABERI] - Campaign: serving hardcoded default (no active campaigns)");
            return HardcodedDefault;
        }
    }
}