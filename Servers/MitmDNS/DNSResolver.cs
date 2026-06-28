using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using CustomLogger;
using DNSLibrary;
using MultiServerLibrary.AdBlocker;
using MultiServerLibrary.Extension;

namespace MitmDNS
{
    public static class DNSResolver
    {
        public static string ServerIp = "127.0.0.1";

        public static AdGuardFilterChecker AdChecker { get; set; } = new AdGuardFilterChecker();
        public static DanPollockChecker DanChecker { get; set; } = new DanPollockChecker();

        private static readonly UdpClientService _udpClientService = new(
            (int)TimeSpan.FromSeconds(5).TotalMilliseconds,
            (int)TimeSpan.FromSeconds(15).TotalMilliseconds,
            Environment.ProcessorCount * 4
        );

        public static async Task<byte[]> ProcRequest(byte[] DnsReq)
        {
            var treated = false;

            try
            {
                var Req = Request.FromArray(DnsReq);

                if (Req.OperationCode == OperationCode.Query)
                {
                    var question = Req.Questions.FirstOrDefault();

                    if (question == null)
                        return null;

                    var fullname = question.Name.ToString();

                    LoggerAccessor.LogInfo($"[DNSResolver] - Host: {fullname} was Requested.");

                    string url = null;

                    if (fullname.EndsWith(".in-addr.arpa", StringComparison.OrdinalIgnoreCase))
                    {
                        var ipPart = fullname[..^13];

                        if (
                            IPAddress.TryParse(ipPart, out var ipv4)
                            && ipv4.AddressFamily == AddressFamily.InterNetwork
                        )
                        {
                            var octets = ipv4.ToString().Split('.');
                            Array.Reverse(octets);

                            url = string.Join(".", octets);
                            treated = true;
                        }
                    }
                    else if (fullname.EndsWith(".ip6.arpa", StringComparison.OrdinalIgnoreCase))
                    {
                        var nibblePart = fullname[..^9];

                        // remove dots
                        var hexReversed = nibblePart.Replace(".", string.Empty);

                        var isHexValid = true;

                        // validate hex
                        foreach (var c in hexReversed)
                        {
                            if (!Uri.IsHexDigit(c))
                            {
                                isHexValid = false;
                                break;
                            }
                        }

                        if (isHexValid && hexReversed.Length <= 32)
                        {
                            // pad missing leading zeros (zone delegation support)
                            hexReversed = hexReversed.PadRight(32, '0');

                            // reverse nibbles
                            var chars = hexReversed.ToCharArray();
                            Array.Reverse(chars);
                            var hex = new string(chars);

                            var bytes = new byte[16];
                            for (var i = 0; i < 16; i++)
                                bytes[i] = byte.Parse(
                                    hex.Substring(i * 2, 2),
                                    NumberStyles.HexNumber
                                );

                            var ipv6 = new IPAddress(bytes);

                            if (ipv6.AddressFamily == AddressFamily.InterNetworkV6)
                            {
                                url = ipv6.ToString();
                                treated = true;
                            }
                        }
                    }
                    else
                    {
                        if (
                            MitmDNSServerConfiguration.EnableAdguardFiltering
                            && AdChecker.isLoaded
                            && AdChecker.IsDomainRefused(fullname)
                        )
                        {
                            url = "0.0.0.0";
                            treated = true;
                        }
                        else if (
                            MitmDNSServerConfiguration.EnableDanPollockHosts && DanChecker.isLoaded
                        )
                        {
                            var danAddr = DanChecker.GetDomainIP(fullname);
                            if (danAddr != null)
                            {
                                url = danAddr.ToString();
                                treated = true;
                            }
                        }

                        if (
                            !treated
                            && DNSConfigProcessor.DicRules != null
                            && DNSConfigProcessor.DicRules.TryGetValue(fullname, out var value)
                        )
                        {
                            if (value.Mode == HandleMode.Allow)
                                url = fullname;
                            else if (value.Mode == HandleMode.Redirect)
                                url = value.Address ?? "127.0.0.1";
                            else if (value.Mode == HandleMode.Deny)
                                url = "NXDOMAIN";
                            treated = true;
                        }

                        if (!treated && DNSConfigProcessor.StarRules != null)
                        {
                            foreach (var rule in DNSConfigProcessor.StarRules)
                            {
                                var regex = new Regex(rule.Key);
                                if (!regex.IsMatch(fullname))
                                    continue;

                                if (rule.Value.Mode == HandleMode.Allow)
                                    url = fullname;
                                else if (rule.Value.Mode == HandleMode.Redirect)
                                    url = rule.Value.Address ?? "127.0.0.1";
                                else if (rule.Value.Mode == HandleMode.Deny)
                                    url = "NXDOMAIN";
                                treated = true;
                                break;
                            }
                        }
                    }

                    if (!treated && MitmDNSServerConfiguration.DNSAllowUnsafeRequests)
                    {
#if DEBUG
                        LoggerAccessor.LogInfo(
                            $"[DNSResolver] - Issuing mitm request for domain: {fullname}"
                        );
#endif
                        var queueRes = _udpClientService.TryDequeue();
                        if (queueRes.Item1)
                        {
                            var error = false;
                            var udpClient = queueRes.Item2;
                            try
                            {
                                if (
                                    await udpClient
                                        .SendAsync(DnsReq, DnsReq.Length)
                                        .TryAwait(
                                            TimeSpan.FromMilliseconds(
                                                _udpClientService.SendTimeoutMs
                                            )
                                        )
                                        .ConfigureAwait(false)
                                )
                                {
                                    var res = udpClient.BeginReceive(null, null);
                                    // begin recieve right after request
                                    if (
                                        res.AsyncWaitHandle.WaitOne(
                                            _udpClientService.ReceiveTimeoutMs
                                        )
                                    )
                                    {
                                        var remoteEP =
                                            udpClient.Client.RemoteEndPoint as IPEndPoint;
#if DEBUG
                                        LoggerAccessor.LogInfo(
                                            $"[DNSResolver] - Recieved message from endpoint:{remoteEP}, returning..."
                                        );
#endif
                                        return udpClient.EndReceive(res, ref remoteEP);
                                    }
                                    else
                                    {
                                        error = true;
                                        LoggerAccessor.LogWarn(
                                            $"[DNSResolver] - No Bytes Recieved from UdpRequest."
                                        );
                                    }
                                }
                                else
                                {
                                    error = true;
                                    LoggerAccessor.LogWarn(
                                        $"[DNSResolver] - No Bytes Sent from UdpRequest."
                                    );
                                }
                            }
                            catch
                            {
                                error = true;
                            }
                            finally
                            {
                                _udpClientService.ReturnToQueue(udpClient, error);
                            }
                        }
                    }
                    else
                    {
                        List<IPAddress> Ips = [];

                        if (!string.IsNullOrEmpty(url) && url != "NXDOMAIN")
                        {
                            try
                            {
                                if (!IPAddress.TryParse(url, out var address))
                                {
                                    var (Success, Result) = await Dns.GetHostEntryAsync(url)
                                        .TryAwaitWithResult(TimeSpan.FromSeconds(5))
                                        .ConfigureAwait(false);

                                    if (Success)
                                    {
                                        foreach (var extractedIp in Result.AddressList)
                                            Ips.Add(extractedIp);
                                    }
                                }
                                else
                                    Ips.Add(address);
                            }
                            catch
                            {
                                Ips.Clear();
                            }
#if DEBUG
                            LoggerAccessor.LogInfo(
                                $"[DNSResolver] - Resolved: {fullname} to: {string.Join(", ", Ips)}"
                            );
#endif
                        }
                        else
                            LoggerAccessor.LogWarn(
                                $"[DNSResolver] - No domain found for: {fullname}"
                            );

                        return Response.MakeType0DnsResponsePacket(DnsReq.Trim(), Ips);
                    }
                }
                else
                    LoggerAccessor.LogWarn(
                        $"[DNSResolver] - The requested OperationCode: {Req.OperationCode} is not yet supported, report to GITHUB!"
                    );
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(
                    $"[DNSResolver] - An assertion was thrown, not returning any results. (Exception:{e})"
                );
            }

            return null;
        }
    }
}
