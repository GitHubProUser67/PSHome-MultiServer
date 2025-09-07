using System.Net;
using CustomLogger;
using DotNetty.Transport.Channels;
using Horizon.RT.Cryptography;
using Horizon.RT.Models;
using Horizon.MUM.Models;
using Horizon.LIBRARY.Pipeline.Attribute;
using HorizonService.RT.Models.ServerPlugins.MAPS;
using HorizonService.ZipperPlugin.Models;
using HorizonService.ZipperPlugin;

namespace Horizon.SERVER.Medius
{
    public class MAPS : BaseMediusComponent
    {
        public override int TCPPort
        {
            get => MediusClass.Settings.MAPSTCPPort;
            set
            {
                throw new ArgumentOutOfRangeException(nameof(value), "[MAPS] - TCP Port can't be assigned.");
            }
        }

        public override int UDPPort
        {
            get => MediusClass.Settings.MAPSUDPPort;
            set
            {
                throw new ArgumentOutOfRangeException(nameof(value), "[MAPS] - UDP Port can't be assigned.");
            }
        }

        public static FactionManager factionManager = new FactionManager(0x7);

        public MAPS()
        {

        }
        public static void ReserveClient(ClientObject client)
        {
            MediusClass.Manager.AddClient(client);
        }

        protected override async Task ProcessMessage(BaseScertMessage message, IChannel clientChannel, ChannelData data)
        {
            // Get ScertClient data
            var scertClient = clientChannel.GetAttribute(LIBRARY.Pipeline.Constants.SCERT_CLIENT).Get();
            bool enableEncryption = MediusClass.GetAppSettingsOrDefault(data.ApplicationId).EnableEncryption;
            if (scertClient.CipherService != null)
                scertClient.CipherService.EnableEncryption = enableEncryption;

            switch (message)
            {
                case RT_MSG_CLIENT_HELLO clientHello:
                    {
                        // send hello
                        Queue(new RT_MSG_SERVER_HELLO() { RsaPublicKey = enableEncryption ? MediusClass.Settings.DefaultKey.N : Org.BouncyCastle.Math.BigInteger.Zero }, clientChannel);
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
                case RT_MSG_CLIENT_CONNECT_TCP clientConnectTcp:
                    {
                        #region Check if AppId from Client matches Server
                        if (!MediusClass.Manager.IsAppIdSupported(clientConnectTcp.AppId))
                        {
LoggerAccessor.LogError($"[MAPS] - Client {clientChannel.RemoteAddress} attempting to authenticate with incompatible app id {clientConnectTcp.AppId}");
                            await clientChannel.CloseAsync();
                            return;
                        }
                        #endregion

                        data.ApplicationId = clientConnectTcp.AppId;
                        scertClient.ApplicationID = clientConnectTcp.AppId;

                        Channel? targetChannel = MediusClass.Manager.GetChannelByChannelId(clientConnectTcp.TargetWorldId, data.ApplicationId);

                        if (targetChannel == null)
                        {
                            Channel DefaultChannel = MediusClass.Manager.GetOrCreateDefaultLobbyChannel(data.ApplicationId, scertClient.MediusVersion ?? 0);

                            if (DefaultChannel.Id == clientConnectTcp.TargetWorldId)
                                targetChannel = DefaultChannel;

                            if (targetChannel == null)
                            {
                                LoggerAccessor.LogError($"[MAPS] - Client: {clientConnectTcp.AccessToken} tried to join, but targetted WorldId:{clientConnectTcp.TargetWorldId} doesn't exist!");
                                await clientChannel.CloseAsync();
                                break;
                            }
                        }

                        // If booth are null, it means MAS client wants a new object.
                        if (!string.IsNullOrEmpty(clientConnectTcp.AccessToken) && !string.IsNullOrEmpty(clientConnectTcp.SessionKey))
                        {
                            data.ClientObject = MediusClass.Manager.GetClientByAccessToken(clientConnectTcp.AccessToken, clientConnectTcp.AppId);
                            if (data.ClientObject == null)
                                data.ClientObject = MediusClass.Manager.GetClientBySessionKey(clientConnectTcp.SessionKey, clientConnectTcp.AppId);
                        }

                        if (data.ClientObject != null)
                            LoggerAccessor.LogInfo($"[MAPS] - Client Connected {clientChannel.RemoteAddress}!");
                        else
                        {
                            LoggerAccessor.LogInfo($"[MAPS] - Client Connected {clientChannel.RemoteAddress} with new ClientObject!");

                            data.ClientObject = new(scertClient.MediusVersion ?? 0)
                            {
                                ApplicationId = clientConnectTcp.AppId
                            };
                            data.ClientObject.OnConnected();

                            ReserveClient(data.ClientObject); // We reserve a client on MAPS as MAG/SOCOM 4 call this before MAS Login!
                        }

                        data.ClientObject.MediusVersion = scertClient.MediusVersion ?? 0;
                        data.ClientObject.ApplicationId = clientConnectTcp.AppId;
                        data.ClientObject.OnConnected();

                        await data.ClientObject.JoinChannel(targetChannel);

                        Queue(new RT_MSG_SERVER_CONNECT_REQUIRE(), clientChannel);
                        break;
                    }
                case RT_MSG_CLIENT_CONNECT_READY_REQUIRE clientConnectReadyRequire:
                    {
                        Queue(new RT_MSG_SERVER_CONNECT_ACCEPT_TCP()
                        {
                            PlayerId = 0,
                            ScertId = GenerateNewScertClientId(),
                            PlayerCount = 0x0001,
                            IP = (clientChannel.RemoteAddress as IPEndPoint)?.Address
                        }, clientChannel);
                        break;
                    }
                case RT_MSG_CLIENT_CONNECT_READY_TCP clientConnectReadyTcp:
                    {
                        Queue(new RT_MSG_SERVER_CONNECT_COMPLETE() { ClientCountAtConnect = 0x0001 }, clientChannel);
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
                        if (clientAppToPlugin.Message != null)
                            ProcessMediusPluginMessage(clientAppToPlugin.Message, clientChannel, data);

                        break;
                    }

                case RT_MSG_SERVER_PLUGIN_TO_APP serverPluginToApp:
                    {

                        break;
                    }
                case RT_MSG_CLIENT_DISCONNECT _:
                case RT_MSG_CLIENT_DISCONNECT_WITH_REASON clientDisconnectWithReason:
                    {
                        data.State = ClientState.DISCONNECTED;
                        _ = clientChannel.CloseAsync();
                        break;
                    }
                default:
                    {
LoggerAccessor.LogWarn($"[MAPS] - UNHANDLED RT MESSAGE: {message}");

                        break;
                    }
            }
        }

        protected virtual void ProcessMediusPluginMessage(BaseMediusPluginMessage message, IChannel clientChannel, ChannelData data)
        {
            ScertClientAttribute? scertClient = clientChannel.GetAttribute(LIBRARY.Pipeline.Constants.SCERT_CLIENT).Get();
            if (message == null)
            {
                LoggerAccessor.LogError($"[MAPS] - ProcessMediusPluginMessage - MessageType is Null!");
                return;
            }

            switch (message)
            {

                case NetMessageHello netMessageHello:
                    {
                        //MAGDevBuild3 = 1725
                        //MAG BCET70016 v1.3 = 7002
                        data.ClientObject?.Queue(new NetMessageProtocolInfo()
                        {
                            protocolInfo = 1725,
                            buildNumber = 0
                        });
                        break;
                    }

                case NetMessageProtocolInfo protocolInfo:
                    {
                        // NOT WORKING YET.
                        data.ClientObject?.Queue(new NetMAPSHelloMessage()
                        {
                            m_success = true,
                            m_isOnline = true,
                            m_availableFactions = new CBitset3u() { m_bitArray = factionManager.GetMask() }
                        });
                        break;
                    }

                case NetMessageAccountLogoutRequest accountLogoutRequest:
                    {
                        // Nothing to timeout for now.

                        data.ClientObject?.Queue(new NetMessageAccountLogoutResponse()
                        {
                            m_success = true,
                        });

                        _ = clientChannel.CloseAsync();

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
