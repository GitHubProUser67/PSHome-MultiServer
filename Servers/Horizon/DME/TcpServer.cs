using CustomLogger;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Horizon.RT.Common;
using Horizon.RT.Cryptography;
using Horizon.RT.Models;
using Horizon.LIBRARY.Pipeline.Tcp;
using Horizon.DME.Models;
using System.Collections.Concurrent;
using System.Net;
using DotNetty.Handlers.Timeout;
using Horizon.DME.PluginArgs;
using Horizon.PluginManager;
using EndianTools;
using MultiServerLibrary.Extension;
using Horizon.SERVER;
using Horizon.MUM.Models;
using Horizon.SERVER.Medius;
using Horizon.SERVER.Extension.PlayStationHome;

namespace Horizon.DME
{
    public class TcpServer
    {
        public static Random RNG = new();

        public bool IsRunning => _boundChannel != null && _boundChannel.Active;

        public int Port => DmeClass.Settings.TCPPort;

        protected IEventLoopGroup? _bossGroup = null;
        protected IEventLoopGroup? _workerGroup = null;
        protected IChannel? _boundChannel = null;
        protected ScertServerHandler? _scertHandler = null;
        private ushort _clientCounter = 0;

        protected internal class ChannelData
        {
            public int ApplicationId { get; set; } = 0;
            public bool Ignore { get; set; } = false;
            public DMEObject? DMEObject { get; set; } = null;
            public ConcurrentQueue<BaseScertMessage> RecvQueue { get; } = new();
            public ConcurrentQueue<BaseScertMessage> SendQueue { get; } = new();
            public DateTime TimeConnected { get; set; } = DateTimeUtils.GetHighPrecisionUtcTime();


            /// <summary>
            /// Timesout client if they authenticated after a given number of seconds.
            /// </summary>
            public bool ShouldDestroy => DMEObject == null && (DateTimeUtils.GetHighPrecisionUtcTime() - TimeConnected).TotalSeconds > DmeClass.GetAppSettingsOrDefault(ApplicationId).ClientTimeoutSeconds;
        }

        protected ConcurrentQueue<IChannel> _forceDisconnectQueue = new();
        protected ConcurrentDictionary<string, ChannelData> _channelDatas = new();
        protected ConcurrentDictionary<uint, DMEObject> _scertIdToClient = new();

