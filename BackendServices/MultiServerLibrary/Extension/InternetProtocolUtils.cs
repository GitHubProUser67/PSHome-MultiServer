using EndianTools;
using NetHasher.CRC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MultiServerLibrary.Extension
{
    public static class InternetProtocolUtils
    {
        private static readonly object _TryGetIpLock = new object();
        private static readonly object _PublicIpLock = new object();

        private static readonly TimedDictionary<byte, (bool, string)> _InternalIpCache = new TimedDictionary<byte, (bool, string)>();

        /// <summary>
        /// Returns true if the IP address is in a private range.<br/>
        /// IPv4: Loopback, link local ("169.254.x.x"), class A ("10.x.x.x"), class B ("172.16.x.x" to "172.31.x.x") and class C ("192.168.x.x").<br/>
        /// IPv6: Loopback, link local, site local, unique local and private IPv4 mapped to IPv6.<br/>
        /// </summary>
        /// <param name="ip">The IP address.</param>
        /// <returns>True if the IP address was in a private range.</returns>
        /// <example><code>bool isPrivate = IPAddress.Parse("127.0.0.1").IsPrivate();</code></example>
        public static bool IsPrivate(this IPAddress ip)
        {
            // Map back to IPv4 if mapped to IPv6, for example "::ffff:1.2.3.4" to "1.2.3.4".
            if (ip.IsIPv4MappedToIPv6)
                ip = ip.MapToIPv4();

            // Checks loopback ranges for both IPv4 and IPv6.
            if (IPAddress.IsLoopback(ip)) return true;

            byte[] bytes = ip.GetAddressBytes();

            // IPv4
            if (ip.AddressFamily == AddressFamily.InterNetwork)
                return IsPrivateIPv4(bytes);

            // IPv6
            if (ip.AddressFamily == AddressFamily.InterNetworkV6)
            {
                return ip.IsIPv6LinkLocal ||
#if NET6_0_OR_GREATER
                       ip.IsIPv6UniqueLocal ||
#else
                       (bytes[0] & 0xfe) == 0xfc || 
#endif
                       ip.IsIPv6SiteLocal;
            }

            CustomLogger.LoggerAccessor.LogError($"[InternetProtocolUtils] - IsPrivate: IP address family {ip.AddressFamily}" +
                $" is not supported, expected only IPv4 (InterNetwork) or IPv6 (InterNetworkV6).");

            return false;
        }

        public static bool IsZeroIpv4Address(IPAddress address)
        {
#if NETCOREAPP3_0_OR_GREATER
            byte[] bytes = address.GetAddressBytes();
            if (bytes.Length != 4) return false; // Only handle IPv4 here

            return BitOperations.PopCount(BitConverter.ToUInt32(!BitConverter.IsLittleEndian ? EndianUtils.EndianSwap(bytes) : bytes, 0)) == 0;
#else
            return address.AddressFamily == AddressFamily.InterNetwork && IPAddress.Any == address;
#endif
        }

        /// <summary>
        /// Get the public IP of the server.
        /// <para>Obtiens l'IP publique du server.</para>
        /// </summary>
        /// <param name="allowipv6">Allow IPV6 format.</param>
        /// <param name="ipv6urlformat">Format the IPV6 result in a url compatible format ([addr]).</param>
        /// <returns>A nullable string.</returns>
        public static string GetPublicIPAddress(bool allowipv6 = false, bool ipv6urlformat = false)
        {
            const string primaryUrl = "https://icanhazip.com/";
            const string primaryIpv4Url = "https://ipv4.icanhazip.com/";
            const string fallbackUrl = "https://api6.ipify.org";
            const string fallbackIpv4Url = "https://api4.ipify.org";

            string result = null;
            byte cacheKey = CRC8.Create(Encoding.UTF8.GetBytes($"Public{allowipv6}{ipv6urlformat}"));

            lock (_PublicIpLock)
            {
                string cacheEntry = _InternalIpCache.Get(cacheKey).Item2;
                if (cacheEntry != null)
                    return cacheEntry;

                string[] urlList = new string[]
                {
                    allowipv6 ? primaryUrl : primaryIpv4Url,
                    allowipv6 ? fallbackUrl : fallbackIpv4Url
                };

                foreach (string url in urlList)
                {
                    try
                    {
#pragma warning disable
                        using (FixedWebClientWithTimeout client = new FixedWebClientWithTimeout())
                        {
                            result = client.DownloadString(url)
                                .Replace("\r\n", string.Empty).Replace("\n", string.Empty).Trim();

                            if (ipv6urlformat && allowipv6 && result.Length > 15)
                                result = $"[{result}]";

                            break; // Successful response
                        }
#pragma warning restore
                    }
                    catch
                    {
                    }
                }

                if (!string.IsNullOrEmpty(result))
                    _InternalIpCache.Set(cacheKey, (true, result), 60000);
            }

            return result;
        }

        /// <summary>
		/// Get all server IP addresses
		/// </summary>
		/// <returns>All IPv4/IPv6 addresses of this machine</returns>
		public static IPAddress[] GetLocalIPAddresses(bool allowipv6 = false)
        {
            List<IPAddress> IPs = new List<IPAddress>();
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                try
                {
                    foreach ((NetworkInterface Netif, UnicastIPAddressInformation ipa) in
                                     from NetworkInterface Netif in NetworkInterface.GetAllNetworkInterfaces()
                                     .Where(item => item.OperationalStatus == OperationalStatus.Up)
                                     from ipa in Netif.GetIPProperties().UnicastAddresses
                                     select (Netif, ipa))
                    {
                        if (ipa.Address.AddressFamily == AddressFamily.InterNetwork || (allowipv6 && ipa.Address.AddressFamily == AddressFamily.InterNetworkV6))
                            IPs.Add(ipa.Address);
                    }
                }
                catch
                {
                    // On Android 13+ the GetAllNetworkInterfaces() may not work and throw NetworkInformationException or something.
                    // http://www.win3x.org/win3board/viewtopic.php?p=206998#p206998
                    // https://www.cyberforum.ru/xamarin/thread3032822.html
                    // https://stackoverflow.com/questions/6803073/get-local-ip-address/27376368#27376368
                    // Not well tested.
                    try
                    {
                        using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                        {
                            socket.Connect("8.8.8.8", 65530);
                            IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                            IPAddress ipa = endPoint.Address;
                            if (!allowipv6 && ipa.AddressFamily == AddressFamily.InterNetworkV6)
                                ipa = ipa.MapToIPv4();
                            IPs.Add(ipa);
                        }
                    }
                    catch { }
                }
            }
            IPs.Add(IPAddress.Parse("10.0.2.2")); //QEMU, SheepShaver, Basilisk II emulators host system IP address (SLIRP)
            return IPs.ToArray();
        }

        private static bool IsPrivateIPv4(byte[] ipv4Bytes)
        {
            // Link local (no IP assigned by DHCP): 169.254.0.0 to 169.254.255.255 (169.254.0.0/16)
            bool IsLinkLocal() => ipv4Bytes[0] == 169 && ipv4Bytes[1] == 254;
            // Class A private range: 10.0.0.0 � 10.255.255.255 (10.0.0.0/8)
            bool IsClassA() => ipv4Bytes[0] == 10;
            // Class B private range: 172.16.0.0 � 172.31.255.255 (172.16.0.0/12)
            bool IsClassB() => ipv4Bytes[0] == 172 && ipv4Bytes[1] >= 16 && ipv4Bytes[1] <= 31;
            // Class C private range: 192.168.0.0 � 192.168.255.255 (192.168.0.0/16)
            bool IsClassC() => ipv4Bytes[0] == 192 && ipv4Bytes[1] == 168;
            // Carrier Grade NAT (used by ISPs and VPNs): 100.64.0.0/10
            bool IsCarrierGradeNat() => ipv4Bytes[0] == 100 && ipv4Bytes[1] >= 64 && ipv4Bytes[1] <= 127;

            return IsLinkLocal() || IsClassA() || IsClassC() || IsClassB() || IsCarrierGradeNat();
        }

        public static Task<bool> TryGetServerIP(out string extractedIP, bool allowipv6 = false)
        {
            bool isPublic;

            if (!MultiServerLibraryConfiguration.EnableServerIpAutoNegotiation)
            {
                isPublic = MultiServerLibraryConfiguration.UsePublicIp;
                extractedIP = isPublic ? GetPublicIPAddress(allowipv6) ?? MultiServerLibraryConfiguration.FallbackServerIp : GetLocalIPAddresses(allowipv6).First().ToString();
                return Task.FromResult(!IPAddress.Parse(extractedIP).IsPrivate());
            }
            else
                isPublic = false;

            string ServerIP;
            byte cacheKey = CRC8.Create(Encoding.UTF8.GetBytes($"Neg{allowipv6}"));

            lock (_TryGetIpLock)
            {
                (bool, string) cacheEntry = _InternalIpCache.Get(cacheKey);

                if (cacheEntry == default)
                {
                    const ushort testPort = ushort.MaxValue;
                    TcpListener listener = null;

                    try
                    {
                        listener = new TcpListener(IPAddress.Any, testPort);
                        listener.Start();

                        if (allowipv6)
                        {
                            // We want to check if the router allows external IPs first.
                            ServerIP = GetPublicIPAddress(true);
                            try
                            {
                                using (TcpClient client = new TcpClient(ServerIP, testPort))
                                    client.Close();
                                isPublic = true;
                            }
                            catch // Failed to connect to public ip, so we fallback to IPV4 Public IP.
                            {
                                ServerIP = GetPublicIPAddress();
                                try
                                {
                                    using (TcpClient client = new TcpClient(ServerIP, testPort))
                                        client.Close();
                                    isPublic = true;
                                }
                                catch // Failed to connect to public ip, so we fallback to local IP.
                                {
                                    ServerIP = GetLocalIPAddresses(true).First().ToString();

                                    try
                                    {
                                        using (TcpClient client = new TcpClient(ServerIP, testPort))
                                            client.Close();
                                    }
                                    catch // Failed to connect to local ip, trying IPV4 only as a last resort.
                                    {
                                        ServerIP = GetLocalIPAddresses().First().ToString();
                                    }
                                }
                            }
                        }
                        else
                        {
                            // We want to check if the router allows external IPs first.
                            ServerIP = GetPublicIPAddress();
                            try
                            {
                                using (TcpClient client = new TcpClient(ServerIP, testPort))
                                    client.Close();
                                isPublic = true;
                            }
                            catch // Failed to connect to public ip, so we fallback to local IP.
                            {
                                ServerIP = GetLocalIPAddresses().First().ToString();
                            }
                        }
                    }
                    catch
                    {
                        ServerIP = MultiServerLibraryConfiguration.FallbackServerIp;
                        isPublic = !IPAddress.Parse(ServerIP).IsPrivate();
                    }
                    finally
                    {
                        if (listener != null)
                            listener.Stop();

                        if (listener != null)
                            listener = null;
                    }

                    if (!string.IsNullOrEmpty(ServerIP))
                        _InternalIpCache.Set(cacheKey, (isPublic, ServerIP), 60000);
                }
                else
                {
                    ServerIP = cacheEntry.Item2;
                    isPublic = cacheEntry.Item1;
                }
            }

            extractedIP = ServerIP;

            return Task.FromResult(isPublic);
        }

        /// <summary>
        /// Get the first active IP of a given domain.
        /// <para>Obtiens la premi�re IP active disponible d'un domaine.</para>
        /// </summary>
        /// <param name="hostName">The domain on which we search.</param>
        /// <param name="fallback">The fallback IP if we fail to find any results</param>
        /// <returns>A string.</returns>
        public static string GetFirstActiveIPAddress(string hostName, string fallback)
        {
            try
            {
                return Dns.GetHostEntry(hostName).AddressList.FirstOrDefault()?.ToString() ?? fallback;
            }
            catch
            {
                // Not Important.
            }

            return fallback;
        }

        public static uint GetIPAddressAsUInt(string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress))
                throw new ArgumentException(nameof(ipAddress));

            return GetIPAddressAsUInt(IPAddress.Parse(ipAddress));
        }

        public static uint GetIPAddressAsUInt(IPAddress ipAddress)
        {
            if (ipAddress == null)
                throw new ArgumentException(nameof(ipAddress));

            byte[] bytes = ipAddress.GetAddressBytes();
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }

        public static IPAddress GetIPAddressFromUInt(uint address)
        {
            byte[] bytes = BitConverter.GetBytes(address);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return new IPAddress(bytes);
        }

        public static long GetIPAddressAsLong(IPAddress ip)
        {
            byte[] bytes = ip.GetAddressBytes();

            if (bytes.Length != 4)
                throw new ArgumentException("[InternetProtocolUtils] - GetIPAddressAsLong: Only IPv4 addresses are supported.");

            long value = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                value = (value << 8) + bytes[i];
            }

            return value;
        }
    }
}
