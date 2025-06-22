using DotNetty.Transport.Channels;
using Horizon.RT.Common;
using Horizon.RT.Models;
using Horizon.LIBRARY.Pipeline.Udp;
using System.Collections.Concurrent;
using System.Net;
using CustomLogger;
using Horizon.RT.Cryptography;
using NetworkLibrary.Extension;
using Horizon.MUM.Models;

namespace Horizon.DME.Models
{
    public class DMEObject
    {
        protected static Random RNG = new();

        public IPAddress IP { get; protected set; } = IPAddress.Any;

        /// <summary>
        /// 
        /// </summary>
        public UdpServer? Udp { get; protected set; } = null;

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
        public RT_RECV_FLAG RecvFlag { get; set; } = RT_RECV_FLAG.RECV_SINGLE | RT_RECV_FLAG.RECV_LIST;

        /// <summary>
        /// 
        /// </summary>
        public ConcurrentQueue<BaseScertMessage> TcpSendMessageQueue { get; } = new ConcurrentQueue<BaseScertMessage>();

        /// <summary>
        /// 
        /// </summary>
        public ConcurrentQueue<ScertDatagramPacket> UdpSendMessageQueue { get; } = new ConcurrentQueue<ScertDatagramPacket>();

        /// <summary>
        /// 
        /// </summary>
        public DateTime UtcLastServerEchoSent { get; set; } = DateTimeUtils.GetHighPrecisionUtcTime();

        /// <summary>
        /// 
        /// </summary>
        public DateTime UtcLastMessageReceived { get; protected set; } = DateTimeUtils.GetHighPrecisionUtcTime();

        /// <summary>
        /// RTT (ms)
        /// </summary>
        public uint LatencyMs { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime TimeCreated { get; protected set; } = DateTimeUtils.GetHighPrecisionUtcTime();

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

        public virtual bool IsConnectingGracePeriod => !TimeAuthenticated.HasValue && (DateTimeUtils.GetHighPrecisionUtcTime() - TimeCreated).TotalSeconds < DmeClass.GetAppSettingsOrDefault(ApplicationId).ClientTimeoutSeconds;
        public virtual bool Timedout => !IsConnectingGracePeriod && ((DateTimeUtils.GetHighPrecisionUtcTime() - UtcLastMessageReceived).TotalSeconds > DmeClass.GetAppSettingsOrDefault(ApplicationId).ClientTimeoutSeconds);
        public virtual bool LongTimedout => (DateTimeUtils.GetHighPrecisionUtcTime() - UtcLastMessageReceived).TotalSeconds > DmeClass.GetAppSettingsOrDefault(ApplicationId).ClientLongTimeoutSeconds;
        public virtual bool IsConnected => !Disconnected && Tcp != null && Tcp.Active && mumClient.IsInGame && !LongTimedout;
        public virtual bool IsAuthenticated => TimeAuthenticated.HasValue;
        public virtual bool Destroy => Disconnected || (!IsConnected && !IsConnectingGracePeriod);
        public virtual bool IsDestroyed { get; protected set; } = false;
        public virtual bool IsAggTime => !LastAggTime.HasValue || (DateTimeUtils.GetMillisecondsSinceStartup() - LastAggTime.Value) >= AggTimeMs;

        public Action<DMEObject>? OnDestroyed;


        private DateTime _lastServerEchoValue = DateTime.UnixEpoch;
        private DateTime? _lastForceDisconnect = null;

        public DMEObject(string sessionKey, World dmeWorld, int dmeId, ClientObject mumClient)
        {
            SessionKey = sessionKey;

            DmeId = dmeId;
            DmeWorld = dmeWorld;
            AggTimeMs = DmeClass.GetAppSettingsOrDefault(ApplicationId).DefaultClientWorldAggTime;

            // Generate new token
            byte[] tokenBuf = new byte[12];
            RNG.NextBytes(tokenBuf);
            Token = Convert.ToBase64String(tokenBuf);

            this.mumClient = mumClient;

            UtcLastMessageReceived = UtcLastServerEchoSent = DateTimeUtils.GetHighPrecisionUtcTime();
        }

        public DMEObject(string sessionKey, ClientObject mumClient)
        {
            SessionKey = sessionKey;

            AggTimeMs = DmeClass.GetAppSettingsOrDefault(ApplicationId).DefaultClientWorldAggTime;

            // Generate new token
            byte[] tokenBuf = new byte[12];
            RNG.NextBytes(tokenBuf);
            Token = Convert.ToBase64String(tokenBuf);

            this.mumClient = mumClient;

            UtcLastMessageReceived = UtcLastServerEchoSent = DateTimeUtils.GetHighPrecisionUtcTime();
        }

        public void BeginUdp(CipherService? cipher)
        {
            if (Udp != null)
                return;

            Udp = new UdpServer(this, cipher);
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
            if (echoTime > _lastServerEchoValue)
            {
                _lastServerEchoValue = echoTime;
                LatencyMs = (uint)(DateTimeUtils.GetHighPrecisionUtcTime() - echoTime).TotalMilliseconds;
            }
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
            if (Udp != null)
                return Udp.HandleIncomingMessages();

            return Task.CompletedTask;
        }

        public void HandleOutgoingMessages()
        {
            List<BaseScertMessage> responses = new List<BaseScertMessage>();

            // set aggtime to locked intervals of whatever is stored in AggTimeMs
            // sometimes this server will be +- a few milliseconds on an agg and
            // we don't want that to change when messages get sent
            //if (LastAggTime.HasValue)
            //    LastAggTime += AggTimeMs * ((Utils.GetMillisecondsSinceStartup() - LastAggTime.Value) / AggTimeMs);
            //else
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
            if (IsDestroyed)
                return;

            try
            {
                if (Udp != null)
                    await Udp.Stop();

                if (Tcp != null)
                {
                    await Tcp.CloseAsync();
                    Tcp = null;
                }
            }
            catch
            {

            }
            finally
            {
                OnDestroyed?.Invoke(this);
            }

            Tcp = null;
            Udp = null;
            IsDestroyed = true;
        }

        public void OnTcpConnected(IChannel channel)
        {
            Tcp = channel;
        }

        public void OnTcpDisconnected()
        {
            Disconnected = true;
        }

        public void OnUdpConnected()
        {

        }

        public void OnConnectionCompleted()
        {
            TimeAuthenticated = DateTimeUtils.GetHighPrecisionUtcTime();
        }

        public void ForceDisconnect()
        {
            DateTime now = DateTimeUtils.GetHighPrecisionUtcTime();
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

        public void EnqueueTcp(IEnumerable<BaseMediusMessage> messages)
        {
            EnqueueTcp(messages.Select(x => new RT_MSG_SERVER_APP() { Message = x }));
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

        public void EnqueueUdp(BaseMediusMessage message)
        {
            EnqueueUdp(new RT_MSG_SERVER_APP() { Message = message });
        }

        public void EnqueueUdp(IEnumerable<BaseMediusMessage> messages)
        {
            EnqueueUdp(messages.Select(x => new RT_MSG_SERVER_APP() { Message = x }));
        }

        #endregion

        public bool HasRecvFlag(RT_RECV_FLAG flag)
        {
            if (MediusVersion <= 108)
                return true;

            return RecvFlag.HasFlag(flag);
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
                        LoggerAccessor.LogError($"Unhandled UriHostNameType {Uri.CheckHostName(ip)} from {ip} in DMEObject.SetIp()");
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