using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Runtime;
using System.Security.Cryptography;
using CastleLibrary.Utils;
using CustomLogger;
using Horizon.BWPS;
using Horizon.DATABASE;
using Horizon.DME;
using Horizon.HTTPSERVICE;
using Horizon.LIBRARY.Database;
using Horizon.MEDIUS;
using Horizon.MEDIUS.Models.MAPS;
using Horizon.MEDIUS.Processors;
using Horizon.MUIS;
using Horizon.MUIS.Models;
using Horizon.MUM;
using Horizon.NAT;
using Horizon.PlaystationHomePlugin.Models;
using Microsoft.Extensions.Logging;
using MultiServerLibrary;
using MultiServerLibrary.Extension;
using MultiServerLibrary.Extension.NET;
using MultiServerLibrary.GeoLocalization;
using MultiServerLibrary.SNMP;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prometheus;
using Tommunism.SoftFloat;

public static class HorizonServerConfiguration
{
    public static string MediusPluginsFolder { get; set; } =
        $"{Directory.GetCurrentDirectory()}/static/medius_plugins";
    public static string DmePluginsFolder { get; set; } =
        $"{Directory.GetCurrentDirectory()}/static/dme_plugins";
    public static string DatabaseConfig { get; set; } =
        $"{Directory.GetCurrentDirectory()}/static/db.config2.json";
    public static string HTTPSCertificateFile { get; set; } =
        $"{Directory.GetCurrentDirectory()}/static/SSL/HorizonHTTPService.pfx";
    public static string HTTPSCertificatePassword { get; set; } = "qwerty";
    public static HashAlgorithmName HTTPSCertificateHashingAlgorithm { get; set; } =
        HashAlgorithmName.SHA384;
    public static bool EnableMetrics { get; set; } = true;
    public static ushort MetricsPort { get; set; } = 1234;
    public static string MetricsUrl { get; set; } = "metrics/";
    public static string? PlayerAPIStaticPath { get; set; } =
        $"{Directory.GetCurrentDirectory()}/static/wwwroot";
    public static string? EBOOTDEFSConfig { get; set; } =
        $"{Directory.GetCurrentDirectory()}/static/ebootdefs.json";
    public static string? MEDIUSConfig { get; set; } =
        $"{Directory.GetCurrentDirectory()}/static/medius.json";
    public static string SSFWUrl { get; set; } =
        $"http://{(InternetProtocolUtils.TryGetServerIP(out _).Result ? InternetProtocolUtils.GetPublicIPAddress() : InternetProtocolUtils.GetOutboundIPAddresses().First().ToString())}:8080";
    public static string[]? HTTPSDNSList { get; set; }
    #region NAT Settings
    public static bool EnableNAT { get; set; } = true;
    public static ushort NATPort { get; set; } = 10070;
    #endregion
    #region BWPS Settings
    public static bool EnableBWPS { get; set; } = true;
    public static ushort BWPSPort { get; set; } = 50100;
    #endregion
    #region MUIS Settings
    public static bool EnableMuis { get; set; } = true;
    public static ushort[] MUISPorts { get; set; } = new ushort[] { 10071, 10080, 10101 };
    public static bool MUISEncryptMessages { get; set; } = true;
    public static int[] MUISCompatibleApplicationIds { get; set; } =
        new int[]
        {
            11204,
            11354,
            21914,
            21624,
            20764,
            20371,
            20384,
            22500,
            10540,
            10550,
            10582,
            10584,
            22920,
            22923,
            22924,
            21731,
            21834,
            23624,
            20032,
            20034,
            20454,
            20314,
            21874,
            21244,
            20304,
            20463,
            21614,
            20344,
            20434,
            22204,
            23360,
            21513,
            21064,
            20804,
            20374,
            21094,
            20060,
            10984,
            10782,
            10421,
            10130,
            10954,
            21784,
            21564,
            21354,
            21564,
            21574,
            21584,
            21594,
            22274,
            22284,
            22294,
            22304,
            23014,
            20040,
            20041,
            20042,
            20043,
            20044,
        };
    public static string MUISVersion { get; set; } =
        "Medius Universe Information Server Version 3.05.0000";
    public static Dictionary<int, UniverseInfo[]> MUISUniverses { get; set; } = new();
    #endregion
    #region DME Settings
    public static bool EnableDME { get; set; } = true;

    public static int[] DMECompatibleApplicationIds { get; set; } =
        new int[]
        {
            10680,
            10683,
            10684,
            11354,
            21914,
            21624,
            20764,
            20371,
            22500,
            10540,
            22920,
            21731,
            21834,
            23624,
            20043,
            20032,
            20034,
            20454,
            20314,
            21874,
            21244,
            20304,
            20463,
            21614,
            20344,
            20434,
            22204,
            23360,
            21513,
            21064,
            20804,
            20374,
            21094,
            22274,
            20060,
            10984,
            10782,
            10421,
            10130,
            24000,
            24180,
            10954,
            21784,
        };

    public static int DMEClientReconnectInterval { get; set; } = 15;

    public static int DMEPluginTickIntervalMs { get; set; } = 50;

    public static ushort DMETCPPort { get; set; } = 10073;

    public static ushort DMEUDPPort { get; set; } = 50000;

    public static short DMEServerMaxWorld { get; set; } = short.MaxValue;

    public static int DMEMaxClientsPerWorlds { get; set; } = 256;

    public static int DMEMaxClientsOverride { get; set; } = -1; // Use at your own risk!

    public static string DMEMASIp { get; set; } =
        InternetProtocolUtils.GetOutboundIPAddresses().First().ToString();
    public static ushort DMEMASPort { get; set; } = 10075;
    public static string DMEMPSIp { get; set; } =
        InternetProtocolUtils.GetOutboundIPAddresses().First().ToString();
    public static ushort DMEMPSPort { get; set; } = 10077;

    #endregion
    #region MEDIUS Settings

    public static bool EnableMedius { get; set; } = true;

    public static string MEDIUSAPIKey { get; set; } =
        StringUtils.GenerateRandomBase64KeyAsync().Result;

    public static int MEDIUSPluginTickIntervalMs { get; set; } = 50;

    #region Enable Select Servers

    /// <summary>
    /// Enable MAPS Zipper Interactive Only
    /// </summary>
    public static bool MEDIUSEnableMAPS { get; set; } = true;

    /// <summary>
    /// Enable MMS (Medius Matchmaking Server) PS3
    /// </summary>
    public static bool MEDIUSEnableMMS { get; set; } = true;

    /// <summary>
    /// Enable MAS
    /// </summary>
    public static bool MEDIUSEnableMAS { get; set; } = true;

    /// <summary>
    /// Enable MLS
    /// </summary>
    public static bool MEDIUSEnableMLS { get; set; } = true;

    /// <summary>
    /// Enable MPS
    /// </summary>
    public static bool MEDIUSEnableMPS { get; set; } = true;
    #endregion

    #region Ports
    /// <summary>
    /// TCP Port of the MAPS server.
    /// </summary>
    public static ushort MEDIUSMAPSTCPPort { get; set; } = 10471;

    /// <summary>
    /// UDP Port of the MAPS server.
    /// </summary>
    public static ushort MEDIUSMAPSUDPPort { get; set; } = 10472;

