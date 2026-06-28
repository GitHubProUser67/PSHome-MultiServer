using System.Net;
using System.Security.Authentication;
using System.Text;
using CustomLogger;
using Horizon.DME.Extensions.PSHome;
using Horizon.MEDIUS;
using Horizon.MEDIUS.Extensions.PSHome;
using Horizon.MUM.Models;
using MultiServerLibrary.GeoLocalization;
using MultiServerLibrary.HTTP;
using Newtonsoft.Json;
using Prometheus;
using WatsonWebserver;
using WatsonWebserver.Core;

namespace Horizon.HTTPSERVICE
{
    public class CrudServerHandler
    {
        private static readonly Counter _clientsRequests = Metrics.CreateCounter(
            "medius_crud_requests_total",
            "Total number of Medius CRUD API requests."
        );

        private readonly Webserver? _server;
        private readonly int _port;

        public CrudServerHandler(string ip, int port, string certpath = "", string certpass = "")
        {
            _port = port;

            WebserverSettings settings = new() { Hostname = ip, Port = port };

            if (!string.IsNullOrEmpty(certpath))
            {
                settings.Ssl.PfxCertificateFile = certpath;
                settings.Ssl.PfxCertificatePassword = certpass;
                settings.Ssl.Enable = true;
            }

            _server = new Webserver(settings, DefaultRoute);
            _server.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
            StartServer();
        }

        private static async Task AuthorizeConnection(HttpContextBase ctx)
        {
            _clientsRequests.Inc();

            var IpToBan = ctx.Request.Source.IpAddress;
            if (
                !"::1".Equals(IpToBan)
                && !"127.0.0.1".Equals(IpToBan)
                && !"localhost".Equals(IpToBan, StringComparison.InvariantCultureIgnoreCase)
            )
            {
                if (
                    !string.IsNullOrEmpty(IpToBan)
                    && MultiServerLibrary.MultiServerLibraryConfiguration.BannedIPs != null
                    && MultiServerLibrary.MultiServerLibraryConfiguration.BannedIPs.Contains(
                        IpToBan
                    )
                )
                {
                    LoggerAccessor.LogError(
                        $"[SECURITY] - Client - {ctx.Request.Source.IpAddress}:{ctx.Request.Source.Port} Requested the Medius CRUD API server while being banned!"
                    );
                    ctx.Response.StatusCode = 403;
                    await ctx.Response.Send();
                }
            }
        }

        public void StopServer()
        {
            _server?.Dispose();

            LoggerAccessor.LogWarn($"CrudHandler Server on port: {_port} stopped...");
        }

