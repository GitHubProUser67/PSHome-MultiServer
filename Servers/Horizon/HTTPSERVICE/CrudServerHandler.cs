using CustomLogger;
using MultiServerLibrary.GeoLocalization;
using MultiServerLibrary.HTTP;
using Horizon.DME.Extension.PlayStationHome;
using Horizon.SERVER;
using Horizon.SERVER.Extension.PlayStationHome;
using System.Net;
using System.Text;
using WatsonWebserver;
using WatsonWebserver.Core;
using Horizon.MUM.Models;
using Newtonsoft.Json;
using Prometheus;

namespace Horizon.HTTPSERVICE
{
    public class CrudServerHandler
    {
        private static Counter clientsRequests = Metrics.CreateCounter("medius_crud_requests_total", "Total number of Medius CRUD API requests.");

        private Webserver? _Server;
        private string ip;
        private int port;

        public CrudServerHandler(string ip, int port, string certpath = "", string certpass = "")
        {
            this.ip = ip;
            this.port = port;

            WebserverSettings settings = new()
            {
                Hostname = ip,
                Port = port,
            };

            if (!string.IsNullOrEmpty(certpath))
            {
                settings.Ssl.PfxCertificateFile = certpath;
                settings.Ssl.PfxCertificatePassword = certpass;
                settings.Ssl.Enable = true;
            }

            _Server = new Webserver(settings, DefaultRoute);

            StartServer();
        }

        private static async Task AuthorizeConnection(HttpContextBase ctx)
        {
            clientsRequests.Inc();

            string IpToBan = ctx.Request.Source.IpAddress;
            if (!"::1".Equals(IpToBan) && !"127.0.0.1".Equals(IpToBan) && !"localhost".Equals(IpToBan, StringComparison.InvariantCultureIgnoreCase))
            {
                if (!string.IsNullOrEmpty(IpToBan) && MultiServerLibrary.MultiServerLibraryConfiguration.BannedIPs != null && MultiServerLibrary.MultiServerLibraryConfiguration.BannedIPs.Contains(IpToBan))
                {
                    LoggerAccessor.LogError($"[SECURITY] - Client - {ctx.Request.Source.IpAddress}:{ctx.Request.Source.Port} Requested the Medius CRUD API server while being banned!");
                    ctx.Response.StatusCode = 403;
                    await ctx.Response.Send();
                }
            }
        }

        public void StopServer()
        {
            _Server?.Stop();
            _Server?.Dispose();

            LoggerAccessor.LogWarn($"CrudHandler Server on port: {port} stopped...");
        }

