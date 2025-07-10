using CustomLogger;
using Newtonsoft.Json;
using Horizon.RT.Models;
using Horizon.MUIS.Config;
using Horizon.PluginManager;
using System.Net;
using Horizon.HTTPSERVICE;
using Horizon.LIBRARY.Database.Models;
using Horizon.MUM;
using Horizon.SERVER.Extension.PlayStationHome;
using NetworkLibrary.Extension;

namespace Horizon.MUIS
{
    public class MuisClass
    {
        private static string? CONFIG_FILE => HorizonServerConfiguration.MUISConfig;

        public static ServerSettings Settings = new();

        public static IPAddress SERVER_IP = IPAddress.None;

        public static MumManager Manager = new();
        public static MediusPluginsManager Plugins = new(HorizonServerConfiguration.PluginsFolder);

        public static RSA_KEY? GlobalAuthPublic = null;

        public static MUIS[]? UniverseInfoServers = null;

        public static List<HomeOffsetsJsonData> HomeOffsetsList = new();

        private static Dictionary<int, AppSettings> _appSettings = new();
        private static AppSettings _defaultAppSettings = new(0);
        private static Dictionary<string, int[]> _appIdGroups = new();
        private static ulong _sessionKeyCounter = 0;
        private static readonly object _sessionKeyCounterLock = _sessionKeyCounter;
        private static DateTime lastConfigRefresh = DateTimeUtils.GetHighPrecisionUtcTime();
        private static DateTime? _lastSuccessfulDbAuth = null;

        public static bool started = false;

