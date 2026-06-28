using System.Net;
using CustomLogger;
using DotNetty.Transport.Channels;
using EndianTools;
using Horizon.CustomServers.Models;
using Horizon.MEDIUS.Models;
using Horizon.MUM.Models;
using Horizon.RT.Cryptography;
using Horizon.RT.Models;
using Horizon.RT.Models.ServerPlugins.MAPS;
using Horizon.ZipperPlugin;
using Horizon.ZipperPlugin.Models;
using MultiServerLibrary.Extension;
using Org.BouncyCastle.Math;

namespace Horizon.MEDIUS.Processors
{
    public class MAPS : BaseMediusProcessor
    {
        public override ushort TCPPort
        {
            get => HorizonServerConfiguration.MEDIUSMAPSTCPPort;
            set
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    "[MAPS] - TCP Port can't be assigned."
                );
            }
        }

        public override ushort UDPPort
        {
            get => HorizonServerConfiguration.MEDIUSMAPSUDPPort;
            set
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    "[MAPS] - UDP Port can't be assigned."
                );
            }
        }

        private static readonly BigInteger GlobalRsaPublicKey = BigInteger.Zero; // No MAPS encryption for now.

        private static readonly FactionManager _factionManager = new(0x7);

        public MAPS() { }

        public static void ReserveClient(ClientObject client)
        {
            Program.MUMManager.AddClient(client);
        }

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
                                : BigInteger.Zero,
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
                case RT_MSG_CLIENT_CONNECT_TCP clientConnectTcp:
                {
                    var appid = clientConnectTcp.AppId;

                    #region Check if AppId from Client matches Server
                    if (appid < 0)
                    {
                        LoggerAccessor.LogError(
                            $"[MAPS] - Client Connected {clientChannel.RemoteAddress} with an invalid connect payload!"
                        );
                        break;
                    }
                    else if (!Program.MUMManager.IsAppIdSupported(appid))
                    {
                        LoggerAccessor.LogError(
                            $"[MAPS] - Client {clientChannel.RemoteAddress} attempting to authenticate with incompatible app id {appid}"
                        );
                        await clientChannel.CloseAsync();
                        return;
                    }
                    #endregion

                    if (clientConnectTcp.Key == RSA_KEY.Empty)
                    {
                        LoggerAccessor.LogError(
                            $"[MAPS] - Client Connected {clientChannel.RemoteAddress} with an empty key!"
                        );
                        break;
                    }

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
                                $"[MAPS] - Client: {clientConnectTcp.AccessToken} tried to join, but targetted WorldId:{clientConnectTcp.TargetWorldId} doesn't exist!"
                            );
                            await clientChannel.CloseAsync();
                            break;
                        }
                    }

                    // If booth are null, it means MAS client wants a new object.
                    if (
                        !string.IsNullOrEmpty(clientConnectTcp.AccessToken)
                        && !string.IsNullOrEmpty(clientConnectTcp.SessionKey)
                    )
                    {
                        data.ClientObject = Program.MUMManager.GetClientByAccessToken(
                            clientConnectTcp.AccessToken,
                            appid
                        );
                        data.ClientObject ??= Program.MUMManager.GetClientBySessionKey(
                            clientConnectTcp.SessionKey,
                            appid
                        );
                    }

                    if (data.ClientObject != null)
                        LoggerAccessor.LogInfo(
                            $"[MAPS] - Client Connected {clientChannel.RemoteAddress}!"
                        );
                    else
                    {
                        LoggerAccessor.LogInfo(
                            $"[MAPS] - Client Connected {clientChannel.RemoteAddress} with new ClientObject!"
                        );

                        data.ClientObject = new(scertClient.MediusVersion ?? 0)
                        {
                            ApplicationId = appid,
                        };
                        data.ClientObject.OnConnected();

                        ReserveClient(data.ClientObject); // We reserve a client on MAPS as MAG/SOCOM 4 call this before MAS Login!
                    }

                    data.ClientObject.MediusVersion = scertClient.MediusVersion ?? 0;
                    data.ClientObject.ApplicationId = appid;
                    data.ClientObject.OnConnected();

                    await data.ClientObject.JoinChannel(targetChannel);

                    Queue(new RT_MSG_SERVER_CONNECT_REQUIRE(), clientChannel);
                    break;
                }
                case RT_MSG_CLIENT_CONNECT_READY_REQUIRE clientConnectReadyRequire:
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
                    break;
                }
                case RT_MSG_CLIENT_CONNECT_READY_TCP clientConnectReadyTcp:
                {
                    Queue(
                        new RT_MSG_SERVER_CONNECT_COMPLETE() { ClientCountAtConnect = 0x0001 },
                        clientChannel
                    );
                    Queue(new RT_MSG_SERVER_ECHO(), clientChannel);
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
                    break;
                }

                case RT_MSG_CLIENT_APP_TO_PLUGIN clientAppToPlugin:
                {
                    await ProcessMediusPluginMessage(clientAppToPlugin.Message, clientChannel, data).ConfigureAwait(false);

                    break;
                }

                case RT_MSG_SERVER_PLUGIN_TO_APP serverPluginToApp:
                {
                    break;
                }
                case RT_MSG_CLIENT_DISCONNECT _:
                case RT_MSG_CLIENT_DISCONNECT_WITH_REASON clientDisconnectWithReason:
                {
                    data.State = ServerClientState.DISCONNECTED;

                        await clientChannel
                                 .CloseAsync()
                                 .TryAwait(TimeSpan.FromMilliseconds(2000))
                                 .ConfigureAwait(false); break;
                }
                default:
                {
                    LoggerAccessor.LogWarn($"[MAPS] - UNHANDLED RT MESSAGE: {message}");

                    break;
                }
            }
        }

        protected virtual async Task ProcessMediusPluginMessage(
            BaseMediusPluginMessage message,
            IChannel clientChannel,
            ChannelData data
        )
        {
            if (message == null)
            {
                LoggerAccessor.LogError(
                    $"[MAPS] - ProcessMediusPluginMessage - MessageType is Null!"
                );
                return;
            }

            switch (message)
            {
                case NetMessageHello netMessageHello:
                {
                    //MAGDevBuild3 = 1725
                    //MAG BCET70016 v1.3 = 7002
                    data.ClientObject?.Queue(
                        new NetMessageProtocolInfo() { protocolVersion = 1725, buildNumber = 0 }
                    );
                    break;
                }

                case NetMessageProtocolInfo protocolInfo:
                {
                    data.ClientObject?.Queue(
                        new NetMAPSHelloMessage()
                        {
                            RsaPublicKey = GlobalRsaPublicKey,
                            m_success = true,
                            m_isOnline = true,
                            m_availableFactions = new CBitset3u()
                            {
                                m_bitArray = _factionManager.GetMask(),
                            },
                        }
                    );
                    break;
                }

                case NetMessageUniverseListRequest universeListRequest:
                    {
                        if (
                            HorizonServerConfiguration.MAPSUniverses.TryGetValue(
                                data.ApplicationId,
                                out var infos
                            )
                        )
                        {
                            foreach (var info in infos)
                            {
                                data.ClientObject?.Queue(new RT_MSG_SERVER_PLUGIN_TO_APP()
                                {
                                    Message = new NetMessageUniverseListResponse
                                    {
                                        RsaPublicKey = universeListRequest.RsaPublicKey,
                                        m_transId = universeListRequest.m_transId,
                                        m_success = true,
                                        m_isLast = infos.LastOrDefault() == info,
                                        UniverseName = info.Name,
                                        UniverseAuthDNS = info.AuthDNS,
                                        UniverseAuthIP = info.AuthIP,
                                        UniverseSvoURL = info.SvoURL,
                                        UniversePort = info.Port,
                                        UniverseId = info.UniverseId
                                    }
                                });
                            }
                        }
                        else
                        {
                            LoggerAccessor.LogWarn($"[MAPS] - No universes out there.");

                            data.ClientObject?.Queue(new RT_MSG_SERVER_PLUGIN_TO_APP()
                            {
                                Message = new NetMessageUniverseListResponse
                                {
                                    RsaPublicKey = universeListRequest.RsaPublicKey,
                                    m_transId = universeListRequest.m_transId,
                                    m_success = false
                                }
                            });
                        }

                        break;
                    }

                case NetMessageAccountLogoutRequest accountLogoutRequest:
                {
                    // Nothing to timeout for now.

                    data.ClientObject?.Queue(
                        new NetMessageAccountLogoutResponse() { m_success = true }
                    );

                    await clientChannel
                                   .CloseAsync()
                                   .TryAwait(TimeSpan.FromMilliseconds(2000))
                                   .ConfigureAwait(false);

                    LoggerAccessor.LogWarn($"[MAPS] - Client disconnected by request");

                    break;
                }

                default:
                {
                    LoggerAccessor.LogWarn($"[MAPS] - Unhandled Medius Plugin Message: {message}");
                    break;
                }
            }
        }
    }
}
