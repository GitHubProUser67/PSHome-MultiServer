using CustomLogger;
using Horizon.RT.Common;
using Horizon.RT.Models;
using Horizon.LIBRARY.Common;
using System.Collections.Concurrent;
using System.Net;
using static Horizon.LIBRARY.libAntiCheat.Models.Game;
using System.Collections.Generic;
using System;
using System.Linq;
using MultiServerLibrary.Extension;

namespace Horizon.LIBRARY.libAntiCheat.Models
{
    public class ClientObject
    {
        protected static Random RNG = new Random();
        public IPAddress IP { get; protected set; } = IPAddress.Any;

        public List<GameClient> Clients = new List<GameClient>();

        public bool PassedAntiCheat = false;

        /// <summary>
        /// 
        /// </summary>
        //public MediusPlayerStatus PlayerStatus => GetStatus();

        /// <summary>
        /// 
        /// </summary>
        public MGCL_GAME_HOST_TYPE ServerType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MediusConnectionType ConnectionType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int AccountId { get; protected set; } = -1;

        /// <summary>
        /// 
        /// </summary>
        public string AccountName { get; protected set; } = null;

        /// <summary>
        /// 
        /// </summary>
        public string AccountStats { get; protected set; } = null;

        /// <summary>
        /// 
        /// </summary>
        public string AccountDisplayName { get; set; } = null;

        /// <summary>
        /// Current access token required to access the account.
        /// </summary>
        public string Token { get; protected set; } = null;

        /// <summary>
        /// 
        /// </summary>
        public string SessionKey { get; protected set; } = null;

        /// <summary>
        /// MGCL Session Key
        /// </summary>
        public string MGCLSessionKey { get; protected set; } = null;

