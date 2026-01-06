using Org.BouncyCastle.Math;
using Horizon.RT.Cryptography.RSA;
using MultiServerLibrary.Extension;


namespace Horizon.DME.Config
{
    public class ServerSettings
    {
        /// <summary>
        /// MAS connection information
        /// </summary>
        public MASSettings MAS { get; set; } = new();

        /// <summary>
        /// MPS connection information
        /// </summary>
        public MPSSettings MPS { get; set; } = new();

        /// <summary>
        /// Key used to authenticate clients.
        /// </summary>
        public RsaKeyPair DefaultKey { get; set; } = new(
            new BigInteger("10315955513017997681600210131013411322695824559688299373570246338038100843097466504032586443986679280716603540690692615875074465586629501752500179100369237", 10),
            new BigInteger("17", 10),
            new BigInteger("4854567300243763614870687120476899445974505675147434999327174747312047455575182761195687859800492317495944895566174677168271650454805328075020357360662513", 10)
            );

        /// <summary>
        /// How many milliseconds before refreshing the config.
        /// </summary>
        public int RefreshConfigInterval = 5000;

        /// <summary>
        /// Application id.
        /// </summary>
        public List<int> ApplicationIds { get; set; } = new();

        #region DME SCE-RT Service Location
        /// <summary>
        /// IP of the DME.
        /// </summary>
        public string DMEIp { get; set; } = InternetProtocolUtils.TryGetServerIP(out _).Result ? InternetProtocolUtils.GetPublicIPAddress() : InternetProtocolUtils.GetLocalIPAddresses().First().ToString();
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
        /// Seconds between disconnects before the client attempts to reconnect to the proxy server.
        /// </summary>
        public int ClientReconnectInterval { get; set; } = 15;

        /// <summary>
        /// Milliseconds between plugin ticks.
        /// </summary>
        public int PluginTickIntervalMs { get; set; } = 50;

        /// <summary>
        /// Port of the TCP server.
        /// </summary>
        public int TCPPort { get; set; } = 10073;

        /// <summary>
        /// Port to bind the udp server to.
        /// </summary>
        public int UDPPort { get; set; } = 50000;

        /// <summary>
        /// The configuration setting determines whether or not the primary server thread 
        /// will sleep.Sleeps are recommended when running multiple instances of RTIME
        /// services on any given machine or when do any type of development on the same
        /// machine running the game service. Sleeps should be disabled when performance
        /// testing or running single instances of production services.
        /// The DmeServerUseThreads will determine whether or not the server will
        /// be multi-threaded.For most development, running the server as a single
        /// thread (setting DmeServerWorldsPerThread to "0") is recommended.
        /// </summary>
        public bool EnableSleeps = MultiServerLibrary.Extension.Microsoft.Win32API.IsWindows; // (DEFAULT: 1 for Win32; 0 for Linux)
        public bool UseThread = true; // (DEFAULT: 0)

        public bool EnableMedius = true; // (DEFAULT: 1)
        public bool EnforceAuthentication = true; // (DEFAULT: 1)

        public short DmeServerMaxWorld = 25000; //		# (DEFAULT: 10, MAXIMUM 25000)

        /// <summary>
        /// There is no limit to clients per world as far as the service is concerned
        /// except for the maximum number of clients allow by the service (set via the 
        /// DmeServerMaxClients setting in DmeServer.cfg). However, the host may want to 
        /// limit the clients per world to guarantee the server can support some defined 
        /// number of worlds.
        ///</summary>
        public short MaxClientsPerWorld = 256; //			# (DEFAULT 2.10: 32 - DEFAULT: 256)

        /// <summary>
        /// If set to a different than -1 value, this will override the MaxClient
        /// atribute sent via Medius, use at your own risk!
        ///</summary>
        public int MaxClientsOverride = -1;
    }

    public class MASSettings
    {
        /// <summary>
        /// Ip of the Medius Authentication Server.
        /// </summary>
        public string Ip { get; set; } = InternetProtocolUtils.GetLocalIPAddresses().First().ToString();

        /// <summary>
        /// The port that the Proxy Server is bound to.
        /// </summary>
        public int Port { get; set; } = 10075;

        /// <summary>
        /// Key used to establish initial handshake with MPS.
        /// </summary>
        public RsaKeyPair Key { get; set; } = new RsaKeyPair(
            new BigInteger("10315955513017997681600210131013411322695824559688299373570246338038100843097466504032586443986679280716603540690692615875074465586629501752500179100369237"),
            new BigInteger("17"),
            new BigInteger("4854567300243763614870687120476899445974505675147434999327174747312047455575182761195687859800492317495944895566174677168271650454805328075020357360662513")
            );
    }

    public class MPSSettings
    {
        /// <summary>
        /// Ip of the Medius Proxy Server.
        /// </summary>
        public string Ip { get; set; } = InternetProtocolUtils.GetLocalIPAddresses().First().ToString();

        /// <summary>
        /// The port that the Proxy Server is bound to.
        /// </summary>
        public int Port { get; set; } = 10077;

        /// <summary>
        /// Key used to establish initial handshake with MPS.
        /// </summary>
        public RsaKeyPair Key { get; set; } = new(
            new BigInteger("10315955513017997681600210131013411322695824559688299373570246338038100843097466504032586443986679280716603540690692615875074465586629501752500179100369237"),
            new BigInteger("17"),
            new BigInteger("4854567300243763614870687120476899445974505675147434999327174747312047455575182761195687859800492317495944895566174677168271650454805328075020357360662513")
            );
    }
}