        public void StartServer()
        {
            if (_server != null && !_server.IsListening)
            {
                _server.Routes.AuthenticateRequest = AuthorizeConnection;
                _server.Events.ExceptionEncountered += ExceptionEncountered;
                _server.Events.Logger = LoggerAccessor.LogInfo;
#if DEBUG
                _server.Settings.Debug.Responses = true;
                _server.Settings.Debug.Routing = true;
#endif
                _server.Routes.PostAuthentication.Parameter.Add(
                    WatsonWebserver.Core.HttpMethod.GET,
                    "/GetRooms/",
                    async (HttpContextBase ctx) =>
                    {
                        var userAgent = ctx.Request.Useragent;
                        var clientip = ctx.Request.Source.IpAddress;

                        if (
                            (
                                IPAddress.TryParse(clientip, out var clientipAddr)
                                && await HorizonServerConfiguration.Database.GetIsIpBanned(
                                    clientipAddr
                                )
                            )
                            || (
                                !string.IsNullOrEmpty(userAgent)
                                && userAgent.Contains(
                                    "bytespider",
                                    StringComparison.InvariantCultureIgnoreCase
                                )
                            )
                        ) // Get Away TikTok and cheaters.
                        {
                            ctx.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                            ctx.Response.ContentType = "text/plain";
                            await ctx.Response.Send();
                        }
                        else
                        {
                            var managerPayload = RoomManager.ToJson();

                            ctx.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                            ctx.Response.ContentType = "application/json; charset=UTF-8";
                            ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                            var encoding = ctx.Request.RetrieveHeaderValue("Accept-Encoding");
                            if (!string.IsNullOrEmpty(encoding))
                            {
                                if (encoding.Contains("zstd"))
                                {
                                    ctx.Response.Headers.Add("Content-Encoding", "zstd");
                                    await ctx.Response.Send(
                                        HTTPProcessor.CompressZstd(
                                            Encoding.UTF8.GetBytes(managerPayload)
                                        )
                                    );
                                }
                                else if (encoding.Contains("br"))
                                {
                                    ctx.Response.Headers.Add("Content-Encoding", "br");
                                    await ctx.Response.Send(
                                        HTTPProcessor.CompressBrotli(
                                            Encoding.UTF8.GetBytes(managerPayload)
                                        )
                                    );
                                }
                                else if (encoding.Contains("gzip"))
                                {
                                    ctx.Response.Headers.Add("Content-Encoding", "gzip");
                                    await ctx.Response.Send(
                                        HTTPProcessor.CompressGzip(
                                            Encoding.UTF8.GetBytes(managerPayload)
                                        )
                                    );
                                }
                                else if (encoding.Contains("deflate"))
                                {
                                    ctx.Response.Headers.Add("Content-Encoding", "deflate");
                                    await ctx.Response.Send(
                                        HTTPProcessor.Deflate(
                                            Encoding.UTF8.GetBytes(managerPayload)
                                        )
                                    );
                                }
                                else
                                    await ctx.Response.Send(managerPayload);
                            }
                            else
                                await ctx.Response.Send(managerPayload);
                        }
                    }
                );

                _server.Routes.PostAuthentication.Parameter.Add(
                    WatsonWebserver.Core.HttpMethod.GET,
                    "/GetCIDsList/",
                    async (HttpContextBase ctx) =>
                    {
                        var userAgent = ctx.Request.Useragent;
                        var clientip = ctx.Request.Source.IpAddress;

                        if (
                            (
                                IPAddress.TryParse(clientip, out var clientipAddr)
                                && await HorizonServerConfiguration.Database.GetIsIpBanned(
                                    clientipAddr
                                )
                            )
                            || (
                                !string.IsNullOrEmpty(userAgent)
                                && userAgent.Contains(
                                    "bytespider",
                                    StringComparison.InvariantCultureIgnoreCase
                                )
                            )
                        ) // Get Away TikTok and cheaters.
                        {
                            ctx.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                            ctx.Response.ContentType = "text/plain";
                            await ctx.Response.Send();
                        }
                        else
                        {
                            var localhost = false;

                            if (
                                "::1".Equals(clientip)
                                || "127.0.0.1".Equals(clientip)
                                || "localhost".Equals(
                                    clientip,
                                    StringComparison.InvariantCultureIgnoreCase
                                )
                            )
                                localhost = true;

                            ctx.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                            ctx.Response.ContentType = localhost
                                ? "application/json; charset=UTF-8"
                                : "text/plain";
                            ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                            var encoding = ctx.Request.RetrieveHeaderValue("Accept-Encoding");
                            if (!string.IsNullOrEmpty(encoding))
                            {
                                if (encoding.Contains("zstd"))
                                {
                                    ctx.Response.Headers.Add("Content-Encoding", "zstd");
                                    await ctx.Response.Send(
                                        HTTPProcessor.CompressZstd(
                                            Encoding.UTF8.GetBytes(CIDManager.ToJson(!localhost))
                                        )
                                    );
                                }
                                else if (encoding.Contains("br"))
                                {
                                    ctx.Response.Headers.Add("Content-Encoding", "br");
                                    await ctx.Response.Send(
                                        HTTPProcessor.CompressBrotli(
                                            Encoding.UTF8.GetBytes(CIDManager.ToJson(!localhost))
                                        )
                                    );
                                }
                                else if (encoding.Contains("gzip"))
                                {
                                    ctx.Response.Headers.Add("Content-Encoding", "gzip");
                                    await ctx.Response.Send(
                                        HTTPProcessor.CompressGzip(
                                            Encoding.UTF8.GetBytes(CIDManager.ToJson(!localhost))
                                        )
                                    );
                                }
                                else if (encoding.Contains("deflate"))
                                {
                                    ctx.Response.Headers.Add("Content-Encoding", "deflate");
                                    await ctx.Response.Send(
                                        HTTPProcessor.Deflate(
                                            Encoding.UTF8.GetBytes(CIDManager.ToJson(!localhost))
                                        )
                                    );
                                }
                                else
                                    await ctx.Response.Send(CIDManager.ToJson(!localhost));
                            }
                            else
                                await ctx.Response.Send(CIDManager.ToJson(!localhost));
                        }
                    }
                );

                _server.Routes.PostAuthentication.Parameter.Add(
                    WatsonWebserver.Core.HttpMethod.GET,
                    "/HomeIGA/{command}/",
                    async (HttpContextBase ctx) =>
                    {
                        var Command = ctx.Request.Url.Parameters["command"];
                        var userAgent = ctx.Request.Useragent;
                        var clientip = ctx.Request.Source.IpAddress;

                        if (
                            (
                                IPAddress.TryParse(clientip, out var clientipAddr)
                                && await HorizonServerConfiguration.Database.GetIsIpBanned(
                                    clientipAddr
                                )
                            )
                            || (
                                !string.IsNullOrEmpty(userAgent)
                                && userAgent.Contains(
                                    "bytespider",
                                    StringComparison.InvariantCultureIgnoreCase
                                )
                            )
                        ) // Get Away TikTok and cheaters.
                        {
                            ctx.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                            ctx.Response.ContentType = "text/plain";
                            await ctx.Response.Send();
                        }
                        else
                        {
                            if (
                                !string.IsNullOrEmpty(clientip)
                                && (
                                    "::1".Equals(clientip)
                                    || "127.0.0.1".Equals(clientip)
                                    || "localhost".Equals(
                                        clientip,
                                        StringComparison.InvariantCultureIgnoreCase
                                    )
                                    || HorizonServerConfiguration.MEDIUSPlaystationHomeUsersServersAccessList.Any(
                                        entry =>
                                            entry.Key.Contains($":{clientip}")
                                            && "ADMIN".Equals(entry.Value)
                                    )
                                )
                            )
                            {
                                if (
                                    !string.IsNullOrEmpty(Command)
                                    && ctx.Request.QuerystringExists("DmeId")
                                    && short.TryParse(
                                        ctx.Request.RetrieveQueryValue("DmeId"),
                                        out var DmeId
                                    )
                                    && ctx.Request.QuerystringExists("WorldId")
                                    && int.TryParse(
                                        ctx.Request.RetrieveQueryValue("WorldId"),
                                        out var WorldId
                                    )
                                )
                                {
                                    var Retail = true;
                                    var result = "Command Unknown!";

                                    if (
                                        ctx.Request.QuerystringExists("Retail")
                                        && bool.TryParse(
                                            ctx.Request.RetrieveQueryValue("Retail"),
                                            out Retail
                                        )
                                    ) { }

                                    LoggerAccessor.LogWarn(
                                        $"[CrudServerHandler] - client:{clientip}:{ctx.Request.Source.Port} issued command: {Command}"
                                    );

                                    switch (Command)
                                    {
                                        case "Kick":
                                            result = NewIGA.KickClient(DmeId, WorldId, Retail);
                                            break;
                                        case "Release":
                                            result = NewIGA.ReleaseClient(DmeId, WorldId, Retail);
                                            break;
                                        case "Mute":
                                            result = NewIGA.MuteClient(DmeId, WorldId, Retail);
                                            break;
                                        case "MuteFreeze":
                                            result = NewIGA.MuteAndFreezeClient(
                                                DmeId,
                                                WorldId,
                                                Retail
                                            );
                                            break;
                                        case "Freeze":
                                            result = NewIGA.FreezeClient(DmeId, WorldId, Retail);
                                            break;
                                        default:
                                            LoggerAccessor.LogWarn(
                                                $"[CrudServerHandler] - Unknown Home IGA command: {Command}"
                                            );
                                            break;
                                    }

                                    ctx.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                    ctx.Response.ContentType = "text/plain";
                                    ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                                    var encoding = ctx.Request.RetrieveHeaderValue(
                                        "Accept-Encoding"
                                    );
                                    if (!string.IsNullOrEmpty(encoding))
                                    {
                                        if (encoding.Contains("zstd"))
                                        {
                                            ctx.Response.Headers.Add("Content-Encoding", "zstd");
                                            await ctx.Response.Send(
                                                HTTPProcessor.CompressZstd(
                                                    Encoding.UTF8.GetBytes(result)
                                                )
                                            );
                                        }
                                        else if (encoding.Contains("br"))
                                        {
                                            ctx.Response.Headers.Add("Content-Encoding", "br");
                                            await ctx.Response.Send(
                                                HTTPProcessor.CompressBrotli(
                                                    Encoding.UTF8.GetBytes(result)
                                                )
                                            );
                                        }
                                        else if (encoding.Contains("gzip"))
                                        {
                                            ctx.Response.Headers.Add("Content-Encoding", "gzip");
                                            await ctx.Response.Send(
                                                HTTPProcessor.CompressGzip(
                                                    Encoding.UTF8.GetBytes(result)
                                                )
                                            );
                                        }
                                        else if (encoding.Contains("deflate"))
                                        {
                                            ctx.Response.Headers.Add("Content-Encoding", "deflate");
                                            await ctx.Response.Send(
                                                HTTPProcessor.Deflate(
                                                    Encoding.UTF8.GetBytes(result)
                                                )
                                            );
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
                            else if (
                                File.Exists(
                                    Directory.GetCurrentDirectory()
                                        + "/static/creepy_iga_fallback.mp4"
                                )
                            )
                            {
                                var videoData = File.ReadAllBytes(
                                    Directory.GetCurrentDirectory()
                                        + "/static/creepy_iga_fallback.mp4"
                                );

                                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                                ctx.Response.ContentType = "video/mp4";
                                var encoding = ctx.Request.RetrieveHeaderValue("Accept-Encoding");
                                if (!string.IsNullOrEmpty(encoding))
                                {
                                    if (encoding.Contains("zstd"))
                                    {
                                        ctx.Response.Headers.Add("Content-Encoding", "zstd");
                                        await ctx.Response.Send(
                                            HTTPProcessor.CompressZstd(videoData)
                                        );
                                    }
                                    else if (encoding.Contains("br"))
                                    {
                                        ctx.Response.Headers.Add("Content-Encoding", "br");
                                        await ctx.Response.Send(
                                            HTTPProcessor.CompressBrotli(videoData)
                                        );
                                    }
                                    else if (encoding.Contains("gzip"))
                                    {
                                        ctx.Response.Headers.Add("Content-Encoding", "gzip");
                                        await ctx.Response.Send(
                                            HTTPProcessor.CompressGzip(videoData)
                                        );
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
                                var htmlPayload =
                                    "<!DOCTYPE html>\r\n<html lang=\"en\">\r\n<head>\r\n"
                                    + "    <meta charset=\"UTF-8\">\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n"
                                    + "    <title>DARK WEB</title>\r\n    <style>\r\n        body {\r\n            margin: 0;\r\n            padding: 0;\r\n"
                                    + "            display: flex;\r\n            justify-content: center;\r\n            align-items: center;\r\n"
                                    + "            height: 100vh;\r\n            background-image: url('https://media1.tenor.com/m/IKo-c45o9XUAAAAC/horror-gif.gif');\r\n"
                                    + "            background-size: cover;\r\n            background-position: center;\r\n        }\r\n\r\n        h1 {\r\n"
                                    + "            color: red;\r\n            font-size: 100px;\r\n            font-family: 'Creepster', cursive; /* You can link to a scary font if you want */\r\n"
                                    + "            text-shadow: 4px 4px 8px black;\r\n        }\r\n    </style>\r\n</head>\r\n<body>\r\n"
                                    + "    <iframe width=\"0\" height=\"0\" src=\"https://www.youtube.com/embed/XfQrgDbisAo?autoplay=1&loop=1\"\r\n    frameborder=\"0\" allowfullscreen></iframe>"
                                    + $"    <h1>BEWARE! {$"We know your IP: {clientip} and where you live: {await WebLocalization.GetOpenStreetMapUrl(clientip) ?? "Earth"}"}</h1>\r\n</body>\r\n"
                                    + "</html>";

                                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                                ctx.Response.ContentType = "text/html";
                                var encoding = ctx.Request.RetrieveHeaderValue("Accept-Encoding");
                                if (!string.IsNullOrEmpty(encoding))
                                {
                                    if (encoding.Contains("zstd"))
                                    {
                                        ctx.Response.Headers.Add("Content-Encoding", "zstd");
                                        await ctx.Response.Send(
                                            HTTPProcessor.CompressZstd(
                                                Encoding.UTF8.GetBytes(htmlPayload)
                                            )
                                        );
                                    }
                                    else if (encoding.Contains("br"))
                                    {
                                        ctx.Response.Headers.Add("Content-Encoding", "br");
                                        await ctx.Response.Send(
                                            HTTPProcessor.CompressBrotli(
                                                Encoding.UTF8.GetBytes(htmlPayload)
                                            )
                                        );
                                    }
                                    else if (encoding.Contains("gzip"))
                                    {
                                        ctx.Response.Headers.Add("Content-Encoding", "gzip");
                                        await ctx.Response.Send(
                                            HTTPProcessor.CompressGzip(
                                                Encoding.UTF8.GetBytes(htmlPayload)
                                            )
                                        );
                                    }
                                    else if (encoding.Contains("deflate"))
                                    {
                                        ctx.Response.Headers.Add("Content-Encoding", "deflate");
                                        await ctx.Response.Send(
                                            HTTPProcessor.Deflate(
                                                Encoding.UTF8.GetBytes(htmlPayload)
                                            )
                                        );
                                    }
                                    else
                                        await ctx.Response.Send(htmlPayload);
                                }
                                else
                                    await ctx.Response.Send(htmlPayload);
                            }
                        }
                    }
                );

                _server.Routes.PostAuthentication.Parameter.Add(
                    WatsonWebserver.Core.HttpMethod.GET,
                    "/HomeAdminMessage/{region_code}/{message}/",
                    async (HttpContextBase ctx) =>
                    {
                        var region_code = ctx.Request.Url.Parameters["region_code"];
                        var message = HTTPProcessor.DecodeUrl(
                            ctx.Request.Url.Parameters["message"]
                        );
                        var userAgent = ctx.Request.Useragent;
                        var clientip = ctx.Request.Source.IpAddress;

                        if (
                            (
                                IPAddress.TryParse(clientip, out var clientipAddr)
                                && await HorizonServerConfiguration.Database.GetIsIpBanned(
                                    clientipAddr
                                )
                            )
                            || (
                                !string.IsNullOrEmpty(userAgent)
                                && userAgent.Contains(
                                    "bytespider",
                                    StringComparison.InvariantCultureIgnoreCase
                                )
                            )
                        ) // Get Away TikTok and cheaters.
                        {
                            ctx.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                            ctx.Response.ContentType = "text/plain";
                            await ctx.Response.Send();
                        }
                        else
                        {
                            var Admin = false;

                            if (
                                !string.IsNullOrEmpty(clientip)
                                && (
                                    "::1".Equals(clientip)
                                    || "127.0.0.1".Equals(clientip)
                                    || "localhost".Equals(
                                        clientip,
                                        StringComparison.InvariantCultureIgnoreCase
                                    )
                                    || HorizonServerConfiguration.MEDIUSPlaystationHomeUsersServersAccessList.Any(
                                        entry =>
                                            entry.Key.Contains($":{clientip}")
                                            && "ADMIN".Equals(entry.Value)
                                    )
                                )
                            )
                                Admin = true;

                            if (!Admin)
                            {
                                ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                ctx.Response.ContentType = "text/plain";
                                await ctx.Response.Send();
                                return;
                            }

                            var Retail = true;
                            var IsLcCompatible = false;
                            var worldId = -1;
                            string? AccessToken = null;

                            if (
                                ctx.Request.QuerystringExists("Retail")
                                && bool.TryParse(
                                    ctx.Request.RetrieveQueryValue("Retail"),
                                    out Retail
                                )
                            ) { }
                            if (
                                ctx.Request.QuerystringExists("Lc")
                                && bool.TryParse(
                                    ctx.Request.RetrieveQueryValue("Lc"),
                                    out IsLcCompatible
                                )
                            ) { }
                            if (
                                ctx.Request.QuerystringExists("worldId")
                                && int.TryParse(
                                    ctx.Request.RetrieveQueryValue("worldId"),
                                    out worldId
                                )
                            ) { }

                            ctx.Response.StatusCode = (int)HttpStatusCode.OK;

                            if (
                                Admin
                                && ctx.Request.QuerystringExists("BroadcastAcrossEntireUniverse")
                                && bool.TryParse(
                                    ctx.Request.RetrieveQueryValue("BroadcastAcrossEntireUniverse"),
                                    out var Broadcast
                                )
                                && Broadcast
                            )
                            {
                                ctx.Response.ContentType = "text/plain; charset=utf-8";

                                await ctx.Response.Send(
                                    await HomeServerMessage.BroadcastAdminMessage(
                                        region_code,
                                        message,
                                        IsLcCompatible,
                                        Retail
                                    )
                                        ? "Requested Message sent successfully!"
                                        : "Error while sending the Requested Message!"
                                );
                            }
                            else
                            {
                                ctx.Response.ContentType = "text/plain; charset=utf-8";

                                if (ctx.Request.QuerystringExists("AccessToken"))
                                    AccessToken = HTTPProcessor.DecodeUrl(
                                        ctx.Request.RetrieveQueryValue("AccessToken")
                                    );

                                await ctx.Response.Send(
                                    await HomeServerMessage.SendAdminMessage(
                                        clientip,
                                        AccessToken,
                                        region_code,
                                        worldId,
                                        message,
                                        IsLcCompatible,
                                        Retail
                                    )
                                        ? "Requested Message sent successfully!"
                                        : "Error while sending the Requested Message!"
                                );
                            }
                        }
                    }
                );

                _server.Routes.PostAuthentication.Parameter.Add(
                    WatsonWebserver.Core.HttpMethod.GET,
                    "/HomeLogOff/{region_code}/{user_name}/",
                    async (HttpContextBase ctx) =>
                    {
                        var region_code = ctx.Request.Url.Parameters["region_code"];
                        var user_name = HTTPProcessor.DecodeUrl(
                            ctx.Request.Url.Parameters["user_name"]
                        );
                        var userAgent = ctx.Request.Useragent;
                        var clientip = ctx.Request.Source.IpAddress;

                        if (
                            (
                                IPAddress.TryParse(clientip, out var clientipAddr)
                                && await HorizonServerConfiguration.Database.GetIsIpBanned(
                                    clientipAddr
                                )
                            )
                            || (
                                !string.IsNullOrEmpty(userAgent)
                                && userAgent.Contains(
                                    "bytespider",
                                    StringComparison.InvariantCultureIgnoreCase
                                )
                            )
                        ) // Get Away TikTok and cheaters.
                        {
                            ctx.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                            ctx.Response.ContentType = "text/plain";
                            await ctx.Response.Send();
                        }
                        else
                        {
                            var Retail = true;
                            var Admin = false;
                            var IsLcCompatible = false;

                            if (
                                !string.IsNullOrEmpty(clientip)
                                && (
                                    "::1".Equals(clientip)
                                    || "127.0.0.1".Equals(clientip)
                                    || "localhost".Equals(
                                        clientip,
                                        StringComparison.InvariantCultureIgnoreCase
                                    )
                                    || HorizonServerConfiguration.MEDIUSPlaystationHomeUsersServersAccessList.Any(
                                        entry =>
                                            entry.Key.Contains($":{clientip}")
                                            && "ADMIN".Equals(entry.Value)
                                    )
                                )
                            )
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

                            if (
                                ctx.Request.QuerystringExists("Retail")
                                && bool.TryParse(
                                    ctx.Request.RetrieveQueryValue("Retail"),
                                    out Retail
                                )
                            ) { }
                            if (
                                ctx.Request.QuerystringExists("Lc")
                                && bool.TryParse(
                                    ctx.Request.RetrieveQueryValue("Lc"),
                                    out IsLcCompatible
                                )
                            ) { }

                            ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                            ctx.Response.ContentType = "text/plain; charset=utf-8";

                            var clientTarget = Program.MUMManager.GetClientByAccountName(
                                user_name,
                                Retail ? 20374 : 20371
                            );
                            if (clientTarget != null)
                                await ctx.Response.Send(
                                    await HomeServerMessage.SendLogOffCommand(
                                        clientTarget,
                                        region_code,
                                        Array.Empty<byte>(),
                                        IsLcCompatible
                                    )
                                        ? "Requested LogOff sent successfully!"
                                        : "Error while sending the Requested LogOff!"
                                );
                            else
                                await ctx.Response.Send("Requested User is not connected on Home!");
                        }
                    }
                );

                _server.Routes.PostAuthentication.Parameter.Add(
                    WatsonWebserver.Core.HttpMethod.GET,
                    "/HomeRTM/{command}/",
                    async (HttpContextBase ctx) =>
                    {
                        var Command = ctx.Request.Url.Parameters["command"];
                        var userAgent = ctx.Request.Useragent;
                        var clientip = ctx.Request.Source.IpAddress;

                        if (
                            (
                                IPAddress.TryParse(clientip, out var clientipAddr)
                                && await HorizonServerConfiguration.Database.GetIsIpBanned(
                                    clientipAddr
                                )
                            )
                            || (
                                !string.IsNullOrEmpty(userAgent)
                                && userAgent.Contains(
                                    "bytespider",
                                    StringComparison.InvariantCultureIgnoreCase
                                )
                            )
                        ) // Get Away TikTok and cheaters.
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

                            var Retail = true;
                            var Admin = false;
                            string? AccessToken = null;

                            if (
                                !string.IsNullOrEmpty(clientip)
                                && (
                                    "::1".Equals(clientip)
                                    || "127.0.0.1".Equals(clientip)
                                    || "localhost".Equals(
                                        clientip,
                                        StringComparison.InvariantCultureIgnoreCase
                                    )
                                    || HorizonServerConfiguration.MEDIUSPlaystationHomeUsersServersAccessList.Any(
                                        entry =>
                                            entry.Key.Contains($":{clientip}")
                                            && "ADMIN".Equals(entry.Value)
                                    )
                                )
                            )
                                Admin = true;

                            if (
                                ctx.Request.QuerystringExists("Retail")
                                && bool.TryParse(
                                    ctx.Request.RetrieveQueryValue("Retail"),
                                    out Retail
                                )
                            ) { }

                            ctx.Response.StatusCode = (int)HttpStatusCode.OK;

                            if (
                                Admin
                                && ctx.Request.QuerystringExists("BroadcastAcrossEntireUniverse")
                                && bool.TryParse(
                                    ctx.Request.RetrieveQueryValue("BroadcastAcrossEntireUniverse"),
                                    out var Broadcast
                                )
                                && Broadcast
                            )
                            {
                                if (
                                    ctx.Request.QuerystringExists("SupplementalCommands")
                                    && !string.IsNullOrEmpty(
                                        ctx.Request.RetrieveQueryValue("SupplementalCommands")
                                    )
                                )
                                {
                                    StringBuilder st = new("[");

                                    ctx.Response.ContentType = "application/json; charset=utf-8";

                                    foreach (
                                        var tmpCommand in ctx
                                            .Request.RetrieveQueryValue("SupplementalCommands")
                                            .Split('|')
                                    )
                                    {
                                        if (st.Length > 1)
                                            st.Append(
                                                $",\"{tmpCommand}\":\""
                                                    + (
                                                        await HomeRTMTools.BroadcastRemoteCommand(
                                                            tmpCommand,
                                                            Retail
                                                        )
                                                            ? "Requested Command sent successfully!"
                                                            : "Error while sending the Requested Command!"
                                                    )
                                                    + '\"'
                                            );
                                        else
                                            st.Append(
                                                $"\"{tmpCommand}\":\""
                                                    + (
                                                        await HomeRTMTools.BroadcastRemoteCommand(
                                                            tmpCommand,
                                                            Retail
                                                        )
                                                            ? "Requested Command sent successfully!"
                                                            : "Error while sending the Requested Command!"
                                                    )
                                                    + '\"'
                                            );
                                    }

                                    if (st.Length > 1)
                                        st.Append(
                                            $",\"{Command}\":\""
                                                + (
                                                    await HomeRTMTools.BroadcastRemoteCommand(
                                                        Command,
                                                        Retail
                                                    )
                                                        ? "Requested Command sent successfully!"
                                                        : "Error while sending the Requested Command!"
                                                )
                                                + '\"'
                                        );
                                    else
                                        st.Append(
                                            $"\"{Command}\":\""
                                                + (
                                                    await HomeRTMTools.BroadcastRemoteCommand(
                                                        Command,
                                                        Retail
                                                    )
                                                        ? "Requested Command sent successfully!"
                                                        : "Error while sending the Requested Command!"
                                                )
                                                + '\"'
                                        );

                                    await ctx.Response.Send(st.ToString() + ']');
                                }
                                else
                                {
                                    ctx.Response.ContentType = "text/plain; charset=utf-8";

                                    await ctx.Response.Send(
                                        await HomeRTMTools.BroadcastRemoteCommand(Command, Retail)
                                            ? "Requested Command sent successfully!"
                                            : "Error while sending the Requested Command!"
                                    );
                                }
                            }
                            else if (
                                ctx.Request.QuerystringExists("SupplementalCommands")
                                && !string.IsNullOrEmpty(
                                    ctx.Request.RetrieveQueryValue("SupplementalCommands")
                                )
                            )
                            {
                                StringBuilder st = new("[");

                                ctx.Response.ContentType = "application/json; charset=utf-8";

                                if (ctx.Request.QuerystringExists("AccessToken"))
                                    AccessToken = HTTPProcessor.DecodeUrl(
                                        ctx.Request.RetrieveQueryValue("AccessToken")
                                    );

                                foreach (
                                    var tmpCommand in ctx
                                        .Request.RetrieveQueryValue("SupplementalCommands")
                                        .Split('|')
                                )
                                {
                                    if (st.Length > 1)
                                        st.Append(
                                            $",\"{tmpCommand}\":\""
                                                + (
                                                    await HomeRTMTools.SendRemoteCommand(
                                                        clientip,
                                                        AccessToken,
                                                        tmpCommand,
                                                        Retail
                                                    )
                                                        ? "Requested Command sent successfully!"
                                                        : "Error while sending the Requested Command!"
                                                )
                                                + '\"'
                                        );
                                    else
                                        st.Append(
                                            $"\"{tmpCommand}\":\""
                                                + (
                                                    await HomeRTMTools.SendRemoteCommand(
                                                        clientip,
                                                        AccessToken,
                                                        tmpCommand,
                                                        Retail
                                                    )
                                                        ? "Requested Command sent successfully!"
                                                        : "Error while sending the Requested Command!"
                                                )
                                                + '\"'
                                        );
                                }

                                if (st.Length > 1)
                                    st.Append(
                                        $",\"{Command}\":\""
                                            + (
                                                await HomeRTMTools.SendRemoteCommand(
                                                    clientip,
                                                    AccessToken,
                                                    Command,
                                                    Retail
                                                )
                                                    ? "Requested Command sent successfully!"
                                                    : "Error while sending the Requested Command!"
                                            )
                                            + '\"'
                                    );
                                else
                                    st.Append(
                                        $"\"{Command}\":\""
                                            + (
                                                await HomeRTMTools.SendRemoteCommand(
                                                    clientip,
                                                    AccessToken,
                                                    Command,
                                                    Retail
                                                )
                                                    ? "Requested Command sent successfully!"
                                                    : "Error while sending the Requested Command!"
                                            )
                                            + '\"'
                                    );

                                await ctx.Response.Send(st.ToString() + ']');
                            }
                            else
                            {
                                ctx.Response.ContentType = "text/plain; charset=utf-8";

                                if (ctx.Request.QuerystringExists("AccessToken"))
                                    AccessToken = HTTPProcessor.DecodeUrl(
                                        ctx.Request.RetrieveQueryValue("AccessToken")
                                    );

                                await ctx.Response.Send(
                                    await HomeRTMTools.SendRemoteCommand(
                                        clientip,
                                        AccessToken,
                                        Command,
                                        Retail
                                    )
                                        ? "Requested Command sent successfully!"
                                        : "Error while sending the Requested Command!"
                                );
                            }
                        }
                    }
                );

                _server.Routes.PostAuthentication.Parameter.Add(
                    WatsonWebserver.Core.HttpMethod.GET,
                    "/HomeSSFW/{command}/",
                    async (HttpContextBase ctx) =>
                    {
                        var Command = ctx.Request.Url.Parameters["command"];
                        var userAgent = ctx.Request.Useragent;
                        var clientip = ctx.Request.Source.IpAddress;

                        if (
                            (
                                IPAddress.TryParse(clientip, out var clientipAddr)
                                && await HorizonServerConfiguration.Database.GetIsIpBanned(
                                    clientipAddr
                                )
                            )
                            || (
                                !string.IsNullOrEmpty(userAgent)
                                && userAgent.Contains(
                                    "bytespider",
                                    StringComparison.InvariantCultureIgnoreCase
                                )
                            )
                        ) // Get Away TikTok and cheaters.
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

                            var Retail = true;
                            string? AccessToken = null;

                            if (
                                ctx.Request.QuerystringExists("Retail")
                                && bool.TryParse(
                                    ctx.Request.RetrieveQueryValue("Retail"),
                                    out Retail
                                )
                            ) { }

                            if (ctx.Request.QuerystringExists("AccessToken"))
                                AccessToken = HTTPProcessor.DecodeUrl(
                                    ctx.Request.RetrieveQueryValue("AccessToken")
                                );

                            switch (Command)
                            {
                                case "GetUserIds":
                                    var AccessTokenProvided = !string.IsNullOrEmpty(AccessToken);
                                    StringBuilder sb = new("[");
                                    List<string> userIds = new();
                                    List<ClientObject>? clients = null;

                                    if (AccessTokenProvided)
                                    {
                                        var client = Program.MUMManager.GetClientByAccessToken(
                                            AccessToken,
                                            Retail ? 20374 : 20371
                                        );
                                        if (client != null)
                                        {
                                            clients = new() { client };
                                        }
                                    }
                                    else
                                        clients = Program.MUMManager.GetClientsByIp(
                                            ctx.Request.Source.IpAddress,
                                            Retail ? 20374 : 20371
                                        );

                                    if (clients != null)
                                    {
                                        foreach (var client in clients)
                                        {
                                            var userId = client.SSFWid;

                                            if (
                                                !string.IsNullOrEmpty(userId)
                                                && !userIds.Contains(userId)
                                            )
                                                userIds.Add(userId);
                                        }
                                    }

                                    foreach (var userId in userIds)
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
                    }
                );

                _server.Routes.PostAuthentication.Parameter.Add(
                    WatsonWebserver.Core.HttpMethod.GET,
                    "/HomeGJS/{command}/",
                    async (HttpContextBase ctx) =>
                    {
                        var Command = ctx.Request.Url.Parameters["command"];
                        var userAgent = ctx.Request.Useragent;
                        var clientip = ctx.Request.Source.IpAddress;

                        if (
                            (
                                IPAddress.TryParse(clientip, out var clientipAddr)
                                && await HorizonServerConfiguration.Database.GetIsIpBanned(
                                    clientipAddr
                                )
                            )
                            || (
                                !string.IsNullOrEmpty(userAgent)
                                && userAgent.Contains(
                                    "bytespider",
                                    StringComparison.InvariantCultureIgnoreCase
                                )
                            )
                        ) // Get Away TikTok and cheaters.
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

                            var Retail = true;
                            var Admin = false;
                            string? AccessToken = null;

                            if (
                                !string.IsNullOrEmpty(clientip)
                                && (
                                    "::1".Equals(clientip)
                                    || "127.0.0.1".Equals(clientip)
                                    || "localhost".Equals(
                                        clientip,
                                        StringComparison.InvariantCultureIgnoreCase
                                    )
                                    || HorizonServerConfiguration.MEDIUSPlaystationHomeUsersServersAccessList.Any(
                                        entry =>
                                            entry.Key.Contains($":{clientip}")
                                            && "ADMIN".Equals(entry.Value)
                                    )
                                )
                            )
                                Admin = true;

                            if (
                                ctx.Request.QuerystringExists("Retail")
                                && bool.TryParse(
                                    ctx.Request.RetrieveQueryValue("Retail"),
                                    out Retail
                                )
                            ) { }

                            if (ctx.Request.QuerystringExists("AccessToken"))
                                AccessToken = HTTPProcessor.DecodeUrl(
                                    ctx.Request.RetrieveQueryValue("AccessToken")
                                );

                            switch (Command)
                            {
                                case "SendCrc":
                                    ctx.Response.ContentType = "text/plain; charset=utf-8";

                                    if (ctx.Request.QuerystringExists("Crc"))
                                    {
                                        ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                                        await ctx.Response.Send(
                                            await HomeGuestJoiningSystem.SendCrcOverride(
                                                clientip,
                                                AccessToken,
                                                ctx.Request.RetrieveQueryValue("Crc"),
                                                Retail,
                                                ctx.Request.RetrieveQueryValue("env")
                                            )
                                                ? "Requested Crc sent successfully!"
                                                : "Error while sending the Requested Crc!"
                                        );
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

                                    CrcListJsonOutputString =
                                        Admin
                                        && ctx.Request.QuerystringExists("GetAll")
                                        && bool.TryParse(
                                            ctx.Request.RetrieveQueryValue("GetAll"),
                                            out var getAll
                                        )
                                        && getAll
                                            ? JsonConvert.SerializeObject(
                                                await HomeGuestJoiningSystem.GetCrcList(
                                                    clientip,
                                                    null,
                                                    Retail,
                                                    true
                                                ),
                                                Formatting.Indented
                                            )
                                            : JsonConvert.SerializeObject(
                                                await HomeGuestJoiningSystem.GetCrcList(
                                                    clientip,
                                                    AccessToken,
                                                    Retail,
                                                    false
                                                ),
                                                Formatting.Indented
                                            );

                                    await ctx.Response.Send(CrcListJsonOutputString);
                                    return;
                                default:
                                    ctx.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                                    ctx.Response.ContentType = "text/plain";
                                    await ctx.Response.Send();
                                    return;
                            }
                        }
                    }
                );

                _server.Routes.PostAuthentication.Parameter.Add(
                    WatsonWebserver.Core.HttpMethod.GET,
                    "/favicon.ico",
                    async (HttpContextBase ctx) =>
                    {
                        var userAgent = ctx.Request.Useragent;
                        var clientip = ctx.Request.Source.IpAddress;

                        if (
                            (
                                IPAddress.TryParse(clientip, out var clientipAddr)
                                && await HorizonServerConfiguration.Database.GetIsIpBanned(
                                    clientipAddr
                                )
                            )
                            || (
                                !string.IsNullOrEmpty(userAgent)
                                && userAgent.Contains(
                                    "bytespider",
                                    StringComparison.InvariantCultureIgnoreCase
                                )
                            )
                        ) // Get Away TikTok and cheaters.
                        {
                            ctx.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                            ctx.Response.ContentType = "text/plain";
                            await ctx.Response.Send();
                        }
                        else
                        {
                            if (
                                File.Exists(
                                    Directory.GetCurrentDirectory() + "/static/wwwroot/favicon.ico"
                                )
                            )
                            {
                                ctx.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                ctx.Response.ContentType = "image/x-icon";
                                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                                var encoding = ctx.Request.RetrieveHeaderValue("Accept-Encoding");
                                if (!string.IsNullOrEmpty(encoding))
                                {
                                    if (encoding.Contains("zstd"))
                                    {
                                        ctx.Response.Headers.Add("Content-Encoding", "zstd");
                                        await ctx.Response.Send(
                                            HTTPProcessor.CompressZstd(
                                                File.ReadAllBytes(
                                                    Directory.GetCurrentDirectory()
                                                        + "/static/wwwroot/favicon.ico"
                                                )
                                            )
                                        );
                                    }
                                    else if (encoding.Contains("br"))
                                    {
                                        ctx.Response.Headers.Add("Content-Encoding", "br");
                                        await ctx.Response.Send(
                                            HTTPProcessor.CompressBrotli(
                                                File.ReadAllBytes(
                                                    Directory.GetCurrentDirectory()
                                                        + "/static/wwwroot/favicon.ico"
                                                )
                                            )
                                        );
                                    }
                                    else if (encoding.Contains("gzip"))
                                    {
                                        ctx.Response.Headers.Add("Content-Encoding", "gzip");
                                        await ctx.Response.Send(
                                            HTTPProcessor.CompressGzip(
                                                File.ReadAllBytes(
                                                    Directory.GetCurrentDirectory()
                                                        + "/static/wwwroot/favicon.ico"
                                                )
                                            )
                                        );
                                    }
                                    else if (encoding.Contains("deflate"))
                                    {
                                        ctx.Response.Headers.Add("Content-Encoding", "deflate");
                                        await ctx.Response.Send(
                                            HTTPProcessor.Deflate(
                                                File.ReadAllBytes(
                                                    Directory.GetCurrentDirectory()
                                                        + "/static/wwwroot/favicon.ico"
                                                )
                                            )
                                        );
                                    }
                                    else
                                        await ctx.Response.Send(
                                            File.ReadAllBytes(
                                                Directory.GetCurrentDirectory()
                                                    + "/static/wwwroot/favicon.ico"
                                            )
                                        );
                                }
                                else
                                    await ctx.Response.Send(
                                        File.ReadAllBytes(
                                            Directory.GetCurrentDirectory()
                                                + "/static/wwwroot/favicon.ico"
                                        )
                                    );
                            }
                            else
                            {
                                ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                                ctx.Response.ContentType = "text/plain";
                                await ctx.Response.Send();
                            }
                        }
                    }
                );

                _server.Start();

                LoggerAccessor.LogInfo($"CrudHandler Server initiated on port:{_port}...");
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
