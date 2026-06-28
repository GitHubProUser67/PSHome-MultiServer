using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using CustomLogger;
using HttpMultipartParser;
using MultiServerLibrary.CustomServers;
using MultiServerLibrary.Extension;
using MultiServerLibrary.HTTP;
using SpaceWizards.HttpListener;
using SpaceWizards.HttpListener.CustomServers;
using SVO.Games.PS3;
using WatsonWebserver;

namespace SVO
{
    public class SVOProcessor
    {
        public const int FileLockAwaitMs = 500;

        public int ProxyMilisecondsTimeout { get; set; } = 5000;

        private (bool, ushort) availableTunnelEnd = default;

        private readonly HTTPRateLimiter _rateLimiter = new();

        private readonly HTTPServer? _httpServer;
        private readonly TCPServer? _tcpServer;

        public SVOProcessor()
        {
            _httpServer ??= new HTTPServer
            {
                PreferHttpSys = false, // low priority TODO, make SVO more dynamic in that aspect (if it really matters...).
                FireClientAsTask = false,
            };
            _tcpServer ??= new TCPServer() { FireClientAsTask = false };
        }

        public void Start(
            string host,
            X509Certificate2? certificate = null,
            int MaxConcurrentListeners = 10,
            CancellationToken token = default
        )
        {
            _httpServer!.Host = host;

            _ = _httpServer
                .StartAsync(
                    new Dictionary<ushort, bool>() { { 10058, false } }, // Prefer using a single listener (SVO is not performance critical), the rest will be populated later.
                    MaxConcurrentListeners,
                    (serverPort, listener) =>
                    {
                        if (listener is HttpListener managed)
                        {
                            const ushort startingSVOPort = 10060;

                            if (certificate != null)
                            {
                                var hostAddr = System.Net.IPAddress.Parse(
                                    InternetProtocolUtils.GetFirstActiveIPAddress(
                                        host,
                                        System.Net.IPAddress.Any.ToString()
                                    )
                                );

                                managed.SetCertificate(hostAddr, 10061, certificate);
                                managed.SetCertificate(hostAddr, 10062, certificate);
                            }
#pragma warning disable
                            managed.SslProtocols =
                                SslProtocols.Ssl2
                                | SslProtocols.Default
                                | SslProtocols.Tls11
                                | SslProtocols.Tls12;
#pragma warning restore
                            for (byte i = 0; i < 3; i++)
                            {
                                var port = startingSVOPort + i;

                                if (TcpUdpUtils.IsTCPPortAvailable(port))
                                {
                                    bool isPlain = i == 0;
                                    if (availableTunnelEnd == default)
                                        availableTunnelEnd = (isPlain, (ushort)port);
                                    var prefix =
                                        $"http{(isPlain ? string.Empty : 's')}://{host}:{port}/";
                                    managed.Prefixes.Add(prefix);
                                    LoggerAccessor.LogInfo(
                                        $"[SVO] - Added supplemental prefix: {prefix}."
                                    );
                                }
                                else
                                    LoggerAccessor.LogWarn(
                                        $"[SVO] - Port:{port} is not available, skipping..."
                                    );
                            }
                        }
                    },
                    null,
                    null,
                    async (serverPort, listenerCtx, remoteEP) =>
                    {
                        var rateLimitResult = await _rateLimiter.TryGetRateLimitSlot(remoteEP).ConfigureAwait(false);

                        if (rateLimitResult.Item1)
                            _ = ProcessMessagesFromClient(
                                                       (HttpListenerContext)listenerCtx,
                                                       remoteEP,
                                                       token
                                                   );
                        else if (listenerCtx is System.Net.HttpListenerContext nativeCtx)
                        {
                            nativeCtx.Response.StatusCode = 429;
                            if (rateLimitResult.Item2 != null)
                                nativeCtx.Response.Headers.Add(rateLimitResult.Item2, rateLimitResult.Item3);
                            nativeCtx.Response.Close();
                        }
                        else if (listenerCtx is HttpListenerContext managedCtx)
                        {
                            managedCtx.Response.StatusCode = 429;
                            if (rateLimitResult.Item2 != null)
                                managedCtx.Response.Headers.Add(rateLimitResult.Item2, rateLimitResult.Item3);
                            managedCtx.Response.Close();
                        }
                    },
                    token
                )
                .ContinueWith(
                    previousTask => StartTunnelAsync(previousTask, MaxConcurrentListeners, token),
                    token
                );
        }

