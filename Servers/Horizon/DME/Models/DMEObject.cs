using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using CustomLogger;
using DotNetty.Transport.Channels;
using Horizon.LIBRARY.Pipeline.Udp;
using Horizon.MUM.Models;
using Horizon.RT.Common;
using Horizon.RT.Cryptography;
using Horizon.RT.Models;
using MultiServerLibrary.Extension;
using Org.BouncyCastle.Utilities.Encoders;

namespace Horizon.DME.Models
{
    public class DMEObject
    {
        protected static Random RNG = new();

        public IPAddress IP { get; protected set; } = IPAddress.Any;

        /// <summary>
        ///
        /// </summary>
        public UDPClientServer? Udp { get; protected set; } = null;

        public ClientObject mumClient { get; protected set; }

        /// <summary>
        ///
        /// </summary>
        public int UdpPort => Udp?.Port ?? -1;

        /// <summary>
        ///
        /// </summary>
        public IChannel? Tcp { get; protected set; } = null;

        /// <summary>
        ///
        /// </summary>
        public int DmeId { get; protected set; } = 0;

        /// <summary>
        ///
        /// </summary>
        public World? DmeWorld { get; protected set; } = null;

        /// <summary>
        /// Current access token required to access the account.
        /// </summary>
        public string? Token { get; protected set; } = null;

        /// <summary>
        ///
        /// </summary>
        public string? SessionKey { get; protected set; } = null;

        /// <summary>
        /// Used for encrypting UDP packets. This is generated during the connection process.
        /// </summary>
        public CipherService? CryptoContext { get; set; } = null;

        /// <summary>
        ///
        /// </summary>
        public int ApplicationId { get; set; } = 0;

        /// <summary>
        ///
        /// </summary>
        public int? MediusVersion { get; set; } = 0;

        /// <summary>
        ///
        /// </summary>
        public uint ScertId { get; set; } = 0;

        /// <summary>
        ///
        /// </summary>
        public RT_RECV_FLAG RecvFlag { get; set; } =
            RT_RECV_FLAG.RECV_SINGLE | RT_RECV_FLAG.RECV_LIST;

        /// <summary>
        ///
        /// </summary>
        public ConcurrentQueue<BaseScertMessage> TcpSendMessageQueue { get; } =
            new ConcurrentQueue<BaseScertMessage>();

        /// <summary>
        ///
        /// </summary>
        public ConcurrentQueue<ScertDatagramPacket> UdpSendMessageQueue { get; } =
            new ConcurrentQueue<ScertDatagramPacket>();

        /// <summary>
        ///
        /// </summary>
        public DateTime UtcLastServerEchoSent { get; set; } =
            DateTimeUtils.GetHighPrecisionUtcTime();

        /// <summary>
        ///
        /// </summary>
        public DateTime UtcLastMessageReceived { get; protected set; } =
            DateTimeUtils.GetHighPrecisionUtcTime();

        /// <summary>
        /// RTT (ms)
        /// </summary>
        public uint LatencyMs { get; protected set; }

        /// <summary>
        ///
        /// </summary>
        public DateTime TimeCreated { get; protected set; } =
            DateTimeUtils.GetHighPrecisionUtcTime();

        /// <summary>
        ///
        /// </summary>
        public DateTime? TimeAuthenticated { get; protected set; } = null;

        /// <summary>
        ///
        /// </summary>
        public bool Disconnected { get; protected set; } = false;

        /// <summary>
        ///
        /// </summary>
        public IPEndPoint? RemoteUdpEndpoint { get; set; } = null;

        /// <summary>
        ///
        /// </summary>
        public int AggTimeMs { get; set; } = 20;

        /// <summary>
        ///
        /// </summary>
        long? LastAggTime { get; set; } = null;

        /// <summary>
        ///
        /// </summary>
        public bool HasJoined { get; set; } = false;

