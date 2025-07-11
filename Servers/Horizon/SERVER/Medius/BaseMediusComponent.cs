using CustomLogger;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Horizon.RT.Common;
using Horizon.RT.Models;
using Horizon.LIBRARY.Common;
using Horizon.RT.Cryptography.RC;
using Horizon.SERVER.PluginArgs;
using Horizon.LIBRARY.Pipeline.Tcp;
using System.Collections.Concurrent;
using System.Net;
using Horizon.MUM.Models;
using MultiServerLibrary.Extension;

namespace Horizon.SERVER.Medius
{
    public abstract class BaseMediusComponent : IMediusComponent
    {
        public static Random RNG = new();

        public enum ClientState
        {
            DISCONNECTED,
            CONNECTED,
            HELLO,
            HANDSHAKE,
            CONNECT_1,
            AUTHENTICATED
        }

        public abstract int TCPPort { get; }
        public abstract int UDPPort { get; }

        public IPAddress IPAddress => MediusClass.SERVER_IP;

        protected IEventLoopGroup? _bossGroup = null;
        protected IEventLoopGroup? _workerGroup = null;
        protected ConcurrentBag<IChannel?>? _boundChannel = null;
        protected ScertServerHandler? _scertHandler = null;
        private uint _clientCounter = 0;

        public class ChannelData
        {
            public int ApplicationId { get; set; } = 0;
            public ClientObject? ClientObject { get; set; } = null;
            public ClientObject? MeClientObject { get; set; } = null;
            public string? MachineId { get; set; } = null;
            public ConcurrentQueue<BaseScertMessage> RecvQueue { get; } = new ConcurrentQueue<BaseScertMessage>();
            public ConcurrentQueue<BaseScertMessage> SendQueue { get; } = new ConcurrentQueue<BaseScertMessage>();

            public ClientState State { get; set; } = ClientState.DISCONNECTED;

            public bool? IsBanned { get; set; } = null;

            /// <summary>
            /// When true, all messages from this client will be ignored.
            /// </summary>
            public bool Ignore { get; set; } = false;
            public DateTime TimeConnected { get; set; } = DateTimeUtils.GetHighPrecisionUtcTime();


            /// <summary>
            /// Timesout client if they haven't authenticated after a given number of seconds.
            /// </summary>
            public bool ShouldDestroy => ClientObject == null && (DateTimeUtils.GetHighPrecisionUtcTime() - TimeConnected).TotalSeconds > MediusClass.GetAppSettingsOrDefault(ApplicationId).ClientTimeoutSeconds;
        }

        protected ConcurrentQueue<IChannel> _forceDisconnectQueue = new ConcurrentQueue<IChannel>();
        protected ConcurrentDictionary<string, ChannelData> _channelDatas = new ConcurrentDictionary<string, ChannelData>();

        protected PS2_RC4? _sessionCipher = null;

        protected DateTime _timeLastEcho = DateTimeUtils.GetHighPrecisionUtcTime();


