using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MultiServerLibrary.Extension;
using MultiServerLibrary.HTTP;
using SpaceWizards.HttpListener;
using SpaceWizards.HttpListener.CustomServers;
using WatsonWebserver.Core;

namespace WatsonWebserver
{
    /// <summary>
    /// Watson webserver.
    /// </summary>
    public class Webserver : WebserverBase, IDisposable
    {
        #region Public-Members

        /// <summary>
        /// Indicates whether or not the server is listening.
        /// </summary>
        public override bool IsListening
        {
            get { return _HttpServer.IsAnyListening(); }
        }

        /// <summary>
        /// Number of requests being serviced currently.
        /// </summary>
        public override int RequestCount
        {
            get { return _RequestCount; }
        }

        public bool LogResponseSentMsg
        {
            get { return _ResponseMsg; }
            set { _ResponseMsg = value; }
        }

        public bool KeepAliveResponseData
        {
            get { return _KeepAliveResponseData; }
            set { _KeepAliveResponseData = value; }
        }

        public SslProtocols SslProtocols
        {
            get { return _sslprotocols; }
            set { _sslprotocols = value; }
        }

        #endregion

        #region Private-Members

        private readonly string _Header = "[Webserver] ";
        private readonly HTTPServer _HttpServer = new();
        private bool _ResponseMsg = true;
        private bool _KeepAliveResponseData = true;
        private int _RequestCount = 0;
        private SemaphoreSlim _RequestSemaphore = null;

        private readonly HTTPRateLimiter _rateLimiter = new();

#pragma warning disable
        private SslProtocols _sslprotocols =
            SslProtocols.Default | SslProtocols.Tls11 | SslProtocols.Tls12 | SslProtocols.Tls13;
#pragma warning restore

        private readonly int MaxConcurrentListeners;

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Creates a new instance of the webserver.
        /// If you do not provide a settings object, default settings will be used, which will cause the webserver to listen on http://localhost:8000, and send events to the console.
        /// </summary>
        /// <param name="settings">Webserver settings.</param>
        /// <param name="defaultRoute">Method used when a request is received and no matching routes are found.  Commonly used as the 404 handler when routes are used.</param>
        public Webserver(
            WebserverSettings settings,
            Func<HttpContextBase, Task> defaultRoute,
            int MaxConcurrentListeners = 10
        )
            : base(settings, defaultRoute)
        {
            settings ??= new WebserverSettings();

            this.MaxConcurrentListeners = MaxConcurrentListeners;

            Settings = settings;
            Settings.Headers.DefaultHeaders[WebserverConstants.HeaderHost] =
                settings.Hostname + ":" + settings.Port;
            Routes.Default = defaultRoute;

            _HttpServer.FireClientAsTask = false;
            _HttpServer.Prefix = settings.Prefix;
            _HttpServer.Host = settings.Hostname;

            _RequestSemaphore = new SemaphoreSlim(Settings.IO.MaxRequests, Settings.IO.MaxRequests);

            _Header = "[Webserver " + Settings.Prefix + "] ";
        }

        #endregion

        #region Public-Methods

