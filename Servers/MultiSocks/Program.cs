using CustomLogger;
using MultiSocks.Aries;
using MultiSocks.Blaze;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Runtime;
using NetworkLibrary.SNMP;
using NetworkLibrary;
using Microsoft.Extensions.Logging;
using NetworkLibrary.Extension;

public static class MultiSocksServerConfiguration
{
    public static string ServerBindAddress { get; set; } = InternetProtocolUtils.TryGetServerIP(out string extractedIp).Result ? extractedIp : extractedIp;
    public static bool RPCS3Workarounds { get; set; } = true;
    public static bool EnableBlazeEncryption { get; set; } = false;
    public static string DirtySocksDatabasePath { get; set; } = $"{Directory.GetCurrentDirectory()}/static/dirtysocks.db.json";

    /// <summary>
    /// Tries to load the specified configuration file.
    /// Throws an exception if it fails to find the file.
    /// </summary>
    /// <param name="configPath"></param>
    /// <exception cref="FileNotFoundException"></exception>
    public static void RefreshVariables(string configPath)
    {
        // Make sure the file exists
        if (!File.Exists(configPath))
        {
            LoggerAccessor.LogWarn($"Could not find the configuration file:{configPath}, writing and using server's default.");

            Directory.CreateDirectory(Path.GetDirectoryName(configPath) ?? Directory.GetCurrentDirectory() + "/static");

            // Write the JObject to a file
            File.WriteAllText(configPath, new JObject(
                new JProperty("server_bind_address", ServerBindAddress),
                new JProperty("rpcs3_workarounds", RPCS3Workarounds),
                new JProperty("enable_blaze_encryption", EnableBlazeEncryption),
                new JProperty("dirtysocks_database_path", DirtySocksDatabasePath)
            ).ToString());

            return;
        }

        try
        {
            // Parse the JSON configuration
            dynamic config = JObject.Parse(File.ReadAllText(configPath));

            ServerBindAddress = GetValueOrDefault(config, "server_bind_address", ServerBindAddress);
            RPCS3Workarounds = GetValueOrDefault(config, "rpcs3_workarounds", RPCS3Workarounds);
            EnableBlazeEncryption = GetValueOrDefault(config, "enable_blaze_encryption", EnableBlazeEncryption);
            DirtySocksDatabasePath = GetValueOrDefault(config, "dirtysocks_database_path", DirtySocksDatabasePath);
        }
        catch (Exception ex)
        {
            LoggerAccessor.LogWarn($"{configPath} file is malformed (exception: {ex}), using server's default.");
        }
    }

