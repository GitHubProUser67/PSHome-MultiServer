using CustomLogger;
using DotNetty.Transport.Channels;
using Horizon.RT.Common;
using Horizon.RT.Cryptography;
using Horizon.RT.Models;
using Horizon.LIBRARY.Common;
using Horizon.SERVER.Config;
using Horizon.SERVER.PluginArgs;
using Horizon.LIBRARY.Pipeline.Attribute;
using System.Net;
using Horizon.PluginManager;
using Horizon.RT.Models.MGCL;
using Horizon.MUM.Models;
using Horizon.DME;
using MultiServerLibrary.Extension;

namespace Horizon.SERVER.Medius
{
    public class MPS : BaseMediusComponent
    {
        public override int TCPPort
        {
            get => MediusClass.Settings.MPSPort;
            set
            {
                throw new ArgumentOutOfRangeException(nameof(value), "[MPS] - TCP Port can't be assigned.");
            }
        }

        public override int UDPPort
        {
            get => 0;
            set
            {
                throw new ArgumentOutOfRangeException(nameof(value), "[MPS] - UDP Port can't be assigned.");
            }
        }

        private static DateTime lastSend = DateTimeUtils.GetHighPrecisionUtcTime();

        private static IChannel? channel = null;
        private static ChannelData? channelData = null;

        public MPS()
        {

        }

        protected override Task OnConnected(IChannel clientChannel)
        {
            // Get ScertClient data
            if (!clientChannel.HasAttribute(LIBRARY.Pipeline.Constants.SCERT_CLIENT))
                clientChannel.GetAttribute(LIBRARY.Pipeline.Constants.SCERT_CLIENT).Set(new ScertClientAttribute());
            var scertClient = clientChannel.GetAttribute(LIBRARY.Pipeline.Constants.SCERT_CLIENT).Get();
            scertClient.RsaAuthKey = MediusClass.Settings.MPSKey;
            scertClient.CipherService?.GenerateCipher(MediusClass.Settings.MPSKey);

            return base.OnConnected(clientChannel);
        }

        protected override async Task ProcessMessage(BaseScertMessage message, IChannel clientChannel, ChannelData data)
        {
            channelData = data;
            channel = clientChannel;

            // Get ScertClient data
            var scertClient = clientChannel.GetAttribute(LIBRARY.Pipeline.Constants.SCERT_CLIENT).Get();
            bool enableEncryption = MediusClass.GetAppSettingsOrDefault(data.ApplicationId).EnableEncryption;
            if (scertClient.CipherService != null)
                scertClient.CipherService.EnableEncryption = enableEncryption;

            switch (message)
            {
                case RT_MSG_CLIENT_HELLO clientHello:
                    {
                        if (data.State > ClientState.HELLO)
                        {
LoggerAccessor.LogError($"[MPS] - Unexpected RT_MSG_CLIENT_HELLO from {clientChannel.RemoteAddress}: {clientHello}");
                            break;
                        }

                        data.State = ClientState.HELLO;
                        Queue(new RT_MSG_SERVER_HELLO() { RsaPublicKey = enableEncryption ? MediusClass.Settings.DefaultKey.N : Org.BouncyCastle.Math.BigInteger.Zero }, clientChannel);
                        break;
                    }
                case RT_MSG_CLIENT_CRYPTKEY_PUBLIC clientCryptKeyPublic:
                    {
                        if (data.State > ClientState.HANDSHAKE)
                        {
LoggerAccessor.LogError($"[MPS] - Unexpected RT_MSG_CLIENT_CRYPTKEY_PUBLIC from {clientChannel.RemoteAddress}: {clientCryptKeyPublic}");
                            break;
                        }

                        data.State = ClientState.CONNECT_1;

                        // generate new client session key
                        scertClient.CipherService?.GenerateCipher(CipherContext.RSA_AUTH, clientCryptKeyPublic.PublicKey.Reverse().ToArray());
                        scertClient.CipherService?.GenerateCipher(CipherContext.RC_CLIENT_SESSION);

                        Queue(new RT_MSG_SERVER_CRYPTKEY_PEER() { SessionKey = scertClient.CipherService?.GetPublicKey(CipherContext.RC_CLIENT_SESSION) }, clientChannel);
                        break;
                    }
                case RT_MSG_CLIENT_CONNECT_TCP clientConnectTcp:
                    {
                        if (data.State > ClientState.CONNECT_1)
                        {
LoggerAccessor.LogError($"[MPS] - Unexpected RT_MSG_CLIENT_CONNECT_TCP from {clientChannel.RemoteAddress}: {clientConnectTcp}");
                            break;
                        }

                        List<int> pre108ServerComplete = new() { 10114, 10164, 10190, 10124, 10130, 10164, 10284, 10330, 10334, 10414, 10421, 10442, 10538, 10540, 10550, 10582, 10584, 10680, 10681, 10683, 10684, 10724 };

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
                                LoggerAccessor.LogError($"[MPS] - Client: {clientConnectTcp.AccessToken} tried to join, but targetted WorldId:{clientConnectTcp.TargetWorldId} doesn't exist!");
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

                            if (data.ClientObject != null)
                                LoggerAccessor.LogInfo($"[MPS] - Client Connected {clientChannel.RemoteAddress}!");
                            else
                            {
                                data.Ignore = true;
                                LoggerAccessor.LogError($"[MPS] - ClientObject could not be granted for {clientChannel.RemoteAddress}: {clientConnectTcp}");
                                break;
                            }

                            data.ClientObject.MediusVersion = scertClient.MediusVersion ?? 0;
                            data.ClientObject.ApplicationId = clientConnectTcp.AppId;
                            data.ClientObject.OnConnected();
                        }
                        else // MAG uses MPS directly to register a ClientObject.
                        {
                            LoggerAccessor.LogInfo($"[MPS] - Client Connected {clientChannel.RemoteAddress} with new ClientObject!");

                            data.ClientObject = new(scertClient.MediusVersion ?? 0)
                            {
                                ApplicationId = clientConnectTcp.AppId
                            };
                            data.ClientObject.OnConnected();

                            MAS.ReserveClient(data.ClientObject); // ONLY RESERVE CLIENTS HERE!
                        }

                        await data.ClientObject.JoinChannel(targetChannel);

                        data.State = ClientState.AUTHENTICATED;

                        #region if PS3
                        if (scertClient.IsPS3Client)
                        {
                            List<int> ConnectAcceptTCPGames = new() { 20623, 20624, 21564, 21574, 21584, 21594, 22274, 22284, 22294, 22304, 20040, 20041, 20042, 20043, 20044 };

                            //CAC & Warhawk
                            if (ConnectAcceptTCPGames.Contains(data.ApplicationId))
                            {
                                Queue(new RT_MSG_SERVER_CONNECT_ACCEPT_TCP()
                                {
                                    PlayerId = 0,
                                    ScertId = GenerateNewScertClientId(),
                                    PlayerCount = 0x0001,
                                    IP = (clientChannel.RemoteAddress as IPEndPoint)?.Address
                                }, clientChannel);
                            }
                            else
                                Queue(new RT_MSG_SERVER_CONNECT_REQUIRE(), clientChannel);
                        }
                        #endregion
                        else if (scertClient.MediusVersion > 108 && scertClient.ApplicationID != 11484)
                            Queue(new RT_MSG_SERVER_CONNECT_REQUIRE(), clientChannel);
                        else
                        {
                            //Older Medius titles do NOT use CRYPTKEY_GAME, newer ones have this.
                            if (scertClient.CipherService != null && scertClient.CipherService.HasKey(CipherContext.RC_CLIENT_SESSION) && scertClient.MediusVersion >= 109)
                                Queue(new RT_MSG_SERVER_CRYPTKEY_GAME() { GameKey = scertClient.CipherService.GetPublicKey(CipherContext.RC_CLIENT_SESSION) }, clientChannel);
                            Queue(new RT_MSG_SERVER_CONNECT_ACCEPT_TCP()
                            {
                                PlayerId = 0,
                                ScertId = GenerateNewScertClientId(),
                                PlayerCount = 0x0001,
                                IP = (clientChannel.RemoteAddress as IPEndPoint)?.Address
                            }, clientChannel);

                            if (pre108ServerComplete.Contains(data.ApplicationId))
                                Queue(new RT_MSG_SERVER_CONNECT_COMPLETE() { ClientCountAtConnect = 0x0001 }, clientChannel);
                        }

                        break;
                    }
                case RT_MSG_CLIENT_CONNECT_READY_REQUIRE clientConnectReadyRequire:
                    {
                        if (scertClient.CipherService != null && scertClient.CipherService.HasKey(CipherContext.RC_CLIENT_SESSION) && !scertClient.IsPS3Client)
                            Queue(new RT_MSG_SERVER_CRYPTKEY_GAME() { GameKey = scertClient.CipherService.GetPublicKey(CipherContext.RC_CLIENT_SESSION) }, clientChannel);
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
                        Queue(new RT_MSG_CLIENT_ECHO() { Value = clientEcho.Value }, clientChannel);
                        break;
                    }
                case RT_MSG_CLIENT_APP_TOSERVER clientAppToServer:
                    {
                        if (data.State != ClientState.AUTHENTICATED)
                        {
LoggerAccessor.LogError($"[MPS] - Unexpected RT_MSG_CLIENT_APP_TOSERVER from {clientChannel.RemoteAddress}: {clientAppToServer}");
                            break;
                        }

                        await ProcessMediusMessage(clientAppToServer.Message, clientChannel, data);
                        break;
                    }
                case RT_MSG_CLIENT_DISCONNECT _:
                    {
                        //Medius 1.08 (Used on WRC 4) haven't a state
                        if (scertClient.MediusVersion > 108)
                            data.State = ClientState.DISCONNECTED;

                        await clientChannel.CloseAsync();

                        LoggerAccessor.LogInfo($"[MPS] - Client disconnected by request with no specific reason\n");
                        break;
                    }
                case RT_MSG_CLIENT_DISCONNECT_WITH_REASON clientDisconnectWithReason:
                    {
                        if (clientDisconnectWithReason.Reason <= RT_MSG_CLIENT_DISCONNECT_REASON.RT_MSG_CLIENT_DISCONNECT_LENGTH_MISMATCH)
                            LoggerAccessor.LogInfo($"[MPS] - Disconnected by request with reason of {clientDisconnectWithReason.Reason}\n");
                        else
                            LoggerAccessor.LogInfo($"[MPS] - Disconnected by request with (application specified) reason of {clientDisconnectWithReason.Reason}\n");

                        data.State = ClientState.DISCONNECTED;
                        await clientChannel.CloseAsync();
                        break;
                    }
                default:
                    {
LoggerAccessor.LogWarn($"[MPS] - UNHANDLED RT MESSAGE: {message}");
                        break;
                    }
            }

