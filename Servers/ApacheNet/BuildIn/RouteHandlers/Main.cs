using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CustomLogger;
using WebAPIService.WebServices.AdobeFlash.binaries.JwPlayer;
using ApacheNet.Models;

namespace ApacheNet.BuildIn.RouteHandlers
{
    public class Main
    {
        public static List<Route> index = new() {
                new() {
                    Name = "Server shutdown endpoint",
                    UrlRegex = "^/shutdown$",
                    Method = "GET",
                    Hosts = null,
                    Callable = (ctx) => {
                        string ipAddr = ctx.Request.Source.IpAddress;
                        if (!string.IsNullOrEmpty(ipAddr) && (ApacheNetServerConfiguration.AllowedManagementIPs != null && ApacheNetServerConfiguration.AllowedManagementIPs.Contains(ipAddr)
                        || "::1".Equals(ipAddr) || "127.0.0.1".Equals(ipAddr) || "localhost".Equals(ipAddr, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            LoggerAccessor.LogWarn($"[Main] - Allowed IP:{ipAddr} issued a server shutdown command at:{DateTime.Now}.");
                            ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                            ctx.Response.Send("Shutdown initiated.").Wait();
                            LoggerAccessor.LogInfo("Shutting down. Goodbye!");
                            Environment.Exit(0);
                        }
                        LoggerAccessor.LogError($"[Main] - IP:{ipAddr} tried to issue a server shutdown command at:{DateTime.Now}, but this is not allowed for this address!");
                        ctx.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        ctx.Response.Send().Wait();
                        return true;
                     }
                },
                new() {
                    Name = "Server reboot endpoint",
                    UrlRegex = "^/reboot$",
                    Method = "GET",
                    Hosts = null,
                    Callable = (ctx) => {
                        string ipAddr = ctx.Request.Source.IpAddress;
                        if (!string.IsNullOrEmpty(ipAddr) && (ApacheNetServerConfiguration.AllowedManagementIPs != null && ApacheNetServerConfiguration.AllowedManagementIPs.Contains(ipAddr)
                        || "::1".Equals(ipAddr) || "127.0.0.1".Equals(ipAddr) || "localhost".Equals(ipAddr, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            LoggerAccessor.LogWarn($"[Main] - Allowed IP:{ipAddr} issued a server reboot command at:{DateTime.Now}.");
                            ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                            ctx.Response.Send("Reboot initiated.").Wait();
                            _ = Task.Run(() => {
                                LoggerAccessor.LogInfo("Rebooting!");

                                ApacheNetServerConfiguration.RefreshVariables(Program.configPath);

                                Program.StartOrUpdateServer();
                            });
                            return true;
                        }
                        LoggerAccessor.LogError($"[Main] - IP:{ipAddr} tried to issue a server reboot command at:{DateTime.Now}, but this is not allowed for this address!");
                        ctx.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        ctx.Response.Send().Wait();
                        return true;
                     }
                },
                new() {
                    Name = "AdobeFlash JW Player",
                    UrlRegex = "jwplayer/player",
                    Method = "GET",
                    Hosts = null,
                    Callable = (ctx) => {
                        ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                        ctx.Response.ContentType = "application/x-shockwave-flash";
                        if (ctx.Request.Url.RawWithoutQuery.EndsWith("53.swf", StringComparison.InvariantCultureIgnoreCase))
                            return ctx.Response.Send(jwPlayer53Swf.Data).Result;
                        else if (ctx.Request.Url.RawWithoutQuery.EndsWith("43.swf", StringComparison.InvariantCultureIgnoreCase))
                            return ctx.Response.Send(jwPlayer43Swf.Data).Result;
                        return ctx.Response.Send(jwPlayer6Swf.Data).Result;
                     }
                },
            };
    }
}