        private async Task StartTunnelAsync(
            Task previousTask,
            int MaxConcurrentListeners,
            CancellationToken token
        )
        {
            try
            {
                await previousTask.ConfigureAwait(false);

                LoggerAccessor.LogInfo(
                    "[SVO] - HTTP server started successfully, initiating tunnel..."
                );
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError("[SVO] - Unexpected error during HTTP server start.", ex);

                return;
            }

            if (availableTunnelEnd == default)
            {
                LoggerAccessor.LogWarn(
                    $"[SVO] - No available endpoint detected for the tunnel, skipping..."
                );

                return;
            }

            const ushort tunnelSVOPort = 10063;

            const int maxAllowedHeaderSize = 32768; // 32 KB

            await _tcpServer!
                .StartAsync(
                    new List<ushort> { tunnelSVOPort },
                    MaxConcurrentListeners,
                    null,
                    null,
                    null,
                    (serverPort, client, remoteEP) =>
                    {
                        _ = HTTPTunnelProcessor.HandleClient(
                            default,
                            null,
                            new MultiServerLibrary.Extension.NET.FixedTcpClient(client, token),
                            remoteEP,
                            !availableTunnelEnd.Item1,
                            availableTunnelEnd.Item2,
                            ProxyMilisecondsTimeout,
                            maxAllowedHeaderSize
                        );
                    },
                    token
                )
                .ConfigureAwait(false);
        }

        public void Stop()
        {
            _httpServer?.Stop();
        }

