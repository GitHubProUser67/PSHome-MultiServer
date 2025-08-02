using CustomLogger;
using Newtonsoft.Json;
using Horizon.RT.Common;
using Horizon.RT.Models;
using Horizon.SERVER.Config;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using Horizon.LIBRARY.libAntiCheat;
using Horizon.PluginManager;
using Horizon.SERVER.Medius;
using Horizon.HTTPSERVICE;
using Horizon.LIBRARY.Database.Models;
using Horizon.MUM;
using Horizon.MUM.Models;
using Horizon.SERVER.Extension.PlayStationHome;
using MultiServerLibrary.Extension;
using Horizon.Extension.PlayStationHome.Models;

namespace Horizon.SERVER
{
    public class MediusClass
    {
        private static string? CONFIG_FILE => HorizonServerConfiguration.MEDIUSConfig;

        public static RSA_KEY? GlobalAuthPublic = null;

        public static ServerSettings Settings = new();

        public static IPAddress SERVER_IP = IPAddress.None;

        public static MumManager Manager = new();
        public static MediusPluginsManager Plugins = new(HorizonServerConfiguration.PluginsFolder);

        public SECURITY_MODE eSecurityMode = SECURITY_MODE.MODE_UNKNOWN;

        public static MAPS ProfileServer = new();
        public static MMS MatchmakingServer = new();
        public static MAS AuthenticationServer = new();
        public static MLS LobbyServer = new();
        public static MPS ProxyServer = new();

        public static AntiCheat AntiCheatPlugin = new();
        public static LIBRARY.libAntiCheat.Models.ClientObject AntiCheatClient = new();

        public static Dictionary<string, string> MUMLocalServersAccessList = new();

        public static List<HomeOffsetsJsonData> HomeOffsetsList = new();

        private static Dictionary<int, AppSettings> _appSettings = new();
        private static AppSettings _defaultAppSettings = new(0);
        private static ulong _sessionKeyCounter = 0;
        private static readonly object _sessionKeyCounterLock = _sessionKeyCounter;
        private static DateTime? _lastSuccessfulDbAuth = null;
        private static DateTime _lastConfigRefresh = DateTimeUtils.GetHighPrecisionUtcTime();
        private static DateTime _lastComponentLog = DateTimeUtils.GetHighPrecisionUtcTime();

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
                        LoggerAccessor.LogInfo("Successfully authenticated with the db middleware server");

                        // pass to manager
                        await Manager.OnDatabaseAuthenticated();

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

                // Tick
                await Task.WhenAll(
                    AuthenticationServer.Tick(),
                    LobbyServer.Tick(),
                    ProxyServer.Tick(),
                    ProfileServer.Tick(),
                    MatchmakingServer.Tick());

                // Tick manager
                await Manager.Tick();

                // Tick plugins
                await Plugins.Tick();

                if ((DateTimeUtils.GetHighPrecisionUtcTime() - _lastComponentLog).TotalSeconds > 15f)
                {
                    AuthenticationServer.Log();
                    LobbyServer.Log();
                    ProxyServer.Log();
                    ProfileServer.Log();
                    MatchmakingServer.Log();
                    _lastComponentLog = DateTimeUtils.GetHighPrecisionUtcTime();
                }

                // Reload config
                if ((DateTimeUtils.GetHighPrecisionUtcTime() - _lastConfigRefresh).TotalMilliseconds > Settings.RefreshConfigInterval)
                {
                    RefreshConfig();
                    _lastConfigRefresh = DateTimeUtils.GetHighPrecisionUtcTime();
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

                await Task.Delay(100);
            }
        }

        public static async void StopServer()
        {
            started = false;

            await AuthenticationServer.Stop();
            await LobbyServer.Stop();
            await ProxyServer.Stop();
            await ProfileServer.Stop();
            await MatchmakingServer.Stop();
        }