        private static async Task TickAsync()
        {
            try
            {
                // Attempt to authenticate with the db middleware
                // We do this every 24 hours to get a fresh new token
                if (_lastSuccessfulDbAuth == null || (DateTimeUtils.GetHighPrecisionUtcTime() - _lastSuccessfulDbAuth.Value).TotalHours > 24)
                {
                    if (!await HorizonServerConfiguration.Database.Authenticate())
                    {
                        // Log and exit when unable to authenticate
                        LoggerAccessor.LogError($"Unable to authenticate connection to Cache Server.");
                        return;
                    }
                    else
                    {
                        _lastSuccessfulDbAuth = DateTimeUtils.GetHighPrecisionUtcTime();

                        // pass to manager
                        await OnDatabaseAuthenticated();

                        // refresh app settings
                        await RefreshAppSettings();

                        #region Check Cache Server Simulated
                        if (HorizonServerConfiguration.Database._settings.SimulatedMode != true)
                            LoggerAccessor.LogInfo("Connected to Cache Server");
                        else
                            LoggerAccessor.LogInfo("Connected to Cache Server (Simulated)");
                        #endregion
                    }
                }

                if (UniverseInfoServers != null)
                    await Task.WhenAll(UniverseInfoServers.Select(x => x.Tick()));

                // Reload config
                if ((DateTimeUtils.GetHighPrecisionUtcTime() - lastConfigRefresh).TotalMilliseconds > Settings.RefreshConfigInterval)
                {
                    RefreshConfig();
                    lastConfigRefresh = DateTimeUtils.GetHighPrecisionUtcTime();
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }
        }

        private static async Task LoopServer()
        {
            // iterate
            while (started)
            {
                // tick
                await TickAsync();

                await Task.Delay(100); // this value is the one used in Horizon by default.
            }
        }

        public static async void StopServer()
        {
            started = false;

            if (UniverseInfoServers != null)
                await Task.WhenAll(UniverseInfoServers.Select(x => x.Stop()));
        }

        private static Task StartServerAsync()
        {
            string datetime = DateTime.Now.ToString("MMMM/dd/yyyy hh:mm:ss tt");

            LoggerAccessor.LogInfo("**************************************************");

            string gpszVersionString = "3.05.201109161400";

            LoggerAccessor.LogInfo($"* Medius Universe Information Server Version {gpszVersionString}");
            LoggerAccessor.LogInfo($"* Launched on {datetime}");

            if (HorizonServerConfiguration.Database._settings.SimulatedMode == true)
                LoggerAccessor.LogInfo("* Database Disabled Medius Stack");
            else
                LoggerAccessor.LogInfo("* Database Enabled Medius Stack");

            UniverseInfoServers = new MUIS[Settings.Ports.Length];
            for (int i = 0; i < UniverseInfoServers.Length; ++i)
            {
                try
                {
                    LoggerAccessor.LogInfo($"* Enabling MUIS on TCP Port = {Settings.Ports[i]}.");
                    UniverseInfoServers[i] = new MUIS(Settings.Ports[i]);
                    UniverseInfoServers[i].Start();
                }
                catch (Exception)
                {
                    LoggerAccessor.LogError($"MUIS failed to start on TCP Port = {Settings.Ports[i]}");
                }
            }

            LoggerAccessor.LogInfo($"* Server Key Type: {Settings.EncryptMessages}");

            #region Remote Log Viewing
            if (Settings.RemoteLogViewPort == 0)
                //* Remote log viewing setup failure with port %d.
                LoggerAccessor.LogInfo("* Remote log viewing disabled.");
            else if (Settings.RemoteLogViewPort != 0)
                LoggerAccessor.LogInfo($"* Remote log viewing enabled at port {Settings.RemoteLogViewPort}.");
            #endregion

            #region MediusGetVersion
            if (Settings.MediusServerVersionOverride == true)
            {
                // Use override methods in code to send our own version string from config
                LoggerAccessor.LogInfo("Using config input server version");
                LoggerAccessor.LogInfo($"MUISVersion Version: {Settings.MUISVersion}");

            }
            else
                // Use hardcoded methods in code to handle specific games server versions
                LoggerAccessor.LogInfo("Using game specific server versions");

            #endregion

            //* Diagnostic Profiling Enabled: %d Counts

            LoggerAccessor.LogInfo("**************************************************");

            if (Settings.NATIp == null)
                LoggerAccessor.LogError("[MUIS] - No NAT ip found! Errors can happen.");

            LoggerAccessor.LogInfo($"MUIS initalized.");

            started = true;

            _ = Task.Run(LoopServer);

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
            // Determine server ip
            RefreshServerIp();

            // Load settings
            #region Check Config.json
            if (File.Exists(CONFIG_FILE))
                // Populate existing object
                JsonConvert.PopulateObject(File.ReadAllText(CONFIG_FILE), Settings, new JsonSerializerSettings()
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                });
            else
            {
                // Add the appids to the ApplicationIds list
                Settings.CompatibleApplicationIds.AddRange(new List<int>
                {
                    11204, 11354, 21914, 21624, 20764, 20371, 20384, 22500, 10540, 10550, 10582, 10584, 22920, 
                    22923, 22924, 21731, 21834, 23624, 20032, 20034, 20454, 20314, 21874, 21244, 20304, 20463, 
                    21614, 20344, 20434, 22204, 23360, 21513, 21064, 20804, 20374, 21094, 20060, 10984, 10782, 
                    10421, 10130, 10954, 21784, 21564, 21354, 21564, 21574, 21584, 21594, 22274, 22284, 22294, 
                    22304, 20040, 20041, 20042, 20043, 20044
                });

                string? iptofile = SERVER_IP?.ToString();

                if (string.IsNullOrEmpty(iptofile))
                    iptofile = "127.0.0.1";

                // Add default localhost entry
                Settings.Universes.Add(0, new UniverseInfo[] {
                    new UniverseInfo()
                    {
                        Name = "sample universe",
                        Description = null,
                        UserCount = 0,
                        MaxUsers = 0,
                        Endpoint = "url",
                        SvoURL = "url",
                        ExtendedInfo = null,
                        UniverseBilling = null,
                        BillingSystemName = null,
                        Port = 10075,
                        UniverseId = 1
                    }
                });

                // Populate with other entries
                Settings.Universes.Add(10130, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "Hardware Online Arena Beta",
                        Description = "Beta Universe",
                        Endpoint = iptofile,
                        Port = 10075,
                        UniverseId = 31,
                        SvoURL = null,
                        Status = 1,
                        UserCount = 1,
                        MaxUsers = 256,
                        ExtendedInfo = null,
                        UniverseBilling = "SCEA",
                        BillingSystemName = "Sony Computer Entertainment America, Inc. Billing System"
                    }
                });