        public virtual bool IsConnectingGracePeriod =>
            !TimeAuthenticated.HasValue
            && (DateTimeUtils.GetHighPrecisionUtcTime() - TimeCreated).TotalSeconds
                < DATABASE
                    .DatabaseManager.GetAppSettingsOrDefault(ApplicationId)
                    .ClientTimeoutSeconds;
        public virtual bool Timedout
        {
            get
            {
                if (IsConnectingGracePeriod)
                    _timedout = false;
                else
                {
                    const int expirationDelay = 2;
                    var deltaSec = (
                        DateTimeUtils.GetHighPrecisionUtcTime() - UtcLastMessageReceived
                    ).TotalSeconds;
                    var timeoutThreshold = DATABASE
                        .DatabaseManager.GetAppSettingsOrDefault(ApplicationId)
                        .ClientTimeoutSeconds;

                    if (deltaSec > timeoutThreshold + expirationDelay)
                    {
                        if (!_timedout)
                        {
                            _missedEchos++;
                            LoggerAccessor.LogWarn(
                                $"[DMEObject] - TIMEOUT - Client {mumClient.AccountName} missed echo #{_missedEchos}. Delta={deltaSec:0.000}s, Threshold={timeoutThreshold}s"
                            );

                            if (_missedEchos > expirationDelay)
                            {
                                _missedEchos = 0;
                                _timedout = true;
                            }
                        }
                    }
                    else
                    {
                        if (_timedout)
                        {
                            _timedout = false;

                            LoggerAccessor.LogInfo(
                                $"[DMEObject] - RECOVERED - Client {mumClient.AccountName} recovered. Delta={deltaSec:0.000}s, Threshold={timeoutThreshold}s"
                            );
                        }

                        _missedEchos = 0;
                    }
                }

                return _timedout;
            }
        }
        public virtual bool LongTimedout
        {
            get
            {
                const int expirationDelay = 2;
                var deltaSec = (
                    DateTimeUtils.GetHighPrecisionUtcTime() - UtcLastMessageReceived
                ).TotalSeconds;
                var timeoutThreshold = DATABASE
                    .DatabaseManager.GetAppSettingsOrDefault(ApplicationId)
                    .ClientLongTimeoutSeconds;

                if (deltaSec > timeoutThreshold + expirationDelay)
                {
                    if (!_long_timedout)
                    {
                        _missedLongEchos++;
                        LoggerAccessor.LogWarn(
                            $"[DMEObject] - LONG_TIMEOUT - Client {mumClient.AccountName} missed echo #{_missedLongEchos}. Delta={deltaSec:0.000}s, Threshold={timeoutThreshold}s"
                        );

                        if (_missedLongEchos > expirationDelay)
                        {
                            _missedLongEchos = 0;
                            _long_timedout = true;
                        }
                    }
                }
                else
                {
                    if (_long_timedout)
                    {
                        _long_timedout = false;

                        LoggerAccessor.LogInfo(
                            $"[DMEObject] - LONG_RECOVERED - Client {mumClient.AccountName} recovered. Delta={deltaSec:0.000}s, Threshold={timeoutThreshold}s"
                        );
                    }

                    _missedLongEchos = 0;
                }

                return _long_timedout;
            }
        }
        public virtual bool IsConnected =>
            !Disconnected && Tcp != null && Tcp.Active && mumClient.IsInGame && !LongTimedout;
        public virtual bool IsAuthenticated => TimeAuthenticated.HasValue;
        public virtual bool Destroy => Disconnected || (!IsConnected && !IsConnectingGracePeriod);
        public virtual bool IsDestroyed { get; protected set; } = false;
        public virtual bool IsAggTime =>
            !LastAggTime.HasValue
            || (DateTimeUtils.GetMillisecondsSinceStartup() - LastAggTime.Value) >= AggTimeMs;

        /// <summary>
        ///
        /// </summary>
        protected int _missedEchos;

        /// <summary>
        ///
        /// </summary>
        protected int _missedLongEchos;

