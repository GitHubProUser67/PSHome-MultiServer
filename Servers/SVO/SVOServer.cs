using MultiServerLibrary.HTTP;
using CustomLogger;
using HttpMultipartParser;
using System.Text;
using SpaceWizards.HttpListener;
using System.Security.Cryptography.X509Certificates;
using SVO.Games.PS3;
using MultiServerLibrary.Extension;

namespace SVO
{
    public class SVOServer
    {
        public static bool IsStarted = false;

        private X509Certificate2? certificate;
        private Thread? thread;
        private volatile bool threadActive;

        private HttpListener? listener;

        private List<Task> HttpClientTasks = new();

        private readonly int AwaiterTimeoutInMS;
        private int MaxConcurrentListeners;

        private readonly string host;

        public SVOServer(string host, X509Certificate2? certificate = null, int MaxConcurrentListeners = 10, int awaiterTimeoutInMS = 500)
        {
            this.host = host;
            this.certificate = certificate;
            this.MaxConcurrentListeners = MaxConcurrentListeners;
            AwaiterTimeoutInMS = awaiterTimeoutInMS;

            Start();
        }

        public static bool IsIPBanned(string ipAddress)
        {
            if (MultiServerLibrary.MultiServerLibraryConfiguration.BannedIPs != null && MultiServerLibrary.MultiServerLibraryConfiguration.BannedIPs.Contains(ipAddress))
                return true;

            return false;
        }

        public void Start()
        {
            if (thread != null)
            {
                LoggerAccessor.LogWarn("[SVO] - Server already active.");
                return;
            }
            thread = new Thread(Listen);
            thread.Start();
            IsStarted = true;
        }

        public void Stop()
        {
            // stop thread and listener
            threadActive = false;
            if (listener != null && listener.IsListening) listener.Stop();

            // wait for thread to finish
            if (thread != null)
            {
                thread.Join();
                thread = null;
            }

            // finish closing listener
            if (listener != null)
            {
                RemoveAllPrefixes(listener);
                listener.Close();
                listener = null;
            }
            IsStarted = false;
        }

        private bool RemoveAllPrefixes(HttpListener listener)
        {
            try
            {
                // Get the prefixes that the Web server is listening to.
                listener.Prefixes.Clear();
            }
            // If the operation failed, return false.
            catch
            {
                return false;
            }

            return true;
        }

        private void Listen()
        {
            threadActive = true;

            object _sync = new();

            // start listener
            try
            {
                System.Net.IPAddress hostAddr = System.Net.IPAddress.Parse(InternetProtocolUtils.GetFirstActiveIPAddress(host, "0.0.0.0"));

                listener = new HttpListener();
                if (certificate != null)
                {
                    listener.SetCertificate(hostAddr, 10061, certificate);
                    listener.SetCertificate(hostAddr, 10062, certificate);
                }
                listener.Prefixes.Add(string.Format("http://{0}:{1}/", host, 10058));
                const ushort startingSVOPort = 10060;
                for (byte i = 0; i < 3; i++)
                {
                    if (i == 0)
                        listener.Prefixes.Add(string.Format("http://{0}:{1}/", host, startingSVOPort));
                    else
                        listener.Prefixes.Add(string.Format("https://{0}:{1}/", host, startingSVOPort + i));
                }
                listener.Start();
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError("[SVO] - An Exception Occured while starting the http server: " + e.Message);
                threadActive = false;
                return;
            }

            LoggerAccessor.LogInfo("[SVO] - Server started...");

            // wait for requests
            while (threadActive)
            {
                lock (_sync)
                {
                    if (!threadActive)
                        break;
                }

                while (HttpClientTasks.Count < MaxConcurrentListeners) //Maximum number of concurrent listeners
                    HttpClientTasks.Add(listener.GetContextAsync().ContinueWith(t =>
                    {
                        HttpListenerContext? client = null;
                        try
                        {
                            if (!t.IsCompleted)
                                return;
                            client = t.Result;
                        }
                        catch
                        {
                        }
                        _ = ProcessMessagesFromClient(client);
                    }));

                int RemoveAtIndex = Task.WaitAny(HttpClientTasks.ToArray(), AwaiterTimeoutInMS); //Synchronously Waits up to 500ms for any Task completion
                if (RemoveAtIndex != -1) //Remove the completed task from the list
                    HttpClientTasks.RemoveAt(RemoveAtIndex);
            }
        }