        public virtual async void Start()
        {
            _bossGroup = new MultithreadEventLoopGroup(1);
            _workerGroup = new MultithreadEventLoopGroup();

            _scertHandler = new ScertServerHandler();

            // Add client on connect
            _scertHandler.OnChannelActive += (channel) =>
            {
                string key = channel.Id.AsLongText();
                var data = new ChannelData()
                {
                    State = ClientState.CONNECTED
                };
                _channelDatas.TryAdd(key, data);

                OnConnected(channel);
            };

            // Remove client on disconnect
            _scertHandler.OnChannelInactive += async (channel) =>
            {
                await Tick(channel);
                string key = channel.Id.AsLongText();
                if (_channelDatas.TryRemove(key, out var data))
                {
                    data.State = ClientState.DISCONNECTED;
                    data.ClientObject?.OnDisconnected();
                }

                await OnDisconnected(channel);
            };

            // Queue all incoming messages
            _scertHandler.OnChannelMessage += (channel, message) =>
            {
                string key = channel.Id.AsLongText();
                if (_channelDatas.TryGetValue(key, out var data))
                {
                    // Don't queue message if client is ignored
                    if (!data.Ignore)
                    {
                        // Don't queue if banned
                        if (data.IsBanned == null || data.IsBanned == false)
                        {
                            data.RecvQueue.Enqueue(message);

                            if (message is RT_MSG_SERVER_ECHO serverEcho)
                                data.ClientObject?.OnRecvServerEcho(serverEcho);
                            else if (message is RT_MSG_CLIENT_ECHO clientEcho)
                                data.ClientObject?.OnRecvClientEcho(clientEcho);

                            data.ClientObject?.OnRecv(message);
                        }
                    }
                }

                // Log if id is set
                if (message.CanLog())
                    LoggerAccessor.LogInfo($"MEDIUS RECV {data?.ClientObject},{channel}: {message}");
            };

            try
            {
                var bootstrap = new ServerBootstrap();
                bootstrap
                    .Group(_bossGroup, _workerGroup)
                    .Channel<TcpServerSocketChannel>()
                    .Handler(new LoggingHandler(LogLevel.INFO))
                    .ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
                    {
                        IChannelPipeline pipeline = channel.Pipeline;

                        pipeline.AddLast(new WriteTimeoutHandler(60 * 15));
                        pipeline.AddLast(new ScertEncoder());
                        pipeline.AddLast(new ScertIEnumerableEncoder());
                        pipeline.AddLast(new ScertTcpFrameDecoder(DotNetty.Buffers.ByteOrder.LittleEndian, 2048, 1, 2, 0, 0, false));
                        pipeline.AddLast(new ScertDecoder());
                        pipeline.AddLast(new ScertMultiAppDecoder());
                        pipeline.AddLast(_scertHandler);
                    }))
                    .ChildOption(ChannelOption.TcpNodelay, true)
                    .ChildOption(ChannelOption.SoTimeout, 1000 * 60 * 15);