        /// <summary>
        ///
        /// </summary>
        protected bool _timedout;

        /// <summary>
        ///
        /// </summary>
        protected bool _long_timedout;

        public Action<DMEObject>? OnDestroyed;

        private DateTime _lastServerEchoValue = DateTime.UnixEpoch;
        private DateTime? _lastForceDisconnect = null;
        private int _isStopping = 0;

        public DMEObject(string sessionKey, World dmeWorld, int dmeId, ClientObject mumClient)
        {
            SessionKey = sessionKey;

            DmeId = dmeId;
            DmeWorld = dmeWorld;
            AggTimeMs = DATABASE
                .DatabaseManager.GetAppSettingsOrDefault(ApplicationId)
                .DefaultClientWorldAggTime;

            // Generate new token
            var tokenBuf = new byte[12];
            RNG.NextBytes(tokenBuf);
            Token = Base64.ToBase64String(tokenBuf);

            this.mumClient = mumClient;

            UtcLastMessageReceived = UtcLastServerEchoSent =
                DateTimeUtils.GetHighPrecisionUtcTime();
        }

        public DMEObject(string sessionKey, ClientObject mumClient)
        {
            SessionKey = sessionKey;

            AggTimeMs = DATABASE
                .DatabaseManager.GetAppSettingsOrDefault(ApplicationId)
                .DefaultClientWorldAggTime;

            // Generate new token
            var tokenBuf = new byte[12];
            RNG.NextBytes(tokenBuf);
            Token = Base64.ToBase64String(tokenBuf);

            this.mumClient = mumClient;

            UtcLastMessageReceived = UtcLastServerEchoSent =
                DateTimeUtils.GetHighPrecisionUtcTime();
        }

        public void BeginUdp(CipherService? cipher)
        {
            if (Udp != null)
                return;

            Udp = new UDPClientServer(this, cipher);
            _ = Udp.Start();
        }

        public void QueueServerEcho()
        {
            TcpSendMessageQueue.Enqueue(new RT_MSG_SERVER_ECHO());
            UtcLastServerEchoSent = DateTimeUtils.GetHighPrecisionUtcTime();
        }

        public void OnRecvServerEcho(RT_MSG_SERVER_ECHO echo)
        {
            var echoTime = echo.UnixTimestamp.ToUtcDateTime();
            var latencyMs = (DateTimeUtils.GetHighPrecisionUtcTime() - echoTime).TotalMilliseconds;

            if (latencyMs >= 0 && latencyMs < 10000)
                LatencyMs = (uint)latencyMs;

            _lastServerEchoValue = echoTime;
        }

        public void OnRecvClientEcho(RT_MSG_CLIENT_ECHO echo)
        {
            // older medius doesn't use server echo
            // so instead we'll increment our timeout dates by the client echo
            if (MediusVersion <= 108)
                // reply must be before sent for the timeout to work
                UtcLastServerEchoSent = DateTimeUtils.GetHighPrecisionUtcTime().AddSeconds(1);
        }

        public virtual void OnRecv(BaseScertMessage msg)
        {
            UtcLastMessageReceived = DateTimeUtils.GetHighPrecisionUtcTime();
        }

        public virtual void OnRecv(ScertDatagramPacket msg)
        {
            UtcLastMessageReceived = DateTimeUtils.GetHighPrecisionUtcTime();
        }

        public Task HandleIncomingMessages()
        {
            // udp
            return Udp != null ? Udp.HandleIncomingMessages() : Task.CompletedTask;
        }

        public void HandleOutgoingMessages()
        {
            var responses = new List<BaseScertMessage>();

            // set aggtime to locked intervals of whatever is stored in AggTimeMs
            // sometimes this server will be +- a few milliseconds on an agg and
            // we don't want that to change when messages get sent
            LastAggTime = DateTimeUtils.GetMillisecondsSinceStartup();

            // tcp
            if (Tcp != null)
            {
                while (TcpSendMessageQueue.TryDequeue(out var message))
                    responses.Add(message);

                // send
                if (responses.Count > 0)
                    _ = Tcp.WriteAndFlushAsync(responses);
            }

            // udp
            Udp?.HandleOutgoingMessages();
        }

