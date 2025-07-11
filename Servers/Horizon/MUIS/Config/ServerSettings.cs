using Org.BouncyCastle.Math;
using Horizon.RT.Cryptography.RSA;
using MultiServerLibrary.Extension;

namespace Horizon.MUIS.Config
{
    public class ServerSettings
    {
        /// <summary>
        /// How many milliseconds before refreshing the config.
        /// </summary>
        public int RefreshConfigInterval = 5000;

        /// <summary>
        /// Ports of the MUIS server.
        /// Default is 10071, EU is 10080, and Japan is 10101
        /// </summary>
        public int[] Ports { get; set; } = new int[] { 10071, 10080, 10101 };

        #region MUIS SCE-RT Service Location
        /// <summary>
        /// IP of the MUIS.
        /// </summary>
        public string MUISIp { get; set; } = InternetProtocolUtils.TryGetServerIP(out _).Result ? InternetProtocolUtils.GetPublicIPAddress() : InternetProtocolUtils.GetLocalIPAddresses().First().ToString();
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

        public List<int> CompatibleApplicationIds { get; set; } = new();

        #region NAT SCE-RT Service Location
        /// <summary>
        /// Ip address of the NAT server.
        /// Provide the IP of the SCE-RT NAT Service
        /// Default is: natservice.pdonline.scea.com:10070
        /// </summary>
        public string? NATIp { get; set; } = InternetProtocolUtils.GetFirstActiveIPAddress("natservice.pdonline.scea.com", "34.199.94.233");
        /// <summary>
        /// Port of the NAT server.
        /// Provide the Port of the SCE-RT NAT Service
        /// </summary>
        public int NATPort { get; set; } = 10070;
        #endregion

        public bool MediusServerVersionOverride { get; set; } = false;

        public string MUISVersion { get; set; } = "Medius Universe Information Server Version 3.05.0000";

        #region Remote Log Viewer Port To Listen
        /// <summary>
        /// Any value greater than 0 will enable remote logging with the SCE-RT logviewer
        /// on that port, which must not be in use by other applications (default 0)
        /// </summary>
        public int RemoteLogViewPort = 0;
        #endregion

        /// <summary>
        /// Key used to authenticate clients.
        /// </summary>
        public RsaKeyPair DefaultKey { get; set; } = new(
            new BigInteger("10315955513017997681600210131013411322695824559688299373570246338038100843097466504032586443986679280716603540690692615875074465586629501752500179100369237", 10),
            new BigInteger("17", 10),
            new BigInteger("4854567300243763614870687120476899445974505675147434999327174747312047455575182761195687859800492317495944895566174677168271650454805328075020357360662513", 10)
            );

        /// <summary>
        /// Whether or not to encrypt messages.
        /// </summary>
        public bool EncryptMessages { get; set; } = true;

        /// <summary>
        /// Universes.
        /// </summary>
        public Dictionary<int, UniverseInfo[]> Universes { get; set; } = new();
    }

    public class UniverseInfo
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Status { get; set; }
        public int UserCount { get; set; }
        public int MaxUsers { get; set; }
        public string? Endpoint { get; set; }
        public string? SvoURL { get; set; }
        public string? ExtendedInfo { get; set; }
        public string? UniverseBilling { get; set; }
        public string? BillingSystemName { get; set; }
        public int Port { get; set; }
        public uint UniverseId { get; set; }
    }
}