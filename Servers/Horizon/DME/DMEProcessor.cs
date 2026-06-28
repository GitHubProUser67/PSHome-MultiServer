using System.Collections.Concurrent;
using System.Net;
using System.Runtime.CompilerServices;
using CustomLogger;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using EndianTools;
using Horizon.COMMON.Extensions.PSHome;
using Horizon.CustomServers;
using Horizon.DME.Models;
using Horizon.DME.PluginArgs;
using Horizon.LIBRARY.Pipeline.Tcp;
using Horizon.MEDIUS.Extensions.PSHome;
using Horizon.MEDIUS.Processors;
using Horizon.MUM.Models;
using Horizon.PluginManager;
using Horizon.RT.Common;
using Horizon.RT.Cryptography;
using Horizon.RT.Models;
using MultiServerLibrary.Extension;

namespace Horizon.DME
{
    public class DMEProcessor
    {
        public readonly DNTCPHybridServer _DMEServer = new();

        private ushort _clientCounter = 0;

        protected ConcurrentQueue<IChannel> _forceDisconnectQueue = new();
        protected ConcurrentDictionary<string, DMEChannelData> _channelDatas = new();
        protected ConcurrentDictionary<uint, DMEObject> _scertIdToClient = new();

        public Task StartAsync(int maxConcurrentListeners = 10)
        {
            return Task.Run(() =>
            {
                _DMEServer.Start(
                    new Dictionary<ushort, bool>
                    {
                        { HorizonServerConfiguration.DMETCPPort, true },
                    },
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
                        pipeline.AddLast(_DMEServer.ScertHandler);
                    }),
                    null,
                    (channel) =>
                    {
                        _channelDatas.TryAdd(channel.Id.AsLongText(), new DMEChannelData());
                    },
                    (channel) =>
                    {
                        if (_channelDatas.TryRemove(channel.Id.AsLongText(), out var data))
                        {
                            if (data.DMEObject != null)
                            {
                                data.DMEObject.OnTcpDisconnected();
                                _scertIdToClient.TryRemove(data.DMEObject.ScertId, out _);
                            }
                        }
                    },
                    (channel, messageObj) =>
                    {
                        BaseScertMessage message = (BaseScertMessage)messageObj;

                        var key = channel.Id.AsLongText();
                        if (_channelDatas.TryGetValue(key, out var data))
                        {
                            if (
                                !data.Ignore
                                && (data.DMEObject == null || !data.DMEObject.IsDestroyed)
                            )
                            {
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
                            LoggerAccessor.LogInfo($"[DMEProcessor] - RECV {channel}: {message}");
                    }
                );
            });
        }

        /// <summary>
        /// Gets DME Server client object.
        /// </summary>
        public DMEObject? GetServerPerAppId(int ApplicationId)
        {
            return _channelDatas
                .Values.Where(channel => channel.ApplicationId == ApplicationId)
                .FirstOrDefault()
                ?.DMEObject;
        }

        /// <summary>
        /// Process incoming messages.
        /// </summary>
        public async Task HandleIncomingMessages()
        {
            if (_DMEServer.ScertHandler != null)
                await Task.WhenAll(
                        _DMEServer.ScertHandler.Channels.Select(HandleIncomingMessages).ToArray()
                    )
                    .ConfigureAwait(false);
        }

