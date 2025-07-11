using CustomLogger;
using EdenServer.AMHLair;
using EdenServer.Database;
using EdenServer.EdNet;
using Microsoft.Extensions.Logging;
using MultiServerLibrary;
using MultiServerLibrary.Extension;
using MultiServerLibrary.GeoLocalization;
using MultiServerLibrary.SNMP;
using System.Net;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.RegularExpressions;

public static partial class EdenServerConfiguration
{
    public static string ProxyServerAddress { get; set; } = "0.0.0.0";
    public static ushort ProxyServerPort { get; set; } = 0;
    public static string ORBServerAddress { get; set; } = InternetProtocolUtils.TryGetServerIP(out string ip).Result ? ip : ip;
    public static ushort ORBServerPort { get; set; } = 9000;
    public static string STATSServerAddress { get; set; } = InternetProtocolUtils.TryGetServerIP(out string ip).Result ? ip : ip;
    public static ushort STATSServerPort { get; set; } = 9001;
    public static string CLANServerAddress { get; set; } = InternetProtocolUtils.TryGetServerIP(out string ip).Result ? ip : ip;
    public static ushort CLANServerPort { get; set; } = 9002;
    public static string WEATHERServerAddress { get; set; } = InternetProtocolUtils.TryGetServerIP(out string ip).Result ? ip : ip;
    public static ushort WEATHERServerPort { get; set; } = 9003;
    public static string PHOTOServerAddress { get; set; } = InternetProtocolUtils.TryGetServerIP(out string ip).Result ? ip : ip;
    public static ushort PHOTOServerPort { get; set; } = 9004;
    public static string GAMBLINGServerAddress { get; set; } = InternetProtocolUtils.TryGetServerIP(out string ip).Result ? ip : ip;
    public static ushort GAMBLINGServerPort { get; set; } = 9005;
    public static string MODERATIONServerAddress { get; set; } = InternetProtocolUtils.TryGetServerIP(out string ip).Result ? ip : ip;
    public static ushort MODERATIONServerPort { get; set; } = 9006;
    public static string CHECKServerAddress { get; set; } = InternetProtocolUtils.TryGetServerIP(out string ip).Result ? ip : ip;
    public static ushort CHECKServerPort { get; set; } = 9007;
    public static string CARDEALERServerAddress { get; set; } = InternetProtocolUtils.TryGetServerIP(out string ip).Result ? ip : ip;
    public static ushort CARDEALERServerPort { get; set; } = 9008;
    public static string RANKINGServerAddress { get; set; } = InternetProtocolUtils.TryGetServerIP(out string ip).Result ? ip : ip;
    public static ushort RANKINGServerPort { get; set; } = 9009;
    public static string SAVEGAMEServerAddress { get; set; } = InternetProtocolUtils.TryGetServerIP(out string ip).Result ? ip : ip;
    public static ushort SAVEGAMEServerPort { get; set; } = 9010;
    public static string LOGINServerAddress { get; set; } = InternetProtocolUtils.TryGetServerIP(out string ip).Result ? ip : ip;
    public static ushort LOGINServerPort { get; set; } = 9011;
    public static string AMHProxyServerAddress { get; set; } = InternetProtocolUtils.TryGetServerIP(out string ip).Result ? ip : ip;
    public static uint AMHProxyEncryptionKey { get; set; } = 0x12345678;
    public static ushort AMHMasterServerPort { get; set; } = 8124;
    public static bool EnableEncryption { get; set; } = true;
    public static bool BigEndianEncryption { get; set; } = true;
    public static string LoginDatabasePath { get; set; } = $"{Directory.GetCurrentDirectory()}/static/Eden/Database/tdu_accounts.sqlite";
    public static int ClientLongTimeoutSeconds { get; set; } = 60 * 5;

    public static (string? address, ushort? port)? GetServerConfigByServiceName(string serviceName)
    {
#if NET7_0_OR_GREATER
        Match match = ServerCrcRegex().Match(serviceName);
#else
        Match match = Regex.Match(serviceName, @"^[A-Z]{3}_[A-Z]_([^_]+)_SERVER$");
#endif
        if (!match.Success)
            return null;

        string serviceToken = match.Groups[1].Value;

        if (string.IsNullOrEmpty(serviceToken))
            return null;

        // Use reflection to get property values
        var configType = typeof(EdenServerConfiguration);
        var addressProperty = configType.GetProperty($"{serviceToken}ServerAddress");
        var portProperty = configType.GetProperty($"{serviceToken}ServerPort");

        if (addressProperty == null || portProperty == null)
            return null;

        return (addressProperty.GetValue(null) as string, (ushort?)portProperty.GetValue(null));
    }

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