    // Helper method to get a value or default value if not present
    private static T GetValueOrDefault<T>(dynamic obj, string propertyName, T defaultValue)
    {
        try
        {
            if (obj != null)
            {
                if (obj is JObject jObject)
                {
                    if (jObject.TryGetValue(propertyName, out JToken? value))
                    {
                        T? returnvalue = value.ToObject<T>();
                        if (returnvalue != null)
                            return returnvalue;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            LoggerAccessor.LogError($"[Program] - GetValueOrDefault thrown an exception: {ex}");
        }

        return defaultValue;
    }
}

class Program
{
    public static string configDir = Directory.GetCurrentDirectory() + "/static/";
    private static string configPath = configDir + "MultiSocks.json";
    private static string configNetworkLibraryPath = configDir + "NetworkLibrary.json";
    private static SnmpTrapSender? trapSender = null;
    private static AriesServer? DirtySocksServer;
    private static BlazeClass? BlazeDirtySocksServer;

    private static void StartOrUpdateServer()
    {
        DirtySocksServer?.Dispose();
        BlazeDirtySocksServer?.Dispose();

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        DirtySocksServer = new AriesServer(new CancellationTokenSource().Token);
        BlazeDirtySocksServer = new BlazeClass(new CancellationTokenSource().Token);
    }

    static void Main()
    {
        if (!NetworkLibrary.Extension.Microsoft.Win32API.IsWindows)
            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
        else
            TechnitiumLibrary.Net.Firewall.FirewallHelper.CheckFirewallEntries(Assembly.GetEntryAssembly()?.Location);

        LoggerAccessor.SetupLogger("MultiSocks", Directory.GetCurrentDirectory());

#if DEBUG
        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            LoggerAccessor.LogError("[Program] - A FATAL ERROR OCCURED!");
            LoggerAccessor.LogError(args.ExceptionObject as Exception);
        };

        TaskScheduler.UnobservedTaskException += (sender, args) =>
        {
            LoggerAccessor.LogError("[Program] - A task has thrown a Unobserved Exception!");
            LoggerAccessor.LogError(args.Exception);
            args.SetObserved();
        };
#endif

        NetworkLibraryConfiguration.RefreshVariables(configNetworkLibraryPath);

        if (NetworkLibraryConfiguration.EnableSNMPReports)
        {
            trapSender = new SnmpTrapSender(NetworkLibraryConfiguration.SNMPHashAlgorithm.Name, NetworkLibraryConfiguration.SNMPTrapHost, NetworkLibraryConfiguration.SNMPUserName,
                    NetworkLibraryConfiguration.SNMPAuthPassword, NetworkLibraryConfiguration.SNMPPrivatePassword,
                    NetworkLibraryConfiguration.SNMPEnterpriseOid);

            if (trapSender.report != null)
            {
                LoggerAccessor.RegisterPostLogAction(LogLevel.Information, (msg, args) =>
                {
                    if (NetworkLibraryConfiguration.EnableSNMPReports)
                        trapSender!.SendInfo(msg);
                });

                LoggerAccessor.RegisterPostLogAction(LogLevel.Warning, (msg, args) =>
                {
                    if (NetworkLibraryConfiguration.EnableSNMPReports)
                        trapSender!.SendWarn(msg);
                });

                LoggerAccessor.RegisterPostLogAction(LogLevel.Error, (msg, args) =>
                {
                    if (NetworkLibraryConfiguration.EnableSNMPReports)
                        trapSender!.SendCrit(msg);
                });

                LoggerAccessor.RegisterPostLogAction(LogLevel.Critical, (msg, args) =>
                {
                    if (NetworkLibraryConfiguration.EnableSNMPReports)
                        trapSender!.SendCrit(msg);
                });
#if DEBUG
                LoggerAccessor.RegisterPostLogAction(LogLevel.Debug, (msg, args) =>
                {
                    if (NetworkLibraryConfiguration.EnableSNMPReports)
                        trapSender!.SendInfo(msg);
                });
#endif
            }
        }

        MultiSocksServerConfiguration.RefreshVariables(configPath);

        StartOrUpdateServer();

        if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != "true")
        {
            while (true)
            {
                LoggerAccessor.LogInfo("Press any keys to access server actions...");

                Console.ReadLine();

                LoggerAccessor.LogInfo("Press one of the following keys to trigger an action: [R (Reboot),S (Shutdown)]");

                switch (char.ToLower(Console.ReadKey().KeyChar))
                {
                    case 's':
                        LoggerAccessor.LogWarn("Are you sure you want to shut down the server? [y/N]");

                        if (char.ToLower(Console.ReadKey().KeyChar) == 'y')
                        {
                            LoggerAccessor.LogInfo("Shutting down. Goodbye!");
                            Environment.Exit(0);
                        }
                        break;
                    case 'r':
                        LoggerAccessor.LogWarn("Are you sure you want to reboot the server? [y/N]");

                        if (char.ToLower(Console.ReadKey().KeyChar) == 'y')
                        {
                            LoggerAccessor.LogInfo("Rebooting!");

                            MultiSocksServerConfiguration.RefreshVariables(configPath);

                            StartOrUpdateServer();
                        }
                        break;
                }
            }
        }
        else
        {
            LoggerAccessor.LogWarn("\nConsole Inputs are locked while server is running. . .");

            Thread.Sleep(Timeout.Infinite);
        }
    }
}