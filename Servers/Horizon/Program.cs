using CustomLogger;
using Horizon.LIBRARY.Database;
using Horizon.PluginManager;
using Newtonsoft.Json.Linq;
using MultiServerLibrary.GeoLocalization;
using System.Runtime;
using System.Security.Cryptography;
using System.Collections.Concurrent;
using System.Reflection;
using Horizon.HTTPSERVICE;
using Horizon.MUM;
using MultiServerLibrary.Extension;
using MultiServerLibrary.SNMP;
using MultiServerLibrary;
using Microsoft.Extensions.Logging;
using Prometheus;
using WebAPIService.WebServices.WebCrypto;

public static class HorizonServerConfiguration
{
    public static string PluginsFolder { get; set; } = $"{Directory.GetCurrentDirectory()}/static/medius_plugins";
    public static string DatabaseConfig { get; set; } = $"{Directory.GetCurrentDirectory()}/static/db.config.json";
    public static string HTTPSCertificateFile { get; set; } = $"{Directory.GetCurrentDirectory()}/static/SSL/HorizonHTTPService.pfx";
    public static string HTTPSCertificatePassword { get; set; } = "qwerty";
    public static HashAlgorithmName HTTPSCertificateHashingAlgorithm { get; set; } = HashAlgorithmName.SHA384;
    public static bool EnableMedius { get; set; } = true;
    public static bool EnableDME { get; set; } = true;
    public static bool EnableMuis { get; set; } = true;
    public static bool EnableBWPS { get; set; } = true;
    public static bool EnableNAT { get; set; } = false;
    public static bool EnableMetrics { get; set; } = true;
    public static ushort MetricsPort { get; set; } = 1234;
    public static string MetricsUrl { get; set; } = "metrics/";
    public static string? PlayerAPIStaticPath { get; set; } = $"{Directory.GetCurrentDirectory()}/static/wwwroot";
    public static string? EBOOTDEFSConfig { get; set; } = $"{Directory.GetCurrentDirectory()}/static/ebootdefs.json";
    public static string? DMEConfig { get; set; } = $"{Directory.GetCurrentDirectory()}/static/dme.json";
    public static string? MEDIUSConfig { get; set; } = $"{Directory.GetCurrentDirectory()}/static/medius.json";
    public static string? MUISConfig { get; set; } = $"{Directory.GetCurrentDirectory()}/static/muis.json";
    public static string? BWPSConfig { get; set; } = $"{Directory.GetCurrentDirectory()}/static/bwps.json";
    public static string? NATConfig { get; set; } = $"{Directory.GetCurrentDirectory()}/static/nat.json";
    public static string MediusAPIKey { get; set; } = WebCryptoClass.GenerateRandomBase64KeyAsync().Result;
    public static string SSFWUrl { get; set; } = $"http://{(InternetProtocolUtils.TryGetServerIP(out _).Result ? InternetProtocolUtils.GetPublicIPAddress() : InternetProtocolUtils.GetLocalIPAddresses().First().ToString())}:8080";
    public static string[]? HTTPSDNSList { get; set; }

    public static DbController Database = new(DatabaseConfig);