            // Write the JsonObject to a file
            var configObject = new
            {
                config_version = (ushort)2,
                database = new
                {
                    login = LoginDatabasePath
                },
                amh = new
                {
                    proxy_server_address = AMHProxyServerAddress,
                    proxy_encryption_key = AMHProxyEncryptionKey,
                    master_server_port = AMHMasterServerPort,
                },
                encryption = new
                {
                    enable = EnableEncryption,
                    big_endian = BigEndianEncryption,
                },
                proxy_server_address = ProxyServerAddress,
                proxy_server_port = ProxyServerPort,
                orb_server_address = ORBServerAddress,
                orb_server_port = ORBServerPort,
                stats_server_address = STATSServerAddress,
                stats_server_port = STATSServerPort,
                clan_server_address = CLANServerAddress,
                clan_server_port = CLANServerPort,
                weather_server_address = WEATHERServerAddress,
                weather_server_port = WEATHERServerPort,
                photo_server_address = PHOTOServerAddress,
                photo_server_port = PHOTOServerPort,
                gambling_server_address = GAMBLINGServerAddress,
                gambling_server_port = GAMBLINGServerPort,
                moderation_server_address = MODERATIONServerAddress,
                moderation_server_port = MODERATIONServerPort,
                check_server_address = CHECKServerAddress,
                check_server_port = CHECKServerPort,
                cardealer_server_address = CARDEALERServerAddress,
                cardealer_server_port = CARDEALERServerPort,
                ranking_server_address = RANKINGServerAddress,
                ranking_server_port = RANKINGServerPort,
                savegame_server_address = SAVEGAMEServerAddress,
                savegame_server_port = SAVEGAMEServerPort,
                login_server_address = LOGINServerAddress,
                login_server_port = LOGINServerPort,
                client_long_timeout_seconds = ClientLongTimeoutSeconds,
            };

            File.WriteAllText(configPath, JsonSerializer.Serialize(configObject, new JsonSerializerOptions
            {
                WriteIndented = true
            }));

