using ApacheNet.BuildIn.Extensions;
using ApacheNet.BuildIn.RouteHandlers;
using ApacheNet.Models;
using CustomLogger;
using MultiServerLibrary;
using MultiServerLibrary.Extension;
using MultiServerLibrary.GeoLocalization;
using MultiServerLibrary.HTTP;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using WatsonWebserver;
using WatsonWebserver.Core;
using WebAPIService.WebServices.WebArchive;

namespace ApacheNet
{
    public partial class ApacheNetProcessor
    {
        public const string allowedMethods = "OPTIONS, HEAD, GET, PUT, POST, DELETE, PATCH, PROPFIND";
        public static List<string> allowedOrigins = new() { };

        public readonly static List<Route> Routes = new();

        private static readonly string serverRevision = Assembly.GetExecutingAssembly().GetName().Name + " " + Assembly.GetExecutingAssembly().GetName().Version;

        private readonly Webserver? _server;
        private readonly ushort _port;
        private readonly Thread? _starterThread;

        public ApacheNetProcessor(string certpath, string certpass, string ip, ushort port, bool secure, int MaxConcurrentListeners)
        {
            _port = port;
            WebserverSettings settings = new()
            {
                Hostname = ip,
                Port = port,
            };
            settings.IO.StreamBufferSize = ApacheNetServerConfiguration.BufferSize;
            settings.IO.EnableKeepAlive = ApacheNetServerConfiguration.EnableKeepAlive;
            if (secure)
            {
                settings.Ssl.PfxCertificateFile = certpath;
                settings.Ssl.PfxCertificatePassword = certpass;
                settings.Ssl.Enable = true;
            }
            _server = new Webserver(settings, DefaultRoute, MaxConcurrentListeners)
            {
#if !DEBUG
                LogResponseSentMsg = false,
#endif
                KeepAliveResponseData = false
            };

            _starterThread = new Thread(StartServer)
            {
                Name = "Server Starter"
            };
            _starterThread.Start();
        }

        private static void SetCorsHeaders(HttpContextBase ctx)
        {
            const string originHeader = "Origin";
            string origin = ctx.Request.RetrieveHeaderValue(originHeader);

            if (string.IsNullOrEmpty(origin) || allowedOrigins.Count == 0)
                // Allow requests with no Origin header (e.g., direct server-to-server requests) or if we not set any CORS rules.
                ctx.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            else if (allowedOrigins.Contains(origin))
                // Allow requests with a valid Origin
                ctx.Response.Headers.Add("Access-Control-Allow-Origin", origin);
            else
            {
                ctx.Response.Headers.Add("Access-Control-Deny-Origin", origin);
                return;
            }

            ctx.Response.Headers.Add("Access-Control-Allow-Methods", allowedMethods);
            ctx.Response.Headers.Add("Access-Control-Allow-Headers", "*");
            ctx.Response.Headers.Add("Access-Control-Expose-Headers", string.Empty);
        }

        private static async Task AuthorizeConnection(HttpContextBase ctx)
        {
            string IpToBan = ctx.Request.Source.IpAddress;
            if (!"::1".Equals(IpToBan) && !"127.0.0.1".Equals(IpToBan) && !"localhost".Equals(IpToBan, StringComparison.InvariantCultureIgnoreCase))
            {
                if (!string.IsNullOrEmpty(IpToBan) && ((MultiServerLibraryConfiguration.BannedIPs != null && MultiServerLibraryConfiguration.BannedIPs.Contains(IpToBan))
                    || (MultiServerLibraryConfiguration.VpnCheck != null && MultiServerLibraryConfiguration.VpnCheck.IsVpnOrProxy(IpToBan))))
                {
                    LoggerAccessor.LogError($"[SECURITY] - Client - {ctx.Request.Source.IpAddress}:{ctx.Request.Source.Port} Requested the HTTPS server while being banned!");
                    ctx.Response.StatusCode = 403;
                    await ctx.Response.Send();
                    return;
                }
            }
            const string svoMacHeader = "X-SVOMac";
            if (ctx.Request.HeaderExists(svoMacHeader))
            {
                string? serverMac = CastleLibrary.Sony.SVO.WebSecurityUtils.CalcuateSVOMac(ctx.Request.RetrieveHeaderValue(svoMacHeader));

                if (string.IsNullOrEmpty(serverMac))
                {
                    ctx.Response.StatusCode = 403;
                    await ctx.Response.Send();
                    return;
                }

                ctx.Response.Headers.Set(svoMacHeader, serverMac);
            }
        }

        public void StopServer()
        {
            _server?.Dispose();

            LoggerAccessor.LogWarn($"{(_port.ToString().EndsWith("443") ? "HTTPS" : "HTTP")} Server on port: {_port} stopped...");
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
                PostAuthParameters.Build(_server);

                _server.Start();
                LoggerAccessor.LogInfo($"{(_port.ToString().EndsWith("443") ? "HTTPS" : "HTTP")} Server initiated on port: {_port}...");
            }
        }

        private static bool RouteRequest(ApacheContext ctx, string absolutepath, string Host)
        {
            string? userAgent = ctx.Request.Useragent;
            string? contentType = ctx.Request.ContentType;
            string method = ctx.Request.Method.ToString();

            for (int i = 0; i < Routes.Count; i++)
            {
                Route route = Routes[i];

                // Skip routes that don't match URL if any
                if (string.IsNullOrEmpty(route.UrlRegex) || Regex.IsMatch(absolutepath, route.UrlRegex))
                {
                    // Match criteria early and short-circuit when false
                    if (route.UserAgentCriteria != null &&
                        (string.IsNullOrEmpty(userAgent) || !userAgent.Contains(route.UserAgentCriteria)))
                        continue;

                    if (route.HostCriteria != null && !Host.Contains(route.HostCriteria))
                        continue;

                    if (route.Method != null && route.Method != method)
                        continue;

                    if (route.Hosts != null && route.Hosts.Length > 0 && !route.Hosts.Contains(Host))
                        continue;

                    if (route.ContentTypeCriteria != null &&
                        (string.IsNullOrEmpty(contentType) || !contentType.Contains(route.ContentTypeCriteria)))
                        continue;

                    try
                    {
                        if (route.Callable != null)
                        {
                            bool? result = route.Callable(ctx);
                            if (result.HasValue)
                                return result.Value;
                        }
                    }
                    catch (Exception ex)
                    {
                        LoggerAccessor.LogError($"[ApacheNetProcessor] - RouteRequest match thrown an assertion: (Exception:{ex})");
                    }

                    break;
                }
            }

            return false;
        }


        private static void SetHttpVersion(HttpResponseBase response)
        {
            response.ProtocolVersion = ApacheNetServerConfiguration.HttpVersion;
        }

