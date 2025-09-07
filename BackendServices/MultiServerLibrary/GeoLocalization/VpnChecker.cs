using System;
using System.Net;
using System.Text.Json;
using MultiServerLibrary.Extension;
using MultiServerLibrary.HTTP;

namespace MultiServerLibrary.GeoLocalization
{
    public class VpnChecker
    {
        private readonly string ipQualityScoreKey;

        public VpnChecker(string ipqsApiKey)
        {
            ipQualityScoreKey = ipqsApiKey;
        }

        public bool IsVpnOrProxy(string ip)
        {
            const string fallbackHttpData = "{}";

            try
            {
                if (!InternetProtocolUtils.IsPrivate(IPAddress.Parse(ip)))
                {
                    using JsonDocument ipApiJson = JsonDocument.Parse(HTTPProcessor.RequestURLGET($"http://ip-api.com/json/{ip}?fields=as,isp,org,proxy,hosting") ?? fallbackHttpData);

                    bool hosting = ipApiJson.RootElement.GetProperty("hosting").GetBoolean();
                    bool proxy = ipApiJson.RootElement.GetProperty("proxy").GetBoolean();

                    if (hosting || proxy)
                    {
                        CustomLogger.LoggerAccessor.LogError($"[VpnChecker] - ip-api flagged {ip} (hosting={hosting}, proxy={proxy})");
                        return true;
                    }

                    using JsonDocument ipqsJson = JsonDocument.Parse(HTTPProcessor.RequestURLGET($"https://ipqualityscore.com/api/json/ip/{ipQualityScoreKey}/{ip}") ?? fallbackHttpData);

                    bool vpn = ipqsJson.RootElement.GetProperty("vpn").GetBoolean();
                    bool proxy2 = ipqsJson.RootElement.GetProperty("proxy").GetBoolean();
                    bool tor = ipqsJson.RootElement.GetProperty("tor").GetBoolean();

                    if (vpn || proxy2 || tor)
                    {
                        CustomLogger.LoggerAccessor.LogError($"[VpnChecker] - IPQS flagged {ip} (VPN={vpn}, Proxy={proxy2}, Tor={tor})");
                        return true;
                    }
                }
#if DEBUG
                CustomLogger.LoggerAccessor.LogInfo($"[VpnChecker] - {ip} is OK.");
#endif
            }
            catch (Exception ex)
            {
                 CustomLogger.LoggerAccessor.LogError($"[VpnChecker] - an assertion was thrown while checking VPNs for ip:{ip}. (Exception:{ex})");
            }

            return false;
        }
    }
}
