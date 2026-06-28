using System.Collections.Concurrent;
using System.Net;
using CustomLogger;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Horizon.CustomServers;
using Horizon.CustomServers.Models;
using Horizon.LIBRARY.Pipeline.Tcp;
using Horizon.MEDIUS.Models;
using Horizon.MEDIUS.PluginArgs;
using Horizon.RT.Common;
using Horizon.RT.Models;
using MultiServerLibrary.Extension;
using MultiServerLibrary.Extension.NET;

namespace Horizon.MEDIUS.Processors
{
    public abstract class BaseMediusProcessor : IMediusProcessor
    {
        private static readonly TimeSpan _defaultTimeout = TimeSpan.FromMilliseconds(5000);

        public readonly DNTCPHybridServer _MediusServer = new();

        public abstract ushort TCPPort { get; set; }
        public abstract ushort UDPPort { get; set; }

        private readonly UniqueIDGenerator _clientCounter = new UniqueIDGenerator();

        protected ConcurrentQueue<IChannel> _forceDisconnectQueue = new();
        protected ConcurrentQueue<IChannel> _disconnectedQueue = new ConcurrentQueue<IChannel>();
        protected ConcurrentDictionary<string, ChannelData> _channelDatas = new();

        public Task StartAsync(int maxConcurrentListeners = 10)
        {
            var portsConfig = new Dictionary<ushort, bool> { { TCPPort, true } };

            if (UDPPort != 0)
                portsConfig.Add(UDPPort, true); // UDP over TCP Hybrid Server.

            return Task.Run(() =>
            {
                _MediusServer.Start(
                    portsConfig,
                    maxConcurrentListeners,
                    new ActionChannelInitializer<ISocketChannel>(channel =>
                    {
                        var pipeline = channel.Pipeline;

                        pipeline.AddLast(new WriteTimeoutHandler(60 * 15));
                        pipeline.AddLast(new ScertEncoder());
                        pipeline.AddLast(new ScertIEnumerableEncoder());
                        pipeline.AddLast(
                            new ScertTcpFrameDecoder(
                                DotNetty.Buffers.ByteOrder.LittleEndian,
                                2048,
                                1,
                                2,
                                0,
                                0,
                                false
                            )
                        );
                        pipeline.AddLast(new ScertDecoder());
                        pipeline.AddLast(new ScertMultiAppDecoder());
                        pipeline.AddLast(_MediusServer.ScertHandler);
                    }),
                    null,
                    (channel) =>
                    {
                        var data = new ChannelData() { State = ServerClientState.CONNECTED };
                        _channelDatas.TryAdd(channel.Id.AsLongText(), data);

                        OnConnected(channel);

                        // Check if IP is banned
                        HorizonServerConfiguration
                            .Database.GetIsIpBanned(
                                (channel.RemoteAddress as IPEndPoint).Address.MapToIPv4()
                            )
                            .TimeoutAfter(_defaultTimeout)
                            .ContinueWith(
                                (r) =>
                                {
                                    data.IsBanned = r.IsCompletedSuccessfully && r.Result;
                                    if (data.IsBanned == true)
                                        QueueBanMessage(data);
                                    else
                                    {
                                        // Check if in maintenance mode
                                        HorizonServerConfiguration
                                            .Database.GetServerFlags()
                                            .TimeoutAfter(_defaultTimeout)
                                            .ContinueWith(
                                                (r) =>
                                                {
                                                    if (
                                                        r.IsCompletedSuccessfully
                                                        && r.Result != null
                                                        && r.Result.MaintenanceMode != null
                                                    )
                                                    {
                                                        // Ensure that maintenance is active
                                                        // Ensure that we're past the from date
                                                        // Ensure that we're before the to date (if set)
                                                        if (
                                                            r.Result.MaintenanceMode.IsActive
                                                            && DateTimeUtils.GetHighPrecisionUtcTime()
                                                                > r.Result.MaintenanceMode.FromDt
                                                            && (
                                                                !r.Result
                                                                    .MaintenanceMode
                                                                    .ToDt
                                                                    .HasValue
                                                                || r.Result.MaintenanceMode.ToDt
                                                                    > DateTimeUtils.GetHighPrecisionUtcTime()
                                                            )
                                                        )
                                                            QueueBanMessage(
                                                                data,
                                                                "Server in maintenance."
                                                            );
                                                    }
                                                }
                                            );
                                    }
                                }
                            );
                    },
                    (channel) =>
                    {
                        _disconnectedQueue.Enqueue(channel);
                    },
                    (channel, messageObj) =>
                    {
                        BaseScertMessage message = (BaseScertMessage)messageObj;

                        if (_channelDatas.TryGetValue(channel.Id.AsLongText(), out var data))
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
                            LoggerAccessor.LogInfo(
                                $"[BaseMediusProcessor] - RECV {data?.ClientObject},{channel}: {message}"
                            );
                    }
                );
            });
        }

