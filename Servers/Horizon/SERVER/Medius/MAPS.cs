using CustomLogger;
using DotNetty.Transport.Channels;
using Horizon.RT.Cryptography;
using Horizon.RT.Models;
using Horizon.RT.Models.ServerPlugins;
using System.Net;
using Horizon.MUM.Models;
using Horizon.LIBRARY.Pipeline.Attribute;
using EndianTools;

namespace Horizon.SERVER.Medius
{
    public class MAPS : BaseMediusComponent
    {
        public override int TCPPort => MediusClass.Settings.MAPSTCPPort;
        public override int UDPPort => MediusClass.Settings.MAPSUDPPort;

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
                        int appid = clientConnectTcp.AppId;

                        #region Check if AppId from Client matches Server
                        if (appid < 0)
                        {
                            LoggerAccessor.LogError($"[MAPS] - Client Connected {clientChannel.RemoteAddress} with an invalid connect payload!");
                            break;
                        }
                        else if (!MediusClass.Manager.IsAppIdSupported(appid))
                        {
                            LoggerAccessor.LogError($"[MAPS] - Client {clientChannel.RemoteAddress} attempting to authenticate with incompatible app id {appid}");
                            await clientChannel.CloseAsync();
                            return;
                        }
                        #endregion

                        if (clientConnectTcp.Key == RSA_KEY.Empty)
                        {
                            LoggerAccessor.LogError($"[MAPS] - Client Connected {clientChannel.RemoteAddress} with an empty key!");
                            break;
                        }

                        data.ApplicationId = appid;
                        scertClient.ApplicationID = appid;

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
                            data.ClientObject = MediusClass.Manager.GetClientByAccessToken(clientConnectTcp.AccessToken, appid);
                            if (data.ClientObject == null)
                                data.ClientObject = MediusClass.Manager.GetClientBySessionKey(clientConnectTcp.SessionKey, appid);
                        }

                        if (data.ClientObject != null)
                            LoggerAccessor.LogInfo($"[MAPS] - Client Connected {clientChannel.RemoteAddress}!");
                        else
                        {
                            LoggerAccessor.LogInfo($"[MAPS] - Client Connected {clientChannel.RemoteAddress} with new ClientObject!");

                            data.ClientObject = new(scertClient.MediusVersion ?? 0)
                            {
                                ApplicationId = appid
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
                        LoggerAccessor.LogWarn($"UNHANDLED RT MESSAGE: {message}");

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
                        data.ClientObject.Queue(new NetMessageTypeProtocolInfo()
                        {
                            protocolInfo = EndianUtils.ReverseUint(1725), //1725 //1958
                            //protocolInfo = 1958,
                            buildNumber = EndianUtils.ReverseUint(0)
                        });
                        

                        break;
                    }

                case NetMessageTypeProtocolInfo protocolInfo:
                    {
                        byte[] availFactions = new byte[] { 0b00000111, 0, 0, 0 }; //= new byte[4];

                        //0b11100000
                        //availFactions[0] = 0;
                        //availFactions[1] = 0;
                        //availFactions[2] = 0;
                        //availFactions[3] = 31;

                        data.ClientObject.Queue(new NetMAPSHelloMessage()
                        {
                            m_success = true,
                            m_isOnline = true,
                            m_availableFactions = availFactions

                        });

                        //Time
                        DateTime time = DateTime.Now;
                        long timeBS = time.Ticks >> 1;

                        //bool finBs = true >> 1;
                        //Content string bitshift
                        //string newsBs = ShiftString("Test News");
                        //string eulaBs = ShiftString("Test Eula");
                        // News/Eula Type bitshifted
                        int newsBS = 0;//Convert.ToInt32(NetMessageNewsEulaResponseContentType.News) >> 1;
                        int eulaBS = 1;//Convert.ToInt32(NetMessageNewsEulaResponseContentType.Eula) >> 1;

                        byte[] sequence = new byte[1];
                        byte[] type = new byte[1];

                        /*
                        data.ClientObject.Queue(new NetMessageNewsEulaRequest()
                        {
                            m_languageExtension = "",
                        });
                        */
                        /*
                        data.ClientObject.Queue(new NetMessageNewsEulaResponse()
                        {
                            m_finished = BitShift(sequence, 1).First(),
                            m_content = newsBs,
                            m_type = (NetMessageNewsEulaResponseContentType)BitShift(type, 1).First(),
                            m_timestamp = timeBS
                        });
                        data.ClientObject.Queue(new NetMessageNewsEulaResponse()
                        {
                            m_finished = 1,
                            m_content = eulaBs,
                            m_type = (NetMessageNewsEulaResponseContentType)eulaBS,
                            m_timestamp = timeBS
                        });
                        
                        */

                        break;
                    }

                    /*
                case NetMessageTypeKeepAlive keepAlive:
                    {
                        data.ClientObject.KeepAliveUntilNextConnection();
                        break;
                    }
                    */

                case NetMessageAccountLogoutRequest accountLogoutRequest:
                    {
                        bool success = true;
                        data.ClientObject?.Queue(new NetMessageAccountLogoutResponse()
                        {
                            m_success = success,
                        });

                        break;
                    }

                default:
                    {
                        LoggerAccessor.LogWarn($"Unhandled Medius Plugin Message: {message}");
                        break;
                    }
            }
        }
    }
}
