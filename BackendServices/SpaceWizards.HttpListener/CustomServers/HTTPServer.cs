using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CustomLogger;
using MultiServerLibrary;

namespace SpaceWizards.HttpListener.CustomServers
{
    public class HTTPServer
    {
        private static readonly bool _httpSysCompatible =
#if ENABLE_HTTPSYS_CUSTOM_SERVER
                MultiServerLibrary.Extension.Microsoft.Win32API.IsWindows
                && MultiServerLibrary.Extension.Microsoft.Win32API.IsAdministrator()
                && System.Net.HttpListener.IsSupported;
#else
                false;
#endif
        private readonly object _Lock = new object();

        public bool FireClientAsTask { get; set; } = true;
        public bool PreferHttpSys { get; set; } = true;
        public string Prefix { get; set; } = null;
        public string Host { get; set; } = "*";

        private List<Task> _AcceptConnections = new();

        private readonly List<object> _listeners = new();
        private CancellationTokenSource _cts = null;

        public Task<bool> StartAsync(
            IDictionary<ushort, bool> portsConfiguration,
            int maxConcurrentListeners,
            Action<ushort, object> onPrepareListener = null,
            Action<ushort, object> onInitalizedListener = null,
            Func<ushort, bool> onUpdate = null,
            Action<ushort, object, IPEndPoint> onPacketReceived = null,
            CancellationToken cancellationToken = default)
        {
            if (portsConfiguration == null || !portsConfiguration.Any())
                return Task.FromResult(false);

            lock (_Lock)
            {
                if (_cts != null)
                {
                    LoggerAccessor.LogWarn("[HTTP Server] - Server already active.");
                    return Task.FromResult(false);
                }

                _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

                foreach (var portConfig in portsConfiguration)
                {
                    ushort port = portConfig.Key;

                    if (MultiServerLibrary.Extension.TcpUdpUtils.IsTCPPortAvailable(port))
                        StartListener(portConfig, maxConcurrentListeners, onPrepareListener, onInitalizedListener, onUpdate, onPacketReceived);
                    else
                        LoggerAccessor.LogError($"[HTTP Server] - Port:{port} is not available, skipping...");
                }
            }

            return Task.FromResult(true);
        }

        public bool Stop()
        {
            lock (_Lock)
            {
                if (_cts == null)
                    return false;

                _cts.Cancel();

                foreach (var listener in _listeners)
                {
                    if (listener is System.Net.HttpListener native)
                    {
                        try
                        {
                            // Get the prefixes that the Web server is listening to.
                            native.Prefixes.Clear();
                        }
                        catch { }
                        try { native.Close(); } catch { }
                    }
                    else if (listener is HttpListener managed)
                    {
                        try
                        {
                            // Get the prefixes that the Web server is listening to.
                            managed.Prefixes.Clear();
                        }
                        catch { }
                        try { managed.Close(); } catch { }
                    }
                }

                _listeners.Clear();
                _cts = null;
            }

            _AcceptConnections = null;

            LoggerAccessor.LogInfo("[HTTP Server] - All listeners stopped.");

            return true;
        }

        public bool IsAnyListening()
        {
            lock (_Lock)
            {
                if (_cts == null)
                    return false;

                return _listeners.Any(listener =>
                {
                    if (listener is System.Net.HttpListener native)
                        return native.IsListening;
                    else if (listener is HttpListener managed)
                        return managed.IsListening;

                    return false;
                });
            }
        }

        public static bool IsIPBanned(ushort port, string ipAddress, int? clientport)
        {
            if (MultiServerLibraryConfiguration.BannedIPs != null && MultiServerLibraryConfiguration.BannedIPs.Contains(ipAddress))
            {
                LoggerAccessor.LogError($"[SECURITY] - {ipAddress}:{clientport} Requested the HTTP Server on port {port} while being banned!");
                return true;
            }

            return false;
        }

