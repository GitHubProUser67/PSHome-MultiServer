using System;
using System.IO;
using System.Text;
using CustomLogger;

namespace WebAPIService.GameServices.PSHOME.OSHABERI
{
    public class Announce
    {
        private const string HardcodedDefault =
            "{" +
                "\"ver_\":1," +
                "\"updateTime_\":300," +
                "\"infoLifeTime_\":10," +
                "\"announces_\":[]" +
            "}";

        public static string loadEntrygate(string workPath, string fulluripath)
        {
            return Serve(workPath, "Announce_entrygate.json", "entrygate");
        }

        public static string loadFarm(string workPath, string fulluripath)
        {
            return Serve(workPath, "Announce_farm.json", "farm");
        }

        public static string loadGarden(string workPath, string fulluripath)
        {
            return Serve(workPath, "Announce_garden.json", "garden");
        }

        private static string Serve(string workPath, string fileName, string sceneName)
        {
            string overridePath = Path.Combine(workPath, "oshaberi", "config", fileName);

            if (File.Exists(overridePath))
            {
                try
                {
                    string json = File.ReadAllText(overridePath, Encoding.UTF8);
                    LoggerAccessor.LogInfo($"[OSHABERI] - Announce ({sceneName}): serving override ({json.Length} bytes)");
                    return json;
                }
                catch (Exception ex)
                {
                    LoggerAccessor.LogWarn($"[OSHABERI] - Announce ({sceneName}): override read failed, using default: {ex}");
                }
            }

            LoggerAccessor.LogInfo($"[OSHABERI] - Announce ({sceneName}): serving empty feed");
            return HardcodedDefault;
        }
    }
}