        private static Task ProcessMessagesFromClient(
            HttpListenerContext listenerCtx,
            System.Net.IPEndPoint remoteEP,
            CancellationToken token
        ) =>
            Task.Run(
                async () =>
                {
                    var isAllowed = false;

                    try
                    {
                        listenerCtx.Response.KeepAlive = SVOServerConfiguration.EnableKeepAlive;

                        var absolutepath = listenerCtx.Request.Url.AbsolutePath;
                        var clientip = remoteEP.Address.ToString();
                        var clientport = remoteEP.Port;

                        if (!string.IsNullOrEmpty(absolutepath))
                        {
                            string? UserAgent = null;

                            if (!string.IsNullOrEmpty(listenerCtx.Request.UserAgent))
                                UserAgent = listenerCtx.Request.UserAgent.ToLower();

                            if (
                                !string.IsNullOrEmpty(UserAgent) && UserAgent.Contains("bytespider")
                            ) // Get Away TikTok.
                                LoggerAccessor.LogInfo(
                                    $"[SVO] - Client - {clientip}:{clientport} Requested the SVO Server while not being allowed!"
                                );
                            else
                            {
                                LoggerAccessor.LogInfo(
                                    $"[SVO] - Client - {clientip}:{clientport} Requested the SVO Server with absolute URL : {absolutepath}"
                                );
                                isAllowed = true;
                            }
                        }

                        if (isAllowed)
                        {
                            if (absolutepath == "/dataloaderweb/queue")
                            {
                                switch (listenerCtx.Request.HttpMethod)
                                {
                                    case "POST":
                                        if (!string.IsNullOrEmpty(listenerCtx.Request.ContentType))
                                        {
                                            listenerCtx.Response.Headers.Set(
                                                "Content-Type",
                                                "application/xml;charset=UTF-8"
                                            );
                                            listenerCtx.Response.Headers.Set(
                                                "Content-Language",
                                                string.Empty
                                            );
                                            var boundary = HTTPProcessor.ExtractBoundary(
                                                listenerCtx.Request.ContentType
                                            );

                                            var dataOutput = Encoding.UTF8.GetBytes(
                                                MultipartFormDataParser
                                                    .Parse(
                                                        listenerCtx.Request.InputStream,
                                                        boundary
                                                    )
                                                    .GetParameterValue("body")
                                            );

                                            Directory.CreateDirectory(
                                                $"{SVOServerConfiguration.SVOStaticFolder}/dataloaderweb/queue"
                                            );

                                            var files = new DirectoryInfo(
                                                $"{SVOServerConfiguration.SVOStaticFolder}/dataloaderweb/queue"
                                            ).GetFiles();

                                            if (files.Length > 19)
                                            {
                                                var oldestFile = files
                                                    .OrderBy(file => file.CreationTime)
                                                    .First();
                                                LoggerAccessor.LogInfo(
                                                    "[SVO] - Replacing Home Debug log file: "
                                                        + oldestFile.Name
                                                );
                                                if (File.Exists(oldestFile.FullName))
                                                    File.Delete(oldestFile.FullName);
                                            }

                                            File.WriteAllBytes(
                                                $"{SVOServerConfiguration.SVOStaticFolder}/dataloaderweb/queue/{Guid.NewGuid()}.xml",
                                                dataOutput
                                            );

                                            listenerCtx.Response.StatusCode = (int)
                                                System.Net.HttpStatusCode.OK;
                                            listenerCtx.Response.SendChunked = true;

                                            if (listenerCtx.Response.OutputStream.CanWrite)
                                            {
                                                try
                                                {
                                                    listenerCtx.Response.ContentLength64 =
                                                        dataOutput.Length;
                                                    await listenerCtx
                                                        .Response.OutputStream.WriteAsync(
                                                            dataOutput
                                                        )
                                                        .ConfigureAwait(false);
                                                }
                                                catch
                                                {
                                                    // Not Important.
                                                }
                                            }
                                        }
                                        else
                                            listenerCtx.Response.StatusCode = (int)
                                                System.Net.HttpStatusCode.Forbidden;
                                        break;
                                    default:
                                        listenerCtx.Response.StatusCode = (int)
                                            System.Net.HttpStatusCode.Forbidden;
                                        break;
                                }
                            }
                            else if (absolutepath.Contains("/HUBPS3_SVML/"))
                                await PlayStationHome
                                    .Home_SVO(listenerCtx.Request, listenerCtx.Response)
                                    .ConfigureAwait(false);
                            else if (absolutepath.Contains("/WARHAWK_SVML/"))
                                await Warhawk
                                    .Warhawk_SVO(listenerCtx.Request, listenerCtx.Response)
                                    .ConfigureAwait(false);
                            else if (
                                absolutepath.Contains("/MOTORSTORM2PS3_SVML/")
                                || absolutepath.Contains("/MOTORSTORM2PS3_XML/")
                            )
                                await MotorstormPR2
                                    .MotorStormPR_SVO(listenerCtx.Request, listenerCtx.Response)
                                    .ConfigureAwait(false);
                            else if (absolutepath.Contains("/motorstorm3ps3_xml/"))
                                await MotorStormApocalypse
                                    .MSApocalypse_OTG(listenerCtx.Request, listenerCtx.Response)
                                    .ConfigureAwait(false);
                            else if (absolutepath.Contains("/BUZZPS3_SVML/"))
                                await BuzzQuizGame
                                    .BuzzQuizGame_SVO(listenerCtx.Request, listenerCtx.Response)
                                    .ConfigureAwait(false);
                            else if (absolutepath.Contains("/BOURBON_XML/"))
                                await Starhawk
                                    .Starhawk_SVO(listenerCtx.Request, listenerCtx.Response)
                                    .ConfigureAwait(false);
                            else if (absolutepath.Contains("/CONFRONTATION_XML/"))
                                await SocomConfrontation
                                    .SocomConfrontation_SVO(
                                        listenerCtx.Request,
                                        listenerCtx.Response
                                    )
                                    .ConfigureAwait(false);
                            else if (absolutepath.Contains("/SINGSTARPS3_SVML/"))
                                await SingStar
                                    .Singstar_SVO(listenerCtx.Request, listenerCtx.Response)
                                    .ConfigureAwait(false);
                            else if (absolutepath.Contains("/TWISTEDMETALX_XML/"))
                                await TwistedMetalX
                                    .TwistedMetalX_SVO(listenerCtx.Request, listenerCtx.Response)
                                    .ConfigureAwait(false);
                            else if (absolutepath.Contains("/wox_ws/"))
                                await Wipeout2048
                                    .Wipeout2048_OTG(listenerCtx.Request, listenerCtx.Response)
                                    .ConfigureAwait(false);
                            else
                            {
                                FileInfo fileInfo = new FileInfo(
                                    Path.Combine(
                                        SVOServerConfiguration.SVOStaticFolder,
                                        absolutepath[1..]
                                    )
                                );

                                if (fileInfo.Exists)
                                {
                                    var st = await FileSystemUtils
                                        .TryOpen(
                                            fileInfo.FullName,
                                            FileShare.ReadWrite,
                                            FileLockAwaitMs
                                        )
                                        .ConfigureAwait(false);

                                    if (st == null)
                                        listenerCtx.Response.StatusCode = (int)
                                            System.Net.HttpStatusCode.InternalServerError;
                                    else
                                    {
                                        using (st)
                                        {
                                            listenerCtx.Response.StatusCode = (int)
                                                System.Net.HttpStatusCode.OK;
                                            listenerCtx.Response.ContentType =
                                                HTTPProcessor.GetMimeType(
                                                    fileInfo.Extension,
                                                    HTTPProcessor.MimeTypes
                                                );

                                            listenerCtx.Response.Headers.Add(
                                                "Access-Control-Allow-Origin",
                                                "*"
                                            );
                                            listenerCtx.Response.Headers.Add(
                                                "Date",
                                                DateTime.Now.ToString("r")
                                            );
                                            listenerCtx.Response.Headers.Add(
                                                "ETag",
                                                Guid.NewGuid().ToString()
                                            ); // Well, kinda wanna avoid client caching.
                                            listenerCtx.Response.Headers.Add(
                                                "Last-Modified",
                                                fileInfo.LastWriteTime.ToString("r")
                                            );

                                            if (listenerCtx.Response.OutputStream.CanWrite)
                                            {
                                                try
                                                {
                                                    listenerCtx.Response.ContentLength64 =
                                                        fileInfo.Length;
                                                    await StreamUtils
                                                        .CopyStreamAsync(
                                                            st,
                                                            listenerCtx.Response.OutputStream,
                                                            4096,
                                                            false,
                                                            token
                                                        )
                                                        .ConfigureAwait(false);
                                                }
                                                catch
                                                {
                                                    // Not Important.
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                    listenerCtx.Response.StatusCode = (int)
                                        System.Net.HttpStatusCode.NotFound;
                            }
                        }
                        else
                            listenerCtx.Response.StatusCode = (int)
                                System.Net.HttpStatusCode.Forbidden;

                        if (listenerCtx.Response.StatusCode < 400)
                            LoggerAccessor.LogInfo(
                                $"[SVO] - {clientip}:{clientport} -> {listenerCtx.Response.StatusCode}"
                            );
                        else
                        {
                            switch (listenerCtx.Response.StatusCode)
                            {
                                case (int)System.Net.HttpStatusCode.NotFound:
                                case (int)System.Net.HttpStatusCode.NotImplemented:
                                case (int)System.Net.HttpStatusCode.RequestedRangeNotSatisfiable:
                                    LoggerAccessor.LogWarn(
                                        $"[SVO] - {clientip}:{clientport} -> {listenerCtx.Response.StatusCode}"
                                    );
                                    break;

                                default:
                                    LoggerAccessor.LogError(
                                        $"[SVO] - {clientip}:{clientport} -> {listenerCtx.Response.StatusCode}"
                                    );
                                    break;
                            }
                        }
                    }
                    catch (HttpListenerException e)
                    {
                        // Unfortunately, some client side implementation of HTTP (like RPCS3) freeze the interface at regular interval.
                        // This will cause server to throw error 64 (network interface not openned anymore)
                        // In that case, we send internalservererror so client try again.

                        var errorCode = e.ErrorCode;

                        if (errorCode != 995 && errorCode != 64)
                            LoggerAccessor.LogError(
                                "[SVO] - HttpListenerException ERROR: " + e.Message
                            );

                        listenerCtx.Response.StatusCode = (int)
                            System.Net.HttpStatusCode.InternalServerError;
                    }
                    catch (Exception e)
                    {
                        LoggerAccessor.LogError("[SVO] - Exception ERROR: " + e.Message);

                        listenerCtx.Response.StatusCode = (int)
                            System.Net.HttpStatusCode.InternalServerError;
                    }

                    try
                    {
                        listenerCtx.Response.OutputStream.Close();
                    }
                    catch { }
                    listenerCtx.Response.Close();
                },
                token
            );
    }
}