                Settings.Universes.Add(10582, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "Hot Shots Golf: Fore! Online Public Beta",
                        Description = "PuBeta Universe",
                        Endpoint = iptofile,
                        Port = 10075,
                        UniverseId = 31,
                        SvoURL = null,
                        Status = 1,
                        UserCount = 1,
                        MaxUsers = 256,
                        ExtendedInfo = null,
                        UniverseBilling = "SCEA",
                        BillingSystemName = "Sony Computer Entertainment America, Inc. Billing System"
                    }
                });

                Settings.Universes.Add(10584, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "Hot Shots Golf: Fore!",
                        Description = "Retail Universe",
                        Endpoint = iptofile,
                        Port = 10075,
                        UniverseId = 31,
                        SvoURL = null,
                        Status = 1,
                        UserCount = 1,
                        MaxUsers = 256,
                        ExtendedInfo = null,
                        UniverseBilling = "SCEA",
                        BillingSystemName = "Sony Computer Entertainment America, Inc. Billing System"
                    }
                });

                Settings.Universes.Add(10954, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "Formula One 05 Server",
                        Description = "Retail Universe",
                        Endpoint = iptofile,
                        Port = 10075,
                        UniverseId = 31,
                        SvoURL = null,
                        Status = 1,
                        UserCount = 1,
                        MaxUsers = 256,
                        ExtendedInfo = null,
                        UniverseBilling = "SCEA",
                        BillingSystemName = "Sony Computer Entertainment America, Inc. Billing System"
                    }
                });

                Settings.Universes.Add(11204, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "JakX Online",
                        Description = "Retail Europe Universe",
                        Status = 1,
                        UserCount = 1,
                        MaxUsers = 15000,
                        Endpoint = iptofile,
                        SvoURL = null,
                        ExtendedInfo = null,
                        Port = 10075,
                        UniverseId = 1
                    }
                });

                Settings.Universes.Add(21784, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "Killzone 2 Lobby",
                        Description = "Crush your ennemies",
                        Endpoint = iptofile,
                        Port = 10075,
                        UniverseId = 31,
                        SvoURL = $"http://{iptofile}:10060/SOCOMCF_SVML/index.jsp ",
                        Status = 1,
                        UserCount = 1,
                        MaxUsers = 15000,
                        ExtendedInfo = null,
                        UniverseBilling = "SCEA",
                        BillingSystemName = "Sony Computer Entertainment America, Inc. Billing System"
                    }
                });

                Settings.Universes.Add(21914, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "PAIN",
                        Description = "Here comes the PAIN",
                        Endpoint = iptofile,
                        Port = 10075,
                        UniverseId = 31,
                        SvoURL = null,
                        Status = 1,
                        UserCount = 1,
                        MaxUsers = 256,
                        ExtendedInfo = null,
                        UniverseBilling = "SCEA",
                        BillingSystemName = "Sony Computer Entertainment America, Inc. Billing System"
                    }
                });

                Settings.Universes.Add(10540, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "Socom II November Beta",
                        Description = "Beta Universe",
                        Endpoint = iptofile,
                        Port = 10075,
                        UniverseId = 31,
                        SvoURL = null,
                        Status = 1,
                        UserCount = 1,
                        MaxUsers = 256,
                        ExtendedInfo = null,
                        UniverseBilling = "SCEA",
                        BillingSystemName = "Sony Computer Entertainment America, Inc. Billing System"
                    }
                });


                Settings.Universes.Add(10550, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "Eyetoy Chat Beta",
                        Description = "Beta Universe",
                        Endpoint = iptofile,
                        Port = 10075,
                        UniverseId = 1,
                        SvoURL = null,
                        Status = 1,
                        UserCount = 1,
                        MaxUsers = 256,
                        ExtendedInfo = null,
                        UniverseBilling = "SCEA",
                        BillingSystemName = "Sony Computer Entertainment America, Inc. Billing System"
                    }
                });

                Settings.Universes.Add(10782, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "GT4 Online Public Beta",
                        Description = "Revived by MultiServer",
                        Status = 1,
                        UserCount = 0,
                        MaxUsers = 15000,
                        Endpoint = iptofile,
                        SvoURL = null,
                        ExtendedInfo = null,
                        Port = 10075,
                        UniverseId = 1
                    }
                });

                Settings.Universes.Add(10421, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "Arc the Lad: Generations Preview",
                        Description = "Revived by MultiServer",
                        Status = 1,
                        UserCount = 0,
                        MaxUsers = 10000,
                        Endpoint = iptofile,
                        Port = 10075,
                        UniverseId = 1
                    }
                });

                Settings.Universes.Add(10984, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "Arc the Lad: EoD US",
                        Description = "Revived by MultiServer",
                        Status = 1,
                        UserCount = 0,
                        MaxUsers = 10000,
                        Endpoint = iptofile,
                        Port = 10075,
                        UniverseId = 1
                    }
                });

                Settings.Universes.Add(11354, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "Ratchet Deadlocked Online",
                        Description = "New Universe",
                        Status = 1,
                        UserCount = 1,
                        MaxUsers = 15000,
                        Endpoint = iptofile,
                        SvoURL = null,
                        ExtendedInfo = null,
                        Port = 10075,
                        UniverseId = 1
                    }
                });

                Settings.Universes.Add(20060, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "F1 2006",
                        Description = "Revived by MultiServer",
                        Status = 1,
                        UserCount = 0,
                        MaxUsers = 10000,
                        Endpoint = iptofile,
                        SvoURL = $"http://{iptofile}:10060/F12006_SVML/index.jsp ",
                        Port = 10075,
                        UniverseId = 1
                    }
                });

                Settings.Universes.Add(21064, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "Syphon Filter: Logan's Shadow",
                        Description = "Test",
                        Status = 1,
                        UserCount = 1,
                        MaxUsers = 256,
                        Endpoint = iptofile,
                        SvoURL = $"http://{iptofile}:10060/SFO2PSP_SVML/index.jsp ",
                        ExtendedInfo = null,
                        UniverseBilling = null,
                        BillingSystemName = null,
                        Port = 10075,
                        UniverseId = 1
                    }
                });

                Settings.Universes.Add(21094, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "Socom Confrontation Prod",
                        Description = "v1.61",
                        Status = 1,
                        UserCount = 1,
                        MaxUsers = 256,
                        Endpoint = iptofile,
                        SvoURL = $"http://{iptofile}:10060/CONFRONTATION_XML/uri/index.jsp ",
                        ExtendedInfo = null,
                        UniverseBilling = null,
                        BillingSystemName = null,
                        Port = 10075,
                        UniverseId = 1
                    }
                });

                Settings.Universes.Add(20804, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "Syphon Filter: Logan's Shadow Test Sample",
                        Description = "Test",
                        Status = 1,
                        UserCount = 1,
                        MaxUsers = 256,
                        Endpoint = iptofile,
                        SvoURL = $"http://{iptofile}:10060/SFO2PSP_SVML/index.jsp ",
                        ExtendedInfo = null,
                        UniverseBilling = null,
                        BillingSystemName = null,
                        Port = 10075,
                        UniverseId = 1
                    }
                });

                Settings.Universes.Add(21513, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "Syphon Filter: Logan's Shadow Test Sample",
                        Description = "Test",
                        Status = 1,
                        UserCount = 1,
                        MaxUsers = 256,
                        Endpoint = iptofile,
                        SvoURL = $"http://{iptofile}:10060/SFO2PSP_SVML/index.jsp ",
                        ExtendedInfo = null,
                        UniverseBilling = null,
                        BillingSystemName = null,
                        Port = 10075,
                        UniverseId = 1
                    }
                });

                Settings.Universes.Add(22204, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "Motorstorm PSP",
                        Description = "Revived by MultiServer",
                        Status = 1,
                        UserCount = 1,
                        MaxUsers = 256,
                        Endpoint = iptofile,
                        SvoURL = $"http://{iptofile}:10060/MOTORSTORMPSP_SVML/index.jsp ",
                        ExtendedInfo = null,
                        UniverseBilling = null,
                        BillingSystemName = null,
                        Port = 10075,
                        UniverseId = 1
                    }
                });

                Settings.Universes.Add(23360, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "Wipeout HD",
                        Description = "Revived by MultiServer",
                        Status = 1,
                        UserCount = 1,
                        MaxUsers = 256,
                        Endpoint = iptofile,
                        SvoURL = $"https://{iptofile}:10062/wox_ws/rest/main/Start ",
                        ExtendedInfo = null,
                        UniverseBilling = null,
                        BillingSystemName = null,
                        Port = 10075,
                        UniverseId = 1
                    }
                });

                Settings.Universes.Add(20624, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "Calling All Cars",
                        Description = null,
                        Status = 1,
                        UserCount = 1,
                        MaxUsers = 256,
                        Endpoint = iptofile,
                        SvoURL = null,
                        ExtendedInfo = null,
                        UniverseBilling = null,
                        BillingSystemName = null,
                        Port = 10075,
                        UniverseId = 2
                    }
                });

                Settings.Universes.Add(20764, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "Motorstorm NTSC",
                        Description = "Revival by MultiServer",
                        Endpoint = iptofile,
                        Port = 10075,
                        UniverseId = 4,
                        SvoURL = null,
                        ExtendedInfo = $"v3.1 http://{iptofile}/frostfight.prod/myuser/BCUS98137",
                        Status = 1,
                        UserCount = 1,
                        MaxUsers = 256,
                    }
                });

                Settings.Universes.Add(20364, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "Motorstorm PAL",
                        Description = "Revival by MultiServer",
                        Endpoint = iptofile,
                        Port = 10075,
                        UniverseId = 5,
                        SvoURL = $"http://{iptofile}:10060/socomcf/index ",
                        ExtendedInfo = $"v3.1 http://{iptofile}/frostfight.prod/myuser/BCES00006",
                        Status = 1,
                        UserCount = 1,
                        MaxUsers = 256,
                    }
                });

                Settings.Universes.Add(21624, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "Motorstorm: Pacific Rift",
                        Description = "Revival by MultiServer",
                        Endpoint = iptofile,
                        Port = 10075,
                        UniverseId = 4,
                        SvoURL = null,
                        ExtendedInfo = null,
                        Status = 1,
                        UserCount = 1,
                        MaxUsers = 256,
                    }
                });

                Settings.Universes.Add(21614, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "Wipeout Pulse PSP",
                        Description = "Revival by MultiServer",
                        Endpoint = iptofile,
                        Port = 10075,
                        UniverseId = 4,
                        SvoURL = "NONE",
                        ExtendedInfo = null,
                        Status = 1,
                        UserCount = 1,
                        MaxUsers = 256,
                    }
                });

                Settings.Universes.Add(20344, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "ATV Offroad Fury Pro PSP",
                        Endpoint = iptofile,
                        Port = 10075,
                        SvoURL = $"http://{iptofile}:10060/ATV4UNIFIED_SVML/index.jsp ",
                    }
                });

                Settings.Universes.Add(20371, new UniverseInfo[]
                    {
                        new UniverseInfo()
                        {
                            Name = "muis",
                            Description = "01",
                            Endpoint = iptofile,
                            Status = 1,
                            UserCount = 1,
                            MaxUsers = 15000,
                            SvoURL = $"http://{iptofile}:10060/HUBPS3_SVML/unity/start.jsp ",
                            UniverseBilling = "SCEA",
                            BillingSystemName = "Sony Computer Entertainment America, Inc. Billing System",
                            ExtendedInfo = $"01.86 http://{iptofile}/dev.01.86/",
                            Port = 10075,
                            UniverseId = 1
                        }
                    });

                Settings.Universes.Add(20374, new UniverseInfo[]
                     {
                        new UniverseInfo()
                        {
                            Name = "CPROD prod1 (Public MUIS)",
                            Description = "01",
                            Endpoint = iptofile,
                            Status = 1,
                            UserCount = 1,
                            MaxUsers = 15000,
                            SvoURL = $"http://{iptofile}:10060/HUBPS3_SVML/unity/start.jsp ",
                            ExtendedInfo = $"01.86 http://{iptofile}/01.86/",
                            UniverseBilling = "SCEA",
                            BillingSystemName = "Sony Computer Entertainment America, Inc. Billing System",
                            Port = 10075,
                            UniverseId = 1
                        }
                     });

                Settings.Universes.Add(20384, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "Singstar Lobby",
                        Description = "SingAllTogether",
                        Status = 1,
                        UserCount = 1,
                        MaxUsers = 256,
                        Endpoint = iptofile,
                        SvoURL = $"http://{iptofile}:10060/SINGSTARPS3_SVML/start.jsp ",
                        ExtendedInfo = null,
                        UniverseBilling = "SCEA",
                        BillingSystemName = "Sony Computer Entertainment America, Inc. Billing System",
                        Port = 10075,
                        UniverseId = 1
                    }
                });

                Settings.Universes.Add(21354, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "Singstar Lobby",
                        Description = "SingAllTogether",
                        Status = 1,
                        UserCount = 1,
                        MaxUsers = 256,
                        Endpoint = iptofile,
                        SvoURL = $"http://{iptofile}:10060/SINGSTARPS3_SVML/start.jsp ",
                        ExtendedInfo = null,
                        UniverseBilling = "SCEA",
                        BillingSystemName = "Sony Computer Entertainment America, Inc. Billing System",
                        Port = 10075,
                        UniverseId = 1
                    }
                });

                Settings.Universes.Add(23014, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "Singstar Lobby",
                        Description = "SingAllTogether",
                        Status = 1,
                        UserCount = 1,
                        MaxUsers = 256,
                        Endpoint = iptofile,
                        SvoURL = $"http://{iptofile}:10060/SINGSTARPS3_SVML/start.jsp ",
                        ExtendedInfo = null,
                        UniverseBilling = "SCEA",
                        BillingSystemName = "Sony Computer Entertainment America, Inc. Billing System",
                        Port = 10075,
                        UniverseId = 1
                    }
                });

                Settings.Universes.Add(21834, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "Twisted Metal X Online",
                        Description = "Revived by MultiServer",
                        Status = 1,
                        UserCount = 1,
                        MaxUsers = 256,
                        Endpoint = iptofile,
                        SvoURL = "http://twistedmetalx-prod3.svo.online.scea.com:10060/TWISTEDMETALX_XML/uri/URIStore.do",
                        ExtendedInfo = null,
                        UniverseBilling = null,
                        BillingSystemName = null,
                        Port = 10075,
                        UniverseId = 1
                    }
                });

                Settings.Universes.Add(20304, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "Socom FTB2",
                        Description = "Revived by MultiServer",
                        Status = 1,
                        UserCount = 1,
                        MaxUsers = 256,
                        Endpoint = iptofile,
                        SvoURL = $"http://{iptofile}:10060/socom3/index ",
                        ExtendedInfo = $"v1.60 http://{iptofile}/ftb2/manifest.txt",
                        UniverseBilling = null,
                        BillingSystemName = null,
                        Port = 10075,
                        UniverseId = 1
                    }
                });

                Settings.Universes.Add(20032, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "Socom FTB Pubeta",
                        Description = "Revived by MultiServer",
                        Status = 1,
                        UserCount = 1,
                        MaxUsers = 256,
                        Endpoint = iptofile,
                        SvoURL = $"http:/{iptofile}:10060/SOCOMPUBETAPSP_SVML/index.jsp ",
                        ExtendedInfo = null,
                        UniverseBilling = null,
                        BillingSystemName = null,
                        Port = 10075,
                        UniverseId = 1
                    }
                });

                Settings.Universes.Add(22920, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "Starhawk Online Beta",
                        Description = "Starhawk Online Beta lobby",
                        Endpoint = iptofile,
                        Port = 10075,
                        UniverseId = 2,
                        SvoURL = $"http://{iptofile}:10060/BOURBON_XML/uri/URIStore.do ",
                        Status = 1,
                        UserCount = 1,
                        MaxUsers = 256,
                        ExtendedInfo = "<XML><A url=`http://blob117.scea.com` latest=`0` access=`0` /></XML>",
                        UniverseBilling = "SCEA",
                        BillingSystemName = "Sony Computer Entertainment America, Inc. Billing System"
                    }
                });

                Settings.Universes.Add(22500, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "Motorstorm 3 Apocalypse",
                        Description = "Motorstorm 3 Apocalypse",
                        Endpoint = iptofile,
                        Port = 10075,
                        UniverseId = 11,
                        SvoURL = $"http://{iptofile}:10060/MOTORSTORM3PS3_XML/ ",
                        Status = 1,
                        UserCount = 1,
                        MaxUsers = 256,
                        ExtendedInfo = null,
                    }
                });

                Settings.Universes.Add(21731, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "Resistance 2 Private Beta",
                        Description = "Revived by MultiServer",
                        Endpoint = iptofile,
                        Port = 10075,
                        UniverseId = 2,
                        Status = 1,
                        UserCount = 1,
                        MaxUsers = 256,
                    }
                });

                Settings.Universes.Add(23624, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "Buzz! Quiz Player",
                        Description = "Revived by MultiServer",
                        Endpoint = iptofile,
                        Port = 10075,
                        UniverseId = 2,
                        SvoURL = $"http://{iptofile}:10060/BUZZPS3_XML/index.jsp ",
                        Status = 1,
                        UserCount = 1,
                        MaxUsers = 256,
                    }
                });

                Settings.Universes.Add(20034, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "Socom FTB Prod",
                        Description = "Revived by MultiServer",
                        Status = 1,
                        UserCount = 1,
                        MaxUsers = 256,
                        Endpoint = iptofile,
                        Port = 10075,
                        UniverseId = 1
                    }
                });

                Settings.Universes.Add(20454, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "Killzone Lib v1.20",
                        Description = "Revival by MultiServer",
                        Endpoint = iptofile,
                        Port = 10075,
                        UniverseId = 4,
                        SvoURL = $"http://{iptofile}:1006/KILLZONEPSP_SVML/index.jsp ",
                        ExtendedInfo = null,
                        Status = 1,
                        UserCount = 1,
                        MaxUsers = 256,
                    }
                });

                Settings.Universes.Add(20314, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "Lemmings PSP",
                        Description = "Revival by MultiServer",
                        Endpoint = iptofile,
                        Port = 10075,
                        UniverseId = 4,
                        SvoURL = $"http://{iptofile}:10060/LEMMINGSPSP_SVML/index.jsp ",
                        ExtendedInfo = null,
                        UniverseBilling = "SCEA",
                        BillingSystemName = "Sony Computer Entertainment America Inc.",
                        Status = 1,
                        UserCount = 1,
                        MaxUsers = 256,
                    }
                });

                Settings.Universes.Add(21874, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "Resistance PSP",
                        Description = "Revival by MultiServer",
                        Status = 1,
                        UserCount = 1,
                        MaxUsers = 256,
                        Endpoint = iptofile,
                        SvoURL = $"http://{iptofile}:10060/SOCOMTACTICS_SVML/index.jsp ",
                        ExtendedInfo = null,
                        Port = 10075,
                        UniverseId = 1
                    }
                });

                Settings.Universes.Add(21244, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "Socom Tactical PSP",
                        Description = "Revival by MultiServer",
                        Status = 1,
                        UserCount = 1,
                        MaxUsers = 256,
                        Endpoint = iptofile,
                        SvoURL = "NONE",
                        ExtendedInfo = null,
                        UniverseBilling = null,
                        BillingSystemName = null,
                        Port = 10075,
                        UniverseId = 1
                    }
                });

                Settings.Universes.Add(20463, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "Syphon Filter: Dark Mirror Pre-Prod 0.02",
                        Description = "Revived by MultiServer",
                        Endpoint = iptofile,
                        Port = 10075,
                        UniverseId = 2,
                        Status = 1,
                        UserCount = 1,
                        MaxUsers = 256,
                        SvoURL = "NONE",
                    }
                });

                Settings.Universes.Add(20434, new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "WTS 2006",
                        Description = "Revived by MultiServer",
                        Endpoint = iptofile,
                        Port = 10075,
                        Status = 1,
                        UserCount = 1,
                        MaxUsers = 256,
                        SvoURL = $"http://{iptofile}:10060/WTS06_SVML/index.svml ",
                    }
                });

                List<int> WarhawkAppIDs = new() { 21564, 21574, 21584, 21594, 22274, 22284, 22294, 22304, 20040, 20041, 20042, 20043, 20044 };

                foreach (int AppID in WarhawkAppIDs)
                {
                    Settings.Universes.Add(AppID, new UniverseInfo[]
                    {
                        new()
                        {
                            Name = "Warhawk",
                            Description = "Matchmaking Server",
                            Status = 1,
                            UserCount = 1,
                            MaxUsers = 256,
                            Endpoint = iptofile,
                            SvoURL = $"http://{iptofile}:10060/WARHAWK_SVML/index.jsp?languageID=1 ",
                            ExtendedInfo = $"v1.50 http://{iptofile}/medius-patch/warhawk-prod/r016/",
                            UniverseBilling = "SCEA",
                            BillingSystemName = "Sony Computer Entertainment America Inc.",
                            Port = 10075,
                            UniverseId = 1
                        }
                    });
                }

                Directory.CreateDirectory(Path.GetDirectoryName(CONFIG_FILE) ?? Directory.GetCurrentDirectory() + "/static");

                // Save defaults
                File.WriteAllText(CONFIG_FILE ?? Directory.GetCurrentDirectory() + "/static/muis.json", JsonConvert.SerializeObject(Settings, Formatting.Indented));
            }
            #endregion

            #region Check ebootdefs.json
            if (!string.IsNullOrEmpty(HorizonServerConfiguration.EBOOTDEFSConfig) && File.Exists(HorizonServerConfiguration.EBOOTDEFSConfig))
                LoadHomeOffsetsJson(File.ReadAllText(HorizonServerConfiguration.EBOOTDEFSConfig));
            #endregion

            // Update default rsa key
            LIBRARY.Pipeline.Attribute.ScertClientAttribute.DefaultRsaAuthKey = Settings.DefaultKey;
        }

        private static void LoadHomeOffsetsJson(string? jsonData)
        {
            if (string.IsNullOrEmpty(jsonData))
                return;

            Dictionary<string, HomeOffsetsJsonData>? HomeOffsetsDic = JsonConvert.DeserializeObject<Dictionary<string, HomeOffsetsJsonData>>(jsonData);

            if (HomeOffsetsDic != null)
            {
                foreach (var kvp in HomeOffsetsDic)
                {
                    HomeOffsetsJsonData data = kvp.Value;
                    data.Sha1Hash = kvp.Key;

                    string[]? parts = data.Version?.Split('.');

                    if (parts != null && parts.Length >= 2)
                    {
                        // Take the first two parts for major and minor versions.
                        string major = parts[0];
                        string minor = parts[1];

                        // Concatenate any remaining parts as part of the decimal, if they exist.
                        string remaining = string.Concat(parts.Skip(2));

                        // Construct the double as major.minor and append the remaining if exists
                        data.VersionAsDouble = Convert.ToDouble(remaining.Length > 0 ? $"{major},{minor}{remaining}" : $"{major},{minor}");
                    }
                }

                lock (HomeOffsetsList)
                    HomeOffsetsList = new List<HomeOffsetsJsonData>(HomeOffsetsDic.Values);
            }
            else
                LoggerAccessor.LogError("[MediusClass] - LoadHomeOffsetsJson - jsonData was null or empty!");
        }

        private static void RefreshServerIp()
        {
            #region Determine Server IP
            if (!Settings.UsePublicIp)
                SERVER_IP = IPAddress.Parse(Settings.MUISIp);
            else
            {
                if (string.IsNullOrWhiteSpace(Settings.PublicIpOverride))
                    SERVER_IP = IPAddress.Parse(InternetProtocolUtils.GetPublicIPAddress());
                else
                    SERVER_IP = IPAddress.Parse(Settings.PublicIpOverride);
            }
            #endregion
        }

        public static async Task OnDatabaseAuthenticated()
        {
            // get supported app ids
            var appids = await HorizonServerConfiguration.Database.GetAppIds();

            if (appids != null)
                // build dictionary of app ids from response
                _appIdGroups = appids.ToDictionary(x => x.Name, x => x.AppIds != null ? x.AppIds.ToArray() : Array.Empty<int>());
        }

        private static async Task RefreshAppSettings()
        {
            try
            {
                if (!HorizonServerConfiguration.Database.AmIAuthenticated())
                    return;

                // get supported app ids
                var appIdGroups = await HorizonServerConfiguration.Database.GetAppIds();
                if (appIdGroups == null)
                    return;

                // get settings
                foreach (AppIdDTO appIdGroup in appIdGroups)
                {
                    if (appIdGroup.AppIds != null)
                    {
                        foreach (int appId in appIdGroup.AppIds)
                        {
                            var settings = await HorizonServerConfiguration.Database.GetServerSettings(appId);
                            if (settings != null)
                            {
                                if (_appSettings.TryGetValue(appId, out var appSettings))
                                    appSettings.SetSettings(settings);
                                else
                                {
                                    appSettings = new AppSettings(appId);
                                    appSettings.SetSettings(settings);
                                    _appSettings.Add(appId, appSettings);

                                    // we also want to send this back to the server since this is new locally
                                    // and there might be new setting fields that aren't yet on the db
                                    await HorizonServerConfiguration.Database.SetServerSettings(appId, appSettings.GetSettings());
                                }

                                RoomManager.UpdateOrCreateRoom(Convert.ToString(appId), null, null, null, null, 0, null, false);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }
        }

        /// <summary>
        /// Generates a incremental session key number
        /// </summary>
        /// <returns></returns>
        public static string GenerateSessionKey()
        {
            lock (_sessionKeyCounterLock)
            {
                return (++_sessionKeyCounter).ToString();
            }
        }

        public static AppSettings GetAppSettingsOrDefault(int appId)
        {
            if (_appSettings.TryGetValue(appId, out var appSettings))
                return appSettings;

            return _defaultAppSettings;
        }
    }
}
