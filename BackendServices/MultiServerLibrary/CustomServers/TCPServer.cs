using CustomLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace MultiServerLibrary.CustomServers
{
    public class TCPServer
    {
        private readonly object _Lock = new object();

        public bool FireClientAsTask { get; set; } = true;

        private List<Task> _AcceptConnections = new();

        private readonly List<TcpListener> _listeners = new();
        private CancellationTokenSource _cts = null;

        public Task StartAsync(
            IEnumerable<ushort> ports,
            int maxConcurrentListeners,
            Action<ushort> onPrepareListener = null,
            Action<ushort, TcpListener> onInitalizedListener = null,
            Action<ushort> onUpdate = null,
            Action<ushort, TcpClient, IPEndPoint> onPacketReceived = null,
            CancellationToken cancellationToken = default)
        {
            if (ports == null || !ports.Any())
                return Task.CompletedTask;

            lock (_Lock)
            {
                
                if (_cts != null)
                {
                    LoggerAccessor.LogWarn("[TCP Server] - Server already active.");
                    return Task.CompletedTask;
                }

                _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

                foreach (ushort port in ports)
                {
                    if (Extension.TCPUtils.IsTCPPortAvailable(port))
                        StartListener(port, maxConcurrentListeners, onPrepareListener, onInitalizedListener, onUpdate, onPacketReceived);
                    else
                        LoggerAccessor.LogError($"[TCP Server] - Port:{port} is not available, skipping...");
                }
            }

            return Task.CompletedTask;
        }

        public void Stop()
        {
            lock (_Lock)
            {
                if (_cts == null)
                    return;

                _cts.Cancel();

                foreach (var listener in _listeners)
                {
                    try { listener.Stop(); } catch { }
                }

                _listeners.Clear();
                _cts = null;
            }

            _AcceptConnections = null;

            LoggerAccessor.LogInfo("[TCP Server] - All listeners stopped.");
        }

        public static bool IsIPBanned(ushort port, string ipAddress, int? clientport)
        {
            if (MultiServerLibraryConfiguration.BannedIPs != null && MultiServerLibraryConfiguration.BannedIPs.Contains(ipAddress))
            {
                LoggerAccessor.LogError($"[SECURITY] - {ipAddress}:{clientport} Requested the TCP Server on port {port} while being banned!");
                return true;
            }

            return false;
        }

        private void StartListener(ushort port, int maxConcurrentListeners, Action<ushort> onPrepareListener, Action<ushort, TcpListener> onInitalizedListener, Action<ushort> onUpdate, Action<ushort, TcpClient, IPEndPoint> onPacketReceived)
        {
            onPrepareListener?.Invoke(port);

            TcpListener listener = new TcpListener(IPAddress.Any, port);
            try
            {
                listener.Start();
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[TCP Server] - Failed to bind TCP port {port}. (Exception:" + ex + ")");
                return;
            }

            onInitalizedListener?.Invoke(port, listener);

            _listeners.Add(listener);
            LoggerAccessor.LogInfo($"[TCP Server] - Listening on port {port}...");

            _AcceptConnections.Add(Task.Run(() => AcceptConnections(port, maxConcurrentListeners, listener, onUpdate, onPacketReceived, _cts.Token), _cts.Token));
        }

        private Task AcceptConnections(
            ushort port,
            int maxConcurrentListeners,
            TcpListener listener,
            Action<ushort> onUpdate,
            Action<ushort, TcpClient, IPEndPoint> onPacketReceived,
            CancellationToken token)
        {
            List<Task> ClientTasks = new();

            try
            {
                while (!token.IsCancellationRequested)
                {
                    onUpdate?.Invoke(port);

                    while (ClientTasks.Count < maxConcurrentListeners) //Maximum number of concurrent listeners
                        ClientTasks.Add(Task.Run(async () =>
                        {
                            TcpClient client = null;
                            try
                            {
                                client = await listener.AcceptTcpClientAsync(token).ConfigureAwait(false);
                            }
                            catch (OperationCanceledException)
                            {

                            }
                            catch (Exception ex)
                            {
#if DEBUG
                                LoggerAccessor.LogWarn($"[TCP Server] - Exception while accepting client on {port}: (Exception:" + ex + ")");
#endif
                            }
                            if (client != null)
                            {
                                void clientHandler()
                                {
                                    IPEndPoint remoteEndPoint = null;
                                    try
                                    {
                                        remoteEndPoint = (IPEndPoint)client.Client.RemoteEndPoint;
                                    }
                                    catch { }
#if DEBUG
                                    LoggerAccessor.LogInfo($"[TCP Server] - Connection received on port {port} (Thread {Environment.CurrentManagedThreadId})");
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
                                    LoggerAccessor.LogInfo($"[TCP Server] - endpoint = {!isEndpointMissing}");
#endif
                                    if (!(isEndpointMissing || IsIPBanned(port, clientip, clientport) || (MultiServerLibraryConfiguration.VpnCheck != null && MultiServerLibraryConfiguration.VpnCheck.IsVpnOrProxy(clientip))))
                                        onPacketReceived?.Invoke(port, client, remoteEndPoint);
                                }
                                if (FireClientAsTask)
                                    _ = Task.Run(clientHandler);
                                else
                                    clientHandler();
                            }
                        }, token));

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
                LoggerAccessor.LogError($"[TCP Server] - Exception on port {port}. (Exception:" + ex + ")");
            }

            return Task.CompletedTask;
        }
    }
}
