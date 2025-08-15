using Org.BouncyCastle.Math;
using Horizon.RT.Cryptography.RSA;
using MultiServerLibrary.Extension;


namespace Horizon.SERVER.Config
{
    public class ServerSettings
    {
        /// <summary>
        /// How many milliseconds before refreshing the config.
        /// </summary>
        public int RefreshConfigInterval = 5000;

        #region MEDIUS SCE-RT Service Location
        /// <summary>
        /// IP of the MEDIUS.
        /// </summary>
        public string MEDIUSIp { get; set; } = InternetProtocolUtils.TryGetServerIP(out _).Result ? InternetProtocolUtils.GetPublicIPAddress() : InternetProtocolUtils.GetLocalIPAddresses().First().ToString();
        #endregion

        #region PublicIp
        /// <summary>
        /// By default the server will grab its local ip.
        /// If this is set, it will use its public ip instead.
        /// </summary>
        public bool UsePublicIp { get; set; } = InternetProtocolUtils.TryGetServerIP(out _).Result;

        /// <summary>
        /// If UsePublicIp is set to true, allow overriding and skipping using dyndns's dynamic
        /// ip address finder, since it goes down often enough to throw exceptions
        /// </summary>
        public string PublicIpOverride { get; set; } = string.Empty;
        #endregion

        /// <summary>
        /// LocationID of this Medius Stack, applies to MAS, and MLS
        /// </summary>
        public int LocationID = 0;

        #region NpMLSOverride
        /// <summary>
        /// If NpMLSIpOverride is set, server will authenticate on the designed MLS server that is validated for NpTickets.
        /// </summary>
        public string NpMLSIpOverride { get; set; } = string.Empty;

        /// <summary>
        /// If NpMLSIpOverride is set, server will authenticate on the designed MLS server that is validated for NpTickets.
        /// </summary>
        public int NpMLSPortOverride { get; set; } = -1;
        #endregion

        #region Database
        //#####################################
        //# Database Connection Configuration #
        //#####################################

        /// <summary>
        /// Set to 1 to enable database connectivity, or 0 for simulated-DB mode (default 1)
        /// </summary>
        public int ConfigDBEnabled = 0;

        public string DBInfoFileName = "dbinfo.txt";
        #endregion

        #region Enable Select Servers

        /// <summary>
        /// Enable MAPS Zipper Interactive Only
        /// </summary>
        public bool EnableMAPS { get; set; } = true;

        /// <summary>
        /// Enable MMS (Medius Matchmaking Server) PS3
        /// </summary>
        public bool EnableMMS { get; set; } = true;

        /// <summary>
        /// Enable MAS
        /// </summary>
        public bool EnableMAS { get; set; } = true;

        /// <summary>
        /// Enable MLS
        /// </summary>
        public bool EnableMLS { get; set; } = true;

        /// <summary>
        /// Enable MPS
        /// </summary>
        public bool EnableMPS { get; set; } = true;
        #endregion

        #region Ports
        /// <summary>
        /// TCP Port of the MAPS server.
        /// </summary>
        public int MAPSTCPPort { get; set; } = 10471;

        /// <summary>
        /// UDP Port of the MAPS server.
        /// </summary>
        public int MAPSUDPPort { get; set; } = 10472;

        public int MMSTCPPort { get; set; } = 10079;

        #region Standard 
        /// <summary>
        /// Port of the MAS server.
        /// </summary>
        public int[] MASPorts { get; set; } = new int[] { 10075, 10475 };
        /// <summary>
        /// Port of the MLS server.
        /// </summary>
        public int MLSPort { get; set; } = 10078;

        /// <summary>
        /// Port of the MPS server.
        /// </summary>
        public int MPSPort { get; set; } = 10077;
        #endregion

        #endregion

        #region Medius Versions
        public bool MediusServerVersionOverride { get; set; } = true;

        public string MMSVersion { get; set; } = "Medius Matchmaking Server Version 3.03.0000";

        public string MASVersion { get; set; } = "Medius Authentication Server Version 3.03.0000";