        public void StartServer()
        {
            if (_Server != null && !_Server.IsListening)
            {
                _Server.Routes.AuthenticateRequest = AuthorizeConnection;
                _Server.Events.ExceptionEncountered += ExceptionEncountered;
                _Server.Events.Logger = LoggerAccessor.LogInfo;
#if DEBUG
                _Server.Settings.Debug.Responses = true;
                _Server.Settings.Debug.Routing = true;
#endif
                _Server.Routes.PostAuthentication.Parameter.Add(WatsonWebserver.Core.HttpMethod.GET, "/GetRooms/", async (HttpContextBase ctx) =>
                {
                    string userAgent = ctx.Request.Useragent;
                    string clientip = ctx.Request.Source.IpAddress;

                    if ((IPAddress.TryParse(clientip, out IPAddress? clientipAddr) && await HorizonServerConfiguration.Database.GetIsIpBanned(clientipAddr))
                    || (!string.IsNullOrEmpty(userAgent) && userAgent.Contains("bytespider", StringComparison.InvariantCultureIgnoreCase))) // Get Away TikTok and cheaters.
                    {
                        ctx.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        ctx.Response.ContentType = "text/plain";
                        await ctx.Response.Send();
                    }
                    else
                    {
                        string managerPayload = RoomManager.ToJson();

                        ctx.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                        ctx.Response.ContentType = "application/json; charset=UTF-8";
                        ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                        string? encoding = ctx.Request.RetrieveHeaderValue("Accept-Encoding");
                        if (!string.IsNullOrEmpty(encoding))
                        {
                            if (encoding.Contains("zstd"))
                            {
                                ctx.Response.Headers.Add("Content-Encoding", "zstd");
                                await ctx.Response.Send(HTTPProcessor.CompressZstd(Encoding.UTF8.GetBytes(managerPayload)));
                            }
                            else if (encoding.Contains("br"))
                            {
                                ctx.Response.Headers.Add("Content-Encoding", "br");
                                await ctx.Response.Send(HTTPProcessor.CompressBrotli(Encoding.UTF8.GetBytes(managerPayload)));
                            }
                            else if (encoding.Contains("gzip"))
                            {
                                ctx.Response.Headers.Add("Content-Encoding", "gzip");
                                await ctx.Response.Send(HTTPProcessor.CompressGzip(Encoding.UTF8.GetBytes(managerPayload)));
                            }
                            else if (encoding.Contains("deflate"))
                            {
                                ctx.Response.Headers.Add("Content-Encoding", "deflate");
                                await ctx.Response.Send(HTTPProcessor.Deflate(Encoding.UTF8.GetBytes(managerPayload)));
                            }
                            else
                                await ctx.Response.Send(managerPayload);
                        }
                        else
                            await ctx.Response.Send(managerPayload);
                    }
                });

                _Server.Routes.PostAuthentication.Parameter.Add(WatsonWebserver.Core.HttpMethod.GET, "/GetCIDsList/", async (HttpContextBase ctx) =>
                {
                    string userAgent = ctx.Request.Useragent;
                    string clientip = ctx.Request.Source.IpAddress;

                    if ((IPAddress.TryParse(clientip, out IPAddress? clientipAddr) && await HorizonServerConfiguration.Database.GetIsIpBanned(clientipAddr))
                    || (!string.IsNullOrEmpty(userAgent) && userAgent.Contains("bytespider", StringComparison.InvariantCultureIgnoreCase))) // Get Away TikTok and cheaters.
                    {
                        ctx.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        ctx.Response.ContentType = "text/plain";
                        await ctx.Response.Send();
                    }
                    else
                    {
                        bool localhost = false;

                        if ("::1".Equals(clientip) || "127.0.0.1".Equals(clientip) || "localhost".Equals(clientip, StringComparison.InvariantCultureIgnoreCase))
                            localhost = true;

                        ctx.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                        ctx.Response.ContentType = localhost ? "application/json; charset=UTF-8" : "text/plain";
                        ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                        string? encoding = ctx.Request.RetrieveHeaderValue("Accept-Encoding");
                        if (!string.IsNullOrEmpty(encoding))
                        {
                            if (encoding.Contains("zstd"))
                            {
                                ctx.Response.Headers.Add("Content-Encoding", "zstd");
                                await ctx.Response.Send(HTTPProcessor.CompressZstd(Encoding.UTF8.GetBytes(CIDManager.ToJson(!localhost))));
                            }
                            else if (encoding.Contains("br"))
                            {
                                ctx.Response.Headers.Add("Content-Encoding", "br");
                                await ctx.Response.Send(HTTPProcessor.CompressBrotli(Encoding.UTF8.GetBytes(CIDManager.ToJson(!localhost))));
                            }
                            else if (encoding.Contains("gzip"))
                            {
                                ctx.Response.Headers.Add("Content-Encoding", "gzip");
                                await ctx.Response.Send(HTTPProcessor.CompressGzip(Encoding.UTF8.GetBytes(CIDManager.ToJson(!localhost))));
                            }
                            else if (encoding.Contains("deflate"))
                            {
                                ctx.Response.Headers.Add("Content-Encoding", "deflate");
                                await ctx.Response.Send(HTTPProcessor.Deflate(Encoding.UTF8.GetBytes(CIDManager.ToJson(!localhost))));
                            }
                            else
                                await ctx.Response.Send(CIDManager.ToJson(!localhost));
                        }
                        else
                            await ctx.Response.Send(CIDManager.ToJson(!localhost));
                    }
                });

                _Server.Routes.PostAuthentication.Parameter.Add(WatsonWebserver.Core.HttpMethod.GET, "/HomeIGA/{command}/", async (HttpContextBase ctx) =>
                {
                    string? Command = ctx.Request.Url.Parameters["command"];
                    string userAgent = ctx.Request.Useragent;
                    string clientip = ctx.Request.Source.IpAddress;

                    if ((IPAddress.TryParse(clientip, out IPAddress? clientipAddr) && await HorizonServerConfiguration.Database.GetIsIpBanned(clientipAddr))
                    || (!string.IsNullOrEmpty(userAgent) && userAgent.Contains("bytespider", StringComparison.InvariantCultureIgnoreCase))) // Get Away TikTok and cheaters.
                    {
                        ctx.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        ctx.Response.ContentType = "text/plain";
                        await ctx.Response.Send();
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(clientip) && ("::1".Equals(clientip) || "127.0.0.1".Equals(clientip)
                        || "localhost".Equals(clientip, StringComparison.InvariantCultureIgnoreCase) || MediusClass.Settings.PlaystationHomeUsersServersAccessList.Any(entry => entry.Key.Contains($":{clientip}") && "ADMIN".Equals(entry.Value))))
                        {
                            if (!string.IsNullOrEmpty(Command) && ctx.Request.QuerystringExists("DmeId") && short.TryParse(ctx.Request.RetrieveQueryValue("DmeId"), out short DmeId)
                             && ctx.Request.QuerystringExists("WorldId") && int.TryParse(ctx.Request.RetrieveQueryValue("WorldId"), out int WorldId)
                             && ctx.Request.QuerystringExists("DmeWorldId") && int.TryParse(ctx.Request.RetrieveQueryValue("DmeWorldId"), out int DmeWorldId))
                            {
                                bool Retail = true;
                                string result = "Command Unknown!";

                                if (ctx.Request.QuerystringExists("Retail") && bool.TryParse(ctx.Request.RetrieveQueryValue("Retail"), out Retail))
                                {
                                    
                                }

                                LoggerAccessor.LogWarn($"[CrudServerHandler] - client:{clientip}:{ctx.Request.Source.Port} issued command: {Command}");

                                switch (Command)
                                {
                                    case "Kick":
                                        result = NewIGA.KickClient(DmeId, WorldId, DmeWorldId, Retail);
                                        break;
                                    case "Release":
                                        result = NewIGA.ReleaseClient(DmeId, WorldId, DmeWorldId, Retail);
                                        break;
                                    case "Mute":
                                        result = NewIGA.MuteClient(DmeId, WorldId, DmeWorldId, Retail);
                                        break;
                                    case "MuteFreeze":
                                        result = NewIGA.MuteAndFreezeClient(DmeId, WorldId, DmeWorldId, Retail);
                                        break;
                                    case "Freeze":
                                        result = NewIGA.FreezeClient(DmeId, WorldId, DmeWorldId, Retail);
                                        break;
                                    default:
                                        LoggerAccessor.LogWarn($"[CrudServerHandler] - Unknown Home IGA command: {Command}");
                                        break;
                                }

                                ctx.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                ctx.Response.ContentType = "text/plain";
                                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                                string? encoding = ctx.Request.RetrieveHeaderValue("Accept-Encoding");
                                if (!string.IsNullOrEmpty(encoding))
                                {
                                    if (encoding.Contains("zstd"))
                                    {
                                        ctx.Response.Headers.Add("Content-Encoding", "zstd");
                                        await ctx.Response.Send(HTTPProcessor.CompressZstd(Encoding.UTF8.GetBytes(result)));
                                    }
                                    else if (encoding.Contains("br"))
                                    {
                                        ctx.Response.Headers.Add("Content-Encoding", "br");
                                        await ctx.Response.Send(HTTPProcessor.CompressBrotli(Encoding.UTF8.GetBytes(result)));
                                    }
                                    else if (encoding.Contains("gzip"))
                                    {
                                        ctx.Response.Headers.Add("Content-Encoding", "gzip");
                                        await ctx.Response.Send(HTTPProcessor.CompressGzip(Encoding.UTF8.GetBytes(result)));
                                    }
                                    else if (encoding.Contains("deflate"))
                                    {
                                        ctx.Response.Headers.Add("Content-Encoding", "deflate");
                                        await ctx.Response.Send(HTTPProcessor.Deflate(Encoding.UTF8.GetBytes(result)));
                                    }
                                    else
                                        await ctx.Response.Send(result);
                                }
                                else
                                    await ctx.Response.Send(result);
                            }
                            else
                            {
                                ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                                ctx.Response.ContentType = "text/plain";
                                await ctx.Response.Send();
                            }
                        }
                        else if (File.Exists(Directory.GetCurrentDirectory() + "/static/creepy_iga_fallback.mp4"))
                        {
                            byte[] videoData = File.ReadAllBytes(Directory.GetCurrentDirectory() + "/static/creepy_iga_fallback.mp4");

                            ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                            ctx.Response.ContentType = "video/mp4";
                            string? encoding = ctx.Request.RetrieveHeaderValue("Accept-Encoding");
                            if (!string.IsNullOrEmpty(encoding))
                            {
                                if (encoding.Contains("zstd"))
                                {
                                    ctx.Response.Headers.Add("Content-Encoding", "zstd");
                                    await ctx.Response.Send(HTTPProcessor.CompressZstd(videoData));
                                }
                                else if (encoding.Contains("br"))
                                {
                                    ctx.Response.Headers.Add("Content-Encoding", "br");
                                    await ctx.Response.Send(HTTPProcessor.CompressBrotli(videoData));
                                }
                                else if (encoding.Contains("gzip"))
                                {
                                    ctx.Response.Headers.Add("Content-Encoding", "gzip");
                                    await ctx.Response.Send(HTTPProcessor.CompressGzip(videoData));
                                }
                                else if (encoding.Contains("deflate"))
                                {
                                    ctx.Response.Headers.Add("Content-Encoding", "deflate");
                                    await ctx.Response.Send(HTTPProcessor.Deflate(videoData));
                                }
                                else
                                    await ctx.Response.Send(videoData);
                            }
                            else
                                await ctx.Response.Send(videoData);
                        }
                        else
                        {
                            string htmlPayload = "<!DOCTYPE html>\r\n<html lang=\"en\">\r\n<head>\r\n" +
                                "    <meta charset=\"UTF-8\">\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n" +
                                "    <title>DARK WEB</title>\r\n    <style>\r\n        body {\r\n            margin: 0;\r\n            padding: 0;\r\n" +
                                "            display: flex;\r\n            justify-content: center;\r\n            align-items: center;\r\n" +
                                "            height: 100vh;\r\n            background-image: url('https://media1.tenor.com/m/IKo-c45o9XUAAAAC/horror-gif.gif');\r\n" +
                                "            background-size: cover;\r\n            background-position: center;\r\n        }\r\n\r\n        h1 {\r\n" +
                                "            color: red;\r\n            font-size: 100px;\r\n            font-family: 'Creepster', cursive; /* You can link to a scary font if you want */\r\n" +
                                "            text-shadow: 4px 4px 8px black;\r\n        }\r\n    </style>\r\n</head>\r\n<body>\r\n" +
                                "    <iframe width=\"0\" height=\"0\" src=\"https://www.youtube.com/embed/XfQrgDbisAo?autoplay=1&loop=1\"\r\n    frameborder=\"0\" allowfullscreen></iframe>" +
                                $"    <h1>BEWARE! {$"We know your IP: {clientip} and where you live: {await WebLocalization.GetOpenStreetMapUrl(clientip) ?? "Earth"}"}</h1>\r\n</body>\r\n" +
                                "</html>";

                            ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                            ctx.Response.ContentType = "text/html";
                            string? encoding = ctx.Request.RetrieveHeaderValue("Accept-Encoding");
                            if (!string.IsNullOrEmpty(encoding))
                            {
                                if (encoding.Contains("zstd"))
                                {
                                    ctx.Response.Headers.Add("Content-Encoding", "zstd");
                                    await ctx.Response.Send(HTTPProcessor.CompressZstd(Encoding.UTF8.GetBytes(htmlPayload)));
                                }
                                else if (encoding.Contains("br"))
                                {
                                    ctx.Response.Headers.Add("Content-Encoding", "br");
                                    await ctx.Response.Send(HTTPProcessor.CompressBrotli(Encoding.UTF8.GetBytes(htmlPayload)));
                                }
                                else if (encoding.Contains("gzip"))
                                {
                                    ctx.Response.Headers.Add("Content-Encoding", "gzip");
                                    await ctx.Response.Send(HTTPProcessor.CompressGzip(Encoding.UTF8.GetBytes(htmlPayload)));
                                }
                                else if (encoding.Contains("deflate"))
                                {
                                    ctx.Response.Headers.Add("Content-Encoding", "deflate");
                                    await ctx.Response.Send(HTTPProcessor.Deflate(Encoding.UTF8.GetBytes(htmlPayload)));
                                }
                                else
                                    await ctx.Response.Send(htmlPayload);
                            }
                            else
                                await ctx.Response.Send(htmlPayload);
                        }
                    }
                });

                _Server.Routes.PostAuthentication.Parameter.Add(WatsonWebserver.Core.HttpMethod.GET, "/HomeAdminMessage/{region_code}/{message}/", async (HttpContextBase ctx) =>
                {
                    string? region_code = ctx.Request.Url.Parameters["region_code"];
                    string? message = HTTPProcessor.DecodeUrl(ctx.Request.Url.Parameters["message"]);
                    string userAgent = ctx.Request.Useragent;
                    string clientip = ctx.Request.Source.IpAddress;

                    if ((IPAddress.TryParse(clientip, out IPAddress? clientipAddr) && await HorizonServerConfiguration.Database.GetIsIpBanned(clientipAddr))
                    || (!string.IsNullOrEmpty(userAgent) && userAgent.Contains("bytespider", StringComparison.InvariantCultureIgnoreCase))) // Get Away TikTok and cheaters.
                    {
                        ctx.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        ctx.Response.ContentType = "text/plain";
                        await ctx.Response.Send();
                    }
                    else
                    {
                        bool Admin = false;

                        if (!string.IsNullOrEmpty(clientip) && ("::1".Equals(clientip) || "127.0.0.1".Equals(clientip)
                        || "localhost".Equals(clientip, StringComparison.InvariantCultureIgnoreCase) || MediusClass.Settings.PlaystationHomeUsersServersAccessList.Any(entry => entry.Key.Contains($":{clientip}") && "ADMIN".Equals(entry.Value))))
                            Admin = true;

                        if (!Admin)
                        {
                            ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            ctx.Response.ContentType = "text/plain";
                            await ctx.Response.Send();
                            return;
                        }

                        bool Retail = true;
                        bool IsLcCompatible = false;
                        int worldId = -1;
                        string? AccessToken = null;

                        if (ctx.Request.QuerystringExists("Retail") && bool.TryParse(ctx.Request.RetrieveQueryValue("Retail"), out Retail))
                        {

                        }
                        if (ctx.Request.QuerystringExists("Lc") && bool.TryParse(ctx.Request.RetrieveQueryValue("Lc"), out IsLcCompatible))
                        {

                        }
                        if (ctx.Request.QuerystringExists("worldId") && int.TryParse(ctx.Request.RetrieveQueryValue("worldId"), out worldId))
                        {

                        }

                        ctx.Response.StatusCode = (int)HttpStatusCode.OK;

                        if (Admin && ctx.Request.QuerystringExists("BroadcastAcrossEntireUniverse") && bool.TryParse(ctx.Request.RetrieveQueryValue("BroadcastAcrossEntireUniverse"), out bool Broadcast) && Broadcast)
                        {
                            ctx.Response.ContentType = "text/plain; charset=utf-8";

                            await ctx.Response.Send(await HomeServerMessage.BroadcastAdminMessage(region_code, message, IsLcCompatible, Retail) ? "Requested Message sent successfully!" : "Error while sending the Requested Message!");
                        }
                        else
                        {
                            ctx.Response.ContentType = "text/plain; charset=utf-8";

                            if (ctx.Request.QuerystringExists("AccessToken"))
                                AccessToken = HTTPProcessor.DecodeUrl(ctx.Request.RetrieveQueryValue("AccessToken"));

                            await ctx.Response.Send(await HomeServerMessage.SendAdminMessage(clientip, AccessToken, region_code, worldId, message, IsLcCompatible, Retail) ? "Requested Message sent successfully!" : "Error while sending the Requested Message!");
                        }
                    }
                });

                _Server.Routes.PostAuthentication.Parameter.Add(WatsonWebserver.Core.HttpMethod.GET, "/HomeLogOff/{region_code}/{user_name}/", async (HttpContextBase ctx) =>
                {
                    string? region_code = ctx.Request.Url.Parameters["region_code"];
                    string? user_name = HTTPProcessor.DecodeUrl(ctx.Request.Url.Parameters["user_name"]);
                    string userAgent = ctx.Request.Useragent;
                    string clientip = ctx.Request.Source.IpAddress;

                    if ((IPAddress.TryParse(clientip, out IPAddress? clientipAddr) && await HorizonServerConfiguration.Database.GetIsIpBanned(clientipAddr))
                    || (!string.IsNullOrEmpty(userAgent) && userAgent.Contains("bytespider", StringComparison.InvariantCultureIgnoreCase))) // Get Away TikTok and cheaters.
                    {
                        ctx.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        ctx.Response.ContentType = "text/plain";
                        await ctx.Response.Send();
                    }
                    else
                    {
                        bool Retail = true;
                        bool Admin = false;
                        bool IsLcCompatible = false;

                        if (!string.IsNullOrEmpty(clientip) && ("::1".Equals(clientip) || "127.0.0.1".Equals(clientip)
                        || "localhost".Equals(clientip, StringComparison.InvariantCultureIgnoreCase) || MediusClass.Settings.PlaystationHomeUsersServersAccessList.Any(entry => entry.Key.Contains($":{clientip}") && "ADMIN".Equals(entry.Value))))
                            Admin = true;

                        if (!Admin)
                        {
                            ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            ctx.Response.ContentType = "text/plain";
                            await ctx.Response.Send();
                            return;
                        }
                        else if (string.IsNullOrEmpty(user_name))
                        {
                            ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            ctx.Response.ContentType = "text/plain";
                            await ctx.Response.Send("Empty Username parameter!");
                            return;
                        }

                        if (ctx.Request.QuerystringExists("Retail") && bool.TryParse(ctx.Request.RetrieveQueryValue("Retail"), out Retail))
                        {

                        }
                        if (ctx.Request.QuerystringExists("Lc") && bool.TryParse(ctx.Request.RetrieveQueryValue("Lc"), out IsLcCompatible))
                        {

                        }

                        ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                        ctx.Response.ContentType = "text/plain; charset=utf-8";

                        var clientTarget = MediusClass.Manager.GetClientByAccountName(user_name, Retail ? 20374 : 20371);
                        if (clientTarget != null)
                            await ctx.Response.Send(await HomeServerMessage.SendLogOffCommand(clientTarget, region_code, Array.Empty<byte>(), IsLcCompatible) ? "Requested LogOff sent successfully!" : "Error while sending the Requested LogOff!");
                        else
                            await ctx.Response.Send("Requested User is not connected on Home!");
                    }
                });

                _Server.Routes.PostAuthentication.Parameter.Add(WatsonWebserver.Core.HttpMethod.GET, "/HomeRTM/{command}/", async (HttpContextBase ctx) =>
                {
                    string? Command = ctx.Request.Url.Parameters["command"];
                    string userAgent = ctx.Request.Useragent;
                    string clientip = ctx.Request.Source.IpAddress;

                    if ((IPAddress.TryParse(clientip, out IPAddress? clientipAddr) && await HorizonServerConfiguration.Database.GetIsIpBanned(clientipAddr))
                    || (!string.IsNullOrEmpty(userAgent) && userAgent.Contains("bytespider", StringComparison.InvariantCultureIgnoreCase))) // Get Away TikTok and cheaters.
                    {
                        ctx.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        ctx.Response.ContentType = "text/plain";
                        await ctx.Response.Send();
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(Command))
                        {
                            ctx.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            ctx.Response.ContentType = "text/plain";
                            await ctx.Response.Send();
                            return;
                        }
                        else
                            Command = HTTPProcessor.DecodeUrl(Command);

                        bool Retail = true;
                        bool Admin = false;
                        string? AccessToken = null;

                        if (!string.IsNullOrEmpty(clientip) && ("::1".Equals(clientip) || "127.0.0.1".Equals(clientip)
                        || "localhost".Equals(clientip, StringComparison.InvariantCultureIgnoreCase) || MediusClass.Settings.PlaystationHomeUsersServersAccessList.Any(entry => entry.Key.Contains($":{clientip}") && "ADMIN".Equals(entry.Value))))
                            Admin = true;

                        if (ctx.Request.QuerystringExists("Retail") && bool.TryParse(ctx.Request.RetrieveQueryValue("Retail"), out Retail))
                        {

                        }

                        ctx.Response.StatusCode = (int)HttpStatusCode.OK;

                        if (Admin && ctx.Request.QuerystringExists("BroadcastAcrossEntireUniverse") && bool.TryParse(ctx.Request.RetrieveQueryValue("BroadcastAcrossEntireUniverse"), out bool Broadcast) && Broadcast)
                        {
                            if (ctx.Request.QuerystringExists("SupplementalCommands") && !string.IsNullOrEmpty(ctx.Request.RetrieveQueryValue("SupplementalCommands")))
                            {
                                StringBuilder st = new("[");

                                ctx.Response.ContentType = "application/json; charset=utf-8";

                                foreach (string tmpCommand in ctx.Request.RetrieveQueryValue("SupplementalCommands").Split('|'))
                                {
                                    if (st.Length > 1)
                                        st.Append($",\"{tmpCommand}\":\"" + (await HomeRTMTools.BroadcastRemoteCommand(tmpCommand, Retail) ? "Requested Command sent successfully!" : "Error while sending the Requested Command!") + '\"');
                                    else
                                        st.Append($"\"{tmpCommand}\":\"" + (await HomeRTMTools.BroadcastRemoteCommand(tmpCommand, Retail) ? "Requested Command sent successfully!" : "Error while sending the Requested Command!") + '\"');
                                }

                                if (st.Length > 1)
                                    st.Append($",\"{Command}\":\"" + (await HomeRTMTools.BroadcastRemoteCommand(Command, Retail) ? "Requested Command sent successfully!" : "Error while sending the Requested Command!") + '\"');
                                else
                                    st.Append($"\"{Command}\":\"" + (await HomeRTMTools.BroadcastRemoteCommand(Command, Retail) ? "Requested Command sent successfully!" : "Error while sending the Requested Command!") + '\"');

                                await ctx.Response.Send(st.ToString() + ']');
                            }
                            else
                            {
                                ctx.Response.ContentType = "text/plain; charset=utf-8";

                                await ctx.Response.Send(await HomeRTMTools.BroadcastRemoteCommand(Command, Retail) ? "Requested Command sent successfully!" : "Error while sending the Requested Command!");
                            }
                        }
                        else if (ctx.Request.QuerystringExists("SupplementalCommands") && !string.IsNullOrEmpty(ctx.Request.RetrieveQueryValue("SupplementalCommands")))
                        {
                            StringBuilder st = new("[");

                            ctx.Response.ContentType = "application/json; charset=utf-8";

                            if (ctx.Request.QuerystringExists("AccessToken"))
                                AccessToken = HTTPProcessor.DecodeUrl(ctx.Request.RetrieveQueryValue("AccessToken"));

                            foreach (string tmpCommand in ctx.Request.RetrieveQueryValue("SupplementalCommands").Split('|'))
                            {
                                if (st.Length > 1)
                                    st.Append($",\"{tmpCommand}\":\"" + (await HomeRTMTools.SendRemoteCommand(clientip, AccessToken, tmpCommand, Retail) ? "Requested Command sent successfully!" : "Error while sending the Requested Command!") + '\"');
                                else
                                    st.Append($"\"{tmpCommand}\":\"" + (await HomeRTMTools.SendRemoteCommand(clientip, AccessToken, tmpCommand, Retail) ? "Requested Command sent successfully!" : "Error while sending the Requested Command!") + '\"');
                            }

                            if (st.Length > 1)
                                st.Append($",\"{Command}\":\"" + (await HomeRTMTools.SendRemoteCommand(clientip, AccessToken, Command, Retail) ? "Requested Command sent successfully!" : "Error while sending the Requested Command!") + '\"');
                            else
                                st.Append($"\"{Command}\":\"" + (await HomeRTMTools.SendRemoteCommand(clientip, AccessToken, Command, Retail) ? "Requested Command sent successfully!" : "Error while sending the Requested Command!") + '\"');

                            await ctx.Response.Send(st.ToString() + ']');
                        }
                        else
                        {
                            ctx.Response.ContentType = "text/plain; charset=utf-8";

                            if (ctx.Request.QuerystringExists("AccessToken"))
                                AccessToken = HTTPProcessor.DecodeUrl(ctx.Request.RetrieveQueryValue("AccessToken"));

                            await ctx.Response.Send(await HomeRTMTools.SendRemoteCommand(clientip, AccessToken, Command, Retail) ? "Requested Command sent successfully!" : "Error while sending the Requested Command!");
                        }
                    }
                });