        /// <summary>
        /// Process outgoing messages.
        /// </summary>
        public async Task HandleOutgoingMessages()
        {
            if (_DMEServer.ScertHandler == null)
                return;

            await Task.WhenAll(
                    _DMEServer.ScertHandler.Channels.Select(HandleOutgoingMessages).ToArray()
                )
                .ConfigureAwait(false);

            // Disconnect and remove timedout unauthenticated channels
            while (_forceDisconnectQueue.TryDequeue(out var channel))
            {
                // Remove
                _channelDatas.TryRemove(channel.Id.AsLongText(), out var d);

                LoggerAccessor.LogWarn(
                    $"[DMEProcessor] - REMOVING CHANNEL {channel},{d},{d?.DMEObject}"
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
        }

        private async Task HandleIncomingMessages(IChannel clientChannel)
        {
            if (clientChannel == null)
                return;

            try
            {
                if (_channelDatas.TryGetValue(clientChannel.Id.AsLongText(), out var data))
                {
                    // Process all messages in queue
                    while (data.RecvQueue.TryDequeue(out var message))
                    {
                        try
                        {
                            if (
                                !await PassMessageToPlugins(clientChannel, data, message, true)
                                    .ConfigureAwait(false)
                            )
                                await ProcessMessage(message, clientChannel, data)
                                    .ConfigureAwait(false);
                        }
                        catch (Exception e)
                        {
                            LoggerAccessor.LogError(
                                $"[DMEProcessor] - clientChannel ticking thrown an assertion while processing the message queue. (Exception:{e})"
                            );

                            _ = ForceDisconnectClient(clientChannel);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(
                    $"[DMEProcessor] - clientChannel ticking thrown an assertion. (Exception:{e})"
                );
            }
        }

        private async Task HandleOutgoingMessages(IChannel clientChannel)
        {
            if (clientChannel == null)
                return;

            List<BaseScertMessage> responses = new();

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
                            if (
                                !await PassMessageToPlugins(clientChannel, data, message, false)
                                    .ConfigureAwait(false)
                            )
                                responses.Add(message);

                        if (data.DMEObject != null)
                        {
                            // Echo
                            if (
                                data.DMEObject.MediusVersion > 108
                                && (
                                    DateTimeUtils.GetHighPrecisionUtcTime()
                                    - data.DMEObject.UtcLastServerEchoSent
                                ).TotalSeconds
                                    > DATABASE
                                        .DatabaseManager.GetAppSettingsOrDefault(
                                            data.DMEObject.ApplicationId
                                        )
                                        .ServerEchoIntervalSeconds
                            )
                            {
                                var message = new RT_MSG_SERVER_ECHO();
                                if (
                                    !await PassMessageToPlugins(clientChannel, data, message, false)
                                        .ConfigureAwait(false)
                                )
                                    responses.Add(message);
                                data.DMEObject.UtcLastServerEchoSent =
                                    DateTimeUtils.GetHighPrecisionUtcTime();
                            }

                            // Add client object's send queue to responses
                            // But only if not in a world
                            if (
                                data.DMEObject.DmeWorld == null
                                || data.DMEObject.DmeWorld.Destroyed
                            )
                                while (
                                    data.DMEObject.TcpSendMessageQueue.TryDequeue(out var message)
                                )
                                    if (
                                        !await PassMessageToPlugins(
                                                clientChannel,
                                                data,
                                                message,
                                                false
                                            )
                                            .ConfigureAwait(false)
                                    )
                                        responses.Add(message);
                        }

                        if (responses.Count > 0)
                            _ = clientChannel.WriteAndFlushAsync(responses);
                    }
                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(
                    $"[DMEProcessor] - clientChannel ticking thrown an assertion. (Exception:{e})"
                );
            }
        }

        #region Message Processing

        protected async Task ProcessMessage(
            BaseScertMessage message,
            IChannel clientChannel,
            DMEChannelData data
        )
        {
            // Get ScertClient data
            var scertClient = clientChannel
                .GetAttribute(LIBRARY.Pipeline.Constants.SCERT_CLIENT)
                .Get();
            var enableEncryption = DATABASE
                .DatabaseManager.GetAppSettingsOrDefault(data.ApplicationId)
                .EnableDmeEncryption;
            if (scertClient.CipherService != null)
                scertClient.CipherService.EnableEncryption = enableEncryption;

            switch (message)
            {
                case RT_MSG_CLIENT_HELLO clientHello:
                {
                    // send hello
                    Queue(
                        new RT_MSG_SERVER_HELLO()
                        {
                            RsaPublicKey = enableEncryption
                                ? LIBRARY
                                    .Pipeline
                                    .Attribute
                                    .ScertClientAttribute
                                    .DefaultRsaAuthKey
                                    .N
                                : Org.BouncyCastle.Math.BigInteger.Zero,
                        },
                        clientChannel
                    );
                    break;
                }
                case RT_MSG_CLIENT_CRYPTKEY_PUBLIC clientCryptKeyPublic:
                {
                    if (clientCryptKeyPublic.PublicKey != null)
                    {
                        // generate new client session key
                        scertClient.CipherService?.GenerateCipher(
                            CipherContext.RSA_AUTH,
                            clientCryptKeyPublic.PublicKey.ReverseArray()
                        );
                        scertClient.CipherService?.GenerateCipher(CipherContext.RC_CLIENT_SESSION);

                        Queue(
                            new RT_MSG_SERVER_CRYPTKEY_PEER()
                            {
                                SessionKey = scertClient.CipherService?.GetPublicKey(
                                    CipherContext.RC_CLIENT_SESSION
                                ),
                            },
                            clientChannel
                        );
                    }
                    break;
                }
                case RT_MSG_CLIENT_CONNECT_TCP_AUX_UDP clientConnectTcpAuxUdp:
                {
                    var appid = clientConnectTcpAuxUdp.AppId;

                    if (appid < 0)
                    {
                        LoggerAccessor.LogError(
                            $"[DMEProcessor] - Client Connected {clientChannel.RemoteAddress} with an invalid connect payload!"
                        );
                        break;
                    }

                    if (clientConnectTcpAuxUdp.Key == RSA_KEY.Empty)
                    {
                        LoggerAccessor.LogError(
                            $"[DMEProcessor] - Client Connected {clientChannel.RemoteAddress} with an empty key!"
                        );
                        break;
                    }

                    ClientObject? mumClient;

                    data.ApplicationId = appid;
                    scertClient.ApplicationID = appid;

                    var targetChannel = Program.MUMManager.GetChannelByChannelId(
                        clientConnectTcpAuxUdp.TargetWorldId,
                        data.ApplicationId
                    );

                    if (targetChannel == null)
                    {
                        var DefaultChannel = Program.MUMManager.GetOrCreateDefaultLobbyChannel(
                            data.ApplicationId,
                            scertClient.MediusVersion!.Value
                        );

                        if (DefaultChannel.Id == clientConnectTcpAuxUdp.TargetWorldId)
                            targetChannel = DefaultChannel;

                        if (targetChannel == null)
                        {
                            LoggerAccessor.LogError(
                                $"[DMEProcessor] - Client: {clientConnectTcpAuxUdp.AccessToken} tried to join, but targetted WorldId:{clientConnectTcpAuxUdp.TargetWorldId} doesn't exist!"
                            );
                            _ = clientChannel.CloseAsync();
                            break;
                        }
                    }

                    // If booth are null, it means DME client wants a new object.
                    if (
                        !string.IsNullOrEmpty(clientConnectTcpAuxUdp.AccessToken)
                        && !string.IsNullOrEmpty(clientConnectTcpAuxUdp.SessionKey)
                    )
                    {
                        mumClient = Program.MUMManager.GetClientByAccessToken(
                            clientConnectTcpAuxUdp.AccessToken,
                            appid
                        );
                        mumClient ??= Program.MUMManager.GetClientBySessionKey(
                            clientConnectTcpAuxUdp.SessionKey,
                            appid
                        );

                        if (mumClient != null)
                            LoggerAccessor.LogInfo(
                                $"[DMEProcessor] - Client Connected {clientChannel.RemoteAddress}:{data.DMEObject}: {clientChannel}"
                            );
                        else
                        {
                            data.Ignore = true;
                            LoggerAccessor.LogError(
                                $"[DMEProcessor] - ClientObject could not be granted for {clientChannel.RemoteAddress}:{data.DMEObject}: {clientConnectTcpAuxUdp}"
                            );
                            break;
                        }

                        mumClient.MediusVersion = scertClient.MediusVersion ?? 0;
                        mumClient.ApplicationId = appid;
                        mumClient.OnConnected();
                    }
                    else
                    {
                        data.Ignore = true;
                        LoggerAccessor.LogError(
                            $"[DMEProcessor] - ClientObject could not be found for {clientChannel.RemoteAddress}:{data.DMEObject}: {clientConnectTcpAuxUdp}"
                        );
                        break;
                    }

                    await mumClient.JoinChannel(targetChannel).ConfigureAwait(false);

                    if (
                        !string.IsNullOrEmpty(clientConnectTcpAuxUdp.AccessToken)
                        && !string.IsNullOrEmpty(clientConnectTcpAuxUdp.SessionKey)
                    )
                    {
                        data.DMEObject = Program.DmeManager.GetMPSClientByAccessToken(
                            clientConnectTcpAuxUdp.AccessToken
                        );
                        data.DMEObject ??= Program.DmeManager.GetMPSClientBySessionKey(
                            clientConnectTcpAuxUdp.SessionKey
                        );

                        if (data.DMEObject != null)
                            LoggerAccessor.LogInfo(
                                $"[DMEProcessor] - DMEClient Connected {clientChannel.RemoteAddress}:{data.DMEObject}: {clientChannel}"
                            );
                        else
                        {
                            data.Ignore = true;
                            LoggerAccessor.LogError(
                                $"[DMEProcessor] - DMEClientObject could not be granted for {clientChannel.RemoteAddress}:{data.DMEObject}: {clientConnectTcpAuxUdp}"
                            );
                            break;
                        }
                    }
                    else // MAG uses DME TCP directly to register a ClientObject.
                    {
                        LoggerAccessor.LogInfo(
                            $"[DMEProcessor] - DMEClient Connected {clientChannel.RemoteAddress} with new ClientObject!"
                        );

                        data.DMEObject = new DMEObject(
                            clientConnectTcpAuxUdp.SessionKey,
                            mumClient
                        );
                    }

                    data.DMEObject.ApplicationId = appid;
                    data.DMEObject.OnTcpConnected(clientChannel);
                    data.DMEObject.ScertId = GenerateNewScertClientId();
                    data.DMEObject.MediusVersion = scertClient.MediusVersion;
                    data.DMEObject.CryptoContext = scertClient.CipherService;

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
                        List<int> ConnectAcceptTCPGames = new() { };

                        if (ConnectAcceptTCPGames.Contains(data.ApplicationId))
                        {
                            Queue(
                                new RT_MSG_SERVER_CONNECT_ACCEPT_TCP()
                                {
                                    PlayerId = (ushort)data.DMEObject.DmeId,
                                    ScertId = data.DMEObject.ScertId,
                                    PlayerCount =
                                        (ushort?)data.DMEObject.DmeWorld?.Clients.Length ?? 0x0001,
                                    IP = (clientChannel.RemoteAddress as IPEndPoint)?.Address,
                                },
                                clientChannel
                            );
                        }
                        else
                            Queue(
                                new RT_MSG_SERVER_CONNECT_REQUIRE()
                                {
                                    MaxPacketSize = Constants.MEDIUS_MESSAGE_MAXLEN,
                                    MaxUdpPacketSize = Constants.MEDIUS_UDP_MESSAGE_MAXLEN,
                                },
                                clientChannel
                            );
                    }
                    #endregion
                    else if (scertClient.MediusVersion > 108 && scertClient.ApplicationID != 11484)
                        Queue(
                            new RT_MSG_SERVER_CONNECT_REQUIRE()
                            {
                                MaxPacketSize = Constants.MEDIUS_MESSAGE_MAXLEN,
                                MaxUdpPacketSize = Constants.MEDIUS_UDP_MESSAGE_MAXLEN,
                            },
                            clientChannel
                        );
                    else
                    {
                        if (
                            scertClient.CipherService != null
                            && scertClient.CipherService.HasKey(CipherContext.RC_CLIENT_SESSION)
                            && scertClient.MediusVersion >= 109
                            && !scertClient.IsPS3Client
                        )
                            Queue(
                                new RT_MSG_SERVER_CRYPTKEY_GAME()
                                {
                                    GameKey = scertClient.CipherService.GetPublicKey(
                                        CipherContext.RC_CLIENT_SESSION
                                    ),
                                },
                                clientChannel
                            );
                        Queue(
                            new RT_MSG_SERVER_CONNECT_ACCEPT_TCP()
                            {
                                PlayerId = (ushort)data.DMEObject.DmeId,
                                ScertId = data.DMEObject.ScertId,
                                PlayerCount =
                                    (ushort?)data.DMEObject.DmeWorld?.Clients.Length ?? 0x0001,
                                IP = (clientChannel.RemoteAddress as IPEndPoint)?.Address,
                            },
                            clientChannel
                        );

                        await InternetProtocolUtils
                            .TryGetServerIP(out string dmeIp)
                            .ConfigureAwait(false);

                        Queue(
                            new RT_MSG_SERVER_INFO_AUX_UDP()
                            {
                                Ip = string.IsNullOrEmpty(dmeIp)
                                    ? IPAddress.Any
                                    : IPAddress.Parse(dmeIp),
                                Port = (ushort)data.DMEObject.UdpPort,
                            },
                            clientChannel
                        );
                    }

                    break;
                }
                case RT_MSG_CLIENT_CONNECT_TCP clientConnectTcp:
                {
                    var appid = clientConnectTcp.AppId;

                    if (appid < 0)
                    {
                        LoggerAccessor.LogError(
                            $"[DMEProcessor] - Client Connected {clientChannel.RemoteAddress} with an invalid connect payload!"
                        );
                        break;
                    }

                    if (clientConnectTcp.Key == RSA_KEY.Empty)
                    {
                        LoggerAccessor.LogError(
                            $"[DMEProcessor] - Client Connected {clientChannel.RemoteAddress} with an empty key!"
                        );
                        break;
                    }

                    ClientObject? mumClient;

                    data.ApplicationId = appid;
                    scertClient.ApplicationID = appid;

                    var targetChannel = Program.MUMManager.GetChannelByChannelId(
                        clientConnectTcp.TargetWorldId,
                        data.ApplicationId
                    );

                    if (targetChannel == null)
                    {
                        var DefaultChannel = Program.MUMManager.GetOrCreateDefaultLobbyChannel(
                            data.ApplicationId,
                            scertClient.MediusVersion!.Value
                        );

                        if (DefaultChannel.Id == clientConnectTcp.TargetWorldId)
                            targetChannel = DefaultChannel;

                        if (targetChannel == null)
                        {
                            LoggerAccessor.LogError(
                                $"[DMEProcessor] - Client: {clientConnectTcp.AccessToken} tried to join, but targetted WorldId:{clientConnectTcp.TargetWorldId} doesn't exist!"
                            );
                            _ = clientChannel.CloseAsync();
                            break;
                        }
                    }

                    // If booth are null, it means DME client wants a new object.
                    if (
                        !string.IsNullOrEmpty(clientConnectTcp.AccessToken)
                        && !string.IsNullOrEmpty(clientConnectTcp.SessionKey)
                    )
                    {
                        mumClient = Program.MUMManager.GetClientByAccessToken(
                            clientConnectTcp.AccessToken,
                            appid
                        );
                        mumClient ??= Program.MUMManager.GetClientBySessionKey(
                            clientConnectTcp.SessionKey,
                            appid
                        );

                        if (mumClient != null)
                            LoggerAccessor.LogInfo(
                                $"[DMEProcessor] - Client Connected {clientChannel.RemoteAddress}:{data.DMEObject}: {clientChannel}"
                            );
                        else
                        {
                            data.Ignore = true;
                            LoggerAccessor.LogError(
                                $"[DMEProcessor] - ClientObject could not be granted for {clientChannel.RemoteAddress}:{data.DMEObject}: {clientConnectTcp}"
                            );
                            break;
                        }

                        mumClient.MediusVersion = scertClient.MediusVersion ?? 0;
                        mumClient.ApplicationId = appid;
                        mumClient.OnConnected();
                    }
                    else // MAG uses DME directly to register a ClientObject.
                    {
                        LoggerAccessor.LogInfo(
                            $"[DMEProcessor] - Client Connected {clientChannel.RemoteAddress} with new ClientObject!"
                        );

                        mumClient = new(scertClient.MediusVersion ?? 0) { ApplicationId = appid };
                        mumClient.OnConnected();

                        MAS.ReserveClient(mumClient); // ONLY RESERVE CLIENTS HERE!
                    }

                    await mumClient.JoinChannel(targetChannel).ConfigureAwait(false);

                    if (
                        !string.IsNullOrEmpty(clientConnectTcp.AccessToken)
                        && !string.IsNullOrEmpty(clientConnectTcp.SessionKey)
                    )
                    {
                        data.DMEObject = Program.DmeManager.GetMPSClientByAccessToken(
                            clientConnectTcp.AccessToken
                        );
                        data.DMEObject ??= Program.DmeManager.GetMPSClientBySessionKey(
                            clientConnectTcp.SessionKey
                        );

                        if (data.DMEObject != null)
                            LoggerAccessor.LogInfo(
                                $"[DMEProcessor] - DMEClient Connected {clientChannel.RemoteAddress}:{data.DMEObject}: {clientChannel}"
                            );
                        else
                        {
                            data.Ignore = true;
                            LoggerAccessor.LogError(
                                $"[DMEProcessor] - DMEClientObject could not be granted for {clientChannel.RemoteAddress}:{data.DMEObject}: {clientConnectTcp}"
                            );
                            break;
                        }
                    }
                    else // MAG uses DME TCP directly to register a ClientObject.
                    {
                        LoggerAccessor.LogInfo(
                            $"[DMEProcessor] - DMEClient Connected {clientChannel.RemoteAddress} with new ClientObject!"
                        );

                        data.DMEObject = new DMEObject(clientConnectTcp.SessionKey, mumClient);
                    }

                    data.DMEObject.ApplicationId = appid;
                    data.DMEObject.OnTcpConnected(clientChannel);
                    data.DMEObject.ScertId = GenerateNewScertClientId();
                    data.DMEObject.MediusVersion = scertClient.MediusVersion;
                    data.DMEObject.CryptoContext = scertClient.CipherService;

                    if (!_scertIdToClient.TryAdd(data.DMEObject.ScertId, data.DMEObject))
                    {
                        LoggerAccessor.LogWarn($"[DMEProcessor] - Duplicate scert client id");
                        break;
                    }

                    List<int> pre108ServerComplete = new() { 10130, 10442, 10721, 10536, 10538 };

                    #region if PS3
                    if (scertClient.IsPS3Client)
                    {
                        List<int> ConnectAcceptTCPGames = new() { };

                        //CAC & Warhawk
                        if (ConnectAcceptTCPGames.Contains(data.ApplicationId))
                        {
                            Queue(
                                new RT_MSG_SERVER_CONNECT_ACCEPT_TCP()
                                {
                                    PlayerId = (ushort)data.DMEObject.DmeId,
                                    ScertId = data.DMEObject.ScertId,
                                    PlayerCount =
                                        (ushort?)data.DMEObject.DmeWorld?.Clients.Length ?? 0x0001,
                                    IP = (clientChannel.RemoteAddress as IPEndPoint)?.Address,
                                },
                                clientChannel
                            );
                        }
                        else
                            Queue(
                                new RT_MSG_SERVER_CONNECT_REQUIRE()
                                {
                                    MaxPacketSize = Constants.MEDIUS_MESSAGE_MAXLEN,
                                    MaxUdpPacketSize = Constants.MEDIUS_UDP_MESSAGE_MAXLEN,
                                },
                                clientChannel
                            );
                    }
                    #endregion
                    else if (scertClient.MediusVersion > 108 && scertClient.ApplicationID != 11484)
                        Queue(
                            new RT_MSG_SERVER_CONNECT_REQUIRE()
                            {
                                MaxPacketSize = Constants.MEDIUS_MESSAGE_MAXLEN,
                                MaxUdpPacketSize = Constants.MEDIUS_UDP_MESSAGE_MAXLEN,
                            },
                            clientChannel
                        );
                    else
                    {
                        if (
                            scertClient.CipherService != null
                            && scertClient.CipherService.HasKey(CipherContext.RC_CLIENT_SESSION)
                            && scertClient.MediusVersion >= 109
                            && !scertClient.IsPS3Client
                        )
                            Queue(
                                new RT_MSG_SERVER_CRYPTKEY_GAME()
                                {
                                    GameKey = scertClient.CipherService.GetPublicKey(
                                        CipherContext.RC_CLIENT_SESSION
                                    ),
                                },
                                clientChannel
                            );
                        Queue(
                            new RT_MSG_SERVER_CONNECT_ACCEPT_TCP()
                            {
                                PlayerId = (ushort)data.DMEObject.DmeId,
                                ScertId = data.DMEObject.ScertId,
                                PlayerCount =
                                    (ushort?)data.DMEObject.DmeWorld?.Clients.Length ?? 0x0001,
                                IP = (clientChannel.RemoteAddress as IPEndPoint)?.Address,
                            },
                            clientChannel
                        );

                        if (pre108ServerComplete.Contains(data.ApplicationId))
                            Queue(
                                new RT_MSG_SERVER_CONNECT_COMPLETE()
                                {
                                    ClientCountAtConnect =
                                        (ushort?)data.DMEObject.DmeWorld?.Clients.Length ?? 0x0001,
                                },
                                clientChannel
                            );
                    }

                    break;
                }
                case RT_MSG_CLIENT_CONNECT_READY_REQUIRE clientConnectReadyRequire:
                {
                    if (
                        scertClient.CipherService != null
                        && scertClient.CipherService.HasKey(CipherContext.RC_CLIENT_SESSION)
                        && !scertClient.IsPS3Client
                    )
                        Queue(
                            new RT_MSG_SERVER_CRYPTKEY_GAME()
                            {
                                GameKey = scertClient.CipherService.GetPublicKey(
                                    CipherContext.RC_CLIENT_SESSION
                                ),
                            },
                            clientChannel
                        );
                    Queue(
                        new RT_MSG_SERVER_CONNECT_ACCEPT_TCP()
                        {
                            PlayerId = (ushort)data.DMEObject.DmeId,
                            ScertId = data.DMEObject.ScertId,
                            PlayerCount =
                                (ushort?)data.DMEObject.DmeWorld?.Clients.Length ?? 0x0001,
                            IP = (clientChannel.RemoteAddress as IPEndPoint)?.Address,
                        },
                        clientChannel
                    );
                    break;
                }
                case RT_MSG_CLIENT_CONNECT_READY_TCP clientConnectReadyTcp:
                {
                    if (data.DMEObject != null)
                    {
                        // Update recv flag
                        data.DMEObject.RecvFlag = clientConnectReadyTcp.RecvFlag;

                        Queue(
                            new RT_MSG_SERVER_STARTUP_INFO_NOTIFY()
                            {
                                GameHostType = (byte)
                                    MGCL_GAME_HOST_TYPE.MGCLGameHostClientServerAuxUDP,
                                Timebase =
                                    (uint?)data.DMEObject.DmeWorld?.WorldTimer.ElapsedMilliseconds
                                    ?? DateTimeUtils.GetUnixTimeU32(),
                            },
                            clientChannel
                        );

                        await InternetProtocolUtils
                            .TryGetServerIP(out string dmeIp)
                            .ConfigureAwait(false);

                        Queue(
                            new RT_MSG_SERVER_INFO_AUX_UDP()
                            {
                                Ip = string.IsNullOrEmpty(dmeIp)
                                    ? IPAddress.Any
                                    : IPAddress.Parse(dmeIp),
                                Port = (ushort)data.DMEObject.UdpPort,
                            },
                            clientChannel
                        );
                    }
                    break;
                }
                case RT_MSG_CLIENT_CONNECT_READY_AUX_UDP connectReadyAuxUdp:
                {
                    data.DMEObject?.OnConnectionCompleted();

                    _ = data.DMEObject?.DmeWorld?.EnqueuePlayerSyncTask(
                        Task.Run(() =>
                            {
                                Queue(
                                    new RT_MSG_SERVER_CONNECT_COMPLETE()
                                    {
                                        ClientCountAtConnect =
                                            (ushort?)data.DMEObject?.DmeWorld?.Clients.Length
                                            ?? 0x0001,
                                    },
                                    clientChannel
                                );

                                // Some clients doesn't expect TypeServerVersion.
                                if (
                                    scertClient.MediusVersion > 108
                                    && scertClient.ApplicationID != 20371
                                    && scertClient.ApplicationID != 20374
                                    && scertClient.ApplicationID != 20364
                                    && scertClient.ApplicationID != 20764
                                    && scertClient.ApplicationID != 21624
                                )
                                {
                                    Queue(
                                        new RT_MSG_SERVER_APP()
                                        {
                                            Message = new TypeServerVersion()
                                            {
                                                Version = "2.10.0009",
                                            },
                                        },
                                        clientChannel
                                    );
                                }
                            })
                            .ContinueWith(x =>
                                data.DMEObject?.DmeWorld?.OnPlayerJoined(data.DMEObject)
                            )
                    );
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
                    List<int> preClientObject = new() { 10952, 10954, 10130 };

                    if (
                        data.DMEObject != null
                        && preClientObject.Contains(scertClient.ApplicationID)
                    )
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
                            ServerTime = (uint)
                                data.DMEObject.DmeWorld.WorldTimer.ElapsedMilliseconds,
                        };

                        //if (data.DMEObject?.Udp != null && data.DMEObject.RemoteUdpEndpoint != null)
                        //{
                        //    await data.DMEObject.Udp.SendImmediate(timebaseQueryNotifyMessage).ConfigureAwait(false);
                        //}
                        //else
                        //{
                        //    await clientChannel.WriteAndFlushAsync(timebaseQueryNotifyMessage).ConfigureAwait(false);
                        //}

                        await clientChannel.WriteAndFlushAsync(timebaseQueryNotifyMessage);
                        //await clientChannel.WriteAndFlushAsync(new RT_MSG_SERVER_TIMEBASE_QUERY_NOTIFY()
                        //{
                        //    ClientTime = timebaseQuery.Timestamp,
                        //    ServerTime = (uint)data.DMEObject.DmeWorld.WorldTimer.ElapsedMilliseconds
                        //}).ConfigureAwait(false);
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
                    await ProcessRTTHostTokenMessage(tokenMessage, clientChannel, data)
                        .ConfigureAwait(false);
                    break;
                }
                case RT_MSG_CLIENT_APP_BROADCAST clientAppBroadcast:
                {
                    if (data.DMEObject != null)
                    {
                        Action<RT_MSG_CLIENT_APP_SINGLE, DMEObject>? modifyMessagePerClient = null;
                        var MessagePayload = clientAppBroadcast.Payload;

                        var InvalidatedRequest = false;

                        if (
                            data.DMEObject.ApplicationId == 20371
                            || data.DMEObject.ApplicationId == 20374
                        )
                            InvalidatedRequest = HomeHubProxy.ProcessDMEProxyTunneling(
                                MessagePayload,
                                data.DMEObject,
                                ref modifyMessagePerClient
                            );

                        if (!InvalidatedRequest)
                            data.DMEObject?.DmeWorld?.BroadcastTcp(
                                data.DMEObject,
                                MessagePayload ?? Array.Empty<byte>(),
                                modifyMessagePerClient
                            );
                    }
                    break;
                }
                case RT_MSG_CLIENT_APP_LIST clientAppList:
                {
                    if (data.DMEObject != null)
                    {
                        Action<RT_MSG_CLIENT_APP_SINGLE, DMEObject>? modifyMessagePerClient = null;
                        var MessagePayload = clientAppList.Payload;

                        var InvalidatedRequest = false;

                        if (
                            data.DMEObject.ApplicationId == 20371
                            || data.DMEObject.ApplicationId == 20374
                        )
                            InvalidatedRequest = HomeHubProxy.ProcessDMEProxyTunneling(
                                MessagePayload,
                                data.DMEObject,
                                ref modifyMessagePerClient
                            );

                        if (!InvalidatedRequest)
                            data.DMEObject.DmeWorld?.SendTcpAppList(
                                data.DMEObject,
                                clientAppList.Targets,
                                MessagePayload ?? Array.Empty<byte>(),
                                modifyMessagePerClient
                            );
                    }

                    break;
                }
                case RT_MSG_CLIENT_APP_SINGLE clientAppSingle:
                {
                    if (data.DMEObject != null)
                    {
                        Action<RT_MSG_CLIENT_APP_SINGLE, DMEObject>? modifyMessagePerClient = null;
                        var MessagePayload = clientAppSingle.Payload;

                        var InvalidatedRequest = false;

                        if (
                            data.DMEObject.ApplicationId == 20371
                            || data.DMEObject.ApplicationId == 20374
                        )
                            InvalidatedRequest = HomeHubProxy.ProcessDMEProxyTunneling(
                                MessagePayload,
                                data.DMEObject,
                                ref modifyMessagePerClient
                            );

                        if (!InvalidatedRequest)
                            data.DMEObject.DmeWorld?.SendTcpAppSingle(
                                data.DMEObject,
                                clientAppSingle.TargetOrSource,
                                MessagePayload ?? Array.Empty<byte>(),
                                modifyMessagePerClient
                            );
                    }
                    break;
                }
                case RT_MSG_CLIENT_APP_TOSERVER clientAppToServer:
                {
                    if (clientAppToServer.Message != null)
                        await ProcessMediusMessage(clientAppToServer.Message, clientChannel, data)
                            .ConfigureAwait(false);
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
                    LoggerAccessor.LogWarn($"[DMEProcessor] - UNHANDLED RT MESSAGE: {message}");

                    break;
                }
            }

            return;
        }

        protected virtual Task ProcessMediusMessage(
            BaseMediusMessage message,
            IChannel clientChannel,
            DMEChannelData data
        )
        {
            if (message == null)
                return Task.CompletedTask;

            switch (message)
            {
                case TypePing ping:
                {
#if DEBUG
                    LoggerAccessor.LogInfo(
                        $"[DMEProcessor] - PingPacketHandler: client {data.DMEObject} received"
                    );
#endif
                    if (ping.RequestEcho)
                    {
                        var value = new byte[0xA];
                        Queue(new RT_MSG_CLIENT_ECHO() { Value = value }, clientChannel);
                        break;
                    }

                    data.DMEObject?.EnqueueTcp(
                        new RT_MSG_SERVER_APP()
                        {
                            Message = new TypePing()
                            {
                                TimeOfSend = DateTimeUtils.GetUnixTimeU32(),
                                PingInstance = ping.PingInstance,
                                RequestEcho = ping.RequestEcho,
                            },
                        }
                    );

                    break;
                }
            }

            return Task.CompletedTask;
        }

        protected virtual Task ProcessRTTHostTokenMessage(
            RT_MSG_CLIENT_TOKEN_MESSAGE clientTokenMsg,
            IChannel clientChannel,
            DMEChannelData data
        )
        {
#if DEBUG
            LoggerAccessor.LogInfo(
                $"[DMEProcessor] - ProcessRTTHostTokenMessage: rt_msg_server_process_client_token_msg: msg type {clientTokenMsg.RT_TOKEN_MESSAGE_TYPE}, client {data.DMEObject?.ScertId}, target token = {clientTokenMsg.targetToken}"
            );
#endif
            if (!Rt_token_is_valid(clientTokenMsg.targetToken))
                LoggerAccessor.LogWarn(
                    $"[DMEProcessor] - ProcessRTTHostTokenMessage: rt_msg_server_process_client_token_msg: bad target token {clientTokenMsg.targetToken}"
                );
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
                                if (
                                    !data.DMEObject.DmeWorld.clientTokens.TryGetValue(
                                        clientTokenMsg.targetToken,
                                        out var value
                                    )
                                )
                                {
                                    data.DMEObject.DmeWorld.clientTokens.TryAdd(
                                        clientTokenMsg.targetToken,
                                        new List<int>() { data.DMEObject.DmeId }
                                    );

                                    if (
                                        data.DMEObject
                                            .DmeWorld
                                            .clientTokens[clientTokenMsg.targetToken]
                                            .Count > 0
                                    )
                                        data.DMEObject.DmeWorld.BroadcastTcpScertMessage(
                                            new RT_MSG_SERVER_TOKEN_MESSAGE() // We need to broadcast the signal that this token is owned.
                                            {
                                                TokenList = new List<(
                                                    RT_TOKEN_MESSAGE_TYPE,
                                                    ushort,
                                                    ushort
                                                )>
                                                {
                                                    (
                                                        RT_TOKEN_MESSAGE_TYPE.RT_TOKEN_SERVER_OWNED,
                                                        clientTokenMsg.targetToken,
                                                        (ushort)
                                                            data.DMEObject.DmeWorld.clientTokens[
                                                                clientTokenMsg.targetToken
                                                            ][0]
                                                    ),
                                                },
                                            }
                                        );
                                    else
                                    {
                                        LoggerAccessor.LogError(
                                            $"[DMEProcessor] - ProcessRTTHostTokenMessage: Client {data.DMEObject?.IP} requested a token request but errored out while owning a token!"
                                        );

                                        Queue(
                                            new RT_MSG_SERVER_FORCED_DISCONNECT()
                                            {
                                                Reason =
                                                    SERVER_FORCE_DISCONNECT_REASON.SERVER_FORCED_DISCONNECT_ERROR,
                                            },
                                            clientChannel
                                        );
                                    }
                                }
                                else
                                {
                                    value.Add(data.DMEObject.DmeId);

                                    Queue(
                                        new RT_MSG_SERVER_TOKEN_MESSAGE() // This message should not be broadcasted, Home doesn't like it.
                                        {
                                            TokenList = new List<(
                                                RT_TOKEN_MESSAGE_TYPE,
                                                ushort,
                                                ushort
                                            )>
                                            {
                                                (
                                                    RT_TOKEN_MESSAGE_TYPE.RT_TOKEN_SERVER_GRANTED,
                                                    clientTokenMsg.targetToken,
                                                    0
                                                ),
                                            },
                                        },
                                        clientChannel
                                    );
                                }
                            }
                        }
                        else
                        {
                            LoggerAccessor.LogError(
                                $"[DMEProcessor] - ProcessRTTHostTokenMessage: Client {data.DMEObject?.IP} requested a token request without being in a DmeWorld!"
                            );

                            Queue(
                                new RT_MSG_SERVER_FORCED_DISCONNECT()
                                {
                                    Reason =
                                        SERVER_FORCE_DISCONNECT_REASON.SERVER_FORCED_DISCONNECT_ERROR,
                                },
                                clientChannel
                            );
                        }