        public string MLSVersion { get; set; } = "Medius Lobby Server Version 3.03.0000";

        public string MPSVersion { get; set; } = "Medius Proxy Server Version 3.03.0000";

        public string MAPSVersion { get; set; } = "Medius Authorative Profile Server Version 3.03.0000";
        #endregion

        #region Medius Universe Manager Location
        /// <summary>
        /// Provide the IP, Port and WorldID of the MUM that will control this MLS
        /// (no valid defaults)
        /// </summary>
        public string MUMIp { get; set; } = InternetProtocolUtils.GetLocalIPAddresses().First().ToString();
        public int MUMPort { get; set; } = 10076;
        public int MUMWorldID { get; set; } = 1;
        #endregion

        #region NAT SCE-RT Service Location
        /// <summary>
        /// Ip address of the NAT server.
        /// Provide the IP of the SCE-RT NAT Service
        /// Default is: natservice.pdonline.scea.com:10070
        /// </summary>
        public string? NATIp { get; set; } = InternetProtocolUtils.TryGetServerIP(out _).Result ? InternetProtocolUtils.GetPublicIPAddress() : InternetProtocolUtils.GetLocalIPAddresses().First().ToString();

        /// <summary>
        /// Port of the NAT server.
        /// Provide the Port of the SCE-RT NAT Service
        /// </summary>
        public int NATPort { get; set; } = 10070;
        #endregion

        #region Remote Log Viewer Port To Listen
        /// <summary>
        /// Any value greater than 0 will enable remote logging with the SCE-RT logviewer
        /// on that port, which must not be in use by other applications (default 0)
        /// </summary>
        public int RemoteLogViewPort = 0;
        #endregion

        #region System Message Test
        /// <summary>
        /// System Message Test
        /// This setting controls whether a single system message is sent
        /// to a user who starts a session. This tests handling of "You have
        /// been banned from the system!" type messages pushed from the server.
        /// 1 = Turned on
        /// 0 = Turned off
        /// </summary>
        public bool SystemMessageSingleTest { get; set; } = false;
        #endregion

        /// <summary>
        /// # Anonymous account ID seed, for games that use anonymous login
        /// # such as ATV2.
        /// # Set each authentication server to a different value,
        /// # preferably between 0 and 127
        /// # A possible scheme would be to have each MAS## use the ## as
        /// # the value.  For example, MAS01 could use the value 1, etc....
        /// </summary>

        public int AnonymousIDRangeSeed = 1;

        #region DNAS
        /// <summary>
        /// Enable posting of machine signature to database (1 = enable, 0 = disable)
        /// </summary>
        public bool DnasEnablePost { get; set; } = true;
        #endregion

        #region Medius File Services - File Server Configuration

        /// <summary>
        /// Root path of the medius file service directory.
        /// </summary>
        public string MediusFileServerRootPath { get; set; } = "static/Files";

        /// <summary>
        /// Set the hostname to the ApacheWebServerHostname
        /// </summary>
        public string MFSTransferURI { get; set; } = $"http://{InternetProtocolUtils.GetLocalIPAddresses().First()}/";

        /// <summary>
        /// Max number of download requests in the download queue
        /// </summary>
        public int MFSDownloadQSize = 8192;

        /// <summary>
        /// Max number of upload requests in the download queue
        /// </summary>
        public int MFSUploadQSize = 8192;

        /// <summary>
        /// Time out interval for activity on upload/download
        /// requests, in seconds. Once timeout interval is
        /// exceeded, the request will be removed from queue.
        /// The reference time stamp gets updated when
        /// activity occurs on the request in queue
        /// </summary>
        public int MFSQueueTimeoutInterval = 360;
        #endregion

        #region Chat Channel Audio Headset Support
        /// <summary>
        /// Enable/Disable peer-to-peer Chat Channel Audio Headset Support (0 default)
        /// </summary>
        public bool EnableBroadcastChannel = true;
        #endregion