                _Server.Routes.PostAuthentication.Parameter.Add(WatsonWebserver.Core.HttpMethod.GET, "/HomeSSFW/{command}/", async (HttpContextBase ctx) =>
                {
                    string? Command = ctx.Request.Url.Parameters["command"];
                    string userAgent = ctx.Request.Useragent;
                    string clientip = ctx.Request.Source.IpAddress;

                    if ((IPAddress.TryParse(clientip, out IPAddress? clientipAddr) && await HorizonServerConfiguration.Database.GetIsIpBanned(clientipAddr))
                    || (!string.IsNullOrEmpty(userAgent) && userAgent.Contains("bytespider", StringComparison.InvariantCultureIgnoreCase))) // Get Away TikTok and cheaters.
                    {
                        ctx.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        ctx.Response.ContentType = "text/plain";
                        await ctx.Response.Send();
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(Command))
                        {
                            ctx.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            ctx.Response.ContentType = "text/plain";
                            await ctx.Response.Send();
                            return;
                        }
                        else
                            Command = HTTPProcessor.DecodeUrl(Command);

                        bool Retail = true;
                        string? AccessToken = null;

                        if (ctx.Request.QuerystringExists("Retail") && bool.TryParse(ctx.Request.RetrieveQueryValue("Retail"), out Retail))
                        {

                        }

                        if (ctx.Request.QuerystringExists("AccessToken"))
                            AccessToken = HTTPProcessor.DecodeUrl(ctx.Request.RetrieveQueryValue("AccessToken"));

                        switch (Command)
                        {
                            case "GetUserIds":
                                bool AccessTokenProvided = !string.IsNullOrEmpty(AccessToken);
                                StringBuilder sb = new("[");
                                List<string> userIds = new();
                                List<ClientObject>? clients = null;

                                if (AccessTokenProvided)
                                {
                                    ClientObject? client = MediusClass.Manager.GetClientByAccessToken(AccessToken, Retail ? 20374 : 20371);
                                    if (client != null)
                                    {
                                        clients = new()
                                        {
                                            client
                                        };
                                    }
                                }
                                else
                                    clients = MediusClass.Manager.GetClientsByIp(ctx.Request.Source.IpAddress, Retail ? 20374 : 20371);

                                if (clients != null)
                                {
                                    foreach (ClientObject client in clients)
                                    {
                                        string? userId = client.SSFWid;

                                        if (!string.IsNullOrEmpty(userId) && !userIds.Contains(userId))
                                            userIds.Add(userId);
                                    }
                                }

                                foreach (string userId in userIds)
                                {
                                    if (sb.Length > 1)
                                        sb.Append($",\"{userId}\"");
                                    else
                                        sb.Append($"\"{userId}\"");
                                }

                                sb.Append(']');

                                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                                ctx.Response.ContentType = "application/json; charset=utf-8";
                                await ctx.Response.Send(sb.ToString());
                                break;
                            default:
                                ctx.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                                ctx.Response.ContentType = "text/plain";
                                await ctx.Response.Send();
                                return;
                        }
                    }
                });