        private static async Task DefaultRoute(HttpContextBase ctx)
        {
            bool sent = false;
            bool isAllowed = false;
            ApacheContext apacheContext = new(ctx);
            string loggerprefix = apacheContext.Secure ? "HTTPS" : "HTTP";
            string fullurl = apacheContext.FullUrl;
            string absolutepath = apacheContext.AbsolutePath;

            SetHttpVersion(apacheContext.Response);
            SetCorsHeaders(ctx);

            if (!string.IsNullOrEmpty(apacheContext.Request.Useragent) && apacheContext.Request.Useragent.Contains("bytespider", StringComparison.InvariantCultureIgnoreCase)) // Get Away TikTok.
                LoggerAccessor.LogInfo($"[{loggerprefix}] - {apacheContext.ClientIP}:{apacheContext.ClientPort} Requested the {loggerprefix} Server with a ByteDance crawler!");
            else if (fullurl != string.Empty)
            {
                string SuplementalMessage = string.Empty;
                string? GeoCodeString = GeoIP.GetGeoCodeFromIP(IPAddress.Parse(apacheContext.ClientIP));

                if (!string.IsNullOrEmpty(GeoCodeString))
                {
                    string[] parts = GeoCodeString.Split('-');
                    int partsLength = parts.Length;

                    if (partsLength >= 2)
                    {
                        string CountryCode = parts[0];

                        SuplementalMessage = " Located at " + CountryCode + $"{(partsLength == 3 ? $" In City {parts[3]}" : string.Empty)}" + (bool.Parse(parts[1]) ? " Situated in Europe " : string.Empty) + $" ({await WebLocalization.GetOpenStreetMapUrl(apacheContext.ClientIP)})";
                    }
                }
#if DEBUG
                IEnumerable<string> HeadersValue;
                try
                {
                    HeadersValue = ctx.Request.Headers.AllKeys.SelectMany(key => ctx.Request.Headers.GetValues(key) ?? Enumerable.Empty<string>());
                }
                catch (ArgumentNullException)
                {
                    HeadersValue = Enumerable.Empty<string>();
                }
                LoggerAccessor.LogInfo($"[{loggerprefix}] - {apacheContext.ClientIP}:{apacheContext.ClientPort}{SuplementalMessage} Requested the {loggerprefix} Server with URL : {fullurl} (Details: " + JsonConvert.SerializeObject(new
                {
                    HttpMethod = apacheContext.Request.Method,
                    Url = fullurl,
                    Headers = apacheContext.Request.Headers,
                    HeadersValues = HeadersValue,
                    UserAgent = string.IsNullOrEmpty(apacheContext.Request.Useragent) ? string.Empty : apacheContext.Request.Useragent,
                    ClientAddress = apacheContext.ClientIP + ":" + apacheContext.ClientPort,
#if false // Serve as a HTTP json debugging.
                    Body = request.ContentLength >= 0 ? Convert.ToBase64String(apacheContext.Request.DataAsBytes) : string.Empty
#endif
                }, Formatting.Indented) + ") (" + ctx.Timestamp.TotalMs + "ms)");
#else
                    LoggerAccessor.LogInfo($"[{loggerprefix}] - {apacheContext.ClientIP}:{apacheContext.ClientPort}{SuplementalMessage} Requested the {loggerprefix} Server with URL : {fullurl} (" + ctx.Timestamp.TotalMs + "ms)");
#endif
                isAllowed = true;
            }

            if (isAllowed)
            {
                sent = await ApacheRedirector.RedirectRequest(apacheContext, ref absolutepath, ref fullurl).ConfigureAwait(false);

                if (!sent)
                {
                    bool noCompressCacheControl = apacheContext.NoCompressCacheControl;
                    string Host = apacheContext.GetHost();
                    string Accept = apacheContext.Request.RetrieveHeaderValue("Accept");

                    // Split the URL into segments
                    string[] segments = absolutepath.Trim('/').Split('/');

                    // Combine the folder segments into a directory path
                    apacheContext.DirectoryPath = Path.Combine(ApacheNetServerConfiguration.HTTPStaticFolder, string.Join("/", segments.Take(segments.Length - 1).ToArray()));

                    // Process the request based on the HTTP method
                    apacheContext.FilePath = Path.Combine(ApacheNetServerConfiguration.HTTPStaticFolder, absolutepath[1..]);
                    apacheContext.ApiPath = Path.Combine(ApacheNetServerConfiguration.APIStaticFolder, absolutepath[1..]);

                    sent = await ApachePlugin.ProcessPlugin(apacheContext).ConfigureAwait(false);

                    if (!sent && !RouteRequest(apacheContext, absolutepath, Host))
                    {
                        bool isHtmlCompatible = !string.IsNullOrEmpty(Accept) && Accept.Contains("html");

                        string encoding = apacheContext.Request.RetrieveHeaderValue("Accept-Encoding");

                        switch (apacheContext.Request.Method.ToString())
                        {
                            case "GET":
                                switch (absolutepath)
                                {
                                    case "/":
                                        bool root_handled = false;

                                        foreach (string indexFile in HTTPProcessor._DefaultFiles)
                                        {
                                            if (File.Exists(ApacheNetServerConfiguration.HTTPStaticFolder + $"/{indexFile}"))
                                            {
                                                root_handled = true;

                                                if (indexFile.EndsWith(".php") && Directory.Exists(ApacheNetServerConfiguration.PHPStaticFolder))
                                                {
                                                    var CollectPHP = new PHP().ProcessPHPPage(ApacheNetServerConfiguration.HTTPStaticFolder + $"/{indexFile}", ApacheNetServerConfiguration.PHPStaticFolder, ApacheNetServerConfiguration.PHPVersion, ctx, apacheContext.Secure);
                                                    foreach (var innerArray in CollectPHP.Item3)
                                                        apacheContext.Response.Headers.Add(innerArray.Key, innerArray.Value);
                                                    apacheContext.StatusCode = (HttpStatusCode)CollectPHP.Item1;
                                                    apacheContext.Response.ContentType = "text/html";
                                                    apacheContext.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                                    apacheContext.Response.Headers.Add("Last-Modified", File.GetLastWriteTime(ApacheNetServerConfiguration.HTTPStaticFolder + $"/{indexFile}").ToString("r"));
                                                    sent = await apacheContext.SendImmediate(CollectPHP.Item2 ?? Array.Empty<byte>(), apacheContext.AcceptChunked).ConfigureAwait(false);
                                                }
                                                else
                                                {
                                                    using FileStream stream = await FileSystemUtils.TryOpen(ApacheNetServerConfiguration.HTTPStaticFolder + $"/{indexFile}", FileShare.ReadWrite, LocalFileStreamHelper.FileLockAwaitMs).ConfigureAwait(false);
                                                    byte[]? buffer = null;

                                                    using (MemoryStream ms = new())
                                                    {
                                                        stream.CopyTo(ms);
                                                        buffer = ms.ToArray();
                                                        ms.Flush();
                                                    }

                                                    if (buffer != null)
                                                    {
                                                        apacheContext.StatusCode = HttpStatusCode.OK;
                                                        apacheContext.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                                        apacheContext.Response.Headers.Add("Last-Modified", File.GetLastWriteTime(ApacheNetServerConfiguration.HTTPStaticFolder + $"/{indexFile}").ToString("r"));
                                                        apacheContext.Response.ContentType = HTTPProcessor.GetMimeType(Path.GetExtension(ApacheNetServerConfiguration.HTTPStaticFolder + $"/{indexFile}"), ApacheNetServerConfiguration.MimeTypes ?? HTTPProcessor.MimeTypes);
                                                        sent = await apacheContext.SendImmediate(buffer, apacheContext.AcceptChunked).ConfigureAwait(false);

                                                    }
                                                    else
                                                    {
                                                        apacheContext.StatusCode = HttpStatusCode.InternalServerError;
                                                        sent = await apacheContext.SendImmediate().ConfigureAwait(false);
                                                    }

                                                    stream.Flush();
                                                }
                                                break;
                                            }
                                        }

                                        if (!root_handled)
                                        {
                                            apacheContext.StatusCode = HttpStatusCode.NotFound;

                                            if (isHtmlCompatible)
                                            {
                                                string hostToDisplay = string.IsNullOrEmpty(Host) ? (apacheContext.ServerIP.Length > 15 ? "[" + apacheContext.ServerIP + "]" : apacheContext.ServerIP) : Host;
                                                string htmlPage = await DefaultHTMLPages.GenerateErrorPageAsync(apacheContext.StatusCode, absolutepath, $"{(apacheContext.Secure ? "https" : "http")}://{hostToDisplay}",
                                                    ApacheNetServerConfiguration.HTTPStaticFolder, serverRevision, hostToDisplay, apacheContext.ServerPort);

                                                apacheContext.Response.ContentType = "text/html";
                                                sent = await apacheContext.SendImmediate(htmlPage, apacheContext.AcceptChunked).ConfigureAwait(false);

                                            }
                                            else
                                                sent = await apacheContext.SendImmediate().ConfigureAwait(false);
                                        }
                                        break;
                                    case "/dns-query":
                                        sent = await DOHRequestHandler.DohRequest(apacheContext, Accept, true).ConfigureAwait(false);
                                        break;
                                    case "/networktest/get_2m":
                                        apacheContext.StatusCode = HttpStatusCode.OK;
                                        sent = await apacheContext.SendImmediate(new byte[2097152]).ConfigureAwait(false);
                                        break;
                                    default:
                                        if (Directory.Exists(apacheContext.FilePath))
                                        {
                                            bool endsWithSlash = apacheContext.FilePath.EndsWith("/");
                                            if (!endsWithSlash)
                                            {
                                                byte[] movedPayloadBytes = Encoding.Latin1.GetBytes($@"
                                                        <!DOCTYPE HTML PUBLIC ""-//IETF//DTD HTML 2.0//EN"">
                                                        <html><head>
                                                        <title>301 Moved Permanently</title>
                                                        </head><body>
                                                        <h1>Moved Permanently</h1>
                                                        <p>The document has moved <a href=""{(apacheContext.Secure ? "https" : "http")}://{Host}{absolutepath}/"">here</a>.</p>
                                                        <hr>
                                                        <address>{apacheContext.ServerIP} Port {apacheContext.ServerPort}</address>
                                                        </body></html>");
                                                apacheContext.StatusCode = HttpStatusCode.MovedPermanently;
                                                apacheContext.Response.Headers.Add("Location", $"{(apacheContext.Secure ? "https" : "http")}://{Host}{absolutepath}/{HTTPProcessor.ProcessQueryString(fullurl, true)}");
                                                apacheContext.Response.ContentType = "text/html; charset=iso-8859-1";
                                                if (ApacheNetServerConfiguration.EnableHTTPCompression && !noCompressCacheControl && !string.IsNullOrEmpty(encoding))
                                                {
                                                    if (encoding.Contains("zstd"))
                                                    {
                                                        apacheContext.Response.Headers.Add("Content-Encoding", "zstd");
                                                        movedPayloadBytes = HTTPProcessor.CompressZstd(movedPayloadBytes);
                                                    }
                                                    else if (encoding.Contains("br"))
                                                    {
                                                        apacheContext.Response.Headers.Add("Content-Encoding", "br");
                                                        movedPayloadBytes = HTTPProcessor.CompressBrotli(movedPayloadBytes);
                                                    }
                                                    else if (encoding.Contains("gzip"))
                                                    {
                                                        apacheContext.Response.Headers.Add("Content-Encoding", "gzip");
                                                        movedPayloadBytes = HTTPProcessor.CompressGzip(movedPayloadBytes);
                                                    }
                                                    else if (encoding.Contains("deflate"))
                                                    {
                                                        apacheContext.Response.Headers.Add("Content-Encoding", "deflate");
                                                        movedPayloadBytes = HTTPProcessor.Deflate(movedPayloadBytes);
                                                    }
                                                }
                                                sent = await apacheContext.SendImmediate(movedPayloadBytes, apacheContext.AcceptChunked).ConfigureAwait(false);
                                            }
                                            else if (apacheContext.Request.RetrieveQueryValue("directory") == "on")
                                            {
                                                apacheContext.StatusCode = HttpStatusCode.OK;
                                                apacheContext.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                                apacheContext.Response.ContentType = isHtmlCompatible ? "text/html" : "application/json" + ";charset=utf-8";
                                                byte[] reportOutputBytes = Encoding.UTF8.GetBytes(await FileStructureFormater.GetFileStructureAsync(endsWithSlash ? apacheContext.FilePath[..^1] : apacheContext.FilePath, $"{(apacheContext.Secure ? "https" : "http")}://{Host}{(endsWithSlash ? absolutepath[..^1] : absolutepath)}",
                                                    apacheContext.ServerPort, isHtmlCompatible, ApacheNetServerConfiguration.NestedDirectoryReporting, apacheContext.Request.RetrieveQueryValue("properties") == "on", ApacheNetServerConfiguration.MimeTypes ?? HTTPProcessor.MimeTypes).ConfigureAwait(false)
                                                     ?? await DefaultHTMLPages.GenerateErrorPageAsync(HttpStatusCode.InternalServerError, absolutepath, $"{(apacheContext.Secure ? "https" : "http")}://{Host}",
                                                            ApacheNetServerConfiguration.HTTPStaticFolder, serverRevision, Host, apacheContext.ServerPort).ConfigureAwait(false));
                                                if (ApacheNetServerConfiguration.EnableHTTPCompression && !noCompressCacheControl && !string.IsNullOrEmpty(encoding))
                                                {
                                                    if (encoding.Contains("zstd"))
                                                    {
                                                        apacheContext.Response.Headers.Add("Content-Encoding", "zstd");
                                                        reportOutputBytes = HTTPProcessor.CompressZstd(reportOutputBytes);
                                                    }
                                                    else if (encoding.Contains("br"))
                                                    {
                                                        apacheContext.Response.Headers.Add("Content-Encoding", "br");
                                                        reportOutputBytes = HTTPProcessor.CompressBrotli(reportOutputBytes);
                                                    }
                                                    else if (encoding.Contains("gzip"))
                                                    {
                                                        apacheContext.Response.Headers.Add("Content-Encoding", "gzip");
                                                        reportOutputBytes = HTTPProcessor.CompressGzip(reportOutputBytes);
                                                    }
                                                    else if (encoding.Contains("deflate"))
                                                    {
                                                        apacheContext.Response.Headers.Add("Content-Encoding", "deflate");
                                                        reportOutputBytes = HTTPProcessor.Deflate(reportOutputBytes);
                                                    }
                                                }
                                                sent = await apacheContext.SendImmediate(reportOutputBytes, apacheContext.AcceptChunked).ConfigureAwait(false);

                                            }
                                            else if (apacheContext.Request.RetrieveQueryValue("m3u") == "on")
                                            {
                                                string? m3ufile = FileSystemUtils.GetM3UStreamFromDirectory(endsWithSlash ? apacheContext.FilePath[..^1] : apacheContext.FilePath, $"{(apacheContext.Secure ? "https" : "http")}://{Host}{(endsWithSlash ? absolutepath[..^1] : absolutepath)}");
                                                if (!string.IsNullOrEmpty(m3ufile))
                                                {
                                                    apacheContext.StatusCode = HttpStatusCode.OK;
                                                    apacheContext.Response.ContentType = "audio/x-mpegurl";
                                                    sent = await apacheContext.SendImmediate(m3ufile, apacheContext.AcceptChunked).ConfigureAwait(false);
                                                }
                                                else
                                                {
                                                    apacheContext.StatusCode = HttpStatusCode.NoContent;
                                                    sent = await apacheContext.SendImmediate().ConfigureAwait(false);
                                                }
                                            }
                                            else
                                            {
                                                bool handled = false;

                                                foreach (string indexFile in HTTPProcessor._DefaultFiles)
                                                {
                                                    if (File.Exists(apacheContext.FilePath + indexFile))
                                                    {
                                                        handled = true;

                                                        if (indexFile.EndsWith(".php") && Directory.Exists(ApacheNetServerConfiguration.PHPStaticFolder))
                                                        {
                                                            var CollectPHP = new PHP().ProcessPHPPage(apacheContext.FilePath + indexFile, ApacheNetServerConfiguration.PHPStaticFolder, ApacheNetServerConfiguration.PHPVersion, ctx, apacheContext.Secure);
                                                            foreach (var innerArray in CollectPHP.Item3)
                                                                apacheContext.Response.Headers.Add(innerArray.Key, innerArray.Value);
                                                            apacheContext.StatusCode = (HttpStatusCode)CollectPHP.Item1;
                                                            apacheContext.Response.ContentType = "text/html";
                                                            apacheContext.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                                            apacheContext.Response.Headers.Add("Last-Modified", File.GetLastWriteTime(apacheContext.FilePath + indexFile).ToString("r"));
                                                            sent = await apacheContext.SendImmediate(CollectPHP.Item2 ?? Array.Empty<byte>(), apacheContext.AcceptChunked).ConfigureAwait(false);
                                                        }
                                                        else
                                                        {
                                                            using FileStream stream = await FileSystemUtils.TryOpen(apacheContext.FilePath + indexFile, FileShare.ReadWrite, LocalFileStreamHelper.FileLockAwaitMs).ConfigureAwait(false);
                                                            byte[]? buffer = null;

                                                            using (MemoryStream ms = new())
                                                            {
                                                                stream.CopyTo(ms);
                                                                buffer = ms.ToArray();
                                                                ms.Flush();
                                                            }

                                                            if (buffer != null)
                                                            {
                                                                apacheContext.StatusCode = HttpStatusCode.OK;
                                                                apacheContext.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                                                apacheContext.Response.Headers.Add("Last-Modified", File.GetLastWriteTime(apacheContext.FilePath + indexFile).ToString("r"));
                                                                apacheContext.Response.ContentType = HTTPProcessor.GetMimeType(Path.GetExtension(apacheContext.FilePath + indexFile), ApacheNetServerConfiguration.MimeTypes ?? HTTPProcessor.MimeTypes);
                                                                sent = await apacheContext.SendImmediate(buffer, apacheContext.AcceptChunked).ConfigureAwait(false);

                                                            }
                                                            else
                                                            {
                                                                apacheContext.StatusCode = HttpStatusCode.InternalServerError;
                                                                sent = await apacheContext.SendImmediate().ConfigureAwait(false);
                                                            }

                                                            stream.Flush();
                                                        }
                                                        break;
                                                    }
                                                }

                                                if (!handled)
                                                {
                                                    apacheContext.StatusCode = HttpStatusCode.NotFound;

                                                    if (isHtmlCompatible)
                                                    {
                                                        string hostToDisplay = string.IsNullOrEmpty(Host) ? (apacheContext.ServerIP.Length > 15 ? "[" + apacheContext.ServerIP + "]" : apacheContext.ServerIP) : Host;
                                                        string htmlPage = await DefaultHTMLPages.GenerateErrorPageAsync(apacheContext.StatusCode, absolutepath, $"{(apacheContext.Secure ? "https" : "http")}://{hostToDisplay}",
                                                            ApacheNetServerConfiguration.HTTPStaticFolder, serverRevision, hostToDisplay, apacheContext.ServerPort);

                                                        apacheContext.Response.ContentType = "text/html";
                                                        sent = await apacheContext.SendImmediate(htmlPage, apacheContext.AcceptChunked).ConfigureAwait(false);

                                                    }
                                                    else
                                                        sent = await apacheContext.SendImmediate().ConfigureAwait(false);
                                                }
                                            }
                                        }
                                        else if ((absolutepath.EndsWith(".asp", StringComparison.InvariantCultureIgnoreCase) || absolutepath.EndsWith(".aspx", StringComparison.InvariantCultureIgnoreCase)) && !string.IsNullOrEmpty(ApacheNetServerConfiguration.ASPNETRedirectUrl))
                                        {
                                            apacheContext.Response.Headers.Add("Location", $"{ApacheNetServerConfiguration.ASPNETRedirectUrl}{HttpUtility.UrlEncode(apacheContext.FullUrl)}");
                                            apacheContext.StatusCode = HttpStatusCode.PermanentRedirect;
                                            sent = await apacheContext.SendImmediate().ConfigureAwait(false);
                                        }
                                        else if (absolutepath.EndsWith(".php", StringComparison.InvariantCultureIgnoreCase) && !string.IsNullOrEmpty(ApacheNetServerConfiguration.PHPRedirectUrl))
                                        {
                                            apacheContext.Response.Headers.Add("Location", $"{ApacheNetServerConfiguration.PHPRedirectUrl}{HttpUtility.UrlEncode(apacheContext.FullUrl)}");
                                            apacheContext.StatusCode = HttpStatusCode.PermanentRedirect;
                                            sent = await apacheContext.SendImmediate().ConfigureAwait(false);
                                        }
                                        else if (absolutepath.EndsWith(".php", StringComparison.InvariantCultureIgnoreCase) && Directory.Exists(ApacheNetServerConfiguration.PHPStaticFolder) && (File.Exists(apacheContext.FilePath) || File.Exists(apacheContext.ApiPath)))
                                        {
                                            (int, byte[]?, Dictionary<string, string>) CollectPHP;
                                            bool isOnWWWRoot = File.Exists(apacheContext.FilePath);
                                            if (isOnWWWRoot)
                                                CollectPHP = new PHP().ProcessPHPPage(apacheContext.FilePath, ApacheNetServerConfiguration.PHPStaticFolder, ApacheNetServerConfiguration.PHPVersion, ctx, apacheContext.Secure);
                                            else
                                                CollectPHP = new PHP().ProcessPHPPage(apacheContext.ApiPath, ApacheNetServerConfiguration.PHPStaticFolder, ApacheNetServerConfiguration.PHPVersion, ctx, apacheContext.Secure);
                                            foreach (var innerArray in CollectPHP.Item3)
                                                apacheContext.Response.Headers.Add(innerArray.Key, innerArray.Value);
                                            apacheContext.StatusCode = (HttpStatusCode)CollectPHP.Item1;
                                            apacheContext.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                            apacheContext.Response.Headers.Add("Last-Modified", File.GetLastWriteTime(apacheContext.FilePath).ToString("r"));
                                            apacheContext.Response.ContentType = "text/html";
                                            sent = await apacheContext.SendImmediate(CollectPHP.Item2 ?? Array.Empty<byte>(), apacheContext.AcceptChunked).ConfigureAwait(false);
                                        }
                                        else if (File.Exists(apacheContext.FilePath))
                                        {
                                            string ContentType = HTTPProcessor.GetMimeType(Path.GetExtension(apacheContext.FilePath), ApacheNetServerConfiguration.MimeTypes ?? HTTPProcessor.MimeTypes);
                                            if (ContentType == "application/octet-stream")
                                            {
                                                byte[] VerificationChunck = FileSystemUtils.TryReadFileChunck(apacheContext.FilePath, 10, FileShare.ReadWrite, LocalFileStreamHelper.FileLockAwaitMs);
                                                foreach (var entry in HTTPProcessor.PathernDictionary)
                                                {
                                                    if (ByteUtils.FindBytePattern(VerificationChunck, entry.Value) != -1)
                                                    {
                                                        ContentType = entry.Key;
                                                        break;
                                                    }
                                                }
                                            }

                                            if (ApacheNetServerConfiguration.RangeHandling && !string.IsNullOrEmpty(apacheContext.Request.RetrieveHeaderValue("Range")))
                                                sent = await LocalFileStreamHelper.HandlePartialRangeRequest(apacheContext, apacheContext.FilePath, ContentType, noCompressCacheControl);
                                            else
                                            {
                                                // send file
                                                LoggerAccessor.LogInfo($"[{loggerprefix}] - {apacheContext.ClientIP}:{apacheContext.ClientPort} Requested a file : {absolutepath}");

                                                sent = await LocalFileStreamHelper.HandleRequest(apacheContext, encoding, absolutepath, apacheContext.FilePath, ContentType, apacheContext.Request.Useragent, ContentType.StartsWith("video/") || ContentType.StartsWith("audio/"), isHtmlCompatible, noCompressCacheControl);
                                            }
                                        }
                                        else
                                        {
                                            bool ArchiveOrgProcessed = false;

                                            if (ApacheNetServerConfiguration.NotFoundWebArchive && !string.IsNullOrEmpty(Host) && !Host.Equals("web.archive.org") && !Host.Equals("archive.org"))
                                            {
                                                WebArchiveRequest archiveReq = new($"{(apacheContext.Secure ? "https" : "http")}://{Host}" + fullurl);
                                                if (archiveReq.Archived)
                                                {
                                                    const string archivedSourceHeaderKey = "x-archive-src";
                                                    byte[] archiveToolbarPayload = Encoding.UTF8.GetBytes("<!-- END WAYBACK TOOLBAR INSERT -->\n ");

                                                    ArchiveOrgProcessed = true;
                                                    apacheContext.StatusCode = HttpStatusCode.OK;
                                                    apacheContext.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                                    var archivedData = HTTPProcessor.RequestFullURLGET(archiveReq.ArchivedURL);
                                                    if (archivedData.headers.ContainsKey(archivedSourceHeaderKey))
                                                        apacheContext.Response.Headers.Add(archivedSourceHeaderKey, "https://archive.org/download/" + archivedData.headers[archivedSourceHeaderKey]);
                                                    if (archivedData.headers.ContainsKey("Content-Type"))
                                                        apacheContext.Response.ContentType = archivedData.headers["Content-Type"];
                                                    else
                                                        apacheContext.Response.ContentType = HTTPProcessor.GetMimeType(Path.GetExtension(apacheContext.FilePath), ApacheNetServerConfiguration.MimeTypes ?? HTTPProcessor.MimeTypes);
                                                    int archiveToolbarPos = ByteUtils.FindBytePattern(archivedData.data, archiveToolbarPayload);
                                                    int archiveFooterPos = ByteUtils.FindBytePattern(archivedData.data, Encoding.UTF8.GetBytes("<!--\n     FILE ARCHIVED ON "));
                                                    byte[] rawDataPayload;
                                                    if (archiveToolbarPos != -1 && archiveFooterPos != -1 && archiveToolbarPos < archiveFooterPos)
                                                    {
                                                        // Calculate start of content: after the toolbar marker
                                                        int contentStart = archiveToolbarPos + archiveToolbarPayload.Length;

                                                        // Calculate length of content between markers
                                                        int contentLength = archiveFooterPos - contentStart;

                                                        // Copy that range into new byte array
                                                        rawDataPayload = new byte[contentLength];
                                                        Array.Copy(archivedData.data, contentStart, rawDataPayload, 0, contentLength);
                                                    }
                                                    else
                                                        rawDataPayload = archivedData.data;
                                                    if (ApacheNetServerConfiguration.EnableHTTPCompression && !noCompressCacheControl && !string.IsNullOrEmpty(encoding) && rawDataPayload.Length <= LocalFileStreamHelper.compressionSizeLimit)
                                                    {
                                                        if (encoding.Contains("zstd"))
                                                        {
                                                            apacheContext.Response.Headers.Add("Content-Encoding", "zstd");
                                                            rawDataPayload = HTTPProcessor.CompressZstd(rawDataPayload);
                                                        }
                                                        else if (encoding.Contains("br"))
                                                        {
                                                            apacheContext.Response.Headers.Add("Content-Encoding", "br");
                                                            rawDataPayload = HTTPProcessor.CompressBrotli(rawDataPayload);
                                                        }
                                                        else if (encoding.Contains("gzip"))
                                                        {
                                                            apacheContext.Response.Headers.Add("Content-Encoding", "gzip");
                                                            rawDataPayload = HTTPProcessor.CompressGzip(rawDataPayload);
                                                        }
                                                        else if (encoding.Contains("deflate"))
                                                        {
                                                            apacheContext.Response.Headers.Add("Content-Encoding", "deflate");
                                                            rawDataPayload = HTTPProcessor.Deflate(rawDataPayload);
                                                        }
                                                    }
                                                    sent = await apacheContext.SendImmediate(rawDataPayload, apacheContext.AcceptChunked).ConfigureAwait(false);
                                                }
                                            }

                                            if (!ArchiveOrgProcessed)
                                            {
                                                apacheContext.StatusCode = HttpStatusCode.NotFound;

                                                if (isHtmlCompatible)
                                                {
                                                    string hostToDisplay = string.IsNullOrEmpty(Host) ? (apacheContext.ServerIP.Length > 15 ? "[" + apacheContext.ServerIP + "]" : apacheContext.ServerIP) : Host;
                                                    apacheContext.Response.ContentType = "text/html";
                                                    sent = await apacheContext.SendImmediate(await DefaultHTMLPages.GenerateErrorPageAsync(apacheContext.StatusCode, absolutepath, $"{(apacheContext.Secure ? "https" : "http")}://{hostToDisplay}",
                                                        ApacheNetServerConfiguration.HTTPStaticFolder, serverRevision, hostToDisplay, apacheContext.ServerPort), apacheContext.AcceptChunked).ConfigureAwait(false);
                                                }
                                                else
                                                    sent = await apacheContext.SendImmediate().ConfigureAwait(false);
                                            }
                                        }
                                        break;
                                }
                                break;
                            case "POST":
                                switch (absolutepath)
                                {
                                    case "/":
                                        bool root_handled = false;

                                        foreach (string indexFile in HTTPProcessor._DefaultFiles)
                                        {
                                            if (File.Exists(ApacheNetServerConfiguration.HTTPStaticFolder + $"/{indexFile}"))
                                            {
                                                root_handled = true;

                                                if (indexFile.EndsWith(".php") && Directory.Exists(ApacheNetServerConfiguration.PHPStaticFolder))
                                                {
                                                    var CollectPHP = new PHP().ProcessPHPPage(ApacheNetServerConfiguration.HTTPStaticFolder + $"/{indexFile}", ApacheNetServerConfiguration.PHPStaticFolder, ApacheNetServerConfiguration.PHPVersion, ctx, apacheContext.Secure);
                                                    foreach (var innerArray in CollectPHP.Item3)
                                                        apacheContext.Response.Headers.Add(innerArray.Key, innerArray.Value);
                                                    apacheContext.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                                    apacheContext.Response.Headers.Add("Last-Modified", File.GetLastWriteTime(ApacheNetServerConfiguration.HTTPStaticFolder + $"/{indexFile}").ToString("r"));
                                                    apacheContext.StatusCode = (HttpStatusCode)CollectPHP.Item1;
                                                    apacheContext.Response.ContentType = "text/html";
                                                    sent = await apacheContext.SendImmediate(CollectPHP.Item2 ?? Array.Empty<byte>(), apacheContext.AcceptChunked).ConfigureAwait(false);
                                                }
                                                else
                                                {
                                                    using FileStream stream = await FileSystemUtils.TryOpen(ApacheNetServerConfiguration.HTTPStaticFolder + $"/{indexFile}", FileShare.ReadWrite, LocalFileStreamHelper.FileLockAwaitMs).ConfigureAwait(false);
                                                    byte[]? buffer = null;

                                                    using (MemoryStream ms = new())
                                                    {
                                                        stream.CopyTo(ms);
                                                        buffer = ms.ToArray();
                                                        ms.Flush();
                                                    }

                                                    if (buffer != null)
                                                    {
                                                        apacheContext.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                                        apacheContext.Response.Headers.Add("Last-Modified", File.GetLastWriteTime(ApacheNetServerConfiguration.HTTPStaticFolder + $"/{indexFile}").ToString("r"));
                                                        apacheContext.StatusCode = HttpStatusCode.OK;
                                                        apacheContext.Response.ContentType = HTTPProcessor.GetMimeType(Path.GetExtension(ApacheNetServerConfiguration.HTTPStaticFolder + $"/{indexFile}"), ApacheNetServerConfiguration.MimeTypes ?? HTTPProcessor.MimeTypes);
                                                        sent = await apacheContext.SendImmediate(buffer, apacheContext.AcceptChunked).ConfigureAwait(false);
                                                    }
                                                    else
                                                    {
                                                        apacheContext.StatusCode = HttpStatusCode.InternalServerError;
                                                        sent = await apacheContext.SendImmediate().ConfigureAwait(false);
                                                    }

                                                    stream.Flush();
                                                }
                                                break;
                                            }
                                        }

                                        if (!root_handled)
                                        {
                                            apacheContext.StatusCode = HttpStatusCode.NotFound;

                                            if (isHtmlCompatible)
                                            {
                                                string hostToDisplay = string.IsNullOrEmpty(Host) ? (apacheContext.ServerIP.Length > 15 ? "[" + apacheContext.ServerIP + "]" : apacheContext.ServerIP) : Host;
                                                string htmlPage = await DefaultHTMLPages.GenerateErrorPageAsync(apacheContext.StatusCode, absolutepath, $"{(apacheContext.Secure ? "https" : "http")}://{hostToDisplay}",
                                                    ApacheNetServerConfiguration.HTTPStaticFolder, serverRevision, hostToDisplay, apacheContext.ServerPort);

                                                apacheContext.Response.ContentType = "text/html";
                                                sent = await apacheContext.SendImmediate(htmlPage, apacheContext.AcceptChunked).ConfigureAwait(false);
                                            }
                                            else
                                                sent = await apacheContext.SendImmediate().ConfigureAwait(false);
                                        }
                                        break;
                                    case "/dns-query":
                                        await DOHRequestHandler.DohRequest(apacheContext, Accept, false).ConfigureAwait(false);
                                        break;
                                    default:
                                        if (Directory.Exists(apacheContext.FilePath))
                                        {
                                            bool endsWithSlash = apacheContext.FilePath.EndsWith("/");
                                            if (!endsWithSlash)
                                            {
                                                byte[] movedPayloadBytes = Encoding.Latin1.GetBytes($@"
                                                        <!DOCTYPE HTML PUBLIC ""-//IETF//DTD HTML 2.0//EN"">
                                                        <html><head>
                                                        <title>301 Moved Permanently</title>
                                                        </head><body>
                                                        <h1>Moved Permanently</h1>
                                                        <p>The document has moved <a href=""{(apacheContext.Secure ? "https" : "http")}://{Host}{absolutepath}/"">here</a>.</p>
                                                        <hr>
                                                        <address>{apacheContext.ServerIP} Port {apacheContext.ServerPort}</address>
                                                        </body></html>");
                                                apacheContext.StatusCode = HttpStatusCode.MovedPermanently;
                                                apacheContext.Response.Headers.Add("Location", $"{(apacheContext.Secure ? "https" : "http")}://{Host}{absolutepath}/{HTTPProcessor.ProcessQueryString(fullurl, true)}");
                                                apacheContext.Response.ContentType = "text/html; charset=iso-8859-1";
                                                if (ApacheNetServerConfiguration.EnableHTTPCompression && !noCompressCacheControl && !string.IsNullOrEmpty(encoding))
                                                {
                                                    if (encoding.Contains("zstd"))
                                                    {
                                                        apacheContext.Response.Headers.Add("Content-Encoding", "zstd");
                                                        movedPayloadBytes = HTTPProcessor.CompressZstd(movedPayloadBytes);
                                                    }
                                                    else if (encoding.Contains("br"))
                                                    {
                                                        apacheContext.Response.Headers.Add("Content-Encoding", "br");
                                                        movedPayloadBytes = HTTPProcessor.CompressBrotli(movedPayloadBytes);
                                                    }
                                                    else if (encoding.Contains("gzip"))
                                                    {
                                                        apacheContext.Response.Headers.Add("Content-Encoding", "gzip");
                                                        movedPayloadBytes = HTTPProcessor.CompressGzip(movedPayloadBytes);
                                                    }
                                                    else if (encoding.Contains("deflate"))
                                                    {
                                                        apacheContext.Response.Headers.Add("Content-Encoding", "deflate");
                                                        movedPayloadBytes = HTTPProcessor.Deflate(movedPayloadBytes);
                                                    }
                                                }
                                                sent = await apacheContext.SendImmediate(movedPayloadBytes, apacheContext.AcceptChunked).ConfigureAwait(false);
                                            }
                                            else if (apacheContext.Request.RetrieveQueryValue("directory") == "on")
                                            {
                                                apacheContext.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                                apacheContext.StatusCode = HttpStatusCode.OK;
                                                apacheContext.Response.ContentType = isHtmlCompatible ? "text/html" : "application/json" + ";charset=utf-8";
                                                byte[] reportOutputBytes = Encoding.UTF8.GetBytes(await FileStructureFormater.GetFileStructureAsync(endsWithSlash ? apacheContext.FilePath[..^1] : apacheContext.FilePath, $"{(apacheContext.Secure ? "https" : "http")}://{Host}{(endsWithSlash ? absolutepath[..^1] : absolutepath)}",
                                                    apacheContext.ServerPort, isHtmlCompatible, ApacheNetServerConfiguration.NestedDirectoryReporting, apacheContext.Request.RetrieveQueryValue("properties") == "on", ApacheNetServerConfiguration.MimeTypes ?? HTTPProcessor.MimeTypes).ConfigureAwait(false)
                                                    ?? await DefaultHTMLPages.GenerateErrorPageAsync(HttpStatusCode.InternalServerError, absolutepath, $"{(apacheContext.Secure ? "https" : "http")}://{Host}",
                                                            ApacheNetServerConfiguration.HTTPStaticFolder, serverRevision, Host, apacheContext.ServerPort).ConfigureAwait(false));
                                                if (ApacheNetServerConfiguration.EnableHTTPCompression && !noCompressCacheControl && !string.IsNullOrEmpty(encoding))
                                                {
                                                    if (encoding.Contains("zstd"))
                                                    {
                                                        apacheContext.Response.Headers.Add("Content-Encoding", "zstd");
                                                        reportOutputBytes = HTTPProcessor.CompressZstd(reportOutputBytes);
                                                    }
                                                    else if (encoding.Contains("br"))
                                                    {
                                                        apacheContext.Response.Headers.Add("Content-Encoding", "br");
                                                        reportOutputBytes = HTTPProcessor.CompressBrotli(reportOutputBytes);
                                                    }
                                                    else if (encoding.Contains("gzip"))
                                                    {
                                                        apacheContext.Response.Headers.Add("Content-Encoding", "gzip");
                                                        reportOutputBytes = HTTPProcessor.CompressGzip(reportOutputBytes);
                                                    }
                                                    else if (encoding.Contains("deflate"))
                                                    {
                                                        apacheContext.Response.Headers.Add("Content-Encoding", "deflate");
                                                        reportOutputBytes = HTTPProcessor.Deflate(reportOutputBytes);
                                                    }
                                                }
                                                sent = await apacheContext.SendImmediate(reportOutputBytes, apacheContext.AcceptChunked).ConfigureAwait(false);
                                            }
                                            else if (apacheContext.Request.RetrieveQueryValue("m3u") == "on")
                                            {
                                                string? m3ufile = FileSystemUtils.GetM3UStreamFromDirectory(endsWithSlash ? apacheContext.FilePath[..^1] : apacheContext.FilePath, $"{(apacheContext.Secure ? "https" : "http")}://{Host}{(endsWithSlash ? absolutepath[..^1] : absolutepath)}");
                                                if (!string.IsNullOrEmpty(m3ufile))
                                                {
                                                    apacheContext.StatusCode = HttpStatusCode.OK;
                                                    apacheContext.Response.ContentType = "audio/x-mpegurl";
                                                    sent = await apacheContext.SendImmediate(m3ufile, apacheContext.AcceptChunked).ConfigureAwait(false);
                                                }
                                                else
                                                {
                                                    apacheContext.StatusCode = HttpStatusCode.NoContent;
                                                    sent = await apacheContext.SendImmediate().ConfigureAwait(false);
                                                }
                                            }
                                            else
                                            {
                                                bool handled = false;

                                                foreach (string indexFile in HTTPProcessor._DefaultFiles)
                                                {
                                                    if (File.Exists(apacheContext.FilePath + indexFile))
                                                    {
                                                        handled = true;

                                                        if (indexFile.EndsWith(".php") && Directory.Exists(ApacheNetServerConfiguration.PHPStaticFolder))
                                                        {
                                                            var CollectPHP = new PHP().ProcessPHPPage(apacheContext.FilePath + indexFile, ApacheNetServerConfiguration.PHPStaticFolder, ApacheNetServerConfiguration.PHPVersion, ctx, apacheContext.Secure);
                                                            foreach (var innerArray in CollectPHP.Item3)
                                                                apacheContext.Response.Headers.Add(innerArray.Key, innerArray.Value);
                                                            apacheContext.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                                            apacheContext.Response.Headers.Add("Last-Modified", File.GetLastWriteTime(apacheContext.FilePath + indexFile).ToString("r"));
                                                            apacheContext.StatusCode = (HttpStatusCode)CollectPHP.Item1;
                                                            apacheContext.Response.ContentType = "text/html";
                                                            sent = await apacheContext.SendImmediate(CollectPHP.Item2 ?? Array.Empty<byte>(), apacheContext.AcceptChunked).ConfigureAwait(false);
                                                        }
                                                        else
                                                        {
                                                            using FileStream stream = await FileSystemUtils.TryOpen(apacheContext.FilePath + indexFile, FileShare.ReadWrite, LocalFileStreamHelper.FileLockAwaitMs).ConfigureAwait(false);
                                                            byte[]? buffer = null;

                                                            using (MemoryStream ms = new())
                                                            {
                                                                stream.CopyTo(ms);
                                                                buffer = ms.ToArray();
                                                                ms.Flush();
                                                            }

                                                            if (buffer != null)
                                                            {
                                                                apacheContext.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                                                apacheContext.Response.Headers.Add("Last-Modified", File.GetLastWriteTime(apacheContext.FilePath + indexFile).ToString("r"));
                                                                apacheContext.StatusCode = HttpStatusCode.OK;
                                                                apacheContext.Response.ContentType = HTTPProcessor.GetMimeType(Path.GetExtension(apacheContext.FilePath + indexFile), ApacheNetServerConfiguration.MimeTypes ?? HTTPProcessor.MimeTypes);
                                                                sent = await apacheContext.SendImmediate(buffer, apacheContext.AcceptChunked).ConfigureAwait(false);
                                                            }
                                                            else
                                                            {
                                                                apacheContext.StatusCode = HttpStatusCode.InternalServerError;
                                                                sent = await apacheContext.SendImmediate().ConfigureAwait(false);
                                                            }

                                                            stream.Flush();
                                                        }
                                                        break;
                                                    }
                                                }

                                                if (!handled)
                                                {
                                                    apacheContext.StatusCode = HttpStatusCode.NotFound;

                                                    if (isHtmlCompatible)
                                                    {
                                                        string hostToDisplay = string.IsNullOrEmpty(Host) ? (apacheContext.ServerIP.Length > 15 ? "[" + apacheContext.ServerIP + "]" : apacheContext.ServerIP) : Host;
                                                        string htmlPage = await DefaultHTMLPages.GenerateErrorPageAsync(apacheContext.StatusCode, absolutepath, $"{(apacheContext.Secure ? "https" : "http")}://{hostToDisplay}",
                                                            ApacheNetServerConfiguration.HTTPStaticFolder, serverRevision, hostToDisplay, apacheContext.ServerPort);

                                                        apacheContext.Response.ContentType = "text/html";
                                                        sent = await apacheContext.SendImmediate(htmlPage, apacheContext.AcceptChunked).ConfigureAwait(false);
                                                    }
                                                    else
                                                        sent = await apacheContext.SendImmediate().ConfigureAwait(false);
                                                }
                                            }
                                        }
                                        else if ((absolutepath.EndsWith(".asp", StringComparison.InvariantCultureIgnoreCase) || absolutepath.EndsWith(".aspx", StringComparison.InvariantCultureIgnoreCase)) && !string.IsNullOrEmpty(ApacheNetServerConfiguration.ASPNETRedirectUrl))
                                        {
                                            apacheContext.Response.Headers.Add("Location", $"{ApacheNetServerConfiguration.ASPNETRedirectUrl}{HttpUtility.UrlEncode(apacheContext.FullUrl)}");
                                            apacheContext.StatusCode = HttpStatusCode.PermanentRedirect;
                                            sent = await apacheContext.SendImmediate().ConfigureAwait(false);

                                        }
                                        else if (absolutepath.EndsWith(".php", StringComparison.InvariantCultureIgnoreCase) && !string.IsNullOrEmpty(ApacheNetServerConfiguration.PHPRedirectUrl))
                                        {
                                            apacheContext.Response.Headers.Add("Location", $"{ApacheNetServerConfiguration.PHPRedirectUrl}{HttpUtility.UrlEncode(apacheContext.FullUrl)}");
                                            apacheContext.StatusCode = HttpStatusCode.PermanentRedirect;
                                            sent = await apacheContext.SendImmediate().ConfigureAwait(false);
                                        }
                                        else if (absolutepath.EndsWith(".php", StringComparison.InvariantCultureIgnoreCase) && Directory.Exists(ApacheNetServerConfiguration.PHPStaticFolder) && (File.Exists(apacheContext.FilePath) || File.Exists(apacheContext.ApiPath)))
                                        {
                                            (int, byte[]?, Dictionary<string, string>) CollectPHP;
                                            bool isOnWWWRoot = File.Exists(apacheContext.FilePath);
                                            if (isOnWWWRoot)
                                                CollectPHP = new PHP().ProcessPHPPage(apacheContext.FilePath, ApacheNetServerConfiguration.PHPStaticFolder, ApacheNetServerConfiguration.PHPVersion, ctx, apacheContext.Secure);
                                            else
                                                CollectPHP = new PHP().ProcessPHPPage(apacheContext.ApiPath, ApacheNetServerConfiguration.PHPStaticFolder, ApacheNetServerConfiguration.PHPVersion, ctx, apacheContext.Secure);
                                            foreach (var innerArray in CollectPHP.Item3)
                                                apacheContext.Response.Headers.Add(innerArray.Key, innerArray.Value);
                                            apacheContext.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                            apacheContext.Response.Headers.Add("Last-Modified", File.GetLastWriteTime(apacheContext.FilePath).ToString("r"));
                                            apacheContext.StatusCode = (HttpStatusCode)CollectPHP.Item1;
                                            apacheContext.Response.ContentType = "text/html";
                                            sent = await apacheContext.SendImmediate(CollectPHP.Item2 ?? Array.Empty<byte>(), apacheContext.AcceptChunked).ConfigureAwait(false);
                                        }
                                        else if (File.Exists(apacheContext.FilePath))
                                        {
                                            string ContentType = HTTPProcessor.GetMimeType(Path.GetExtension(apacheContext.FilePath), ApacheNetServerConfiguration.MimeTypes ?? HTTPProcessor.MimeTypes);

                                            if (ContentType == "application/octet-stream")
                                            {
                                                byte[] VerificationChunck = FileSystemUtils.TryReadFileChunck(apacheContext.FilePath, 10, FileShare.ReadWrite, LocalFileStreamHelper.FileLockAwaitMs);
                                                foreach (var entry in HTTPProcessor.PathernDictionary)
                                                {
                                                    if (ByteUtils.FindBytePattern(VerificationChunck, entry.Value) != -1)
                                                    {
                                                        ContentType = entry.Key;
                                                        break;
                                                    }
                                                }
                                            }

                                            if (ApacheNetServerConfiguration.RangeHandling && !string.IsNullOrEmpty(apacheContext.Request.RetrieveHeaderValue("Range")))
                                                sent = await LocalFileStreamHelper.HandlePartialRangeRequest(apacheContext, apacheContext.FilePath, ContentType, noCompressCacheControl);
                                            else
                                            {
                                                // send file
                                                LoggerAccessor.LogInfo($"[{loggerprefix}] - {apacheContext.ClientIP}:{apacheContext.ClientPort} Requested a file : {absolutepath}");

                                                sent = await LocalFileStreamHelper.HandleRequest(apacheContext, encoding, absolutepath, apacheContext.FilePath, ContentType, apacheContext.Request.Useragent, ContentType.StartsWith("video/") || ContentType.StartsWith("audio/"), isHtmlCompatible, noCompressCacheControl);
                                            }
                                        }
                                        else
                                        {
                                            apacheContext.StatusCode = HttpStatusCode.NotFound;

                                            if (isHtmlCompatible)
                                            {
                                                string hostToDisplay = string.IsNullOrEmpty(Host) ? (apacheContext.ServerIP.Length > 15 ? "[" + apacheContext.ServerIP + "]" : apacheContext.ServerIP) : Host;

                                                apacheContext.Response.ContentType = "text/html";
                                                sent = await apacheContext.SendImmediate(await DefaultHTMLPages.GenerateErrorPageAsync(apacheContext.StatusCode, absolutepath, $"{(apacheContext.Secure ? "https" : "http")}://{hostToDisplay}",
                                                    ApacheNetServerConfiguration.HTTPStaticFolder, serverRevision, hostToDisplay, apacheContext.ServerPort)).ConfigureAwait(false);

                                            }
                                            else
                                                sent = await apacheContext.SendImmediate().ConfigureAwait(false);
                                        }
                                        break;
                                }
                                break;
                            case "HEAD":
                                sent = await ApacheRequestHandler.HandleHEAD(apacheContext).ConfigureAwait(false);
                                break;
                            case "OPTIONS":
                                apacheContext.Response.Headers.Set("Allow", "OPTIONS, GET, HEAD, POST");
                                apacheContext.StatusCode = HttpStatusCode.OK;
                                sent = await apacheContext.SendImmediate().ConfigureAwait(false);
                                break;
                            case "PROPFIND":
                                sent = await ApacheRequestHandler.HandlePROPFIND(apacheContext).ConfigureAwait(false);
                                break;
                            default:
                                apacheContext.StatusCode = HttpStatusCode.Forbidden;
                                sent = await apacheContext.SendImmediate().ConfigureAwait(false);
                                break;
                        }
                    }
                }
            }
            else
            {
                apacheContext.StatusCode = HttpStatusCode.Forbidden;
                sent = await apacheContext.SendImmediate().ConfigureAwait(false);
            }

            if (apacheContext.Response.StatusCode < 400)
                LoggerAccessor.LogInfo($"[{loggerprefix}] - {apacheContext.ClientIP}:{apacheContext.ClientPort} -> {apacheContext.Response.StatusCode}");
            else
            {
                switch (apacheContext.Response.StatusCode)
                {
                    case (int)HttpStatusCode.NotFound:
                        if (string.IsNullOrEmpty(apacheContext.FilePath))
                            LoggerAccessor.LogWarn($"[{loggerprefix}] - {apacheContext.ClientIP}:{apacheContext.ClientPort} -> {apacheContext.Response.StatusCode}");
                        else
                            LoggerAccessor.LogWarn($"[{loggerprefix}] - {apacheContext.ClientIP}:{apacheContext.ClientPort} Requested a non-existent file: {apacheContext.FilePath} -> {apacheContext.Response.StatusCode}");
                        break;

                    case (int)HttpStatusCode.NotImplemented:
                    case (int)HttpStatusCode.RequestedRangeNotSatisfiable:
                        LoggerAccessor.LogWarn($"[{loggerprefix}] - {apacheContext.ClientIP}:{apacheContext.ClientPort} -> {apacheContext.Response.StatusCode}");
                        break;

                    default:
                        LoggerAccessor.LogError($"[{loggerprefix}] - {apacheContext.ClientIP}:{apacheContext.ClientPort} -> {apacheContext.Response.StatusCode}");
                        break;
                }
            }
        }

        private void ExceptionEncountered(object? sender, ExceptionEventArgs args)
        {
            LoggerAccessor.LogError($"[{(_port.ToString().EndsWith("443") ? "HTTPS" : "HTTP")}] - Exception Encountered: {args.Exception}");
        }
    }
}