        private void StartListener(KeyValuePair<ushort, bool> portConfiguration, int maxConcurrentListeners, Action<ushort, object> onPrepareListener, Action<ushort, object> onInitalizedListener, Func<ushort, bool> onUpdate, Action<ushort, object, IPEndPoint> onPacketReceived)
        {
            bool isSecure = portConfiguration.Value;

            // Native HttpListener on Linux is bugged (SpaceWizards is a fixed version based on this implementation) and requires admin rights to listen on 0.0.0.0.
            // It also doesn't have any meaningful ssl handling (only at an OS level and Windows only), but is significantly faster for classic HTTP as it is developed in C (for benchmarks?) and backed into HttpSys.
            if (PreferHttpSys
                && _httpSysCompatible
                && !isSecure)
            {
                StartHttpSysListener(portConfiguration, maxConcurrentListeners, onPrepareListener, onInitalizedListener, onUpdate, onPacketReceived);
                return;
            }

            ushort port = portConfiguration.Key;

            HttpListener listener = new();

            listener.Prefixes.Add(string.IsNullOrEmpty(Prefix) ? (isSecure ? string.Format("https://{0}:{1}/", Host, port) : string.Format("http://{0}:{1}/", Host, port)) : Prefix);

            onPrepareListener?.Invoke(port, listener);

            try
            {
                listener.Start();
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[HTTP Server] - Failed to bind TCP port {port}. (Exception:" + ex + ")");
                return;
            }

            onInitalizedListener?.Invoke(port, listener);

            _listeners.Add(listener);
            LoggerAccessor.LogInfo($"[HTTP Server] - Listening on port {port}...");

            _AcceptConnections.Add(Task.Run(() => AcceptConnections(port, maxConcurrentListeners, listener, onUpdate, onPacketReceived, _cts.Token), _cts.Token));
        }

        private void StartHttpSysListener(KeyValuePair<ushort, bool> portConfiguration, int maxConcurrentListeners, Action<ushort, object> onPrepareListener, Action<ushort, object> onInitalizedListener, Func<ushort, bool> onUpdate, Action<ushort, object, IPEndPoint> onPacketReceived)
        {
            ushort port = portConfiguration.Key;

            System.Net.HttpListener listener = new();

            listener.Prefixes.Add(string.IsNullOrEmpty(Prefix) ? string.Format("http://{0}:{1}/", Host, port) : Prefix);

            onPrepareListener?.Invoke(port, listener);

            try
            {
                listener.Start();
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[HTTPsys Server] - Failed to bind TCP port {port}. (Exception:" + ex + ")");
                return;
            }

            onInitalizedListener?.Invoke(port, listener);

            _listeners.Add(listener);
            LoggerAccessor.LogInfo($"[HTTPsys Server] - Listening on port {port}...");

            _AcceptConnections.Add(Task.Run(() => AcceptHttpSysConnections(port, maxConcurrentListeners, listener, onUpdate, onPacketReceived, _cts.Token), _cts.Token));
        }

        private Task AcceptConnections(
            ushort port,
            int maxConcurrentListeners,
            HttpListener listener,
            Func<ushort, bool> onUpdate,
            Action<ushort, object, IPEndPoint> onPacketReceived,
            CancellationToken token)
        {
            List<Task> ClientTasks = new();

            try
            {
                while (!token.IsCancellationRequested)
                {
                    if (onUpdate == null || onUpdate.Invoke(port))
                    {
                        while (ClientTasks.Count < maxConcurrentListeners) //Maximum number of concurrent listeners
                            ClientTasks.Add(Task.Run(async () =>
                            {
                                HttpListenerContext ctx = null;
                                try
                                {
                                    ctx = await listener.GetContextAsync().ConfigureAwait(false);
                                }
                                catch (ObjectDisposedException)
                                {
                                    // Called when the listener is disposed.
                                }
                                catch (Exception ex)
                                {
#if DEBUG
                                    LoggerAccessor.LogWarn($"[HTTP Server] - Exception while accepting client on {port}: (Exception:" + ex + ")");
#endif
                                }
                                if (ctx != null)
                                {
                                    void clientHandler()
                                    {
                                        IPEndPoint remoteEndPoint = null;
                                        try
                                        {
                                            remoteEndPoint = ctx.Request.RemoteEndPoint;
                                        }
                                        catch { }
#if DEBUG
                                        LoggerAccessor.LogInfo($"[HTTP Server] - Connection received on port {port} (Thread {Environment.CurrentManagedThreadId})");
#endif
                                        string clientip = null;
                                        try
                                        {
                                            clientip = remoteEndPoint?.Address.ToString();
                                        }
                                        catch { }
                                        int? clientport = remoteEndPoint?.Port;
                                        bool isEndpointMissing = !clientport.HasValue || string.IsNullOrEmpty(clientip);
#if DEBUG
                                        LoggerAccessor.LogInfo($"[HTTP Server] - endpoint = {!isEndpointMissing}");
#endif
                                        if (!(isEndpointMissing || IsIPBanned(port, clientip, clientport) || (MultiServerLibraryConfiguration.VpnCheck != null && MultiServerLibraryConfiguration.VpnCheck.IsVpnOrProxy(clientip))))
                                            onPacketReceived?.Invoke(port, ctx, remoteEndPoint);
                                    }
                                    if (FireClientAsTask)
                                        _ = Task.Run(clientHandler);
                                    else
                                        clientHandler();
                                }
                            }, token));
                    }

                    int RemoveAtIndex = Task.WaitAny(ClientTasks.ToArray(), 500, token); //Synchronously Waits up 500ms for any Task completion
                    if (RemoveAtIndex != -1) //Remove the completed task from the list
                        ClientTasks.RemoveAt(RemoveAtIndex);
                }
            }
            catch (TaskCanceledException)
            {

            }
            catch (OperationCanceledException)
            {

            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[HTTP Server] - Exception on port {port}. (Exception:" + ex + ")");
            }

            return Task.CompletedTask;
        }

