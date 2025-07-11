using CustomLogger;
using Newtonsoft.Json;
using Horizon.BWPS.Config;
using Horizon.LIBRARY.Common;
using MultiServerLibrary.Extension;

namespace Horizon.BWPS
{
    public class BWPSClass
    {
        public static string? CONFIG_FILE = HorizonServerConfiguration.BWPSConfig;

        public const string gpszVersion2 = "3.02.200704101920";

        public static ServerSettings Settings = new();
        public static BWPS BWPS = new();

        public static bool started = false;

        public static async void StopServer()
        {
            started = false;

            await BWPS.Stop();
        }

        private static Task StartServerAsync()
        {
            try
            {
                DateTime lastConfigRefresh = DateTimeUtils.GetHighPrecisionUtcTime();

                LoggerAccessor.LogInfo("**************************************************");
                string datetime = DateTime.Now.ToString("MMMM/dd/yyyy hh:mm:ss tt");
                LoggerAccessor.LogInfo($"* Launched on {datetime}");

                Task.WaitAll(BWPS.Start());

                LoggerAccessor.LogInfo($"* Bandwidth Probe Server Version {gpszVersion2}");

                //* Process ID: %d , Parent Process ID: %d

                LoggerAccessor.LogInfo($"* Server Key Type: {Settings.EncryptMessages}");

                //* Diagnostic Profiling Enabled: %d Counts

                LoggerAccessor.LogInfo("**************************************************");

                LoggerAccessor.LogInfo($"UDP BWPS started on port {Settings.BWPSPort}.");

                started = true;
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[BWPS] - Server failed to initialize with error - {ex}");
            }

            return Task.CompletedTask;
        }

        public static void StartServer()
        {
            RefreshConfig();
            _ = StartServerAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        private static void RefreshConfig()
        {
            // Load settings
            if (File.Exists(CONFIG_FILE))
                // Populate existing object
                JsonConvert.PopulateObject(File.ReadAllText(CONFIG_FILE), Settings, new JsonSerializerSettings()
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                });
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(CONFIG_FILE) ?? Directory.GetCurrentDirectory() + "/static");

                // Save defaults
                File.WriteAllText(CONFIG_FILE ?? Directory.GetCurrentDirectory() + "/static/bwps.json", JsonConvert.SerializeObject(Settings, Formatting.Indented));
            }
        }
    }
}