        #region Protected Functions
        protected virtual async Task<bool> ProcessMessagesFromClient(HttpListenerContext? ctx)
        {
            if (ctx == null)
                return false;
#if DEBUG
            LoggerAccessor.LogInfo($"[SVO] - Connection received (Thread " + Thread.CurrentThread.ManagedThreadId.ToString() + ")");
#endif
            ctx.Response.KeepAlive = SVOServerConfiguration.EnableKeepAlive;
            try
            {
                bool isAllowed = false;
                string fullurl = ctx.Request.Url.ToString();
                string absolutepath = ctx.Request.Url.AbsolutePath;
                string clientip = ctx.Request.RemoteEndPoint.Address.ToString();
                int clientport = ctx.Request.RemoteEndPoint.Port;

                if (IsIPBanned(clientip))
                    LoggerAccessor.LogError($"[SECURITY] - Client - {clientip}:{clientport} Requested the SVO server while being banned!");
                else if (!string.IsNullOrEmpty(absolutepath))
                {
                    string? UserAgent = null;

                    if (!string.IsNullOrEmpty(ctx.Request.UserAgent))
                        UserAgent = ctx.Request.UserAgent.ToLower();

                    if (!string.IsNullOrEmpty(UserAgent) && UserAgent.Contains("bytespider")) // Get Away TikTok.
                        LoggerAccessor.LogInfo($"[SVO] - Client - {clientip}:{clientport} Requested the SVO Server while not being allowed!");
                    else
                    {
                        LoggerAccessor.LogInfo($"[SVO] - Client - {clientip}:{clientport} Requested the SVO Server with URL : {fullurl}");
                        isAllowed = true;
                    }
                }

                if (isAllowed)
                {
                    if (absolutepath == "/dataloaderweb/queue")
                    {
                        switch (ctx.Request.HttpMethod)
                        {
                            case "POST":
                                if (!string.IsNullOrEmpty(ctx.Request.ContentType))
                                {
                                    ctx.Response.Headers.Set("Content-Type", "application/xml;charset=UTF-8");
                                    ctx.Response.Headers.Set("Content-Language", string.Empty);
                                    string? boundary = HTTPProcessor.ExtractBoundary(ctx.Request.ContentType);

                                    var data = MultipartFormDataParser.Parse(ctx.Request.InputStream, boundary);

                                    byte[] datatooutput = Encoding.UTF8.GetBytes(data.GetParameterValue("body"));

                                    Directory.CreateDirectory($"{SVOServerConfiguration.SVOStaticFolder}/dataloaderweb/queue");

                                    DirectoryInfo directory = new($"{SVOServerConfiguration.SVOStaticFolder}/dataloaderweb/queue");

                                    FileInfo[] files = directory.GetFiles();

                                    if (files.Length > 19)
                                    {
                                        FileInfo oldestFile = files.OrderBy(file => file.CreationTime).First();
                                        LoggerAccessor.LogInfo("[SVO] - Replacing Home Debug log file: " + oldestFile.Name);
                                        if (File.Exists(oldestFile.FullName))
                                            File.Delete(oldestFile.FullName);
                                    }

                                    File.WriteAllBytes($"{SVOServerConfiguration.SVOStaticFolder}/dataloaderweb/queue/{Guid.NewGuid()}.xml", datatooutput);

                                    ctx.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                                    ctx.Response.SendChunked = true;

                                    if (ctx.Response.OutputStream.CanWrite)
                                    {
                                        try
                                        {
                                            ctx.Response.ContentLength64 = datatooutput.Length;
                                            ctx.Response.OutputStream.Write(datatooutput, 0, datatooutput.Length);
                                        }
                                        catch
                                        {
                                            // Not Important.
                                        }
                                    }
                                }
                                else
                                    ctx.Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                break;
                            default:
                                ctx.Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                break;
                        }
                    }
                    else if (absolutepath.Contains("/HUBPS3_SVML/"))
                        await PlayStationHome.Home_SVO(ctx.Request, ctx.Response);
                    else if (absolutepath.Contains("/WARHAWK_SVML/"))
                        await Warhawk.Warhawk_SVO(ctx.Request, ctx.Response);
                    else if (absolutepath.Contains("/MOTORSTORM2PS3_SVML/") || absolutepath.Contains("/MOTORSTORM2PS3_XML/"))
                        await MotorstormPR2.MotorStormPR_SVO(ctx.Request, ctx.Response);
                    else if (absolutepath.Contains("/motorstorm3ps3_xml/"))
                        await MotorStormApocalypse.MSApocalypse_OTG(ctx.Request, ctx.Response);
                    else if (absolutepath.Contains("/BUZZPS3_SVML/"))
                        await BuzzQuizGame.BuzzQuizGame_SVO(ctx.Request, ctx.Response);
                    else if (absolutepath.Contains("/BOURBON_XML/"))
                        await Starhawk.Starhawk_SVO(ctx.Request, ctx.Response);
                    else if (absolutepath.Contains("/CONFRONTATION_XML/"))
                        await SocomConfrontation.SocomConfrontation_SVO(ctx.Request, ctx.Response);
                    else if (absolutepath.Contains("/SINGSTARPS3_SVML/"))
                        await SingStar.Singstar_SVO(ctx.Request, ctx.Response);
                    else if (absolutepath.Contains("/TWISTEDMETALX_XML/"))
                        await TwistedMetalX.TwistedMetalX_SVO(ctx.Request, ctx.Response);
                    else if (absolutepath.Contains("/wox_ws/"))
                        await Wipeout2048.Wipeout2048_OTG(ctx.Request, ctx.Response);
                    else
                    {
                        // Only meant to be used with fairly small files.
                        string filePath = Path.Combine(SVOServerConfiguration.SVOStaticFolder, absolutepath[1..]);

                        if (File.Exists(filePath))
                        {
                            ctx.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                            ctx.Response.ContentType = HTTPProcessor.GetMimeType(Path.GetExtension(filePath), HTTPProcessor._mimeTypes);

                            ctx.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                            ctx.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                            ctx.Response.Headers.Add("ETag", Guid.NewGuid().ToString()); // Well, kinda wanna avoid client caching.
                            ctx.Response.Headers.Add("Last-Modified", File.GetLastWriteTime(filePath).ToString("r"));

                            byte[] FileContent = File.ReadAllBytes(filePath);

                            if (ctx.Response.OutputStream.CanWrite)
                            {
                                try
                                {
                                    ctx.Response.ContentLength64 = FileContent.Length;
                                    ctx.Response.OutputStream.Write(FileContent, 0, FileContent.Length);
                                }
                                catch
                                {
                                    // Not Important.
                                }
                            }
                        }
                        else
                            ctx.Response.StatusCode = (int)System.Net.HttpStatusCode.NotFound;
                    }
                }
                else
                    ctx.Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;

                if (ctx.Response.StatusCode < 400)
                    LoggerAccessor.LogInfo($"[SVO] - {clientip}:{clientport} -> {ctx.Response.StatusCode}");
                else
                {
                    switch (ctx.Response.StatusCode)
                    {
                        case (int)System.Net.HttpStatusCode.NotFound:
                        case (int)System.Net.HttpStatusCode.NotImplemented:
                        case (int)System.Net.HttpStatusCode.RequestedRangeNotSatisfiable:
                            LoggerAccessor.LogWarn($"[SVO] - {clientip}:{clientport} -> {ctx.Response.StatusCode}");
                            break;

                        default:
                            LoggerAccessor.LogError($"[SVO] - {clientip}:{clientport} -> {ctx.Response.StatusCode}");
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

                if (errorCode != 995 && errorCode != 64) LoggerAccessor.LogError("[SVO] - HttpListenerException ERROR: " + e.Message);

                ctx.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError("[SVO] - REQUEST ERROR: " + e.Message);
                ctx.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            }

            try
            {
                ctx.Response.OutputStream.Close();
            }
            catch
            {
            }
            ctx.Response.Close();
#if DEBUG
            LoggerAccessor.LogWarn($"[SVO] - Client disconnected (Thread " + Thread.CurrentThread.ManagedThreadId.ToString() + ")");
#endif

            return true;
        }
        #endregion
    }
}