        /// <summary>
        /// Start the Dme Tcp Server.
        /// </summary>
        public virtual async void Start()
        {
            _bossGroup = new MultithreadEventLoopGroup(1);
            _workerGroup = new MultithreadEventLoopGroup();
            _scertHandler = new ScertServerHandler();

            // Add client on connect
            _scertHandler.OnChannelActive += (channel) =>
            {
                string key = channel.Id.AsLongText();
                _channelDatas.TryAdd(key, new ChannelData());
            };

            // Remove client on disconnect
            _scertHandler.OnChannelInactive += (channel) =>
            {
                string key = channel.Id.AsLongText();
                if (_channelDatas.TryRemove(key, out var data))
                {
                    if (data.DMEObject != null)
                    {
                        data.DMEObject.OnTcpDisconnected();
                        _scertIdToClient.TryRemove(data.DMEObject.ScertId, out _);
                    }
                }
            };

            // Queue all incoming messages
            _scertHandler.OnChannelMessage += async (channel, message) =>
            {
                string key = channel.Id.AsLongText();
                if (_channelDatas.TryGetValue(key, out var data))
                {
                    if (!data.Ignore && (data.DMEObject == null || !data.DMEObject.IsDestroyed))
                    {

                        OnTcpMsg pluginArgs = new()
                        {
                            Player = data.DMEObject,
                            Packet = message
                        };

                        // Plugin
                        await DmeClass.Plugins.OnEvent(PluginEvent.DME_GAME_ON_RECV_TCP, pluginArgs);

                        data.RecvQueue.Enqueue(message);
                        data.DMEObject?.OnRecv(message);
                        if (message is RT_MSG_SERVER_ECHO serverEcho)
                            data.DMEObject?.OnRecvServerEcho(serverEcho);
                        else if (message is RT_MSG_CLIENT_ECHO clientEcho)
                            data.DMEObject?.OnRecvClientEcho(clientEcho);
                    }
                }

                // Log if id is set
                if (message.CanLog())
                    LoggerAccessor.LogDebug($"DME_TCP {data?.DMEObject},{channel}: {message}");
            };

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
                if (_bossGroup != null && _workerGroup != null)
                    await Task.WhenAll(
                        _bossGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)),
                        _workerGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)));
            }
        }

        /// <summary>
        /// Gets DME Server client object.
        /// </summary>
        public DMEObject? GetServerPerAppId(int ApplicationId)
        {
            return _channelDatas.Values.Where(channel => channel.ApplicationId == ApplicationId).FirstOrDefault()?.DMEObject;
        }

        /// <summary>
        /// Process incoming messages.
        /// </summary>
        public async Task HandleIncomingMessages()
        {
            if (_scertHandler == null || _scertHandler.Group == null)
                return;

#if NET8_0_OR_GREATER
            /* ToArray() is necessary, else, weird issues happens in NET8.0+ (https://github.com/dotnet/runtime/issues/105576) */
            await Task.WhenAll(_scertHandler.Group.ToArray().Select(c => HandleIncomingMessages(c)));
#else
            await Task.WhenAll(_scertHandler.Group.Select(c => HandleIncomingMessages(c)));
#endif
        }

        /// <summary>
        /// Process outgoing messages.
        /// </summary>
        public async Task HandleOutgoingMessages()
        {
            if (_scertHandler == null || _scertHandler.Group == null)
                return;

#if NET8_0_OR_GREATER
            /* ToArray() is necessary, else, weird issues happens in NET8.0+ (https://github.com/dotnet/runtime/issues/105576) */
            await Task.WhenAll(_scertHandler.Group.ToArray().Select(c => HandleOutgoingMessages(c)));
#else
            await Task.WhenAll(_scertHandler.Group.Select(c => HandleOutgoingMessages(c)));
#endif

            // Disconnect and remove timedout unauthenticated channels
            while (_forceDisconnectQueue.TryDequeue(out var channel))
            {
                // Send disconnect message
                _ = ForceDisconnectClient(channel);

                // Remove
                _channelDatas.TryRemove(channel.Id.AsLongText(), out var d);
                LoggerAccessor.LogWarn($"REMOVING CHANNEL {channel},{d},{d?.DMEObject}");

                // close after 5 seconds
                _ = Task.Run(async () =>
                {
                    await Task.Delay(5000);
                    try { await channel.CloseAsync(); } catch { }
                });
            }
        }

        private async Task HandleIncomingMessages(IChannel clientChannel)
        {
            if (clientChannel == null)
                return;

            string key = clientChannel.Id.AsLongText();

            try
            {
                if (_channelDatas.TryGetValue(key, out var data))
                {
                    // Process all messages in queue
                    while (data.RecvQueue.TryDequeue(out var message))
                    {
                        try
                        {
                            if (!await PassMessageToPlugins(clientChannel, data, message, true))
                                await ProcessMessage(message, clientChannel, data);
                        }
                        catch (Exception e)
                        {
                            LoggerAccessor.LogError(e);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }
        }

        private async Task HandleOutgoingMessages(IChannel clientChannel)
        {
            if (clientChannel == null)
                return;

            List<BaseScertMessage> responses = new();
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

                    // Disconnect on destroy
                    if (data.DMEObject != null && data.DMEObject.IsDestroyed)
                    {
                        data.Ignore = true;
                        return;
                    }

                    // Send if writeable
                    if (clientChannel.IsWritable)
                    {
                        // Add send queue to responses
                        while (data.SendQueue.TryDequeue(out var message))
                            if (!await PassMessageToPlugins(clientChannel, data, message, false))
                                responses.Add(message);

                        if (data.DMEObject != null)
                        {
                            // Echo
                            if (data.DMEObject.MediusVersion > 108 && (DateTimeUtils.GetHighPrecisionUtcTime() - data.DMEObject.UtcLastServerEchoSent).TotalSeconds > DmeClass.GetAppSettingsOrDefault(data.DMEObject.ApplicationId).ServerEchoIntervalSeconds)
                            {
                                var message = new RT_MSG_SERVER_ECHO();
                                if (!await PassMessageToPlugins(clientChannel, data, message, false))
                                    responses.Add(message);
                                data.DMEObject.UtcLastServerEchoSent = DateTimeUtils.GetHighPrecisionUtcTime();
                            }

                            // Add client object's send queue to responses
                            // But only if not in a world
                            if (data.DMEObject.DmeWorld == null || data.DMEObject.DmeWorld.Destroyed)
                                while (data.DMEObject.TcpSendMessageQueue.TryDequeue(out var message))
                                    if (!await PassMessageToPlugins(clientChannel, data, message, false))
                                        responses.Add(message);
                        }

                        if (responses.Count > 0)
                            _ = clientChannel.WriteAndFlushAsync(responses);
                    }
                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }
        }

        #region Message Processing

        protected async Task ProcessMessage(BaseScertMessage message, IChannel clientChannel, ChannelData data)
        {
            // Get ScertClient data
            var scertClient = clientChannel.GetAttribute(LIBRARY.Pipeline.Constants.SCERT_CLIENT).Get();
            bool enableEncryption = false/*DmeClass.GetAppSettingsOrDefault(data.ApplicationId).EnableDmeEncryption*/;
            if (scertClient.CipherService != null)
                scertClient.CipherService.EnableEncryption = enableEncryption;

            switch (message)
            {
                case RT_MSG_CLIENT_HELLO clientHello:
                    {
                        // send hello
                        Queue(new RT_MSG_SERVER_HELLO() { RsaPublicKey = enableEncryption ? DmeClass.Settings.DefaultKey.N : Org.BouncyCastle.Math.BigInteger.Zero }, clientChannel);
                        break;
                    }
                case RT_MSG_CLIENT_CRYPTKEY_PUBLIC clientCryptKeyPublic:
                    {
                        if (clientCryptKeyPublic.PublicKey != null)
                        {
                            // generate new client session key
                            scertClient.CipherService?.GenerateCipher(CipherContext.RSA_AUTH, clientCryptKeyPublic.PublicKey.Reverse().ToArray());
                            scertClient.CipherService?.GenerateCipher(CipherContext.RC_CLIENT_SESSION);

                            Queue(new RT_MSG_SERVER_CRYPTKEY_PEER() { SessionKey = scertClient.CipherService?.GetPublicKey(CipherContext.RC_CLIENT_SESSION) }, clientChannel);
                        }
                        break;
                    }
                case RT_MSG_CLIENT_CONNECT_TCP_AUX_UDP clientConnectTcpAuxUdp:
                    {
                        ClientObject? mumClient;

                        data.ApplicationId = clientConnectTcpAuxUdp.AppId;
                        scertClient.ApplicationID = clientConnectTcpAuxUdp.AppId;

                        Channel? targetChannel = MediusClass.Manager.GetChannelByChannelId(clientConnectTcpAuxUdp.TargetWorldId, data.ApplicationId);

                        if (targetChannel == null)
                        {
                            Channel DefaultChannel = MediusClass.Manager.GetOrCreateDefaultLobbyChannel(data.ApplicationId, scertClient.MediusVersion!.Value);

                            if (DefaultChannel.Id == clientConnectTcpAuxUdp.TargetWorldId)
                                targetChannel = DefaultChannel;

                            if (targetChannel == null)
                            {
                                LoggerAccessor.LogError($"[DME] - TcpServer - Client: {clientConnectTcpAuxUdp.AccessToken} tried to join, but targetted WorldId:{clientConnectTcpAuxUdp.TargetWorldId} doesn't exist!");
                                await clientChannel.CloseAsync();
                                break;
                            }
                        }

                        // If booth are null, it means DME client wants a new object.
                        if (!string.IsNullOrEmpty(clientConnectTcpAuxUdp.AccessToken) && !string.IsNullOrEmpty(clientConnectTcpAuxUdp.SessionKey))
                        {
                            mumClient = MediusClass.Manager.GetClientByAccessToken(clientConnectTcpAuxUdp.AccessToken, clientConnectTcpAuxUdp.AppId);
                            if (mumClient == null)
                                mumClient = MediusClass.Manager.GetClientBySessionKey(clientConnectTcpAuxUdp.SessionKey, clientConnectTcpAuxUdp.AppId);

                            if (mumClient != null)
                                LoggerAccessor.LogInfo($"[DME] - TcpServer - Client Connected {clientChannel.RemoteAddress}:{data.DMEObject}: {clientChannel}");
                            else
                            {
                                data.Ignore = true;
                                LoggerAccessor.LogError($"[DME] - TcpServer - ClientObject could not be granted for {clientChannel.RemoteAddress}:{data.DMEObject}: {clientConnectTcpAuxUdp}");
                                break;
                            }

                            mumClient.MediusVersion = scertClient.MediusVersion ?? 0;
                            mumClient.ApplicationId = clientConnectTcpAuxUdp.AppId;
                            mumClient.OnConnected();
                        }
                        else
                        {
                            data.Ignore = true;
                            LoggerAccessor.LogError($"[DME] - TcpServer - ClientObject could not be found for {clientChannel.RemoteAddress}:{data.DMEObject}: {clientConnectTcpAuxUdp}");
                            break;
                        }

                        await mumClient.JoinChannel(targetChannel);

                        if (!string.IsNullOrEmpty(clientConnectTcpAuxUdp.AccessToken) && !string.IsNullOrEmpty(clientConnectTcpAuxUdp.SessionKey))
                        {
                            data.DMEObject = DmeClass.GetMPSClientByAccessToken(clientConnectTcpAuxUdp.AccessToken);
                            if (data.DMEObject == null)
                                data.DMEObject = DmeClass.GetMPSClientBySessionKey(clientConnectTcpAuxUdp.SessionKey);

                            if (data.DMEObject != null)
                                LoggerAccessor.LogInfo($"[DME] - TcpServer - DMEClient Connected {clientChannel.RemoteAddress}:{data.DMEObject}: {clientChannel}");
                            else
                            {
                                data.Ignore = true;
                                LoggerAccessor.LogError($"[DME] - TcpServer - DMEClientObject could not be granted for {clientChannel.RemoteAddress}:{data.DMEObject}: {clientConnectTcpAuxUdp}");
                                break;
                            }
                        }
                        else // MAG uses DME TCP directly to register a ClientObject.
                        {
                            LoggerAccessor.LogInfo($"[DME] - TcpServer - DMEClient Connected {clientChannel.RemoteAddress} with new ClientObject!");

                            data.DMEObject = new DMEObject(clientConnectTcpAuxUdp.SessionKey, mumClient);
                        }

                        data.DMEObject.ApplicationId = clientConnectTcpAuxUdp.AppId;
                        data.DMEObject.OnTcpConnected(clientChannel);
                        data.DMEObject.ScertId = GenerateNewScertClientId();
                        data.DMEObject.MediusVersion = scertClient.MediusVersion;

                        if (!_scertIdToClient.TryAdd(data.DMEObject.ScertId, data.DMEObject))
                        {
                            LoggerAccessor.LogWarn($"Duplicate scert client id");
                            break;
                        }

                        // start udp server
                        data.DMEObject.BeginUdp(scertClient.CipherService);

                        #region if PS3
                        if (scertClient.IsPS3Client)
                        {
                            List<int> ConnectAcceptTCPGames = new() { 20623, 20624, 21564, 21574, 21584, 21594, 22274, 22284, 22294, 22304, 20040, 20041, 20042, 20043, 20044 };

                            //CAC & Warhawk
                            if (ConnectAcceptTCPGames.Contains(data.ApplicationId))
                            {
                                Queue(new RT_MSG_SERVER_CONNECT_ACCEPT_TCP()
                                {
                                    PlayerId = (ushort)data.DMEObject.DmeId,
                                    ScertId = data.DMEObject.ScertId,
                                    PlayerCount = (ushort?)data.DMEObject.DmeWorld?.Clients.Length ?? 0x0001,
                                    IP = (clientChannel.RemoteAddress as IPEndPoint)?.Address
                                }, clientChannel);
                            }
                            else
                                Queue(new RT_MSG_SERVER_CONNECT_REQUIRE() { MaxPacketSize = Constants.MEDIUS_MESSAGE_MAXLEN, MaxUdpPacketSize = Constants.MEDIUS_UDP_MESSAGE_MAXLEN }, clientChannel);
                        }
                        #endregion
                        else if (scertClient.MediusVersion > 108 && scertClient.ApplicationID != 11484)
                            Queue(new RT_MSG_SERVER_CONNECT_REQUIRE() { MaxPacketSize = Constants.MEDIUS_MESSAGE_MAXLEN, MaxUdpPacketSize = Constants.MEDIUS_UDP_MESSAGE_MAXLEN }, clientChannel);
                        else
                        {
                            if (scertClient.CipherService != null && scertClient.CipherService.HasKey(CipherContext.RC_CLIENT_SESSION) && scertClient.MediusVersion >= 109 && !scertClient.IsPS3Client)
                                Queue(new RT_MSG_SERVER_CRYPTKEY_GAME() { GameKey = scertClient.CipherService.GetPublicKey(CipherContext.RC_CLIENT_SESSION) }, clientChannel);
                            Queue(new RT_MSG_SERVER_CONNECT_ACCEPT_TCP()
                            {
                                PlayerId = (ushort)data.DMEObject.DmeId,
                                ScertId = data.DMEObject.ScertId,
                                PlayerCount = (ushort?)data.DMEObject.DmeWorld?.Clients.Length ?? 0x0001,
                                IP = (clientChannel.RemoteAddress as IPEndPoint)?.Address
                            }, clientChannel);

                            // DME has no server complete even on older clients.

                            Queue(new RT_MSG_SERVER_INFO_AUX_UDP()
                            {
                                Ip = DmeClass.SERVER_IP,
                                Port = (ushort)data.DMEObject.UdpPort
                            }, clientChannel);
                        }

                        break;
                    }
                case RT_MSG_CLIENT_CONNECT_TCP clientConnectTcp:
                    {
                        ClientObject? mumClient;

                        data.ApplicationId = clientConnectTcp.AppId;
                        scertClient.ApplicationID = clientConnectTcp.AppId;

                        Channel? targetChannel = MediusClass.Manager.GetChannelByChannelId(clientConnectTcp.TargetWorldId, data.ApplicationId);

                        if (targetChannel == null)
                        {
                            Channel DefaultChannel = MediusClass.Manager.GetOrCreateDefaultLobbyChannel(data.ApplicationId, scertClient.MediusVersion!.Value);

                            if (DefaultChannel.Id == clientConnectTcp.TargetWorldId)
                                targetChannel = DefaultChannel;

                            if (targetChannel == null)
                            {
                                LoggerAccessor.LogError($"[DME] - TcpServer - Client: {clientConnectTcp.AccessToken} tried to join, but targetted WorldId:{clientConnectTcp.TargetWorldId} doesn't exist!");
                                await clientChannel.CloseAsync();
                                break;
                            }
                        }

                        // If booth are null, it means DME client wants a new object.
                        if (!string.IsNullOrEmpty(clientConnectTcp.AccessToken) && !string.IsNullOrEmpty(clientConnectTcp.SessionKey))
                        {
                            mumClient = MediusClass.Manager.GetClientByAccessToken(clientConnectTcp.AccessToken, clientConnectTcp.AppId);
                            if (mumClient == null)
                                mumClient = MediusClass.Manager.GetClientBySessionKey(clientConnectTcp.SessionKey, clientConnectTcp.AppId);

                            if (mumClient != null)
                                LoggerAccessor.LogInfo($"[DME] - TcpServer - Client Connected {clientChannel.RemoteAddress}:{data.DMEObject}: {clientChannel}");
                            else
                            {
                                data.Ignore = true;
                                LoggerAccessor.LogError($"[DME] - TcpServer - ClientObject could not be granted for {clientChannel.RemoteAddress}:{data.DMEObject}: {clientConnectTcp}");
                                break;
                            }

                            mumClient.MediusVersion = scertClient.MediusVersion ?? 0;
                            mumClient.ApplicationId = clientConnectTcp.AppId;
                            mumClient.OnConnected();
                        }
                        else // MAG uses DME directly to register a ClientObject.
                        {
                            LoggerAccessor.LogInfo($"[DME] - TcpServer - Client Connected {clientChannel.RemoteAddress} with new ClientObject!");

                            mumClient = new(scertClient.MediusVersion ?? 0)
                            {
                                ApplicationId = clientConnectTcp.AppId
                            };
                            mumClient.OnConnected();

                            MAS.ReserveClient(mumClient); // ONLY RESERVE CLIENTS HERE!
                        }

                        await mumClient.JoinChannel(targetChannel);

                        if (!string.IsNullOrEmpty(clientConnectTcp.AccessToken) && !string.IsNullOrEmpty(clientConnectTcp.SessionKey))
                        {
                            data.DMEObject = DmeClass.GetMPSClientByAccessToken(clientConnectTcp.AccessToken);
                            if (data.DMEObject == null)
                                data.DMEObject = DmeClass.GetMPSClientBySessionKey(clientConnectTcp.SessionKey);

                            if (data.DMEObject != null)
                                LoggerAccessor.LogInfo($"[DME] - TcpServer - DMEClient Connected {clientChannel.RemoteAddress}:{data.DMEObject}: {clientChannel}");
                            else
                            {
                                data.Ignore = true;
                                LoggerAccessor.LogError($"[DME] - TcpServer - DMEClientObject could not be granted for {clientChannel.RemoteAddress}:{data.DMEObject}: {clientConnectTcp}");
                                break;
                            }
                        }
                        else // MAG uses DME TCP directly to register a ClientObject.
                        {
                            LoggerAccessor.LogInfo($"[DME] - TcpServer - DMEClient Connected {clientChannel.RemoteAddress} with new ClientObject!");

                            data.DMEObject = new DMEObject(clientConnectTcp.SessionKey, mumClient);
                        }

                        data.DMEObject.ApplicationId = clientConnectTcp.AppId;
                        data.DMEObject.OnTcpConnected(clientChannel);
                        data.DMEObject.ScertId = GenerateNewScertClientId();
                        data.DMEObject.MediusVersion = scertClient.MediusVersion;

                        if (!_scertIdToClient.TryAdd(data.DMEObject.ScertId, data.DMEObject))
                        {
                            LoggerAccessor.LogWarn($"Duplicate scert client id");
                            break;
                        }

                        #region if PS3
                        if (scertClient.IsPS3Client)
                        {
                            List<int> ConnectAcceptTCPGames = new() { 20623, 20624, 21564, 21574, 21584, 21594, 22274, 22284, 22294, 22304, 20040, 20041, 20042, 20043, 20044 };

                            //CAC & Warhawk
                            if (ConnectAcceptTCPGames.Contains(data.ApplicationId))
                            {
                                Queue(new RT_MSG_SERVER_CONNECT_ACCEPT_TCP()
                                {
                                    PlayerId = (ushort)data.DMEObject.DmeId,
                                    ScertId = data.DMEObject.ScertId,
                                    PlayerCount = (ushort?)data.DMEObject.DmeWorld?.Clients.Length ?? 0x0001,
                                    IP = (clientChannel.RemoteAddress as IPEndPoint)?.Address
                                }, clientChannel);
                            }
                            else
                                Queue(new RT_MSG_SERVER_CONNECT_REQUIRE() { MaxPacketSize = Constants.MEDIUS_MESSAGE_MAXLEN, MaxUdpPacketSize = Constants.MEDIUS_UDP_MESSAGE_MAXLEN }, clientChannel);
                        }
                        #endregion
                        else if (scertClient.MediusVersion > 108 && scertClient.ApplicationID != 11484)
                            Queue(new RT_MSG_SERVER_CONNECT_REQUIRE() { MaxPacketSize = Constants.MEDIUS_MESSAGE_MAXLEN, MaxUdpPacketSize = Constants.MEDIUS_UDP_MESSAGE_MAXLEN }, clientChannel);
                        else
                        {
                            if (scertClient.CipherService != null && scertClient.CipherService.HasKey(CipherContext.RC_CLIENT_SESSION) && scertClient.MediusVersion >= 109 && !scertClient.IsPS3Client)
                                Queue(new RT_MSG_SERVER_CRYPTKEY_GAME() { GameKey = scertClient.CipherService.GetPublicKey(CipherContext.RC_CLIENT_SESSION) }, clientChannel);
                            Queue(new RT_MSG_SERVER_CONNECT_ACCEPT_TCP()
                            {
                                PlayerId = (ushort)data.DMEObject.DmeId,
                                ScertId = data.DMEObject.ScertId,
                                PlayerCount = (ushort?)data.DMEObject.DmeWorld?.Clients.Length ?? 0x0001,
                                IP = (clientChannel.RemoteAddress as IPEndPoint)?.Address
                            }, clientChannel);

                            // DME has no server complete even on older clients.
                        }

                        break;
                    }
                case RT_MSG_CLIENT_CONNECT_READY_REQUIRE clientConnectReadyRequire:
                    {
                        if (scertClient.CipherService != null && scertClient.CipherService.HasKey(CipherContext.RC_CLIENT_SESSION) && !scertClient.IsPS3Client)
                            Queue(new RT_MSG_SERVER_CRYPTKEY_GAME() { GameKey = scertClient.CipherService.GetPublicKey(CipherContext.RC_CLIENT_SESSION) }, clientChannel);
                        Queue(new RT_MSG_SERVER_CONNECT_ACCEPT_TCP()
                        {
                            PlayerId = (ushort)data.DMEObject.DmeId,
                            ScertId = data.DMEObject.ScertId,
                            PlayerCount = (ushort?)data.DMEObject.DmeWorld?.Clients.Length ?? 0x0001,
                            IP = (clientChannel.RemoteAddress as IPEndPoint)?.Address
                        }, clientChannel);
                        break;
                    }
                case RT_MSG_CLIENT_CONNECT_READY_TCP clientConnectReadyTcp:
                    {
                        if (data.DMEObject != null)
                        {
                            // Update recv flag
                            data.DMEObject.RecvFlag = clientConnectReadyTcp.RecvFlag;

                            Queue(new RT_MSG_SERVER_STARTUP_INFO_NOTIFY()
                            {
                                GameHostType = (byte)MGCL_GAME_HOST_TYPE.MGCLGameHostClientServerAuxUDP,
                                Timebase = (uint?)data.DMEObject.DmeWorld?.WorldTimer.ElapsedMilliseconds ?? DateTimeUtils.GetUnixTimeU32()
                            }, clientChannel);
                            Queue(new RT_MSG_SERVER_INFO_AUX_UDP()
                            {
                                Ip = DmeClass.SERVER_IP,
                                Port = (ushort)data.DMEObject.UdpPort
                            }, clientChannel);
                        }
                        break;
                    }
                case RT_MSG_CLIENT_CONNECT_READY_AUX_UDP connectReadyAuxUdp:
                    {
                        data.DMEObject?.OnConnectionCompleted();

                        _ = data.DMEObject?.DmeWorld?.EnqueuePlayerSyncTask(Task.Run(() => {
                            Queue(new RT_MSG_SERVER_CONNECT_COMPLETE()
                            {
                                ClientCountAtConnect = (ushort?)data.DMEObject?.DmeWorld?.Clients.Length ?? 0x0001,
                            }, clientChannel);

                            // Some clients doesn't expect TypeServerVersion.
                            if (scertClient.MediusVersion > 108 
                            && scertClient.ApplicationID != 20371 
                            && scertClient.ApplicationID != 20374 
                            && scertClient.ApplicationID != 20364
                            && scertClient.ApplicationID != 20764
                            && scertClient.ApplicationID != 21624)
                            {
                                Queue(new RT_MSG_SERVER_APP()
                                {
                                    Message = new TypeServerVersion()
                                    {
                                        Version = "2.10.0009"
                                    }
                                }, clientChannel);
                            }
                        }).ContinueWith(x => data.DMEObject?.DmeWorld?.OnPlayerJoined(data.DMEObject)));
                        break;
                    }
                case RT_MSG_SERVER_ECHO serverEchoReply:
                    {

                        break;
                    }
                case RT_MSG_CLIENT_ECHO clientEcho:
                    {
                        Queue(new RT_MSG_CLIENT_ECHO() { Value = clientEcho.Value }, clientChannel);
                        break;
                    }
                case RT_MSG_CLIENT_SET_RECV_FLAG setRecvFlag:
                    {
                        if (data.DMEObject != null)
                            data.DMEObject.RecvFlag = setRecvFlag.Flag;
                        break;
                    }
                case RT_MSG_CLIENT_SET_AGG_TIME setAggTime:
                    {
                        LoggerAccessor.LogInfo($"rt_msg_server_process_client_set_agg_time_msg: new agg time = {setAggTime.AggTime}");
                        List<int> preClientObject = new() { 10952, 10954, 10130 };

                        if (data.DMEObject != null && preClientObject.Contains(scertClient.ApplicationID))
                            data.DMEObject.AggTimeMs = setAggTime.AggTime; //Else we don't set AggTime here YET, the client object isn't created! for Pre-108 clients
                        break;
                    }
                case RT_MSG_CLIENT_FLUSH_ALL flushAll:
                    {

                        return;
                    }

                case RT_MSG_CLIENT_TIMEBASE_QUERY timebaseQuery:
                    {
                        if (data.DMEObject != null && data.DMEObject.DmeWorld != null)
                        {
                            RT_MSG_SERVER_TIMEBASE_QUERY_NOTIFY timebaseQueryNotifyMessage = new()
                            {
                                ClientTime = timebaseQuery.Timestamp,
                                ServerTime = (uint)data.DMEObject.DmeWorld.WorldTimer.ElapsedMilliseconds
                            };

                            //if (data.DMEObject?.Udp != null && data.DMEObject.RemoteUdpEndpoint != null)
                            //{
                            //    await data.DMEObject.Udp.SendImmediate(timebaseQueryNotifyMessage);
                            //}
                            //else
                            //{
                            //    await clientChannel.WriteAndFlushAsync(timebaseQueryNotifyMessage);
                            //}

                            await clientChannel.WriteAndFlushAsync(timebaseQueryNotifyMessage);
                            //await clientChannel.WriteAndFlushAsync(new RT_MSG_SERVER_TIMEBASE_QUERY_NOTIFY()
                            //{
                            //    ClientTime = timebaseQuery.Timestamp,
                            //    ServerTime = (uint)data.DMEObject.DmeWorld.WorldTimer.ElapsedMilliseconds
                            //});
                            //Queue(new RT_MSG_SERVER_TIMEBASE_QUERY_NOTIFY()
                            //{
                            //    ClientTime = timebaseQuery.Timestamp,
                            //    ServerTime = (uint)data.DMEObject.DmeWorld.WorldTimer.ElapsedMilliseconds
                            //}, clientChannel);
                        }
                        break;
                    }
                case RT_MSG_CLIENT_TOKEN_MESSAGE tokenMessage:
                    {
                        await ProcessRTTHostTokenMessage(tokenMessage, clientChannel, data);
                        break;
                    }
                case RT_MSG_CLIENT_APP_BROADCAST clientAppBroadcast:
                    {
                        data.DMEObject?.DmeWorld?.BroadcastTcp(data.DMEObject, clientAppBroadcast.Payload);
                        break;
                    }
                case RT_MSG_CLIENT_APP_LIST clientAppList:
                    {
                        data.DMEObject?.DmeWorld?.SendTcpAppList(data.DMEObject, clientAppList.Targets, clientAppList.Payload ?? Array.Empty<byte>());
                        break;
                    }
                case RT_MSG_CLIENT_APP_SINGLE clientAppSingle:
                    {
                        if (data.DMEObject != null)
                        {
                            bool InvalidatedRequest = false;

                            if (data.DMEObject.ApplicationId == 20371 || data.DMEObject.ApplicationId == 20374)
                            {
                                string? HomeUserEntry = null;
                                ClientObject? mumClient = MediusClass.Manager.GetClientBySessionKey(data.DMEObject.SessionKey, data.DMEObject.ApplicationId);

                                if (mumClient != null)
                                    HomeUserEntry = mumClient.AccountName + ":" + mumClient.IP;

                                if (clientAppSingle.Payload.Length > 8)
                                {
                                    byte[] HubMessagePayload = clientAppSingle.Payload;
                                    int HubPathernOffset = ByteUtils.FindBytePattern(HubMessagePayload, new byte[] { 0x64, 0x00 });

                                    if (HubPathernOffset != -1 && HubMessagePayload.Length >= HubPathernOffset + 8) // Hub command.
                                    {
                                        string? value;

                                        switch (BitConverter.IsLittleEndian ? EndianUtils.ReverseInt(BitConverter.ToInt32(HubMessagePayload, HubPathernOffset + 4)) : BitConverter.ToInt32(HubMessagePayload, HubPathernOffset + 4))
                                        {
                                            case -85: // IGA
                                                if (!string.IsNullOrEmpty(HomeUserEntry) && MediusClass.Settings.PlaystationHomeUsersServersAccessList.TryGetValue(HomeUserEntry, out value) && !string.IsNullOrEmpty(value))
                                                {
                                                    switch (value)
                                                    {
                                                        case "ADMIN":
                                                        case "IGA":
                                                            break;
                                                        default:
                                                            InvalidatedRequest = true;

                                                            LoggerAccessor.LogError($"[DME] - TcpServer - HOME ANTI-CHEAT - DETECTED MALICIOUS USAGE (Reason: UNAUTHORISED IGA COMMAND) - DmeId:{data.DMEObject.DmeId}");

                                                            await clientChannel.CloseAsync();
                                                            break;
                                                    }
                                                }
                                                else
                                                {
                                                    InvalidatedRequest = true;
                                                    string SupplementalMessage = "Unknown";

                                                    switch (HubMessagePayload[HubPathernOffset + 3]) // TODO, add all the other codes.
                                                    {
                                                        case 0x0B:
                                                            SupplementalMessage = "Kick";
                                                            break;
                                                    }

                                                    LoggerAccessor.LogError($"[DME] - TcpServer - HOME ANTI-CHEAT - DETECTED MALICIOUS USAGE (Reason: UNAUTHORISED IGA COMMAND - {SupplementalMessage}) - DmeId:{data.DMEObject.DmeId}");

                                                    await clientChannel.CloseAsync();
                                                }
                                                break;
                                            case -27: // REXEC
                                                if (!string.IsNullOrEmpty(HomeUserEntry) && MediusClass.Settings.PlaystationHomeUsersServersAccessList.TryGetValue(HomeUserEntry, out value) && !string.IsNullOrEmpty(value))
                                                {
                                                    switch (value)
                                                    {
                                                        case "ADMIN":
                                                            break;
                                                        default:
                                                            InvalidatedRequest = true;

                                                            LoggerAccessor.LogError($"[DME] - TcpServer - HOME ANTI-CHEAT - DETECTED MALICIOUS USAGE (Reason: UNAUTHORISED REXEC COMMAND) - DmeId:{data.DMEObject.DmeId}");

                                                            await clientChannel.CloseAsync();
                                                            break;
                                                    }
                                                }
                                                else
                                                {
                                                    InvalidatedRequest = true;

                                                    LoggerAccessor.LogError($"[DME] - TcpServer - HOME ANTI-CHEAT - DETECTED MALICIOUS USAGE (Reason: UNAUTHORISED REXEC COMMAND) - DmeId:{data.DMEObject.DmeId}");

                                                    await clientChannel.CloseAsync();
                                                }
                                                break;
                                        }
                                    }
                                }
                            }

                            if (!InvalidatedRequest)
                                data.DMEObject.DmeWorld?.SendTcpAppSingle(data.DMEObject, clientAppSingle.TargetOrSource, clientAppSingle.Payload ?? Array.Empty<byte>());
                        }
                        break;
                    }
                case RT_MSG_CLIENT_APP_TOSERVER clientAppToServer:
                    {
                        if (clientAppToServer.Message != null)
                            await ProcessMediusMessage(clientAppToServer.Message, clientChannel, data);
                        break;
                    }

                case RT_MSG_CLIENT_DISCONNECT _:
                case RT_MSG_CLIENT_DISCONNECT_WITH_REASON _:
                    {
                        _ = clientChannel.CloseAsync();
                        break;
                    }
                default:
                    {
                        LoggerAccessor.LogWarn($"UNHANDLED RT MESSAGE: {message}");

                        break;
                    }
            }

            return;
        }

        protected virtual Task ProcessMediusMessage(BaseMediusMessage message, IChannel clientChannel, ChannelData data)
        {
            if (message == null)
                return Task.CompletedTask;

            switch (message)
            {
                case TypePing ping:
                    {
#if DEBUG
                        LoggerAccessor.LogInfo($"PingPacketHandler: client {data.DMEObject} received");
#endif
                        if (ping.RequestEcho)
                        {
                            byte[] value = new byte[0xA];
                            Queue(new RT_MSG_CLIENT_ECHO()
                            {
                                Value = value
                            }, clientChannel);
                            break;
                        }

                        data.DMEObject?.EnqueueTcp(new RT_MSG_SERVER_APP() { 
                            Message = new TypePing()
                            {
                                TimeOfSend = DateTimeUtils.GetUnixTimeU32(),
                                PingInstance = ping.PingInstance,
                                RequestEcho = ping.RequestEcho
                            }
                        });

                        break;
                    }
            }

            return Task.CompletedTask;
        }

        protected virtual Task ProcessRTTHostTokenMessage(RT_MSG_CLIENT_TOKEN_MESSAGE clientTokenMsg, IChannel clientChannel, ChannelData data)
        {
#if DEBUG
            LoggerAccessor.LogInfo($"[DME] - TcpServer - ProcessRTTHostTokenMessage: rt_msg_server_process_client_token_msg: msg type {clientTokenMsg.RT_TOKEN_MESSAGE_TYPE}, client {data.DMEObject?.ScertId}, target token = {clientTokenMsg.targetToken}");
#endif
            bool isTokenValid = rt_token_is_valid(clientTokenMsg.targetToken);

            if (!isTokenValid)
                LoggerAccessor.LogWarn($"[DME] - TcpServer - ProcessRTTHostTokenMessage: rt_msg_server_process_client_token_msg: bad target token {clientTokenMsg.targetToken}");
            else
            {
                switch (clientTokenMsg.RT_TOKEN_MESSAGE_TYPE)
                {
                    case RT_TOKEN_MESSAGE_TYPE.RT_TOKEN_CLIENT_REQUEST:
                        {
                            if (data.DMEObject != null && data.DMEObject.DmeWorld != null)
                            {
                                lock (data.DMEObject.DmeWorld.clientTokens)
                                {
                                    if (!data.DMEObject.DmeWorld.clientTokens.ContainsKey(clientTokenMsg.targetToken))
                                    {
                                        data.DMEObject.DmeWorld.clientTokens.TryAdd(clientTokenMsg.targetToken, new List<int>() { data.DMEObject.DmeId });

                                        if (data.DMEObject.DmeWorld.clientTokens[clientTokenMsg.targetToken].Count > 0)
                                            data.DMEObject.DmeWorld.BroadcastTcpScertMessage(new RT_MSG_SERVER_TOKEN_MESSAGE() // We need to broadcast the signal that this token is owned.
                                            {
                                                TokenList = new List<(RT_TOKEN_MESSAGE_TYPE, ushort, ushort)> { (RT_TOKEN_MESSAGE_TYPE.RT_TOKEN_SERVER_OWNED, clientTokenMsg.targetToken, (ushort)data.DMEObject.DmeWorld.clientTokens[clientTokenMsg.targetToken][0]) }
                                            });
                                        else
                                        {
                                            LoggerAccessor.LogError($"[DME] - TcpServer - ProcessRTTHostTokenMessage: Client {data.DMEObject?.IP} requested a token request but errored out while owning a token!");

                                            Queue(new RT_MSG_SERVER_FORCED_DISCONNECT()
                                            {
                                                Reason = SERVER_FORCE_DISCONNECT_REASON.SERVER_FORCED_DISCONNECT_ERROR
                                            }, clientChannel);
                                        }
                                    }
                                    else
                                    {
                                        data.DMEObject.DmeWorld.clientTokens[clientTokenMsg.targetToken].Add(data.DMEObject.DmeId);

                                        Queue(new RT_MSG_SERVER_TOKEN_MESSAGE() // This message should not be broadcasted, Home doesn't like it.
                                        {
                                            TokenList = new List<(RT_TOKEN_MESSAGE_TYPE, ushort, ushort)> { (RT_TOKEN_MESSAGE_TYPE.RT_TOKEN_SERVER_GRANTED, clientTokenMsg.targetToken, 0) }
                                        }, clientChannel);
                                    }
                                }
                            }
                            else
                            {
                                LoggerAccessor.LogError($"[DME] - TcpServer - ProcessRTTHostTokenMessage: Client {data.DMEObject?.IP} requested a token request without being in a DmeWorld!");

                                Queue(new RT_MSG_SERVER_FORCED_DISCONNECT()
                                {
                                    Reason = SERVER_FORCE_DISCONNECT_REASON.SERVER_FORCED_DISCONNECT_ERROR
                                }, clientChannel);
                            }

                            break;
                        }

                    case RT_TOKEN_MESSAGE_TYPE.RT_TOKEN_CLIENT_RELEASE:
                        {
                            if (data.DMEObject != null && data.DMEObject.DmeWorld != null)
                            {
                                lock (data.DMEObject.DmeWorld.clientTokens)
                                {
                                    if (data.DMEObject.DmeWorld.clientTokens.TryGetValue(clientTokenMsg.targetToken, out List<int>? value) && value != null)
                                    {
                                        if (value.Contains(data.DMEObject.DmeId))
                                        {
                                            if (value.IndexOf(data.DMEObject.DmeId) == 0)
                                            {
                                                data.DMEObject.DmeWorld.clientTokens.Remove(clientTokenMsg.targetToken, out _);

                                                data.DMEObject.DmeWorld.BroadcastTcpScertMessage(new RT_MSG_SERVER_TOKEN_MESSAGE()
                                                {
                                                    TokenList = new List<(RT_TOKEN_MESSAGE_TYPE, ushort, ushort)> { (RT_TOKEN_MESSAGE_TYPE.RT_TOKEN_SERVER_FREED, clientTokenMsg.targetToken, 0) }
                                                });
                                            }
                                            else
                                            {
                                                value.Remove(data.DMEObject.DmeId);

                                                Queue(new RT_MSG_SERVER_TOKEN_MESSAGE()
                                                {
                                                    TokenList = new List<(RT_TOKEN_MESSAGE_TYPE, ushort, ushort)> { (RT_TOKEN_MESSAGE_TYPE.RT_TOKEN_SERVER_RELEASED, clientTokenMsg.targetToken, 0) }
                                                }, clientChannel);
                                            }
                                        }
                                        else
                                            Queue(new RT_MSG_SERVER_TOKEN_MESSAGE()
                                            {
                                                TokenList = new List<(RT_TOKEN_MESSAGE_TYPE, ushort, ushort)> { (RT_TOKEN_MESSAGE_TYPE.RT_TOKEN_SERVER_RELEASED, clientTokenMsg.targetToken, 0) }
                                            }, clientChannel);
                                    }
                                    else
                                        Queue(new RT_MSG_SERVER_TOKEN_MESSAGE()
                                        {
                                            TokenList = new List<(RT_TOKEN_MESSAGE_TYPE, ushort, ushort)> { (RT_TOKEN_MESSAGE_TYPE.RT_TOKEN_SERVER_OWNER_REMOVED, 0, 0) }
                                        }, clientChannel);
                                }

                                // Hotfix the arcade cabinets MLAA enabling in PS Home.
                                ClientObject? mumClient = MediusClass.Manager.GetClientBySessionKey(data.DMEObject.SessionKey, data.DMEObject.ApplicationId);

                                if (mumClient != null && (mumClient.ApplicationId == 20371 || mumClient.ApplicationId == 20374) && mumClient.IsOnRPCN && mumClient.ClientHomeData != null && mumClient.ClientHomeData.VersionAsDouble >= 01.83)
                                    _ = HomeRTMTools.SendRemoteCommand(mumClient, "lc Debug.System( 'mlaaenable 0' )");
                            }
                            else
                            {
                                LoggerAccessor.LogError($"[DME] - TcpServer - ProcessRTTHostTokenMessage: Client {data.DMEObject?.IP} requested a token release without being in a DmeWorld!");

                                Queue(new RT_MSG_SERVER_FORCED_DISCONNECT()
                                {
                                    Reason = SERVER_FORCE_DISCONNECT_REASON.SERVER_FORCED_DISCONNECT_ERROR
                                }, clientChannel);
                            }

                            break;
                        }

                    default:
                        {
                            LoggerAccessor.LogWarn($"UNHANDLED RT TOKEN MESSAGE: {clientTokenMsg.RT_TOKEN_MESSAGE_TYPE}");
                            break;
                        }
                }
            }

            return Task.CompletedTask;
        }

        #endregion

        #region Channel

        /// <summary>
        /// Closes the client channel.
        /// </summary>
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
            catch
            {
                // Silence exception since the client probably just closed the socket before we could write to it
            }
        }

        #endregion

        #region Queue

        public void Queue(BaseScertMessage message, params IChannel[] clientChannels)
        {
            Queue(message, (IEnumerable<IChannel>)clientChannels);
        }

        public void Queue(BaseScertMessage message, IEnumerable<IChannel> clientChannels)
        {
            foreach (var clientChannel in clientChannels)
                if (clientChannel != null)
                    if (_channelDatas.TryGetValue(clientChannel.Id.AsLongText(), out var data))
                        data.SendQueue.Enqueue(message);
        }

        public void Queue(IEnumerable<BaseScertMessage> messages, params IChannel[] clientChannels)
        {
            Queue(messages, (IEnumerable<IChannel>)clientChannels);
        }

        public void Queue(IEnumerable<BaseScertMessage> messages, IEnumerable<IChannel> clientChannels)
        {
            foreach (var clientChannel in clientChannels)
                if (clientChannel != null)
                    if (_channelDatas.TryGetValue(clientChannel.Id.AsLongText(), out var data))
                        foreach (var message in messages)
                            data.SendQueue.Enqueue(message);
        }

        #endregion

        #region Plugins

        protected async Task<bool> PassMessageToPlugins(IChannel clientChannel, ChannelData data, BaseScertMessage message, bool isIncoming)
        {
            var onMsg = new OnMessageArgs(isIncoming)
            {
                Player = data.DMEObject,
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
                    Player = data.DMEObject,
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
                    Player = data.DMEObject,
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

        #endregion

        public DMEObject? GetClientByScertId(ushort scertId)
        {
            if (_scertIdToClient.TryGetValue(scertId, out var result))
                return result;

            return null;
        }

        protected ushort GenerateNewScertClientId()
        {
            return _clientCounter++;
        }

        public bool rt_token_is_valid(ushort TokenId)
        {
            return TokenId <= 65534;
        }
    }
}