                _Server.Routes.PostAuthentication.Parameter.Add(WatsonWebserver.Core.HttpMethod.GET, "/HomeGJS/{command}/", async (HttpContextBase ctx) =>
                {
                    string? Command = ctx.Request.Url.Parameters["command"];
                    string userAgent = ctx.Request.Useragent;
                    string clientip = ctx.Request.Source.IpAddress;

                    if ((IPAddress.TryParse(clientip, out IPAddress? clientipAddr) && await HorizonServerConfiguration.Database.GetIsIpBanned(clientipAddr))
                    || (!string.IsNullOrEmpty(userAgent) && userAgent.Contains("bytespider", StringComparison.InvariantCultureIgnoreCase))) // Get Away TikTok and cheaters.
                    {
                        ctx.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        ctx.Response.ContentType = "text/plain";
                        await ctx.Response.Send();
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(Command))
                        {
                            ctx.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            ctx.Response.ContentType = "text/plain";
                            await ctx.Response.Send();
                            return;
                        }
                        else
                            Command = HTTPProcessor.DecodeUrl(Command);

                        bool Retail = true;
                        bool Admin = false;
                        string? AccessToken = null;

                        if (!string.IsNullOrEmpty(clientip) && ("::1".Equals(clientip) || "127.0.0.1".Equals(clientip)
                        || "localhost".Equals(clientip, StringComparison.InvariantCultureIgnoreCase) || MediusClass.Settings.PlaystationHomeUsersServersAccessList.Any(entry => entry.Key.Contains($":{clientip}") && "ADMIN".Equals(entry.Value))))
                            Admin = true;

                        if (ctx.Request.QuerystringExists("Retail") && bool.TryParse(ctx.Request.RetrieveQueryValue("Retail"), out Retail))
                        {

                        }

                        if (ctx.Request.QuerystringExists("AccessToken"))
                            AccessToken = HTTPProcessor.DecodeUrl(ctx.Request.RetrieveQueryValue("AccessToken"));

                        switch (Command)
                        {
                            case "SendCrc":
                                ctx.Response.ContentType = "text/plain; charset=utf-8";

                                if (ctx.Request.QuerystringExists("Crc"))
                                {
                                    ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                                    await ctx.Response.Send(await HomeGuestJoiningSystem.SendCrcOverride(clientip, AccessToken, ctx.Request.RetrieveQueryValue("Crc"), Retail, ctx.Request.RetrieveQueryValue("env")) ? "Requested Crc sent successfully!" : "Error while sending the Requested Crc!");
                                }
                                else
                                {
                                    ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                                    await ctx.Response.Send("No Crc given for the request!");
                                }

                                return;
                            case "GetCrcList":
                                ctx.Response.ContentType = "application/json; charset=utf-8";
                                ctx.Response.StatusCode = (int)HttpStatusCode.OK;

                                string CrcListJsonOutputString;

                                if (Admin && ctx.Request.QuerystringExists("GetAll") && bool.TryParse(ctx.Request.RetrieveQueryValue("GetAll"), out bool getAll) && getAll)
                                    CrcListJsonOutputString = JsonConvert.SerializeObject(await HomeGuestJoiningSystem.getCrcList(clientip, null, Retail, true), Formatting.Indented);
                                else
                                    CrcListJsonOutputString = JsonConvert.SerializeObject(await HomeGuestJoiningSystem.getCrcList(clientip, AccessToken, Retail, false), Formatting.Indented);

                                await ctx.Response.Send(CrcListJsonOutputString);
                                return;
                            default:
                                ctx.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                                ctx.Response.ContentType = "text/plain";
                                await ctx.Response.Send();
                                return;
                        }
                    }
                });

                _Server.Routes.PostAuthentication.Parameter.Add(WatsonWebserver.Core.HttpMethod.GET, "/favicon.ico", async (HttpContextBase ctx) =>
                {
                    string userAgent = ctx.Request.Useragent;
                    string clientip = ctx.Request.Source.IpAddress;

                    if ((IPAddress.TryParse(clientip, out IPAddress? clientipAddr) && await HorizonServerConfiguration.Database.GetIsIpBanned(clientipAddr))
                    || (!string.IsNullOrEmpty(userAgent) && userAgent.Contains("bytespider", StringComparison.InvariantCultureIgnoreCase))) // Get Away TikTok and cheaters.
                    {
                        ctx.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        ctx.Response.ContentType = "text/plain";
                        await ctx.Response.Send();
                    }
                    else
                    {
                        if (File.Exists(Directory.GetCurrentDirectory() + "/static/wwwroot/favicon.ico"))
                        {
                            ctx.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                            ctx.Response.ContentType = "image/x-icon";
                            ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                            string? encoding = ctx.Request.RetrieveHeaderValue("Accept-Encoding");
                            if (!string.IsNullOrEmpty(encoding))
                            {
                                if (encoding.Contains("zstd"))
                                {
                                    ctx.Response.Headers.Add("Content-Encoding", "zstd");
                                    await ctx.Response.Send(HTTPProcessor.CompressZstd(File.ReadAllBytes(Directory.GetCurrentDirectory() + "/static/wwwroot/favicon.ico")));
                                }
                                else if (encoding.Contains("br"))
                                {
                                    ctx.Response.Headers.Add("Content-Encoding", "br");
                                    await ctx.Response.Send(HTTPProcessor.CompressBrotli(File.ReadAllBytes(Directory.GetCurrentDirectory() + "/static/wwwroot/favicon.ico")));
                                }
                                else if (encoding.Contains("gzip"))
                                {
                                    ctx.Response.Headers.Add("Content-Encoding", "gzip");
                                    await ctx.Response.Send(HTTPProcessor.CompressGzip(File.ReadAllBytes(Directory.GetCurrentDirectory() + "/static/wwwroot/favicon.ico")));
                                }
                                else if (encoding.Contains("deflate"))
                                {
                                    ctx.Response.Headers.Add("Content-Encoding", "deflate");
                                    await ctx.Response.Send(HTTPProcessor.Deflate(File.ReadAllBytes(Directory.GetCurrentDirectory() + "/static/wwwroot/favicon.ico")));
                                }
                                else
                                    await ctx.Response.Send(File.ReadAllBytes(Directory.GetCurrentDirectory() + "/static/wwwroot/favicon.ico"));
                            }
                            else
                                await ctx.Response.Send(File.ReadAllBytes(Directory.GetCurrentDirectory() + "/static/wwwroot/favicon.ico"));
                        }
                        else
                        {
                            ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                            ctx.Response.ContentType = "text/plain";
                            await ctx.Response.Send();
                        }
                    }
                });

                _Server.Start();

                LoggerAccessor.LogInfo($"CrudHandler Server initiated on port:{port}...");
            }
        }

        private void ExceptionEncountered(object? sender, ExceptionEventArgs args)
        {
            LoggerAccessor.LogError(args.Exception);
        }

        private static async Task DefaultRoute(HttpContextBase ctx)
        {
            ctx.Response.StatusCode = 403;
            ctx.Response.ContentType = "text/plain";
            await ctx.Response.Send();
        }
    }
}