        #region PostDebugInfo
        /// <summary>
        /// Enable posting of debug information from the client <br></br>
        /// Set to false to disable, set to true to enable.            
        /// </summary>
        public bool PostDebugInfoEnable = true;
        #endregion

        #region Anti-Cheat (and related info)

        /// <summary>
        /// Set this to 1 to activate AntiCheat.  Or to 0 (the default) to
        /// deactivate it.
        /// </summary>
        public bool AntiCheatOn = true;

        #endregion

        #region MUCG Connection Attributes
        // (no valid defaults)
        // Uncomment MUCG params to enable connectivity to MUCG

        public string MUCGServerIP = InternetProtocolUtils.GetLocalIPAddresses().First().ToString();
        public int MUCGServerPort = 10072;
        public int MUCGWorldID = 1;
        #endregion

        #region Billing configuration
        /// <summary>        /// Billing functionality.
        /// Biling enabled settings: 0 = disabled, 1=SCEK, 2=SCEA, 3=SCEJ, 4=SCEE
        /// 
        ///  The following settings enable and configure billing. Setting BillingEnabled
        ///  to non-zero will enable billing support within the MAS service. 
        ///  # to non-zero will enable billing support within the MAS service. 
        /// If the Billing enabled is 1, then the supported module is for SCEK
        /// If the Billing enabled is 2, then the supported module is for SCEA
        /// If the Billing enabled is 3, then the supported module is for SCEJ
        /// If the Billing enabled is 4, then the supported module is for SCEE
        /// Note, the remaining fields have no meaning if billing is disabled. 
        /// The BillingServiceProvider identifies the billing hosting service and
        /// determines which billing plugin will be loaded. BillingProviderIpAddr and 
        /// BillingProviderPort configures access to the billing provider services.
        /// BillingSecurityLevel (0 | 1) determines whether or not encryption is used.
        /// BillingEntryPointReference should be set to the DNS value of the MAS service.
        /// </summary>
        public int BillingEnabled = 0;

        // Provider designation and plugin
        // Biling service provider settings: SCEK, SCERT
        public string BillingServiceProvider = "SCERT";

        public string BillingPluginPath = "./libSCERTBilling_unix.so";

        // IP Address and port of the billing service provider.
        // This is the SCE-RT Product Service if the billing provider is SCE-RT
        // Or the SCEK connection if the provider is SCEK
        public string BillingProviderIpAddr = InternetProtocolUtils.GetLocalIPAddresses().First().ToString();
        public int BillingServiceProviderPort = 2222;

        // Billing security settings
        public int BillingSecurityLevel = 0;
        public string BillingKeyPath = "./SCERTKey.txt";

        public string BillingEntryPointRef = "title.pdonline.scea.com";

        // Billing memory allocation scheme.
        public int BillingBucketCount = 5;
        public int BillingPreallocCount = 40;
        #endregion

        #region WMErrorHandling
        // Change API callback return status from WMError to something more useful in certain cases.
        // MediusJoinGame will return MediusGameNotFound if the game no longer exists
        // MediusJoinGame will return MediusWorldIsFull if the game has no open slots as defined by Medius.
        // MediusJoinChannel will return MediusChannelNotFound if the channel no longer exists.
        // MediusJoinChannel will return MediusWorldIsFull if the channel is full.
        // MediusGetGameInfo will return MediusGameNotFound if you pass in a game that either just got destroyed or doesn't exit.
        //MediusGetGamePlayers will return MediusGameNotFound if you pass in a game that doesn't exist.
        public bool WMErrorHandling = true;
        #endregion

        #region Clan 
        // Special configuration to allow for a non-clan leader to retrieve clan team challenges.
        // Set this to 1 to enable this override.
        // Or to 0 (the default) to maintain strict clan leader control.
        public bool EnableNonClanLeaderToGetTeamChallenges = false;

        /// <summary>
        ///  Clan Ladders 
        /// If enabled, allows for any member of a clan to post clan ladder scores via <br></br>
        /// the API MediusUpdateClanLadderStatsWide_Delta()
        /// </summary>
        public bool EnableClanLaddersDeltaOpenAccess = false;
        #endregion

