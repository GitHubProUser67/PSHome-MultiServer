using CustomLogger;
using SSFWServer;
using System.Reflection;
using System.Runtime;
using Microsoft.Extensions.Logging;
using MultiServerLibrary;
using MultiServerLibrary.SNMP;
using SSFWServer.Helpers.FileHelper;

class Program
{
    private static readonly string configDir = Directory.GetCurrentDirectory() + "/static/";
    private static readonly string configPath = configDir + "SSFWServer.json";
    private static readonly string configMultiServerLibraryPath = configDir + "MultiServerLibrary.json";
    private static SnmpTrapSender? trapSender = null;
    private static Timer? SceneListTimer;
    private static Timer? SessionTimer;
    private static SSFWProcessor? HTTPServer = null;

    private static void StartOrUpdateServer()
    {
        HTTPServer?.StopSSFW();

        SceneListTimer?.Dispose();
        SessionTimer?.Dispose();

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        SceneListTimer = new Timer(ScenelistParser.UpdateSceneDictionary, null, TimeSpan.Zero, TimeSpan.FromMinutes(30));
        SessionTimer = new Timer(SSFWUserSessionManager.SessionCleanupLoop, null, TimeSpan.Zero, TimeSpan.FromMinutes(15));

        MultiServerLibrary.SSL.CertificateHelper.InitializeSSLChainSignedCertificates(SSFWServerConfiguration.HTTPSCertificateFile, SSFWServerConfiguration.HTTPSCertificatePassword,
            SSFWServerConfiguration.HTTPSDNSList, SSFWServerConfiguration.HTTPSCertificateHashingAlgorithm);

        HTTPServer = new SSFWProcessor(SSFWServerConfiguration.HTTPSCertificateFile, SSFWServerConfiguration.HTTPSCertificatePassword, SSFWServerConfiguration.SSFWLegacyKey);
        HTTPServer.StartSSFW();
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

    /// <summary>
    /// Extract a portion of a string winthin boundaries.
    /// <para>Extrait une portion d'un string entre des limites.</para>
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="startToRemove">The amount of characters to remove from the left to the right.</param>
    /// <param name="endToRemove">The amount of characters to remove from the right to the left.</param>
    /// <returns>A string.</returns>
    public static string? ExtractPortion(string input, int startToRemove, int endToRemove)
    {
        if (input.Length < startToRemove + endToRemove)
            return null;

        return input[startToRemove..][..^endToRemove];
    }
}