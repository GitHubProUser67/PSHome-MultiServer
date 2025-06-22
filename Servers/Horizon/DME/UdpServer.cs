using CustomLogger;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Horizon.RT.Common;
using Horizon.RT.Models;
using Horizon.LIBRARY.Pipeline.Udp;
using Horizon.DME.Models;
using System.Collections.Concurrent;
using System.Net;
using Horizon.DME.PluginArgs;
using Horizon.LIBRARY.Pipeline.Attribute;
using Horizon.PluginManager;
using Horizon.RT.Cryptography;

namespace Horizon.DME
{
    public class UdpServer
    {
        public int Port { get; protected set; } = -1;

        protected IEventLoopGroup? _workerGroup = null;
        protected IChannel? _boundChannel = null;
        protected ScertDatagramHandler? _scertHandler = null;
        protected CipherService? _cipher = null;

        protected DMEObject? ClientObject { get; set; } = null;
        protected EndPoint? AuthenticatedEndPoint { get; set; } = null;

        private ConcurrentQueue<ScertDatagramPacket> _recvQueue = new();
        private ConcurrentQueue<ScertDatagramPacket> _sendQueue = new();

        private BaseScertMessage? _lastMessage { get; set; } = null;

        #region Port Management

        private static ConcurrentDictionary<int, UdpServer> _portToServer = new ConcurrentDictionary<int, UdpServer>();
        private void RegisterPort()
        {
            int i = DmeClass.Settings.UDPPort;
            while (_portToServer.ContainsKey(i))
                ++i;

            if (_portToServer.TryAdd(i, this))
                Port = i;
        }

        private void FreePort()
        {
            if (Port < 0)
                return;

            _portToServer.TryRemove(Port, out _);
        }

        #endregion

        public UdpServer(DMEObject clientObject, CipherService? cipher)
        {
            _cipher = cipher;
            ClientObject = clientObject;
            RegisterPort();
        }

        /// <summary>
        /// Start the Dme Udp Client Server.
        /// </summary>
        public virtual async Task Start()
        {
            _workerGroup = new MultithreadEventLoopGroup();
            _scertHandler = new ScertDatagramHandler
            {
                OnChannelActive = channel =>
                {
                    // get scert client
                    if (!channel.HasAttribute(LIBRARY.Pipeline.Constants.SCERT_CLIENT))
                        channel.GetAttribute(LIBRARY.Pipeline.Constants.SCERT_CLIENT).Set(new ScertClientAttribute());
                    var scertClient = channel.GetAttribute(LIBRARY.Pipeline.Constants.SCERT_CLIENT).Get();
                    scertClient.CipherService = _cipher;
                    // pass medius version
                    scertClient.MediusVersion = ClientObject?.MediusVersion;
                }
            };

            // Queue all incoming messages
            _scertHandler.OnChannelMessage += async (channel, message) =>
            {
                OnUdpMsg pluginArgs = new()
                {
                    Player = ClientObject,
                    Packet = message
                };

                // Plugin
                await DmeClass.Plugins.OnEvent(PluginEvent.DME_GAME_ON_RECV_UDP, pluginArgs);

                if (!pluginArgs.Ignore)
                    _recvQueue.Enqueue(message);

                ClientObject?.OnRecv(message);

                // Log if id is set
                if (message.CanLog())
                    LoggerAccessor.LogInfo($"DME_UDP RECV {channel}: {message}");
            };

            var bootstrap = new Bootstrap();
            bootstrap
                .Group(_workerGroup)
                .Channel<SocketDatagramChannel>()
                .Handler(new LoggingHandler(LogLevel.INFO))
                .Handler(new ActionChannelInitializer<IChannel>(channel =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;

                    pipeline.AddLast(new ScertDatagramEncoder(Constants.MEDIUS_UDP_MESSAGE_MAXLEN));
                    pipeline.AddLast(new ScertDatagramIEnumerableEncoder(Constants.MEDIUS_UDP_MESSAGE_MAXLEN));
                    pipeline.AddLast(new ScertDatagramDecoder());
                    pipeline.AddLast(new ScertDatagramMultiAppDecoder());
                    pipeline.AddLast(_scertHandler);
                }));

            _boundChannel = await bootstrap.BindAsync(Port);
        }