        private Task AcceptHttpSysConnections(
            ushort port,
            int maxConcurrentListeners,
            System.Net.HttpListener listener,
            Func<ushort, bool> onUpdate,
            Action<ushort, object, IPEndPoint> onPacketReceived,
            CancellationToken token)
        {
            List<Task> ClientTasks = new();

            try
            {
                while (!token.IsCancellationRequested)
                {
                    if (onUpdate == null || onUpdate.Invoke(port))
                    {
                        while (ClientTasks.Count < maxConcurrentListeners) //Maximum number of concurrent listeners
                            ClientTasks.Add(Task.Run(async () =>
                            {
                                System.Net.HttpListenerContext ctx = null;
                                try
                                {
                                    ctx = await listener.GetContextAsync().ConfigureAwait(false);
                                }
                                catch (ObjectDisposedException)
                                {
                                    // Called when the listener is disposed.
                                }
                                catch (Exception ex)
                                {
#if DEBUG
                                    LoggerAccessor.LogWarn($"[HTTPsys Server] - Exception while accepting client on {port}: (Exception:" + ex + ")");
#endif
                                }
                                if (ctx != null)
                                {
                                    void clientHandler()
                                    {
                                        IPEndPoint remoteEndPoint = null;
                                        try
                                        {
                                            remoteEndPoint = ctx.Request.RemoteEndPoint;
                                        }
                                        catch { }
#if DEBUG
                                        LoggerAccessor.LogInfo($"[HTTPsys Server] - Connection received on port {port} (Thread {Environment.CurrentManagedThreadId})");
#endif
                                        string clientip = null;
                                        try
                                        {
                                            clientip = remoteEndPoint?.Address.ToString();
                                        }
                                        catch { }
                                        int? clientport = remoteEndPoint?.Port;
                                        bool isEndpointMissing = !clientport.HasValue || string.IsNullOrEmpty(clientip);
#if DEBUG
                                        LoggerAccessor.LogInfo($"[HTTPsys Server] - endpoint = {!isEndpointMissing}");
#endif
                                        if (!(isEndpointMissing || IsIPBanned(port, clientip, clientport) || (MultiServerLibraryConfiguration.VpnCheck != null && MultiServerLibraryConfiguration.VpnCheck.IsVpnOrProxy(clientip))))
                                            onPacketReceived?.Invoke(port, ctx, remoteEndPoint);
                                    }
                                    if (FireClientAsTask)
                                        _ = Task.Run(clientHandler);
                                    else
                                        clientHandler();
                                }
                            }, token));
                    }

                    int RemoveAtIndex = Task.WaitAny(ClientTasks.ToArray(), 500, token); //Synchronously Waits up 500ms for any Task completion
                    if (RemoveAtIndex != -1) //Remove the completed task from the list
                        ClientTasks.RemoveAt(RemoveAtIndex);
                }
            }
            catch (TaskCanceledException)
            {

            }
            catch (OperationCanceledException)
            {

            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[HTTPsys Server] - Exception on port {port}. (Exception:" + ex + ")");
            }

            return Task.CompletedTask;
        }
    }
}
