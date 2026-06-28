using System.Net;
using CustomLogger;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using EndianTools;
using Horizon.CustomServers.Models;
using Horizon.LIBRARY.Pipeline.Tcp;
using Horizon.MEDIUS.Models;
using Horizon.RT.Common;
using Horizon.RT.Cryptography;
using Horizon.RT.Models;
using MultiServerLibrary.Extension;
using MultiServerLibrary.Extension.NET;

namespace Horizon.MEDIUS.Processors
{
    /// <summary>
    /// Introduced in Medius 3.03
    /// </summary>
    public class MMS : BaseMediusProcessor
    {
        public override ushort TCPPort
        {
            get => HorizonServerConfiguration.MEDIUSMMSTCPPort;
            set
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    "[MMS] - TCP Port can't be assigned."
                );
            }
        }

        public override ushort UDPPort
        {
            get => 0;
            set
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    "[MMS] - UDP Port can't be assigned."
                );
            }
        }

        private UniqueIDGenerator _clientCounter = new UniqueIDGenerator();

        public MMS() { }

        /// <summary>
        /// Start the MMS TCP Server.
        /// </summary>
        public virtual Task StartAsync(int maxConcurrentListeners = 10)
        {
            return Task.Run(() =>
            {
                _MediusServer.Start(
                    new Dictionary<ushort, bool> { { TCPPort, true } },
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
                                1024,
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
                        _channelDatas.TryAdd(channel.Id.AsLongText(), new ChannelData());
                    },
                    (channel) =>
                    {
                        _channelDatas.TryRemove(channel.Id.AsLongText(), out var data);
                    },
                    (channel, messageObj) =>
                    {
                        BaseScertMessage message = (BaseScertMessage)messageObj;

                        var key = channel.Id.AsLongText();
                        if (_channelDatas.TryGetValue(key, out var data))
                        {
                            data.RecvQueue.Enqueue(message);

                            if (message is RT_MSG_SERVER_ECHO serverEcho)
                                data.ClientObject?.OnRecvServerEcho(serverEcho);
                            else if (message is RT_MSG_CLIENT_ECHO clientEcho)
                                data.ClientObject?.OnRecvClientEcho(clientEcho);

                            data.ClientObject?.OnRecv(message);
                        }

                        // Log if id is set
                        if (message.CanLog())
                            LoggerAccessor.LogInfo($"[MMS] - RECV {channel}: {message}");
                    }
                );
            });
        }

        /// <summary>
        /// Stop the server.
        /// </summary>
        public virtual Task StopAsync()
        {
            return _MediusServer.StopAsync();
        }

        /// <summary>
        /// Process messages.
        /// </summary>
        public async Task Tick()
        {
            if (_MediusServer.ScertHandler == null)
                return;

            await Task.WhenAll(_MediusServer.ScertHandler.Channels.Select(Tick).ToArray())
                .ConfigureAwait(false);
        }

        private async Task Tick(IChannel clientChannel)
        {
            if (clientChannel == null)
                return;

            var responses = new List<BaseScertMessage>();
            var key = clientChannel.Id.AsLongText();

            try
            {
                if (_channelDatas.TryGetValue(key, out var data))
                {
                    // Process all messages in queue
                    while (data.RecvQueue.TryDequeue(out var message))
                    {
                        try
                        {
                            await ProcessMessage(message, clientChannel, data)
                                .ConfigureAwait(false);
                        }
                        catch (Exception e)
                        {
                            LoggerAccessor.LogError(
                                $"[MMS] - clientChannel ticking thrown an assertion while processing the message queue. (Exception:{e})"
                            );
                        }
                    }

                    // Send if writeable
                    if (clientChannel.IsWritable)
                    {
                        // Add send queue to responses
                        while (data.SendQueue.TryDequeue(out var message))
                            responses.Add(message);

                        if (responses.Count > 0)
                            _ = clientChannel.WriteAndFlushAsync(responses);
                    }
                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(
                    $"[MMS] - clientChannel ticking thrown an assertion. (Exception:{e})"
                );
            }
        }

        #region Message Processing

        protected override async Task ProcessMessage(
            BaseScertMessage message,
            IChannel clientChannel,
            ChannelData data
        )
        {
            // Get ScertClient data
            var scertClient = clientChannel
                .GetAttribute(LIBRARY.Pipeline.Constants.SCERT_CLIENT)
                .Get();
            var enableEncryption = DATABASE
                .DatabaseManager.GetAppSettingsOrDefault(data.ApplicationId)
                .EnableEncryption;
            scertClient.CipherService.EnableEncryption = enableEncryption;

            switch (message)
            {
                case RT_MSG_CLIENT_HELLO clientHello:
                {
                    // send hello
                    Queue(
                        new RT_MSG_SERVER_HELLO()
                        {
                            RsaPublicKey = false
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
                    // generate new client session key
                    scertClient.CipherService.GenerateCipher(
                        CipherContext.RSA_AUTH,
                        clientCryptKeyPublic.PublicKey.ReverseArray()
                    );
                    scertClient.CipherService.GenerateCipher(CipherContext.RC_CLIENT_SESSION);

                    Queue(
                        new RT_MSG_SERVER_CRYPTKEY_PEER()
                        {
                            SessionKey = scertClient.CipherService.GetPublicKey(
                                CipherContext.RC_CLIENT_SESSION
                            ),
                        },
                        clientChannel
                    );
                    break;
                }
                case RT_MSG_CLIENT_CONNECT_TCP clientConnectTcp:
                {
                    var appid = clientConnectTcp.AppId;

                    #region Compatible AppId
                    if (appid < 0)
                    {
                        LoggerAccessor.LogError(
                            $"[MMS] - Client Connected {clientChannel.RemoteAddress} with an invalid connect payload!"
                        );
                        break;
                    }
                    else if (!Program.MUMManager.IsAppIdSupported(appid))
                    {
                        LoggerAccessor.LogError(
                            $"[MMS] - Client {clientChannel.RemoteAddress} attempting to authenticate with incompatible app id {appid}"
                        );
                        await clientChannel
                            .CloseAsync()
                            .TryAwait(TimeSpan.FromMilliseconds(2000))
                            .ConfigureAwait(false);
                        return;
                    }
                    #endregion

                    if (clientConnectTcp.Key == RSA_KEY.Empty)
                    {
                        LoggerAccessor.LogError(
                            $"[MMS] - Client Connected {clientChannel.RemoteAddress} with an empty key!"
                        );
                        break;
                    }

                    List<int> pre108ServerComplete = new()
                    {
                        10130,
                        10442,
                        10721,
                        10536,
                        10538,
                        10114,
                        10164,
                        10190,
                        10124,
                        10284,
                        10330,
                        10334,
                        10414,
                        10421,
                        10442,
                        10538,
                        10540,
                        10550,
                        10582,
                        10584,
                        10680,
                        10681,
                        10683,
                        10684,
                        10724,
                    };

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
                            scertClient.MediusVersion ?? 0
                        );

                        if (DefaultChannel.Id == clientConnectTcp.TargetWorldId)
                            targetChannel = DefaultChannel;

                        if (targetChannel == null)
                        {
                            LoggerAccessor.LogError(
                                $"[MMS] - Client: {clientConnectTcp.AccessToken} tried to join, but targetted WorldId:{clientConnectTcp.TargetWorldId} doesn't exist!"
                            );
                            await clientChannel
                                .CloseAsync()
                                .TryAwait(TimeSpan.FromMilliseconds(2000))
                                .ConfigureAwait(false);
                            break;
                        }
                    }

                    data.ClientObject = Program.MUMManager.GetClientByAccessToken(
                        clientConnectTcp.AccessToken,
                        appid
                    );
                    data.ClientObject ??= Program.MUMManager.GetClientBySessionKey(
                        clientConnectTcp.SessionKey,
                        appid
                    );

                    #region Client Object Null?
                    if (data.ClientObject == null)
                    {
                        data.Ignore = true;
                        LoggerAccessor.LogError(
                            $"[MMS] - ClientObject could not be granted for {clientChannel.RemoteAddress}: {clientConnectTcp}"
                        );
                    }
                    #endregion
                    else
                    {
                        data.ClientObject.MediusVersion = scertClient.MediusVersion ?? 0;
                        data.ClientObject.ApplicationId = appid;
                        data.ClientObject.OnConnected();

                        LoggerAccessor.LogInfo(
                            $"[MMS] - Client Connected {clientChannel.RemoteAddress}!"
                        );

                        await data.ClientObject.JoinChannel(targetChannel).ConfigureAwait(false);

                        #region if PS3
                        if (scertClient.IsPS3Client)
                        {
                            List<int> ConnectAcceptTCPGames = new()
                            {
                                20623,
                                20624,
                                21564,
                                21574,
                                21584,
                                21594,
                                22274,
                                22284,
                                22294,
                                22304,
                                20040,
                                20041,
                                20042,
                                20043,
                                20044,
                            };

                            //CAC & Warhawk
                            if (ConnectAcceptTCPGames.Contains(data.ClientObject.ApplicationId))
                            {
                                Queue(
                                    new RT_MSG_SERVER_CONNECT_ACCEPT_TCP()
                                    {
                                        PlayerId = 0,
                                        ScertId = GenerateNewScertClientId(),
                                        PlayerCount = 0x0001,
                                        IP = (clientChannel.RemoteAddress as IPEndPoint)?.Address,
                                    },
                                    clientChannel
                                );
                            }
                            else
                                Queue(new RT_MSG_SERVER_CONNECT_REQUIRE(), clientChannel);
                        }
                        #endregion
                        else if (
                            scertClient.MediusVersion > 108
                            && scertClient.ApplicationID != 11484
                        )
                            Queue(new RT_MSG_SERVER_CONNECT_REQUIRE(), clientChannel);
                        else
                        {
                            //Older Medius titles do NOT use CRYPTKEY_GAME, newer ones have this.
                            if (
                                scertClient.CipherService != null
                                && scertClient.CipherService.HasKey(CipherContext.RC_CLIENT_SESSION)
                                && scertClient.MediusVersion >= 109
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
                                    PlayerId = 0,
                                    ScertId = GenerateNewScertClientId(),
                                    PlayerCount = 0x0001,
                                    IP = (clientChannel.RemoteAddress as IPEndPoint)?.Address,
                                },
                                clientChannel
                            );

                            if (pre108ServerComplete.Contains(data.ApplicationId))
                                Queue(
                                    new RT_MSG_SERVER_CONNECT_COMPLETE()
                                    {
                                        ClientCountAtConnect = 0x0001,
                                    },
                                    clientChannel
                                );
                        }
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
                            PlayerId = 0,
                            ScertId = GenerateNewScertClientId(),
                            PlayerCount = 0x0001,
                            IP = (clientChannel.RemoteAddress as IPEndPoint)?.Address,
                        },
                        clientChannel
                    );
                    break;
                }
                case RT_MSG_CLIENT_CONNECT_READY_TCP clientConnectReadyTcp:
                {
                    Queue(
                        new RT_MSG_SERVER_CONNECT_COMPLETE() { ClientCountAtConnect = 0x0001 },
                        clientChannel
                    );

                    if (scertClient.MediusVersion > 108)
                        Queue(new RT_MSG_SERVER_ECHO(), clientChannel);
                    break;
                }
                case RT_MSG_SERVER_ECHO serverEchoReply:
                {
                    break;
                }
                case RT_MSG_CLIENT_ECHO clientEcho:
                {
                    if (data.ClientObject == null || !data.ClientObject.IsLoggedIn)
                        break;

                    Queue(new RT_MSG_CLIENT_ECHO() { Value = clientEcho.Value }, clientChannel);
                    break;
                }
                case RT_MSG_CLIENT_APP_TOSERVER clientAppToServer:
                {
                    await ProcessMediusMessage(clientAppToServer.Message, clientChannel, data)
                        .ConfigureAwait(false);
                    break;
                }

                case RT_MSG_CLIENT_DISCONNECT _:
                {
                    //Medius 1.08 (Used on WRC 4) haven't a state
                    if (scertClient.MediusVersion > 108)
                        data.State = ServerClientState.DISCONNECTED;

                    await clientChannel
                        .CloseAsync()
                        .TryAwait(TimeSpan.FromMilliseconds(2000))
                        .ConfigureAwait(false);

                    LoggerAccessor.LogInfo(
                        $"[MMS] - Client id = {data.ClientObject?.AccountId} disconnected by request with no specific reason\n"
                    );
                    break;
                }
                case RT_MSG_CLIENT_DISCONNECT_WITH_REASON clientDisconnectWithReason:
                {
                    if (
                        clientDisconnectWithReason.Reason
                        <= RT_MSG_CLIENT_DISCONNECT_REASON.RT_MSG_CLIENT_DISCONNECT_LENGTH_MISMATCH
                    )
                        LoggerAccessor.LogInfo(
                            $"[MMS] - disconnected by request with reason of {clientDisconnectWithReason.Reason}\n"
                        );
                    else
                        LoggerAccessor.LogInfo(
                            $"[MMS] - disconnected by request with (application specified) reason of {clientDisconnectWithReason.Reason}\n"
                        );

                    data.State = ServerClientState.DISCONNECTED;
                    await clientChannel
                        .CloseAsync()
                        .TryAwait(TimeSpan.FromMilliseconds(2000))
                        .ConfigureAwait(false);
                    break;
                }

                default:
                {
                    LoggerAccessor.LogWarn($"[MMS] - UNHANDLED RT MESSAGE: {message}");

                    break;
                }
            }

            return;
        }

        protected virtual Task ProcessMediusMessage(
            BaseMediusMessage message,
            IChannel clientChannel,
            ChannelData data
        )
        {
            if (message == null)
                return Task.CompletedTask;

            switch (message)
            {
                default:
                {
                    LoggerAccessor.LogWarn($"[MMS] - UNHANDLED MEDIUS MESSAGE: {message}");
                    break;
                }
            }

            return Task.CompletedTask;
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

        protected uint GenerateNewScertClientId()
        {
            return _clientCounter.CreateSequentialID();
        }
    }
}
