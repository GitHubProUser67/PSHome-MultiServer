using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using CustomLogger;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Horizon.COMMON.Extensions.PSHome;
using Horizon.DME.Models;
using Horizon.DME.PluginArgs;
using Horizon.LIBRARY.Pipeline.Attribute;
using Horizon.LIBRARY.Pipeline.Udp;
using Horizon.PluginManager;
using Horizon.RT.Common;
using Horizon.RT.Cryptography;
using Horizon.RT.Models;
using MultiServerLibrary.Extension;

namespace Horizon.DME
{
    public class UDPClientServer
    {
        public int Port { get; protected set; } = -1;

        protected IEventLoopGroup? _group = null;
        protected IChannel? _boundChannel = null;
        protected ScertDatagramHandler? _scertHandler = null;
        protected CipherService? _cipher = null;

        protected DMEObject? ClientObject { get; set; } = null;
        protected EndPoint? AuthenticatedEndPoint { get; set; } = null;

        private readonly ConcurrentQueue<ScertDatagramPacket> _recvQueue = new();
        private readonly ConcurrentQueue<ScertDatagramPacket> _sendQueue = new();

        #region Port Management

        private static readonly ConcurrentDictionary<ushort, UDPClientServer> _portToServer = new();

        private void RegisterPort()
        {
            var i = HorizonServerConfiguration.DMEUDPPort;

            // Optimization, Windows allows us to spin around free ports.
            if (MultiServerLibrary.Extension.Windows.Win32API.IsWindows)
            {
                var port = TcpUdpUtils.GetNextVacantUDPPort(i);

                if (port == -1)
                    return; // Keep port -1 value (no more ports available).

                i = (ushort)port;
            }

            while (_portToServer.ContainsKey(i))
                ++i;

            if (_portToServer.TryAdd(i, this))
                Port = i;
        }

        private void FreePort()
        {
            if (Port < 0)
                return;

            _portToServer.TryRemove((ushort)Port, out _);
        }

        #endregion

        public UDPClientServer(DMEObject clientObject, CipherService? cipher)
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
            _group = new MultithreadEventLoopGroup();
            _scertHandler = new ScertDatagramHandler
            {
                OnChannelActive = channel =>
                {
                    // get scert client
                    if (!channel.HasAttribute(LIBRARY.Pipeline.Constants.SCERT_CLIENT))
                        channel
                            .GetAttribute(LIBRARY.Pipeline.Constants.SCERT_CLIENT)
                            .Set(new ScertClientAttribute());
                    var scertClient = channel
                        .GetAttribute(LIBRARY.Pipeline.Constants.SCERT_CLIENT)
                        .Get();
                    scertClient.CipherService = _cipher;
                    scertClient.MediusVersion = ClientObject?.MediusVersion;
                },
                OnChannelMessage = (channel, message) =>
                {
                    _recvQueue.Enqueue(message);
                    ClientObject?.OnRecv(message);

                    // Log if id is set
                    if (message.CanLog())
                        LoggerAccessor.LogInfo($"[UDPClientServer] - RECV {channel}: {message}");
                },
            };

            var bootstrap = new Bootstrap();
            bootstrap
                .Group(_group)
                .ChannelFactory(() =>
                {
                    var socket = new Socket(
                        AddressFamily.InterNetwork,
                        SocketType.Dgram,
                        ProtocolType.Udp
                    );
                    return new SocketDatagramChannel(socket);
                })
                .Handler(new LoggingHandler(LogLevel.INFO))
                .Handler(
                    new ActionChannelInitializer<IChannel>(channel =>
                    {
                        var pipeline = channel.Pipeline;

                        pipeline.AddLast(
                            new ScertDatagramEncoder(Constants.MEDIUS_UDP_MESSAGE_MAXLEN)
                        );
                        pipeline.AddLast(
                            new ScertDatagramIEnumerableEncoder(Constants.MEDIUS_UDP_MESSAGE_MAXLEN)
                        );
                        pipeline.AddLast(new ScertDatagramDecoder());
                        pipeline.AddLast(new ScertDatagramMultiAppDecoder());
                        pipeline.AddLast(_scertHandler);
                    })
                );

