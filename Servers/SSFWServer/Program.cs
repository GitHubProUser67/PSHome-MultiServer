using CustomLogger;
using Newtonsoft.Json.Linq;
using SSFWServer;
using System.Reflection;
using System.Runtime;
using System.Security.Cryptography;
using MultiServerLibrary.SNMP;
using MultiServerLibrary;
using Microsoft.Extensions.Logging;
using HomeTools.Crypto;
using MultiServerLibrary.Extension;
using System.Diagnostics;

public static class SSFWServerConfiguration
{
    public static bool SSFWCrossSave { get; set; } = true;
    public static bool EnableHTTPCompression { get; set; } = false;
    public static int SSFWTTL { get; set; } = 60;
    public static bool PreferNativeHttpListenerEngine { get; set; } = false;
    public static int BufferSize { get; set; } = 4096;
    public static bool EnableKeepAlive { get; set; } = false;
    public static string SSFWMinibase { get; set; } = "[]";
    public static string SSFWLegacyKey { get; set; } = "**NoNoNoYouCantHaxThis****69";
    public static string SSFWSessionIdKey { get; set; } = WebAPIService.WebCrypto.WebCryptoClass.GenerateRandomBase64KeyAsync().Result;
    public static string SSFWStaticFolder { get; set; } = $"{Directory.GetCurrentDirectory()}/static/wwwssfwroot";
    public static string HTTPSCertificateFile { get; set; } = $"{Directory.GetCurrentDirectory()}/static/SSL/SSFW.pfx";
    public static string HTTPSCertificatePassword { get; set; } = "qwerty";
    public static HashAlgorithmName HTTPSCertificateHashingAlgorithm { get; set; } = HashAlgorithmName.SHA384;
    public static string ScenelistFile { get; set; } = $"{Directory.GetCurrentDirectory()}/static/wwwssfwroot/SceneList.xml";
    public static string[]? HTTPSDNSList { get; set; } = {
            "cprod.homerewards.online.scee.com",
            "cprod.homeidentity.online.scee.com",
            "cprod.homeserverservices.online.scee.com",
            "cdev.homerewards.online.scee.com",
            "cdev.homeidentity.online.scee.com",
            "cdev.homeserverservices.online.scee.com",
            "cdevb.homerewards.online.scee.com",
            "cdevb.homeidentity.online.scee.com",
            "cdevb.homeserverservices.online.scee.com",
            "nonprod1.homerewards.online.scee.com",
            "nonprod1.homeidentity.online.scee.com",
            "nonprod1.homeserverservices.online.scee.com",
            "nonprod2.homerewards.online.scee.com",
            "nonprod2.homeidentity.online.scee.com",
            "nonprod2.homeserverservices.online.scee.com",
            "nonprod3.homerewards.online.scee.com",
            "nonprod3.homeidentity.online.scee.com",
            "nonprod3.homeserverservices.online.scee.com",
            "nonprod4.homerewards.online.scee.com",
            "nonprod4.homeidentity.online.scee.com",
            "nonprod4.homeserverservices.online.scee.com",
        };

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
                new JProperty("minibase", SSFWMinibase),
                new JProperty("legacyKey", SSFWLegacyKey),
                new JProperty("sessionidKey", SSFWSessionIdKey),
                new JProperty("time_to_live", SSFWTTL),
                new JProperty("cross_save", SSFWCrossSave),
                new JProperty("prefer_native_httplistener_engine", PreferNativeHttpListenerEngine),
                new JProperty("buffer_size", BufferSize),
                new JProperty("enable_keep_alive", EnableKeepAlive),
                new JProperty("enable_http_compression", EnableHTTPCompression),
                new JProperty("static_folder", SSFWStaticFolder),
                new JProperty("https_dns_list", HTTPSDNSList ?? Array.Empty<string>()),
                new JProperty("certificate_file", HTTPSCertificateFile),
                new JProperty("certificate_password", HTTPSCertificatePassword),
                new JProperty("certificate_hashing_algorithm", HTTPSCertificateHashingAlgorithm.Name),
                new JProperty("scenelist_file", ScenelistFile)
            ).ToString());

            return;
        }

        try
        {
            // Parse the JSON configuration
            dynamic config = JObject.Parse(File.ReadAllText(configPath));

            ushort config_version = GetValueOrDefault(config, "config_version", (ushort)0);
            if (config_version >= 2)
            {
                if (config_version >= 3)
                {
                    PreferNativeHttpListenerEngine = GetValueOrDefault(config, "prefer_native_httplistener_engine", PreferNativeHttpListenerEngine);
                    BufferSize = GetValueOrDefault(config, "buffer_size", BufferSize);
                    EnableKeepAlive = GetValueOrDefault(config, "enable_keep_alive", EnableKeepAlive);
                    EnableHTTPCompression = GetValueOrDefault(config, "enable_http_compression", EnableHTTPCompression);
                }
                SSFWMinibase = GetValueOrDefault(config, "minibase", SSFWMinibase);
                SSFWTTL = GetValueOrDefault(config, "time_to_live", SSFWTTL);
                SSFWLegacyKey = GetValueOrDefault(config, "legacyKey", SSFWLegacyKey);
                SSFWSessionIdKey = GetValueOrDefault(config, "sessionidKey", SSFWSessionIdKey);
                SSFWCrossSave = GetValueOrDefault(config, "cross_save", SSFWCrossSave);
                SSFWStaticFolder = GetValueOrDefault(config, "static_folder", SSFWStaticFolder);
                HTTPSCertificateFile = GetValueOrDefault(config, "certificate_file", HTTPSCertificateFile);
                HTTPSCertificatePassword = GetValueOrDefault(config, "certificate_password", HTTPSCertificatePassword);
                HTTPSCertificateHashingAlgorithm = new HashAlgorithmName(GetValueOrDefault(config, "certificate_hashing_algorithm", HTTPSCertificateHashingAlgorithm.Name));
                HTTPSDNSList = GetValueOrDefault(config, "https_dns_list", HTTPSDNSList);
                ScenelistFile = GetValueOrDefault(config, "scenelist_file", ScenelistFile);
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
    private static string configDir = Directory.GetCurrentDirectory() + "/static/";
    private static string configPath = configDir + "SSFWServer.json";
    private static string configMultiServerLibraryPath = configDir + "MultiServerLibrary.json";
    private static SnmpTrapSender? trapSender = null;
    private static Timer? SceneListTimer;
    private static Timer? SessionTimer;
    private static List<SSFWProcessor>? HTTPBag = null;
    private static Thread? WarmUpThread;

    private static void StartOrUpdateServer()
    {
        if (HTTPBag != null)
        {
            foreach (var httpsBag in HTTPBag)
            {
                httpsBag.StopServer();
            }
        }

        SceneListTimer?.Dispose();
        SessionTimer?.Dispose();

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        SceneListTimer = new Timer(ScenelistParser.UpdateSceneDictionary, null, TimeSpan.Zero, TimeSpan.FromMinutes(30));
        SessionTimer = new Timer(SSFWUserSessionManager.SessionCleanupLoop, null, TimeSpan.Zero, TimeSpan.FromMinutes(15));

        MultiServerLibrary.SSL.CertificateHelper.InitializeSSLChainSignedCertificates(SSFWServerConfiguration.HTTPSCertificateFile, SSFWServerConfiguration.HTTPSCertificatePassword,
            SSFWServerConfiguration.HTTPSDNSList, SSFWServerConfiguration.HTTPSCertificateHashingAlgorithm);

        WarmUpThread = new Thread(WarmUpServers)
        {
            Name = "Server Warm Up"
        };
        WarmUpThread.Start();
    }

    private static void WarmUpServers()
    {
        int optimalProcessorCount = Environment.ProcessorCount;

        HTTPBag = new();

        lock (HTTPBag)
        {
            foreach (var port in new ushort[] { 8080, 10443 })
            {
                if (TCPUtils.IsTCPPortAvailable(port))
                    HTTPBag.Add(new SSFWProcessor(SSFWServerConfiguration.HTTPSCertificateFile, SSFWServerConfiguration.HTTPSCertificatePassword, "*", port, port.ToString().EndsWith("443"), optimalProcessorCount));
            }
        }
    }

    static void Main()
    {
        if (!MultiServerLibrary.Extension.Microsoft.Win32API.IsWindows)
            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
        else
            TechnitiumLibrary.Net.Firewall.FirewallHelper.CheckFirewallEntries(Assembly.GetEntryAssembly()?.Location);

        LoggerAccessor.SetupLogger("SSFWServer", Directory.GetCurrentDirectory());

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

        if (SSFWServerConfiguration.PreferNativeHttpListenerEngine
            && MultiServerLibrary.Extension.Microsoft.Win32API.IsWindows
            && !MultiServerLibrary.Extension.Microsoft.Win32API.IsAdministrator())
        {
            LoggerAccessor.LogWarn("[Program] - Trying to restart as admin...");
            if (MultiServerLibrary.Extension.Microsoft.Win32API.StartAsAdmin(Process.GetCurrentProcess().MainModule?.FileName))
                Environment.Exit(0);
        }

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

        // Previous versions had an erronious config label, we hotfix that.
        string oldConfigPath = Path.GetDirectoryName(configPath) + $"/ssfw.json";
        if (File.Exists(oldConfigPath))
        {
            if (!File.Exists(configPath))
            {
                LoggerAccessor.LogWarn("[Main] - Detected older incorrect SSFWServer configuration file path, performing file renaming...");
                File.Move(oldConfigPath, configPath);
            }
        }

        SSFWServerConfiguration.RefreshVariables(configPath);

        LoggerAccessor.LogInfo($"[Program] - Home client certificate challenges exporting to disk {(ToolsImplementation.ClientsCertificatesWrittenToDisk ? "was successful" : "failed, challenges check will be disabled")}.");

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

                            SSFWServerConfiguration.RefreshVariables(configPath);

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