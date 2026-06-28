using System.Net;
using System.Text.Json;
using MultiServerLibrary.Extension;
using MultiServerLibrary.HTTP;

namespace MultiServerLibrary.GeoLocalization
{
    public class VpnChecker(string ipqsApiKey)
    {
        private readonly string ipQualityScoreKey = ipqsApiKey;

        public bool IsVpnOrProxy(string ip)
        {
            const string fallbackHttpData = "{}";

            try
            {
                if (!InternetProtocolUtils.IsPrivate(IPAddress.Parse(ip)))
                {
                    using var ipApiJson = JsonDocument.Parse(
                        HTTPProcessor.RequestURLGET(
                            $"http://ip-api.com/json/{ip}?fields=as,isp,org,proxy,hosting",
                            true
                        ) ?? fallbackHttpData
                    );

                    var hosting = ipApiJson.RootElement.GetProperty("hosting").GetBoolean();
                    var proxy = ipApiJson.RootElement.GetProperty("proxy").GetBoolean();

                    if (hosting || proxy)
                    {
                        CustomLogger.LoggerAccessor.LogError(
                            $"[VpnChecker] - ip-api flagged {ip} (hosting={hosting}, proxy={proxy})"
                        );
                        return true;
                    }

                    using var ipqsJson = JsonDocument.Parse(
                        HTTPProcessor.RequestURLGET(
                            $"https://ipqualityscore.com/api/json/ip/{ipQualityScoreKey}/{ip}",
                            true
                        ) ?? fallbackHttpData
                    );

                    var vpn = ipqsJson.RootElement.GetProperty("vpn").GetBoolean();
                    var proxy2 = ipqsJson.RootElement.GetProperty("proxy").GetBoolean();
                    var tor = ipqsJson.RootElement.GetProperty("tor").GetBoolean();

                    if (vpn || proxy2 || tor)
                    {
                        CustomLogger.LoggerAccessor.LogError(
                            $"[VpnChecker] - IPQS flagged {ip} (VPN={vpn}, Proxy={proxy2}, Tor={tor})"
                        );
                        return true;
                    }
                }
#if DEBUG
                CustomLogger.LoggerAccessor.LogInfo($"[VpnChecker] - {ip} is OK.");
#endif
            }
            catch (Exception ex)
            {
                CustomLogger.LoggerAccessor.LogError(
                    $"[VpnChecker] - an assertion was thrown while checking VPNs for ip:{ip}. (Exception:{ex})"
                );
            }

            return false;
        }
    }
}
