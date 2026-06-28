using System.Net;
using ApacheNet.Models;
using CustomLogger;

namespace ApacheNet.BuildIn.RouteHandlers
{
    public class Main
    {
        public static List<Route> index =
        [
            new()
            {
                Name = "Server shutdown endpoint",
                UrlRegex = "^/shutdown$",
                Method = "GET",
                Hosts = null,
                Callable = (ctx) =>
                {
                    var ipAddr = ctx.Request.Source.IpAddress;
                    if (
                        !string.IsNullOrEmpty(ipAddr)
                        && (
                            (
                                ApacheNetServerConfiguration.AllowedManagementIPs != null
                                && ApacheNetServerConfiguration.AllowedManagementIPs.Contains(
                                    ipAddr
                                )
                            )
                            || "::1".Equals(ipAddr)
                            || "127.0.0.1".Equals(ipAddr)
                            || "localhost".Equals(
                                ipAddr,
                                StringComparison.InvariantCultureIgnoreCase
                            )
                        )
                    )
                    {
                        LoggerAccessor.LogWarn(
                            $"[Main] - Allowed IP:{ipAddr} issued a server shutdown command at:{DateTime.Now}."
                        );
                        ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                        ctx.Response.Send("Shutdown initiated.").Wait();
                        LoggerAccessor.LogInfo("Shutting down. Goodbye!");
                        Environment.Exit(0);
                    }
                    LoggerAccessor.LogError(
                        $"[Main] - IP:{ipAddr} tried to issue a server shutdown command at:{DateTime.Now}, but this is not allowed for this address!"
                    );
                    ctx.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    ctx.Response.Send().Wait();
                    return true;
                },
            },
            new()
            {
                Name = "Server reboot endpoint",
                UrlRegex = "^/reboot$",
                Method = "GET",
                Hosts = null,
                Callable = (ctx) =>
                {
                    var ipAddr = ctx.Request.Source.IpAddress;
                    if (
                        !string.IsNullOrEmpty(ipAddr)
                        && (
                            (
                                ApacheNetServerConfiguration.AllowedManagementIPs != null
                                && ApacheNetServerConfiguration.AllowedManagementIPs.Contains(
                                    ipAddr
                                )
                            )
                            || "::1".Equals(ipAddr)
                            || "127.0.0.1".Equals(ipAddr)
                            || "localhost".Equals(
                                ipAddr,
                                StringComparison.InvariantCultureIgnoreCase
                            )
                        )
                    )
                    {
                        LoggerAccessor.LogWarn(
                            $"[Main] - Allowed IP:{ipAddr} issued a server reboot command at:{DateTime.Now}."
                        );
                        ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                        ctx.Response.Send("Reboot initiated.").Wait();
                        _ = Task.Run(() =>
                        {
                            LoggerAccessor.LogInfo("Rebooting!");

                            ApacheNetServerConfiguration.RefreshVariables(Program.configPath);

                            Program.StartOrUpdateServer();
                        });
                        return true;
                    }
                    LoggerAccessor.LogError(
                        $"[Main] - IP:{ipAddr} tried to issue a server reboot command at:{DateTime.Now}, but this is not allowed for this address!"
                    );
                    ctx.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    ctx.Response.Send().Wait();
                    return true;
                },
            },
        ];
    }
}