        #region Syphon Filter - The Omega Strain

        public int SFOOverrideClanLobbyMaxPlayers = 64;
        public int SFOOverrideLobbyPlayerCountThreshold = 0;

        #endregion

        #region Keys
        /// <summary>
        /// Key used to authenticate dme servers.
        /// </summary>
        public RsaKeyPair MPSKey { get; set; } = new(
            new BigInteger("10315955513017997681600210131013411322695824559688299373570246338038100843097466504032586443986679280716603540690692615875074465586629501752500179100369237", 10),
            new BigInteger("17", 10),
            new BigInteger("4854567300243763614870687120476899445974505675147434999327174747312047455575182761195687859800492317495944895566174677168271650454805328075020357360662513", 10)
            );

        /// <summary>
        /// Key used to authenticate clients.
        /// </summary>
        public RsaKeyPair DefaultKey { get; set; } = new(
            new BigInteger("10315955513017997681600210131013411322695824559688299373570246338038100843097466504032586443986679280716603540690692615875074465586629501752500179100369237", 10),
            new BigInteger("17", 10),
            new BigInteger("4854567300243763614870687120476899445974505675147434999327174747312047455575182761195687859800492317495944895566174677168271650454805328075020357360662513", 10)
            );
        #endregion

        #region VULARITY FILTER
        /// <summary>
        /// Regex text filters for 
        /// </summary>
        public Dictionary<TextFilterContext, string> TextBlacklistFilters { get; set; } = new();

        /// <summary>
        /// # Specify name/location of SCE-RT dictionary files
        /// # (no valid defaults)
        /// </summary>
        public string MediusVulgarityRootPath { get; set; } = "VulgarityFiles";

        public string HardDictionaryName = "d1.cl";
        public string SoftNoDictionaryName = "d2.cl";
        public string SoftYesDictionaryName = "d3.cl";

        /// # Substring dictionary
        public string VulgaritySubstringDictionary = "vulgar.fpat";

        #endregion

        #region MUM
        /// <summary>
        /// List of MUM Ips/Ports.
        /// </summary>
        public Dictionary<string, string> MUMServersAccessList { get; set; } = new();
        #endregion

        #region MLS
        /// <summary>
        /// Allows the login of guests, will be needed if you plan on using multi-MLS setup.
        /// </summary>
        public bool AllowGuests { get; set; } = true;
        #endregion

        /// <summary>
        /// Tries to patch HTTPS ticketlogin check inside Medius client SDK.
        /// </summary>
        public bool HttpsSVOCheckPatcher { get; set; } = false;

        /// <summary>
        /// Enables Memory Poking.
        /// </summary>
        public bool PokePatchOn { get; set; } = false;

        #region PSHOME Internal Plugin

        /// <summary>
        /// Enables the closed beta auto-create plugin.
        /// </summary>
        public bool PlaystationHomeClosedBetaAutoCreatePlugin { get; set; } = false;

        /// <summary>
        /// Needed for the closed beta auto-create plugin.
        /// </summary>
        public string PlaystationHomeClosedBetaSceneListPath { get; set; } = Program.configDir + "SCENELIST.XML";

        /// <summary>
        /// Enables the use of non-validated home eboots.
        /// </summary>
        public bool PlaystationHomeAllowAnyEboot { get; set; } = true;

        /// <summary>
        /// Enables home anti-cheat checks.
        /// </summary>
        public bool PlaystationHomeAntiCheat { get; set; } = false;

        /// <summary>
        /// Enables home ForceInvite mitigation fixes.
        /// </summary>
        public bool PlaystationHomeForceInviteExploitPatch { get; set; } = false;

        public Dictionary<string, string> PlaystationHomeUsersServersAccessList { get; set; } = new();

        #endregion
    }

    #region TextFilterContext
    public enum TextFilterContext
    {
        DEFAULT,
        ACCOUNT_NAME,
        CLAN_NAME,
        CLAN_MESSAGE,
        CHAT,
        GAME_NAME
    }
    #endregion
}