        /// <summary>
        /// Tear down the server and dispose of background workers.
        /// Do not use this object after disposal.
        /// </summary>
        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Start accepting new connections.
        /// </summary>
        /// <param name="token">Cancellation token useful for canceling the server.</param>
        public override void Start(CancellationToken token = default)
        {
            if (
                !_HttpServer
                    .StartAsync(
                        new Dictionary<ushort, bool>
                        {
                            { (ushort)Settings.Port, Settings.Ssl.Enable },
                        },
                        MaxConcurrentListeners,
                        (serverPort, listener) =>
                        {
                            Statistics = new WebserverStatistics();

                            if (listener is HttpListener managed)
                            {
                                managed.SslProtocols = _sslprotocols;

                                if (Settings.Ssl.Enable)
                                    managed.SetCertificate(
                                        System.Net.IPAddress.Parse(
                                            InternetProtocolUtils.GetFirstActiveIPAddress(
                                                Settings.Hostname,
                                                System.Net.IPAddress.Any.ToString()
                                            )
                                        ),
                                        Settings.Port,
                                        Settings.Ssl.SslCertificate
                                    );
                            }
                        },
                        null,
                        null,
                        async (serverPort, listenerCtx, remoteEP) =>
                        {
                            var requestSemaphore = Volatile.Read(ref _RequestSemaphore);

                            if (await TryAcquireRequestSlotAsync(requestSemaphore, token).ConfigureAwait(false))
                            {
                                Interlocked.Increment(ref _RequestCount);

                                var rateLimitResult = await _rateLimiter.TryGetRateLimitSlot(remoteEP).ConfigureAwait(false);

                                if (rateLimitResult.Item1)
                                {
                                    _ = ProcessMessagesFromClient(requestSemaphore, listenerCtx, remoteEP, token);
                                    return;
                                }
                                else
                                {
                                    try
                                    {
                                        if (listenerCtx is System.Net.HttpListenerContext nativeCtx)
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
                                    }
                                    catch
                                    {
                                        // Not Important.
                                    }
                                }

                                Interlocked.Decrement(ref _RequestCount);

                                ReleaseRequestSlot(requestSemaphore);
                            }
                        },
                        token
                    )
                    .Result
            )
                throw new InvalidOperationException("WatsonWebserver is already listening.");

            Events.HandleServerStarted(this, EventArgs.Empty);
        }

        /// <summary>
        /// Start accepting new connections.
        /// </summary>
        /// <param name="token">Cancellation token useful for canceling the server.</param>
        /// <returns>Task.</returns>
        public override async Task StartAsync(CancellationToken token = default)
        {
            if (
                !await _HttpServer
                    .StartAsync(
                        new Dictionary<ushort, bool>
                        {
                            { (ushort)Settings.Port, Settings.Ssl.Enable },
                        },
                        MaxConcurrentListeners,
                        (serverPort, listener) =>
                        {
                            Statistics = new WebserverStatistics();

                            if (listener is HttpListener managed)
                            {
                                managed.SslProtocols = _sslprotocols;

                                if (Settings.Ssl.Enable)
                                    managed.SetCertificate(
                                        System.Net.IPAddress.Parse(
                                            InternetProtocolUtils.GetFirstActiveIPAddress(
                                                Settings.Hostname,
                                                System.Net.IPAddress.Any.ToString()
                                            )
                                        ),
                                        Settings.Port,
                                        Settings.Ssl.SslCertificate
                                    );
                            }
                        },
                        null,
                        null,
                        async (serverPort, listenerCtx, remoteEP) =>
                        {
                            var requestSemaphore = Volatile.Read(ref _RequestSemaphore);

                            if (await TryAcquireRequestSlotAsync(requestSemaphore, token).ConfigureAwait(false))
                            {
                                Interlocked.Increment(ref _RequestCount);

                                var rateLimitResult = await _rateLimiter.TryGetRateLimitSlot(remoteEP).ConfigureAwait(false);

                                if (rateLimitResult.Item1)
                                {
                                    _ = ProcessMessagesFromClient(requestSemaphore, listenerCtx, remoteEP, token);
                                    return;
                                }
                                else
                                {
                                    try
                                    {
                                        if (listenerCtx is System.Net.HttpListenerContext nativeCtx)
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
                                    }
                                    catch
                                    {
                                        // Not Important.
                                    }
                                }

                                Interlocked.Decrement(ref _RequestCount);

                                ReleaseRequestSlot(requestSemaphore);
                            }
                        },
                        token
                    )
                    .ConfigureAwait(false)
            )
                throw new InvalidOperationException("WatsonWebserver is already listening.");

            Events.HandleServerStarted(this, EventArgs.Empty);
        }

        /// <summary>
        /// Stop accepting new connections.
        /// </summary>
        public override void Stop()
        {
            if (!_HttpServer.Stop())
                throw new InvalidOperationException("WatsonWebserver is already stopped.");

            Events.HandleServerStopped(this, EventArgs.Empty);
        }

        #endregion

        #region Private-Methods