                        break;
                    }

                    case RT_TOKEN_MESSAGE_TYPE.RT_TOKEN_CLIENT_RELEASE:
                    {
                        if (data.DMEObject != null && data.DMEObject.DmeWorld != null)
                        {
                            lock (data.DMEObject.DmeWorld.clientTokens)
                            {
                                if (
                                    data.DMEObject.DmeWorld.clientTokens.TryGetValue(
                                        clientTokenMsg.targetToken,
                                        out var value
                                    )
                                    && value != null
                                )
                                {
                                    if (value.Contains(data.DMEObject.DmeId))
                                    {
                                        if (value.IndexOf(data.DMEObject.DmeId) == 0)
                                        {
                                            data.DMEObject.DmeWorld.clientTokens.Remove(
                                                clientTokenMsg.targetToken,
                                                out _
                                            );

                                            data.DMEObject.DmeWorld.BroadcastTcpScertMessage(
                                                new RT_MSG_SERVER_TOKEN_MESSAGE()
                                                {
                                                    TokenList = new List<(
                                                        RT_TOKEN_MESSAGE_TYPE,
                                                        ushort,
                                                        ushort
                                                    )>
                                                    {
                                                        (
                                                            RT_TOKEN_MESSAGE_TYPE.RT_TOKEN_SERVER_FREED,
                                                            clientTokenMsg.targetToken,
                                                            0
                                                        ),
                                                    },
                                                }
                                            );
                                        }
                                        else
                                        {
                                            value.Remove(data.DMEObject.DmeId);

                                            Queue(
                                                new RT_MSG_SERVER_TOKEN_MESSAGE()
                                                {
                                                    TokenList = new List<(
                                                        RT_TOKEN_MESSAGE_TYPE,
                                                        ushort,
                                                        ushort
                                                    )>
                                                    {
                                                        (
                                                            RT_TOKEN_MESSAGE_TYPE.RT_TOKEN_SERVER_RELEASED,
                                                            clientTokenMsg.targetToken,
                                                            0
                                                        ),
                                                    },
                                                },
                                                clientChannel
                                            );
                                        }
                                    }
                                    else
                                        Queue(
                                            new RT_MSG_SERVER_TOKEN_MESSAGE()
                                            {
                                                TokenList = new List<(
                                                    RT_TOKEN_MESSAGE_TYPE,
                                                    ushort,
                                                    ushort
                                                )>
                                                {
                                                    (
                                                        RT_TOKEN_MESSAGE_TYPE.RT_TOKEN_SERVER_RELEASED,
                                                        clientTokenMsg.targetToken,
                                                        0
                                                    ),
                                                },
                                            },
                                            clientChannel
                                        );
                                }
                                else
                                    Queue(
                                        new RT_MSG_SERVER_TOKEN_MESSAGE()
                                        {
                                            TokenList = new List<(
                                                RT_TOKEN_MESSAGE_TYPE,
                                                ushort,
                                                ushort
                                            )>
                                            {
                                                (
                                                    RT_TOKEN_MESSAGE_TYPE.RT_TOKEN_SERVER_OWNER_REMOVED,
                                                    0,
                                                    0
                                                ),
                                            },
                                        },
                                        clientChannel
                                    );
                            }

                            // Hotfix the arcade cabinets MLAA enabling in PS Home.
                            var mumClient = Program.MUMManager.GetClientBySessionKey(
                                data.DMEObject.SessionKey,
                                data.DMEObject.ApplicationId
                            );

                            if (
                                mumClient != null
                                && (
                                    mumClient.ApplicationId == 20371
                                    || mumClient.ApplicationId == 20374
                                )
                                && mumClient.IsOnRPCN
                                && mumClient.ClientHomeData != null
                                && mumClient.ClientHomeData.VersionAsDouble >= 01.83
                            )
                                _ = HomeRTMTools.SendRemoteCommand(
                                    mumClient,
                                    "lc Debug.System( 'mlaaenable 0' )"
                                );
                        }
                        else
                        {
                            LoggerAccessor.LogError(
                                $"[DMEProcessor] - ProcessRTTHostTokenMessage: Client {data.DMEObject?.IP} requested a token release without being in a DmeWorld!"
                            );

                            Queue(
                                new RT_MSG_SERVER_FORCED_DISCONNECT()
                                {
                                    Reason =
                                        SERVER_FORCE_DISCONNECT_REASON.SERVER_FORCED_DISCONNECT_ERROR,
                                },
                                clientChannel
                            );
                        }

                        break;
                    }