        #region Connection / Disconnection

        public async Task Stop()
        {
            if (IsDestroyed || Interlocked.Exchange(ref _isStopping, 1) == 1)
                return;

            var udp = Udp;
            var tcp = Tcp;

            // Mark destroyed and detach channels early to prevent re-entrant close races.
            Udp = null;
            Tcp = null;
            IsDestroyed = true;

            try
            {
                if (udp != null)
                    await udp.Stop().ConfigureAwait(false);

                if (tcp != null)
                {
                    var closeTask = tcp.CloseAsync();
                    if (
                        !await closeTask
                            .TryAwait(TimeSpan.FromMilliseconds(2000))
                            .ConfigureAwait(false)
                    )
                        LoggerAccessor.LogWarn(
                            $"[DMEObject] - Timed out waiting for TCP close for client {this}"
                        );
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(
                    $"[DMEObject] - Client connection stop thrown an assertion. (Exception:{ex})"
                );
            }
            finally
            {
                OnDestroyed?.Invoke(this);
            }
        }

        public void OnTcpConnected(IChannel channel)
        {
            Tcp = channel;
        }

        public void OnTcpDisconnected()
        {
            Disconnected = true;
        }

        public static void OnUdpConnected() { }

        public void OnConnectionCompleted()
        {
            TimeAuthenticated = DateTimeUtils.GetHighPrecisionUtcTime();
        }

        public void ForceDisconnect()
        {
            var now = DateTimeUtils.GetHighPrecisionUtcTime();
            if ((now - _lastForceDisconnect)?.TotalSeconds < 5)
                return;

            LoggerAccessor.LogWarn($"[DMEObject] - Force disconnecting client {this}");
            TcpSendMessageQueue.Enqueue(new RT_MSG_CLIENT_DISCONNECT_WITH_REASON() { Reason = 0 });
            _lastForceDisconnect = now;
        }

        #endregion

        #region Send Queue

        public void EnqueueTcp(BaseScertMessage message)
        {
            TcpSendMessageQueue.Enqueue(message);
        }

        public void EnqueueTcp(IEnumerable<BaseScertMessage> messages)
        {
            foreach (var message in messages)
                EnqueueTcp(message);
        }

        public void EnqueueUdp(BaseScertMessage message)
        {
            Udp?.Send(message);
        }

        public void EnqueueUdp(IEnumerable<BaseScertMessage> messages)
        {
            foreach (var message in messages)
                EnqueueUdp(message);
        }

        #endregion

        public bool HasRecvFlag(RT_RECV_FLAG flag)
        {
            return MediusVersion <= 108 || RecvFlag.HasFlag(flag);
        }

        #region SetIP
        public void SetIp(string ip)
        {
            if (string.IsNullOrEmpty(ip))
                return;

            switch (Uri.CheckHostName(ip))
            {
                case UriHostNameType.IPv4:
                {
                    IP = IPAddress.Parse(ip);
                    break;
                }
                case UriHostNameType.IPv6:
                {
                    IP = IPAddress.Parse(ip).MapToIPv4();
                    break;
                }
                case UriHostNameType.Dns:
                {
                    IP = Dns.GetHostAddresses(ip).FirstOrDefault()?.MapToIPv4() ?? IPAddress.Any;
                    break;
                }
                default:
                {
                    LoggerAccessor.LogError(
                        $"Unhandled UriHostNameType {Uri.CheckHostName(ip)} from {ip} in DMEObject.SetIp()"
                    );
                    break;
                }
            }
        }
        #endregion

        public override string ToString()
        {
            return $"(worldId: {DmeWorld?.WorldId}, clientId: {DmeId})";
        }
    }
}
