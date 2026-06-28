using System.Collections.Concurrent;
using System.Net;
using CastleLibrary.Utils;
using CustomLogger;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using EndianTools;
using Horizon.CustomServers;
using Horizon.CustomServers.Models;
using Horizon.LIBRARY.Pipeline.Tcp;
using Horizon.MEDIUS.Models;
using Horizon.MUM.Models;
using Horizon.RT.Common;
using Horizon.RT.Cryptography;
using Horizon.RT.Models;
using MultiServerLibrary.Extension;
using MultiServerLibrary.Extension.NET;

namespace Horizon.MUIS
{
    public class MUISProcessor
    {
        public readonly DNTCPHybridServer _MUISServer = new();

        private readonly UniqueIDGenerator _clientCounter = new UniqueIDGenerator();

        protected ConcurrentDictionary<string, ChannelData> _channelDatas = new();

        public Task StartAsync(int maxConcurrentListeners = 10)
        {
            var muisPortsConfig = new Dictionary<ushort, bool>();

            foreach (var port in HorizonServerConfiguration.MUISPorts)
                muisPortsConfig.Add(port, true);

            return Task.Run(() =>
            {
                _MUISServer.Start(
                    muisPortsConfig,
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
                        pipeline.AddLast(_MUISServer.ScertHandler);
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

                        if (_channelDatas.TryGetValue(channel.Id.AsLongText(), out var data))
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
                            LoggerAccessor.LogInfo($"[MUISProcessor] - RECV {channel}: {message}");
                    }
                );
            });
        }

        /// <summary>
        /// Process messages.
        /// </summary>
        public async Task Tick()
        {
            if (_MUISServer.ScertHandler != null)
                await Task.WhenAll(_MUISServer.ScertHandler.Channels.Select(Tick).ToArray())
                    .ConfigureAwait(false);
        }