            return;
        }

        try
        {
            // Parse the JSON configuration
            using (var doc = JsonDocument.Parse(File.ReadAllText(configPath)))
            {
                JsonElement config = doc.RootElement;

                ushort config_version = GetValueOrDefault(config, "config_version", (ushort)0);

                if (config_version >= 2)
                {
                    if (config.TryGetProperty("amh", out JsonElement amhElement))
                    {
                        AMHProxyServerAddress = GetValueOrDefault(amhElement, "proxy_server_address", AMHProxyServerAddress);
                        AMHProxyEncryptionKey = GetValueOrDefault(amhElement, "proxy_encryption_key", AMHProxyEncryptionKey);
                        AMHMasterServerPort = GetValueOrDefault(amhElement, "master_server_port", AMHMasterServerPort);
                    }
                    if (config.TryGetProperty("database", out JsonElement databaseElement))
                    {
                        LoginDatabasePath = GetValueOrDefault(amhElement, "login", LoginDatabasePath);
                    }
                    if (config.TryGetProperty("encryption", out JsonElement encryptionElement))
                    {
                        EnableEncryption = GetValueOrDefault(encryptionElement, "enable", EnableEncryption);
                        BigEndianEncryption = GetValueOrDefault(encryptionElement, "big_endian", BigEndianEncryption);
                    }
                    ProxyServerAddress = GetValueOrDefault(config, "proxy_server_address", ProxyServerAddress);
                    ProxyServerPort = GetValueOrDefault(config, "proxy_server_port", ProxyServerPort);
                    ORBServerAddress = GetValueOrDefault(config, "orb_server_address", ORBServerAddress);
                    ORBServerPort = GetValueOrDefault(config, "orb_server_port", ORBServerPort);
                    STATSServerAddress = GetValueOrDefault(config, "stats_server_address", STATSServerAddress);
                    STATSServerPort = GetValueOrDefault(config, "stats_server_port", STATSServerPort);
                    CLANServerAddress = GetValueOrDefault(config, "clan_server_address", CLANServerAddress);
                    CLANServerPort = GetValueOrDefault(config, "clan_server_port", CLANServerPort);
                    WEATHERServerAddress = GetValueOrDefault(config, "weather_server_address", WEATHERServerAddress);
                    WEATHERServerPort = GetValueOrDefault(config, "weather_server_port", WEATHERServerPort);
                    PHOTOServerAddress = GetValueOrDefault(config, "photo_server_address", PHOTOServerAddress);
                    PHOTOServerPort = GetValueOrDefault(config, "photo_server_port", PHOTOServerPort);
                    GAMBLINGServerAddress = GetValueOrDefault(config, "gambling_server_address", GAMBLINGServerAddress);
                    GAMBLINGServerPort = GetValueOrDefault(config, "gambling_server_port", GAMBLINGServerPort);
                    MODERATIONServerAddress = GetValueOrDefault(config, "moderation_server_address", MODERATIONServerAddress);
                    MODERATIONServerPort = GetValueOrDefault(config, "moderation_server_port", MODERATIONServerPort);
                    CHECKServerAddress = GetValueOrDefault(config, "check_server_address", CHECKServerAddress);
                    CHECKServerPort = GetValueOrDefault(config, "check_server_port", CHECKServerPort);
                    CARDEALERServerAddress = GetValueOrDefault(config, "cardealer_server_address", CARDEALERServerAddress);
                    CARDEALERServerPort = GetValueOrDefault(config, "cardealer_server_port", CARDEALERServerPort);
                    RANKINGServerAddress = GetValueOrDefault(config, "ranking_server_address", RANKINGServerAddress);
                    RANKINGServerPort = GetValueOrDefault(config, "ranking_server_port", RANKINGServerPort);
                    SAVEGAMEServerAddress = GetValueOrDefault(config, "savegame_server_address", SAVEGAMEServerAddress);
                    SAVEGAMEServerPort = GetValueOrDefault(config, "savegame_server_port", SAVEGAMEServerPort);
                    LOGINServerAddress = GetValueOrDefault(config, "login_server_address", LOGINServerAddress);
                    LOGINServerPort = GetValueOrDefault(config, "login_server_port", LOGINServerPort);
                    ClientLongTimeoutSeconds = GetValueOrDefault(config, "client_long_timeout_seconds", ClientLongTimeoutSeconds);
                }
                else
                    LoggerAccessor.LogWarn($"{configPath} file is outdated, using server's default.");
            }
        }
        catch (Exception ex)
        {
            LoggerAccessor.LogWarn($"{configPath} file is malformed (exception: {ex}), using server's default.");
        }
    }

    // Helper method to get a value or default value if not present
    private static T GetValueOrDefault<T>(JsonElement config, string propertyName, T defaultValue)
    {
        try
        {
            if (config.TryGetProperty(propertyName, out JsonElement value))
            {
                T? extractedValue = JsonSerializer.Deserialize<T>(value.GetRawText());
                if (extractedValue == null)
                    return defaultValue;
                return extractedValue;
            }
        }
        catch (Exception ex)
        {
            LoggerAccessor.LogError($"[Program] - GetValueOrDefault thrown an exception: {ex}");
        }

        return defaultValue;
    }
#if NET7_0_OR_GREATER
    [GeneratedRegex("^[A-Z]{3}_[A-Z]_([^_]+)_SERVER$")]
    private static partial Regex ServerCrcRegex();
#endif
}

class Program
{
    public enum CtrlType
    {
        CTRL_C_EVENT = 0,
        CTRL_BREAK_EVENT = 1,
        CTRL_CLOSE_EVENT = 2,
        CTRL_LOGOFF_EVENT = 5,
        CTRL_SHUTDOWN_EVENT = 6
    }

    private delegate bool EventHandler(CtrlType sig);

    const string serverName = "EdenServer";

    private static string configDir = Directory.GetCurrentDirectory() + "/static/";
    private static string configPath = configDir + serverName + ".json";
    private static string configMultiServerLibraryPath = configDir + "MultiServerLibrary.json";
    private static TDUMasterServer? amhTDUMasterServer = null;
    private static ProxyServer? proxyServer = null;
    private static ORBServer? orbServer = null;
    private static SnmpTrapSender? trapSender = null;
    private static EventHandler? _closeHandler;

