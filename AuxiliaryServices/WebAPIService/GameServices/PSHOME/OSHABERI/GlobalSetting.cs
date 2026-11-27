using System;
using System.IO;
using System.Text;
using CustomLogger;

namespace WebAPIService.GameServices.PSHOME.OSHABERI
{
    public class GlobalSetting
    {
        private const string HardcodedDefault =
            "{" +
                "\"releaseVersion_\":0," +
                "\"gcStep_\":2," +
                "\"asignFarmReqTime_\":1," +
                "\"asignFarmAnsTime_\":1," +
                "\"asignFarmKeepTime_\":10," +
                "\"uniqueFarmInstance_\":true" +
            "}";

        public static string loadOption(string workPath)
        {
            string overridePath = Path.Combine(workPath, "oshaberi", "config", "GlobalSetting.json");

            if (File.Exists(overridePath))
            {
                try
                {
                    string json = File.ReadAllText(overridePath, Encoding.UTF8);
                    LoggerAccessor.LogInfo($"[OSHABERI] - GlobalSetting: serving override ({json.Length} bytes)");
                    return json;
                }
                catch (Exception ex)
                {
                    LoggerAccessor.LogWarn($"[OSHABERI] - GlobalSetting: override read failed, using defaults: {ex}");
                }
            }

            LoggerAccessor.LogInfo("[OSHABERI] - GlobalSetting: serving hardcoded defaults");
            return HardcodedDefault;
        }
    }
}