        private Task ProcessMessagesFromClient(
            SemaphoreSlim requestSemaphore,
            object listenerCtx,
            System.Net.IPEndPoint remoteEP,
            CancellationToken token
        ) =>
            Task.Run(
                async () =>
                {
                    HttpContext ctx = null;
                    Func<HttpContext, Task> handler = null;

                    try
                    {
                        #region Build-Context

                        Events.HandleConnectionReceived(
                            this,
                            new ConnectionEventArgs(remoteEP.Address.ToString(), remoteEP.Port)
                        );

                        ctx = new HttpContext(
                            listenerCtx,
                            Settings,
                            Events,
                            Serializer,
                            _KeepAliveResponseData
                        );

                        Events.HandleRequestReceived(this, new RequestEventArgs(ctx));

                        if (Settings.Debug.Requests)
                        {
                            Events.Logger?.Invoke(
                                _Header
                                    + ctx.Request.Source.IpAddress
                                    + ":"
                                    + ctx.Request.Source.Port
                                    + " "
                                    + ctx.Request.Method.ToString()
                                    + " "
                                    + ctx.Request.Url.RawWithoutQuery
                            );
                        }

                        Statistics.IncrementRequestCounter(ctx.Request.Method);
                        Statistics.IncrementReceivedPayloadBytes(ctx.Request.ContentLength);

                        #endregion

                        #region Check-Access-Control

                        if (!Settings.AccessControl.Permit(ctx.Request.Source.IpAddress))
                        {
                            Events.HandleRequestDenied(this, new RequestEventArgs(ctx));

                            if (Settings.Debug.AccessControl)
                            {
                                Events.Logger?.Invoke(
                                    _Header
                                        + ctx.Request.Source.IpAddress
                                        + ":"
                                        + ctx.Request.Source.Port
                                        + " denied due to access control"
                                );
                            }

                            if (listenerCtx is System.Net.HttpListenerContext nativeCtx)
                            {
                                nativeCtx.Response.StatusCode = 403;
                                nativeCtx.Response.Close();
                            }
                            else if (listenerCtx is HttpListenerContext managedCtx)
                            {
                                managedCtx.Response.StatusCode = 403;
                                managedCtx.Response.Close();
                            }
                            return;
                        }

                        #endregion

                        #region Preflight-Handler

                        if (ctx.Request.Method == HttpMethod.OPTIONS)
                        {
                            if (Routes.Preflight != null)
                            {
                                if (Settings.Debug.Routing)
                                {
                                    Events.Logger?.Invoke(
                                        _Header
                                            + "preflight route for "
                                            + ctx.Request.Source.IpAddress
                                            + ":"
                                            + ctx.Request.Source.Port
                                            + " "
                                            + ctx.Request.Method.ToString()
                                            + " "
                                            + ctx.Request.Url.RawWithoutQuery
                                    );
                                }

                                await Routes.Preflight(ctx).ConfigureAwait(false);
                                if (!ctx.Response.ResponseSent)
                                    throw new InvalidOperationException(
                                        "Preflight route for "
                                            + ctx.Request.Method.ToString()
                                            + " "
                                            + ctx.Request.Url.RawWithoutQuery
                                            + " did not send a response to the HTTP request."
                                    );
                                return;
                            }
                        }

                        #endregion

                        #region Pre-Routing-Handler

                        if (Routes.PreRouting != null)
                        {
                            await Routes.PreRouting(ctx).ConfigureAwait(false);
                            if (ctx.Response.ResponseSent)
                            {
                                if (Settings.Debug.Routing)
                                {
                                    Events.Logger?.Invoke(
                                        _Header
                                            + "prerouting terminated connection for "
                                            + ctx.Request.Source.IpAddress
                                            + ":"
                                            + ctx.Request.Source.Port
                                            + " "
                                            + ctx.Request.Method.ToString()
                                            + " "
                                            + ctx.Request.Url.RawWithoutQuery
                                    );
                                }

                                return;
                            }
                            else
                            {
                                // allow the connection to continue
                            }
                        }

                        #endregion

                        #region Pre-Authentication

                        if (Routes.PreAuthentication != null)
                        {
                            if (
                                await ProcessRoutingGroup(ctx, Routes.PreAuthentication, "pre-auth")
                                    .ConfigureAwait(false)
                            )
                                return;
                        }

                        #endregion

                        #region Authentication

                        if (Routes.AuthenticateRequest != null)
                        {
                            await Routes.AuthenticateRequest(ctx).ConfigureAwait(false);
                            if (ctx.Response.ResponseSent)
                            {
                                if (Settings.Debug.Routing)
                                {
                                    Events.Logger?.Invoke(
                                        _Header
                                            + "response sent during authentication for "
                                            + ctx.Request.Source.IpAddress
                                            + ":"
                                            + ctx.Request.Source.Port
                                            + " "
                                            + ctx.Request.Method.ToString()
                                            + " "
                                            + ctx.Request.Url.Full
                                    );
                                }

                                return;
                            }
                            else
                            {
                                // allow the connection to continue
                            }
                        }

                        #endregion

                        #region Post-Authentication

                        if (Routes.PostAuthentication != null)
                        {
                            if (
                                await ProcessRoutingGroup(
                                        ctx,
                                        Routes.PostAuthentication,
                                        "post-auth"
                                    )
                                    .ConfigureAwait(false)
                            )
                                return;
                        }

                        #endregion

                        #region Default-Route

                        if (Settings.Debug.Routing)
                        {
                            Events.Logger?.Invoke(
                                _Header
                                    + "default route for "
                                    + ctx.Request.Source.IpAddress
                                    + ":"
                                    + ctx.Request.Source.Port
                                    + " "
                                    + ctx.Request.Method.ToString()
                                    + " "
                                    + ctx.Request.Url.Full
                            );
                        }

                        if (Routes.Default != null)
                        {
                            ctx.RouteType = RouteTypeEnum.Default;
                            await Routes.Default(ctx).ConfigureAwait(false);
                            return;
                        }
                        else
                        {
                            ctx.Response.StatusCode = 404;
                            ctx.Response.ContentType = DefaultPages.Pages[404].ContentType;
                            if (ctx.Response.ChunkedTransfer)
                                await ctx
                                    .Response.SendChunk(
                                        Encoding.UTF8.GetBytes(DefaultPages.Pages[404].Content),
                                        true
                                    )
                                    .ConfigureAwait(false);
                            else
                                await ctx
                                    .Response.Send(DefaultPages.Pages[404].Content)
                                    .ConfigureAwait(false);
                            return;
                        }

                        #endregion
                    }
                    catch (HttpListenerException eListener)
                    {
                        // Unfortunately, some client side implementation of HTTP (like RPCS3) freeze the interface at regular interval.
                        // This will cause server to throw error 64 (network interface not openned anymore)
                        // In that case, we send internalservererror so client try again.

                        var errorCode = eListener.ErrorCode;

                        if (errorCode != 995 && errorCode != 64)
                            Events.HandleExceptionEncountered(
                                this,
                                new ExceptionEventArgs(ctx, eListener)
                            );
                    }
                    catch (Exception eInner)
                    {
                        Events.HandleExceptionEncountered(
                            this,
                            new ExceptionEventArgs(ctx, eInner)
                        );
                    }
                    finally
                    {
                        Interlocked.Decrement(ref _RequestCount);

                        ReleaseRequestSlot(requestSemaphore);

                        if (ctx != null)
                        {
                            ctx.Timestamp.End = DateTime.UtcNow;

                            Events.HandleResponseSent(
                                this,
                                new ResponseEventArgs(ctx, ctx.Timestamp.TotalMs.Value)
                            );

                            if (Settings.Debug.Responses)
                            {
                                Events.Logger?.Invoke(
                                    _Header
                                        + ctx.Request.Source.IpAddress
                                        + ":"
                                        + ctx.Request.Source.Port
                                        + " "
                                        + ctx.Request.Method.ToString()
                                        + " "
                                        + ctx.Request.Url.Full
                                        + ": "
                                        + ctx.Response.StatusCode
                                        + " ["
                                        + ctx.Timestamp.TotalMs.Value
                                        + "ms]"
                                );
                            }

                            if (ctx.Response.ContentLength > 0)
                                Statistics.IncrementSentPayloadBytes(
                                    Convert.ToInt64(ctx.Response.ContentLength)
                                );

                            if (Routes.PostRouting != null)
                            {
                                try
                                {
                                    await Routes.PostRouting.Invoke(ctx).ConfigureAwait(false);
                                }
                                catch
                                {
                                    // PostRouting exceptions should not mask the original response
                                }
                            }

                            ctx.Dispose();

                            if (ctx.Response is HttpResponseNative nativeResponse)
                                nativeResponse.Close();
                            else if (ctx.Response is HttpResponse managedResponse)
                                managedResponse.Close();
                        }
                        else
                        {
                            try
                            {
                                if (listenerCtx is System.Net.HttpListenerContext nativeCtx)
                                    nativeCtx.Response.Close();
                                else if (listenerCtx is HttpListenerContext managedCtx)
                                    managedCtx.Response.Close();
                            }
                            catch
                            {
                                // Not Important.
                            }
                        }
                    }
                },
                token
            );

