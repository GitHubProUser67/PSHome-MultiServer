using CustomLogger;
using FixedSsl;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace BlazeCommon
{
    public abstract class ProtoFireServer
    {
        public string Name { get; private set; }
        public IPEndPoint LocalEP { get; private set; }
        public bool IsRunning { get; private set; }
        public X509Certificate2? Certificate { get; private set; }
        public bool ForceSsl { get; private set; }

        [MemberNotNullWhen(true, nameof(Certificate))]
        public bool Secure { get => Certificate != null; }

        private Socket? _listenSocket;
        private long _nextConnectionId;
        private ConcurrentDictionary<long, ProtoFireConnection> _connections;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
#pragma warning disable
        private static readonly SslProtocols _sslProtocols = SslProtocols.Default | SslProtocols.Tls11 | SslProtocols.Tls12;
#pragma warning restore
        public ProtoFireServer(string name, IPEndPoint localEP, X509Certificate2? cert, bool forceSsl)
        {
            Name = name;
            LocalEP = localEP;
            IsRunning = false;
            Certificate = cert;
            ForceSsl = forceSsl;

            _connections = new ConcurrentDictionary<long, ProtoFireConnection>();
            _cancellationTokenSource = new CancellationTokenSource();
            _nextConnectionId = 0;
        }

        public void KillConnection(ProtoFireConnection connection)
        {
            if (connection.Connected)
                connection.Disconnect(); //will call this method again after disconnect
            else
                OnProtoFireDisconnectInternalAsync(connection).GetAwaiter().GetResult();
        }

        public void Stop()
        {
            IsRunning = false;
            _cancellationTokenSource.Cancel();
        }

        public async Task Start(int backlog)
        {
            //check if already running or is cancelled
            if (IsRunning)
                return;

            if (_cancellationTokenSource.IsCancellationRequested)
                _cancellationTokenSource = new CancellationTokenSource();

            //server not running, start it
            try
            {
                LoggerAccessor.LogInfo($"[ProtoFireServer] - Starting {(Secure ? "secure" : "insecure")} ProtoFireServer({Name}) on port {LocalEP.Port}...");
                _listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _listenSocket.Bind(LocalEP);
                _listenSocket.Listen(backlog);
                IsRunning = true;
                LoggerAccessor.LogInfo($"[ProtoFireServer] - ProtoFireServer({Name}) started.");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[ProtoFireServer] - Failed to start {(Secure ? "secure" : "insecure")} ProtoFireServer({Name}) on port {LocalEP.Port} (Exception: {ex}).");
                IsRunning = false;
                return;
            }

            try
            {
                //start accepting connections
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    Socket socket = await _listenSocket.AcceptAsync(_cancellationTokenSource.Token).ConfigureAwait(false);
                    long clientId = Interlocked.Increment(ref _nextConnectionId);

                    ProtoFireConnection connection = new ProtoFireConnection(clientId, this, socket);
                    await OnProtoFireConnectInternalAsync(connection).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException) { }

            IsRunning = false;

            _listenSocket.Close();
            _nextConnectionId = 0;
            //kill all server connections
            foreach (var connection in _connections.Values)
                connection.Disconnect();
            _connections.Clear();

        }

        public async void AuthenticateAsServerCallback(IAsyncResult result)
        {
            ProtoFireConnection connection = (ProtoFireConnection)result.AsyncState!;

            try
            {
                Stream? stream = SslSocket.EndAuthenticateAsServer(result);
                if (stream == null)
                {
                    LoggerAccessor.LogError($"[ProtoFireServer] - Failed to authenticate as server for connection({connection.ID}).");
                    connection.Disconnect();
                    return;
                }

                connection.SetStream(stream);

                if (Secure)
                    LoggerAccessor.LogInfo($"[ProtoFireServer] - Authenticated as server for connection({connection.ID}). Stream type: {stream.GetType().Name}");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[ProtoFireServer] - Failed to authenticate as server for connection({connection.ID}) (Exception: {ex}).");
                connection.Disconnect();
                return;
            }

            while (IsRunning)
            {
                ProtoFirePacket? packet = await connection.ReadPacketAsync().ConfigureAwait(false);

                //disconnected
                if (packet == null)
                    break;

                try { await OnProtoFirePacketReceivedAsync(connection, packet).ConfigureAwait(false); }
                catch (Exception ex) { await OnProtoFireErrorInternalAsync(connection, ex).ConfigureAwait(false); }
            }

            connection.Disconnect();
        }

        public ValueTask KillConnectionAsync(ProtoFireConnection connection)
        {
            if (connection.Connected)
            {
                connection.Disconnect(); //will call this method again after disconnect
                return ValueTask.CompletedTask;
            }

            return OnProtoFireDisconnectInternalAsync(connection);
        }

        private async ValueTask OnProtoFireConnectInternalAsync(ProtoFireConnection connection)
        {
            if (!_connections.TryAdd(connection.ID, connection))
            {
                connection.Disconnect();
                return;
            }

            LoggerAccessor.LogInfo($"[ProtoFireServer] - Connection({connection.ID}) accepted from {connection.Socket.RemoteEndPoint}.");

            try
            {
                await OnProtoFireConnectAsync(connection).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await OnProtoFireErrorInternalAsync(connection, ex).ConfigureAwait(false);
            }

            if (connection.Connected)
            {
                if (Secure)
                    LoggerAccessor.LogInfo($"[ProtoFireServer] - Authenticating as server for connection({connection.ID}).");

                SslSocket.BeginAuthenticateAsServer(_sslProtocols, connection.Socket, Certificate, ForceSsl, true, AuthenticateAsServerCallback, connection);
            }
        }

        private async ValueTask OnProtoFireDisconnectInternalAsync(ProtoFireConnection connection)
        {
            if (!_connections.TryRemove(connection.ID, out _))
                return;

            LoggerAccessor.LogInfo($"[ProtoFireServer] - Connection({connection.ID}) disconnected.");

            try
            {
                await OnProtoFireDisconnectAsync(connection).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await OnProtoFireErrorInternalAsync(connection, ex).ConfigureAwait(false);
            }
        }


        private async Task OnProtoFireErrorInternalAsync(ProtoFireConnection connection, Exception exception)
        {
            try
            {
                await OnProtoFireErrorAsync(connection, exception).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                //an error occured while handling an error, doesnt sound good...
                await OnProtoFireErrorInternalAsync(connection, ex).ConfigureAwait(false);
            }
        }

        public abstract Task OnProtoFireConnectAsync(ProtoFireConnection connection);
        public abstract Task OnProtoFirePacketReceivedAsync(ProtoFireConnection connection, ProtoFirePacket packet);
        public abstract Task OnProtoFireDisconnectAsync(ProtoFireConnection connection);
        public abstract Task OnProtoFireErrorAsync(ProtoFireConnection connection, Exception exception);
    }
}
