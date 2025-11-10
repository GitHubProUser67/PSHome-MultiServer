using CustomLogger;
using HttpMultipartParser;
using MultiServerLibrary.Extension;
using MultiServerLibrary.HTTP;
using SpaceWizards.HttpListener;
using SpaceWizards.HttpListener.CustomServers;
using SVO.Games.PS3;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SVO
{
    public class SVOProcessor
    {
        private readonly HTTPServer? _server;

        public SVOProcessor()
        {
            if (_server == null)
            {
                _server = new HTTPServer
                {
                    PreferHttpSys = false, // low priority TODO, make SVO more dynamic in that aspect (if it really matters...).
                    FireClientAsTask = false
                };
            }
        }

        public void Start(string host, X509Certificate2? certificate = null, int MaxConcurrentListeners = 10, CancellationToken token = default)
        {
            _server!.Host = host;

            _ = _server.StartAsync(
                new Dictionary<ushort, bool>() { { 10058, false } }, // Prefer using a single listener (SVO is not performance critical), the rest will be populated later.
                MaxConcurrentListeners,
                (serverPort, listener) =>
                {
                    if (listener is HttpListener managed)
                    {
                        const ushort startingSVOPort = 10060;

                        if (certificate != null)
                        {
                            System.Net.IPAddress hostAddr = System.Net.IPAddress.Parse(InternetProtocolUtils.GetFirstActiveIPAddress(host, System.Net.IPAddress.Any.ToString()));

                            managed.SetCertificate(hostAddr, 10061, certificate);
                            managed.SetCertificate(hostAddr, 10062, certificate);
                        }
#pragma warning disable
                        managed.SslProtocols = SslProtocols.Ssl2 | SslProtocols.Default | SslProtocols.Tls11 | SslProtocols.Tls12;
#pragma warning restore
                        for (byte i = 0; i < 3; i++)
                        {
                            int port = startingSVOPort + i;

                            if (TCPUtils.IsTCPPortAvailable(port))
                            {
                                string prefix = $"http{(i == 0 ? string.Empty : 's')}://{host}:{port}/";
                                managed.Prefixes.Add(prefix);
                                LoggerAccessor.LogInfo($"[SVO] - Added supplemental prefix: {prefix}.");
                            }
                            else
                                LoggerAccessor.LogError($"[SVO] - Port:{port} is not available, skipping...");
                        }
                    }
                },
                null,
                null,
                (serverPort, listenerCtx, remoteEP) =>
                {
                    _ = ProcessMessagesFromClient((HttpListenerContext)listenerCtx, remoteEP, token);
                },
                token
                );
        }

        public void Stop()
        {
            _server?.Stop();
        }

        private static Task ProcessMessagesFromClient(HttpListenerContext listenerCtx, System.Net.IPEndPoint remoteEP, CancellationToken token) =>
            Task.Run(async () =>
            {
                bool isAllowed = false;

                try
                {
                    listenerCtx.Response.KeepAlive = SVOServerConfiguration.EnableKeepAlive;

                    string absolutepath = listenerCtx.Request.Url.AbsolutePath;
                    string clientip = remoteEP.Address.ToString();
                    int clientport = remoteEP.Port;

                    if (!string.IsNullOrEmpty(absolutepath))
                    {
                        string? UserAgent = null;

                        if (!string.IsNullOrEmpty(listenerCtx.Request.UserAgent))
                            UserAgent = listenerCtx.Request.UserAgent.ToLower();

                        if (!string.IsNullOrEmpty(UserAgent) && UserAgent.Contains("bytespider")) // Get Away TikTok.
                            LoggerAccessor.LogInfo($"[SVO] - Client - {clientip}:{clientport} Requested the SVO Server while not being allowed!");
                        else
                        {
                            LoggerAccessor.LogInfo($"[SVO] - Client - {clientip}:{clientport} Requested the SVO Server with absolute URL : {absolutepath}");
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
                                        listenerCtx.Response.Headers.Set("Content-Type", "application/xml;charset=UTF-8");
                                        listenerCtx.Response.Headers.Set("Content-Language", string.Empty);
                                        string? boundary = HTTPProcessor.ExtractBoundary(listenerCtx.Request.ContentType);

                                        byte[] dataOutput = Encoding.UTF8.GetBytes(MultipartFormDataParser.Parse(listenerCtx.Request.InputStream, boundary).GetParameterValue("body"));

                                        Directory.CreateDirectory($"{SVOServerConfiguration.SVOStaticFolder}/dataloaderweb/queue");

                                        FileInfo[] files = new DirectoryInfo($"{SVOServerConfiguration.SVOStaticFolder}/dataloaderweb/queue").GetFiles();

                                        if (files.Length > 19)
                                        {
                                            FileInfo oldestFile = files.OrderBy(file => file.CreationTime).First();
                                            LoggerAccessor.LogInfo("[SVO] - Replacing Home Debug log file: " + oldestFile.Name);
                                            if (File.Exists(oldestFile.FullName))
                                                File.Delete(oldestFile.FullName);
                                        }

                                        File.WriteAllBytes($"{SVOServerConfiguration.SVOStaticFolder}/dataloaderweb/queue/{Guid.NewGuid()}.xml", dataOutput);

                                        listenerCtx.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                                        listenerCtx.Response.SendChunked = true;

                                        if (listenerCtx.Response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                listenerCtx.Response.ContentLength64 = dataOutput.Length;
                                                await listenerCtx.Response.OutputStream.WriteAsync(dataOutput).ConfigureAwait(false);
                                            }
                                            catch
                                            {
                                                // Not Important.
                                            }
                                        }
                                    }
                                    else
                                        listenerCtx.Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                    break;
                                default:
                                    listenerCtx.Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                    break;
                            }
                        }
                        else if (absolutepath.Contains("/HUBPS3_SVML/"))
                            await PlayStationHome.Home_SVO(listenerCtx.Request, listenerCtx.Response).ConfigureAwait(false);
                        else if (absolutepath.Contains("/WARHAWK_SVML/"))
                            await Warhawk.Warhawk_SVO(listenerCtx.Request, listenerCtx.Response).ConfigureAwait(false);
                        else if (absolutepath.Contains("/MOTORSTORM2PS3_SVML/") || absolutepath.Contains("/MOTORSTORM2PS3_XML/"))
                            await MotorstormPR2.MotorStormPR_SVO(listenerCtx.Request, listenerCtx.Response).ConfigureAwait(false);
                        else if (absolutepath.Contains("/motorstorm3ps3_xml/"))
                            await MotorStormApocalypse.MSApocalypse_OTG(listenerCtx.Request, listenerCtx.Response).ConfigureAwait(false);
                        else if (absolutepath.Contains("/BUZZPS3_SVML/"))
                            await BuzzQuizGame.BuzzQuizGame_SVO(listenerCtx.Request, listenerCtx.Response).ConfigureAwait(false);
                        else if (absolutepath.Contains("/BOURBON_XML/"))
                            await Starhawk.Starhawk_SVO(listenerCtx.Request, listenerCtx.Response).ConfigureAwait(false);
                        else if (absolutepath.Contains("/CONFRONTATION_XML/"))
                            await SocomConfrontation.SocomConfrontation_SVO(listenerCtx.Request, listenerCtx.Response).ConfigureAwait(false);
                        else if (absolutepath.Contains("/SINGSTARPS3_SVML/"))
                            await SingStar.Singstar_SVO(listenerCtx.Request, listenerCtx.Response).ConfigureAwait(false);
                        else if (absolutepath.Contains("/TWISTEDMETALX_XML/"))
                            await TwistedMetalX.TwistedMetalX_SVO(listenerCtx.Request, listenerCtx.Response).ConfigureAwait(false);
                        else if (absolutepath.Contains("/wox_ws/"))
                            await Wipeout2048.Wipeout2048_OTG(listenerCtx.Request, listenerCtx.Response).ConfigureAwait(false);
                        else
                        {
                            // Only meant to be used with fairly small files.
                            string filePath = Path.Combine(SVOServerConfiguration.SVOStaticFolder, absolutepath[1..]);

                            if (File.Exists(filePath))
                            {
                                listenerCtx.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                                listenerCtx.Response.ContentType = HTTPProcessor.GetMimeType(Path.GetExtension(filePath), HTTPProcessor.MimeTypes);

                                listenerCtx.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                                listenerCtx.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                listenerCtx.Response.Headers.Add("ETag", Guid.NewGuid().ToString()); // Well, kinda wanna avoid client caching.
                                listenerCtx.Response.Headers.Add("Last-Modified", File.GetLastWriteTime(filePath).ToString("r"));

                                byte[] FileContent = await File.ReadAllBytesAsync(filePath).ConfigureAwait(false);

                                if (listenerCtx.Response.OutputStream.CanWrite)
                                {
                                    try
                                    {
                                        listenerCtx.Response.ContentLength64 = FileContent.Length;
                                        await listenerCtx.Response.OutputStream.WriteAsync(FileContent).ConfigureAwait(false);
                                    }
                                    catch
                                    {
                                        // Not Important.
                                    }
                                }
                            }
                            else
                                listenerCtx.Response.StatusCode = (int)System.Net.HttpStatusCode.NotFound;
                        }
                    }
                    else
                        listenerCtx.Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;

                    if (listenerCtx.Response.StatusCode < 400)
                        LoggerAccessor.LogInfo($"[SVO] - {clientip}:{clientport} -> {listenerCtx.Response.StatusCode}");
                    else
                    {
                        switch (listenerCtx.Response.StatusCode)
                        {
                            case (int)System.Net.HttpStatusCode.NotFound:
                            case (int)System.Net.HttpStatusCode.NotImplemented:
                            case (int)System.Net.HttpStatusCode.RequestedRangeNotSatisfiable:
                                LoggerAccessor.LogWarn($"[SVO] - {clientip}:{clientport} -> {listenerCtx.Response.StatusCode}");
                                break;

                            default:
                                LoggerAccessor.LogError($"[SVO] - {clientip}:{clientport} -> {listenerCtx.Response.StatusCode}");
                                break;
                        }
                    }
                }
                catch (HttpListenerException e)
                {
                    // Unfortunately, some client side implementation of HTTP (like RPCS3) freeze the interface at regular interval.
                    // This will cause server to throw error 64 (network interface not openned anymore)
                    // In that case, we send internalservererror so client try again.

                    int errorCode = e.ErrorCode;

                    if (errorCode != 995 && errorCode != 64)
                        LoggerAccessor.LogError("[SVO] - HttpListenerException ERROR: " + e.Message);

                    listenerCtx.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                }
                catch (Exception e)
                {
                    LoggerAccessor.LogError("[SVO] - Exception ERROR: " + e.Message);

                    listenerCtx.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                }

                try
                {
                    listenerCtx.Response.OutputStream.Close();
                }
                catch
                {
                }
                listenerCtx.Response.Close();
            });
    }
}
