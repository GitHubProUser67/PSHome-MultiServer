using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using CastleLibrary.NetHasher.CRC;
using EndianTools;
using MultiServerLibrary.Extension.NET;

namespace MultiServerLibrary.Extension
{
    public static class InternetProtocolUtils
    {
        private static readonly Lock _TryGetIpLock = new();
        private static readonly Lock _PublicIpLock = new();

        private static readonly TimedDictionary<byte, (bool, string)> _InternalIpCache = new();

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
            if (IPAddress.IsLoopback(ip))
                return true;

            var bytes = ip.GetAddressBytes();

            // IPv4
            if (ip.AddressFamily == AddressFamily.InterNetwork)
                return IsPrivateIPv4(bytes);
            // IPv6
            else if (ip.AddressFamily == AddressFamily.InterNetworkV6)
            {
                return ip.IsIPv6LinkLocal || ip.IsIPv6UniqueLocal || ip.IsIPv6SiteLocal;
            }

            CustomLogger.LoggerAccessor.LogError(
                $"[InternetProtocolUtils] - IsPrivate: IP address family {ip.AddressFamily}"
                    + $" is not supported, expected only IPv4 (InterNetwork) or IPv6 (InterNetworkV6)."
            );

            return false;
        }

        public static bool IsZeroIpv4Address(IPAddress address)
        {
            var bytes = address.GetAddressBytes();
            if (bytes.Length != 4)
                return false; // Only handle IPv4 here

            return BitOperations.PopCount(
                    BitConverter.ToUInt32(
                        !EndianAwareConverter.isLittleEndianSystem
                            ? EndianUtils.EndianSwap(bytes)
                            : bytes,
                        0
                    )
                ) == 0;
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
            var cacheKey = CRC8.Create(Encoding.UTF8.GetBytes($"Public{allowipv6}{ipv6urlformat}"));

            lock (_PublicIpLock)
            {
                var cacheEntry = _InternalIpCache.Get(cacheKey).Item2;
                if (cacheEntry != null)
                    return cacheEntry;

                var urlList = new string[]
                {
                    allowipv6 ? primaryUrl : primaryIpv4Url,
                    allowipv6 ? fallbackUrl : fallbackIpv4Url,
                };

                foreach (var url in urlList)
                {
                    try
                    {
#pragma warning disable
                        using (FixedWebClientWithTimeout client = new FixedWebClientWithTimeout())
                        {
                            result = client
                                .DownloadString(url)
                                .Replace("\r\n", string.Empty)
                                .Replace("\n", string.Empty)
                                .Trim();

                            if (ipv6urlformat && allowipv6 && result.Length > 15)
                                result = $"[{result}]";

                            break; // Successful response
                        }
#pragma warning restore
                    }
                    catch
                    {
                        // Not Important.
                    }
                }

                if (!string.IsNullOrEmpty(result))
                    _InternalIpCache.Set(cacheKey, (true, result), 60000);
            }

            return result;
        }

        /// <summary>
        /// Gets the preferred outbound local IP addresses used by the OS routing table.
        /// </summary>
        /// <returns>
        /// The IPv4 address and optionally IPv6 address that would be used
        /// for outbound network traffic.
        /// </returns>
        public static IPAddress[] GetOutboundIPAddresses(bool allowipv6 = false)
        {
            var hasSocketResult = false;
            var ips = new List<IPAddress>();

            try
            {
                using var s4 = new Socket(
                    AddressFamily.InterNetwork,
                    SocketType.Dgram,
                    ProtocolType.Udp
                );
                s4.Connect("8.8.8.8", 65530);

                if (s4.LocalEndPoint is IPEndPoint ep4)
                {
                    ips.Add(ep4.Address);
                    hasSocketResult = true;
                }
            }
            catch
            {
                // Not Important.
            }

            if (allowipv6)
            {
                try
                {
                    using var s6 = new Socket(
                        AddressFamily.InterNetworkV6,
                        SocketType.Dgram,
                        ProtocolType.Udp
                    );
                    s6.Connect("2001:4860:4860::8888", 65530);

                    if (s6.LocalEndPoint is IPEndPoint ep6)
                    {
                        ips.Add(ep6.Address);
                        hasSocketResult = true;
                    }
                }
                catch
                {
                    // Not Important.
                }
            }

            if (!hasSocketResult)
            {
                try
                {
                    foreach (
                        (var Netif, var ip) in from NetworkInterface Netif in NetworkInterface
                            .GetAllNetworkInterfaces()
                            .Where(item => item.OperationalStatus == OperationalStatus.Up)
                        from ipa in Netif.GetIPProperties().UnicastAddresses
                        select (Netif, ipa)
                    )
                    {
                        var address = ip.Address;

                        if (
                            (
                                address.AddressFamily == AddressFamily.InterNetwork
                                || (
                                    allowipv6
                                    && address.AddressFamily == AddressFamily.InterNetworkV6
                                )
                            ) && !IPAddress.IsLoopback(address)
                        )
                            ips.Add(address);
                    }
                }
                catch
                {
                    /* On Android 13+ the GetAllNetworkInterfaces() may not work and throw NetworkInformationException or something.
                       http://www.win3x.org/win3board/viewtopic.php?p=206998#p206998
                       https://www.cyberforum.ru/xamarin/thread3032822.html
                       https://stackoverflow.com/questions/6803073/get-local-ip-address/27376368#27376368 */
                }
            }

            ips.Add(IPAddress.Parse("10.0.2.2")); //QEMU, SheepShaver, Basilisk II emulators host system IP address (SLIRP)

            return [.. ips];
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
            bool IsCarrierGradeNat() =>
                ipv4Bytes[0] == 100 && ipv4Bytes[1] >= 64 && ipv4Bytes[1] <= 127;

            return IsLinkLocal() || IsClassA() || IsClassC() || IsClassB() || IsCarrierGradeNat();
        }