        public async Task Tick()
        {
            if (_MediusServer.ScertHandler == null)
                return;

            // Tick clients
            await Task.WhenAll(_MediusServer.ScertHandler.Channels.Select(Tick).ToArray())
                .ConfigureAwait(false);

            // Disconnect and remove timedout unauthenticated channels
            while (_forceDisconnectQueue.TryDequeue(out var channel))
            {
                _channelDatas.TryGetValue(channel.Id.AsLongText(), out var d);

                // Logout
                if (d?.ClientObject?.IsLoggedIn == true)
                    await d
                        .ClientObject.Logout()
                        .TryAwait(TimeSpan.FromMilliseconds(2000))
                        .ConfigureAwait(false);

                LoggerAccessor.LogWarn(
                    $"[BaseMediusProcessor] - REMOVING CHANNEL {channel},{d},{d?.ClientObject}"
                );

                // close after 5 seconds
                _ = Task.Run(async () =>
                {
                    await Task.Delay(5000).ConfigureAwait(false);
                    try
                    {
                        await channel
                            .CloseAsync()
                            .TryAwait(TimeSpan.FromMilliseconds(2000))
                            .ConfigureAwait(false);
                    }
                    catch
                    {
                        // Not Important.
                    }
                });
            }

            // Handle disconnected clients
            while (_disconnectedQueue.TryDequeue(out var channel))
            {
                if (_channelDatas.TryRemove(channel.Id.AsLongText(), out var data))
                {
                    data.State = ServerClientState.DISCONNECTED;
                    data.ClientObject?.OnDisconnected();
                }

                await OnDisconnected(channel).ConfigureAwait(false);
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

            var responses = new List<BaseScertMessage>();

            try
            {
                if (_channelDatas.TryGetValue(clientChannel.Id.AsLongText(), out var data))
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
                            if (
                                !await PassMessageToPlugins(clientChannel, data, message, true)
                                    .ConfigureAwait(false)
                                && data.State != ServerClientState.DISCONNECTED
                            )
                                await ProcessMessage(message, clientChannel, data)
                                    .ConfigureAwait(false);
                        }
                        catch (Exception e)
                        {
                            LoggerAccessor.LogError(
                                $"[BaseMediusProcessor] - clientChannel ticking thrown an assertion while processing the message queue. (Exception:{e})"
                            );

                            _ = ForceDisconnectClient(clientChannel);
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
                            if (
                                !await PassMessageToPlugins(clientChannel, data, message, false)
                                    .ConfigureAwait(false)
                            )
                                responses.Add(message);
                        }

                        if (data.ClientObject != null)
                        {
                            // Echo
                            if (
                                data.ClientObject.MediusVersion > 108
                                && (
                                    DateTimeUtils.GetHighPrecisionUtcTime()
                                    - data.ClientObject.UtcLastServerEchoSent
                                ).TotalSeconds
                                    > DATABASE
                                        .DatabaseManager.GetAppSettingsOrDefault(
                                            data.ClientObject.ApplicationId
                                        )
                                        .ServerEchoIntervalSeconds
                            )
                                data.ClientObject.QueueServerEcho();

                            // Add client object's send queue to responses
                            while (data.ClientObject.SendMessageQueue.TryDequeue(out var message))
                            {
                                // Send to plugins
                                // Ignore if ignored
                                if (
                                    !await PassMessageToPlugins(clientChannel, data, message, false)
                                        .ConfigureAwait(false)
                                )
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
                LoggerAccessor.LogError(
                    $"[BaseMediusComponent] - clientChannel ticking thrown an assertion. (Exception:{e})"
                );

                _forceDisconnectQueue.Enqueue(clientChannel);
            }
        }

        protected virtual Task QueueBanMessage(
            ChannelData? data,
            string msg = "You have been banned!"
        )
        {
            // Send ban message
            data?.SendQueue.Enqueue(
                new RT_MSG_SERVER_SYSTEM_MESSAGE()
                {
                    Severity = (byte)
                        DATABASE
                            .DatabaseManager.GetAppSettingsOrDefault(data.ApplicationId)
                            .BanSystemMessageSeverity,
                    EncodingType = DME_SERVER_ENCODING_TYPE.DME_SERVER_ENCODING_UTF8,
                    LanguageType = DME_SERVER_LANGUAGE_TYPE.DME_SERVER_LANGUAGE_US_ENGLISH,
                    EndOfMessage = true,
                    Message = msg,
                }
            );

            return Task.CompletedTask;
        }

        protected virtual void QueueClanKickMessage(ChannelData data, string msg)
        {
            // Send clan kick message
            data.SendQueue.Enqueue(
                new RT_MSG_SERVER_SYSTEM_MESSAGE()
                {
                    Severity = (byte)
                        DATABASE
                            .DatabaseManager.GetAppSettingsOrDefault(data.ApplicationId)
                            .BanSystemMessageSeverity,
                    EncodingType = DME_SERVER_ENCODING_TYPE.DME_SERVER_ENCODING_UTF8,
                    LanguageType = DME_SERVER_LANGUAGE_TYPE.DME_SERVER_LANGUAGE_US_ENGLISH,
                    EndOfMessage = true,
                    Message = msg,
                }
            );
        }

        protected abstract Task ProcessMessage(
            BaseScertMessage message,
            IChannel clientChannel,
            ChannelData data
        );

        #region Channel

        protected static async Task ForceDisconnectClient(IChannel channel)
        {
            try
            {
                // send force disconnect message
                await channel
                    .WriteAndFlushAsync(
                        new RT_MSG_SERVER_FORCED_DISCONNECT()
                        {
                            Reason = SERVER_FORCE_DISCONNECT_REASON.SERVER_FORCED_DISCONNECT_ERROR,
                        }
                    )
                    .ConfigureAwait(false);

                // close channel
                await channel.CloseAsync().ConfigureAwait(false);
            }
            catch
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
                foreach (var clientChannel in clientChannels)
                    if (clientChannel != null)
                        if (_channelDatas.TryGetValue(clientChannel.Id.AsLongText(), out var data))
                            data.SendQueue.Enqueue(message);
            }
        }

        public void Queue(IEnumerable<BaseScertMessage> messages, params IChannel[] clientChannels)
        {
            Queue(messages, (IEnumerable<IChannel>)clientChannels);
        }

        public void Queue(
            IEnumerable<BaseScertMessage> messages,
            IEnumerable<IChannel> clientChannels
        )
        {
            foreach (var clientChannel in clientChannels)
                if (clientChannel != null)
                    if (_channelDatas.TryGetValue(clientChannel.Id.AsLongText(), out var data))
                        foreach (var message in messages)
                            data.SendQueue.Enqueue(message);
        }

        #endregion

        #region Plugins

        protected static async Task<bool> PassMessageToPlugins(
            IChannel clientChannel,
            ChannelData data,
            BaseScertMessage message,
            bool isIncoming
        )
        {
            OnMessageArgs onMsg = new(isIncoming)
            {
                Player = data.ClientObject,
                Channel = clientChannel,
                Message = message,
            };

            // Send to plugins
            await Program
                .MediusManager.Plugins.OnMessageEvent(message.Id, onMsg)
                .ConfigureAwait(false);
            if (onMsg.Ignore)
                return true;

            // Send medius message to plugins
            if (message is RT_MSG_CLIENT_APP_TOSERVER clientApp)
            {
                OnMediusMessageArgs onMediusMsg = new(isIncoming)
                {
                    Player = data.ClientObject,
                    Channel = clientChannel,
                    Message = clientApp.Message,
                };
                if (clientApp.Message != null)
                    await Program
                        .MediusManager.Plugins.OnMediusMessageEvent(
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
                    Player = data.ClientObject,
                    Channel = clientChannel,
                    Message = serverApp.Message,
                };
                if (serverApp.Message != null)
                    await Program
                        .MediusManager.Plugins.OnMediusMessageEvent(
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

        #endregion

        protected uint GenerateNewScertClientId()
        {
            return _clientCounter.CreateSequentialID();
        }

        public Task StopAsync()
        {
            return _MediusServer.StopAsync();
        }
    }
}