        /// <summary>
        /// Stop the server.
        /// </summary>
        public virtual async Task Stop()
        {
            try
            {
                if (_boundChannel != null)
                {
                    await _boundChannel.CloseAsync();
                    _boundChannel = null;
                }
            }
            finally
            {
                if (_workerGroup != null)
                    await Task.WhenAll(
                        _workerGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)));

                FreePort();
            }
        }

        #region Message Processing

        protected void ProcessMessage(ScertDatagramPacket packet)
        {
            var message = packet.Message;

            switch (message)
            {
                case RT_MSG_CLIENT_CONNECT_AUX_UDP connectAuxUdp:
                    {
                        var clientObject = DmeClass.TcpServer.GetClientByScertId(connectAuxUdp.ScertId);
                        if (clientObject != ClientObject && ClientObject?.DmeId != connectAuxUdp.PlayerId)
                            break;

                        AuthenticatedEndPoint = packet.Source;

                        if (ClientObject != null)
                        {
                            ClientObject.RemoteUdpEndpoint = AuthenticatedEndPoint as IPEndPoint;
                            ClientObject.OnUdpConnected();

                            RT_MSG_SERVER_CONNECT_ACCEPT_AUX_UDP msg = new()
                            {
                                PlayerId = (ushort)ClientObject.DmeId,
                                ScertId = ClientObject.ScertId,
                                PlayerCount = (ushort?)ClientObject.DmeWorld?.Clients.Length ?? 0x0001,
                                EndPoint = ClientObject.RemoteUdpEndpoint
                            };

                            // Send it twice in case of packet loss
                            //_boundChannel.WriteAndFlushAsync(new ScertDatagramPacket(msg, packet.Source));
                            _boundChannel?.WriteAndFlushAsync(new ScertDatagramPacket(msg, packet.Source));
                        }
                        break;
                    }
                case RT_MSG_CLIENT_CONNECT_READY_AUX_UDP readyAuxUdp:
                    {

                        break;
                    }
                case RT_MSG_SERVER_ECHO serverEchoReply:
                    {

                        break;
                    }
                case RT_MSG_CLIENT_ECHO clientEcho:
                    {
                        SendTo(new RT_MSG_CLIENT_ECHO() { Value = clientEcho.Value }, packet.Source);
                        break;
                    }
                case RT_MSG_CLIENT_APP_BROADCAST clientAppBroadcast:
                    {
                        if (AuthenticatedEndPoint == null || !AuthenticatedEndPoint.Equals(packet.Source))
                            break;

                        ClientObject?.DmeWorld?.BroadcastUdp(ClientObject, clientAppBroadcast.Payload);
                        break;
                    }
                case RT_MSG_CLIENT_APP_LIST clientAppList:
                    {
                        if (AuthenticatedEndPoint == null || !AuthenticatedEndPoint.Equals(packet.Source))
                            break;

                        ClientObject?.DmeWorld?.SendUdpAppList(ClientObject, clientAppList.Targets, clientAppList.Payload ?? Array.Empty<byte>());
                        break;
                    }
                case RT_MSG_CLIENT_APP_SINGLE clientAppSingle:
                    {
                        if (AuthenticatedEndPoint == null || !AuthenticatedEndPoint.Equals(packet.Source))
                            break;

                        ClientObject?.DmeWorld?.SendUdpAppSingle(ClientObject, clientAppSingle.TargetOrSource, clientAppSingle.Payload ?? Array.Empty<byte>());
                        break;
                    }
                case RT_MSG_CLIENT_APP_TOSERVER clientAppToServer:
                    {
                        if (AuthenticatedEndPoint == null || !AuthenticatedEndPoint.Equals(packet.Source))
                            break;

                        if (clientAppToServer.Message != null)
                            ProcessMediusMessage(clientAppToServer.Message);
                        break;
                    }
                case RT_MSG_CLIENT_FLUSH_SINGLE clientFlushSingle:
                    {

                        break;
                    }
                case RT_MSG_CLIENT_FLUSH_ALL flushAll:
                    {

                        return;
                    }
                case RT_MSG_CLIENT_DISCONNECT _:
                case RT_MSG_CLIENT_DISCONNECT_WITH_REASON _:
                    {

                        break;
                    }
                default:
                    {
                        LoggerAccessor.LogWarn($"UNHANDLED MESSAGE: {message}");

                        break;
                    }
            }

            return;
        }

        protected virtual void ProcessMediusMessage(BaseMediusMessage message)
        {
            if (message == null)
                return;
        }

        #endregion

        #region Send

        private void SendTo(BaseScertMessage message, EndPoint target)
        {
            if (target == null)
                return;

            _sendQueue.Enqueue(new ScertDatagramPacket(message, target));
        }

        public void Send(BaseScertMessage message)
        {
            if (AuthenticatedEndPoint == null)
                return;

            _sendQueue.Enqueue(new ScertDatagramPacket(message, AuthenticatedEndPoint));
        }

        public void Send(IEnumerable<BaseScertMessage> messages)
        {
            if (AuthenticatedEndPoint == null)
                return;

            foreach (var message in messages)
                _sendQueue.Enqueue(new ScertDatagramPacket(message, AuthenticatedEndPoint));
        }

        public Task SendImmediate(BaseScertMessage message)
        {
            if (AuthenticatedEndPoint == null || _boundChannel == null)
                return Task.CompletedTask;

            return _boundChannel.WriteAndFlushAsync(new ScertDatagramPacket(message, AuthenticatedEndPoint));
        }

        public Task SendImmediate(IEnumerable<BaseScertMessage> messages)
        {
            if (AuthenticatedEndPoint == null || _boundChannel == null)
                return Task.CompletedTask;

            return _boundChannel.WriteAndFlushAsync(messages.Select(x => new ScertDatagramPacket(x, AuthenticatedEndPoint)));
        }

        #endregion

        #region Tick

        public async Task HandleIncomingMessages()
        {
            if (_boundChannel == null || !_boundChannel.Active)
                return;

            try
            {
                // Process all messages in queue
                while (_recvQueue.TryDequeue(out var message))
                {
                    try
                    {
                        if (ClientObject != null && !await PassMessageToPlugins(_boundChannel, ClientObject, message.Message, true))
                            ProcessMessage(message);
                    }
                    catch (Exception e)
                    {
                        LoggerAccessor.LogError(e);
                    }
                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }
        }

        public async Task HandleOutgoingMessages()
        {
            if (_boundChannel == null || !_boundChannel.Active)
                return;

            List<ScertDatagramPacket> responses = new();

            try
            {
                // Send if writeable
                if (_boundChannel.IsWritable)
                {
                    // Add send queue to responses
                    while (_sendQueue.TryDequeue(out var message))
                    {
                        if (ClientObject != null && !await PassMessageToPlugins(_boundChannel, ClientObject, message.Message, false))
                            responses.Add(message);
                    }

                    if (responses.Count > 0)
                        await _boundChannel.WriteAndFlushAsync(responses);
                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }
        }

        #endregion

        protected async Task<bool> PassMessageToPlugins(IChannel clientChannel, DMEObject clientObject, BaseScertMessage message, bool isIncoming)
        {
            OnMessageArgs onMsg = new(isIncoming)
            {
                Player = clientObject,
                Channel = clientChannel,
                Message = message
            };

            // Send to plugins
            await DmeClass.Plugins.OnMessageEvent(message.Id, onMsg);
            if (onMsg.Ignore)
                return true;

            // Send medius message to plugins
            if (message is RT_MSG_CLIENT_APP_TOSERVER clientApp)
            {
                OnMediusMessageArgs onMediusMsg = new(isIncoming)
                {
                    Player = clientObject,
                    Channel = clientChannel,
                    Message = clientApp.Message
                };
                if (clientApp.Message != null)
                    await DmeClass.Plugins.OnMediusMessageEvent(clientApp.Message.PacketClass, clientApp.Message.PacketType, onMediusMsg);
                if (onMediusMsg.Ignore)
                    return true;
            }
            else if (message is RT_MSG_SERVER_APP serverApp)
            {
                OnMediusMessageArgs onMediusMsg = new(isIncoming)
                {
                    Player = clientObject,
                    Channel = clientChannel,
                    Message = serverApp.Message
                };
                if (serverApp.Message != null)
                    await DmeClass.Plugins.OnMediusMessageEvent(serverApp.Message.PacketClass, serverApp.Message.PacketType, onMediusMsg);
                if (onMediusMsg.Ignore)
                    return true;
            }

            return false;
        }
    }
}