        public static bool IsLocalhost(string host)
        {
            if ("localhost".Equals(host, StringComparison.InvariantCultureIgnoreCase))
                return true;
            else if (IPAddress.TryParse(host, out var ip) && ip != null)
                return IPAddress.IsLoopback(ip);

            return false;
        }

        public static Task<bool> TryGetServerIP(out string extractedIP, bool allowipv6 = false)
        {
            if (!MultiServerLibraryConfiguration.EnableServerIpAutoNegotiation)
            {
                if (!string.IsNullOrEmpty(MultiServerLibraryConfiguration.ServerIpOverride))
                    extractedIP = MultiServerLibraryConfiguration.ServerIpOverride;
                else
                {
                    extractedIP = MultiServerLibraryConfiguration.UsePublicIp
                        ? GetPublicIPAddress(allowipv6)
                            ?? MultiServerLibraryConfiguration.FallbackServerPublicIp
                        : GetOutboundIPAddresses(allowipv6).First().ToString();
                    if (string.IsNullOrEmpty(extractedIP))
                        extractedIP = allowipv6
                            ? IPAddress.IPv6Any.ToString()
                            : IPAddress.Any.ToString();
                }

                return Task.FromResult(!IPAddress.Parse(extractedIP).IsPrivate());
            }

            var isPublic = false;

            string serverIP = null;
            var cacheKey = CRC8.Create(Encoding.UTF8.GetBytes($"Neg{allowipv6}"));

            lock (_TryGetIpLock)
            {
                var cacheEntry = _InternalIpCache.Get(cacheKey);

                if (cacheEntry == default)
                {
                    try
                    {
                        // Build candidates in priority order.
                        List<string> candidates = [];

                        if (allowipv6)
                            candidates.Add(GetPublicIPAddress(true));

                        candidates.Add(GetPublicIPAddress());

                        candidates.AddRange(
                            GetOutboundIPAddresses(allowipv6).Select(x => x.ToString())
                        );

                        candidates =
                        [
                            .. candidates.Where(x => !string.IsNullOrEmpty(x)).Distinct(),
                        ];

                        using TcpListener listener = new TcpListener(
                            allowipv6 ? IPAddress.IPv6Any : IPAddress.Any,
                            0
                        );

                        listener.Start();

                        ushort testPort = (ushort)((IPEndPoint)listener.LocalEndpoint).Port;

                        foreach (var candidate in candidates)
                        {
                            try
                            {
                                if (TcpClientUtils.TryConnectAsync(candidate, testPort).Result)
                                {
                                    serverIP = candidate;
#if DEBUG
                                    CustomLogger.LoggerAccessor.LogInfo(
                                        $"[InternetProtocolUtils] - TryGetServerIP: Got valid usable IP from negotiation: {serverIP}"
                                    );
#endif
                                    break;
                                }
                            }
                            catch
                            {
                                // Not Important.
                            }
                        }

                        if (serverIP == null)
                            serverIP = allowipv6
                                ? IPAddress.IPv6Any.ToString()
                                : IPAddress.Any.ToString();
                    }
                    catch (SocketException ex)
                    {
                        CustomLogger.LoggerAccessor.LogWarn(
                            $"[InternetProtocolUtils] - TryGetServerIP: Assertion while trying to initiate the negotiation server, falling back to traditional approach (might be innacurate). (Exception:{ex})"
                        );

                        if (!string.IsNullOrEmpty(MultiServerLibraryConfiguration.ServerIpOverride))
                            serverIP = MultiServerLibraryConfiguration.ServerIpOverride;
                        else
                        {
                            serverIP = MultiServerLibraryConfiguration.UsePublicIp
                                ? GetPublicIPAddress(allowipv6)
                                    ?? MultiServerLibraryConfiguration.FallbackServerPublicIp
                                : GetOutboundIPAddresses(allowipv6).First().ToString();
                            if (string.IsNullOrEmpty(serverIP))
                                serverIP = allowipv6
                                    ? IPAddress.IPv6Any.ToString()
                                    : IPAddress.Any.ToString();
                        }
                    }

                    isPublic = !IPAddress.Parse(serverIP).IsPrivate();

                    _InternalIpCache.Set(cacheKey, (isPublic, serverIP), 60000);

                    extractedIP = serverIP;
                }
                else
                {
                    extractedIP = cacheEntry.Item2;
                    isPublic = cacheEntry.Item1;
                }
            }

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
                var (Success, Result) = Dns.GetHostEntryAsync(hostName)
                    .TryAwaitWithResult(TimeSpan.FromSeconds(5))
                    .Result;

                if (Success)
                    return Result.AddressList.FirstOrDefault()?.ToString() ?? fallback;
            }
            catch
            {
                // Not Important.
            }

            return fallback;
        }

        public static uint GetIPAddressAsUInt(string ipAddress)
        {
            return string.IsNullOrEmpty(ipAddress)
                ? throw new ArgumentException(nameof(ipAddress))
                : GetIPAddressAsUInt(IPAddress.Parse(ipAddress));
        }

        public static uint GetIPAddressAsUInt(IPAddress ipAddress)
        {
            if (ipAddress == null)
                throw new ArgumentException(nameof(ipAddress));

            var bytes = ipAddress.GetAddressBytes();
            if (EndianTools.EndianAwareConverter.isLittleEndianSystem)
                Array.Reverse(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }

        public static IPAddress GetIPAddressFromUInt(uint address)
        {
            var bytes = BitConverter.GetBytes(address);
            if (EndianTools.EndianAwareConverter.isLittleEndianSystem)
                Array.Reverse(bytes);
            return new IPAddress(bytes);
        }
    }
}