        private static async Task StartServerAsync()
        {
            try
            {
                string? AppIdArray = null; //string.Join(", ", Settings.ApplicationIds);

                LoggerAccessor.LogInfo("Initializing medius components...");
                // Program
                LoggerAccessor.LogInfo("**************************************************");

                string datetime = DateTime.Now.ToString("MMMM/dd/yyyy hh:mm:ss tt");
                LoggerAccessor.LogInfo($"* Launched on {datetime}");

                // Get the current process.
                Process currentProcess = Process.GetCurrentProcess();

                //Parent ProcessId
                LoggerAccessor.LogInfo($"* Process ID: {currentProcess.Id}");

                if (HorizonServerConfiguration.Database._settings.SimulatedMode == true)
                    LoggerAccessor.LogInfo("* Database Disabled Medius Stack");
                else
                    LoggerAccessor.LogInfo("* Database Enabled Medius Stack");

                #region Remote Log Viewing
                if (Settings.RemoteLogViewPort == 0)
                    //* Remote log viewing setup failure with port %d.
                    LoggerAccessor.LogInfo("* Remote log viewing disabled.");
                else if (Settings.RemoteLogViewPort > 0)
                    LoggerAccessor.LogInfo($"* Remote log viewing enabled at port {Settings.RemoteLogViewPort}.");
                #endregion

                LoggerAccessor.LogInfo("**************************************************");

                #region Anti-Cheat Init (WIP)
                if (Settings.AntiCheatOn)
                    await AntiCheatPlugin.AntiCheatInit(LM_SEVERITY_LEVEL.LM_INFO, Settings.AntiCheatOn);
                #endregion

                #region Home Closed Beta Plugin
                if (Settings.PlaystationHomeClosedBetaAutoCreatePlugin)
                {
                    HomeClosedBetaChannelManager.InitiateBetaChannelsId(Settings.PlaystationHomeClosedBetaSceneListPath);
                    await HomeClosedBetaChannelManager.GenerateOrUpdateChatChannels(20371).ConfigureAwait(false);
                    await HomeClosedBetaChannelManager.GenerateOrUpdateChatChannels(20374).ConfigureAwait(false);
                }
                #endregion

                #region MediusGetVersion
                if (Settings.MediusServerVersionOverride)
                {
                    #region MAPS - Zipper Interactive MAG/Socom 4
                    if (Settings.EnableMAPS == true)
                    {
                        LoggerAccessor.LogInfo($"MAPS Version: {Settings.MAPSVersion}");
                        LoggerAccessor.LogInfo($"Enabling MAPS on Server IP = {SERVER_IP} TCP Port = {ProfileServer.TCPPort} UDP Port = {ProfileServer.UDPPort}.");

                        ProfileServer.Start();
                        LoggerAccessor.LogInfo("Medius Profile Server Intialized and Now Accepting Clients");
                    }
                    #endregion

                    #region MMS Enabled?
                    if (Settings.EnableMMS == true)
                    {
                        #region MAS 
                        LoggerAccessor.LogInfo($"MMS Version: {Settings.MMSVersion}");
                        LoggerAccessor.LogInfo($"Enabling MMS on Server IP = {SERVER_IP} TCP Port = {MatchmakingServer.TCPPort} UDP Port = {MatchmakingServer.UDPPort}.");
                        //LoggerAccessor.LogInfo($"Medius Matchmaking Server running under ApplicationID {AppIdArray}");

                        //Connecting to Medius Universe Manager 127.0.0.1 10076 1
                        //Connected to Universe Manager server

                        MatchmakingServer.Start();
                        LoggerAccessor.LogInfo("Medius Matchmaking Server Initialized");
                        #endregion

                    }
                    #endregion

                    #region MAS Enabled?
                    if (Settings.EnableMAS == true)
                    {
                        #region MAS 
                        LoggerAccessor.LogInfo($"MAS Version: {Settings.MASVersion}");
                        LoggerAccessor.LogInfo($"Enabling MAS on Server IP = {SERVER_IP} TCP Port = {AuthenticationServer.TCPPort} UDP Port = {AuthenticationServer.UDPPort}.");
                        //LoggerAccessor.LogInfo($"Medius Authentication Server running under ApplicationID {AppIdArray}");

                        //Connecting to Medius Universe Manager 127.0.0.1 10076 1
                        //Connected to Universe Manager server

                        AuthenticationServer.Start();
                        LoggerAccessor.LogInfo("Medius Authentication Server Initialized");
                        #endregion

                    }
                    #endregion

                    #region MLS Enabled?
                    if (Settings.EnableMLS == true)
                    {
                        LoggerAccessor.LogInfo($"MLS Version: {Settings.MLSVersion}");
                        LoggerAccessor.LogInfo($"Enabling MLS on Server IP = {SERVER_IP} TCP Port = {LobbyServer.TCPPort} UDP Port = {LobbyServer.UDPPort}.");
                        LoggerAccessor.LogInfo($"Medius Lobby Server running under ApplicationID {AppIdArray}");

                        LobbyServer.Start();
                        LoggerAccessor.LogInfo("Medius Lobby Server Initialized and Now Accepting Clients");
                    }
                    #endregion

                    #region MPS Enabled?
                    if (Settings.EnableMPS == true)
                    {
                        LoggerAccessor.LogInfo($"MPS Version: {Settings.MPSVersion}");
                        LoggerAccessor.LogInfo($"Enabling MPS on Server IP = {SERVER_IP} TCP Port = {ProxyServer.TCPPort}.");
                        //LoggerAccessor.LogInfo($"Medius Proxy Server running under ApplicationID {AppIdArray}");

                        ProxyServer.Start();
                        LoggerAccessor.LogInfo("Medius Proxy Server Initialized and Now Accepting Clients");

                        //Connecting to Medius Universe Manager 127.0.0.1 10076 1
                        //Connected to Universe Manager server after 1 attempts
                    }
                    #endregion
                }
                else
                {

                    #region MAPS - Zipper Interactive MAG/Socom 4
                    if (Settings.EnableMAPS == true)
                    {
                        LoggerAccessor.LogInfo($"Enabling MAPS on Server IP = {SERVER_IP} TCP Port = {ProfileServer.TCPPort} UDP Port = {ProfileServer.UDPPort}.");
                        //LoggerAccessor.LogInfo($"Medius Profile Server running under ApplicationID {AppIdArray}");

                        ProfileServer.Start();
                        LoggerAccessor.LogInfo("Medius Profile Server Intialized and Now Accepting Clients");
                    }
                    #endregion

                    #region MMS Enabled?
                    if (Settings.EnableMMS == true)
                    {
                        #region MAS 
                        LoggerAccessor.LogInfo($"MMS Version: {Settings.MMSVersion}");
                        LoggerAccessor.LogInfo($"Enabling MMS on Server IP = {SERVER_IP} TCP Port = {MatchmakingServer.TCPPort} UDP Port = {MatchmakingServer.UDPPort}.");
                        //LoggerAccessor.LogInfo($"Medius Matchmaking Server running under ApplicationID {AppIdArray}");

                        //Connecting to Medius Universe Manager 127.0.0.1 10076 1
                        //Connected to Universe Manager server

                        MatchmakingServer.Start();
                        LoggerAccessor.LogInfo("Medius Matchmaking Server Initialized");
                        #endregion

                    }
                    #endregion

                    #region MAS
                    if (Settings.EnableMAS == true)
                    {
                        LoggerAccessor.LogInfo($"MAS Version: {Settings.MASVersion}");
                        LoggerAccessor.LogInfo($"Enabling MAS on Server IP = {SERVER_IP} TCP Port = {AuthenticationServer.TCPPort} UDP Port = {AuthenticationServer.UDPPort}.");
                        //LoggerAccessor.LogInfo($"Medius Authentication Server running under ApplicationID {AppIdArray}");

                        //Connecting to Medius Universe Manager 127.0.0.1 10076 1
                        //Connected to Universe Manager server

                        AuthenticationServer.Start();
                        LoggerAccessor.LogInfo("Medius Authentication Server Initialized");

                    }
                    #endregion

                    #region MLS Enabled?
                    if (Settings.EnableMAS == true)
                    {
                        LoggerAccessor.LogInfo($"MLS Version: {Settings.MLSVersion}");
                        LoggerAccessor.LogInfo($"Enabling MLS on Server IP = {SERVER_IP} TCP Port = {LobbyServer.TCPPort} UDP Port = {LobbyServer.UDPPort}.");
                        //LoggerAccessor.LogInfo($"Medius Lobby Server running under ApplicationID {AppIdArray}");

                        //DMEServerResetMetrics();

                        LobbyServer.Start();
                        LoggerAccessor.LogInfo("Medius Lobby Server Initialized and Now Accepting Clients");
                    }
                    #endregion

                    #region MPS Enabled?
                    if (Settings.EnableMPS == true)
                    {
                        LoggerAccessor.LogInfo($"MPS Version: {Settings.MPSVersion}");
                        LoggerAccessor.LogInfo($"Enabling MPS on Server IP = {SERVER_IP} TCP Port = {ProxyServer.TCPPort}.");
                        //LoggerAccessor.LogInfo($"Medius Proxy Server running under ApplicationID {AppIdArray}");

                        ProxyServer.Start();
                        LoggerAccessor.LogInfo("Medius Proxy Server Initialized and Now Accepting Clients");

                        //Connecting to Medius Universe Manager 127.0.0.1 10076 1
                        //Connected to Universe Manager server after 1 attempts
                    }
                    #endregion

                    // Use hardcoded methods in code to handle specific games server versions
                    LoggerAccessor.LogInfo("Using Game Specific Server Versions");
                }

                #region NAT
                //Get NATIp
                if (string.IsNullOrEmpty(Settings.NATIp))
                    LoggerAccessor.LogError("[MEDIUS] - No NAT ip found! Fallback to Sony's one.");
                #endregion

                //* Diagnostic Profiling Enabled: %d Counts

                //Test:NGS Environment flag: %d

                //Billing Service Provider

                //Server-Side Vulgarity Filter Switch 
                //Valid Characters= %s
                //Dictionary Hard[%s] SoftNo[%s] SoftYes[%s] Substring[%s] Substring[%s]

                //ERROR: Could not reset DME Svr metrics[%d]?
                //TOMUM -  SEND PERCENTAGE[%d] RECV PERCENTAGE [%d]
                //DME SVR -  SEND BYTES[%ld] RECV BYTES[%ld]
                //SYS -  MAX SYS[%f]
                //Error initializing MediusTimer.  Continuing...

                //MediusParseLadderList0AppIDs
                //BinaryParseInitialize

                /*
                string rt_msg_client_get_version_string = "rt_msg_client version: 1.08.0206";
                LoggerAccessor.LogInfo($"Initialized Message Client Library {rt_msg_client_get_version_string}");
                */

                //DMEServer Enabling Dynamic Client Memory
                //rt_msg_server_enable_dynamic_client_memory
                //DmeServerEnableDynamicClientMemory failed. Continuing

                //Messaging Version = %d.%02d.%04d
                //%s library version = %d.%02d.%04d
                //Medius Lobby Server Intialized and Now Accepting Clients

                //Unable to connect to Cache Server. Error %d
                //Connected to Cache Server

                //MediusConnectLobbyToMUCG
                //Connecting to MUCG %s %d %d
                //MUCGIP
                //MUCGPort
                //WorldID
                //MUMIP
                //MUMPort
                //ForwardChatMsgFromMUCG
                //ForwardClanChatMsgFromMUCG
                //CurrentOnlineCount return value
                // Recovery Callback
                //EventsRequested
                //MUCGProcessSyncCB
                //MediusMUCGEventCB

                //Error connecting to MUCG

                //MUCGSendSync returned error %d

                //MFS_ProcessDownloadRequests
                //Error processing download queue. Error %d
                //MFS_ProcessUploadRequests
                //Error processing upload queue. Error %d

                //socket:Lost connection to Cache Server. Reconnecting[%d]
                //Unable to connect to Cache Server. Error %d
                //"Connected to Cache Server

                //ERR: LOST CONNECTION WITH MUM -- Cleaning up DME Worlds.  ATTEMPTING TO RE-ESTABLISH!!

                //MediusConnectLobbyToMUM

                //MUCGGetConnectState
                //Lost Connection to MUCG. Will attempt reconnect in %d seconds

                //ForceConfigReload
                //
                //ConfigManager Cannot Reload Configuration File %d

                //Reloading Dictionary Files
                //clSoftNo
                //clSoftYes
                //clHard
                //FPATExists
                //load_fpat
                //read_file_type
                //read_fpat
                //Incorrect file type in %s.\n
                //Unable to open %s.\n
                //fpat
                //loadclassifier

                //DmeServerUpdateAllWorlds
                //update:Error Code %d Updating All Worlds
                //update:DME Server Network Error = %d

                //AOSCacheFlushCheck
                //Error during AOS cache flush

                //BillingProviderProcessResultQueue
                //BillingProviderProcessResultQueue error.

                //RunningStatus
                //Shutting down NotificationScheduler.
                //Error shutting down MFS download queue
                //Error shutting down MFS upload queue

                //DMEServerCleanup or DMEServerCleanupWorld
                //update:DmeServerCleanupWorld error - world %d error =%d
                //update:DmeServerCleanup error =%d

                //Destroy Billing
                //BillingProviderDestroy
                //update:Ending Medius Lobby Server Operations

                //CacheDestroy
                //clSoftNo
                //clSoftYes
                //clHard
                //fpat

                //pendingTransClose
                //destroyMediusHttpd
                //MFS_transferDestroy
                //ClanCache_Destroy
                //Deleting ClanCache Error: %d

                #endregion

                LoggerAccessor.LogInfo("Medius Initialized.");

                started = true;

                _ = Task.Run(LoopServer);
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Medius] - Failed to start with exception : {ex}");
            }
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
            #region Check Config.json
            // Create Defaults if File doesn't exist
            if (!File.Exists(CONFIG_FILE))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(CONFIG_FILE) ?? Directory.GetCurrentDirectory() + "/static");
                File.WriteAllText(CONFIG_FILE ?? Directory.GetCurrentDirectory() + "/static/medius.json", JsonConvert.SerializeObject(Settings, Formatting.Indented));
            }
            else
                // Populate existing object
                JsonConvert.PopulateObject(File.ReadAllText(CONFIG_FILE), Settings, new JsonSerializerSettings()
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                });
            #endregion

            #region Check ebootdefs.json
            if (!string.IsNullOrEmpty(HorizonServerConfiguration.EBOOTDEFSConfig) && File.Exists(HorizonServerConfiguration.EBOOTDEFSConfig))
                LoadHomeOffsetsJson(File.ReadAllText(HorizonServerConfiguration.EBOOTDEFSConfig));
            #endregion

            // Determine server ip
            RefreshServerIp();

            if (string.IsNullOrEmpty(Settings.NATIp)) // Update NAT Ip with server ip if null
                Settings.NATIp = "natservice.pdonline.scea.com";

            // Update default rsa key
            LIBRARY.Pipeline.Attribute.ScertClientAttribute.DefaultRsaAuthKey = Settings.DefaultKey;

            if (Settings.DefaultKey != null)
                GlobalAuthPublic = new RSA_KEY(Settings.DefaultKey.N.ToByteArrayUnsigned().Reverse().ToArray());

            if (Settings.MUMServersAccessList.Count > 0)
            {
                foreach (KeyValuePair<string, string> kvp in Settings.MUMServersAccessList)
                {
                    string IPValue = kvp.Key;

                    if (IPAddress.TryParse(IPValue, out _))
                    {
                        if (MUMLocalServersAccessList.ContainsKey(IPValue))
                            MUMLocalServersAccessList[IPValue] = kvp.Value;
                        else
                            MUMLocalServersAccessList.Add(IPValue, kvp.Value);
                    }
                }
            }
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

                // get locations
                LocationDTO[]? locations = await HorizonServerConfiguration.Database.GetLocations();
                ChannelDTO[]? channels = await HorizonServerConfiguration.Database.GetChannels();

                // add new channels
                if (channels != null)
                {
                    foreach (ChannelDTO channel in channels)
                    {
                        if (Manager.GetChannelByChannelId(channel.Id, channel.AppId) == null)
                            await Manager.AddChannel(new Channel(channel.Id, channel.AppId, channel.MediusVersion, channel.Name ?? "MediusLobby", string.Empty,
                                    channel.MaxPlayers, channel.GenericField1, channel.GenericField2, channel.GenericField3, channel.GenericField4,
                                    (MediusWorldGenericFieldLevelType)channel.GenericFieldFilter, ChannelType.Lobby));
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }
        }

        private static void RefreshServerIp()
        {
            #region Determine Server IP
            if (!Settings.UsePublicIp)
                SERVER_IP = IPAddress.Parse(Settings.MEDIUSIp);
            else
            {
                if (string.IsNullOrWhiteSpace(Settings.PublicIpOverride))
                    SERVER_IP = IPAddress.Parse(InternetProtocolUtils.GetPublicIPAddress());
                else
                    SERVER_IP = IPAddress.Parse(Settings.PublicIpOverride);
            }
            #endregion
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

        #region Text Filter
        private static string GetTextFilterRegexExpression(int appId, TextFilterContext context)
        {
            var appSettings = GetAppSettingsOrDefault(appId);
            string? regex = null;

            switch (context)
            {
                case TextFilterContext.ACCOUNT_NAME: regex = appSettings.TextFilterAccountName; break;
                case TextFilterContext.CHAT: regex = appSettings.TextFilterChat; break;
                case TextFilterContext.CLAN_MESSAGE: regex = appSettings.TextFilterClanMessage; break;
                case TextFilterContext.CLAN_NAME: regex = appSettings.TextFilterClanName; break;
                case TextFilterContext.DEFAULT: regex = appSettings.TextFilterDefault; break;
                case TextFilterContext.GAME_NAME: regex = appSettings.TextFilterGameName; break;
            }

            if (string.IsNullOrEmpty(regex))
                return appSettings.TextFilterDefault;

            return regex;
        }

        public static bool PassTextFilter(int appId, TextFilterContext context, string text)
        {
            var rExp = GetTextFilterRegexExpression(appId, context);
            if (string.IsNullOrEmpty(rExp))
                return true;

            return !new Regex(rExp, RegexOptions.IgnoreCase | RegexOptions.Multiline).IsMatch(text);
        }

        public static string FilterTextFilter(int appId, TextFilterContext context, string text)
        {
            string rExp = GetTextFilterRegexExpression(appId, context);
            if (string.IsNullOrEmpty(rExp))
                return text;

            return new Regex(rExp, RegexOptions.IgnoreCase | RegexOptions.Multiline).Replace(text, string.Empty);
        }
        #endregion

        public static string? GetFileSystemPath(int appId, string filename)
        {
            if (!GetAppSettingsOrDefault(appId).EnableMediusFileServices)
                return null;
            if (string.IsNullOrEmpty(Settings.MediusFileServerRootPath))
                return null;
            if (string.IsNullOrEmpty(filename))
                return null;

            string rootPath = Path.GetFullPath(Settings.MediusFileServerRootPath);
            string path = Path.GetFullPath(Path.Combine(Settings.MediusFileServerRootPath, appId.ToString(), filename));

            // prevent filename from moving up directories
            if (!path.StartsWith(rootPath))
                return null;

            return path;
        }

        /// <summary>
        /// Gets File Path with AppId included in the path
        /// </summary>
        /// <param name="appId">AppId passed in</param>
        /// <returns></returns>
        public static string? GetFileAppIdPath(int appId)
        {
            if (!GetAppSettingsOrDefault(appId).EnableMediusFileServices)
                return null;
            if (string.IsNullOrEmpty(Settings.MediusFileServerRootPath))
                return null;

            var rootPath = Path.GetFullPath(Settings.MediusFileServerRootPath);
            var path = Path.Combine(rootPath, appId.ToString());

            if (!Directory.Exists(path))
            {
                LoggerAccessor.LogWarn($"Path being created! {path} for appId {appId}");
                Directory.CreateDirectory(path);
            }

            // prevent filename from moving up directories
            if (!path.StartsWith(rootPath))
                return null;

            return path;
        }

        public static AppSettings GetAppSettingsOrDefault(int appId)
        {
            if (_appSettings.TryGetValue(appId, out var appSettings))
                return appSettings;

            return _defaultAppSettings;
        }

        public static List<MediusGetPolicyResponse> GetPolicyFromText(MessageId messageId, string policy)
        {
            List<MediusGetPolicyResponse> policies = new();
            int i = 0;

            while (i < policy.Length)
            {
                // Determine length of string
                int len = policy.Length - i;
                if (len > Constants.POLICY_MAXLEN)
                    len = Constants.POLICY_MAXLEN;

                // Add policy subtext
                policies.Add(new MediusGetPolicyResponse()
                {
                    MessageID = messageId,
                    StatusCode = MediusCallbackStatus.MediusSuccess,
                    Policy = policy.Substring(i, len)
                });

                // Increment i
                i += len;
                LoggerAccessor.LogDebug($"Sending Policy Chunk {i} of {len} Len {policy.Length} bytes");
            }

            // Set end of text
            if (policies.Count > 0)
                policies[policies.Count - 1].EndOfText = true;

            return policies;
        }

        public static void MFS_transferInit()
        {
            LoggerAccessor.LogInfo($"Initializing MFS_transfer with url {Settings.MFSTransferURI} "); //numThreads{}"
            return;
        }

        public static MPS GetMPS()
        {
            MPS mps = ProxyServer;

            return mps;
        }
    }
}
