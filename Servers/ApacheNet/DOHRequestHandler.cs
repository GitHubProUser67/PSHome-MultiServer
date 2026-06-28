using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using ApacheNet.Models;
using CustomLogger;
using DNSLibrary;
using MultiServerLibrary.AdBlocker;
using MultiServerLibrary.Extension;

namespace ApacheNet
{
    public static class DOHRequestHandler
    {
        public static AdGuardFilterChecker AdChecker { get; set; } = new AdGuardFilterChecker();
        public static DanPollockChecker DanChecker { get; set; } = new DanPollockChecker();

        private static readonly UdpClientService _udpClientService = new(
            (int)TimeSpan.FromSeconds(5).TotalMilliseconds,
            (int)TimeSpan.FromSeconds(15).TotalMilliseconds,
            Environment.ProcessorCount * 4
        );

        public static async Task<bool> DohRequest(ApacheContext ctx, string Accept, bool get)
        {
            if (get)
            {
                var acceptsDoH = false;

                if (string.IsNullOrEmpty(Accept))
                    acceptsDoH = true;
                else
                {
                    foreach (var mediaType in Accept.Split(','))
                    {
                        if (
                            mediaType.Equals(
                                "application/dns-message",
                                StringComparison.OrdinalIgnoreCase
                            )
                        )
                        {
                            acceptsDoH = true;
                            break;
                        }
                    }
                }

                if (!SecureDNSConfigProcessor.Initiated || !ctx.Secure)
                {
                    ctx.StatusCode = HttpStatusCode.MethodNotAllowed;
                    return await ctx.SendImmediate("DNS system not enabled or initializing")
                        .ConfigureAwait(false);
                }
                else if (!acceptsDoH)
                {
                    ctx.StatusCode = HttpStatusCode.BadRequest;
                    return await ctx.SendImmediate("Bad Request").ConfigureAwait(false);
                }
                else
                {
                    var dnsRequestBase64Url = ctx.Request.Query.Elements["dns"];
                    if (string.IsNullOrEmpty(dnsRequestBase64Url))
                    {
                        ctx.StatusCode = HttpStatusCode.BadRequest;
                        return await ctx.SendImmediate("Bad Request").ConfigureAwait(false);
                    }
                    else
                    {
                        //convert from base64url to base64
                        dnsRequestBase64Url = dnsRequestBase64Url.Replace('-', '+');
                        dnsRequestBase64Url = dnsRequestBase64Url.Replace('_', '/');

                        //add padding
                        var x = dnsRequestBase64Url.Length % 4;
                        if (x > 0)
                            dnsRequestBase64Url = dnsRequestBase64Url.PadRight(
                                dnsRequestBase64Url.Length - x + 4,
                                '='
                            );

                        var treated = false;

                        try
                        {
                            var DnsReq = dnsRequestBase64Url.IsBase64().DecodedBytes;
                            var Req = Request.FromArray(DnsReq);

                            if (Req.OperationCode == OperationCode.Query)
                            {
                                var question = Req.Questions.FirstOrDefault();

                                if (question == null)
                                {
                                    ctx.StatusCode = HttpStatusCode.BadRequest;
                                    return await ctx.SendImmediate("Bad Request")
                                        .ConfigureAwait(false);
                                }
                                else
                                {
                                    var fullname = question.Name.ToString();

                                    LoggerAccessor.LogInfo(
                                        $"[HTTPS_DNS] - Host: {fullname} was Requested."
                                    );

                                    string? url = null;

                                    if (
                                        fullname.EndsWith(
                                            ".in-addr.arpa",
                                            StringComparison.OrdinalIgnoreCase
                                        )
                                    )
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
                                    else if (
                                        fullname.EndsWith(
                                            ".ip6.arpa",
                                            StringComparison.OrdinalIgnoreCase
                                        )
                                    )
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
                                            ApacheNetServerConfiguration.EnableAdguardFiltering
                                            && AdChecker.isLoaded
                                            && AdChecker.IsDomainRefused(fullname)
                                        )
                                        {
                                            url = "0.0.0.0";
                                            treated = true;
                                        }
                                        else if (
                                            ApacheNetServerConfiguration.EnableDanPollockHosts
                                            && DanChecker.isLoaded
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
                                            && SecureDNSConfigProcessor.DicRules != null
                                            && SecureDNSConfigProcessor.DicRules.TryGetValue(
                                                fullname,
                                                out var value
                                            )
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

                                        if (!treated && SecureDNSConfigProcessor.StarRules != null)
                                        {
                                            foreach (var rule in SecureDNSConfigProcessor.StarRules)
                                            {
                                                Regex regex = new(rule.Key);
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

                                    if (
                                        !treated
                                        && ApacheNetServerConfiguration.DNSAllowUnsafeRequests
                                    )
                                    {
#if DEBUG
                                        LoggerAccessor.LogInfo(
                                            $"[HTTPS_DNS] - Issuing mitm request for domain: {fullname}"
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
                                                            udpClient.Client.RemoteEndPoint
                                                            as IPEndPoint;
#if DEBUG
                                                        LoggerAccessor.LogInfo(
                                                            $"[HTTPS_DNS] - Recieved message from endpoint:{remoteEP}, returning..."
                                                        );
#endif
                                                        DnsReq = udpClient.EndReceive(
                                                            res,
                                                            ref remoteEP
                                                        );
                                                    }
                                                    else
                                                    {
                                                        error = true;
                                                        LoggerAccessor.LogWarn(
                                                            $"[HTTPS_DNS] - No Bytes Recieved from UdpRequest."
                                                        );

                                                        DnsReq = null;
                                                    }
                                                }
                                                else
                                                {
                                                    error = true;
                                                    LoggerAccessor.LogWarn(
                                                        $"[HTTPS_DNS] - No Bytes Sent from UdpRequest."
                                                    );
                                                }
                                            }
                                            catch
                                            {
                                                error = true;
                                                DnsReq = null;
                                            }
                                            finally
                                            {
                                                _udpClientService.ReturnToQueue(udpClient, error);
                                            }
                                        }
                                        else
                                            DnsReq = null;
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
                                                    var (Success, Result) =
                                                        await Dns.GetHostEntryAsync(url)
                                                            .TryAwaitWithResult(
                                                                TimeSpan.FromSeconds(5)
                                                            )
                                                            .ConfigureAwait(false);

                                                    if (Success)
                                                    {
                                                        foreach (
                                                            var extractedIp in Result.AddressList
                                                        )
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
                                                $"[HTTPS_DNS] - Resolved: {fullname} to: {string.Join(", ", Ips)}"
                                            );
#endif
                                        }
                                        else
                                            LoggerAccessor.LogWarn(
                                                $"[HTTPS_DNS] - No domain found for: {fullname}"
                                            );

                                        DnsReq = Response.MakeType0DnsResponsePacket(
                                            DnsReq.Trim(),
                                            Ips
                                        );
                                    }

                                    if (DnsReq != null)
                                    {
                                        ctx.StatusCode = HttpStatusCode.OK;
                                        ctx.Response.ContentType = "application/dns-message";
                                        return await ctx.SendImmediate(
                                                DnsReq,
                                                ApacheContext.AcceptChunked
                                            )
                                            .ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        ctx.StatusCode = HttpStatusCode.InternalServerError;
                                        return await ctx.SendImmediate().ConfigureAwait(false);
                                    }
                                }
                            }
                            else
                            {
                                LoggerAccessor.LogWarn(
                                    $"[HTTPS_DNS] - The requested OperationCode: {Req.OperationCode} is not yet supported, report to GITHUB!"
                                );

                                ctx.StatusCode = HttpStatusCode.NotImplemented;
                                return await ctx.SendImmediate().ConfigureAwait(false);
                            }
                        }
                        catch (Exception e)
                        {
                            LoggerAccessor.LogError(
                                $"[HTTPS_DNS] - An assertion was thrown, not returning any results. (Exception:{e})"
                            );

                            ctx.StatusCode = HttpStatusCode.InternalServerError;
                            return await ctx.SendImmediate().ConfigureAwait(false);
                        }
                    }
                }
            }
            else
            {
                if (!SecureDNSConfigProcessor.Initiated || !ctx.Secure)
                {
                    ctx.StatusCode = HttpStatusCode.MethodNotAllowed;
                    return await ctx.SendImmediate("DNS system not enabled or initializing")
                        .ConfigureAwait(false);
                }
                else if (
                    !string.Equals(
                        ctx.Request.ContentType,
                        "application/dns-message",
                        StringComparison.OrdinalIgnoreCase
                    )
                )
                {
                    ctx.StatusCode = HttpStatusCode.UnsupportedMediaType;
                    return await ctx.SendImmediate("Unsupported Media Type").ConfigureAwait(false);
                }
                else
                {
                    var treated = false;

                    try
                    {
                        var DnsReq = ctx.Request.DataAsBytes;
                        var Req = Request.FromArray(DnsReq);

                        if (Req.OperationCode == OperationCode.Query)
                        {
                            var question = Req.Questions.FirstOrDefault();

                            if (question == null)
                            {
                                ctx.StatusCode = HttpStatusCode.BadRequest;
                                return await ctx.SendImmediate("Bad Request").ConfigureAwait(false);
                            }
                            else
                            {
                                var fullname = question.Name.ToString();

                                LoggerAccessor.LogInfo(
                                    $"[HTTPS_DNS] - Host: {fullname} was Requested."
                                );

                                string? url = null;

                                if (
                                    fullname.EndsWith(
                                        ".in-addr.arpa",
                                        StringComparison.OrdinalIgnoreCase
                                    )
                                )
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
                                else if (
                                    fullname.EndsWith(
                                        ".ip6.arpa",
                                        StringComparison.OrdinalIgnoreCase
                                    )
                                )
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
                                        ApacheNetServerConfiguration.EnableAdguardFiltering
                                        && AdChecker.isLoaded
                                        && AdChecker.IsDomainRefused(fullname)
                                    )
                                    {
                                        url = "0.0.0.0";
                                        treated = true;
                                    }
                                    else if (
                                        ApacheNetServerConfiguration.EnableDanPollockHosts
                                        && DanChecker.isLoaded
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
                                        && SecureDNSConfigProcessor.DicRules != null
                                        && SecureDNSConfigProcessor.DicRules.TryGetValue(
                                            fullname,
                                            out var value
                                        )
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

                                    if (!treated && SecureDNSConfigProcessor.StarRules != null)
                                    {
                                        foreach (var rule in SecureDNSConfigProcessor.StarRules)
                                        {
                                            Regex regex = new(rule.Key);
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

                                if (!treated && ApacheNetServerConfiguration.DNSAllowUnsafeRequests)
                                {
#if DEBUG
                                    LoggerAccessor.LogInfo(
                                        $"[HTTPS_DNS] - Issuing mitm request for domain: {fullname}"
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
                                                        udpClient.Client.RemoteEndPoint
                                                        as IPEndPoint;
#if DEBUG
                                                    LoggerAccessor.LogInfo(
                                                        $"[HTTPS_DNS] - Recieved message from endpoint:{remoteEP}, returning..."
                                                    );
#endif
                                                    DnsReq = udpClient.EndReceive(
                                                        res,
                                                        ref remoteEP
                                                    );
                                                }
                                                else
                                                {
                                                    error = true;
                                                    LoggerAccessor.LogWarn(
                                                        $"[HTTPS_DNS] - No Bytes Recieved from UdpRequest."
                                                    );

                                                    DnsReq = null;
                                                }
                                            }
                                            else
                                            {
                                                error = true;
                                                LoggerAccessor.LogWarn(
                                                    $"[HTTPS_DNS] - No Bytes Sent from UdpRequest."
                                                );
                                            }
                                        }
                                        catch
                                        {
                                            error = true;
                                            DnsReq = null;
                                        }
                                        finally
                                        {
                                            _udpClientService.ReturnToQueue(udpClient, error);
                                        }
                                    }
                                    else
                                        DnsReq = null;
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
                                                var (Success, Result) = await Dns.GetHostEntryAsync(
                                                        url
                                                    )
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
                                            $"[HTTPS_DNS] - Resolved: {fullname} to: {string.Join(", ", Ips)}"
                                        );
#endif
                                    }
                                    else
                                        LoggerAccessor.LogWarn(
                                            $"[HTTPS_DNS] - No domain found for: {fullname}"
                                        );

                                    DnsReq = Response.MakeType0DnsResponsePacket(
                                        DnsReq.Trim(),
                                        Ips
                                    );
                                }

                                if (DnsReq != null)
                                {
                                    ctx.StatusCode = HttpStatusCode.OK;
                                    ctx.Response.ContentType = "application/dns-message";
                                    return await ctx.SendImmediate(
                                            DnsReq,
                                            ApacheContext.AcceptChunked
                                        )
                                        .ConfigureAwait(false);
                                }
                                else
                                {
                                    ctx.StatusCode = HttpStatusCode.InternalServerError;
                                    return await ctx.SendImmediate().ConfigureAwait(false);
                                }
                            }
                        }
                        else
                        {
                            LoggerAccessor.LogWarn(
                                $"[HTTPS_DNS] - The requested OperationCode: {Req.OperationCode} is not yet supported, report to GITHUB!"
                            );

                            ctx.StatusCode = HttpStatusCode.NotImplemented;
                            return await ctx.SendImmediate().ConfigureAwait(false);
                        }
                    }
                    catch (Exception e)
                    {
                        LoggerAccessor.LogError(
                            $"[HTTPS_DNS] - An assertion was thrown, not returning any results. (Exception:{e})"
                        );

                        ctx.StatusCode = HttpStatusCode.InternalServerError;
                        return await ctx.SendImmediate().ConfigureAwait(false);
                    }
                }
            }
        }
    }
}