    [DllImport("Kernel32")]
    private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

    private static void StartOrUpdateServer()
    {
        proxyServer?.Stop();
        orbServer?.Stop();

        amhTDUMasterServer?.Dispose();

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        if (proxyServer == null)
            proxyServer = new(8889);
        else
            proxyServer.Start();

        if (orbServer == null)
            orbServer = new(EdenServerConfiguration.ORBServerPort);
        else
            orbServer.Start();

        amhTDUMasterServer = new TDUMasterServer(IPAddress.Any, EdenServerConfiguration.AMHMasterServerPort);
    }

    private static bool CloseHandler(CtrlType sig)
    {
        LoginDatabase._instance?.Dispose();

        switch (sig)
        {
            case CtrlType.CTRL_C_EVENT:
            case CtrlType.CTRL_LOGOFF_EVENT:
            case CtrlType.CTRL_SHUTDOWN_EVENT:
            case CtrlType.CTRL_CLOSE_EVENT:
            default:
                return false;
        }
    }

    static void Main()
    {
        if (!MultiServerLibrary.Extension.Microsoft.Win32API.IsWindows)
            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
        else
            TechnitiumLibrary.Net.Firewall.FirewallHelper.CheckFirewallEntries(Assembly.GetEntryAssembly()?.Location);

        LoggerAccessor.SetupLogger(serverName, Directory.GetCurrentDirectory());

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

        GeoIP.Initialize();

        MultiServerLibraryConfiguration.RefreshVariables(configMultiServerLibraryPath);

        // we need to safely dispose of the database when the application closes
        // this is a console app, so we need to hook into the console ctrl signal
        _closeHandler += CloseHandler;
        SetConsoleCtrlHandler(_closeHandler, true);

        LoginDatabase.Initialize(EdenServerConfiguration.LoginDatabasePath);

        if (MultiServerLibraryConfiguration.EnableSNMPReports)
        {
            trapSender = new SnmpTrapSender(MultiServerLibraryConfiguration.SNMPHashAlgorithm.Name, MultiServerLibraryConfiguration.SNMPTrapHost, MultiServerLibraryConfiguration.SNMPUserName,
                    MultiServerLibraryConfiguration.SNMPAuthPassword, MultiServerLibraryConfiguration.SNMPPrivatePassword,
                    MultiServerLibraryConfiguration.SNMPEnterpriseOid);

            if (trapSender.report != null)
            {
                LoggerAccessor.RegisterPostLogAction(LogLevel.Information, (msg, args) =>
                {
                    if (MultiServerLibraryConfiguration.EnableSNMPReports)
                        trapSender!.SendInfo(msg);
                });

                LoggerAccessor.RegisterPostLogAction(LogLevel.Warning, (msg, args) =>
                {
                    if (MultiServerLibraryConfiguration.EnableSNMPReports)
                        trapSender!.SendWarn(msg);
                });

                LoggerAccessor.RegisterPostLogAction(LogLevel.Error, (msg, args) =>
                {
                    if (MultiServerLibraryConfiguration.EnableSNMPReports)
                        trapSender!.SendCrit(msg);
                });

                LoggerAccessor.RegisterPostLogAction(LogLevel.Critical, (msg, args) =>
                {
                    if (MultiServerLibraryConfiguration.EnableSNMPReports)
                        trapSender!.SendCrit(msg);
                });
#if DEBUG
                LoggerAccessor.RegisterPostLogAction(LogLevel.Debug, (msg, args) =>
                {
                    if (MultiServerLibraryConfiguration.EnableSNMPReports)
                        trapSender!.SendInfo(msg);
                });
#endif
            }
        }

        EdenServerConfiguration.RefreshVariables(configPath);

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

                            CloseHandler(CtrlType.CTRL_C_EVENT);

                            Environment.Exit(0);
                        }
                        break;
                    case 'r':
                        LoggerAccessor.LogWarn("Are you sure you want to reboot the server? [y/N]");

                        if (char.ToLower(Console.ReadKey().KeyChar) == 'y')
                        {
                            LoggerAccessor.LogInfo("Rebooting!");

                            EdenServerConfiguration.RefreshVariables(configPath);

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