    public static ushort MEDIUSMMSTCPPort { get; set; } = 10079;

    #region Standard
    /// <summary>
    /// Port of the MAS server.
    /// </summary>
    public static ushort[] MEDIUSMASPorts { get; set; } = new ushort[] { 10075, 10475 };

    /// <summary>
    /// Port of the MLS server.
    /// </summary>
    public static ushort MEDIUSMLSPort { get; set; } = 10078;

    /// <summary>
    /// Port of the MPS server.
    /// </summary>
    public static ushort MEDIUSMPSPort { get; set; } = 10077;
    #endregion

    #endregion

    #region Medius Versions
    public static string MEDIUSMMSVersion { get; set; } =
        "Medius Matchmaking Server Version 3.03.0000";

    public static string MEDIUSMASVersion { get; set; } =
        "Medius Authentication Server Version 3.03.0000";

    public static string MEDIUSMLSVersion { get; set; } = "Medius Lobby Server Version 3.03.0000";

    public static string MEDIUSMPSVersion { get; set; } = "Medius Proxy Server Version 3.03.0000";

    public static string MEDIUSMAPSVersion { get; set; } =
        "Medius Authorative Profile Server Version 3.03.0000";
    #endregion

    #region NAT SCE-RT Service Location
    /// <summary>
    /// Ip address of the NAT server.
    /// Provide the IP of the SCE-RT NAT Service
    /// Default is: natservice.pdonline.scea.com:10070
    /// </summary>
    public static string? MEDIUSNATIp { get; set; } =
        InternetProtocolUtils.TryGetServerIP(out var serverIP).Result ? serverIP : serverIP;

    /// <summary>
    /// Port of the NAT server.
    /// Provide the Port of the SCE-RT NAT Service
    /// </summary>
    public static int MEDIUSNATPort { get; set; } = 10070;
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
    public static bool MEDIUSSystemMessageSingleTest { get; set; } = false;
    #endregion

    /// <summary>
    /// # Anonymous account ID seed, for games that use anonymous login
    /// # such as ATV2.
    /// # Set each authentication server to a different value,
    /// # preferably between 0 and 127
    /// # A possible scheme would be to have each MAS## use the ## as
    /// # the value.  For example, MAS01 could use the value 1, etc....
    /// </summary>
    public static int MEDIUSAnonymousIDRangeSeed = 1;

    #region DNAS
    /// <summary>
    /// Enable posting of machine signature to database (1 = enable, 0 = disable)
    /// </summary>
    public static bool MEDIUSDnasEnablePost { get; set; } = true;
    #endregion

    #region Medius File Services - File Server Configuration

    /// <summary>
    /// Root path of the medius file service directory.
    /// </summary>
    public static string MEDIUSMFSRootPath { get; set; } = "static/wwwmfsroot";

    /// <summary>
    /// Set the hostname to the ApacheWebServerHostname
    /// </summary>
    public static string MEDIUSMFSTransferURI { get; set; } =
        $"http://{InternetProtocolUtils.GetOutboundIPAddresses().First()}/";

    /// <summary>
    /// Max number of download requests in the download queue
    /// </summary>
    public static int MEDIUSMFSDownloadQSize = 8192;

    /// <summary>
    /// Max number of upload requests in the download queue
    /// </summary>
    public static int MEDIUSMFSUploadQSize = 8192;

    /// <summary>
    /// Time out interval for activity on upload/download
    /// requests, in seconds. Once timeout interval is
    /// exceeded, the request will be removed from queue.
    /// The reference time stamp gets updated when
    /// activity occurs on the request in queue
    /// </summary>
    public static int MEDIUSMFSQueueTimeoutInterval = 360;
    #endregion

    #region Clan
    // Special configuration to allow for a non-clan leader to retrieve clan team challenges.
    // Set this to 1 to enable this override.
    // Or to 0 (the default) to maintain strict clan leader control.
    public static bool MEDIUSEnableNonClanLeaderToGetTeamChallenges = false;

    /// <summary>
    ///  Clan Ladders
    /// If enabled, allows for any member of a clan to post clan ladder scores via <br></br>
    /// the API MediusUpdateClanLadderStatsWide_Delta()
    /// </summary>
    public static bool MEDIUSEnableClanLaddersDeltaOpenAccess = false;
    #endregion

    #region Syphon Filter - The Omega Strain

    public static int MEDIUSSFOOverrideClanLobbyMaxPlayers = 64;
    public static int MEDIUSSFOOverrideLobbyPlayerCountThreshold = 0;

    #endregion

    #region MUM
    /// <summary>
    /// List of MUM Ips/Ports.
    /// </summary>
    public static Dictionary<string, string> MEDIUSMUMServersAccessList { get; set; } = new();
    #endregion

    #region MLS

    public static string MEDIUSNpMLSIpOverride { get; set; } = string.Empty;
    public static int MEDIUSNpMLSPortOverride { get; set; } = -1;

    /// <summary>
    /// Allows the login of guests, will be needed if you plan on using multi-MLS setup.
    /// </summary>
    public static bool MEDIUSAllowGuests { get; set; } = true;
    #endregion

    #region MAPS
    public static Dictionary<int, NetUniverseInfo[]> MAPSUniverses { get; set; } = new();

    #endregion

    /// <summary>
    /// Tries to patch HTTPS ticketlogin check inside Medius client SDK.
    /// </summary>
    public static bool MEDIUSHttpsSVOCheckPatcher { get; set; } = false;

    /// <summary>
    /// Enables Memory Poking.
    /// </summary>
    public static bool MEDIUSPokePatchOn { get; set; } = false;

    #region PSHOME Internal Plugin

    /// <summary>
    /// Enables the closed beta auto-create plugin.
    /// </summary>
    public static bool MEDIUSPlaystationHomeClosedBetaAutoCreatePlugin { get; set; } = false;

    /// <summary>
    /// Needed for the closed beta auto-create plugin.
    /// </summary>
    public static string MEDIUSPlaystationHomeClosedBetaSceneListPath { get; set; } =
        Program.configDir + "SCENELIST.XML";

    /// <summary>
    /// Enables the use of non-validated home eboots.
    /// </summary>
    public static bool MEDIUSPlaystationHomeAllowAnyEboot { get; set; } = true;

    /// <summary>
    /// Enables home anti-cheat checks.
    /// </summary>
    public static bool MEDIUSPlaystationHomeAntiCheat { get; set; } = false;

    /// <summary>
    /// Enables home ForceInvite mitigation fixes.
    /// </summary>
    public static bool MEDIUSPlaystationHomeForceInviteExploitPatch { get; set; } = false;

    public static Dictionary<
        string,
        string
    > MEDIUSPlaystationHomeUsersServersAccessList { get; set; } = new();

    #endregion

    #endregion

    public static List<HomeOffsetsJsonData> HomeOffsetsList = new();

    public static DbController Database;

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
            LoggerAccessor.LogWarn(
                $"Could not find the configuration file:{configPath}, writing and using server's default."
            );

            Directory.CreateDirectory(
                Path.GetDirectoryName(configPath) ?? Directory.GetCurrentDirectory() + "/static"
            );

            InternetProtocolUtils.TryGetServerIP(out var iptofile);

            #region MAPS Default Universes

