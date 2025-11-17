using Blaze2SDK;
using Blaze3SDK;
using BlazeCommon;
using CustomLogger;
using Microsoft.Extensions.Logging;
using MultiServerLibrary;
using MultiServerLibrary.Extension;
using MultiServerLibrary.SNMP;
using MultiSocks;
using MultiSocks.Aries;
using MultiSocks.Aries.DataStore;
using MultiSocks.Blaze.Redirector;
using MultiSocks.ProtoSSL;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Reflection;
using System.Runtime;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

public static class MultiSocksServerConfiguration
{
    public static string ServerBindAddress { get; set; } = InternetProtocolUtils.TryGetServerIP(out string extractedIp).Result ? extractedIp : extractedIp;
    public static bool RPCS3Workarounds { get; set; } = true;
    public static bool EnableBlazeEncryption { get; set; } = false;
    public static string DirtySocksDatabasePath { get; set; } = $"{Directory.GetCurrentDirectory()}/static/dirtysocks.db.sqlite";
    public static List<ServerConfig> Servers { get; private set; } = new();

    /// <summary>
    /// Tries to load the specified configuration file.
    /// Throws an exception if it fails to find the file.
    /// </summary>
    /// <param name="configPath"></param>
    /// <exception cref="FileNotFoundException"></exception>
    public static void RefreshVariables(string configPath)
    {
        var defaultServers = new JArray(

        #region Aries Servers
        #region Redirectors
        #region SSX3 (NTSC)
                new JObject(
                    new JProperty("type", "Aries"),
                    new JProperty("subtype", "Redirector"),
                    new JProperty("port", 11000),
                    new JProperty("target_ip", ServerBindAddress),
                    new JProperty("target_port", 11051),
                    new JProperty("project", "SSX-ER-PS2-2004"),
                    new JProperty("sku", "PS2")
                ),
        #endregion
        #region SSX3 (PAL)
                new JObject(
                    new JProperty("type", "Aries"),
                    new JProperty("subtype", "Redirector"),
                    new JProperty("port", 11050),
                    new JProperty("target_ip", ServerBindAddress),
                    new JProperty("target_port", 11051),
                    new JProperty("project", "SSX-ER-PS2-2004"),
                    new JProperty("sku", "PS2")
                ),
        #endregion
        #region Burnout Paradise PS3
                new JObject(
                    new JProperty("type", "Aries"),
                    new JProperty("subtype", "Redirector"),
                    new JProperty("port", 21850),
                    new JProperty("target_ip", ServerBindAddress),
                    new JProperty("target_port", 21851),
                    new JProperty("project", "BURNOUT5"),
                    new JProperty("sku", "PS3")
                ),
        #endregion
        #region Burnout Paradise Ultimate Box PC
                new JObject(
                    new JProperty("type", "Aries"),
                    new JProperty("subtype", "Redirector"),
                    new JProperty("port", 21841),
                    new JProperty("target_ip", ServerBindAddress),
                    new JProperty("target_port", 21842),
                    new JProperty("project", "BURNOUT5"),
                    new JProperty("sku", "PC"),
                    new JProperty("secure", "true"),
                    new JProperty("cn", "pcburnout08.ea.com")
                ),
        #endregion
        #region Burnout Paradise Ultimate Box PS3
                new JObject(
                    new JProperty("type", "Aries"),
                    new JProperty("subtype", "Redirector"),
                    new JProperty("port", 21870),
                    new JProperty("target_ip", ServerBindAddress),
                    new JProperty("target_port", 21871),
                    new JProperty("project", "BURNOUT5"),
                    new JProperty("sku", "PS3")
                ),
        #endregion
        #region NASCAR 09 PS3
                new JObject(
                    new JProperty("type", "Aries"),
                    new JProperty("subtype", "Redirector"),
                    new JProperty("port", 30671),
                    new JProperty("target_ip", ServerBindAddress),
                    new JProperty("target_port", 30672),
                    new JProperty("project", "NASCAR09"),
                    new JProperty("sku", "PS3"),
                    new JProperty("secure", "true"),
                    new JProperty("cn", "ps3nascar09.ea.com")
                ),
                new JObject(
                    new JProperty("type", "Aries"),
                    new JProperty("subtype", "Redirector"),
                    new JProperty("port", 30670),
                    new JProperty("target_ip", ServerBindAddress),
                    new JProperty("target_port", 30672),
                    new JProperty("project", "NASCAR09"),
                    new JProperty("sku", "PS3")
                ),
        #endregion
        #region Hasbro Family Game Night PS3
                new JObject(
                    new JProperty("type", "Aries"),
                    new JProperty("subtype", "Redirector"),
                    new JProperty("port", 32950),
                    new JProperty("target_ip", ServerBindAddress),
                    new JProperty("target_port", 32951),
                    new JProperty("project", "DPR-09"),
                    new JProperty("sku", "PS3")
                ),
        #endregion
        #region Marvel Nemesis PS2
                new JObject(
                    new JProperty("type", "Aries"),
                    new JProperty("subtype", "Redirector"),
                    new JProperty("port", 31700),
                    new JProperty("target_ip", ServerBindAddress),
                    new JProperty("target_port", 31701),
                    new JProperty("project", "MARVEL06"),
                    new JProperty("sku", "PS2")
                ),
        #endregion
        #region 007: Everything or Nothing PS2
                new JObject(
                    new JProperty("type", "Aries"),
                    new JProperty("subtype", "Redirector"),
                    new JProperty("port", 11600),
                    new JProperty("target_ip", ServerBindAddress),
                    new JProperty("target_port", 11601),
                    new JProperty("project", "PS2-BOND-2004"),
                    new JProperty("sku", "PS2")
                ),
        #endregion
        #endregion
        #region Matchmakers
        #region SSX3 Matchmaker
                new JObject(
                    new JProperty("type", "Aries"),
                    new JProperty("subtype", "Matchmaker"),
                    new JProperty("port", 11051),
                    new JProperty("listen_ip", "0.0.0.0"),
                    new JProperty("project", "SSX-ER-PS2-2004"),
                    new JProperty("sku", "PS2")
                ),
        #endregion
        #region Burnout Paradise PS3 Matchmaker
                new JObject(
                    new JProperty("type", "Aries"),
                    new JProperty("subtype", "Matchmaker"),
                    new JProperty("port", 21851),
                    new JProperty("listen_ip", "0.0.0.0"),
                    new JProperty("project", "BURNOUT5"),
                    new JProperty("sku", "PS3")
                ),
        #endregion
        #region Burnout Paradise Ultimate Box PC Matchmaker
                new JObject(
                    new JProperty("type", "Aries"),
                    new JProperty("subtype", "Matchmaker"),
                    new JProperty("port", 21842),
                    new JProperty("listen_ip", "0.0.0.0"),
                    new JProperty("project", "BURNOUT5"),
                    new JProperty("sku", "PC")
                ),
        #endregion
        #region Burnout Paradise Ultimate Box PS3 Matchmaker
                new JObject(
                    new JProperty("type", "Aries"),
                    new JProperty("subtype", "Matchmaker"),
                    new JProperty("port", 21871),
                    new JProperty("listen_ip", "0.0.0.0"),
                    new JProperty("project", "BURNOUT5"),
                    new JProperty("sku", "PS3")
                ),
        #endregion
        #region NASCAR 09 PS3 Matchmaker
                new JObject(
                    new JProperty("type", "Aries"),
                    new JProperty("subtype", "Matchmaker"),
                    new JProperty("port", 30672),
                    new JProperty("listen_ip", "0.0.0.0"),
                    new JProperty("project", "NASCAR09"),
                    new JProperty("sku", "PS3")
                ),
        #endregion
        #region Hasbro Family Game Night PS3 Matchmaker
                new JObject(
                    new JProperty("type", "Aries"),
                    new JProperty("subtype", "Matchmaker"),
                    new JProperty("port", 32951),
                    new JProperty("listen_ip", "0.0.0.0"),
                    new JProperty("project", "DPR-09"),
                    new JProperty("sku", "PS3")
                ),
        #endregion
        #region Marvel Nemesis PS2 Matchmaker
                new JObject(
                    new JProperty("type", "Aries"),
                    new JProperty("subtype", "Matchmaker"),
                    new JProperty("port", 31701),
                    new JProperty("listen_ip", "0.0.0.0"),
                    new JProperty("project", "MARVEL06"),
                    new JProperty("sku", "PS2"),
                    new JProperty("rooms_to_add", new JArray(
                        new JObject(new JProperty("name", "Earth"), new JProperty("is_global", true))
                    ))
                ),
        #endregion
        #region 007: Everything or Nothing PS2 Matchmaker
                new JObject(
                    new JProperty("type", "Aries"),
                    new JProperty("subtype", "Matchmaker"),
                    new JProperty("port", 11601),
                    new JProperty("listen_ip", "0.0.0.0"),
                    new JProperty("project", "PS2-BOND-2004"),
                    new JProperty("sku", "PS2")
                ),
        #endregion
        #region Sims Bustin Out PS2 Matchmaker
                new JObject(
                    new JProperty("type", "Aries"),
                    new JProperty("subtype", "Matchmaker"),
                    new JProperty("port", 11101),
                    new JProperty("listen_ip", "0.0.0.0"),
                    new JProperty("project", "TSBO"),
                    new JProperty("sku", "PS2"),
                    new JProperty("rooms_to_add", new JArray(
                        new JObject(new JProperty("name", "Veronaville"), new JProperty("is_global", true)),
                        new JObject(new JProperty("name", "Strangetown"), new JProperty("is_global", true)),
                        new JObject(new JProperty("name", "Pleasantview"), new JProperty("is_global", true)),
                        new JObject(new JProperty("name", "Belladonna Cove"), new JProperty("is_global", true)),
                        new JObject(new JProperty("name", "Riverblossom Hills"), new JProperty("is_global", true))
                    ))
                ),
        #endregion
        #endregion
        #endregion
                // ────────────────────────────────
                // Blaze3 Servers
                // ────────────────────────────────
                new JObject(
                    new JProperty("type", "Blaze3"),
                    new JProperty("subtype", "Redirector"),
                    new JProperty("game", "Blaze3 Redirector"),
                    new JProperty("port", 42127),
                    new JProperty("secure", "true"),
                    new JProperty("cn", "gosredirector.ea.com")
                ),
                new JObject(
                    new JProperty("type", "Blaze3"),
                    new JProperty("subtype", "Main"),
                    new JProperty("game", "Mass Effect 3 (PS3)"),
                    new JProperty("port", 33152),
                    new JProperty("cn", "gosredirector.ea.com"),
                    new JProperty("components", new JArray(
                        "MassEffect3PS3Components.Auth.AuthComponent",
                        "MassEffect3PS3Components.Util.UtilComponent"
                    ))
                )
            );

        ushort config_ver = 4;

        // Make sure the file exists
        if (!File.Exists(configPath))
        {
            LoggerAccessor.LogWarn($"Could not find the configuration file:{configPath}, writing and using server's default.");

            Directory.CreateDirectory(Path.GetDirectoryName(configPath) ?? Directory.GetCurrentDirectory() + "/static");

            // Write the JObject to a file
            File.WriteAllText(configPath, new JObject(
                new JProperty("config_version", config_ver),
                new JProperty("server_bind_address", ServerBindAddress),
                new JProperty("rpcs3_workarounds", RPCS3Workarounds),
                new JProperty("enable_blaze_encryption", EnableBlazeEncryption),
                new JProperty("dirtysocks_database_path", DirtySocksDatabasePath),
                new JProperty("servers", defaultServers)
            ).ToString());

            Servers = defaultServers.ToObject<List<ServerConfig>>() ?? new();

            return;
        }

        try
        {
            // Parse the JSON configuration
            dynamic config = JObject.Parse(File.ReadAllText(configPath));

            ushort config_version = GetValueOrDefault(config, "config_version", (ushort)0);
            if (config_version >= config_ver)
            {
                ServerBindAddress = GetValueOrDefault(config, "server_bind_address", ServerBindAddress);
                RPCS3Workarounds = GetValueOrDefault(config, "rpcs3_workarounds", RPCS3Workarounds);
                EnableBlazeEncryption = GetValueOrDefault(config, "enable_blaze_encryption", EnableBlazeEncryption);
                DirtySocksDatabasePath = GetValueOrDefault(config, "dirtysocks_database_path", DirtySocksDatabasePath);
                if (config.servers != null)
                    Servers = ((JArray)config.servers).ToObject<List<ServerConfig>>() ?? new();

                return;
            }
            else
                LoggerAccessor.LogWarn($"{configPath} file is outdated, using server's default.");
        }
        catch (Exception ex)
        {
            LoggerAccessor.LogWarn($"{configPath} file is malformed (exception: {ex}), using server's default.");
        }

        Servers = defaultServers.ToObject<List<ServerConfig>>() ?? new();
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
    private static readonly string configPath = configDir + "MultiSocks.json";
    private static readonly string configMultiServerLibraryPath = configDir + "MultiServerLibrary.json";
    private static SnmpTrapSender? trapSender = null;

    public static IDatabase? DirtySocksDatabase = null;

    private static readonly Dictionary<string, object> AriesServers = new();
    private static readonly Dictionary<string, BlazeServer> BlazeServers = new();

    private static readonly VulnerableCertificateGenerator BlazeSSLCache = new();

    private static void StartOrUpdateServer()
    {
        foreach (var server in AriesServers.Values)
        {
            if (server is MatchmakerServer matchmaker)
                matchmaker.Dispose();
            else if (server is RedirectorServer redirector)
                redirector.Dispose();
            else
                ((EAMessengerServer)server).Dispose();
        }
        foreach (var server in BlazeServers.Values)
        {
            server.Stop();
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        InitializeServers();
    }

    public static void InitializeServers()
    {
        foreach (var server in MultiSocksServerConfiguration.Servers)
        {
            string srvType = server.Type.ToLower();

            try
            {
                switch (srvType)
                {
                    case "aries":
                        InitializeAries(server);
                        break;
                    default:
                        if (srvType.StartsWith("blaze"))
                            InitializeBlaze(server);
                        else
                            LoggerAccessor.LogWarn($"Unknown server type: {server.Type}");
                        break;
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"Failed to initialize {server.Type} {server.Subtype}: {ex}");
            }
        }
    }

    private static void InitializeAries(ServerConfig config)
    {
        if (MultiSocksServerConfiguration.DirtySocksDatabasePath.EndsWith(".json", StringComparison.InvariantCultureIgnoreCase))
            DirtySocksDatabase = new DirtySocksJSONDatabase();
        else
            DirtySocksDatabase = new DirtySocksSQLiteDatabase();

        switch (config.Subtype.ToLower())
        {
            case "redirector":

                var redirector = new RedirectorServer(
                    config.Port,
                    config.TargetIP ?? "127.0.0.1",
                    config.TargetPort ?? 0,
                    config.Project,
                    config.SKU,
                    config.Secure,
                    config.CN,
                    config.WeakChainSignedRSAKey
                );

                AriesServers[$"{config.Project}-{config.SKU}_Redirector"] = redirector;

                LoggerAccessor.LogInfo($"Started Aries Redirector on port {config.Port}");
                break;

            case "messenger":

                var messenger = new EAMessengerServer(
                    config.Port,
                    config.ListenIP ?? "0.0.0.0",
                    config.Project,
                    config.SKU,
                    config.Secure,
                    config.CN,
                    config.WeakChainSignedRSAKey
                );

                AriesServers[$"{config.Project}-{config.SKU}_Messenger"] = messenger;

                LoggerAccessor.LogInfo($"Started Aries Messenger on port {config.Port}");
                break;

            case "matchmaker":

                List<Tuple<string, bool>>? roomTuples = null;
                if (config.RoomsToAdd != null)
                {
                    roomTuples = config.RoomsToAdd
                        .Select(r => Tuple.Create(r.Name, r.IsGlobal))
                        .ToList();
                }

                var matchmaker = new MatchmakerServer(
                    config.Port,
                    config.ListenIP ?? "0.0.0.0",
                    roomTuples,
                    config.Project,
                    config.SKU,
                    config.Secure,
                    config.CN,
                    config.WeakChainSignedRSAKey
                );

                AriesServers[$"{config.Project}-{config.SKU}_Matchmaker"] = matchmaker;

                LoggerAccessor.LogInfo(
                    $"Started Aries Matchmaker on port {config.Port} with {roomTuples?.Count ?? 0} room(s)."
                );
                break;

            default:
                LoggerAccessor.LogWarn($"Unknown Aries subtype: {config.Subtype}");
                break;
        }
    }

    private static void InitializeBlaze(ServerConfig config)
    {
        const string defaultSslDomain = "gosredirector.ea.com";
        const string EA_OU = "Global Online Studio";
        const string componentsClassPrefix = "MultiSocks.Blaze.";

        string blazeVersion = config.Type.ToLower();
        string sslDomain = config.CN ?? defaultSslDomain;

        // Factory selector for Blaze2 (defaults to Blaze3)
        Func<string, IPEndPoint, X509Certificate2?, bool, dynamic> createBlazeServer = blazeVersion switch
        {
            "blaze2" => (name, endpoint, cert, secure) => Blaze2.CreateBlazeServer(name, endpoint, cert, secure),
            _ => (name, endpoint, cert, secure) => Blaze3.CreateBlazeServer(name, endpoint, cert, secure)
        };

        switch (config.Subtype.ToLower())
        {
            case "redirector":
                {
                    var redirector = createBlazeServer(
                        $"{config.Type} Redirector",
                        new IPEndPoint(IPAddress.Any, config.Port),
                        BlazeSSLCache.GetVulnerableCustomEaCert(sslDomain, "Global Online Studio", true).Item3,
                        config.Secure
                    );

                    redirector.AddComponent<RedirectorComponent>();

                    _ = redirector.Start(-1).ConfigureAwait(false);

                    BlazeServers[$"{blazeVersion}_Redirector"] = redirector;
                    LoggerAccessor.LogInfo($"[{blazeVersion.ToUpper()}] Redirector running on port {config.Port}");
                    break;
                }

            case "mitm":
                {
                    if (string.IsNullOrEmpty(config.Game))
                    {
                        LoggerAccessor.LogWarn($"[{blazeVersion.ToUpper()}] Missing 'game' field for Blaze mitm server on port {config.Port}");
                        return;
                    }

                    Func<string, string, string, ushort, uint, IPEndPoint, int, X509Certificate2?, bool, bool, dynamic> createBlazeMitmServer = blazeVersion switch
                    {
                        _ => (name, target_ip, target_hostname, target_port, encryption_key, endpoint, ssl_protocols, cert, write_to_file, secure) => Blaze3.CreateBlazeMitmServer(name, target_ip, target_hostname, target_port, encryption_key, endpoint, (SslProtocols)ssl_protocols, cert, write_to_file, secure)
                    };

                    var blazeServer = createBlazeMitmServer(
                        config.Game,
                        config.TargetIP ?? "127.0.0.1",
                        config.TargetHostname ?? "127.0.0.1",
                        config.TargetPort ?? 0,
                        config.StorageEncryptionKey,
                        new IPEndPoint(IPAddress.Any, config.Port),
                        config.SSLProtocols,
                        BlazeSSLCache.GetVulnerableCustomEaCert(sslDomain, EA_OU, true).Item3,
                        config.WriteClientReportToFile,
                        config.Secure
                    );

                    if (config.Components != null)
                    {
                        foreach (var compName in config.Components)
                        {
                            try
                            {
                                var type = Type.GetType(compName.StartsWith(componentsClassPrefix) ? compName : componentsClassPrefix + compName, throwOnError: false);
                                if (type != null)
                                {
                                    blazeServer.AddComponent(type);
                                    LoggerAccessor.LogInfo($"[{blazeVersion.ToUpper()}] Added component: {compName}");
                                }
                                else
                                {
                                    LoggerAccessor.LogWarn($"[{blazeVersion.ToUpper()}] Component not found: {compName}");
                                }
                            }
                            catch (Exception ex)
                            {
                                LoggerAccessor.LogError($"[{blazeVersion.ToUpper()}] Failed to add component '{compName}': {ex}");
                            }
                        }
                    }

                    _ = blazeServer.Start(-1).ConfigureAwait(false);

                    BlazeServers[$"{config.Type}_{config.Game}"] = blazeServer;

                    LoggerAccessor.LogInfo($"[{blazeVersion.ToUpper()}] Mitm server '{config.Game}' running on port {config.Port}");
                    break;
                }

            case "main":
                {
                    if (string.IsNullOrEmpty(config.Game))
                    {
                        LoggerAccessor.LogWarn($"[{blazeVersion.ToUpper()}] Missing 'game' field for Blaze main server on port {config.Port}");
                        return;
                    }

                    var blazeServer = createBlazeServer(
                        config.Game,
                        new IPEndPoint(IPAddress.Any, config.Port),
                        BlazeSSLCache.GetVulnerableCustomEaCert(sslDomain, EA_OU, true).Item3,
                        config.Secure
                    );

                    if (config.Components != null)
                    {
                        foreach (var compName in config.Components)
                        {
                            try
                            {
                                var type = Type.GetType(compName.StartsWith(componentsClassPrefix) ? compName : componentsClassPrefix + compName, throwOnError: false);
                                if (type != null)
                                {
                                    blazeServer.AddComponent(type);
                                    LoggerAccessor.LogInfo($"[{blazeVersion.ToUpper()}] Added component: {compName}");
                                }
                                else
                                {
                                    LoggerAccessor.LogWarn($"[{blazeVersion.ToUpper()}] Component not found: {compName}");
                                }
                            }
                            catch (Exception ex)
                            {
                                LoggerAccessor.LogError($"[{blazeVersion.ToUpper()}] Failed to add component '{compName}': {ex}");
                            }
                        }
                    }

                    _ = blazeServer.Start(-1).ConfigureAwait(false);

                    BlazeServers[$"{config.Type}_{config.Game}"] = blazeServer;

                    LoggerAccessor.LogInfo($"[{blazeVersion.ToUpper()}] Main server '{config.Game}' running on port {config.Port}");
                    break;
                }

            default:
                LoggerAccessor.LogWarn($"Unknown Blaze subtype: {config.Subtype}");
                break;
        }
    }

    static void Main()
    {
        if (!MultiServerLibrary.Extension.Microsoft.Win32API.IsWindows)
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

        MultiServerLibraryConfiguration.RefreshVariables(configMultiServerLibraryPath);

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