using CustomLogger;
using FixedSsl;
using Org.BouncyCastle.Bcpg;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace BlazeCommon
{
    public class MitmBlazePacket
    {
        public ushort Length;
        public ushort Component;
        public ushort Command;
        public ushort Error;
        public ushort QType;
        public ushort ID;
        public ushort extLength;
        public byte[]? Content;
    }

    public abstract class MitmProtoFireServer
    {
        const int ReadTimeout = 100;

        const string blazeDumpDir = "blaze_dump";

        public string Name { get; private set; }
        public IPEndPoint LocalEP { get; private set; }
        public bool IsRunning { get; private set; }
        public uint AddressEncryptionKey { get; private set; }
        public X509Certificate2? Certificate { get; private set; }
        public bool ForceSsl { get; private set; }

        [MemberNotNullWhen(true, nameof(Certificate))]
        public bool Secure { get => Certificate != null; }
        public BlazeServerConfiguration Configuration { get; }

        private Socket? _listenSocket;
        private long _nextConnectionId;
        private ConcurrentDictionary<long, ProtoFireConnection> _connections;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public MitmProtoFireServer(BlazeServerConfiguration settings, uint addressEncryptionKey)
        {
            AddressEncryptionKey = addressEncryptionKey;

            Configuration = settings;

            Name = settings.Name;
            LocalEP = settings.LocalEP;
            IsRunning = false;
            Certificate = settings.Certificate;
            ForceSsl = settings.ForceSsl;

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
                LoggerAccessor.LogInfo($"[MitmProtoFireServer] - Starting {(Secure ? "secure" : "insecure")} MitmProtoFireServer({Name}) on port {LocalEP.Port}...");
                _listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _listenSocket.Bind(LocalEP);
                _listenSocket.Listen(backlog);
                IsRunning = true;
                LoggerAccessor.LogInfo($"[MitmProtoFireServer] - MitmProtoFireServer({Name}) started.");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[MitmProtoFireServer] - Failed to start {(Secure ? "secure" : "insecure")} MitmProtoFireServer({Name}) on port {LocalEP.Port} (Exception: {ex}).");
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
            string blazePcapDir;
            ProtoFireConnection connection = (ProtoFireConnection)result.AsyncState!;

            try
            {
                blazePcapDir = blazeDumpDir + $"/{Name}/{Configuration.MitmTargetHostname}/{GetCipheredRemoteIPvalue(connection)}/{DateTime.UtcNow:yyyyMMdd_HHmmss}";

                Stream? stream = SslSocket.EndAuthenticateAsServer(result);
                if (stream == null)
                {
                    LoggerAccessor.LogError($"[MitmProtoFireServer] - Failed to authenticate as server for connection({connection.ID}).");
                    connection.Disconnect();
                    return;
                }

                connection.SetStream(stream);

                if (Secure)
                    LoggerAccessor.LogInfo($"[MitmProtoFireServer] - Authenticated as server for connection({connection.ID}). Stream type: {stream.GetType().Name}");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[MitmProtoFireServer] - Failed to authenticate as server for connection({connection.ID}) (Exception: {ex}).");
                connection.Disconnect();
                return;
            }

            // Use a named tuple so it's clearer when using ref
            (TcpClient? target, SslStream? stream) targetClient = (null, null);

            // First-time target connection (now pass by ref so RestartTargetConnectionAsync updates caller tuple)
            if (!await RestartTargetConnectionAsync(ref targetClient, connection).ConfigureAwait(false))
            {
                // failed to connect to target initially
                connection.Disconnect();
                return;
            }

            int clientCounter = 0;
            int targetCounter = 0;
            byte[] clientRequest;
            byte[] targetResponse;

            if (Configuration.MitmWriteToFile)
            {
                try
                {
                    Directory.CreateDirectory(blazePcapDir);
                }
                catch
                {
                }
            }

            ProtoFirePacket? packet;

            // local references - will be refreshed from the tuple each loop iteration and after reconnects
            TcpClient? target = targetClient.target;
            SslStream? targetStream = targetClient.stream;

            while (IsRunning && connection.Connected)
            {
                // small delay, but check connection health frequently
                await Task.Delay(10).ConfigureAwait(false);
                clientRequest = Array.Empty<byte>();
                targetResponse = Array.Empty<byte>();

                try
                {
                    // refresh local references (in case tuple was updated by RestartTargetConnectionAsync previously)
                    target = targetClient.target;
                    targetStream = targetClient.stream;

                    // if target connection dropped, attempt to restart before any I/O
                    if (target == null || target.Client == null || !target.Connected || targetStream == null)
                    {
                        LoggerAccessor.LogWarn($"[MitmProtoFireServer] - Target not connected. Attempting restart for connection({connection.ID}).");
                        if (!await RestartTargetConnectionAsync(ref targetClient, connection).ConfigureAwait(false))
                        {
                            LoggerAccessor.LogError($"[MitmProtoFireServer] - Failed to restart target for connection({connection.ID}). Disconnecting client.");
                            break;
                        }

                        // refresh local references after a successful reconnect
                        target = targetClient.target;
                        targetStream = targetClient.stream;
                    }

                    clientRequest = ReadContent(connection.Stream); // may block/throw
                    packet = await ReadPacketBytes(clientRequest).ConfigureAwait(false);
                    if (clientRequest.Length >= 0xC)
                    {
                        LoggerAccessor.LogInfo($"[MitmProtoFireServer] - Outgoing packet for connection({connection.ID}) -> {{{BitConverter.ToString(clientRequest).Replace("-", string.Empty)}}}.");
                        if (Configuration.MitmWriteToFile)
                        {
                            if (packet != null)
                                _ = File.WriteAllTextAsync(blazePcapDir + $"/outgoing_{clientCounter}.log", BlazeUtils.LogPacket(Configuration.GetComponent(packet.Frame.Component), DecodeMitmPacket(packet), true));
                            File.WriteAllBytes(blazePcapDir + $"/outgoing_{clientCounter}.cap", clientRequest);
                        }
                        else if (packet != null)
                            BlazeUtils.LogPacket(Configuration.GetComponent(packet.Frame.Component), DecodeMitmPacket(packet), true);
                        clientCounter++;

                        // wrap send to target in try so we can detect broken pipe / socket closed
                        try
                        {
                            await targetStream!.WriteAsync(clientRequest, 0, clientRequest.Length).ConfigureAwait(false);
                            await targetStream.FlushAsync().ConfigureAwait(false);
                        }
                        catch (Exception writeEx)
                        {
                            LoggerAccessor.LogWarn($"[MitmProtoFireServer] - Write to target failed for connection({connection.ID}) (Exception: {writeEx}). Attempting restart.");
                            bool reconnected = await RestartTargetConnectionAsync(ref targetClient, connection).ConfigureAwait(false);
                            if (!reconnected)
                            {
                                LoggerAccessor.LogError($"[MitmProtoFireServer] - Could not reconnect to target after write failure for connection({connection.ID}).");
                                break;
                            }

                            // refresh local references after reconnect and optionally retry once immediately
                            target = targetClient.target;
                            targetStream = targetClient.stream;

                            await targetStream!.WriteAsync(clientRequest, 0, clientRequest.Length).ConfigureAwait(false);
                            await targetStream.FlushAsync().ConfigureAwait(false);
                        }
                    }

                    // Read response from target (SSL stream)
                    try
                    {
                        targetResponse = ReadContentSSL(targetStream!);
                    }
                    catch (Exception readTargetEx)
                    {
                        LoggerAccessor.LogWarn($"[MitmProtoFireServer] - Read from target failed for connection({connection.ID}) (Exception: {readTargetEx}). Attempting restart.");
                        if (!await RestartTargetConnectionAsync(ref targetClient, connection).ConfigureAwait(false))
                        {
                            LoggerAccessor.LogError($"[MitmProtoFireServer] - Could not reconnect to target after read failure for connection({connection.ID}).");
                            break;
                        }

                        // refresh local references after reconnect, then try reading again
                        target = targetClient.target;
                        targetStream = targetClient.stream;

                        targetResponse = ReadContentSSL(targetStream!);
                    }

                    packet = await ReadPacketBytes(targetResponse).ConfigureAwait(false);
                    if (targetResponse.Length > 5 && targetResponse[0] == 0x17)
                    {
                        using (MemoryStream m = new MemoryStream())
                        {
                            m.Write(targetResponse, 5, targetResponse.Length - 5);
                            targetResponse = m.ToArray();
                        }
                    }
                    if (targetResponse.Length >= 0xC)
                    {
                        LoggerAccessor.LogInfo($"[MitmProtoFireServer] - Incomming packet for connection({connection.ID}) -> {{{BitConverter.ToString(targetResponse).Replace("-", string.Empty)}}}.");
                        if (Configuration.MitmWriteToFile)
                        {
                            if (packet != null)
                                _ = File.WriteAllTextAsync(blazePcapDir + $"/incomming_{targetCounter}.log", BlazeUtils.LogPacket(Configuration.GetComponent(packet.Frame.Component), DecodeMitmPacket(packet), true));
                            File.WriteAllBytes(blazePcapDir + $"/incomming_{targetCounter}.cap", targetResponse);
                        }
                        else if (packet != null)
                            BlazeUtils.LogPacket(Configuration.GetComponent(packet.Frame.Component), DecodeMitmPacket(packet), true);
                        targetCounter++;
                        connection.Stream?.Write(targetResponse, 0, targetResponse.Length);
                        connection.Stream?.Flush();
                    }
                }
                catch (Exception ex)
                {
                    // try to detect if it's target related; best effort: SocketException, IOException, AuthenticationException
                    if (IsTargetRelatedException(ex))
                    {
                        LoggerAccessor.LogWarn($"[MitmProtoFireServer] - Detected target-related exception for connection({connection.ID}) (Exception: {ex}). Attempting restart.");
                        bool ok = await RestartTargetConnectionAsync(ref targetClient, connection).ConfigureAwait(false);
                        if (!ok)
                        {
                            await OnProtoFireErrorInternalAsync(connection, ex).ConfigureAwait(false);
                            break;
                        }

                        // refresh local references and continue main loop after reconnect
                        target = targetClient.target;
                        targetStream = targetClient.stream;
                        continue;
                    }

                    await OnProtoFireErrorInternalAsync(connection, ex).ConfigureAwait(false);
                    break;
                }
            }

            // final cleanup
            _ = Task.Run(() => {
                try { targetClient.stream?.Dispose(); } catch { }
                try { targetClient.target?.Dispose(); } catch { }
            });

            connection.Disconnect();
        }

        /// <summary>
        /// Tries to dispose the current target connection and create a new one with retries and backoff.
        /// Returns true if successful, false otherwise.
        /// Updates the provided tuple (by ref) so the caller sees the new objects.
        /// </summary>
        private Task<bool> RestartTargetConnectionAsync(
            ref (TcpClient? target, SslStream? stream) targetClient,
            ProtoFireConnection connection,
            int maxAttempts = 3,
            int baseDelayMs = 250)
        {
            int attempt = 0;
            Exception? lastEx = null;

            // Start by disposing any existing objects referenced in the tuple (best-effort)
            try
            {
                if (targetClient.stream != null)
                {
                    try { targetClient.stream.Close(); targetClient.stream.Dispose(); } catch { }
                    targetClient.stream = null;
                }
            }
            catch { /* ignore */ }

            try
            {
                if (targetClient.target != null)
                {
                    try { targetClient.target.Close(); targetClient.target.Dispose(); } catch { }
                    targetClient.target = null;
                }
            }
            catch { /* ignore */ }

            // We'll use local variables while attempting, then assign back to the tuple on success
            TcpClient? target = null;
            SslStream? targetStream = null;

            while (attempt < maxAttempts && IsRunning && connection.Connected)
            {
                attempt++;
                try
                {
                    target = new TcpClient();
                    // optional timeout for connect
                    var timeout = Task.Delay(5000);
                    if (Task.WhenAny(target.ConnectAsync(Configuration.MitmTargetIp, Configuration.MitmTargetPort), timeout).Result == timeout)
                        throw new TimeoutException("Timed out while connecting to target.");

                    LoggerAccessor.LogInfo($"[MitmProtoFireServer] - Connected to target for connection({connection.ID}) (attempt {attempt}).");

                    // create SSL stream and authenticate as client
                    targetStream = new SslStream(target.GetStream(), true, new RemoteCertificateValidationCallback(ValidateAlways), null);

                    targetStream.AuthenticateAsClient(new SslClientAuthenticationOptions
                    {
                        TargetHost = Configuration.MitmTargetHostname,
                        EnabledSslProtocols = Configuration.MitmProtocols,
                    });

                    LoggerAccessor.LogInfo($"[MitmProtoFireServer] - Authenticated as client for connection({connection.ID}) (attempt {attempt}).");

                    // success: update the caller tuple so they see the new objects
                    targetClient = (target, targetStream);
                    return Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    lastEx = ex;
                    LoggerAccessor.LogWarn($"[MitmProtoFireServer] - Restart attempt {attempt}/{maxAttempts} failed for connection({connection.ID}) (Exception: {ex}).");

                    // dispose partially created objects before retry
                    try { targetStream?.Close(); targetStream?.Dispose(); } catch { }
                    targetStream = null;
                    try { target?.Close(); target?.Dispose(); } catch { }
                    target = null;

                    // exponential backoff (best-effort)
                    Thread.Sleep(baseDelayMs * attempt);
                }
            }

            LoggerAccessor.LogError($"[MitmProtoFireServer] - All restart attempts failed for connection({connection.ID}). Last exception: {lastEx}");

            // ensure tuple does not hold any stale references
            targetClient = (null, null);
            return Task.FromResult(false);
        }

        /// <summary>
        /// Best-effort check if exception is likely related to the target socket/ssl
        /// </summary>
        private bool IsTargetRelatedException(Exception ex)
        {
            if (ex == null) return false;
            // direct socket/io exceptions
            if (ex is SocketException) return true;
            if (ex is IOException) return true;
            if (ex is AuthenticationException) return true;
            // unwrap AggregateException / InnerException
            if (ex is AggregateException agg)
            {
                foreach (var inner in agg.InnerExceptions)
                    if (IsTargetRelatedException(inner)) return true;
            }
            if (ex.InnerException != null) return IsTargetRelatedException(ex.InnerException);
            // fallback: check message text (not ideal, but sometimes useful)
            var msg = ex.Message?.ToLowerInvariant() ?? string.Empty;
            if (msg.Contains("connection") && (msg.Contains("reset") || msg.Contains("refused") || msg.Contains("closed") || msg.Contains("broken pipe") || msg.Contains("timed out")))
                return true;

            return false;
        }

        public uint GetCipheredRemoteIPvalue(ProtoFireConnection connection)
        {
            byte[] byteip = ((IPEndPoint)connection.Socket.RemoteEndPoint!).Address.GetAddressBytes();
            if (BitConverter.IsLittleEndian)
                Array.Reverse(byteip);
            // Prevents leaking of IP Addresses.
            return BitConverter.ToUInt32(byteip, 0) ^ ushort.MaxValue ^ AddressEncryptionKey;
        }

        IBlazePacket DecodeMitmPacket(ProtoFirePacket packet)
        {
            FireFrame frame = packet.Frame;
            IBlazeServerComponent? component = Configuration.GetComponent(frame.Component);
            if (component == null)
                return packet.Decode(typeof(NullStruct), Configuration.Decoder);

            Type? type;

            switch (frame.MsgType)
            {
                case FireFrame.MessageType.MESSAGE:
                    type = component.GetCommandRequestType(frame.Command);
                    break;
                case FireFrame.MessageType.REPLY:
                    type = component.GetCommandResponseType(frame.Command);
                    break;
                case FireFrame.MessageType.NOTIFICATION:
                    type = component.GetNotificationType(frame.Command);
                    break;
                case FireFrame.MessageType.ERROR_REPLY:
                    type = component.GetCommandErrorResponseType(frame.Command);
                    break;
                default:
                    type = typeof(NullStruct);
                    break;
            }

            type ??= typeof(NullStruct);
            return packet.Decode(type, Configuration.Decoder);
        }

        public static byte[] ReadContentSSL(SslStream sslStream)
        {
            const int bufferSize = 0x10000;
            int bytesRead;
            byte[] buff = new byte[bufferSize];

            using (MemoryStream res = new MemoryStream())
            {
                try
                {
                    sslStream.ReadTimeout = ReadTimeout;
                    while ((bytesRead = sslStream.Read(buff, 0, bufferSize)) > 0)
                    {
                        res.Write(buff, 0, bytesRead);
                        if (CheckIfStreamComplete(res))
                            break;
                    }
                    sslStream.Flush();
                }
                catch (Exception e)
                {
                    LoggerAccessor.LogDebug("[MitmProtoFireServer] - ReadContentSSL | " + e.Message);
                }

                return res.ToArray();
            }
        }

        public static byte[] ReadContent(Stream? stream)
        {
            if (stream == null)
                throw new InvalidOperationException("Stream is not set");

            int bytesRead;
            byte[] buff = new byte[0x10000];

            try
            {
                stream.ReadTimeout = ReadTimeout;
                using (MemoryStream res = new MemoryStream())
                {
                    while ((bytesRead = stream.Read(buff, 0, 0x10000)) > 0)
                    {
                        res.Write(buff, 0, bytesRead);
                        stream.Flush();
                        if (CheckIfStreamComplete(res))
                            break;
                    }
                    return res.ToArray();
                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogDebug("[MitmProtoFireServer] - ReadContent | " + e.Message);
            }

            return Array.Empty<byte>();
        }

        public async Task<ProtoFirePacket?> ReadPacketBytes(byte[] packet)
        {
            using (MemoryStream ms = new MemoryStream(packet))
            {
                try
                {
                    FireFrame frame = new FireFrame();
                    if (!await ms.ReadAllAsync(frame.Frame, 0, FireFrame.MIN_HEADER_SIZE).ConfigureAwait(false))
                        return null;

                    ushort extraFrameBytesNeeded = frame.ExtraHeaderSize;
                    if (!await ms.ReadAllAsync(frame.Frame, FireFrame.MIN_HEADER_SIZE, extraFrameBytesNeeded).ConfigureAwait(false))
                        return null;

                    byte[] data = new byte[frame.Size];
                    if (!await ms.ReadAllAsync(data, 0, data.Length).ConfigureAwait(false))
                        return null;

                    return new ProtoFirePacket(frame, data);
                }
                catch
                {
                }
            }

            return null;
        }

        public static bool CheckIfStreamComplete(MemoryStream m)
        {
            m.Seek(0, 0);
            byte t = (byte)m.ReadByte();
            if (t == 0x17)
                m.Seek(5, 0);
            else
                m.Seek(0, 0);
            long len = 0;
            while (m.Position + len < m.Length)
            {
                m.Seek(m.Position + len, 0);
                MitmBlazePacket p = ReadBlazePacketHeader(m);
                len = p.Length + (p.extLength << 16);
                if (m.Position + len == m.Length)
                    return true;
            }
            m.Seek(m.Length, 0);
            return false;
        }

        public static MitmBlazePacket ReadBlazePacketHeader(Stream s)
        {
            MitmBlazePacket res = new MitmBlazePacket
            {
                Length = ReadUShort(s),
                Component = ReadUShort(s),
                Command = ReadUShort(s),
                Error = ReadUShort(s),
                QType = ReadUShort(s),
                ID = ReadUShort(s)
            };
            if ((res.QType & 0x10) != 0)
                res.extLength = ReadUShort(s);
            else
                res.extLength = 0;
            int len = res.Length + (res.extLength << 16);
            res.Content = new byte[len];
            return res;
        }

        public static ushort ReadUShort(Stream s)
        {
            byte[] buff = new byte[2];
            s.Read(buff, 0, 2);
            return (ushort)((buff[0] << 8) + buff[1]);
        }

        public static bool ValidateAlways(object? sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
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

                SslSocket.BeginAuthenticateAsServer(connection.Socket, Certificate, ForceSsl, true, AuthenticateAsServerCallback, connection);
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
        public abstract Task OnProtoFireDisconnectAsync(ProtoFireConnection connection);
        public abstract Task OnProtoFireErrorAsync(ProtoFireConnection connection, Exception exception);
    }
}