                    default:
                    {
                        LoggerAccessor.LogWarn(
                            $"[DMEProcessor] - UNHANDLED RT TOKEN MESSAGE: {clientTokenMsg.RT_TOKEN_MESSAGE_TYPE}"
                        );
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
            DMEChannelData data,
            BaseScertMessage message,
            bool isIncoming
        )
        {
            var onMsg = new OnMessageArgs(isIncoming)
            {
                Player = data.DMEObject,
                Channel = clientChannel,
                Message = message,
            };

            // Plugin
            var onTcpMsg = new OnTcpMsg(isIncoming) { Player = data.DMEObject, Packet = message };

            // go from lowest form upwards
            await Program
                .DmeManager.Plugins.OnEvent(PluginEvent.DME_GAME_ON_RECV_TCP, onTcpMsg)
                .ConfigureAwait(false);
            if (onTcpMsg.Ignore)
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
                    Player = data.DMEObject,
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
                    Player = data.DMEObject,
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

        #endregion

        public DMEObject? GetClientByScertId(ushort scertId)
        {
            return _scertIdToClient.TryGetValue(scertId, out var result) ? result : null;
        }

        protected ushort GenerateNewScertClientId()
        {
            return _clientCounter++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool Rt_token_is_valid(ushort TokenId)
        {
            return TokenId <= 65534;
        }

        public Task StopAsync()
        {
            return _DMEServer.StopAsync();
        }
    }
}