            _boundChannel = await bootstrap.BindAsync(Port).ConfigureAwait(false);
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
                    var closeTask = _boundChannel.CloseAsync();
                    if (
                        !await closeTask
                            .TryAwait(TimeSpan.FromMilliseconds(2000))
                            .ConfigureAwait(false)
                    )
                        LoggerAccessor.LogWarn(
                            "[UDPClientServer] - Timed out waiting for DME UDP bound channel close."
                        );
                }
            }
            finally
            {
                if (_group != null)
                    await _group
                        .ShutdownGracefullyAsync(
                            TimeSpan.FromMilliseconds(100),
                            TimeSpan.FromSeconds(1)
                        )
                        .ConfigureAwait(false);

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
                    var clientObject = Program.DmeServer.GetClientByScertId(connectAuxUdp.ScertId);
                    if (
                        clientObject != ClientObject
                        && ClientObject?.DmeId != connectAuxUdp.PlayerId
                    )
                        break;

                    AuthenticatedEndPoint = packet.Source;

                    if (ClientObject != null)
                    {
                        ClientObject.RemoteUdpEndpoint = AuthenticatedEndPoint as IPEndPoint;
                        DMEObject.OnUdpConnected();

                        RT_MSG_SERVER_CONNECT_ACCEPT_AUX_UDP msg = new()
                        {
                            PlayerId = (ushort)ClientObject.DmeId,
                            ScertId = ClientObject.ScertId,
                            PlayerCount = (ushort?)ClientObject.DmeWorld?.Clients.Length ?? 0x0001,
                            EndPoint = ClientObject.RemoteUdpEndpoint,
                        };

                        // Send it twice in case of packet loss
                        //_boundChannel.WriteAndFlushAsync(new ScertDatagramPacket(msg, packet.Source));
                        _boundChannel?.WriteAndFlushAsync(
                            new ScertDatagramPacket(msg, packet.Source)
                        );
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
                    if (
                        AuthenticatedEndPoint == null
                        || !AuthenticatedEndPoint.Equals(packet.Source)
                    )
                        break;

                    if (ClientObject != null)
                    {
                        Action<RT_MSG_CLIENT_APP_SINGLE, DMEObject>? modifyMessagePerClient = null;
                        var MessagePayload = clientAppBroadcast.Payload;

                        var InvalidatedRequest = false;

                        if (
                            ClientObject.ApplicationId == 20371
                            || ClientObject.ApplicationId == 20374
                        )
                            InvalidatedRequest = HomeHubProxy.ProcessDMEProxyTunneling(
                                MessagePayload,
                                ClientObject,
                                ref modifyMessagePerClient
                            );

                        if (!InvalidatedRequest)
                            ClientObject?.DmeWorld?.BroadcastUdp(
                                ClientObject,
                                MessagePayload ?? Array.Empty<byte>(),
                                modifyMessagePerClient
                            );
                    }
                    break;
                }
                case RT_MSG_CLIENT_APP_LIST clientAppList:
                {
                    if (
                        AuthenticatedEndPoint == null
                        || !AuthenticatedEndPoint.Equals(packet.Source)
                    )
                        break;

                    if (ClientObject != null)
                    {
                        Action<RT_MSG_CLIENT_APP_SINGLE, DMEObject>? modifyMessagePerClient = null;
                        var MessagePayload = clientAppList.Payload;

                        var InvalidatedRequest = false;

                        if (
                            ClientObject.ApplicationId == 20371
                            || ClientObject.ApplicationId == 20374
                        )
                            InvalidatedRequest = HomeHubProxy.ProcessDMEProxyTunneling(
                                MessagePayload,
                                ClientObject,
                                ref modifyMessagePerClient
                            );

                        if (!InvalidatedRequest)
                            ClientObject?.DmeWorld?.SendUdpAppList(
                                ClientObject,
                                clientAppList.Targets,
                                MessagePayload ?? Array.Empty<byte>(),
                                modifyMessagePerClient
                            );
                    }
                    break;
                }
                case RT_MSG_CLIENT_APP_SINGLE clientAppSingle:
                {
                    if (
                        AuthenticatedEndPoint == null
                        || !AuthenticatedEndPoint.Equals(packet.Source)
                    )
                        break;

                    if (ClientObject != null)
                    {
                        Action<RT_MSG_CLIENT_APP_SINGLE, DMEObject>? modifyMessagePerClient = null;
                        var MessagePayload = clientAppSingle.Payload;

                        var InvalidatedRequest = false;

                        if (
                            ClientObject.ApplicationId == 20371
                            || ClientObject.ApplicationId == 20374
                        )
                            InvalidatedRequest = HomeHubProxy.ProcessDMEProxyTunneling(
                                MessagePayload,
                                ClientObject,
                                ref modifyMessagePerClient
                            );

                        if (!InvalidatedRequest)
                            ClientObject?.DmeWorld?.SendUdpAppSingle(
                                ClientObject,
                                clientAppSingle.TargetOrSource,
                                MessagePayload ?? Array.Empty<byte>()
                            );
                    }
                    break;
                }
                case RT_MSG_CLIENT_APP_TOSERVER clientAppToServer:
                {
                    if (
                        AuthenticatedEndPoint == null
                        || !AuthenticatedEndPoint.Equals(packet.Source)
                    )
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
                    LoggerAccessor.LogWarn($"[UDPClientServer] - UNHANDLED MESSAGE: {message}");

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
            return AuthenticatedEndPoint == null || _boundChannel == null
                ? Task.CompletedTask
                : _boundChannel.WriteAndFlushAsync(
                    new ScertDatagramPacket(message, AuthenticatedEndPoint)
                );
        }

        public Task SendImmediate(IEnumerable<BaseScertMessage> messages)
        {
            return AuthenticatedEndPoint == null || _boundChannel == null
                ? Task.CompletedTask
                : _boundChannel.WriteAndFlushAsync(
                    messages.Select(x => new ScertDatagramPacket(x, AuthenticatedEndPoint))
                );
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
                        if (
                            !await PassMessageToPlugins(_boundChannel, ClientObject, message, true)
                                .ConfigureAwait(false)
                        )
                            ProcessMessage(message);
                    }
                    catch (Exception e)
                    {
                        LoggerAccessor.LogError(
                            $"[UDPClientServer] - clientChannel ticking thrown an assertion while processing the message queue. (Exception:{e})"
                        );
                    }
                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(
                    $"[UDPClientServer] - clientChannel ticking thrown an assertion. (Exception:{e})"
                );
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
                        if (
                            !await PassMessageToPlugins(_boundChannel, ClientObject, message, false)
                                .ConfigureAwait(false)
                        )
                            responses.Add(message);
                    }

                    if (responses.Count > 0)
                        await _boundChannel.WriteAndFlushAsync(responses).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(
                    $"[UDPClientServer] - clientChannel ticking thrown an assertion. (Exception:{e})"
                );
            }
        }

        #endregion

        protected async Task<bool> PassMessageToPlugins(
            IChannel clientChannel,
            DMEObject clientObject,
            ScertDatagramPacket packet,
            bool isIncoming
        )
        {
            var message = packet.Message;
            OnMessageArgs onMsg = new(isIncoming)
            {
                Player = clientObject,
                Channel = clientChannel,
                Message = message,
            };

            var onUdpMsg = new OnUdpMsg(isIncoming) { Player = ClientObject, Packet = packet };

            // go from lowest form upwards
            await Program
                .DmeManager.Plugins.OnEvent(PluginEvent.DME_GAME_ON_RECV_UDP, onUdpMsg)
                .ConfigureAwait(false);
            if (onUdpMsg.Ignore)
                return true;

            await Program
                .DmeManager.Plugins.OnMessageEvent(message.Id, onMsg)
                .ConfigureAwait(false);
            if (onMsg.Ignore)
                return true;

            // Send medius message to plugins
            if (message is RT_MSG_CLIENT_APP_TOSERVER clientApp)
            {
                OnMediusMessageArgs onMediusMsg = new(isIncoming)
                {
                    Player = clientObject,
                    Channel = clientChannel,
                    Message = clientApp.Message,
                };
                if (clientApp.Message != null)
                    await Program
                        .DmeManager.Plugins.OnMediusMessageEvent(
                            clientApp.Message.PacketClass,
                            clientApp.Message.PacketType,
                            onMediusMsg
                        )
                        .ConfigureAwait(false);
                if (onMediusMsg.Ignore)
                    return true;
            }
            else if (message is RT_MSG_SERVER_APP serverApp)
            {
                OnMediusMessageArgs onMediusMsg = new(isIncoming)
                {
                    Player = clientObject,
                    Channel = clientChannel,
                    Message = serverApp.Message,
                };
                if (serverApp.Message != null)
                    await Program
                        .DmeManager.Plugins.OnMediusMessageEvent(
                            serverApp.Message.PacketClass,
                            serverApp.Message.PacketType,
                            onMediusMsg
                        )
                        .ConfigureAwait(false);
                if (onMediusMsg.Ignore)
                    return true;
            }

            return false;
        }
    }
}
