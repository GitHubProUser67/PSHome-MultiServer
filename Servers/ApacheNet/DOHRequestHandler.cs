using ApacheNet.Models;
using CustomLogger;
using DNS.Protocol;
using DNSLibrary;
using MultiServerLibrary.AdBlocker;
using MultiServerLibrary.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ApacheNet
{
    public static class DOHRequestHandler
    {
        public static AdGuardFilterChecker AdChecker { get; set; } = new AdGuardFilterChecker();
        public static DanPollockChecker DanChecker { get; set; } = new DanPollockChecker();

        private static readonly UdpClientService _udpClientService = new UdpClientService(
                    (int)TimeSpan.FromSeconds(5).TotalMilliseconds,
                    (int)TimeSpan.FromSeconds(15).TotalMilliseconds,
                    Environment.ProcessorCount * 4);

        public static async Task<bool> DohRequest(ApacheContext ctx, string Accept, bool get)
        {
            if (get)
            {
                bool acceptsDoH = false;

                if (string.IsNullOrEmpty(Accept))
                    acceptsDoH = true;
                else
                {
                    foreach (string mediaType in Accept.Split(','))
                    {
                        if (mediaType.Equals("application/dns-message", StringComparison.OrdinalIgnoreCase))
                        {
                            acceptsDoH = true;
                            break;
                        }
                    }
                }

                if (!SecureDNSConfigProcessor.Initiated || !ctx.Secure)
                {
                    ctx.StatusCode = HttpStatusCode.MethodNotAllowed;
                    return await ctx.SendImmediate("DNS system not enabled or initializing").ConfigureAwait(false);
                }
                else if (!acceptsDoH)
                {
                    ctx.StatusCode = HttpStatusCode.BadRequest;
                    return await ctx.SendImmediate("Bad Request").ConfigureAwait(false);
                }
                else
                {
                    string? dnsRequestBase64Url = ctx.Request.Query.Elements["dns"];
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
                        int x = dnsRequestBase64Url.Length % 4;
                        if (x > 0)
                            dnsRequestBase64Url = dnsRequestBase64Url.PadRight(dnsRequestBase64Url.Length - x + 4, '=');

                        bool treated = false;

                        try
                        {
                            byte[]? DnsReq = dnsRequestBase64Url.IsBase64().Item2;
                            Request Req = Request.FromArray(DnsReq);

                            if (Req.OperationCode == OperationCode.Query)
                            {
                                Question? question = Req.Questions.FirstOrDefault();

                                if (question == null)
                                {
                                    ctx.StatusCode = HttpStatusCode.BadRequest;
                                    return await ctx.SendImmediate("Bad Request").ConfigureAwait(false);
                                }
                                else
                                {
                                    string fullname = question.Name.ToString();

                                    LoggerAccessor.LogInfo($"[HTTPS_DNS] - Host: {fullname} was Requested.");

                                    string? url = null;

                                    if (fullname.Length > 13 && fullname.EndsWith("in-addr.arpa") && IPAddress.TryParse(fullname[..^13], out IPAddress? arparuleaddr)) // IPV4 Only.
                                    {
                                        if (arparuleaddr != null && arparuleaddr.AddressFamily == AddressFamily.InterNetwork)
                                        {
                                            // Split the IP address into octets
                                            string[] octets = arparuleaddr.ToString().Split('.');

                                            // Reverse the order of octets
                                            Array.Reverse(octets);

                                            // Join the octets back together
                                            url = string.Join(".", octets);

                                            treated = true;
                                        }
                                    }
                                    else
                                    {
                                        if (ApacheNetServerConfiguration.EnableAdguardFiltering && AdChecker.isLoaded && AdChecker.IsDomainRefused(fullname))
                                        {
                                            url = "0.0.0.0";
                                            treated = true;
                                        }
                                        else if (ApacheNetServerConfiguration.EnableDanPollockHosts && DanChecker.isLoaded)
                                        {
                                            IPAddress danAddr = DanChecker.GetDomainIP(fullname);
                                            if (danAddr != null)
                                            {
                                                url = danAddr.ToString();
                                                treated = true;
                                            }
                                        }

                                        if (!treated && SecureDNSConfigProcessor.DicRules != null && SecureDNSConfigProcessor.DicRules.TryGetValue(fullname, out DnsSettings value))
                                        {
                                            if (value.Mode == HandleMode.Allow) url = fullname;
                                            else if (value.Mode == HandleMode.Redirect) url = value.Address ?? "127.0.0.1";
                                            else if (value.Mode == HandleMode.Deny) url = "NXDOMAIN";
                                            treated = true;
                                        }

                                        if (!treated && SecureDNSConfigProcessor.StarRules != null)
                                        {
                                            foreach (KeyValuePair<string, DnsSettings> rule in SecureDNSConfigProcessor.StarRules)
                                            {
                                                Regex regex = new(rule.Key);
                                                if (!regex.IsMatch(fullname))
                                                    continue;

                                                if (rule.Value.Mode == HandleMode.Allow) url = fullname;
                                                else if (rule.Value.Mode == HandleMode.Redirect) url = rule.Value.Address ?? "127.0.0.1";
                                                else if (rule.Value.Mode == HandleMode.Deny) url = "NXDOMAIN";
                                                treated = true;
                                                break;
                                            }
                                        }
                                    }

                                    if (!treated && ApacheNetServerConfiguration.DNSAllowUnsafeRequests)
                                    {
#if DEBUG
                                        LoggerAccessor.LogInfo($"[HTTPS_DNS] - Issuing mitm request for domain: {fullname}");
#endif
                                        var queueRes = _udpClientService.Dequeue();
                                        if (queueRes.Item1)
                                        {
                                            bool error = false;
                                            var udpClient = queueRes.Item2;
                                            try
                                            {
                                                await udpClient.SendAsync(DnsReq, DnsReq.Length).ConfigureAwait(false);

                                                var res = udpClient.BeginReceive(null, null);
                                                // begin recieve right after request
                                                if (res.AsyncWaitHandle.WaitOne(_udpClientService.SendTimeoutMs))
                                                {
                                                    IPEndPoint? remoteEP = udpClient.Client.RemoteEndPoint as IPEndPoint;
#if DEBUG
                                                    LoggerAccessor.LogInfo($"[HTTPS_DNS] - Recieved message from endpoint:{remoteEP}, returning...");
#endif
                                                    DnsReq = udpClient.EndReceive(res, ref remoteEP);
                                                }
                                                else
                                                {
                                                    LoggerAccessor.LogWarn($"[HTTPS_DNS] - No Bytes Recieved from UdpRequest.");

                                                    DnsReq = null;
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
                                        List<IPAddress> Ips = new();

                                        if (!string.IsNullOrEmpty(url) && url != "NXDOMAIN")
                                        {
                                            try
                                            {
                                                if (!IPAddress.TryParse(url, out IPAddress? address))
                                                {
                                                    foreach (var extractedIp in Dns.GetHostEntry(url).AddressList)
                                                    {
                                                        Ips.Add(extractedIp);
                                                    }
                                                }
                                                else Ips.Add(address);
                                            }
                                            catch
                                            {
                                                Ips.Clear();
                                            }
#if DEBUG
                                            LoggerAccessor.LogInfo($"[HTTPS_DNS] - Resolved: {fullname} to: {string.Join(", ", Ips)}");
#endif
                                            DnsReq = Response.MakeType0DnsResponsePacket(DnsReq.Trim(), Ips);
                                        }
                                        else
                                        {
                                            LoggerAccessor.LogWarn($"[HTTPS_DNS] - No domain found for: {fullname}");

                                            DnsReq = Response.MakeType0DnsResponsePacket(DnsReq.Trim(), Ips);
                                        }
                                    }

                                    if (DnsReq != null)
                                    {
                                        ctx.StatusCode = HttpStatusCode.OK;
                                        ctx.Response.ContentType = "application/dns-message";
                                        return await ctx.SendImmediate(DnsReq, ctx.AcceptChunked).ConfigureAwait(false);
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
                                LoggerAccessor.LogWarn($"[HTTPS_DNS] - The requested OperationCode: {Req.OperationCode} is not yet supported, report to GITHUB!");

                                ctx.StatusCode = HttpStatusCode.NotImplemented;
                                return await ctx.SendImmediate().ConfigureAwait(false);
                            }
                        }
                        catch (Exception e)
                        {
                            LoggerAccessor.LogError($"[HTTPS_DNS] - An assertion was thrown, not returning any results. (Exception:{e})");

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
                    return await ctx.SendImmediate("DNS system not enabled or initializing").ConfigureAwait(false);
                }
                else if (!string.Equals(ctx.Request.ContentType, "application/dns-message", StringComparison.OrdinalIgnoreCase))
                {
                    ctx.StatusCode = HttpStatusCode.UnsupportedMediaType;
                    return await ctx.SendImmediate("Unsupported Media Type").ConfigureAwait(false);
                }
                else
                {
                    bool treated = false;

                    try
                    {
                        byte[]? DnsReq = ctx.Request.DataAsBytes;
                        Request Req = Request.FromArray(DnsReq);

                        if (Req.OperationCode == OperationCode.Query)
                        {
                            Question? question = Req.Questions.FirstOrDefault();

                            if (question == null)
                            {
                                ctx.StatusCode = HttpStatusCode.BadRequest;
                                return await ctx.SendImmediate("Bad Request").ConfigureAwait(false);
                            }
                            else
                            {
                                string fullname = question.Name.ToString();

                                LoggerAccessor.LogInfo($"[HTTPS_DNS] - Host: {fullname} was Requested.");

                                string? url = null;

                                if (fullname.Length > 13 && fullname.EndsWith("in-addr.arpa") && IPAddress.TryParse(fullname[..^13], out IPAddress? arparuleaddr)) // IPV4 Only.
                                {
                                    if (arparuleaddr != null && arparuleaddr.AddressFamily == AddressFamily.InterNetwork)
                                    {
                                        // Split the IP address into octets
                                        string[] octets = arparuleaddr.ToString().Split('.');

                                        // Reverse the order of octets
                                        Array.Reverse(octets);

                                        // Join the octets back together
                                        url = string.Join(".", octets);

                                        treated = true;
                                    }
                                }
                                else
                                {
                                    if (ApacheNetServerConfiguration.EnableAdguardFiltering && AdChecker.isLoaded && AdChecker.IsDomainRefused(fullname))
                                    {
                                        url = "0.0.0.0";
                                        treated = true;
                                    }
                                    else if (ApacheNetServerConfiguration.EnableDanPollockHosts && DanChecker.isLoaded)
                                    {
                                        IPAddress danAddr = DanChecker.GetDomainIP(fullname);
                                        if (danAddr != null)
                                        {
                                            url = danAddr.ToString();
                                            treated = true;
                                        }
                                    }

                                    if (!treated && SecureDNSConfigProcessor.DicRules != null && SecureDNSConfigProcessor.DicRules.TryGetValue(fullname, out DnsSettings value))
                                    {
                                        if (value.Mode == HandleMode.Allow) url = fullname;
                                        else if (value.Mode == HandleMode.Redirect) url = value.Address ?? "127.0.0.1";
                                        else if (value.Mode == HandleMode.Deny) url = "NXDOMAIN";
                                        treated = true;
                                    }

                                    if (!treated && SecureDNSConfigProcessor.StarRules != null)
                                    {
                                        foreach (KeyValuePair<string, DnsSettings> rule in SecureDNSConfigProcessor.StarRules)
                                        {
                                            Regex regex = new(rule.Key);
                                            if (!regex.IsMatch(fullname))
                                                continue;

                                            if (rule.Value.Mode == HandleMode.Allow) url = fullname;
                                            else if (rule.Value.Mode == HandleMode.Redirect) url = rule.Value.Address ?? "127.0.0.1";
                                            else if (rule.Value.Mode == HandleMode.Deny) url = "NXDOMAIN";
                                            treated = true;
                                            break;
                                        }
                                    }
                                }

                                if (!treated && ApacheNetServerConfiguration.DNSAllowUnsafeRequests)
                                {
#if DEBUG
                                    LoggerAccessor.LogInfo($"[HTTPS_DNS] - Issuing mitm request for domain: {fullname}");
#endif
                                    var queueRes = _udpClientService.Dequeue();
                                    if (queueRes.Item1)
                                    {
                                        bool error = false;
                                        var udpClient = queueRes.Item2;
                                        try
                                        {
                                            await udpClient.SendAsync(DnsReq, DnsReq.Length).ConfigureAwait(false);

                                            var res = udpClient.BeginReceive(null, null);
                                            // begin recieve right after request
                                            if (res.AsyncWaitHandle.WaitOne(_udpClientService.SendTimeoutMs))
                                            {
                                                IPEndPoint? remoteEP = udpClient.Client.RemoteEndPoint as IPEndPoint;
#if DEBUG
                                                LoggerAccessor.LogInfo($"[HTTPS_DNS] - Recieved message from endpoint:{remoteEP}, returning...");
#endif
                                                DnsReq = udpClient.EndReceive(res, ref remoteEP);
                                            }
                                            else
                                            {
                                                LoggerAccessor.LogWarn($"[HTTPS_DNS] - No Bytes Recieved from UdpRequest.");

                                                DnsReq = null;
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
                                    List<IPAddress> Ips = new();

                                    if (!string.IsNullOrEmpty(url) && url != "NXDOMAIN")
                                    {
                                        try
                                        {
                                            if (!IPAddress.TryParse(url, out IPAddress? address))
                                            {
                                                foreach (var extractedIp in Dns.GetHostEntry(url).AddressList)
                                                {
                                                    Ips.Add(extractedIp);
                                                }
                                            }
                                            else Ips.Add(address);
                                        }
                                        catch
                                        {
                                            Ips.Clear();
                                        }
#if DEBUG
                                        LoggerAccessor.LogInfo($"[HTTPS_DNS] - Resolved: {fullname} to: {string.Join(", ", Ips)}");
#endif
                                        DnsReq = Response.MakeType0DnsResponsePacket(DnsReq.Trim(), Ips);
                                    }
                                    else
                                    {
                                        LoggerAccessor.LogWarn($"[HTTPS_DNS] - No domain found for: {fullname}");

                                        DnsReq = Response.MakeType0DnsResponsePacket(DnsReq.Trim(), Ips);
                                    }
                                }

                                if (DnsReq != null)
                                {
                                    ctx.StatusCode = HttpStatusCode.OK;
                                    ctx.Response.ContentType = "application/dns-message";
                                    return await ctx.SendImmediate(DnsReq, ctx.AcceptChunked).ConfigureAwait(false);
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
                            LoggerAccessor.LogWarn($"[HTTPS_DNS] - The requested OperationCode: {Req.OperationCode} is not yet supported, report to GITHUB!");

                            ctx.StatusCode = HttpStatusCode.NotImplemented;
                            return await ctx.SendImmediate().ConfigureAwait(false);
                        }
                    }
                    catch (Exception e)
                    {
                        LoggerAccessor.LogError($"[HTTPS_DNS] - An assertion was thrown, not returning any results. (Exception:{e})");

                        ctx.StatusCode = HttpStatusCode.InternalServerError;
                        return await ctx.SendImmediate().ConfigureAwait(false);
                    }
                }
            }
        }
    }
}