            return;
        }

        protected virtual async Task ProcessMediusMessage(BaseMediusMessage? message, IChannel clientChannel, ChannelData data)
        {
            if (message == null)
                return;

            switch (message)
            {
                #region MediusServerCreateGameWithAttributesResponse
                case MediusServerCreateGameWithAttributesResponse createGameWithAttrResponse:
                    {
                        if (!string.IsNullOrEmpty(createGameWithAttrResponse.MessageID.Value) && createGameWithAttrResponse.MessageID.Value.Contains("-") && data.ClientObject != null)
                        {
                            bool handled = false;
                            int partyType = 0;
                            int gameOrPartyId = 0;
                            int accountId = 0;
                            string msgId = string.Empty;

                            string[] messageParts = createGameWithAttrResponse.MessageID.Value.Split('-');

                            // This is an ugly hack, anonymous accounts can have a negative ID which messes up the traditional parser, also handles worldid negative (in case of a fail).
                            try
                            {
                                if (messageParts.Length == 6)
                                {
                                    gameOrPartyId = -int.Parse(messageParts[1]);
                                    accountId = -int.Parse(messageParts[3]);
                                    msgId = messageParts[4];
                                    partyType = int.Parse(messageParts[5]);

                                    handled = true;
                                }
                                else if (messageParts.Length == 5)
                                {
                                    if (string.IsNullOrEmpty(messageParts[0]))
                                    {
                                        gameOrPartyId = -int.Parse(messageParts[1]);
                                        accountId = int.Parse(messageParts[2]);
                                        msgId = messageParts[3];
                                    }
                                    else
                                    {
                                        gameOrPartyId = int.Parse(messageParts[0]);
                                        accountId = -int.Parse(messageParts[2]);
                                        msgId = messageParts[3];
                                    }

                                    partyType = int.Parse(messageParts[4]);

                                    handled = true;
                                }
                                else if (int.TryParse(messageParts[0], out gameOrPartyId) &&
                                    int.TryParse(messageParts[1], out accountId))
                                {
                                    msgId = messageParts[2];
                                    partyType = int.Parse(messageParts[3]);
                                    handled = true;
                                }
                            }
                            catch (Exception ex)
                            {
                                LoggerAccessor.LogError($"[MPS] - An assertion was thrown while parsing createGameWithAttrResponse MessageID: {ex}");
                            }

                            if (!handled)
                            {
                                LoggerAccessor.LogWarn("[MPS] - createGameWithAttrResponse received an invalid MessageID, ignoring request...");
                                break;
                            }

                            Game? game = MediusClass.Manager.GetGameByGameId(gameOrPartyId);
                            Party? party = MediusClass.Manager.GetPartyByPartyId(gameOrPartyId);
                            ClientObject? rClient = MediusClass.Manager.GetClientByAccountId(accountId, data.ClientObject.ApplicationId);

                            if (rClient == null)
                            {
                                LoggerAccessor.LogError($"[MPS] - MediusServerCreateGameWithAttributesResponse - Tried to grab a ClientObject, but AccountId:{accountId} on AppId:{data.ClientObject.ApplicationId} returned no result!");
                                break;
                            }

                            if (partyType == 1 && party != null)
                            {
                                party.MediusWorldId = createGameWithAttrResponse.MediusWorldId;
                                await party.PartyCreated();
                                rClient.Queue(new MediusPartyCreateResponse()
                                {
                                    MessageID = new MessageId(msgId),
                                    MediusWorldID = gameOrPartyId,
                                    StatusCode = MediusCallbackStatus.MediusSuccess
                                });
                            }
                            else if (data.ApplicationId == 22920 || data.ApplicationId == 21834)
                            {
                                //Send Matchmaking create game
                                if (!createGameWithAttrResponse.IsSuccess)
                                {
                                    rClient.Queue(new MediusMatchCreateGameResponse()
                                    {
                                        MessageID = new MessageId(msgId),
                                        StatusCode = MediusCallbackStatus.MediusFail
                                    });

                                    if (game != null)
                                        await game.EndGame(rClient.ApplicationId);
                                }
                                else if (game != null)
                                {
                                    game.MediusWorldId = game.GameChannel!.Id = createGameWithAttrResponse.MediusWorldId;
                                    await game.GameCreated();

                                    rClient.Queue(new MediusMatchCreateGameResponse()
                                    {
                                        MessageID = new MessageId(msgId),
                                        StatusCode = MediusCallbackStatus.MediusSuccess,
                                        MediusWorldID = game.MediusWorldId,
                                        SystemSpecificStatusCode = 0,
                                        RequestData = rClient.requestData,
                                        ApplicationDataSize = rClient.appDataSize,
                                        ApplicationData = rClient.appData,
                                    });

                                    // Send to plugins
                                    await MediusClass.Plugins.OnEvent(PluginEvent.MEDIUS_GAME_ON_CREATED, new OnPlayerGameArgs() { Player = rClient, Game = game });
                                }
                            }
                            else
                            {
                                if (!createGameWithAttrResponse.IsSuccess)
                                {
                                    rClient.Queue(new MediusCreateGameResponse()
                                    {
                                        MessageID = new MessageId(msgId),
                                        StatusCode = MediusCallbackStatus.MediusFail
                                    });

                                    if (game != null)
                                        await game.EndGame(rClient.ApplicationId);
                                }
                                else if (game != null)
                                {
                                    game.MediusWorldId = game.GameChannel!.Id = createGameWithAttrResponse.MediusWorldId;
                                    await game.GameCreated();
                                    rClient.Queue(new MediusCreateGameResponse()
                                    {
                                        MessageID = new MessageId(msgId),
                                        StatusCode = MediusCallbackStatus.MediusSuccess,
                                        MediusWorldID = game.MediusWorldId
                                    });

                                    // Send to plugins
                                    await MediusClass.Plugins.OnEvent(PluginEvent.MEDIUS_GAME_ON_CREATED, new OnPlayerGameArgs() { Player = rClient, Game = game });
                                }
                            }
                        }
                        else
                        {
                            data.ClientObject?.Queue(new MediusCreateGameResponse()
                            {
                                MessageID = new MessageId(createGameWithAttrResponse.MessageID.Value),
                                StatusCode = MediusCallbackStatus.MediusSuccess,
                                MediusWorldID = createGameWithAttrResponse.MediusWorldId
                            });

                            // Send to plugins
                            await MediusClass.Plugins.OnEvent(PluginEvent.MEDIUS_GAME_ON_CREATED, new OnPlayerGameArgs() { Player = data.ClientObject, Game = data.ClientObject?.CurrentGame });
                        }

                        break;
                    }
                #endregion

                #region MediusServerJoinGameResponse
                case MediusServerJoinGameResponse joinGameResponse:
                    {
                        if (data.ClientObject != null)
                        {
                            bool offseted = false;
                            int partyType = 0;
                            int gameOrPartyId = 0;
                            int accountId = 0;
                            string msgId = string.Empty;
                            List<int> approvedMaxPlayersAppIds = new() { 20624, 22500, 22920, 22924, 22930, 23360, 24000, 24180 };

                            string[]? messageParts = joinGameResponse.MessageID?.Value.Split('-');

                            if ((data.ClientObject.ApplicationId == 20371 || data.ClientObject.ApplicationId == 20374) && data.ClientObject.ClientHomeData != null && data.ClientObject.ClientHomeData.VersionAsDouble >= 01.21)
                                approvedMaxPlayersAppIds.Add(data.ClientObject.ApplicationId);

                            if (messageParts != null && messageParts.Length == 5) // This is an ugly hack, anonymous accounts can have a negative ID which messes up the traditional parser.
                            {
                                offseted = true;
                                gameOrPartyId = int.Parse(messageParts[0]);
                                accountId = -int.Parse(messageParts[2]);
                                msgId = messageParts[3];
                            }
                            else if (messageParts != null && int.TryParse(messageParts[0], out gameOrPartyId) &&
                                int.TryParse(messageParts[1], out accountId))
                                msgId = messageParts[2];
                            else
                            {
                                LoggerAccessor.LogWarn("[MPS] - joinGameResponse received an invalid MessageID, ignoring request...");
                                break;
                            }

                            Game? game = MediusClass.Manager.GetGameByGameId(gameOrPartyId);
                            Party? party = MediusClass.Manager.GetPartyByPartyId(gameOrPartyId);
                            ClientObject? rClient = MediusClass.Manager.GetClientByAccountId(accountId, data.ClientObject.ApplicationId);

                            try
                            {
                                if (offseted)
                                    partyType = int.Parse(messageParts[4]);
                                else
                                    partyType = int.Parse(messageParts[3]);
                            }
                            catch
                            {
                                // Not Important.
                            }

                            IPHostEntry host = Dns.GetHostEntry(MediusClass.Settings.NATIp ?? "natservice.pdonline.scea.com");

                            if (!joinGameResponse.IsSuccess)
                            {
                                rClient?.Queue(new MediusJoinGameResponse()
                                {
                                    SetMaxPlayers = approvedMaxPlayersAppIds.Contains(data.ClientObject.ApplicationId),
                                    MessageID = new MessageId(msgId),
                                    StatusCode = MediusCallbackStatus.MediusFail
                                });
                            }
                            else if (partyType == 1 && party != null)
                            {
                                if (rClient != null)
                                {
                                    // Join game DME
                                    await rClient.JoinParty(party, party.MediusWorldId);

                                    rClient?.Queue(new MediusPartyJoinByIndexResponse()
                                    {
                                        SetMaxPlayers = approvedMaxPlayersAppIds.Contains(data.ClientObject.ApplicationId),
                                        MessageID = new MessageId(msgId),
                                        StatusCode = MediusCallbackStatus.MediusSuccess,
                                        PartyHostType = party.PartyHostType,
                                        ConnectionInfo = new NetConnectionInfo()
                                        {
                                            AccessKey = rClient.AccessToken,
                                            SessionKey = rClient.SessionKey,
                                            TargetWorldID = party.MediusWorldId,
                                            ServerKey = joinGameResponse.pubKey,
                                            AddressList = new NetAddressList()
                                            {
                                                AddressList = new NetAddress[Constants.NET_ADDRESS_LIST_COUNT]
                                                        {
                                                    new NetAddress() { Address = data.ClientObject.IP.MapToIPv4().ToString(), Port = data.ClientObject.Port, AddressType = NetAddressType.NetAddressTypeExternal},
                                                    new NetAddress() { Address = host.AddressList.First().ToString(), Port = MediusClass.Settings.NATPort, AddressType = NetAddressType.NetAddressTypeNATService },
                                                        }
                                            },
                                            Type = NetConnectionType.NetConnectionTypeClientServerTCPAuxUDP
                                        },
                                        partyIndex = party.MediusWorldId,
                                        maxPlayers = party.MaxPlayers
                                    });
                                }
                            }
                            else if (gameOrPartyId != 0 && game != null)
                            {
                                if (rClient != null)
                                {
                                    #region P2P
                                    if (game.GameHostType == MGCL_GAME_HOST_TYPE.MGCLGameHostPeerToPeer &&
                                    game.netAddressList?.AddressList[0].AddressType == NetAddressType.NetAddressTypeSignalAddress)
                                    {
                                        rClient.Queue(new MediusJoinGameResponse()
                                        {
                                            SetMaxPlayers = approvedMaxPlayersAppIds.Contains(data.ClientObject.ApplicationId),
                                            MessageID = new MessageId(msgId),
                                            StatusCode = MediusCallbackStatus.MediusSuccess,
                                            GameHostType = game.GameHostType,
                                            ConnectInfo = new NetConnectionInfo()
                                            {
                                                AccessKey = joinGameResponse.AccessKey,
                                                SessionKey = rClient.SessionKey,
                                                TargetWorldID = game.MediusWorldId,
                                                ServerKey = joinGameResponse.pubKey,
                                                AddressList = new NetAddressList()
                                                {
                                                    AddressList = new NetAddress[Constants.NET_ADDRESS_LIST_COUNT]
                                                    {
                                                        new() { Address = game.netAddressList.AddressList[0].Address, Port = game.netAddressList.AddressList[0].Port, AddressType = NetAddressType.NetAddressTypeSignalAddress},
                                                        new() { AddressType = NetAddressType.NetAddressNone},
                                                    }
                                                },
                                                Type = NetConnectionType.NetConnectionTypePeerToPeerUDP
                                            },
                                            MaxPlayers = game.MaxPlayers
                                        });
                                    }
                                    else if (game.GameHostType == MGCL_GAME_HOST_TYPE.MGCLGameHostPeerToPeer &&
                                        game.netAddressList?.AddressList[0].AddressType == NetAddressType.NetAddressTypeExternal &&
                                        game.netAddressList?.AddressList[1].AddressType == NetAddressType.NetAddressTypeInternal)
                                    {
                                        rClient.Queue(new MediusJoinGameResponse()
                                        {
                                            SetMaxPlayers = approvedMaxPlayersAppIds.Contains(data.ClientObject.ApplicationId),
                                            MessageID = new MessageId(msgId),
                                            StatusCode = MediusCallbackStatus.MediusSuccess,
                                            GameHostType = game.GameHostType,
                                            ConnectInfo = new NetConnectionInfo()
                                            {
                                                AccessKey = joinGameResponse.AccessKey,
                                                SessionKey = rClient.SessionKey,
                                                TargetWorldID = game.MediusWorldId,
                                                ServerKey = joinGameResponse.pubKey,
                                                AddressList = new NetAddressList()
                                                {
                                                    AddressList = new NetAddress[Constants.NET_ADDRESS_LIST_COUNT]
                                                    {
                                                        new() { Address = game.netAddressList.AddressList[0].Address, Port = game.netAddressList.AddressList[0].Port, AddressType = NetAddressType.NetAddressTypeExternal},
                                                        new() { Address = game.netAddressList.AddressList[1].Address, Port = game.netAddressList.AddressList[1].Port, AddressType = NetAddressType.NetAddressTypeInternal},
                                                    }
                                                },
                                                Type = NetConnectionType.NetConnectionTypePeerToPeerUDP
                                            },
                                            MaxPlayers = game.MaxPlayers
                                        });
                                    }

                                    else if (game.GameHostType == MGCL_GAME_HOST_TYPE.MGCLGameHostPeerToPeer &&
                                        game.netAddressList?.AddressList[0].AddressType == NetAddressType.NetAddressTypeBinaryExternalVport &&
                                        game.netAddressList?.AddressList[1].AddressType == NetAddressType.NetAddressTypeBinaryInternalVport)
                                    {
                                        rClient.Queue(new MediusJoinGameResponse()
                                        {
                                            SetMaxPlayers = approvedMaxPlayersAppIds.Contains(data.ClientObject.ApplicationId),
                                            MessageID = new MessageId(msgId),
                                            StatusCode = MediusCallbackStatus.MediusSuccess,
                                            GameHostType = game.GameHostType,
                                            ConnectInfo = new NetConnectionInfo()
                                            {
                                                AccessKey = rClient.AccessToken,
                                                SessionKey = rClient.SessionKey,
                                                TargetWorldID = game.MediusWorldId,
                                                ServerKey = game.pubKey,
                                                AddressList = new NetAddressList()
                                                {
                                                    AddressList = new NetAddress[Constants.NET_ADDRESS_LIST_COUNT]
                                                    {
                                                    new NetAddress() {
                                                        AddressType = NetAddressType.NetAddressTypeBinaryExternalVport,
                                                        IPBinaryBitOne = game.netAddressList.AddressList[0].IPBinaryBitOne,
                                                        IPBinaryBitTwo = game.netAddressList.AddressList[0].IPBinaryBitTwo,
                                                        IPBinaryBitThree = game.netAddressList.AddressList[0].IPBinaryBitThree,
                                                        IPBinaryBitFour = game.netAddressList.AddressList[0].IPBinaryBitFour,
                                                        BinaryPort = game.netAddressList.AddressList[0].BinaryPort},
                                                    new NetAddress() {
                                                        AddressType = NetAddressType.NetAddressTypeBinaryInternalVport,
                                                        IPBinaryBitOne = game.netAddressList.AddressList[1].IPBinaryBitOne,
                                                        IPBinaryBitTwo = game.netAddressList.AddressList[1].IPBinaryBitTwo,
                                                        IPBinaryBitThree = game.netAddressList.AddressList[1].IPBinaryBitThree,
                                                        IPBinaryBitFour = game.netAddressList.AddressList[1].IPBinaryBitFour,
                                                        BinaryPort = game.netAddressList.AddressList[1].BinaryPort},
                                                    }
                                                },
                                                Type = NetConnectionType.NetConnectionTypePeerToPeerUDP
                                            },
                                            MaxPlayers = game.MaxPlayers
                                        });
                                    }

                                    #endregion
                                    else if (rClient.MediusVersion > 108 && rClient.ApplicationId != 10994 || rClient.ApplicationId == 10680 || rClient.ApplicationId == 10681 || rClient.ApplicationId == 10683 || rClient.ApplicationId == 10684)
                                    {
                                        // Join game DME
                                        await rClient.JoinGame(game, joinGameResponse.DmeClientIndex);

                                        rClient.Queue(new MediusJoinGameResponse()
                                        {
                                            SetMaxPlayers = approvedMaxPlayersAppIds.Contains(data.ClientObject.ApplicationId),
                                            MessageID = new MessageId(msgId),
                                            StatusCode = MediusCallbackStatus.MediusSuccess,
                                            GameHostType = game.GameHostType,
                                            ConnectInfo = new NetConnectionInfo()
                                            {
                                                AccessKey = joinGameResponse.AccessKey,
                                                SessionKey = rClient.SessionKey,
                                                TargetWorldID = game.MediusWorldId,
                                                ServerKey = joinGameResponse.pubKey,
                                                AddressList = new NetAddressList()
                                                {
                                                    AddressList = new NetAddress[Constants.NET_ADDRESS_LIST_COUNT]
                                                    {
                                                            new NetAddress() { Address = data.ClientObject.IP.MapToIPv4().ToString(), Port = data.ClientObject.Port, AddressType = NetAddressType.NetAddressTypeExternal},
                                                            new NetAddress() { Address = host.AddressList.First().ToString(), Port = MediusClass.Settings.NATPort, AddressType = NetAddressType.NetAddressTypeNATService }
                                                    }
                                                },
                                                Type = NetConnectionType.NetConnectionTypeClientServerTCPAuxUDP
                                            },
                                            MaxPlayers = game.MaxPlayers
                                        });
                                    }
                                    else
                                    {
                                        // Join game DME
                                        await rClient.JoinGame(game, joinGameResponse.DmeClientIndex);

                                        rClient.Queue(new MediusJoinGameResponse()
                                        {
                                            SetMaxPlayers = approvedMaxPlayersAppIds.Contains(data.ClientObject.ApplicationId),
                                            MessageID = new MessageId(msgId),
                                            StatusCode = MediusCallbackStatus.MediusSuccess,
                                            GameHostType = game.GameHostType,
                                            ConnectInfo = new NetConnectionInfo()
                                            {
                                                AccessKey = joinGameResponse.AccessKey,
                                                SessionKey = rClient.SessionKey,
                                                TargetWorldID = game.MediusWorldId,
                                                ServerKey = joinGameResponse.pubKey,
                                                AddressList = new NetAddressList()
                                                {
                                                    AddressList = new NetAddress[Constants.NET_ADDRESS_LIST_COUNT]
                                                    {
                                                    new NetAddress() { Address = data.ClientObject.IP.MapToIPv4().ToString(), Port = data.ClientObject.Port, AddressType = NetAddressType.NetAddressTypeExternal},
                                                    new NetAddress() { Address = host.AddressList.First().ToString(), Port = MediusClass.Settings.NATPort, AddressType = NetAddressType.NetAddressTypeNATService }
                                                    }
                                                },
                                                Type = NetConnectionType.NetConnectionTypeClientServerTCP
                                            }
                                        });
                                    }
                                }

                                // For Legacy Medius v1.50 clients that DO NOT 
                                // send a ServerConnectNotificationConnect when creating a game
                                if (data.ClientObject.MediusVersion < 109 && data.ClientObject.ApplicationId != 10394)
                                    await game.OnMediusJoinGameResponse(rClient?.SessionKey);
                            }
                        }
                        else
                            LoggerAccessor.LogError("[MPS] - joinGameResponse was requested but client is null, aborting!");
                        break;
                    }
                #endregion

                #region MediusServerCreateGameOnSelfRequest
                case MediusServerCreateGameOnSelfRequest serverCreateGameOnSelfRequest:
                    {
                        if (data.ClientObject == null)
                        {
LoggerAccessor.LogError($"[MPS] - INVALID OPERATION: {clientChannel} is trying to MediusServerCreateGameOnSelfRequest without a Client Object");
                            break;
                        }

                        if (serverCreateGameOnSelfRequest.AddressList.AddressList[0].AddressType == NetAddressType.NetAddressTypeBinaryExternal)
                            data.ClientObject?.SetIp(InternetProtocolUtils.GetIPAddressFromUInt(serverCreateGameOnSelfRequest.AddressList.AddressList[0].BinaryAddress).ToString());
                        else if (serverCreateGameOnSelfRequest.AddressList.AddressList[0].AddressType == NetAddressType.NetAddressTypeBinaryExternalVport
                            || serverCreateGameOnSelfRequest.AddressList.AddressList[0].AddressType == NetAddressType.NetAddressTypeBinaryInternalVport)
                        {
                            data.ClientObject?.SetIp(serverCreateGameOnSelfRequest.AddressList.AddressList[0].IPBinaryBitOne + "." +
                                    serverCreateGameOnSelfRequest.AddressList.AddressList[0].IPBinaryBitTwo + "." +
                                    serverCreateGameOnSelfRequest.AddressList.AddressList[0].IPBinaryBitThree + "." +
                                    serverCreateGameOnSelfRequest.AddressList.AddressList[0].IPBinaryBitFour);
                        }
                        else if (serverCreateGameOnSelfRequest.AddressList.AddressList[0].AddressType == NetAddressType.NetAddressNone
                            || serverCreateGameOnSelfRequest.AddressList.AddressList[0].AddressType == NetAddressType.NetAddressTypeSignalAddress /* TODO: Signal Address not supported yet! */)
                            data.ClientObject?.SetIp(((IPEndPoint)clientChannel.RemoteAddress).Address.ToString().Trim(new char[] { ':', 'f', '{', '}' }));
                        else
                            data.ClientObject?.SetIp(serverCreateGameOnSelfRequest.AddressList.AddressList[0].Address ?? "0.0.0.0");

                        // validate name
                        if (!MediusClass.PassTextFilter(data.ApplicationId, TextFilterContext.GAME_NAME, Convert.ToString(serverCreateGameOnSelfRequest.GameName)))
                        {
                            data.ClientObject?.Queue(new MediusServerCreateGameOnMeResponse()
                            {
                                MessageID = serverCreateGameOnSelfRequest.MessageID,
                                Confirmation = MGCL_ERROR_CODE.MGCL_INVALID_ARG
                            });
                            return;
                        }

                        // Send to plugins
                        await MediusClass.Plugins.OnEvent(PluginEvent.MEDIUS_PLAYER_ON_CREATE_GAME, new OnPlayerRequestArgs() { Player = data.ClientObject, Request = serverCreateGameOnSelfRequest });

                        data.MeClientObject = await MediusClass.Manager.CreateGameP2P(data.ClientObject!, serverCreateGameOnSelfRequest, clientChannel);
                        break;
                    }
                #endregion

                #region MediusServerCreateGameOnSelfRequest0
                case MediusServerCreateGameOnSelfRequest0 serverCreateGameOnSelfRequest0:
                    {
                        if (data.ClientObject == null)
                        {
LoggerAccessor.LogError($"[MPS] - INVALID OPERATION: {clientChannel} is trying to MediusServerCreateGameOnSelfRequest0 without a Client Object");
                            break;
                        }

                        if (serverCreateGameOnSelfRequest0.AddressList.AddressList[0].AddressType == NetAddressType.NetAddressTypeBinaryExternal)
                            data.ClientObject?.SetIp(InternetProtocolUtils.GetIPAddressFromUInt(serverCreateGameOnSelfRequest0.AddressList.AddressList[0].BinaryAddress).ToString());
                        else if (serverCreateGameOnSelfRequest0.AddressList.AddressList[0].AddressType == NetAddressType.NetAddressTypeBinaryExternalVport
                            || serverCreateGameOnSelfRequest0.AddressList.AddressList[0].AddressType == NetAddressType.NetAddressTypeBinaryInternalVport)
                        {
                            data.ClientObject?.SetIp(serverCreateGameOnSelfRequest0.AddressList.AddressList[0].IPBinaryBitOne + "." +
                                    serverCreateGameOnSelfRequest0.AddressList.AddressList[0].IPBinaryBitTwo + "." +
                                    serverCreateGameOnSelfRequest0.AddressList.AddressList[0].IPBinaryBitThree + "." +
                                    serverCreateGameOnSelfRequest0.AddressList.AddressList[0].IPBinaryBitFour);
                        }
                        else if (serverCreateGameOnSelfRequest0.AddressList.AddressList[0].AddressType == NetAddressType.NetAddressNone
                            || serverCreateGameOnSelfRequest0.AddressList.AddressList[0].AddressType == NetAddressType.NetAddressTypeSignalAddress /* TODO: Signal Address not supported yet! */)
                            data.ClientObject?.SetIp(((IPEndPoint)clientChannel.RemoteAddress).Address.ToString().Trim(new char[] { ':', 'f', '{', '}' }));
                        else
                            data.ClientObject?.SetIp(serverCreateGameOnSelfRequest0.AddressList.AddressList[0].Address ?? "0.0.0.0");

                        // validate name
                        if (!MediusClass.PassTextFilter(data.ApplicationId, TextFilterContext.GAME_NAME, Convert.ToString(serverCreateGameOnSelfRequest0.GameName)))
                        {
                            data.ClientObject?.Queue(new MediusServerCreateGameOnMeResponse()
                            {
                                MessageID = serverCreateGameOnSelfRequest0.MessageID,
                                Confirmation = MGCL_ERROR_CODE.MGCL_INVALID_ARG
                            });
                            return;
                        }

                        // Send to plugins
                        await MediusClass.Plugins.OnEvent(PluginEvent.MEDIUS_PLAYER_ON_CREATE_GAME, new OnPlayerRequestArgs() { Player = data.ClientObject, Request = serverCreateGameOnSelfRequest0 });

                        data.MeClientObject = await MediusClass.Manager.CreateGameP2P(data.ClientObject!, serverCreateGameOnSelfRequest0, clientChannel);
                        break;
                    }
                #endregion

                #region MediusServerCreateGameOnMeRequest   
                case MediusServerCreateGameOnMeRequest serverCreateGameOnMeRequest:
                    {
                        if (data.ClientObject == null)
                        {
LoggerAccessor.LogError($"[MPS] - INVALID OPERATION: {clientChannel} is trying to MediusServerCreateGameOnMeRequest without a Client Object");
                            break;
                        }

                        if (serverCreateGameOnMeRequest.AddressList.AddressList[0].AddressType == NetAddressType.NetAddressTypeBinaryExternal)
                            data.ClientObject?.SetIp(InternetProtocolUtils.GetIPAddressFromUInt(serverCreateGameOnMeRequest.AddressList.AddressList[0].BinaryAddress).ToString());
                        else if (serverCreateGameOnMeRequest.AddressList.AddressList[0].AddressType == NetAddressType.NetAddressTypeBinaryExternalVport
                            || serverCreateGameOnMeRequest.AddressList.AddressList[0].AddressType == NetAddressType.NetAddressTypeBinaryInternalVport)
                        {
                            data.ClientObject?.SetIp(serverCreateGameOnMeRequest.AddressList.AddressList[0].IPBinaryBitOne + "." +
                                    serverCreateGameOnMeRequest.AddressList.AddressList[0].IPBinaryBitTwo + "." +
                                    serverCreateGameOnMeRequest.AddressList.AddressList[0].IPBinaryBitThree + "." +
                                    serverCreateGameOnMeRequest.AddressList.AddressList[0].IPBinaryBitFour);
                        }
                        else if (serverCreateGameOnMeRequest.AddressList.AddressList[0].AddressType == NetAddressType.NetAddressNone
                            || serverCreateGameOnMeRequest.AddressList.AddressList[0].AddressType == NetAddressType.NetAddressTypeSignalAddress /* TODO: Signal Address not supported yet! */)
                            data.ClientObject?.SetIp(((IPEndPoint)clientChannel.RemoteAddress).Address.ToString().Trim(new char[] { ':', 'f', '{', '}' }));
                        else
                            data.ClientObject?.SetIp(serverCreateGameOnMeRequest.AddressList.AddressList[0].Address ?? "0.0.0.0");

                        // validate name
                        if (!MediusClass.PassTextFilter(data.ApplicationId, TextFilterContext.GAME_NAME, Convert.ToString(serverCreateGameOnMeRequest.GameName)))
                        {
                            data.ClientObject?.Queue(new MediusServerCreateGameOnMeResponse()
                            {
                                MessageID = serverCreateGameOnMeRequest.MessageID,
                                Confirmation = MGCL_ERROR_CODE.MGCL_INVALID_ARG,
                            });
                            return;
                        }

                        // Send to plugins
                        await MediusClass.Plugins.OnEvent(PluginEvent.MEDIUS_PLAYER_ON_CREATE_GAME, new OnPlayerRequestArgs() { Player = data.ClientObject, Request = serverCreateGameOnMeRequest });

                        data.MeClientObject = await MediusClass.Manager.CreateGameP2P(data.ClientObject!, serverCreateGameOnMeRequest, clientChannel);
                        break;
                    }

                #endregion

                #region MediusServerMoveGameWorldOnMeRequest
                case MediusServerMoveGameWorldOnMeRequest serverMoveGameWorldOnMeRequest:
                    {
                        //Fetch Current Game, and Update it with the new one
                        Game? game = MediusClass.Manager.GetGameByGameId(serverMoveGameWorldOnMeRequest.CurrentMediusWorldID);
                        Party? party = MediusClass.Manager.GetPartyByPartyId(serverMoveGameWorldOnMeRequest.CurrentMediusWorldID);

                        if (game != null)
                        {
                            int result = game.ReassignGameMediusWorldID(serverMoveGameWorldOnMeRequest);

                            if (result == 0)
                            {
                                data.MeClientObject?.Queue(new MediusServerMoveGameWorldOnMeResponse()
                                {
                                    MessageID = serverMoveGameWorldOnMeRequest.MessageID,
                                    Confirmation = MGCL_ERROR_CODE.MGCL_UNSUCCESSFUL,
                                });
                            }
                            else
                            {
                                game.netAddressList.AddressList[0] = serverMoveGameWorldOnMeRequest.AddressList.AddressList[0];
                                game.netAddressList.AddressList[1] = serverMoveGameWorldOnMeRequest.AddressList.AddressList[1];

                                data.MeClientObject?.Queue(new MediusServerMoveGameWorldOnMeResponse()
                                {
                                    MessageID = serverMoveGameWorldOnMeRequest.MessageID,
                                    Confirmation = MGCL_ERROR_CODE.MGCL_SUCCESS,
                                    MediusWorldID = serverMoveGameWorldOnMeRequest.NewGameMediusWorldID
                                });
                            }
                        }
                        else if (party != null)
                        {
                            int result = party.ReassignGameMediusWorldID(serverMoveGameWorldOnMeRequest);

                            if (result == 0)
                            {
                                data.MeClientObject?.Queue(new MediusServerMoveGameWorldOnMeResponse()
                                {
                                    MessageID = serverMoveGameWorldOnMeRequest.MessageID,
                                    Confirmation = MGCL_ERROR_CODE.MGCL_UNSUCCESSFUL,
                                });
                            }
                            else
                            {
                                party.netAddressList.AddressList[0] = serverMoveGameWorldOnMeRequest.AddressList.AddressList[0];
                                party.netAddressList.AddressList[1] = serverMoveGameWorldOnMeRequest.AddressList.AddressList[1];

                                data.MeClientObject?.Queue(new MediusServerMoveGameWorldOnMeResponse()
                                {
                                    MessageID = serverMoveGameWorldOnMeRequest.MessageID,
                                    Confirmation = MGCL_ERROR_CODE.MGCL_SUCCESS,
                                    MediusWorldID = serverMoveGameWorldOnMeRequest.NewGameMediusWorldID
                                });
                            }
                        }

                        break;
                    }
                #endregion

                #region MediusServerWorldReportOnMe

                case MediusServerWorldReportOnMe serverWorldReportOnMe:
                    {
                        if (data.MeClientObject?.CurrentGame != null)
                            await data.MeClientObject.CurrentGame.OnWorldReportOnMe(serverWorldReportOnMe);
                        else if (data.MeClientObject?.CurrentParty != null)
                            await data.MeClientObject.CurrentParty.OnWorldReportOnMe(serverWorldReportOnMe);
                        break;
                    }

                #endregion

                #region MediusServerEndGameOnMeRequest
                /// <summary>
                /// This structure uses the game world ID as MediusWorldID. This should not be confused with the net World ID on this host.
                /// </summary>
                case MediusServerEndGameOnMeRequest serverEndGameOnMeRequest:
                    {
                        if (data.MeClientObject == null) // Happens when the game creation fails.
                        {
                            LoggerAccessor.LogWarn("[MPS] - Client Object: {data.ClientObject} called MediusServerEndGameOnMeRequest without a Me Client Object, ignoring.");
                            break;
                        }

                        Game? game = MediusClass.Manager.GetGameByGameId(serverEndGameOnMeRequest.MediusWorldID);
                        Party? party = MediusClass.Manager.GetPartyByPartyId(serverEndGameOnMeRequest.MediusWorldID);

                        if (game != null)
                        {
                            await game.EndGame(data.MeClientObject.ApplicationId);

                            data.MeClientObject.Queue(new MediusServerEndGameOnMeResponse()
                            {
                                MessageID = serverEndGameOnMeRequest.MessageID,
                                Confirmation = MGCL_ERROR_CODE.MGCL_SUCCESS,
                            });
                        }
                        else if (party != null)
                        {
                            await party.EndParty(data.MeClientObject.ApplicationId);

                            data.MeClientObject.Queue(new MediusServerEndGameOnMeResponse()
                            {
                                MessageID = serverEndGameOnMeRequest.MessageID,
                                Confirmation = MGCL_ERROR_CODE.MGCL_SUCCESS,
                            });
                        }
                        else
                        {
                            data.MeClientObject.Queue(new MediusServerEndGameOnMeResponse()
                            {
                                MessageID = serverEndGameOnMeRequest.MessageID,
                                Confirmation = MGCL_ERROR_CODE.MGCL_INVALID_ARG,
                            });
                        }

                        break;
                    }
                #endregion

                #region MediusServerReport
                case MediusServerReport serverReport:
                    {
                        data.ClientObject?.OnServerReport(serverReport);
                        break;
                    }
                #endregion

                #region MediusServerConnectNotification
                case MediusServerConnectNotification connectNotification:
                    {
                        if (data.ClientObject != null)
                        {
                            Game? gameConn = MediusClass.Manager.GetGameByMediusWorldId(data.ClientObject.SessionKey ?? string.Empty, connectNotification.MediusWorldUID);
                            Party? partyConn = MediusClass.Manager.GetPartyByMediusWorldId(data.ClientObject.SessionKey ?? string.Empty, connectNotification.MediusWorldUID);

                            if (gameConn != null)
                                await gameConn.OnMediusServerConnectNotification(connectNotification);
                            else if (partyConn != null)
                                await partyConn.OnMediusServerConnectNotification(connectNotification);
                        }

                        //MediusServerConnectNotification -  sent Notify msg to MUM
                        //DmeServerGetConnectKeys

                        break;
                    }
                #endregion

                #region MediusServerDisconnectPlayerRequest UNIMPLEMENTED
                case MediusServerDisconnectPlayerRequest serverDisconnectPlayerRequest:
                    {

                        break;
                    }
                #endregion

                #region MediusServerEndGameRequest
                case MediusServerEndGameRequest endGameRequest:
                    {
                        if (data.ClientObject == null)
                        {
LoggerAccessor.LogError($"[MPS] - INVALID OPERATION: {clientChannel} is trying to MediusServerEndGameRequest without a Client Object");
                            break;
                        }

                        Game? game = MediusClass.Manager.GetGameByGameId(endGameRequest.MediusWorldID);
                        Party? party = MediusClass.Manager.GetPartyByPartyId(endGameRequest.MediusWorldID);

                        if (game != null)
                        {
                            if (endGameRequest.BrutalFlag)
                                await game.EndGame(data.ClientObject.ApplicationId);
                            else
                                await data.ClientObject.LeaveGame(game);

                            data.ClientObject.Queue(new MediusServerEndGameResponse()
                            {
                                MessageID = endGameRequest.MessageID,
                                Confirmation = MGCL_ERROR_CODE.MGCL_SUCCESS
                            });
                        }
                        else if (party != null)
                        {
                            if (endGameRequest.BrutalFlag)
                                await party.EndParty(data.ClientObject.ApplicationId);
                            else
                                await data.ClientObject.LeaveParty(party);

                            data.ClientObject.Queue(new MediusServerEndGameResponse()
                            {
                                MessageID = endGameRequest.MessageID,
                                Confirmation = MGCL_ERROR_CODE.MGCL_SUCCESS
                            });
                        }
                        else
                        {
                            data.ClientObject.Queue(new MediusServerEndGameResponse()
                            {
                                MessageID = endGameRequest.MessageID,
                                Confirmation = MGCL_ERROR_CODE.MGCL_INVALID_ARG
                            });
                        }
                        break;
                    }
                #endregion

                #region MediusServerSessionEndRequest
                case MediusServerSessionEndRequest sessionEndRequest:
                    {
                        if (data.ClientObject == null)
                        {
LoggerAccessor.LogError($"[MPS] - INVALID OPERATION: {clientChannel} is trying to end server session without a Client Object");
                            break;
                        }

                        data.ClientObject.EndServerSession();

                        data?.ClientObject.Queue(new MediusServerSessionEndResponse()
                        {
                            MessageID = sessionEndRequest.MessageID,
                            ErrorCode = MGCL_ERROR_CODE.MGCL_SUCCESS
                        });

                        break;
                    }
                #endregion

                default:
                    {
LoggerAccessor.LogWarn($"[MPS] - Unhandled Medius Message: {message}");
                        break;
                    }
            }
        }