        /// <summary>
        /// Unique MGCL hardcoded game identifer per Medius title
        /// </summary>
        public int ApplicationId { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public int LocationId { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public int MediusVersion { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public int? ClanId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? WorldId { get; set; } = null;

        /// <summary>
        /// 
        /// </summary>
        public int? SignalId { get; set; } = null;

        /// <summary>
        /// 
        /// </summary>
        public Channel CurrentChannel { get; protected set; } = null;

        /// <summary>
        /// 
        /// </summary>
        public Game CurrentGame { get; protected set; } = null;

        /// <summary>
        /// 
        /// </summary>
        public int? DmeClientId { get; protected set; } = null;

        /// <summary>
        /// 
        /// </summary>
        public ConcurrentQueue<BaseScertMessage> SendMessageQueue { get; } = new ConcurrentQueue<BaseScertMessage>();

        /// <summary>
        /// 
        /// </summary>
        public DateTime UtcLastServerEchoSent { get; protected set; } = DateTimeUtils.GetHighPrecisionUtcTime();

        /// <summary>
        /// 
        /// </summary>
        public DateTime UtcLastServerEchoReply { get; protected set; } = DateTimeUtils.GetHighPrecisionUtcTime();

        /// <summary>
        /// 
        /// </summary>
        public string Metadata { get; set; } = null;

        /// <summary>
        /// RTT (ms)
        /// </summary>
        public uint LatencyMs { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> FriendsListPS3 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<int, string> FriendsList { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int[] Stats { get; set; } = new int[15];

        /// <summary>
        /// 
        /// </summary>
        public int[] WideStats { get; set; } = new int[100];

        /// <summary>
        /// 
        /// </summary>
        public int[] CustomWideStats { get; set; } = new int[0];

        /*
        public virtual bool IsLoggedIn => !_logoutTime.HasValue && _loginTime.HasValue && IsConnected;
        public bool IsInGame => CurrentGame != null && CurrentChannel != null && CurrentChannel.Type == ChannelType.Game;
        //public bool 

        public virtual bool Timedout => UtcLastServerEchoReply < UtcLastServerEchoSent && (Utils.GetHighPrecisionUtcTime() - UtcLastServerEchoReply).TotalSeconds > Program.GetAppSettingsOrDefault(ApplicationId).ClientTimeoutSeconds;
        public virtual bool IsConnected => KeepAlive || (_hasSocket && _hasActiveSession && !Timedout);  //(KeepAlive || _hasActiveSession) && !Timedout;

        public bool KeepAlive => _keepAliveTime.HasValue && (Utils.GetHighPrecisionUtcTime() - _keepAliveTime).Value.TotalSeconds < Program.GetAppSettingsOrDefault(ApplicationId).KeepAliveGracePeriodSeconds;

        */

        /// <summary>
        /// 
        /// </summary>
        protected DateTime? _loginTime = null;

        /// <summary>
        /// 
        /// </summary>
        protected DateTime? _logoutTime = null;

        /// <summary>
        /// 
        /// </summary>
        protected bool _hasActiveSession = true;

        /// <summary>
        /// 
        /// </summary>
        private uint _gameListFilterIdCounter = 0;

        /// <summary>
        /// 
        /// </summary>
        private bool _hasSocket = false;

        /// <summary>
        /// 
        /// </summary>
        protected DateTime? _keepAliveTime = null;

        /// <summary>
        /// 
        /// </summary>
#if NETCOREAPP2_1_OR_GREATER
        private DateTime _lastServerEchoValue = DateTime.UnixEpoch;
#else
        private DateTime _lastServerEchoValue = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
#endif
        /// <summary>
        /// File being Uploaded
        /// </summary>
        public MediusFile mediusFileToUpload;

        public ClientObject()
        {
            // Generate new session key
            SessionKey = AntiCheat.GenerateSessionKey();

            // Generate new token
            byte[] tokenBuf = new byte[12];
            RNG.NextBytes(tokenBuf);
            Token = Convert.ToBase64String(tokenBuf);
        }

        public void QueueServerEcho()
        {
            SendMessageQueue.Enqueue(new RT_MSG_SERVER_ECHO());
            UtcLastServerEchoSent = DateTimeUtils.GetHighPrecisionUtcTime();
        }

        public void OnRecvServerEcho(RT_MSG_SERVER_ECHO echo)
        {
            var echoTime = echo.UnixTimestamp.ToUtcDateTime();
            if (echoTime > _lastServerEchoValue)
            {
                _lastServerEchoValue = echoTime;
                UtcLastServerEchoReply = DateTimeUtils.GetHighPrecisionUtcTime();
                LatencyMs = (uint)(UtcLastServerEchoReply - echoTime).TotalMilliseconds;
            }
        }

        public void OnRecvClientEcho(RT_MSG_CLIENT_ECHO echo)
        {
            // older medius doesn't use server echo
            // so instead we'll increment our timeout dates by the client echo
            if (MediusVersion <= 108)
            {
                UtcLastServerEchoSent = DateTimeUtils.GetHighPrecisionUtcTime();
                UtcLastServerEchoReply = DateTimeUtils.GetHighPrecisionUtcTime();
            }
        }

        #region Connection / Disconnection

        public void KeepAliveUntilNextConnection()
        {
            _keepAliveTime = DateTimeUtils.GetHighPrecisionUtcTime();
        }

        public void OnConnected()
        {
            _keepAliveTime = null;
            _hasSocket = true;
        }

        public void OnDisconnected()
        {
            _hasSocket = false;
        }

        #endregion

        #region Status

        /*
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private MediusPlayerStatus GetStatus()
        {
            if (!IsConnected || !IsLoggedIn)
                return MediusPlayerStatus.MediusPlayerDisconnected;

            if (IsInGame)
                return MediusPlayerStatus.MediusPlayerInGameWorld;


            return MediusPlayerStatus.MediusPlayerInChatWorld;

            /* // Needs proper handling between Universes for MUIS
            if (IsInOtherUniverse)
                return MediusPlayerStatus.MediusPlayerInOtherUniverse;
            

        }
        */
        #endregion

        #region Session

        /// <summary>
        /// Begin DME Session
        /// </summary>
        public void BeginSession()
        {
            _hasActiveSession = true;
        }

        /// <summary>
        /// End DME Session
        /// </summary>
        public void EndSession()
        {
            _hasActiveSession = false;
        }

        #endregion

        #region Send Queue

        public void Queue(BaseScertMessage message)
        {
            SendMessageQueue.Enqueue(message);
        }

        public void Queue(IEnumerable<BaseScertMessage> messages)
        {
            foreach (var message in messages)
                Queue(message);
        }

        public void Queue(BaseMediusMessage message)
        {
            Queue(new RT_MSG_SERVER_APP() { Message = message });
        }

        public void Queue(BaseMediusPluginMessage message)
        {
            Queue(new RT_MSG_SERVER_PLUGIN_TO_APP() { Message = message });
        }

        public void Queue(IEnumerable<BaseMediusMessage> messages)
        {
            Queue(messages.Select(x => new RT_MSG_SERVER_APP() { Message = x }));
        }

        #endregion

        #region SetIP
        public void SetIp(string ip)
        {
            switch (Uri.CheckHostName(ip))
            {
                case UriHostNameType.IPv4:
                    {
                        IP = IPAddress.Parse(ip).MapToIPv4() ?? IPAddress.Any;
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
            return $"({AccountId}:{AccountName})";
        }
    }
}