    public static List<IPlugin> plugins = PluginLoader.LoadPluginsFromFolder(PluginsFolder);

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
                new JProperty("config_version", (ushort)3),
                new JProperty("medius", new JObject(
                    new JProperty("enabled", EnableMedius),
                    new JProperty("config", MEDIUSConfig)
                )),
                new JProperty("dme", new JObject(
                    new JProperty("enabled", EnableDME),
                    new JProperty("config", DMEConfig)
                )),
                new JProperty("muis", new JObject(
                    new JProperty("enabled", EnableMuis),
                    new JProperty("config", MUISConfig)
                )),
                new JProperty("nat", new JObject(
                    new JProperty("enabled", EnableNAT),
                    new JProperty("config", NATConfig)
                )),
                new JProperty("bwps", new JObject(
                    new JProperty("enabled", EnableBWPS),
                    new JProperty("config", BWPSConfig)
                )),
                new JProperty("eboot_defs_config", EBOOTDEFSConfig),
                new JProperty("https_dns_list", HTTPSDNSList ?? Array.Empty<string>()),
                new JProperty("certificate_file", HTTPSCertificateFile),
                new JProperty("certificate_password", HTTPSCertificatePassword),
                new JProperty("certificate_hashing_algorithm", HTTPSCertificateHashingAlgorithm.Name),
                new JProperty("player_api_static_path", PlayerAPIStaticPath),
                new JProperty("medius_api_key", MediusAPIKey),
                new JProperty("ssfw_url", SSFWUrl),
                new JProperty("plugins_folder", PluginsFolder),
                new JProperty("database", DatabaseConfig),
                new JProperty("prometheus", new JObject(
                    new JProperty("enabled", EnableMetrics),
                    new JProperty("url", MetricsUrl),
                    new JProperty("port", MetricsPort)
                ))
            ).ToString());

            return;
        }

        try
        {
            // Parse the JSON configuration
            dynamic config = JObject.Parse(File.ReadAllText(configPath));

            ushort config_version = GetValueOrDefault(config, "config_version", (ushort)0);
            if (config_version > 1)
            {
                if (config_version > 2)
                {
                    EnableMetrics = GetValueOrDefault(config.prometheus, "enabled", EnableMetrics);
                    MetricsUrl = GetValueOrDefault(config.prometheus, "url", MetricsUrl);
                    MetricsPort = GetValueOrDefault(config.prometheus, "port", MetricsPort);
                }

                EnableMedius = GetValueOrDefault(config.medius, "enabled", EnableMedius);
                EnableDME = GetValueOrDefault(config.dme, "enabled", EnableDME);
                EnableMuis = GetValueOrDefault(config.muis, "enabled", EnableMuis);
                EnableNAT = GetValueOrDefault(config.nat, "enabled", EnableNAT);
                EnableBWPS = GetValueOrDefault(config.bwps, "enabled", EnableBWPS);
                HTTPSCertificateFile = GetValueOrDefault(config, "certificate_file", HTTPSCertificateFile);
                HTTPSCertificatePassword = GetValueOrDefault(config, "certificate_password", HTTPSCertificatePassword);
                HTTPSCertificateHashingAlgorithm = new HashAlgorithmName(GetValueOrDefault(config, "certificate_hashing_algorithm", HTTPSCertificateHashingAlgorithm.Name));
                PlayerAPIStaticPath = GetValueOrDefault(config, "player_api_static_path", PlayerAPIStaticPath);
                HTTPSDNSList = GetValueOrDefault(config, "https_dns_list", HTTPSDNSList);
                DMEConfig = GetValueOrDefault(config.dme, "config", DMEConfig);
                MEDIUSConfig = GetValueOrDefault(config.medius, "config", MEDIUSConfig);
                MUISConfig = GetValueOrDefault(config.muis, "config", MUISConfig);
                NATConfig = GetValueOrDefault(config.nat, "config", NATConfig);
                BWPSConfig = GetValueOrDefault(config.bwps, "config", BWPSConfig);
                EBOOTDEFSConfig = GetValueOrDefault(config, "eboot_defs_config", EBOOTDEFSConfig);
                string APIKey = GetValueOrDefault(config, "medius_api_key", MediusAPIKey);
                if (APIKey.IsBase64().Item1)
                    MediusAPIKey = APIKey;
                string ssfwADR = GetValueOrDefault(config, "ssfw_url", SSFWUrl);
                if (!string.IsNullOrEmpty(ssfwADR) && ssfwADR.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
                    SSFWUrl = ssfwADR;
                PluginsFolder = GetValueOrDefault(config, "plugins_folder", PluginsFolder);
                DatabaseConfig = GetValueOrDefault(config, "database", DatabaseConfig);
            }
            else
                LoggerAccessor.LogWarn($"{configPath} file is outdated, using server's default.");
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
    private static string configPath = configDir + "horizon.json";
    private static string configMultiServerLibraryPath = configDir + "MultiServerLibrary.json";
    private static SnmpTrapSender? trapSender = null;
    private static ConcurrentBag<CrudServerHandler>? HTTPBag;
    private static MumServerHandler? MUMServer;
    private static MetricServer? metricsServer;

    private static Task HorizonStarter()
    {
        metricsServer?.Dispose();

        if (HorizonServerConfiguration.EnableMetrics)
        {
            metricsServer = new MetricServer(HorizonServerConfiguration.MetricsPort, HorizonServerConfiguration.MetricsUrl);
            metricsServer.Start();
        }

        if (HorizonServerConfiguration.EnableMedius)
        {
            try
            {
                MUMServer = new MumServerHandler("*", 10076);

                HTTPBag = new ConcurrentBag<CrudServerHandler>
                {
                    new("*", 61920),
                    new("*", 8443, HorizonServerConfiguration.HTTPSCertificateFile, HorizonServerConfiguration.HTTPSCertificatePassword)
                };
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError("[HTTPSERVICE] - An exception was thrown while starting the Medius HTTP Services: " + ex);
            }
        }

        if (HorizonServerConfiguration.EnableMedius)
            Horizon.SERVER.MediusClass.StartServer();

        if (HorizonServerConfiguration.EnableNAT)
            Horizon.NAT.NATClass.StartServer();

        if (HorizonServerConfiguration.EnableBWPS)
            Horizon.BWPS.BWPSClass.StartServer();

        if (HorizonServerConfiguration.EnableMuis)
            Horizon.MUIS.MuisClass.StartServer();

        if (HorizonServerConfiguration.EnableDME)
            Horizon.DME.DmeClass.StartServer();

        return Task.CompletedTask;
    }

    private static void StartOrUpdateServer()
    {
        if (Horizon.DME.DmeClass.started)
            Horizon.DME.DmeClass.StopServer();

        if (Horizon.MUIS.MuisClass.started)
            Horizon.MUIS.MuisClass.StopServer();

        if (Horizon.BWPS.BWPSClass.started)
            Horizon.BWPS.BWPSClass.StopServer();

        if (Horizon.NAT.NATClass.started)
            Horizon.NAT.NATClass.StopServer();

        if (Horizon.SERVER.MediusClass.started)
            Horizon.SERVER.MediusClass.StopServer();

        MUMServer?.StopServer();

        if (HTTPBag != null)
        {
            foreach (var httpBag in HTTPBag)
            {
                httpBag.StopServer();
            }
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        if (HorizonServerConfiguration.EnableMedius)
            MultiServerLibrary.SSL.CertificateHelper.InitializeSSLChainSignedCertificates(HorizonServerConfiguration.HTTPSCertificateFile, HorizonServerConfiguration.HTTPSCertificatePassword,
                    HorizonServerConfiguration.HTTPSDNSList, HorizonServerConfiguration.HTTPSCertificateHashingAlgorithm);

        HorizonStarter().Wait();
    }

    static void Main()
    {
        if (!MultiServerLibrary.Extension.Microsoft.Win32API.IsWindows)
            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
        else
            TechnitiumLibrary.Net.Firewall.FirewallHelper.CheckFirewallEntries(Assembly.GetEntryAssembly()?.Location);

        LoggerAccessor.SetupLogger("Horizon", Directory.GetCurrentDirectory());

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

        HorizonServerConfiguration.RefreshVariables(configPath);

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

                            HorizonServerConfiguration.RefreshVariables(configPath);

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