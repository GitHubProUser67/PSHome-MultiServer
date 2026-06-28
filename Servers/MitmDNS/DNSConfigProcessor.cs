using System.Collections.Concurrent;
using System.Net;
using System.Text.RegularExpressions;
using CustomLogger;
using MultiServerLibrary.Extension;
using MultiServerLibrary.HTTP;

namespace MitmDNS
{
    public static partial class DNSConfigProcessor
    {
        public static ConcurrentDictionary<string, DnsSettings> DicRules = new();
        public static ConcurrentDictionary<string, DnsSettings> StarRules = new();
        public static bool Initiated = false;
        public static IPAddress ServerIp;

        public static void InitDNSSubsystem()
        {
            Initiated = false;

            LoggerAccessor.LogWarn(
                "[DNS] - DNS system configuration is initialising, endpoints will be available when initialized..."
            );

            InternetProtocolUtils.TryGetServerIP(out var ServerIpStr).Wait();
            ServerIp = IPAddress.Parse(ServerIpStr);

            if (!string.IsNullOrEmpty(MitmDNSServerConfiguration.DNSOnlineConfig))
            {
                LoggerAccessor.LogInfo("[DNS] - Downloading Configuration File...");
                var onlineConfig = HTTPProcessor.RequestURLGET(
                    MitmDNSServerConfiguration.DNSOnlineConfig
                );
                if (!string.IsNullOrEmpty(onlineConfig))
                    ParseRules(onlineConfig, false);
            }
            else if (File.Exists(MitmDNSServerConfiguration.DNSConfig))
                ParseRules(MitmDNSServerConfiguration.DNSConfig);

            Initiated = true;
        }

        private static void ParseRules(string Filename, bool IsFilename = true)
        {
            DicRules.Clear();
            StarRules.Clear();

            LoggerAccessor.LogInfo("[DNS] - Parsing Configuration File...");

            if (
                IsFilename
                && Path.GetFileNameWithoutExtension(Filename)
                    .Equals("boot", StringComparison.CurrentCultureIgnoreCase)
            )
                ParseSimpleDNSRules(Filename);
            else
            {
                Parallel.ForEach(
                    IsFilename
                        ? File.ReadAllLines(Filename)
                        : Filename.Split(["\r\n", "\n"], StringSplitOptions.None),
                    s =>
                    {
                        if (s.StartsWith(";") || s.Trim() == string.Empty) { }
                        else
                        {
                            var split = s.Split(',');

                            if (split.Length == 3)
                            {
                                DnsSettings dns = new();
                                switch (split[1].Trim().ToLower())
                                {
                                    case "deny":
                                        dns.Mode = HandleMode.Deny;
                                        break;
                                    case "allow":
                                        dns.Mode = HandleMode.Allow;
                                        break;
                                    case "redirect":
                                        dns.Mode = HandleMode.Redirect;
                                        dns.Address = GetIp(split[2].Trim());
                                        break;
                                    default:
                                        LoggerAccessor.LogWarn(
                                            $"[DNS] - Rule : {s} is not a formated properly, skipping..."
                                        );
                                        break;
                                }

                                var domain = split[0].Trim();

                                // Check if the domain has been processed before
                                if (domain.Contains('*'))
                                {
                                    // Escape all possible URI characters conflicting with Regex
                                    domain = domain.Replace(".", "\\.");
                                    domain = domain.Replace("$", "\\$");
                                    domain = domain.Replace("[", "\\[");
                                    domain = domain.Replace("]", "\\]");
                                    domain = domain.Replace("(", "\\(");
                                    domain = domain.Replace(")", "\\)");
                                    domain = domain.Replace("+", "\\+");
                                    domain = domain.Replace("?", "\\?");
                                    // Replace "*" characters with ".*" which means any number of any character for Regexp
                                    domain = domain.Replace("*", ".*");

                                    StarRules.TryAdd(domain, dns);
                                }
                                else
                                {
                                    DicRules.TryAdd(domain, dns);
                                    DicRules.TryAdd("www." + domain, dns);
                                }
                            }
                            else
                                LoggerAccessor.LogWarn(
                                    $"[DNS] - Rule : {s} is not a formated properly, skipping..."
                                );
                        }
                    }
                );
            }

            LoggerAccessor.LogInfo(
                "[DNS] - "
                    + DicRules.Count.ToString()
                    + " dictionary rules and "
                    + StarRules.Count.ToString()
                    + " star rules loaded"
            );
        }

        private static void ParseSimpleDNSRules(string Filename)
        {
            // Read all lines from the test file
            var lines = File.ReadAllLines(Filename);

            // Define a list to store extracted hostnames
            List<string> hostnames = [];

            foreach (var line in lines)
            {
                // Split the line by tab character
                var parts = line.Split('\t');

                // Check if the line has enough parts and the primary entry is not empty
                if (parts.Length >= 2 && !string.IsNullOrWhiteSpace(parts[1]))
                    // Add the hostname to the list
                    hostnames.Add(parts[1].Trim());
            }

            DnsSettings dns = new();

            Parallel.ForEach(
                hostnames,
                hostname =>
                {
                    var dnsFilePath = Path.GetDirectoryName(Filename) + $"/{hostname}.dns";

                    // Check if the .dns file exists
                    if (File.Exists(dnsFilePath))
                    {
                        foreach (var line in File.ReadAllLines(dnsFilePath))
                        {
                            if (line.StartsWith("\t\tA"))
                            {
                                // Extract the IP address using a regular expression
                                var match = SimpleDNSRegex().Match(line);
                                if (match.Success)
                                {
                                    dns.Mode = HandleMode.Redirect;
                                    dns.Address = GetIp(match.Groups[1].Value);

                                    DicRules.TryAdd(hostname, dns);
                                    DicRules.TryAdd("www." + hostname, dns);

                                    break;
                                }
                            }
                        }
                    }
                }
            );
        }

        #region GetIP
        private static string GetIp(string ip)
        {
            IPAddress IP;

            switch (Uri.CheckHostName(ip))
            {
                case UriHostNameType.IPv4:
                {
                    IP = IPAddress.Parse(ip).MapToIPv4();
                    break;
                }
                case UriHostNameType.IPv6:
                {
                    IP = IPAddress.Parse(ip).MapToIPv6();
                    break;
                }
                case UriHostNameType.Dns:
                {
                    try
                    {
                        IP =
                            Dns.GetHostAddresses(ip).FirstOrDefault()?.MapToIPv4()
                            ?? IPAddress.Loopback;
                    }
                    catch
                    {
                        IP = ServerIp;
                    }
                    break;
                }
                default:
                {
                    IP = ServerIp;
                    LoggerAccessor.LogError(
                        $"Unhandled UriHostNameType {Uri.CheckHostName(ip)} from {ip} in MitmDNSClass.GetIp()"
                    );
                    break;
                }
            }

            return IP.ToString();
        }
        #endregion

        [GeneratedRegex("A\\s+(\\S+)")]
        private static partial Regex SimpleDNSRegex();
    }
}
