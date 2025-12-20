using CustomLogger;
using MultiServerLibrary.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace MultiServerLibrary.CustomServers
{
    public class UDPServer
    {
        private readonly object _Lock = new object();

        public bool UseAlcatrazClientLoop { get; set; } = false;
        public bool FireClientAsTask { get; set; } = true;

        private List<Task> _AcceptConnections = new();

        private readonly List<UdpClient> _listeners = new();
        private CancellationTokenSource _cts = null;

        public Task StartAsync(
            IEnumerable<ushort> ports,
            int maxConcurrentListeners,
            Action<ushort> onPrepareListener = null,
            Action<ushort, UdpClient> onInitalizedListener = null,
            Action<ushort> onUpdate = null,
            Func<ushort, UdpClient, byte[], IPEndPoint, byte[]> onPacketReceived = null,
            CancellationToken cancellationToken = default)
        {
            if (ports == null || !ports.Any())
                return Task.CompletedTask;

            lock (_Lock)
            {
                if (_cts != null)
                {
                    LoggerAccessor.LogWarn("[UDP Server] - Server already active.");
                    return Task.CompletedTask;
                }

                _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

                foreach (ushort port in ports)
                {
                    if (Extension.TcpUdpUtils.IsUDPPortAvailable(port))
                        StartListener(port, maxConcurrentListeners, onPrepareListener, onInitalizedListener, onUpdate, onPacketReceived);
                    else
                        LoggerAccessor.LogError($"[UDP Server] - Port:{port} is not available, skipping...");
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
                    try { listener.Dispose(); } catch { }
                }

                _listeners.Clear();
                _cts = null;
            }

            _AcceptConnections = null;

            LoggerAccessor.LogInfo("[UDP Server] - All listeners stopped.");
        }

        public static bool IsIPBanned(ushort port, string ipAddress, int? clientport)
        {
            if (MultiServerLibraryConfiguration.BannedIPs != null && MultiServerLibraryConfiguration.BannedIPs.Contains(ipAddress))
            {
                LoggerAccessor.LogError($"[SECURITY] - {ipAddress}:{clientport} Requested the UDP server on port {port} while being banned!");
                return true;
            }

            return false;
        }

        private void StartListener(ushort port, int maxConcurrentListeners, Action<ushort> onPrepareListener, Action<ushort, UdpClient> onInitalizedListener, Action<ushort> onUpdate, Func<ushort, UdpClient, byte[], IPEndPoint, byte[]> onPacketReceived)
        {
            onPrepareListener?.Invoke(port);

            UdpClient listener;
            try
            {
                listener = new UdpClient(port);
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[UDP Server] - Failed to bind UDP port {port}. (Exception:" + ex + ")");
                return;
            }

            onInitalizedListener?.Invoke(port, listener);

            _listeners.Add(listener);
            LoggerAccessor.LogInfo($"[UDP Server] - Listening on port {port}...");

            _AcceptConnections.Add(Task.Factory.StartNew(() => AcceptConnections(port, maxConcurrentListeners, listener, onUpdate, onPacketReceived, _cts.Token), TaskCreationOptions.LongRunning));
        }

        private Task AcceptConnections(
            ushort port,
            int maxConcurrentListeners,
            UdpClient listener,
            Action<ushort> onUpdate,
            Func<ushort, UdpClient, byte[], IPEndPoint, byte[]> onPacketReceived,
            CancellationToken token)
        {
            List<Task> ClientTasks = new();

            if (UseAlcatrazClientLoop) // Provided for backward compatibility with the Quazal server (the packet handling is in-order and de-facto, not compatible with our approach).
            {
                Task<UdpReceiveResult> CurrentRecvTask = null;

                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        onUpdate?.Invoke(port);

                        // use non-blocking recieve
                        if (CurrentRecvTask != null)
                        {
                            if (CurrentRecvTask.IsCompleted)
                            {
                                UdpReceiveResult result = CurrentRecvTask.Result;
                                CurrentRecvTask = null;
                                void clientHandler()
                                {
                                    IPEndPoint remoteEndPoint = result.RemoteEndPoint;
#if DEBUG
                                    LoggerAccessor.LogInfo($"[UDP Server] - Connection received on port {port} (Thread {Environment.CurrentManagedThreadId})");
#endif
                                    string clientip = null;
                                    try
                                    {
                                        clientip = remoteEndPoint.Address.ToString();
                                    }
                                    catch { }
                                    int? clientport = remoteEndPoint.Port;
                                    bool isEndpointMissing = !clientport.HasValue || string.IsNullOrEmpty(clientip);
#if DEBUG
                                    LoggerAccessor.LogInfo($"[UDP Server] - endpoint = {!isEndpointMissing}");
#endif
                                    if (!(isEndpointMissing || IsIPBanned(port, clientip, clientport) || (MultiServerLibraryConfiguration.VpnCheck != null && MultiServerLibraryConfiguration.VpnCheck.IsVpnOrProxy(clientip))))
                                    {
                                        byte[] ResultBuffer = onPacketReceived?.Invoke(port, listener, result.Buffer, remoteEndPoint);
                                        if (ResultBuffer != null)
                                        {
                                            try
                                            {
                                                _ = listener.SendAsync(ResultBuffer, ResultBuffer.Length, remoteEndPoint);
                                            }
                                            catch (SocketException socketException)
                                            {
                                                if (socketException.ErrorCode != 995 &&
                                                    socketException.SocketErrorCode != SocketError.ConnectionReset &&
                                                    socketException.SocketErrorCode != SocketError.ConnectionAborted &&
                                                    socketException.SocketErrorCode != SocketError.Interrupted)
                                                    LoggerAccessor.LogError($"[UDP Server] - SocketException while sending response to client. (Exception:" + socketException + ")");
                                            }
                                            catch (Exception e)
                                            {
                                                LoggerAccessor.LogError("[UDP Server] - Assertion while sending response to client. (Exception:" + e + ")");
                                            }
                                        }
                                    }
                                }
                                if (FireClientAsTask)
                                    _ = Task.Run(clientHandler);
                                else
                                    clientHandler();
                            }
                            else if (CurrentRecvTask.IsCanceled || CurrentRecvTask.IsFaulted)
                                CurrentRecvTask = null;
                        }

                        if (CurrentRecvTask == null)
                            CurrentRecvTask = listener.ReceiveAsync(token).AsTask();
                    }
                    catch (OperationCanceledException)
                    {
                        CurrentRecvTask = null;

                        break;
                    }
                    catch (SocketException socketException)
                    {
                        if (socketException.ErrorCode != 995 &&
                            socketException.SocketErrorCode != SocketError.ConnectionReset &&
                            socketException.SocketErrorCode != SocketError.ConnectionAborted &&
                            socketException.SocketErrorCode != SocketError.Interrupted)
                            LoggerAccessor.LogWarn($"[UDP Server] - SocketException while accepting client on {port}. (Exception:" + socketException + ")");

                        CurrentRecvTask = null;
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        LoggerAccessor.LogWarn($"[UDP Server] - Exception while accepting client on {port}. (Exception:" + ex + ")");
#endif
                        CurrentRecvTask = null;
                    }

                    Thread.Sleep(1);
                }
            }
            else
            {
                try
                {
                    while (!token.IsCancellationRequested)
                    {
                        onUpdate?.Invoke(port);

                        while (ClientTasks.Count < maxConcurrentListeners) // Maximum number of concurrent listeners
                            ClientTasks.Add(Task.Run(async () =>
                            {
                                UdpReceiveResult result = default;
                                try
                                {
                                    result = await listener.ReceiveAsync(token).ConfigureAwait(false);
                                }
                                catch (OperationCanceledException)
                                {

                                }
                                catch (SocketException socketException)
                                {
                                    if (socketException.ErrorCode != 995 &&
                                        socketException.SocketErrorCode != SocketError.ConnectionReset &&
                                        socketException.SocketErrorCode != SocketError.ConnectionAborted &&
                                        socketException.SocketErrorCode != SocketError.Interrupted)
                                        LoggerAccessor.LogWarn($"[UDP Server] - SocketException while accepting client on {port}. (Exception:" + socketException + ")");
                                }
                                catch (Exception ex)
                                {
#if DEBUG
                                    LoggerAccessor.LogWarn($"[UDP Server] - Exception while accepting client on {port}. (Exception:" + ex + ")");
#endif
                                }
                                if (result != default)
                                {
                                    void clientHandler()
                                    {
                                        IPEndPoint remoteEndPoint = result.RemoteEndPoint;
#if DEBUG
                                        LoggerAccessor.LogInfo($"[UDP Server] - Connection received on port {port} (Thread {Environment.CurrentManagedThreadId})");
#endif
                                        string clientip = null;
                                        try
                                        {
                                            clientip = remoteEndPoint.Address.ToString();
                                        }
                                        catch { }
                                        int? clientport = remoteEndPoint.Port;
                                        bool isEndpointMissing = !clientport.HasValue || string.IsNullOrEmpty(clientip);
#if DEBUG
                                        LoggerAccessor.LogInfo($"[UDP Server] - endpoint = {!isEndpointMissing}");
#endif
                                        if (!(isEndpointMissing || IsIPBanned(port, clientip, clientport) || (MultiServerLibraryConfiguration.VpnCheck != null && MultiServerLibraryConfiguration.VpnCheck.IsVpnOrProxy(clientip))))
                                        {
                                            byte[] ResultBuffer = onPacketReceived?.Invoke(port, listener, result.Buffer, remoteEndPoint);
                                            if (ResultBuffer != null)
                                            {
                                                try
                                                {
                                                    _ = listener.SendAsync(ResultBuffer, ResultBuffer.Length, remoteEndPoint);
                                                }
                                                catch (SocketException socketException)
                                                {
                                                    if (socketException.ErrorCode != 995 &&
                                                        socketException.SocketErrorCode != SocketError.ConnectionReset &&
                                                        socketException.SocketErrorCode != SocketError.ConnectionAborted &&
                                                        socketException.SocketErrorCode != SocketError.Interrupted)
                                                        LoggerAccessor.LogError($"[UDP Server] - SocketException while sending response to client. (Exception:" + socketException + ")");
                                                }
                                                catch (Exception e)
                                                {
                                                    LoggerAccessor.LogError("[UDP Server] - Assertion while sending response to client. (Exception:" + e + ")");
                                                }
                                            }
                                        }
                                    }
                                    if (FireClientAsTask)
                                        _ = Task.Run(clientHandler);
                                    else
                                        clientHandler();
                                }
                            }, token));

                        int RemoveAtIndex = Task.WaitAny(ClientTasks.ToArray(), ProcessUtils.CustomServersLoopWaitTimeMs, token); // Synchronously Waits up for any Task completion
                        if (RemoveAtIndex != -1) // Remove the completed task from the list and burn a very few cycles to not burn our CPU.
                        {
                            ClientTasks.RemoveAt(RemoveAtIndex);

                            Thread.Sleep(1);
                        }
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
                    LoggerAccessor.LogError($"[UDP Server] - Exception on port {port}: (Exception:" + ex + ")");
                }
            }

            return Task.CompletedTask;
        }
    }
}