                _boundChannel = new ConcurrentBag<IChannel?> { await bootstrap.BindAsync(TCPPort), (UDPPort != 0) ? await bootstrap.BindAsync(UDPPort) : null };
            }
            finally
            {

            }
        }

        public void Log()
        {

        }

        public virtual async Task Stop()
        {
            try
            {
                if (_boundChannel != null)
                {
                    foreach (var boundChannel in _boundChannel)
                    {
                        if (boundChannel != null)
                            await boundChannel.CloseAsync();
                    }

                    _boundChannel = null;
                }
            }
            finally
            {
                if (_bossGroup != null && _workerGroup != null)
                    await Task.WhenAll(
                        _bossGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)),
                        _workerGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)));
            }
        }

        public async Task Tick()
        {
            if (_scertHandler == null || _scertHandler.Group == null)
                return;

            // Tick clients
#if NET8_0_OR_GREATER
            /* ToArray() is necessary, else, weird issues happens in NET8.0+ (https://github.com/dotnet/runtime/issues/105576) */
            await Task.WhenAll(_scertHandler.Group.ToArray().Select(c => Tick(c)));
#else
            await Task.WhenAll(_scertHandler.Group.Select(c => Tick(c)));
#endif
            // Disconnect and remove timedout unauthenticated channels
            while (_forceDisconnectQueue.TryDequeue(out var channel))
            {
                // Send disconnect message
                _ = ForceDisconnectClient(channel);

                // Remove
                _channelDatas.TryRemove(channel.Id.AsLongText(), out var d);

                // Logout
                d?.ClientObject?.Logout();

                LoggerAccessor.LogWarn($"REMOVING CHANNEL {channel},{d},{d?.ClientObject}");

                // close after 5 seconds
                _ = Task.Run(async () =>
                {
                    await Task.Delay(5000);
                    try { await channel.CloseAsync(); } catch { }
                });
            }
        }

        protected virtual Task OnConnected(IChannel clientChannel)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnDisconnected(IChannel clientChannel)
        {
            return Task.CompletedTask;
        }

        protected virtual async Task Tick(IChannel clientChannel)
        {
            if (clientChannel == null)
                return;

            List<BaseScertMessage> responses = new List<BaseScertMessage>();
            string key = clientChannel.Id.AsLongText();

            try
            {
                if (_channelDatas.TryGetValue(key, out var data))
                {
                    // Destroy
                    if (data.ShouldDestroy)
                    {
                        _forceDisconnectQueue.Enqueue(clientChannel);
                        return;
                    }

                    // Ignore
                    if (data.Ignore)
                        return;

                    // Process all messages in queue
                    while (data.RecvQueue.TryDequeue(out var message))
                    {
                        try
                        {
                            // Send to plugins
                            // Ignore if ignored
                            if (!await PassMessageToPlugins(clientChannel, data, message, true) && data.State != ClientState.DISCONNECTED)
                                await ProcessMessage(message, clientChannel, data);
                        }
                        catch (Exception e)
                        {
                            LoggerAccessor.LogError(e);
                            LoggerAccessor.LogError($"FORCE DISCONNECTING CLIENT 1 {data} || {data.ClientObject}");
                            _ = ForceDisconnectClient(clientChannel);
                            data.Ignore = true;
                        }
                    }

                    // Send if writeable
                    if (clientChannel.IsWritable)
                    {
                        // Add send queue to responses
                        while (data.SendQueue.TryDequeue(out var message))
                        {
                            // Send to plugins
                            // Ignore if ignored
                            if (!await PassMessageToPlugins(clientChannel, data, message, false))
                                responses.Add(message);
                        }

                        if (data.ClientObject != null)
                        {
                            // Echo
                            if (data.ClientObject.MediusVersion > 108 && (DateTimeUtils.GetHighPrecisionUtcTime() - data.ClientObject.UtcLastServerEchoSent).TotalSeconds > MediusClass.GetAppSettingsOrDefault(data.ClientObject.ApplicationId).ServerEchoIntervalSeconds)
                                data.ClientObject.QueueServerEcho();

                            // Add client object's send queue to responses
                            while (data.ClientObject.SendMessageQueue.TryDequeue(out var message))
                            {
                                // Send to plugins
                                // Ignore if ignored
                                if (!await PassMessageToPlugins(clientChannel, data, message, false))
                                    responses.Add(message);
                            }
                        }

                        if (responses.Count > 0)
                            _ = clientChannel.WriteAndFlushAsync(responses);
                    }
                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
                _forceDisconnectQueue.Enqueue(clientChannel);
                //await DisconnectClient(clientChannel);
            }
        }

        protected virtual Task QueueBanMessage(ChannelData? data, string msg = "You have been banned!")
        {
            // Send ban message
            data?.SendQueue.Enqueue(new RT_MSG_SERVER_SYSTEM_MESSAGE()
            {
                Severity = (byte)MediusClass.GetAppSettingsOrDefault(data.ApplicationId).BanSystemMessageSeverity,
                EncodingType = DME_SERVER_ENCODING_TYPE.DME_SERVER_ENCODING_UTF8,
                LanguageType = DME_SERVER_LANGUAGE_TYPE.DME_SERVER_LANGUAGE_US_ENGLISH,
                EndOfMessage = true,
                Message = msg
            });

            return Task.CompletedTask;
        }

        protected virtual void QueueClanKickMessage(ChannelData data, string msg)
        {
            // Send clan kick message
            data.SendQueue.Enqueue(new RT_MSG_SERVER_SYSTEM_MESSAGE()
            {
                Severity = (byte)MediusClass.GetAppSettingsOrDefault(data.ApplicationId).BanSystemMessageSeverity,
                EncodingType = DME_SERVER_ENCODING_TYPE.DME_SERVER_ENCODING_UTF8,
                LanguageType = DME_SERVER_LANGUAGE_TYPE.DME_SERVER_LANGUAGE_US_ENGLISH,
                EndOfMessage = true,
                Message = msg
            });
        }

        protected abstract Task ProcessMessage(BaseScertMessage message, IChannel clientChannel, ChannelData data);

        #region Channel

        protected async Task ForceDisconnectClient(IChannel channel)
        {
            try
            {
                // send force disconnect message
                await channel.WriteAndFlushAsync(new RT_MSG_SERVER_FORCED_DISCONNECT()
                {
                    Reason = SERVER_FORCE_DISCONNECT_REASON.SERVER_FORCED_DISCONNECT_ERROR
                });

                // close channel
                await channel.CloseAsync();
            }
            catch (Exception)
            {
                // Silence exception since the client probably just closed the socket before we could write to it
            }
        }

        #endregion

        #region Queue

        public void Queue(BaseScertMessage message, params IChannel[]? clientChannels)
        {
            Queue(message, (IEnumerable<IChannel>?)clientChannels);
        }

        public void Queue(BaseScertMessage message, IEnumerable<IChannel>? clientChannels)
        {
            if (clientChannels != null)
            {
                foreach (IChannel clientChannel in clientChannels)
                    if (clientChannel != null)
                        if (_channelDatas.TryGetValue(clientChannel.Id.AsLongText(), out ChannelData? data))
                            data.SendQueue.Enqueue(message);
            }
        }

        public void Queue(IEnumerable<BaseScertMessage> messages, params IChannel[] clientChannels)
        {
            Queue(messages, (IEnumerable<IChannel>)clientChannels);
        }

        public void Queue(IEnumerable<BaseScertMessage> messages, IEnumerable<IChannel> clientChannels)
        {
            foreach (IChannel clientChannel in clientChannels)
                if (clientChannel != null)
                    if (_channelDatas.TryGetValue(clientChannel.Id.AsLongText(), out ChannelData? data))
                        foreach (var message in messages)
                            data.SendQueue.Enqueue(message);
        }

        #endregion

        #region Plugins

        protected async Task<bool> PassMessageToPlugins(IChannel clientChannel, ChannelData data, BaseScertMessage message, bool isIncoming)
        {
            OnMessageArgs onMsg = new(isIncoming)
            {
                Player = data.ClientObject,
                Channel = clientChannel,
                Message = message
            };

            // Send to plugins
            await MediusClass.Plugins.OnMessageEvent(message.Id, onMsg);
            if (onMsg.Ignore)
                return true;

            // Send medius message to plugins
            if (message is RT_MSG_CLIENT_APP_TOSERVER clientApp)
            {
                OnMediusMessageArgs onMediusMsg = new(isIncoming)
                {
                    Player = data.ClientObject,
                    Channel = clientChannel,
                    Message = clientApp.Message
                };
                if (clientApp.Message != null)
                    await MediusClass.Plugins.OnMediusMessageEvent(clientApp.Message.PacketClass, clientApp.Message.PacketType, onMediusMsg);
                if (onMediusMsg.Ignore)
                    return true;
            }
            else if (message is RT_MSG_SERVER_APP serverApp)
            {
                OnMediusMessageArgs onMediusMsg = new(isIncoming)
                {
                    Player = data.ClientObject,
                    Channel = clientChannel,
                    Message = serverApp.Message
                };
                if (serverApp.Message != null)
                    await MediusClass.Plugins.OnMediusMessageEvent(serverApp.Message.PacketClass, serverApp.Message.PacketType, onMediusMsg);
                if (onMediusMsg.Ignore)
                    return true;
            }

            return false;
        }

        #endregion

        protected uint GenerateNewScertClientId()
        {
            return _clientCounter++;
        }
    }
}