        public ClientObject? GetFreeDme(int appId, int preferredLocation)
        {
            try
            {
                /* Why this exists? Some games register their own DME servers, not a problem.
                 * But in some rare cases (Ratchet UYA Beta Trial/Press) the server is SO OLD that is requires it's own packet format.
                   This option allows us to return only our "modern" server, to save on reverse-engineering efforts ;). */
                if (appId == 10680 || appId == 10681)
                    return _scertHandler?.Group?
                        .Select(x => _channelDatas[x.Id.AsLongText()]?.ClientObject)
                        .Where(x => x is not null && x.LocationId == preferredLocation && (x.ApplicationId == appId || x.ApplicationId == 0) && x.IsActiveServer && x.IP.Equals(DmeClass.SERVER_IP))
                        .OrderBy(x => x!.TimeCreated) // If on same computer as the client.
                        .FirstOrDefault();
                else
                    return _scertHandler?.Group?
                        .Select(x => _channelDatas[x.Id.AsLongText()]?.ClientObject)
                        .Where(x => x is not null && x.LocationId == preferredLocation && (x.ApplicationId == appId || x.ApplicationId == 0) && x.IsActiveServer)
                        .MinBy(x => x!.CurrentWorlds);
            }
            catch
            {
            }

            try
            {
                /* Why this exists? Some games register their own DME servers, not a problem.
                 * But in some rare cases (Ratchet UYA Beta Trial/Press) the server is SO OLD that is requires it's own packet format.
                   This option allows us to return only our "modern" server, to save on reverse-engineering efforts ;). */
                if (appId == 10680 || appId == 10681)
                    return _scertHandler?.Group?
                        .Select(x => _channelDatas[x.Id.AsLongText()]?.ClientObject)
                        .Where(x => x is not null && (x.ApplicationId == appId || x.ApplicationId == 0) && x.IsActiveServer && x.IP.Equals(DmeClass.SERVER_IP))
                        .OrderBy(x => x!.TimeCreated) // If on same computer as the client.
                        .FirstOrDefault();
                else
                    return _scertHandler?.Group?
                        .Select(x => _channelDatas[x.Id.AsLongText()]?.ClientObject)
                        .Where(x => x is not null && (x.ApplicationId == appId || x.ApplicationId == 0) && x.IsActiveServer)
                        .MinBy(x => x!.CurrentWorlds);
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError($"[MPS] - No DME Game Server assigned to this AppId. (Exception:{e})");
            }

            return null;
        }