            MAPSUniverses.Add(
                20230,
                [
                    new NetUniverseInfo()
                    {
                        Name = "Default Universe",
                        AuthDNS = "mag.mas.online.scea.com",
                        AuthIP = iptofile,
                        SvoURL = "http://mag.svo.online.scea.com:10060/MAG_SVML/index.jsp",
                        Port = 10075,
                        UniverseId = 1,
                    },
                ]
            );

            #endregion

            #region MUIS Default Universes

            // Add default localhost entry
            MUISUniverses.Add(
                0,
                new UniverseInfo[]
                {
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
                        UniverseId = 1,
                    },
                }
            );

            // Populate with other entries
            MUISUniverses.Add(
                10130,
                new UniverseInfo[]
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
                        BillingSystemName =
                            "Sony Computer Entertainment America, Inc. Billing System",
                    },
                }
            );

            MUISUniverses.Add(
                10550,
                new UniverseInfo[]
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
                        BillingSystemName =
                            "Sony Computer Entertainment America, Inc. Billing System",
                    },
                }
            );

            MUISUniverses.Add(
                10582,
                new UniverseInfo[]
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
                        BillingSystemName =
                            "Sony Computer Entertainment America, Inc. Billing System",
                    },
                }
            );

            MUISUniverses.Add(
                10584,
                new UniverseInfo[]
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
                        BillingSystemName =
                            "Sony Computer Entertainment America, Inc. Billing System",
                    },
                }
            );

            MUISUniverses.Add(
                10954,
                new UniverseInfo[]
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
                        BillingSystemName =
                            "Sony Computer Entertainment America, Inc. Billing System",
                    },
                }
            );

            MUISUniverses.Add(
                11204,
                new UniverseInfo[]
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
                        UniverseId = 1,
                    },
                }
            );

            MUISUniverses.Add(
                21784,
                new UniverseInfo[]
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
                        BillingSystemName =
                            "Sony Computer Entertainment America, Inc. Billing System",
                    },
                }
            );

            MUISUniverses.Add(
                21914,
                new UniverseInfo[]
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
                        BillingSystemName =
                            "Sony Computer Entertainment America, Inc. Billing System",
                    },
                }
            );

            MUISUniverses.Add(
                10540,
                new UniverseInfo[]
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
                        BillingSystemName =
                            "Sony Computer Entertainment America, Inc. Billing System",
                    },
                }
            );

            MUISUniverses.Add(
                10782,
                new UniverseInfo[]
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
                        UniverseId = 1,
                    },
                }
            );

            MUISUniverses.Add(
                10421,
                new UniverseInfo[]
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
                        UniverseId = 1,
                    },
                }
            );

            MUISUniverses.Add(
                10984,
                new UniverseInfo[]
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
                        UniverseId = 1,
                    },
                }
            );

            MUISUniverses.Add(
                11354,
                new UniverseInfo[]
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
                        UniverseId = 1,
                    },
                }
            );

            MUISUniverses.Add(
                20060,
                new UniverseInfo[]
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
                        UniverseId = 1,
                    },
                }
            );

            MUISUniverses.Add(
                21064,
                new UniverseInfo[]
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
                        UniverseId = 1,
                    },
                }
            );

            MUISUniverses.Add(
                21094,
                new UniverseInfo[]
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
                        UniverseId = 1,
                    },
                }
            );

            MUISUniverses.Add(
                20804,
                new UniverseInfo[]
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
                        UniverseId = 1,
                    },
                }
            );

            MUISUniverses.Add(
                21513,
                new UniverseInfo[]
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
                        UniverseId = 1,
                    },
                }
            );

            MUISUniverses.Add(
                22204,
                new UniverseInfo[]
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
                        UniverseId = 1,
                    },
                }
            );

            MUISUniverses.Add(
                23360,
                new UniverseInfo[]
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
                        UniverseId = 1,
                    },
                }
            );

            MUISUniverses.Add(
                20624,
                new UniverseInfo[]
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
                        UniverseId = 2,
                    },
                }
            );

            MUISUniverses.Add(
                20764,
                new UniverseInfo[]
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
                    },
                }
            );

            MUISUniverses.Add(
                20364,
                new UniverseInfo[]
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
                    },
                }
            );

            MUISUniverses.Add(
                21624,
                new UniverseInfo[]
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
                    },
                }
            );

            MUISUniverses.Add(
                21614,
                new UniverseInfo[]
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
                    },
                }
            );

            MUISUniverses.Add(
                20344,
                new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "ATV Offroad Fury Pro PSP",
                        Endpoint = iptofile,
                        Port = 10075,
                        SvoURL = $"http://{iptofile}:10060/ATV4UNIFIED_SVML/index.jsp ",
                    },
                }
            );

            MUISUniverses.Add(
                20371,
                new UniverseInfo[]
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
                        BillingSystemName =
                            "Sony Computer Entertainment America, Inc. Billing System",
                        ExtendedInfo = $"* http://{iptofile}/dev.01.86/",
                        Port = 10075,
                        UniverseId = 1,
                    },
                }
            );

            MUISUniverses.Add(
                20374,
                new UniverseInfo[]
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
                        ExtendedInfo = $"* http://{iptofile}/01.86/",
                        UniverseBilling = "SCEA",
                        BillingSystemName =
                            "Sony Computer Entertainment America, Inc. Billing System",
                        Port = 10075,
                        UniverseId = 1,
                    },
                }
            );

            MUISUniverses.Add(
                20384,
                new UniverseInfo[]
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
                        BillingSystemName =
                            "Sony Computer Entertainment America, Inc. Billing System",
                        Port = 10075,
                        UniverseId = 1,
                    },
                }
            );

            MUISUniverses.Add(
                21354,
                new UniverseInfo[]
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
                        BillingSystemName =
                            "Sony Computer Entertainment America, Inc. Billing System",
                        Port = 10075,
                        UniverseId = 1,
                    },
                }
            );

            MUISUniverses.Add(
                23014,
                new UniverseInfo[]
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
                        BillingSystemName =
                            "Sony Computer Entertainment America, Inc. Billing System",
                        Port = 10075,
                        UniverseId = 1,
                    },
                }
            );

            MUISUniverses.Add(
                21834,
                new UniverseInfo[]
                {
                    new UniverseInfo()
                    {
                        Name = "Twisted Metal X Online",
                        Description = "Revived by MultiServer",
                        Status = 1,
                        UserCount = 1,
                        MaxUsers = 256,
                        Endpoint = iptofile,
                        SvoURL =
                            "http://twistedmetalx-prod3.svo.online.scea.com:10060/TWISTEDMETALX_XML/uri/URIStore.do",
                        ExtendedInfo = null,
                        UniverseBilling = null,
                        BillingSystemName = null,
                        Port = 10075,
                        UniverseId = 1,
                    },
                }
            );

            MUISUniverses.Add(
                20304,
                new UniverseInfo[]
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
                        UniverseId = 1,
                    },
                }
            );

            MUISUniverses.Add(
                20032,
                new UniverseInfo[]
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
                        UniverseId = 1,
                    },
                }
            );

            MUISUniverses.Add(
                22920,
                new UniverseInfo[]
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
                        ExtendedInfo =
                            "<XML><A url=`http://blob117.scea.com` latest=`0` access=`0` /></XML>",
                        UniverseBilling = "SCEA",
                        BillingSystemName =
                            "Sony Computer Entertainment America, Inc. Billing System",
                    },
                }
            );

            MUISUniverses.Add(
                22500,
                new UniverseInfo[]
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
                    },
                }
            );

            MUISUniverses.Add(
                21731,
                new UniverseInfo[]
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
                    },
                }
            );

            MUISUniverses.Add(
                23624,
                new UniverseInfo[]
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
                    },
                }
            );

            MUISUniverses.Add(
                20034,
                new UniverseInfo[]
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
                        UniverseId = 1,
                    },
                }
            );

            MUISUniverses.Add(
                20454,
                new UniverseInfo[]
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
                    },
                }
            );

            MUISUniverses.Add(
                20314,
                new UniverseInfo[]
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
                    },
                }
            );

            MUISUniverses.Add(
                21874,
                new UniverseInfo[]
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
                        UniverseId = 1,
                    },
                }
            );

            MUISUniverses.Add(
                21244,
                new UniverseInfo[]
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
                        UniverseId = 1,
                    },
                }
            );

            MUISUniverses.Add(
                20463,
                new UniverseInfo[]
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
                    },
                }
            );

            MUISUniverses.Add(
                20434,
                new UniverseInfo[]
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
                    },
                }
            );

            List<int> WarhawkAppIDs = new()
            {
                21564,
                21574,
                21584,
                21594,
                22274,
                22284,
                22294,
                22304,
                20040,
                20041,
                20042,
                20043,
                20044,
            };

            foreach (int AppID in WarhawkAppIDs)
            {
                MUISUniverses.Add(
                    AppID,
                    new UniverseInfo[]
                    {
                        new()
                        {
                            Name = "Warhawk",
                            Description = "Matchmaking Server",
                            Status = 1,
                            UserCount = 1,
                            MaxUsers = 256,
                            Endpoint = iptofile,
                            SvoURL =
                                $"http://{iptofile}:10060/WARHAWK_SVML/index.jsp?languageID=1 ",
                            ExtendedInfo =
                                $"v1.50 http://{iptofile}/medius-patch/warhawk-prod/r016/",
                            UniverseBilling = "SCEA",
                            BillingSystemName = "Sony Computer Entertainment America Inc.",
                            Port = 10075,
                            UniverseId = 1,
                        },
                    }
                );
            }
            #endregion

            // Write the JObject to a file
            File.WriteAllText(
                configPath,
                new JObject(
                    new JProperty("config_version", (ushort)6),
                    new JProperty(
                        "medius",
                        new JObject(
                            new JProperty("enabled", EnableMedius),
                            new JProperty("config", MEDIUSConfig),
                            new JProperty("plugins_folder", MediusPluginsFolder),
                            new JProperty("plugin_tick_interval_ms", MEDIUSPluginTickIntervalMs),
                            new JProperty("api_key", MEDIUSAPIKey),
                            new JProperty(
                                "mfs",
                                new JObject(
                                    new JProperty("file_server_root_path", MEDIUSMFSRootPath),
                                    new JProperty("transfer_uri", MEDIUSMFSTransferURI),
                                    new JProperty("download_qsize", MEDIUSMFSDownloadQSize),
                                    new JProperty("upload_qsize", MEDIUSMFSUploadQSize),
                                    new JProperty(
                                        "queue_timeout_interval",
                                        MEDIUSMFSQueueTimeoutInterval
                                    )
                                )
                            ),
                            new JProperty(
                                "servers",
                                new JObject(
                                    new JProperty(
                                        "maps",
                                        new JObject(
                                            new JProperty("enable", MEDIUSEnableMAPS),
                                            new JProperty("version", MEDIUSMAPSVersion),
                                            new JProperty("tcp_port", MEDIUSMAPSTCPPort),
                                            new JProperty("udp_port", MEDIUSMAPSUDPPort),
                                            BuildUniversesJson(false)
                                        )
                                    ),
                                    new JProperty(
                                        "mms",
                                        new JObject(
                                            new JProperty("enable", MEDIUSEnableMMS),
                                            new JProperty("version", MEDIUSMMSVersion),
                                            new JProperty("tcp_port", MEDIUSMMSTCPPort)
                                        )
                                    ),
                                    new JProperty(
                                        "mas",
                                        new JObject(
                                            new JProperty("enable", MEDIUSEnableMAS),
                                            new JProperty("version", MEDIUSMASVersion),
                                            new JProperty("ports", new JArray(MEDIUSMASPorts))
                                        )
                                    ),
                                    new JProperty(
                                        "mls",
                                        new JObject(
                                            new JProperty("enable", MEDIUSEnableMLS),
                                            new JProperty("version", MEDIUSMLSVersion),
                                            new JProperty("port", MEDIUSMLSPort)
                                        )
                                    ),
                                    new JProperty(
                                        "mps",
                                        new JObject(
                                            new JProperty("enable", MEDIUSEnableMPS),
                                            new JProperty("version", MEDIUSMPSVersion),
                                            new JProperty("port", MEDIUSMPSPort)
                                        )
                                    )
                                )
                            ),
                            new JProperty(
                                "np",
                                new JObject(
                                    new JProperty("mls_ip_override", MEDIUSNpMLSIpOverride),
                                    new JProperty("mls_port_override", MEDIUSNpMLSPortOverride)
                                )
                            ),
                            new JProperty("allow_guests", MEDIUSAllowGuests),
                            new JProperty("https_svo_check_patcher", MEDIUSHttpsSVOCheckPatcher),
                            new JProperty("poke_patch_on", MEDIUSPokePatchOn),
                            new JProperty(
                                "playstation_home_plugin",
                                new JObject(
                                    new JProperty(
                                        "closed_beta_auto_create_plugin",
                                        MEDIUSPlaystationHomeClosedBetaAutoCreatePlugin
                                    ),
                                    new JProperty(
                                        "closed_beta_scene_list_path",
                                        MEDIUSPlaystationHomeClosedBetaSceneListPath
                                    ),
                                    new JProperty(
                                        "allow_any_eboot",
                                        MEDIUSPlaystationHomeAllowAnyEboot
                                    ),
                                    new JProperty("anti_cheat", MEDIUSPlaystationHomeAntiCheat),
                                    new JProperty(
                                        "force_invite_exploit_patch",
                                        MEDIUSPlaystationHomeForceInviteExploitPatch
                                    )
                                )
                            ),
                            new JProperty(
                                "mum_servers_access_list",
                                JObject.FromObject(MEDIUSMUMServersAccessList)
                            )
                        )
                    ),
                    new JProperty(
                        "dme",
                        new JObject(
                            new JProperty("enabled", EnableDME),
                            new JProperty("tcp_port", DMETCPPort),
                            new JProperty("udp_port", DMEUDPPort),
                            new JProperty("plugins_folder", DmePluginsFolder),
                            new JProperty("plugin_tick_interval_ms", DMEPluginTickIntervalMs),
                            new JProperty(
                                "compatible_application_ids",
                                DMECompatibleApplicationIds ?? Array.Empty<int>()
                            ),
                            new JProperty("client_reconnect_interval", DMEClientReconnectInterval),
                            new JProperty("server_max_world", DMEServerMaxWorld),
                            new JProperty("max_clients_per_worlds", DMEMaxClientsPerWorlds),
                            new JProperty("max_clients_override", DMEMaxClientsOverride),
                            new JProperty(
                                "mas_settings",
                                new JObject(
                                    new JProperty("ip", DMEMASIp),
                                    new JProperty("port", DMEMASPort)
                                )
                            ),
                            new JProperty(
                                "mps_settings",
                                new JObject(
                                    new JProperty("ip", DMEMPSIp),
                                    new JProperty("port", DMEMPSPort)
                                )
                            )
                        )
                    ),
                    new JProperty(
                        "muis",
                        new JObject(
                            new JProperty("enabled", EnableMuis),
                            new JProperty("ports", MUISPorts ?? Array.Empty<ushort>()),
                            new JProperty("encrypt_messages", MUISEncryptMessages),
                            new JProperty(
                                "compatible_application_ids",
                                MUISCompatibleApplicationIds ?? Array.Empty<int>()
                            ),
                            new JProperty("version", MUISVersion),
                            BuildUniversesJson(true)
                        )
                    ),
                    new JProperty(
                        "nat",
                        new JObject(
                            new JProperty("enabled", EnableNAT),
                            new JProperty("port", NATPort)
                        )
                    ),
                    new JProperty(
                        "bwps",
                        new JObject(
                            new JProperty("enabled", EnableBWPS),
                            new JProperty("port", BWPSPort)
                        )
                    ),
                    new JProperty("eboot_defs_config", EBOOTDEFSConfig),
                    new JProperty("https_dns_list", HTTPSDNSList ?? Array.Empty<string>()),
                    new JProperty("certificate_file", HTTPSCertificateFile),
                    new JProperty("certificate_password", HTTPSCertificatePassword),
                    new JProperty(
                        "certificate_hashing_algorithm",
                        HTTPSCertificateHashingAlgorithm.Name
                    ),
                    new JProperty("player_api_static_path", PlayerAPIStaticPath),
                    new JProperty("medius_api_key", MEDIUSAPIKey),
                    new JProperty("ssfw_url", SSFWUrl),
                    new JProperty("medius_plugins_folder", MediusPluginsFolder),
                    new JProperty("dme_plugins_folder", DmePluginsFolder),
                    new JProperty("database", DatabaseConfig),
                    new JProperty(
                        "prometheus",
                        new JObject(
                            new JProperty("enabled", EnableMetrics),
                            new JProperty("url", MetricsUrl),
                            new JProperty("port", MetricsPort)
                        )
                    )
                ).ToString()
            );
        }
        else
        {
            try
            {
                // Parse the JSON configuration
                dynamic config = JObject.Parse(File.ReadAllText(configPath));

                ushort config_version = GetValueOrDefault(config, "config_version", (ushort)0);
                if (config_version > 1)
                {
                    if (config_version > 2)
                    {
                        EnableMetrics = GetValueOrDefault(
                            config.prometheus,
                            "enabled",
                            EnableMetrics
                        );
                        MetricsUrl = GetValueOrDefault(config.prometheus, "url", MetricsUrl);
                        MetricsPort = GetValueOrDefault(config.prometheus, "port", MetricsPort);
                    }

                    EnableMedius = GetValueOrDefault(config.medius, "enabled", EnableMedius);
                    MEDIUSConfig = GetValueOrDefault(config.medius, "config", MEDIUSConfig);
                    if (config_version > 5)
                    {
                        MediusPluginsFolder = GetValueOrDefault(
                            config.medius,
                            "plugins_folder",
                            MediusPluginsFolder
                        );

                        // API Key
                        string mediusApiKey = GetValueOrDefault(
                            config.medius,
                            "api_key",
                            MEDIUSAPIKey
                        );
                        if (mediusApiKey.IsBase64().IsValid)
                            MEDIUSAPIKey = mediusApiKey;

                        MEDIUSPluginTickIntervalMs = GetValueOrDefault(
                            config.medius,
                            "plugin_tick_interval_ms",
                            MEDIUSPluginTickIntervalMs
                        );

                        // MFS (Medius File Service)
                        MEDIUSMFSRootPath = GetValueOrDefault(
                            config.medius.mfs,
                            "file_server_root_path",
                            MEDIUSMFSRootPath
                        );
                        MEDIUSMFSTransferURI = GetValueOrDefault(
                            config.medius.mfs,
                            "transfer_uri",
                            MEDIUSMFSTransferURI
                        );
                        MEDIUSMFSDownloadQSize = GetValueOrDefault(
                            config.medius.mfs,
                            "download_qsize",
                            MEDIUSMFSDownloadQSize
                        );
                        MEDIUSMFSUploadQSize = GetValueOrDefault(
                            config.medius.mfs,
                            "upload_qsize",
                            MEDIUSMFSUploadQSize
                        );
                        MEDIUSMFSQueueTimeoutInterval = GetValueOrDefault(
                            config.medius.mfs,
                            "queue_timeout_interval",
                            MEDIUSMFSQueueTimeoutInterval
                        );

                        // Servers (maps, mms, mas, mls, mps)
                        MEDIUSEnableMAPS = GetValueOrDefault(
                            config.medius.servers.maps,
                            "enable",
                            MEDIUSEnableMAPS
                        );
                        MEDIUSMAPSVersion = GetValueOrDefault(
                            config.medius.servers.maps,
                            "version",
                            MEDIUSMAPSVersion
                        );
                        MEDIUSMAPSTCPPort = GetValueOrDefault(
                            config.medius.servers.maps,
                            "tcp_port",
                            MEDIUSMAPSTCPPort
                        );
                        MEDIUSMAPSUDPPort = GetValueOrDefault(
                            config.medius.servers.maps,
                            "udp_port",
                            MEDIUSMAPSUDPPort
                        );
                        if (config.medius.servers.maps.universes != null)
                        {
                            try
                            {
                                MAPSUniverses = config.medius.servers.maps.universes.ToObject<
                                    Dictionary<int, NetUniverseInfo[]>
                                >();
                            }
                            catch (Exception ex)
                            {
                                MAPSUniverses = new Dictionary<int, NetUniverseInfo[]>();

                                LoggerAccessor.LogWarn(
                                    $"Failed to parse MAPS universes config: {ex}"
                                );
                            }
                        }

                        MEDIUSEnableMMS = GetValueOrDefault(
                            config.medius.servers.mms,
                            "enable",
                            MEDIUSEnableMMS
                        );
                        MEDIUSMMSVersion = GetValueOrDefault(
                            config.medius.servers.mms,
                            "version",
                            MEDIUSMMSVersion
                        );
                        MEDIUSMMSTCPPort = GetValueOrDefault(
                            config.medius.servers.mms,
                            "tcp_port",
                            MEDIUSMMSTCPPort
                        );

                        MEDIUSEnableMAS = GetValueOrDefault(
                            config.medius.servers.mas,
                            "enable",
                            MEDIUSEnableMAS
                        );
                        MEDIUSMASVersion = GetValueOrDefault(
                            config.medius.servers.mas,
                            "version",
                            MEDIUSMASVersion
                        );
                        MEDIUSMASPorts = GetValueOrDefault(
                            config.medius.servers.mas,
                            "ports",
                            MEDIUSMASPorts
                        );

                        MEDIUSEnableMLS = GetValueOrDefault(
                            config.medius.servers.mls,
                            "enable",
                            MEDIUSEnableMLS
                        );
                        MEDIUSMLSVersion = GetValueOrDefault(
                            config.medius.servers.mls,
                            "version",
                            MEDIUSMLSVersion
                        );
                        MEDIUSMLSPort = GetValueOrDefault(
                            config.medius.servers.mls,
                            "port",
                            MEDIUSMLSPort
                        );

                        MEDIUSEnableMPS = GetValueOrDefault(
                            config.medius.servers.mps,
                            "enable",
                            MEDIUSEnableMPS
                        );
                        MEDIUSMPSVersion = GetValueOrDefault(
                            config.medius.servers.mps,
                            "version",
                            MEDIUSMPSVersion
                        );
                        MEDIUSMPSPort = GetValueOrDefault(
                            config.medius.servers.mps,
                            "port",
                            MEDIUSMPSPort
                        );

                        // NP overrides
                        MEDIUSNpMLSIpOverride = GetValueOrDefault(
                            config.medius.np,
                            "mls_ip_override",
                            MEDIUSNpMLSIpOverride
                        );
                        MEDIUSNpMLSPortOverride = GetValueOrDefault(
                            config.medius.np,
                            "mls_port_override",
                            MEDIUSNpMLSPortOverride
                        );

                        // Misc
                        MEDIUSAllowGuests = GetValueOrDefault(
                            config.medius,
                            "allow_guests",
                            MEDIUSAllowGuests
                        );
                        MEDIUSHttpsSVOCheckPatcher = GetValueOrDefault(
                            config.medius,
                            "https_svo_check_patcher",
                            MEDIUSHttpsSVOCheckPatcher
                        );
                        MEDIUSPokePatchOn = GetValueOrDefault(
                            config.medius,
                            "poke_patch_on",
                            MEDIUSPokePatchOn
                        );

                        // Playstation Home plugin
                        MEDIUSPlaystationHomeClosedBetaAutoCreatePlugin = GetValueOrDefault(
                            config.medius.playstation_home_plugin,
                            "closed_beta_auto_create_plugin",
                            MEDIUSPlaystationHomeClosedBetaAutoCreatePlugin
                        );
                        MEDIUSPlaystationHomeClosedBetaSceneListPath = GetValueOrDefault(
                            config.medius.playstation_home_plugin,
                            "closed_beta_scene_list_path",
                            MEDIUSPlaystationHomeClosedBetaSceneListPath
                        );
                        MEDIUSPlaystationHomeAllowAnyEboot = GetValueOrDefault(
                            config.medius.playstation_home_plugin,
                            "allow_any_eboot",
                            MEDIUSPlaystationHomeAllowAnyEboot
                        );
                        MEDIUSPlaystationHomeAntiCheat = GetValueOrDefault(
                            config.medius.playstation_home_plugin,
                            "anti_cheat",
                            MEDIUSPlaystationHomeAntiCheat
                        );
                        MEDIUSPlaystationHomeForceInviteExploitPatch = GetValueOrDefault(
                            config.medius.playstation_home_plugin,
                            "force_invite_exploit_patch",
                            MEDIUSPlaystationHomeForceInviteExploitPatch
                        );

                        // MUM servers access list
                        try
                        {
                            if (config.medius.mum_servers_access_list != null)
                                MEDIUSMUMServersAccessList =
                                    config.medius.mum_servers_access_list.ToObject<
                                        Dictionary<string, string>
                                    >();
                        }
                        catch (Exception ex)
                        {
                            LoggerAccessor.LogWarn(
                                $"Failed to parse medius.mum_servers_access_list: {ex}"
                            );
                        }
                    }

                    EnableDME = GetValueOrDefault(config.dme, "enabled", EnableDME);
                    if (config_version > 4)
                    {
                        DMETCPPort = GetValueOrDefault(config.dme, "tcp_port", DMETCPPort);
                        DMEUDPPort = GetValueOrDefault(config.dme, "udp_port", DMEUDPPort);
                        DmePluginsFolder = GetValueOrDefault(
                            config.dme,
                            "plugins_folder",
                            DmePluginsFolder
                        );
                        DMECompatibleApplicationIds = GetValueOrDefault(
                            config.dme,
                            "compatible_application_ids",
                            DMECompatibleApplicationIds
                        );
                        DMEClientReconnectInterval = GetValueOrDefault(
                            config.dme,
                            "client_reconnect_interval",
                            DMEClientReconnectInterval
                        );
                        DMEPluginTickIntervalMs = GetValueOrDefault(
                            config.dme,
                            "plugin_tick_interval_ms",
                            DMEPluginTickIntervalMs
                        );
                        DMEServerMaxWorld = GetValueOrDefault(
                            config.dme,
                            "server_max_world",
                            DMEServerMaxWorld
                        );
                        DMEMaxClientsPerWorlds = GetValueOrDefault(
                            config.dme,
                            "max_clients_per_worlds",
                            DMEMaxClientsPerWorlds
                        );
                        DMEMaxClientsOverride = GetValueOrDefault(
                            config.dme,
                            "max_clients_override",
                            DMEMaxClientsOverride
                        );
                        DMEMASIp = GetValueOrDefault(config.dme.mas_settings, "ip", DMEMASIp);
                        DMEMASPort = GetValueOrDefault(config.dme.mas_settings, "port", DMEMASPort);
                        DMEMPSIp = GetValueOrDefault(config.dme.mps_settings, "ip", DMEMPSIp);
                        DMEMPSPort = GetValueOrDefault(config.dme.mps_settings, "port", DMEMPSPort);
                    }

                    EnableMuis = GetValueOrDefault(config.muis, "enabled", EnableMuis);
                    if (config_version > 4)
                    {
                        MUISPorts = GetValueOrDefault(config.muis, "ports", MUISPorts);
                        MUISEncryptMessages = GetValueOrDefault(
                            config.muis,
                            "encrypt_messages",
                            MUISEncryptMessages
                        );
                        MUISCompatibleApplicationIds = GetValueOrDefault(
                            config.muis,
                            "compatible_application_ids",
                            MUISCompatibleApplicationIds
                        );
                        MUISVersion = GetValueOrDefault(config.muis, "version", MUISVersion);
                        if (config.muis.universes != null)
                        {
                            try
                            {
                                MUISUniverses = config.muis.universes.ToObject<
                                    Dictionary<int, UniverseInfo[]>
                                >();
                            }
                            catch (Exception ex)
                            {
                                MUISUniverses = new Dictionary<int, UniverseInfo[]>();

                                LoggerAccessor.LogWarn(
                                    $"Failed to parse MUIS universes config: {ex}"
                                );
                            }
                        }
                    }

                    EnableNAT = GetValueOrDefault(config.nat, "enabled", EnableNAT);
                    if (config_version > 4)
                        NATPort = GetValueOrDefault(config.nat, "port", NATPort);

                    EnableBWPS = GetValueOrDefault(config.bwps, "enabled", EnableBWPS);
                    if (config_version > 4)
                        BWPSPort = GetValueOrDefault(config.bwps, "port", BWPSPort);

                    HTTPSCertificateFile = GetValueOrDefault(
                        config,
                        "certificate_file",
                        HTTPSCertificateFile
                    );
                    HTTPSCertificatePassword = GetValueOrDefault(
                        config,
                        "certificate_password",
                        HTTPSCertificatePassword
                    );
                    HTTPSCertificateHashingAlgorithm = new HashAlgorithmName(
                        GetValueOrDefault(
                            config,
                            "certificate_hashing_algorithm",
                            HTTPSCertificateHashingAlgorithm.Name
                        )
                    );
                    PlayerAPIStaticPath = GetValueOrDefault(
                        config,
                        "player_api_static_path",
                        PlayerAPIStaticPath
                    );
                    HTTPSDNSList = GetValueOrDefault(config, "https_dns_list", HTTPSDNSList);
                    EBOOTDEFSConfig = GetValueOrDefault(
                        config,
                        "eboot_defs_config",
                        EBOOTDEFSConfig
                    );
                    string APIKey = GetValueOrDefault(config, "medius_api_key", MEDIUSAPIKey);
                    if (APIKey.IsBase64().IsValid)
                        MEDIUSAPIKey = APIKey;
                    string ssfwADR = GetValueOrDefault(config, "ssfw_url", SSFWUrl);
                    if (
                        !string.IsNullOrEmpty(ssfwADR)
                        && ssfwADR.StartsWith("http", StringComparison.InvariantCultureIgnoreCase)
                    )
                        SSFWUrl = ssfwADR;
                    if (config_version <= 4)
                    {
                        if (config_version > 3)
                        {
                            MediusPluginsFolder = GetValueOrDefault(
                                config,
                                "medius_plugins_folder",
                                MediusPluginsFolder
                            );
                            DmePluginsFolder = GetValueOrDefault(
                                config,
                                "dme_plugins_folder",
                                DmePluginsFolder
                            );
                        }
                        else
                            DmePluginsFolder = MediusPluginsFolder = GetValueOrDefault(
                                config,
                                "plugins_folder",
                                MediusPluginsFolder
                            );
                    }
                    DatabaseConfig = GetValueOrDefault(config, "database", DatabaseConfig);
                }
                else
                    LoggerAccessor.LogWarn(
                        $"{configPath} file is outdated, using server's default."
                    );
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogWarn(
                    $"{configPath} file is malformed (exception: {ex}), using server's default."
                );
            }
        }

        Database = new DbController(DatabaseConfig);

        #region Check ebootdefs.json
        if (!string.IsNullOrEmpty(EBOOTDEFSConfig) && File.Exists(EBOOTDEFSConfig))
            LoadHomeOffsetsJson(File.ReadAllText(EBOOTDEFSConfig));
        #endregion
    }

    private static JProperty BuildUniversesJson(bool muis)
    {
        var universesObject = new JObject();

        if (muis)
        {
            foreach (var kvp in MUISUniverses)
            {
                var universeArray = new JArray();

                foreach (var universe in kvp.Value)
                {
                    universeArray.Add(
                        new JObject
                        {
                        new JProperty("Name", universe.Name),
                        new JProperty("Description", universe.Description),
                        new JProperty("UserCount", universe.UserCount),
                        new JProperty("MaxUsers", universe.MaxUsers),
                        new JProperty("Endpoint", universe.Endpoint),
                        new JProperty("SvoURL", universe.SvoURL),
                        new JProperty("Status", universe.Status),
                        new JProperty("ExtendedInfo", universe.ExtendedInfo),
                        new JProperty("UniverseBilling", universe.UniverseBilling),
                        new JProperty("BillingSystemName", universe.BillingSystemName),
                        new JProperty("Port", universe.Port),
                        new JProperty("UniverseId", universe.UniverseId),
                        }
                    );
                }

                universesObject.Add(kvp.Key.ToString(), universeArray);
            }
        }
        else
        {
            foreach (var kvp in MAPSUniverses)
            {
                var universeArray = new JArray();

                foreach (var universe in kvp.Value)
                {
                    universeArray.Add(
                        new JObject
                        {
                        new JProperty("Name", universe.Name),
                        new JProperty("AuthDNS", universe.AuthDNS),
                        new JProperty("AuthIP", universe.AuthIP),
                        new JProperty("SvoURL", universe.SvoURL),
                        new JProperty("Port", universe.Port),
                        new JProperty("UniverseId", universe.UniverseId),
                        }
                    );
                }

                universesObject.Add(kvp.Key.ToString(), universeArray);
            }
        }

        return new JProperty("universes", universesObject);
    }

    private static void LoadHomeOffsetsJson(string? jsonData)
    {
        if (string.IsNullOrEmpty(jsonData))
            return;

        var HomeOffsetsDic = JsonConvert.DeserializeObject<Dictionary<string, HomeOffsetsJsonData>>(
            jsonData
        );

        if (HomeOffsetsDic != null)
        {
            foreach (var kvp in HomeOffsetsDic)
            {
                var data = kvp.Value;
                data.Sha1Hash = kvp.Key;

                var parts = data.Version?.Split('.') ?? Array.Empty<string>();

                if (parts.Length >= 2)
                {
                    // Why softfloats? This operation can vary too much between architectures and lead to failures.
                    var ctx = new SoftFloatContext { Rounding = RoundingMode.NearEven };

                    var major = int.Parse(parts[0]);
                    double value = Float64.FromInt32(ctx, major);

                    var scale = 1;
                    for (var i = 1; i < parts.Length; i++)
                    {
                        if (int.TryParse(parts[i], out var partValue))
                        {
                            // increase scale by the number of digits in this part
                            scale *= (int)Math.Pow(10, parts[i].Length);

                            value +=
                                Float64.FromInt32(ctx, partValue) / Float64.FromInt32(ctx, scale);
                        }
                    }

                    data.VersionAsDouble = value;
                }
            }

            lock (HomeOffsetsList)
                HomeOffsetsList = new List<HomeOffsetsJsonData>(HomeOffsetsDic.Values);
        }
        else
            LoggerAccessor.LogError("LoadHomeOffsetsJson - jsonData was null or empty!");
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
                    if (jObject.TryGetValue(propertyName, out var value))
                    {
                        var returnvalue = value.ToObject<T>();
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
    private static readonly string configPath = configDir + "horizon.json";
    private static readonly string configMultiServerLibraryPath =
        configDir + "MultiServerLibrary.json";
    private static SnmpTrapSender? trapSender = null;
    private static Task? MediusDatabaseLoop;
    private static ConcurrentBag<CrudServerHandler>? HTTPBag;
    private static MetricServer? _metricsServer;

    private static CancellationTokenSource? _cts;

    #region NAT
    private static NATProcessor? _natServer;
    #endregion
    #region BWPS
    private static BWPSProcessor? _bwpsServer;
    #endregion
    #region MUIS
    private static MUISProcessor? _muisServer;
    private static MUISManager? _muisManager;
    #endregion
    #region MUM
    private static MumServerHandler? _MUMServer;
    public static MumManager MUMManager { get; } = new();
    #endregion
    #region DME
    public static DMEProcessor? DmeServer { get; private set; } = null;
    public static DMEManager? DmeManager { get; private set; } = null;
    #endregion
    #region DME
    public static List<BaseMediusProcessor> MediusServers { get; private set; } = new();
    public static MediusManager? MediusManager { get; private set; } = null;
    #endregion
    #region DB
    private static readonly DatabaseManager _dbManager = new();
    #endregion

    private static async Task HorizonStarter()
    {
        int _cpuCount = Environment.ProcessorCount * 2;

        _metricsServer?.Dispose();

        _cts = new CancellationTokenSource();

        if (HorizonServerConfiguration.EnableMetrics)
        {
            _metricsServer = new MetricServer(
                HorizonServerConfiguration.MetricsPort,
                1,
                HorizonServerConfiguration.MetricsUrl
            );
            _metricsServer.Start();
        }

        if (HorizonServerConfiguration.EnableMedius)
        {
            try
            {
                _MUMServer = new MumServerHandler("*", 10076);

                HTTPBag = new ConcurrentBag<CrudServerHandler>
                {
                    new("*", 61920),
                    new(
                        "*",
                        NetworkPorts.Http.SslAux,
                        HorizonServerConfiguration.HTTPSCertificateFile,
                        HorizonServerConfiguration.HTTPSCertificatePassword
                    ),
                };
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(
                    "[HTTPSERVICE] - An exception was thrown while starting the Medius HTTP Services: "
                        + ex
                );
            }
        }

        if (HorizonServerConfiguration.EnableMedius)
        {
            MediusServers.Clear();

            // DO NOT CHANGE THE ORDER (bandwith priority applies to the first servers in the list).

            if (HorizonServerConfiguration.MEDIUSEnableMAS)
            {
                foreach (var port in HorizonServerConfiguration.MEDIUSMASPorts)
                    MediusServers.Add(new MAS() { TCPPort = port });
            }
            if (HorizonServerConfiguration.MEDIUSEnableMLS)
                MediusServers.Add(new MLS());
            if (HorizonServerConfiguration.MEDIUSEnableMPS)
                MediusServers.Add(new MPS());
            if (HorizonServerConfiguration.MEDIUSEnableMMS)
                MediusServers.Add(new MMS());
            if (HorizonServerConfiguration.MEDIUSEnableMAPS)
                MediusServers.Add(new MAPS());

            MediusManager = new MediusManager(MediusServers);

            _ = Task.WhenAll(MediusServers.Select(server => server.StartAsync(_cpuCount)));
            _ = MediusManager.StartTickPooling(_cts.Token);
        }

        if (HorizonServerConfiguration.EnableNAT)
        {
            _natServer ??= new NATProcessor();
            _ = _natServer.StartAsync(_cts.Token);
        }

        if (HorizonServerConfiguration.EnableBWPS)
        {
            _bwpsServer ??= new BWPSProcessor();
            _ = _bwpsServer.StartAsync(1); // Single core is fine, BWPS is not heavy.
        }

        if (HorizonServerConfiguration.EnableMuis)
        {
            _muisServer ??= new MUISProcessor();
            _muisManager = new MUISManager(_muisServer);
            _ = _muisServer.StartAsync(_cpuCount);
            _ = _muisManager.StartTickPooling(_cts.Token);
        }

        if (HorizonServerConfiguration.EnableDME)
        {
            // Wait a bit of time for medius to properly start.
            await Task.Delay(5000).ConfigureAwait(false);

            DmeServer ??= new DMEProcessor();
            DmeManager = new DMEManager(DmeServer);
            _ = DmeServer.StartAsync(_cpuCount);
            _ = DmeManager.StartTickPooling(_cts.Token);
        }
    }

    private static void StartOrUpdateServer()
    {
        _cts?.Cancel();

        DmeServer?.StopAsync().Wait();

        _muisServer?.StopAsync().Wait();

        _bwpsServer?.StopAsync().Wait();

        _natServer?.Stop();

        Task.WhenAll(MediusServers.Select(server => server.StopAsync())).Wait();

        _MUMServer?.StopServer();

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
            MultiServerLibrary.SSL.CertificateHelper.InitializeSSLChainSignedCertificates(
                HorizonServerConfiguration.HTTPSCertificateFile,
                HorizonServerConfiguration.HTTPSCertificatePassword,
                HorizonServerConfiguration.HTTPSDNSList,
                HorizonServerConfiguration.HTTPSCertificateHashingAlgorithm
            );

        MediusDatabaseLoop ??= Task.Run(() =>
            _dbManager.StartTickPooling(new CancellationTokenSource().Token)
        );

        HorizonStarter().Wait();
    }

    static void Main()
    {
        if (!MultiServerLibrary.Extension.Windows.Win32API.IsWindows)
            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
        else
            // TODO, adjust the server to fine tune the firewall entries (can be a secrurity issue).
            TechnitiumLibrary.Net.Firewall.FirewallHelper.CheckFirewallEntries(
                Process.GetCurrentProcess().MainModule.FileName,
                null
            );

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

        _ = Task.Run(GeoIP.Initialize);

        MultiServerLibraryConfiguration.RefreshVariables(configMultiServerLibraryPath);

        if (MultiServerLibraryConfiguration.EnableSNMPReports)
        {
            trapSender = new SnmpTrapSender(
                MultiServerLibraryConfiguration.SNMPHashAlgorithm.Name,
                MultiServerLibraryConfiguration.SNMPTrapHost,
                MultiServerLibraryConfiguration.SNMPUserName,
                MultiServerLibraryConfiguration.SNMPAuthPassword,
                MultiServerLibraryConfiguration.SNMPPrivatePassword,
                MultiServerLibraryConfiguration.SNMPEnterpriseOid
            );

            if (trapSender.report != null)
            {
                LoggerAccessor.RegisterPostLogAction(
                    LogLevel.Information,
                    (msg, args) =>
                    {
                        if (MultiServerLibraryConfiguration.EnableSNMPReports)
                            trapSender!.SendInfo(msg);
                    }
                );

                LoggerAccessor.RegisterPostLogAction(
                    LogLevel.Warning,
                    (msg, args) =>
                    {
                        if (MultiServerLibraryConfiguration.EnableSNMPReports)
                            trapSender!.SendWarn(msg);
                    }
                );

                LoggerAccessor.RegisterPostLogAction(
                    LogLevel.Error,
                    (msg, args) =>
                    {
                        if (MultiServerLibraryConfiguration.EnableSNMPReports)
                            trapSender!.SendCrit(msg);
                    }
                );

                LoggerAccessor.RegisterPostLogAction(
                    LogLevel.Critical,
                    (msg, args) =>
                    {
                        if (MultiServerLibraryConfiguration.EnableSNMPReports)
                            trapSender!.SendCrit(msg);
                    }
                );
#if DEBUG
                LoggerAccessor.RegisterPostLogAction(
                    LogLevel.Debug,
                    (msg, args) =>
                    {
                        if (MultiServerLibraryConfiguration.EnableSNMPReports)
                            trapSender!.SendInfo(msg);
                    }
                );
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

                LoggerAccessor.LogInfo(
                    "Press one of the following keys to trigger an action: [R (Reboot),S (Shutdown)]"
                );

                switch (char.ToLower(Console.ReadKey().KeyChar))
                {
                    case 's':
                        LoggerAccessor.LogWarn(
                            "Are you sure you want to shut down the server? [y/N]"
                        );

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