        private async Task Tick(IChannel clientChannel)
        {
            if (clientChannel == null)
                return;

            List<BaseScertMessage> responses = new();

            try
            {
                if (_channelDatas.TryGetValue(clientChannel.Id.AsLongText(), out var data))
                {
                    // Process all messages in queue
                    while (data.RecvQueue.TryDequeue(out var message))
                    {
                        try
                        {
                            ProcessMessage(message, clientChannel, data);
                        }
                        catch (Exception e)
                        {
                            LoggerAccessor.LogError(
                                $"[MUISProcessor] - clientChannel ticking thrown an assertion while processing the message queue. (Exception:{e})"
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
                            await clientChannel.WriteAndFlushAsync(responses).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(
                    $"[MUISProcessor] - clientChannel ticking thrown an assertion. (Exception:{e})"
                );
            }
        }

        #region Message Processing

        protected async void ProcessMessage(
            BaseScertMessage message,
            IChannel clientChannel,
            ChannelData data
        )
        {
            // Get ScertClient data
            var scertClient = clientChannel
                .GetAttribute(LIBRARY.Pipeline.Constants.SCERT_CLIENT)
                .Get();
            if (scertClient.CipherService != null)
            {
                scertClient.CipherService.EnableEncryption =
                    HorizonServerConfiguration.MUISEncryptMessages;

                switch (message)
                {
                    case RT_MSG_CLIENT_HELLO clientHello:
                    {
                        if (data.State > ServerClientState.HELLO)
                        {
                            LoggerAccessor.LogError(
                                $"[MUISProcessor] - Unexpected RT_MSG_CLIENT_HELLO from {clientChannel.RemoteAddress}: {clientHello}"
                            );
                            break;
                        }

                        data.State = ServerClientState.HELLO;
                        Queue(
                            new RT_MSG_SERVER_HELLO()
                            {
                                RsaPublicKey = scertClient.CipherService.EnableEncryption
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
                        if (data.State > ServerClientState.HANDSHAKE)
                        {
                            LoggerAccessor.LogError(
                                $"[MUISProcessor] - Unexpected RT_MSG_CLIENT_CRYPTKEY_PUBLIC from {clientChannel.RemoteAddress}: {clientCryptKeyPublic}"
                            );
                            break;
                        }

                        data.State = ServerClientState.CONNECT_1;

                        if (clientCryptKeyPublic.PublicKey != null)
                        {
                            // generate new client session key
                            scertClient.CipherService.GenerateCipher(
                                CipherContext.RSA_AUTH,
                                clientCryptKeyPublic.PublicKey.ReverseArray()
                            );
                            scertClient.CipherService.GenerateCipher(
                                CipherContext.RC_CLIENT_SESSION
                            );

                            Queue(
                                new RT_MSG_SERVER_CRYPTKEY_PEER()
                                {
                                    SessionKey = scertClient.CipherService.GetPublicKey(
                                        CipherContext.RC_CLIENT_SESSION
                                    ),
                                },
                                clientChannel
                            );
                        }
                        break;
                    }
                    case RT_MSG_CLIENT_CONNECT_TCP clientConnectTcp:
                    {
                        if (data.State > ServerClientState.CONNECT_1)
                        {
                            LoggerAccessor.LogError(
                                $"[MUISProcessor] - Unexpected RT_MSG_CLIENT_CONNECT_TCP from {clientChannel.RemoteAddress}: {clientConnectTcp}"
                            );
                            break;
                        }

                        var appid = clientConnectTcp.AppId;

                        #region Compatible AppId
                        if (appid < 0)
                        {
                            LoggerAccessor.LogError(
                                $"[MUISProcessor] - Client Connected {clientChannel.RemoteAddress} with an invalid connect payload!"
                            );
                            break;
                        }
                        #endregion

                        if (clientConnectTcp.Key == RSA_KEY.Empty)
                        {
                            LoggerAccessor.LogError(
                                $"[MUISProcessor] - Client Connected {clientChannel.RemoteAddress} with an empty key!"
                            );
                            break;
                        }

                        List<int> pre108ServerComplete = new() { };

                        // No need to apply the connection delay, MUIS is expected to be a One-Shot server.

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
                                    $"[MUISProcessor] - Client: {clientConnectTcp.AccessToken} tried to join, but targetted WorldId:{clientConnectTcp.TargetWorldId} doesn't exist!"
                                );
                                break;
                            }
                        }

                        LoggerAccessor.LogInfo(
                            $"[MUISProcessor] - Client Connected {clientChannel.RemoteAddress} with new ClientObject!"
                        );

                        await InternetProtocolUtils
                            .TryGetServerIP(out string muisIp)
                            .ConfigureAwait(false);

                        data.ClientObject = new(scertClient.MediusVersion ?? 0)
                        {
                            MuisIP = string.IsNullOrEmpty(muisIp)
                                ? IPAddress.Any
                                : IPAddress.Parse(muisIp),
                            ApplicationId = appid,
                        };

                        data.ClientObject.OnConnected();

                        await data.ClientObject.JoinChannel(targetChannel).ConfigureAwait(false);

                        data.State = ServerClientState.AUTHENTICATED;

                        // If this is a PS3 client or medius version superior to 108
                        if (scertClient.IsPS3Client || scertClient.MediusVersion > 108)
                            //Send a Server_Connect_Require with no Password needed
                            Queue(new RT_MSG_SERVER_CONNECT_REQUIRE(), clientChannel);
                        else
                        {
                            //Older Medius titles do NOT use CRYPTKEY_GAME, newer ones have this.
                            if (
                                scertClient.CipherService != null
                                && scertClient.CipherService.HasKey(CipherContext.RC_CLIENT_SESSION)
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

                        if (data.ApplicationId == 20371 || data.ApplicationId == 20374)
                            CheatQuery(
                                0x00010000,
                                512000,
                                clientChannel,
                                CheatQueryType.DME_SERVER_CHEAT_QUERY_SHA1_HASH,
                                unchecked((int)0xDEADBEEF)
                            );

                        break;
                    }
                    case RT_MSG_SERVER_CHEAT_QUERY clientCheatQuery:
                    {
                        var QueryData = clientCheatQuery.Data;

                        if (QueryData != null)
                        {
                            LoggerAccessor.LogDebug(
                                $"[MUISProcessor] - QUERY CHECK - Client:{data.ClientObject?.IP} Has Data:{QueryData.BytesToHexStr()} in offset: {clientCheatQuery.StartAddress}"
                            );

                            if (data.ApplicationId == 20371 || data.ApplicationId == 20374)
                            {
                                switch (clientCheatQuery.SequenceId)
                                {
                                    case -559038737:
                                        switch (clientCheatQuery.StartAddress)
                                        {
                                            case 65536:
                                                if (data.ClientObject != null)
                                                {
                                                    if (data.ClientObject.ClientHomeData == null)
                                                        data.ClientObject.ClientHomeData =
                                                            HorizonServerConfiguration
                                                                .HomeOffsetsList.Where(x =>
                                                                    !string.IsNullOrEmpty(
                                                                        x.Sha1Hash
                                                                    )
                                                                    && x.Sha1Hash[..^8]
                                                                        .Equals(
                                                                            clientCheatQuery.Data.BytesToHexStr(),
                                                                            StringComparison.InvariantCultureIgnoreCase
                                                                        )
                                                                )
                                                                .FirstOrDefault();

                                                    if (
                                                        !HorizonServerConfiguration.MEDIUSPlaystationHomeAllowAnyEboot
                                                        && data.ClientObject.ClientHomeData == null
                                                    )
                                                    {
                                                        var anticheatMsg =
                                                            $"[SECURITY] - HOME ANTI-CHEAT - DETECTED UNKNOWN EBOOT - User:{data.ClientObject.IP + ":" + data.ClientObject.AccountName} CID:{data.MachineId}";

                                                        _ = Channel.BroadcastSystemMessage(
                                                            data.ClientObject.CurrentChannel.LocalClients.Where(
                                                                x => x != data.ClientObject
                                                            ),
                                                            anticheatMsg,
                                                            byte.MaxValue
                                                        );

                                                        LoggerAccessor.LogError(anticheatMsg);

                                                        // Banned
                                                        await QueueBanMessage(data)
                                                            .ConfigureAwait(false);

                                                        data.ClientObject.ForceDisconnect();
                                                        _ = data.ClientObject.Logout();
                                                    }
                                                }
                                                break;
                                        }
                                        break;
                                }
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
                    case RT_MSG_CLIENT_APP_TOSERVER clientAppToServer:
                    {
                        if (data.State != ServerClientState.AUTHENTICATED)
                        {
                            LoggerAccessor.LogError(
                                $"[MUISProcessor] - Unexpected RT_MSG_CLIENT_APP_TOSERVER from {clientChannel.RemoteAddress}: {clientAppToServer}"
                            );
                            break;
                        }

                        if (clientAppToServer.Message != null)
                            ProcessMediusMessage(clientAppToServer.Message, clientChannel, data);
                        break;
                    }
                    case RT_MSG_CLIENT_APP_LIST clientAppList:
                    {
                        break;
                    }
                    case RT_MSG_CLIENT_DISCONNECT _:
                    {
                        //Medius 1.08 (Used on WRC 4) haven't a state
                        if (scertClient.MediusVersion > 108)
                            data.State = ServerClientState.DISCONNECTED;

                        var closeTask = clientChannel.CloseAsync();
                        if (!await closeTask.TryAwait(TimeSpan.FromMilliseconds(2000)))
                            LoggerAccessor.LogWarn(
                                $"[MUISProcessor] - Timed out waiting for MAS client channel close: {clientChannel.RemoteAddress}"
                            );

                        LoggerAccessor.LogInfo(
                            $"[MUISProcessor] - Client disconnected by request with no specific reason"
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
                                $"[MUISProcessor] - Disconnected by request with reason of {clientDisconnectWithReason.Reason}"
                            );
                        else
                            LoggerAccessor.LogInfo(
                                $"[MUISProcessor] - Disconnected by request with (application specified) reason of {clientDisconnectWithReason.Reason}"
                            );

                        var closeTask = clientChannel.CloseAsync();
                        if (!await closeTask.TryAwait(TimeSpan.FromMilliseconds(2000)))
                            LoggerAccessor.LogWarn(
                                $"[MUISProcessor] - Timed out waiting for MAS client channel close: {clientChannel.RemoteAddress}"
                            );
                        break;
                    }
                    default:
                    {
                        LoggerAccessor.LogWarn($"UNHANDLED RT MESSAGE: {message}");
                        break;
                    }
                }
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

        protected virtual void ProcessMediusMessage(
            BaseMediusMessage message,
            IChannel clientChannel,
            ChannelData data
        )
        {
            if (message == null)
                return;

            switch (message)
            {
                #region Version Server
                case MediusVersionServerRequest versionServerRequest:
                {
                    // ERROR - Need a session
                    if (data == null)
                    {
                        LoggerAccessor.LogError(
                            $"[MUISProcessor] - INVALID OPERATION: {clientChannel} sent {versionServerRequest} without channeldata."
                        );
                        break;
                    }

                    #region Killzone TCES/Pubeta Version Override
                    // Killzoze TCES/Pubeta
                    if (data.ApplicationId == 10442)
                    {
                        Queue(
                            new RT_MSG_SERVER_APP()
                            {
                                Message = new MediusVersionServerResponse()
                                {
                                    MessageID = versionServerRequest.MessageID,
                                    VersionServer =
                                        "Medius Universe Information Server Version 1.50.0009",
                                    StatusCode = MediusCallbackStatus.MediusSuccess,
                                },
                            },
                            clientChannel
                        );
                    }
                    #endregion
                    else
                    {
                        Queue(
                            new RT_MSG_SERVER_APP()
                            {
                                Message = new MediusVersionServerResponse()
                                {
                                    MessageID = versionServerRequest.MessageID,
                                    VersionServer = HorizonServerConfiguration.MUISVersion,
                                    StatusCode = MediusCallbackStatus.MediusSuccess,
                                },
                            },
                            clientChannel
                        );
                    }

                    break;
                }

                #endregion

                #region MediusGetUniverse_ExtraInfo
                case MediusGetUniverse_ExtraInfoRequest getUniverse_ExtraInfoRequest:
                {
                    if (
                        HorizonServerConfiguration.MUISCompatibleApplicationIds.Contains(
                            data.ApplicationId
                        )
                    )
                    {
                        if (
                            HorizonServerConfiguration.MUISUniverses.TryGetValue(
                                data.ApplicationId,
                                out var infos
                            )
                        )
                        {
                            if (getUniverse_ExtraInfoRequest.InfoType == 0)
                            {
                                Queue(
                                    new RT_MSG_SERVER_APP()
                                    {
                                        Message = new MediusUniverseStatusList_ExtraInfoResponse()
                                        {
                                            MessageID = new MessageId(),
                                            StatusCode =
                                                MediusCallbackStatus.MediusInvalidRequestMsg,
                                            EndOfList = true,
                                        },
                                    },
                                    clientChannel
                                );
                            }

                            #region INFO_UNIVERSES

                            foreach (var info in infos)
                            {
                                #region SVOUrl
                                if (
                                    getUniverse_ExtraInfoRequest.InfoType.HasFlag(
                                        MediusUniverseVariableInformationInfoFilter.INFO_SVO_URL
                                    )
                                )
                                {
                                    Queue(
                                        new RT_MSG_SERVER_APP()
                                        {
                                            Message = new MediusUniverseSvoURLResponse()
                                            {
                                                MessageID = new MessageId(),
                                                URL = info.SvoURL,
                                            },
                                        },
                                        clientChannel
                                    );
                                }
                                #endregion

                                // MUIS Standard Flow - Deprecated after Medius Client/Server Library 1.50
                                if (
                                    getUniverse_ExtraInfoRequest.InfoType.HasFlag(
                                        MediusUniverseVariableInformationInfoFilter.INFO_UNIVERSES
                                    )
                                )
                                {
                                    Queue(
                                        new RT_MSG_SERVER_APP()
                                        {
                                            Message =
                                                new MediusUniverseStatusList_ExtraInfoResponse()
                                                {
                                                    MessageID = new MessageId(),
                                                    StatusCode = MediusCallbackStatus.MediusSuccess,
                                                    UniverseName = info.Name,
                                                    DNS = info.Endpoint,
                                                    Port = info.Port,
                                                    UniverseDescription = info.Description,
                                                    Status = info.Status,
                                                    UserCount = info.UserCount,
                                                    MaxUsers = info.MaxUsers,
                                                    BillingSystemName = info.BillingSystemName,
                                                    UniverseBilling = info.UniverseBilling,
                                                    EndOfList = true,
                                                    ExtendedInfo = info.ExtendedInfo,
                                                },
                                        },
                                        clientChannel
                                    );

                                    #region News
                                    if (
                                        getUniverse_ExtraInfoRequest.InfoType.HasFlag(
                                            MediusUniverseVariableInformationInfoFilter.INFO_NEWS
                                        )
                                    )
                                    {
#if DEBUG
                                        LoggerAccessor.LogInfo(
                                            "[MUISProcessor] - News bit set in request"
                                        );
#endif
                                        Queue(
                                            new RT_MSG_SERVER_APP()
                                            {
                                                Message = new MediusUniverseNewsResponse()
                                                {
                                                    MessageID =
                                                        getUniverse_ExtraInfoRequest.MessageID,
                                                    StatusCode = MediusCallbackStatus.MediusSuccess,
                                                    News = "Simulated News!",
                                                    EndOfList = true,
                                                },
                                            },
                                            clientChannel
                                        );
                                    }
                                    #endregion
                                }
#if DEBUG
                                LoggerAccessor.LogInfo(
                                    $"[MUISProcessor] - send univ info (ctr=): [{HorizonServerConfiguration.MUISUniverses.ToArray().Length}]"
                                );
#endif
                            }
                            #endregion
                        }
                        else
                        {
                            LoggerAccessor.LogWarn($"[MUISProcessor] - No universes out there.");

                            Queue(
                                new RT_MSG_SERVER_APP()
                                {
                                    Message = new MediusUniverseVariableInformationResponse()
                                    {
                                        MessageID = getUniverse_ExtraInfoRequest.MessageID,
                                        StatusCode = MediusCallbackStatus.MediusNoResult,
                                        InfoFilter = getUniverse_ExtraInfoRequest.InfoType,
                                        EndOfList = true,
                                    },
                                },
                                clientChannel
                            );
                        }
                    }
                    else
                    {
                        LoggerAccessor.LogWarn(
                            $"[MUISProcessor] - ApplicationID not compatible [{data.ApplicationId}]"
                        );

                        Queue(
                            new RT_MSG_SERVER_APP()
                            {
                                Message = new MediusUniverseVariableInformationResponse()
                                {
                                    MessageID = getUniverse_ExtraInfoRequest.MessageID,
                                    StatusCode = MediusCallbackStatus.MediusIncompatibleAppID,
                                    InfoFilter = getUniverse_ExtraInfoRequest.InfoType,
                                    EndOfList = true,
                                },
                            },
                            clientChannel
                        );
                    }

                    break;
                }
                #endregion

                #region MediusGetUniverseInformationRequest
                case MediusGetUniverseInformationRequest getUniverseInfo:
                {
                    //Check if Client AppId equals the Appid in CompatibleAppId list
                    if (
                        HorizonServerConfiguration.MUISCompatibleApplicationIds.Contains(
                            data.ApplicationId
                        )
                    )
                    {
                        if (
                            HorizonServerConfiguration.MUISUniverses.TryGetValue(
                                data.ApplicationId,
                                out var infos
                            )
                        )
                        {
                            //Send Standard/Variable Flow
                            foreach (var info in infos)
                            {
                                var isLast = infos.LastOrDefault() == info;

                                #region INFO_UNIVERSES
                                // MUIS Standard Flow - Deprecated after Medius Client/Server Library 1.50
                                if (
                                    getUniverseInfo.InfoType.HasFlag(
                                        MediusUniverseVariableInformationInfoFilter.INFO_UNIVERSES
                                    )
                                )
                                {
                                    if (getUniverseInfo.InfoType == 0)
                                    {
                                        Queue(
                                            new RT_MSG_SERVER_APP()
                                            {
                                                Message = new MediusUniverseStatusListResponse()
                                                {
                                                    MessageID = getUniverseInfo.MessageID,
                                                    StatusCode =
                                                        MediusCallbackStatus.MediusInvalidRequestMsg,
                                                    EndOfList = true,
                                                },
                                            },
                                            clientChannel
                                        );
                                    }

                                    Queue(
                                        new RT_MSG_SERVER_APP()
                                        {
                                            Message = new MediusUniverseStatusListResponse()
                                            {
                                                MessageID = getUniverseInfo.MessageID,
                                                StatusCode = MediusCallbackStatus.MediusSuccess,
                                                UniverseName = info.Name,
                                                DNS = info.Endpoint,
                                                Port = info.Port,
                                                UniverseDescription = info.Description,
                                                Status = info.Status,
                                                UserCount = info.UserCount,
                                                MaxUsers = info.MaxUsers,
                                                EndOfList = true,
                                            },
                                        },
                                        clientChannel
                                    );
                                #endregion

                                    #region News
                                    if (
                                        getUniverseInfo.InfoType.HasFlag(
                                            MediusUniverseVariableInformationInfoFilter.INFO_NEWS
                                        )
                                    )
                                    {
                                        Queue(
                                            new RT_MSG_SERVER_APP()
                                            {
                                                Message = new MediusUniverseNewsResponse()
                                                {
                                                    MessageID = getUniverseInfo.MessageID,
                                                    StatusCode = MediusCallbackStatus.MediusSuccess,
                                                    News = "Simulated News",
                                                    EndOfList = isLast,
                                                },
                                            },
                                            clientChannel
                                        );
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region InfoFilter = Null
                                    if (getUniverseInfo.InfoType == 0)
                                    {
                                        Queue(
                                            new RT_MSG_SERVER_APP()
                                            {
                                                Message =
                                                    new MediusUniverseVariableInformationResponse()
                                                    {
                                                        MessageID = getUniverseInfo.MessageID,
                                                        StatusCode =
                                                            MediusCallbackStatus.MediusInvalidRequestMsg,
                                                        EndOfList = true,
                                                    },
                                            },
                                            clientChannel
                                        );
                                    }
                                    #endregion

                                    #region SVOUrl
                                    if (
                                        getUniverseInfo.InfoType.HasFlag(
                                            MediusUniverseVariableInformationInfoFilter.INFO_SVO_URL
                                        )
                                    )
                                    {
#if DEBUG
                                        LoggerAccessor.LogInfo(
                                            $"[MUISProcessor] - send svo info: [{HorizonServerConfiguration.MUISUniverses.ToArray().Length}]"
                                        );
#endif
                                        Queue(
                                            new RT_MSG_SERVER_APP()
                                            {
                                                Message = new MediusUniverseSvoURLResponse()
                                                {
                                                    MessageID = getUniverseInfo.MessageID,
                                                    URL = info.SvoURL,
                                                },
                                            },
                                            clientChannel
                                        );
                                    }
                                    #endregion

                                    if (
                                        getUniverseInfo.InfoType.HasFlag(
                                            MediusUniverseVariableInformationInfoFilter.INFO_DNS
                                        )
                                        || getUniverseInfo.InfoType.HasFlag(
                                            MediusUniverseVariableInformationInfoFilter.INFO_EXTRAINFO
                                        )
                                    )
                                    {
                                        var universeExtendedInfo = info.ExtendedInfo;

                                        // Special hotfix for the wildcard support in pre 0.8 Home clients.
                                        if (
                                            data.ClientObject != null
                                            && (
                                                data.ClientObject.ApplicationId == 20371
                                                || data.ClientObject.ApplicationId == 20374
                                            )
                                            && data.ClientObject.ClientHomeData != null
                                            && data.ClientObject.ClientHomeData.VersionAsDouble
                                                < 0.8
                                            && !string.IsNullOrEmpty(universeExtendedInfo)
                                            && universeExtendedInfo.StartsWith("*")
                                        )
                                            universeExtendedInfo = null;

                                        Queue(
                                            new RT_MSG_SERVER_APP()
                                            {
                                                Message =
                                                    new MediusUniverseVariableInformationResponse()
                                                    {
                                                        MessageID = getUniverseInfo.MessageID,
                                                        StatusCode =
                                                            MediusCallbackStatus.MediusSuccess,
                                                        InfoFilter = getUniverseInfo.InfoType,
                                                        UniverseID = info.UniverseId,
                                                        ExtendedInfo = universeExtendedInfo,
                                                        UniverseName = info.Name,
                                                        UniverseDescription = info.Description,
                                                        SvoURL = info.SvoURL,
                                                        Status = info.Status,
                                                        UserCount = info.UserCount,
                                                        MaxUsers = info.MaxUsers,
                                                        DNS = info.Endpoint,
                                                        Port = info.Port,
                                                        UniverseBilling = info.UniverseBilling,
                                                        BillingSystemName = info.BillingSystemName,
                                                        EndOfList = isLast,
                                                    },
                                            },
                                            clientChannel
                                        );
                                    }

                                    #region News
                                    if (
                                        getUniverseInfo.InfoType.HasFlag(
                                            MediusUniverseVariableInformationInfoFilter.INFO_NEWS
                                        )
                                    )
                                    {
#if DEBUG
                                        LoggerAccessor.LogInfo(
                                            "[MUISProcessor] - News bit set in request"
                                        );
#endif
                                        Queue(
                                            new RT_MSG_SERVER_APP()
                                            {
                                                Message = new MediusUniverseNewsResponse()
                                                {
                                                    MessageID = getUniverseInfo.MessageID,
                                                    StatusCode = MediusCallbackStatus.MediusSuccess,
                                                    News = "Simulated News",
                                                    EndOfList = isLast,
                                                },
                                            },
                                            clientChannel
                                        );
                                    }
                                    #endregion
                                }
#if DEBUG
                                LoggerAccessor.LogInfo(
                                    $"[MUISProcessor] - send univ info: [{HorizonServerConfiguration.MUISUniverses.ToArray().Length}]"
                                );
#endif
                            }
                        }
                        else
                        {
                            LoggerAccessor.LogWarn($"[MUISProcessor] - No universes out there.");

                            Queue(
                                new RT_MSG_SERVER_APP()
                                {
                                    Message = new MediusUniverseVariableInformationResponse()
                                    {
                                        MessageID = getUniverseInfo.MessageID,
                                        StatusCode = MediusCallbackStatus.MediusNoResult,
                                        InfoFilter = getUniverseInfo.InfoType,
                                        EndOfList = true,
                                    },
                                },
                                clientChannel
                            );
                        }
                    }
                    else
                    {
                        LoggerAccessor.LogWarn(
                            $"[MUISProcessor] - ApplicationID not compatible [{data.ApplicationId}]"
                        );

                        if (
                            getUniverseInfo.InfoType.HasFlag(
                                MediusUniverseVariableInformationInfoFilter.INFO_UNIVERSES
                            )
                        )
                        {
                            Queue(
                                new RT_MSG_SERVER_APP()
                                {
                                    Message = new MediusUniverseStatusListResponse()
                                    {
                                        MessageID = getUniverseInfo.MessageID,
                                        StatusCode = MediusCallbackStatus.MediusIncompatibleAppID,
                                        EndOfList = true,
                                    },
                                },
                                clientChannel
                            );
                        }
                        else
                        {
                            Queue(
                                new RT_MSG_SERVER_APP()
                                {
                                    Message = new MediusUniverseVariableInformationResponse()
                                    {
                                        MessageID = getUniverseInfo.MessageID,
                                        StatusCode = MediusCallbackStatus.MediusIncompatibleAppID,
                                        InfoFilter = getUniverseInfo.InfoType,
                                        EndOfList = true,
                                    },
                                },
                                clientChannel
                            );
                        }
                    }
                    break;
                }
                #endregion

                #region Channels

                case MediusChannelList_ExtraInfoRequest channelList_ExtraInfoRequest:
                {
                    var channelResponses = new List<MediusChannelList_ExtraInfoResponse>();

                    foreach (
                        var channel in Program.MUMManager.GetChannelList(
                            data.ApplicationId,
                            channelList_ExtraInfoRequest.PageID,
                            channelList_ExtraInfoRequest.PageSize,
                            ChannelType.Lobby
                        )
                    )
                    {
                        channelResponses.Add(
                            new MediusChannelList_ExtraInfoResponse()
                            {
                                MessageID = channelList_ExtraInfoRequest.MessageID,
                                StatusCode = MediusCallbackStatus.MediusSuccess,
                                MediusWorldID = channel.Id,
                                LobbyName = channel.Name,
                                GameWorldCount = (ushort)channel.GameCount,
                                PlayerCount = (ushort)channel.PlayerCount,
                                MaxPlayers = (ushort)channel.MaxPlayers,
                                GenericField1 = (uint)channel.GenericField1,
                                GenericField2 = (uint)channel.GenericField2,
                                GenericField3 = (uint)channel.GenericField3,
                                GenericField4 = (uint)channel.GenericField4,
                                GenericFieldLevel = channel.GenericFieldLevel,
                                SecurityLevel = channel.SecurityLevel,
                                EndOfList = false,
                            }
                        );
                    }

                    if (channelResponses.Count == 0)
                    {
                        Queue(
                            new RT_MSG_SERVER_APP()
                            {
                                Message = new MediusChannelList_ExtraInfoResponse()
                                {
                                    MessageID = channelList_ExtraInfoRequest.MessageID,
                                    StatusCode = MediusCallbackStatus.MediusNoResult,
                                    EndOfList = true,
                                },
                            },
                            clientChannel
                        );
                    }
                    else
                    {
                        // Ensure the end of list flag is set
                        channelResponses[^1].EndOfList = true;

                        // Add to responses
                        Queue(channelResponses, clientChannel);
                    }
                    break;
                }

                #endregion

                #region Time
                case MediusGetServerTimeRequest getServerTimeRequest:
                {
                    _ = GetTimeZone(DateTime.Now)
                        .ContinueWith(
                            (r) =>
                            {
                                if (r.IsCompletedSuccessfully)
                                {
                                    //Fetched
                                    Queue(
                                        new RT_MSG_SERVER_APP()
                                        {
                                            Message = new MediusGetServerTimeResponse()
                                            {
                                                MessageID = getServerTimeRequest.MessageID,
                                                StatusCode = MediusCallbackStatus.MediusSuccess,
                                                Local_server_timezone = r.Result,
                                            },
                                        },
                                        clientChannel
                                    );
                                }
                                else
                                {
                                    //default
                                    Queue(
                                        new RT_MSG_SERVER_APP()
                                        {
                                            Message = new MediusGetServerTimeResponse()
                                            {
                                                MessageID = getServerTimeRequest.MessageID,
                                                StatusCode = MediusCallbackStatus.MediusSuccess,
                                                Local_server_timezone =
                                                    MediusTimeZone.MediusTimeZone_GMT,
                                            },
                                        },
                                        clientChannel
                                    );
                                }
                            }
                        );
                    break;
                }
                #endregion

                default:
                {
                    LoggerAccessor.LogWarn(
                        $"[MUISProcessor] - UNHANDLED MEDIUS MESSAGE: {message}"
                    );
                    break;
                }
            }
        }

        #endregion

        #region Queue

        public void Queue(IEnumerable<BaseMediusMessage> messages, params IChannel[] clientChannels)
        {
            Queue(messages.Select(x => new RT_MSG_SERVER_APP() { Message = x }), clientChannels);
        }

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

        #region TimeZone
        public static Task<MediusTimeZone> GetTimeZone(DateTime time)
        {
            var tz = TimeZoneInfo.Local;
            var tzInt = Convert.ToInt32(tz.Id);

            var tzStanName = tz.StandardName;

            if (tzStanName == "CEST")
                return Task.FromResult(MediusTimeZone.MediusTimeZone_CEST);
            else if (tzInt == 83 && (tzInt + 1) == 83 && (tzInt + 2) == 84)
                return Task.FromResult(MediusTimeZone.MediusTimeZone_SWEDISHST);
            else if (tzInt == 70 && (tzInt + 1) == 83 && (tzInt + 2) == 84)
                return Task.FromResult(MediusTimeZone.MediusTimeZone_FST);
            else if (tzInt == 67 && (tzInt + 1) == 65 && (tzInt + 2) == 84)
                return Task.FromResult(MediusTimeZone.MediusTimeZone_CAT);
            else if (tzStanName == "SAST")
                return Task.FromResult(MediusTimeZone.MediusTimeZone_SAST);
            else if (tzInt == 69 && (tzInt + 1) == 65 && (tzInt + 2) == 84)
                return Task.FromResult(MediusTimeZone.MediusTimeZone_EET);
            else if (tzInt == 73 && (tzInt + 1) == 65 && (tzInt + 2) == 84)
                return Task.FromResult(MediusTimeZone.MediusTimeZone_ISRAELST);

            return Task.FromResult(MediusTimeZone.MediusTimeZone_GMT);
        }
        #endregion

        #region PokeEngine
        private bool CheatQuery(
            uint address,
            int Length,
            IChannel? clientChannel,
            CheatQueryType Type = CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY,
            int SequenceId = 1
        )
        {
            // address = 0, don't read
            if (address == 0)
                return false;

            // client channel is null, don't read
            if (clientChannel == null)
                return false;

            // read client memory
            Queue(
                new RT_MSG_SERVER_CHEAT_QUERY()
                {
                    QueryType = Type,
                    SequenceId = SequenceId,
                    StartAddress = address,
                    Length = Length,
                },
                clientChannel
            );

            // return read
            return true;
        }
        #endregion

        protected uint GenerateNewScertClientId()
        {
            return _clientCounter.CreateSequentialID();
        }

        public Task StopAsync()
        {
            return _MUISServer.StopAsync();
        }
    }
}