        public void SendServerCreateGameWithAttributesRequestP2P(string msgId, int acctId, int gameId, bool partyType, Game game, ClientObject client)
        {
            //{gameId}-{acctId}-{messageId}-{partyType}
            Queue(new RT_MSG_SERVER_APP()
            {
                Message = new MediusServerCreateGameWithAttributesRequest2()
                {
                    MessageID = new MessageId($"{msgId}"),
                    MediusWorldUID = gameId,
                    Attributes = game.Attributes,
                    ApplicationID = client.ApplicationId,
                    MaxClients = game.MaxPlayers,
                    ConnectInfo = new NetConnectionInfo()
                    {
                        AccessKey = client.AccessToken,
                        SessionKey = client.SessionKey,
                        TargetWorldID = game.MediusWorldId,
                        ServerKey = new RSA_KEY(),
                        AddressList = new NetAddressList()
                        {
                            AddressList = new NetAddress[Constants.NET_ADDRESS_LIST_COUNT]
                              {
                                  new() { BinaryAddress = BitConverter.ToUInt32(!BitConverter.IsLittleEndian ? EndianTools.EndianUtils.ReverseArray(client.IP.GetAddressBytes()) : client.IP.GetAddressBytes()), BinaryPort = 0, AddressType = NetAddressType.NetAddressTypeBinaryExternal }, //Address = game.netAddressList.AddressList[0].Address, Port = game.netAddressList.AddressList[0].Port, AddressType = NetAddressType.NetAddressTypeBinaryExternal},
                                  new() { AddressType = NetAddressType.NetAddressNone},
                              }
                        },
                        Type = NetConnectionType.NetConnectionTypePeerToPeerUDP
                    }
                }
            }, channel);
        }

        public void SendServerCreateGameWithAttributesRequest(string msgId, int acctId, int worldId, int gameId, bool partyType, int gameAttributes, int clientAppId, int gameMaxPlayers)
        {
            //{gameId}-{acctId}-{messageId}-{partyType}
            Queue(new RT_MSG_SERVER_APP()
            {
                Message = new MediusServerCreateGameWithAttributesRequest()
                {
                    MessageID = new MessageId($"{gameId}-{acctId}-{msgId}-{(partyType ? 1 : 0)}"),
                    WorldID = worldId,
                    Attributes = (MediusWorldAttributesType)gameAttributes,
                    ApplicationID = clientAppId,
                    MaxClients = gameMaxPlayers
                }
            }, channel);
        }
    }
}