        private async Task<bool> ProcessRoutingGroup(
            HttpContextBase ctx,
            RoutingGroup group,
            string authPhase
        )
        {
            Func<HttpContextBase, Task> handler = null;

            #region Static-Routes

            if (group.Static != null)
            {
                handler = group.Static.Match(
                    ctx.Request.Method,
                    ctx.Request.Url.RawWithoutQuery,
                    out StaticRoute sr
                );
                if (handler != null)
                {
                    if (Settings.Debug.Routing)
                    {
                        Events.Logger?.Invoke(
                            _Header
                                + authPhase
                                + " static route for "
                                + ctx.Request.Source.IpAddress
                                + ":"
                                + ctx.Request.Source.Port
                                + " "
                                + ctx.Request.Method.ToString()
                                + " "
                                + ctx.Request.Url.RawWithoutQuery
                        );
                    }

                    ctx.RouteType = RouteTypeEnum.Static;
                    ctx.Route = sr;

                    try
                    {
                        await handler(ctx).ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        if (sr.ExceptionHandler != null)
                            await sr.ExceptionHandler(ctx, e).ConfigureAwait(false);
                        else
                            throw;
                    }

                    if (!ctx.Response.ResponseSent)
                        throw new InvalidOperationException(
                            authPhase
                                + " static route for "
                                + ctx.Request.Method.ToString()
                                + " "
                                + ctx.Request.Url.RawWithoutQuery
                                + " did not send a response to the HTTP request."
                        );
                    return true;
                }
            }

            #endregion

            #region Content-Routes

            if (
                group.Content != null
                && (ctx.Request.Method == HttpMethod.GET || ctx.Request.Method == HttpMethod.HEAD)
            )
            {
                if (group.Content.Match(ctx.Request.Url.RawWithoutQuery, out ContentRoute cr))
                {
                    if (Settings.Debug.Routing)
                    {
                        Events.Logger?.Invoke(
                            _Header
                                + authPhase
                                + " content route for "
                                + ctx.Request.Source.IpAddress
                                + ":"
                                + ctx.Request.Source.Port
                                + " "
                                + ctx.Request.Method.ToString()
                                + " "
                                + ctx.Request.Url.RawWithoutQuery
                        );
                    }

                    ctx.RouteType = RouteTypeEnum.Content;
                    ctx.Route = cr;

                    try
                    {
                        await group.Content.Handler(ctx).ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        if (cr.ExceptionHandler != null)
                            await cr.ExceptionHandler(ctx, e).ConfigureAwait(false);
                        else
                            throw;
                    }

                    if (!ctx.Response.ResponseSent)
                        throw new InvalidOperationException(
                            authPhase
                                + " content route for "
                                + ctx.Request.Method.ToString()
                                + " "
                                + ctx.Request.Url.RawWithoutQuery
                                + " did not send a response to the HTTP request."
                        );
                    return true;
                }
            }

            #endregion

            #region Parameter-Routes

            if (group.Parameter != null)
            {
                handler = group.Parameter.Match(
                    ctx.Request.Method,
                    ctx.Request.Url.RawWithoutQuery,
                    out NameValueCollection parameters,
                    out ParameterRoute pr
                );
                if (handler != null)
                {
                    ctx.Request.Url.Parameters = parameters;

                    if (Settings.Debug.Routing)
                    {
                        Events.Logger?.Invoke(
                            _Header
                                + authPhase
                                + " parameter route for "
                                + ctx.Request.Source.IpAddress
                                + ":"
                                + ctx.Request.Source.Port
                                + " "
                                + ctx.Request.Method.ToString()
                                + " "
                                + ctx.Request.Url.RawWithoutQuery
                        );
                    }

                    ctx.RouteType = RouteTypeEnum.Parameter;
                    ctx.Route = pr;

                    try
                    {
                        await handler(ctx).ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        if (pr.ExceptionHandler != null)
                            await pr.ExceptionHandler(ctx, e).ConfigureAwait(false);
                        else
                            throw;
                    }

                    if (!ctx.Response.ResponseSent)
                        throw new InvalidOperationException(
                            authPhase
                                + " parameter route for "
                                + ctx.Request.Method.ToString()
                                + " "
                                + ctx.Request.Url.RawWithoutQuery
                                + " did not send a response to the HTTP request."
                        );
                    return true;
                }
            }

            #endregion

            #region Dynamic-Routes

            if (group.Dynamic != null)
            {
                handler = group.Dynamic.Match(
                    ctx.Request.Method,
                    ctx.Request.Url.RawWithoutQuery,
                    out DynamicRoute dr
                );
                if (handler != null)
                {
                    if (Settings.Debug.Routing)
                    {
                        Events.Logger?.Invoke(
                            _Header
                                + authPhase
                                + " dynamic route for "
                                + ctx.Request.Source.IpAddress
                                + ":"
                                + ctx.Request.Source.Port
                                + " "
                                + ctx.Request.Method.ToString()
                                + " "
                                + ctx.Request.Url.RawWithoutQuery
                        );
                    }

                    ctx.RouteType = RouteTypeEnum.Dynamic;
                    ctx.Route = dr;

                    try
                    {
                        await handler(ctx).ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        if (dr.ExceptionHandler != null)
                            await dr.ExceptionHandler(ctx, e).ConfigureAwait(false);
                        else
                            throw;
                    }

                    if (!ctx.Response.ResponseSent)
                        throw new InvalidOperationException(
                            authPhase
                                + " dynamic route for "
                                + ctx.Request.Method.ToString()
                                + " "
                                + ctx.Request.Url.RawWithoutQuery
                                + " did not send a response to the HTTP request."
                        );
                    return true;
                }
            }

            #endregion

            return false;
        }

        /// <summary>
        /// Tear down the server and dispose of background workers.
        /// Do not use this object after disposal.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Stop();

                Events.HandleServerDisposing(this, EventArgs.Empty);

                DisposeRequestSemaphore();

                Settings = null;

                _rateLimiter.Dispose();
            }
        }

        private void DisposeRequestSemaphore()
        {
            SemaphoreSlim requestSemaphore = Interlocked.Exchange(ref _RequestSemaphore, null);
            if (requestSemaphore == null)
                return;
            requestSemaphore.Dispose();
        }

        private static async Task<bool> TryAcquireRequestSlotAsync(SemaphoreSlim requestSemaphore, CancellationToken token)
        {
            if (requestSemaphore == null)
                return false;

            try
            {
                await requestSemaphore.WaitAsync(token).ConfigureAwait(false);
                return true;
            }
            catch (ObjectDisposedException)
            {
                // Not Important.
            }

            return false;
        }

        private static void ReleaseRequestSlot(SemaphoreSlim requestSemaphore)
        {
            if (requestSemaphore == null)
                return;

            try
            {
                requestSemaphore.Release();
            }
            catch (ObjectDisposedException)
            {
                // Not Important.
            }
        }

        #endregion
    }
}
