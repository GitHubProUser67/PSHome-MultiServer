using System.Net;
using System.Net.Sockets;
using CustomLogger;
using MultiServerLibrary.Extension;

namespace MultiServerLibrary.CustomServers
{
    public class UDPServer
    {
        private readonly Lock _Lock = new();

        private List<Task> _AcceptConnections = new();

        private readonly List<UdpClient> _listeners = new();
        private CancellationTokenSource _cts = null;

        public Task StartAsync(
            IEnumerable<ushort> ports,
            Action<ushort> onPrepareListener = null,
            Action<ushort, UdpClient> onInitalizedListener = null,
            Action<ushort> onUpdate = null,
            Func<ushort, UdpClient, byte[], IPEndPoint, byte[]> onPacketReceived = null,
            CancellationToken cancellationToken = default
        )
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

                foreach (var port in ports)
                {
                    if (TcpUdpUtils.IsUDPPortAvailable(port))
                        StartListener(
                            port,
                            onPrepareListener,
                            onInitalizedListener,
                            onUpdate,
                            onPacketReceived
                        );
                    else
                        LoggerAccessor.LogError(
                            $"[UDP Server] - Port:{port} is not available, skipping..."
                        );
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
                    try
                    {
                        listener.Dispose();
                    }
                    catch { }
                }

                _listeners.Clear();
                _cts = null;
            }

            _AcceptConnections = null;

            LoggerAccessor.LogInfo("[UDP Server] - All listeners stopped.");
        }

        public static bool IsIPBanned(ushort port, string ipAddress, int? clientport)
        {
            if (
                MultiServerLibraryConfiguration.BannedIPs != null
                && MultiServerLibraryConfiguration.BannedIPs.Contains(ipAddress)
            )
            {
                LoggerAccessor.LogError(
                    $"[SECURITY] - {ipAddress}:{clientport} Requested the UDP server on port {port} while being banned!"
                );
                return true;
            }

            return false;
        }

        private void StartListener(
            ushort port,
            Action<ushort> onPrepareListener,
            Action<ushort, UdpClient> onInitalizedListener,
            Action<ushort> onUpdate,
            Func<ushort, UdpClient, byte[], IPEndPoint, byte[]> onPacketReceived
        )
        {
            onPrepareListener?.Invoke(port);

            UdpClient listener;
            try
            {
                listener = new UdpClient(port);
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(
                    $"[UDP Server] - Failed to bind UDP port {port}. (Exception:" + ex + ")"
                );
                return;
            }

            onInitalizedListener?.Invoke(port, listener);

            _listeners.Add(listener);
            LoggerAccessor.LogInfo($"[UDP Server] - Listening on port {port}...");

            _AcceptConnections.Add(
                Task.Factory.StartNew(
                    () => AcceptConnections(port, listener, onUpdate, onPacketReceived, _cts.Token),
                    TaskCreationOptions.LongRunning
                )
            );
        }

        private static Task AcceptConnections(
            ushort port,
            UdpClient listener,
            Action<ushort> onUpdate,
            Func<ushort, UdpClient, byte[], IPEndPoint, byte[]> onPacketReceived,
            CancellationToken token
        )
        {
            List<Task> ClientTasks = new();

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
                            var result = CurrentRecvTask.Result;
                            CurrentRecvTask = null;
                            void clientHandler()
                            {
                                var remoteEndPoint = result.RemoteEndPoint;
#if DEBUG
                                LoggerAccessor.LogInfo(
                                    $"[UDP Server] - Connection received on port {port} (Thread {Environment.CurrentManagedThreadId})"
                                );
#endif
                                string clientip = null;
                                try
                                {
                                    clientip = remoteEndPoint.Address.ToString();
                                }
                                catch { }
                                int? clientport = remoteEndPoint.Port;
                                var isEndpointMissing =
                                    !clientport.HasValue || string.IsNullOrEmpty(clientip);
#if DEBUG
                                LoggerAccessor.LogInfo(
                                    $"[UDP Server] - endpoint = {!isEndpointMissing}"
                                );
#endif
                                if (
                                    !(
                                        isEndpointMissing
                                        || IsIPBanned(port, clientip, clientport)
                                        || (
                                            MultiServerLibraryConfiguration.VpnCheck != null
                                            && MultiServerLibraryConfiguration.VpnCheck.IsVpnOrProxy(
                                                clientip
                                            )
                                        )
                                    )
                                )
                                {
                                    var ResultBuffer = onPacketReceived?.Invoke(
                                        port,
                                        listener,
                                        result.Buffer,
                                        remoteEndPoint
                                    );
                                    if (ResultBuffer != null)
                                    {
                                        try
                                        {
                                            _ = listener.SendAsync(
                                                ResultBuffer,
                                                ResultBuffer.Length,
                                                remoteEndPoint
                                            );
                                        }
                                        catch (SocketException socketException)
                                        {
                                            if (
                                                socketException.ErrorCode != 995
                                                && socketException.SocketErrorCode
                                                    != SocketError.ConnectionReset
                                                && socketException.SocketErrorCode
                                                    != SocketError.ConnectionAborted
                                                && socketException.SocketErrorCode
                                                    != SocketError.Interrupted
                                            )
                                                LoggerAccessor.LogError(
                                                    $"[UDP Server] - SocketException while sending response to client. (Exception:"
                                                        + socketException
                                                        + ")"
                                                );
                                        }
                                        catch (Exception e)
                                        {
                                            LoggerAccessor.LogError(
                                                "[UDP Server] - Assertion while sending response to client. (Exception:"
                                                    + e
                                                    + ")"
                                            );
                                        }
                                    }
                                }
                            }
                            clientHandler();
                        }
                        else if (CurrentRecvTask.IsCanceled || CurrentRecvTask.IsFaulted)
                            CurrentRecvTask = null;
                    }

                    CurrentRecvTask ??= listener.ReceiveAsync(token).AsTask();
                }
                catch (OperationCanceledException)
                {
                    CurrentRecvTask = null;

                    break;
                }
                catch (SocketException socketException)
                {
                    if (
                        socketException.ErrorCode != 995
                        && socketException.SocketErrorCode != SocketError.ConnectionReset
                        && socketException.SocketErrorCode != SocketError.ConnectionAborted
                        && socketException.SocketErrorCode != SocketError.Interrupted
                    )
                        LoggerAccessor.LogWarn(
                            $"[UDP Server] - SocketException while accepting client on {port}. (Exception:"
                                + socketException
                                + ")"
                        );

                    CurrentRecvTask = null;
                }
                catch (Exception ex)
                {
#if DEBUG
                    LoggerAccessor.LogWarn(
                        $"[UDP Server] - Exception while accepting client on {port}. (Exception:"
                            + ex
                            + ")"
                    );
#endif
                    CurrentRecvTask = null;
                }

                Thread.Sleep(1);
            }

            return Task.CompletedTask;
        }
    }
}
