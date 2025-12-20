using CustomLogger;
using DotNetty.Transport.Channels;
using Horizon.RT.Common;
using Horizon.RT.Cryptography;
using Horizon.RT.Cryptography.RSA;
using Horizon.RT.Models;
using Horizon.HTTPSERVICE;
using Horizon.SERVER.Config;
using Horizon.SERVER.PluginArgs;
using Horizon.SERVER.Extension.PlayStationHome;
using Horizon.PluginManager;
using Horizon.LIBRARY.Database.Models;
using Newtonsoft.Json.Linq;
using MultiServerLibrary.Extension;
using EndianTools;
using Horizon.MUM.Models;
using System.Text.RegularExpressions;
using System.Net;
using System.Globalization;
using System.Text;
using System.Buffers;
using MultiServerLibrary;
using CastleLibrary.Sony.XI5;

namespace Horizon.SERVER.Medius
{
    public class MAS : BaseMediusComponent
    {
        int _tcpPort = 0;

        public override int TCPPort
        {
            get => _tcpPort;
            set
            {
                if (value < 0 || value > 65535)
                    throw new ArgumentOutOfRangeException(nameof(value), "[MAS] - Port must be between 0 and 65535.");
                _tcpPort = value;
            }
        }

        public override int UDPPort
        {
            get => 0;
            set
            {
                throw new ArgumentOutOfRangeException(nameof(value), "[MAS] - UDP Port can't be assigned.");
            }
        }

        public static ServerSettings Settings = new();

        public MAS()
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
            var enableEncryption = MediusClass.GetAppSettingsOrDefault(data.ApplicationId).EnableEncryption;
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
                            LoggerAccessor.LogError($"[MAS] - Client Connected {clientChannel.RemoteAddress} with an invalid connect payload!");
                            break;
                        }
                        else if (!MediusClass.Manager.IsAppIdSupported(appid))
                        {
LoggerAccessor.LogError($"[MAS] - Client {clientChannel.RemoteAddress} attempting to authenticate with incompatible app id {appid}");
                            await clientChannel.CloseAsync();
                            return;
                        }
                        #endregion

                        if (clientConnectTcp.Key == RSA_KEY.Empty)
                        {
                            LoggerAccessor.LogError($"[MAS] - Client Connected {clientChannel.RemoteAddress} with an empty key!");
                            break;
                        }

                        List<int> pre108ServerComplete = new() { 10114, 10130, 10164, 10190, 10124, 10284, 10330, 10334, 10414, 10421, 10442, 10538, 10540, 10550, 10582, 10584, 10680, 10681, 10683, 10684, 10724 };

                        ///<summary>
                        /// Some do not post-108 so we have to account for those too!
                        /// tmheadon 10694 does not for initial
                        /// </summary>
                        List<int> post108ServerComplete = new() { 10694 };

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
                                LoggerAccessor.LogError($"[MAS] - Client: {clientConnectTcp.AccessToken} tried to join, but targetted WorldId:{clientConnectTcp.TargetWorldId} doesn't exist!");
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

                            if (data.ClientObject != null)
                                LoggerAccessor.LogInfo($"[MAS] - Client Connected {clientChannel.RemoteAddress}!");
                            else
                            {
                                data.Ignore = true;
                                LoggerAccessor.LogError($"[MAS] - ClientObject could not be granted for {clientChannel.RemoteAddress}: {clientConnectTcp}");
                                break;
                            }

                            data.ClientObject.MediusVersion = scertClient.MediusVersion ?? 0;
                            data.ClientObject.ApplicationId = appid;
                            data.ClientObject.OnConnected();
                        }
                        else
                        {
                            LoggerAccessor.LogInfo($"[MAS] - Client Connected {clientChannel.RemoteAddress} with new ClientObject!");

                            data.ClientObject = new(scertClient.MediusVersion ?? 0)
                            {
                                ApplicationId = appid
                            };
                            data.ClientObject.OnConnected();

                            ReserveClient(data.ClientObject); // ONLY RESERVE CLIENTS HERE!
                        }

                        await data.ClientObject.JoinChannel(targetChannel);

                        #region if PS3
                        if (scertClient.IsPS3Client)
                        {
                            List<int> ConnectAcceptTCPGames = new() { 20623, 20624, 21564, 21574, 21584, 21594, 22274, 22284, 22294, 22304, 20040, 20041, 20042, 20043, 20044 };

                            //CAC & Warhawk
                            if (ConnectAcceptTCPGames.Contains(scertClient.ApplicationID))
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

                            if (pre108ServerComplete.Contains(data.ApplicationId) || post108ServerComplete.Contains(data.ApplicationId))
                                Queue(new RT_MSG_SERVER_CONNECT_COMPLETE() { ClientCountAtConnect = 0x0001 }, clientChannel);
                        }

                        if (MediusClass.Settings.HttpsSVOCheckPatcher)
                        {
                            switch (data.ApplicationId)
                            {
                                case 20371:
                                    CheatQuery(0x10085d80, 6, clientChannel); // PS Home 1.50 Beta
                                    break;
                                case 20384:
                                    CheatQuery(0x008625b0, 6, clientChannel); // SingStar Vol3 Retail
                                    CheatQuery(0x00b96850, 6, clientChannel); // SingStar Starter Pack
                                    break;
                                case 21354:
                                    CheatQuery(0x008625E0, 6, clientChannel); // SingStar Hits v1.00
                                    break;
                                case 21574:
                                    CheatQuery(0x0070c068, 6, clientChannel); // Warhawk EU v1.50
                                    break;
                                case 21564:
                                    CheatQuery(0x0070BFF8, 6, clientChannel); // Warhawk US v1.50
                                    break;
                                case 22924:
                                    CheatQuery(0x00df0008, 6, clientChannel); // Starhawk v1.4 Retail
                                    break;
                                case 23014:
                                    CheatQuery(0x010e6b30, 6, clientChannel); // SingStar Viewer
                                    break;
                            }
                        }

                        if (data.ApplicationId == 20371 || data.ApplicationId == 20374)
                            CheatQuery(0x00010000, 512000, clientChannel, CheatQueryType.DME_SERVER_CHEAT_QUERY_SHA1_HASH, unchecked((int)0xDEADBEEF));

                        PokePatch(clientChannel, data);

                        break;
                    }

                case RT_MSG_SERVER_CHEAT_QUERY clientCheatQuery:
                    {
                        byte[]? QueryData = clientCheatQuery.Data;

                        if (QueryData != null)
                        {
                            LoggerAccessor.LogDebug($"[MAS] - QUERY CHECK - Client:{data.ClientObject?.IP} Has Data:{QueryData.ToHexString()} in offset: {clientCheatQuery.StartAddress}");

                            if (MediusClass.Settings.HttpsSVOCheckPatcher && clientCheatQuery.QueryType == CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY && QueryData.Length == 6
                                && QueryData.EqualsTo(new byte[] { 0x68, 0x74, 0x74, 0x70, 0x73, 0x3A }))
                                PatchHttpsSVOCheck(clientCheatQuery.StartAddress + 4, clientChannel);

                            if (data.ApplicationId == 20371 || data.ApplicationId == 20374)
                            {
                                if (data.ClientObject?.ClientHomeData != null)
                                {
                                    switch (data.ClientObject.ClientHomeData.Type)
                                    {
                                        case "HDK With Offline":
                                            switch (data.ClientObject.ClientHomeData.Version)
                                            {
                                                case "01.86.09":
                                                    switch (clientCheatQuery.StartAddress)
                                                    {
                                                        case 0x005478dc:
                                                            // 4096 character command line limit.
                                                            if (MediusClass.Settings.PokePatchOn && clientCheatQuery.QueryType == CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY && QueryData.Length == 4 && QueryData.EqualsTo(new byte[] { 0x2f, 0x83, 0x00, 0xfe }))
                                                            {
                                                                PokeAddress(0x005478dc, new byte[] { 0x2f, 0x83, 0x0f, 0xff }, clientChannel);
                                                                PokeAddress(0x00548378, new byte[] { 0x2b, 0x83, 0x0f, 0xff }, clientChannel);
                                                            }
                                                            break;
                                                        case 0x1054e5c0:
                                                            // Sets WorldCorePointer.
                                                            if (clientCheatQuery.QueryType == CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY && QueryData.Length == 4)
                                                                data.ClientObject.SetWorldCorePointer(BitConverter.ToUInt32(BitConverter.IsLittleEndian ? EndianUtils.ReverseArray(QueryData) : QueryData));
                                                            break;
                                                        case 0x0016cc6c:
                                                            // Patches out the forceInvite command.
                                                            if (MediusClass.Settings.PlaystationHomeForceInviteExploitPatch && MediusClass.Settings.PokePatchOn && clientCheatQuery.QueryType == CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY && QueryData.Length == 4 && QueryData.EqualsTo(new byte[] { 0x2f, 0x80, 0x00, 0x00 }))
                                                                PokeAddress(0x0016cc6c, new byte[] { 0x2f, 0x80, 0x00, 0x02 }, clientChannel);
                                                            break;
                                                    }
                                                    break;
                                                default:
                                                    break;
                                            }
                                            break;
                                        case "HDK Online Only":
                                            switch (data.ClientObject.ClientHomeData.Version)
                                            {
                                                default:
                                                    break;
                                            }
                                            break;
                                        case "HDK Online Only (Dbg Symbols)":
                                            switch (data.ClientObject.ClientHomeData.Version)
                                            {
                                                case "01.82.09":
                                                    switch (clientCheatQuery.StartAddress)
                                                    {
                                                        case 0x00531370:
                                                            // 4096 character command line limit.
                                                            if (MediusClass.Settings.PokePatchOn && clientCheatQuery.QueryType == CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY && QueryData.Length == 4 && QueryData.EqualsTo(new byte[] { 0x2f, 0x83, 0x00, 0xfe }))
                                                            {
                                                                PokeAddress(0x00531370, new byte[] { 0x2f, 0x83, 0x0f, 0xff }, clientChannel);
                                                                PokeAddress(0x00531e08, new byte[] { 0x2b, 0x83, 0x0f, 0xff }, clientChannel);
                                                            }
                                                            break;
                                                        case 0x1053e160:
                                                            // Sets WorldCorePointer.
                                                            if (clientCheatQuery.QueryType == CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY && QueryData.Length == 4)
                                                                data.ClientObject.SetWorldCorePointer(BitConverter.ToUInt32(BitConverter.IsLittleEndian ? EndianUtils.ReverseArray(QueryData) : QueryData));
                                                            break;
                                                        case 0x0001191B:
                                                            // Sets ProtocolVersion.
                                                            if (clientCheatQuery.QueryType == CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY && QueryData.Length == 1)
                                                                data.ClientObject.ProtocolVersion = QueryData[0];
                                                            break;
                                                        case 0x0016b4d0:
                                                            // Patches out the forceInvite command.
                                                            if (MediusClass.Settings.PlaystationHomeForceInviteExploitPatch && MediusClass.Settings.PokePatchOn && clientCheatQuery.QueryType == CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY && QueryData.Length == 4 && QueryData.EqualsTo(new byte[] { 0x2f, 0x80, 0x00, 0x00 }))
                                                                PokeAddress(0x0016b4d0, new byte[] { 0x2f, 0x80, 0x00, 0x02 }, clientChannel);
                                                            break;
                                                    }
                                                    break;
                                                default:
                                                    break;
                                            }
                                            break;
                                        case "Online Debug":
                                        case "Online Debug (QA)":
                                            switch (data.ClientObject.ClientHomeData.Version)
                                            {
                                                case "01.83.12":
                                                    switch (clientCheatQuery.StartAddress)
                                                    {
                                                        case 0x00548bc0:
                                                            // 4096 character command line limit.
                                                            if (MediusClass.Settings.PokePatchOn && clientCheatQuery.QueryType == CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY && QueryData.Length == 4 && QueryData.EqualsTo(new byte[] { 0x2f, 0x83, 0x00, 0xfe }))
                                                            {
                                                                PokeAddress(0x00548bc0, new byte[] { 0x2f, 0x83, 0x0f, 0xff }, clientChannel);
                                                                PokeAddress(0x0054964c, new byte[] { 0x2b, 0x83, 0x0f, 0xff }, clientChannel);
                                                            }
                                                            break;
                                                        case 0x1054e1c0:
                                                            // Sets WorldCorePointer.
                                                            if (clientCheatQuery.QueryType == CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY && QueryData.Length == 4)
                                                                data.ClientObject.SetWorldCorePointer(BitConverter.ToUInt32(BitConverter.IsLittleEndian ? EndianUtils.ReverseArray(QueryData) : QueryData));
                                                            break;
                                                        case 0x0001193F:
                                                            // Sets ProtocolVersion.
                                                            if (clientCheatQuery.QueryType == CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY && QueryData.Length == 1)
                                                                data.ClientObject.ProtocolVersion = QueryData[0];
                                                            break;
                                                        case 0x001709e0:
                                                            // Patches out the forceInvite command.
                                                            if (MediusClass.Settings.PlaystationHomeForceInviteExploitPatch && MediusClass.Settings.PokePatchOn && clientCheatQuery.QueryType == CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY && QueryData.Length == 4 && QueryData.EqualsTo(new byte[] { 0x2f, 0x80, 0x00, 0x00 }))
                                                                PokeAddress(0x001709e0, new byte[] { 0x2f, 0x80, 0x00, 0x02 }, clientChannel);
                                                            break;
                                                    }
                                                    break;
                                                case "01.86.09":
                                                    switch (clientCheatQuery.StartAddress)
                                                    {
                                                        case 0x00555cb4:
                                                            // 4096 character command line limit.
                                                            if (MediusClass.Settings.PokePatchOn && clientCheatQuery.QueryType == CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY && QueryData.Length == 4 && QueryData.EqualsTo(new byte[] { 0x2f, 0x83, 0x00, 0xfe }))
                                                            {
                                                                PokeAddress(0x00555cb4, new byte[] { 0x2f, 0x83, 0x0f, 0xff }, clientChannel);
                                                                PokeAddress(0x00556740, new byte[] { 0x2b, 0x83, 0x0f, 0xff }, clientChannel);
                                                            }
                                                            break;
                                                        case 0x1054e358:
                                                            // Sets WorldCorePointer.
                                                            if (clientCheatQuery.QueryType == CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY && QueryData.Length == 4)
                                                                data.ClientObject.SetWorldCorePointer(BitConverter.ToUInt32(BitConverter.IsLittleEndian ? EndianUtils.ReverseArray(QueryData) : QueryData));
                                                            break;
                                                        case 0x0001193F:
                                                            // Sets ProtocolVersion.
                                                            if (clientCheatQuery.QueryType == CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY && QueryData.Length == 1)
                                                                data.ClientObject.ProtocolVersion = QueryData[0];
                                                            break;
                                                        case 0x0016dac0:
                                                            // Patches out the forceInvite command.
                                                            if (MediusClass.Settings.PlaystationHomeForceInviteExploitPatch && MediusClass.Settings.PokePatchOn && clientCheatQuery.QueryType == CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY && QueryData.Length == 4 && QueryData.EqualsTo(new byte[] { 0x2f, 0x80, 0x00, 0x00 }))
                                                                PokeAddress(0x0016dac0, new byte[] { 0x2f, 0x80, 0x00, 0x02 }, clientChannel);
                                                            break;
                                                    }
                                                    break;
                                                default:
                                                    break;
                                            }
                                            break;
                                        case "Retail":
                                            switch (data.ClientObject.ClientHomeData.Version)
                                            {
                                                case "01.86.09":
                                                    switch (clientCheatQuery.StartAddress)
                                                    {
                                                        case 0x006f59b8:
                                                            // Grant PS Plus for 1.86 retail.
                                                            if (MediusClass.Settings.PokePatchOn && clientCheatQuery.QueryType == CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY && QueryData.Length == 4 && QueryData.EqualsTo(new byte[] { 0x54, 0x63, 0xd9, 0x7e }))
                                                            {
                                                                byte[] liPatch = new byte[] { 0x38, 0x60, 0x00, 0x01 };
                                                                PokeAddress(0x006f59b8, liPatch, clientChannel);
                                                                PokeAddress(0x0073bdb0, liPatch, clientChannel);
                                                            }
                                                            break;
                                                        case 0x0017A8DF:
                                                            // Sets ProtocolVersion.
                                                            if (clientCheatQuery.QueryType == CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY && QueryData.Length == 1)
                                                                data.ClientObject.ProtocolVersion = QueryData[0];
                                                            break;
                                                        case 0x002aa960:
                                                            // Disable SSFW Reward check for 1.86 retail.
                                                            if (MediusClass.Settings.PokePatchOn && clientCheatQuery.QueryType == CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY && QueryData.Length == 4 && QueryData.EqualsTo(new byte[] { 0x7c, 0x65, 0x1b, 0x78 }))
                                                                PokeAddress(0x002aa960, new byte[] { 0x48, 0x40, 0xe2, 0x2c }, clientChannel);
                                                            break;
                                                        case 0x105c24c8:
                                                            // Sets WorldCorePointer.
                                                            if (clientCheatQuery.QueryType == CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY && QueryData.Length == 4)
                                                                data.ClientObject.SetWorldCorePointer(BitConverter.ToUInt32(BitConverter.IsLittleEndian ? EndianUtils.ReverseArray(QueryData) : QueryData));
                                                            break;
                                                    }
                                                    break;
                                                default:
                                                    break;
                                            }
                                            break;
                                    }
                                }

                                switch (clientCheatQuery.SequenceId)
                                {
                                    case int.MinValue:
                                        if (data.ClientObject != null)
                                            data.ClientObject.SSFWid = Encoding.ASCII.GetString(clientCheatQuery.Data);
                                        break;
                                    case -559038737:
                                        switch (clientCheatQuery.StartAddress)
                                        {
                                            case 65536:
                                                if (data.ClientObject != null)
                                                {
                                                    if (data.ClientObject.ClientHomeData == null)
                                                        data.ClientObject.ClientHomeData = MediusClass.HomeOffsetsList.Where(x => !string.IsNullOrEmpty(x.Sha1Hash) && x.Sha1Hash[..^8]
                                                        .Equals(clientCheatQuery.Data.ToHexString(), StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

                                                    if (data.ClientObject.ClientHomeData != null)
                                                    {
                                                        switch (data.ClientObject.ClientHomeData.Type)
                                                        {
                                                            case "HDK With Offline":
                                                                switch (data.ClientObject.ClientHomeData.Version)
                                                                {
                                                                    case "01.86.09":
                                                                        CheatQuery(0x10244430, 36, clientChannel, CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY, int.MinValue);
                                                                        break;
                                                                    default:
                                                                        break;
                                                                }
                                                                break;
                                                            case "HDK Online Only":
                                                                switch (data.ClientObject.ClientHomeData.Version)
                                                                {
                                                                    default:
                                                                        break;
                                                                }
                                                                break;
                                                            case "HDK Online Only (Dbg Symbols)":
                                                                switch (data.ClientObject.ClientHomeData.Version)
                                                                {
                                                                    case "01.82.09":
                                                                        CheatQuery(0x10234440, 36, clientChannel, CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY, int.MinValue);
                                                                        break;
                                                                    default:
                                                                        break;
                                                                }
                                                                break;
                                                            case "Online Debug":
                                                            case "Online Debug (QA)":
                                                                switch (data.ClientObject.ClientHomeData.Version)
                                                                {
                                                                    case "01.83.12":
                                                                        CheatQuery(0x10244438, 36, clientChannel, CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY, int.MinValue);
                                                                        break;
                                                                    case "01.86.09":
                                                                        CheatQuery(0x10244428, 36, clientChannel, CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY, int.MinValue);
                                                                        break;
                                                                    default:
                                                                        break;
                                                                }
                                                                break;
                                                            case "Retail":
                                                                switch (data.ClientObject.ClientHomeData.Version)
                                                                {
                                                                    case "01.86.09":
                                                                        CheatQuery(0x101555f0, 36, clientChannel, CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY, int.MinValue);
                                                                        break;
                                                                    default:
                                                                        break;
                                                                }
                                                                break;
                                                        }
                                                    }
                                                    else if (!MediusClass.Settings.PlaystationHomeAllowAnyEboot)
                                                    {
                                                        string anticheatMsg = $"[SECURITY] - HOME ANTI-CHEAT - DETECTED UNKNOWN EBOOT - User:{data.ClientObject.IP + ":" + data.ClientObject.AccountName} CID:{data.MachineId}";

                                                        _ = data.ClientObject.CurrentChannel?.BroadcastSystemMessage(data.ClientObject.CurrentChannel.LocalClients.Where(x => x != data.ClientObject), anticheatMsg, byte.MaxValue);

                                                        LoggerAccessor.LogError(anticheatMsg);

                                                        // Banned
                                                        await QueueBanMessage(data).ConfigureAwait(false);

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

                #region Echos
                case RT_MSG_SERVER_ECHO serverEchoReply:
                    {

                        break;
                    }
                case RT_MSG_CLIENT_ECHO clientEcho:
                    {
                        Queue(new RT_MSG_CLIENT_ECHO() { Value = clientEcho.Value }, clientChannel);
                        break;
                    }
                #endregion

                case RT_MSG_CLIENT_APP_TOSERVER clientAppToServer:
                    {
                        if (clientAppToServer.Message != null)
                            await ProcessMediusMessage(clientAppToServer.Message, clientChannel, data);
                        break;
                    }

                #region Client Disconnect
                case RT_MSG_CLIENT_DISCONNECT _:
                    {
                        //Medius 1.08 (Used on WRC 4) haven't a state
                        if (scertClient.MediusVersion > 108)
                            data.State = ClientState.DISCONNECTED;

                        await clientChannel.CloseAsync();

                        LoggerAccessor.LogInfo($"[MAS] - Client disconnected by request with no specific reason\n");
                        break;
                    }
                case RT_MSG_CLIENT_DISCONNECT_WITH_REASON clientDisconnectWithReason:
                    {
                        if (clientDisconnectWithReason.Reason <= RT_MSG_CLIENT_DISCONNECT_REASON.RT_MSG_CLIENT_DISCONNECT_LENGTH_MISMATCH)
                            LoggerAccessor.LogInfo($"[MAS] - Disconnected by request with reason of {clientDisconnectWithReason.Reason}\n");
                        else
                            LoggerAccessor.LogInfo($"[MAS] - Disconnected by request with (application specified) reason of {clientDisconnectWithReason.Reason}\n");

                        data.State = ClientState.DISCONNECTED;
                        await clientChannel.CloseAsync();
                        break;
                    }
                #endregion

                default:
                    {
LoggerAccessor.LogWarn($"[MAS] - UNHANDLED RT MESSAGE: {message}");
                        break;
                    }
            }

            return;
        }

        protected virtual async Task ProcessMediusMessage(BaseMediusMessage message, IChannel clientChannel, ChannelData data)
        {
            var scertClient = clientChannel.GetAttribute(LIBRARY.Pipeline.Constants.SCERT_CLIENT).Get();
            if (message == null)
                return;

            var appSettings = MediusClass.GetAppSettingsOrDefault(data.ApplicationId);

            switch (message)
            {
                #region MGCL - Dme

                case MediusServerSessionBeginRequest serverSessionBeginRequest:
                    {
                        List<int> nonSecure = new() { 10010, 10031 };

                        if (data.ClientObject != null)
                        {
                            // MGCL_SEND_FAILED, MGCL_UNSUCCESSFUL
                            if (!data.ClientObject.IsConnected)
                            {
                                data.ClientObject.Queue(new MediusServerSessionBeginResponse()
                                {
                                    MessageID = serverSessionBeginRequest.MessageID,
                                    Confirmation = MGCL_ERROR_CODE.MGCL_UNSUCCESSFUL
                                });
                            }
                            else
                            {
                                IPHostEntry host = Dns.GetHostEntry(MediusClass.Settings.NATIp ?? "natservice.pdonline.scea.com");

                                data.ClientObject.LocationId = serverSessionBeginRequest.LocationID;
                                data.ClientObject.ServerType = serverSessionBeginRequest.ServerType;
                                data.ClientObject.ServerVersion = serverSessionBeginRequest.ServerVersion;
                                data.ClientObject.Port = serverSessionBeginRequest.Port;
                                data.ClientObject.SetIp(((IPEndPoint)clientChannel.RemoteAddress).Address.ToString().Trim(new char[] { ':', 'f', '{', '}' }));
                                data.ClientObject.BeginServerSession();

                                if (nonSecure.Contains(data.ClientObject.ApplicationId))
                                {
                                    // TM:BO Reply unencrypted
                                    data.ClientObject.Queue(new MediusServerSessionBeginResponse()
                                    {
                                        MessageID = serverSessionBeginRequest.MessageID,
                                        Confirmation = MGCL_ERROR_CODE.MGCL_SUCCESS,
                                        ConnectInfo = new NetConnectionInfo()
                                        {
                                            AccessKey = data.ClientObject.AccessToken,
                                            SessionKey = data.ClientObject.SessionKey,
                                            TargetWorldID = data.ClientObject.CurrentChannel!.Id,
                                            ServerKey = new RSA_KEY(),
                                            AddressList = new NetAddressList()
                                            {
                                                AddressList = new NetAddress[Constants.NET_ADDRESS_LIST_COUNT]
                                                    {
                                                new NetAddress() { Address = host.AddressList.First().ToString(), Port = MediusClass.Settings.NATPort, AddressType = NetAddressType.NetAddressTypeNATService },
                                                new NetAddress() { Address = MediusClass.ProxyServer.IPAddress.ToString(), Port = MediusClass.ProxyServer.TCPPort, AddressType = NetAddressType.NetAddressTypeExternal },
                                                    }
                                            },
                                            Type = NetConnectionType.NetConnectionTypeClientServerUDP
                                        }
                                    });
                                }
                                else
                                {
                                    // Default Reply
                                    data.ClientObject.Queue(new MediusServerSessionBeginResponse()
                                    {
                                        MessageID = serverSessionBeginRequest.MessageID,
                                        Confirmation = MGCL_ERROR_CODE.MGCL_SUCCESS,
                                        ConnectInfo = new NetConnectionInfo()
                                        {
                                            AccessKey = data.ClientObject.AccessToken,
                                            SessionKey = data.ClientObject.SessionKey,
                                            TargetWorldID = data.ClientObject.CurrentChannel!.Id,
                                            ServerKey = MediusClass.GlobalAuthPublic,
                                            AddressList = new NetAddressList()
                                            {
                                                AddressList = new NetAddress[Constants.NET_ADDRESS_LIST_COUNT]
                                                    {
                                                new NetAddress() { Address = host.AddressList.First().ToString(), Port = MediusClass.Settings.NATPort, AddressType = NetAddressType.NetAddressTypeNATService },
                                                new NetAddress() { Address = MediusClass.ProxyServer.IPAddress.ToString(), Port = MediusClass.ProxyServer.TCPPort, AddressType = NetAddressType.NetAddressTypeExternal },
                                                    }
                                            },
                                            Type = NetConnectionType.NetConnectionTypeClientServerUDP
                                        }
                                    });
                                }

                                data.ClientObject.KeepAliveUntilNextConnection();
                            }
                        }
                        break;
                    }

                case MediusServerSessionBeginRequest1 serverSessionBeginRequest1:
                    {
                        if (data.ClientObject != null)
                        {
                            IPHostEntry host = Dns.GetHostEntry(MediusClass.Settings.NATIp ?? "natservice.pdonline.scea.com");

                            data.ClientObject.LocationId = serverSessionBeginRequest1.LocationID;
                            data.ClientObject.ServerType = serverSessionBeginRequest1.ServerType;
                            data.ClientObject.Port = serverSessionBeginRequest1.Port;
                            data.ClientObject.SetIp(((IPEndPoint)clientChannel.RemoteAddress).Address.ToString().Trim(new char[] { ':', 'f', '{', '}' }));
                            data.ClientObject.BeginServerSession();

                            LoggerAccessor.LogInfo($"[MAS] - Registered MGCL client for appid {data.ClientObject.ApplicationId} with access token {data.ClientObject.AccessToken}");

                            //Send NAT Service
                            data.ClientObject.Queue(new MediusServerSessionBeginResponse()
                            {
                                MessageID = serverSessionBeginRequest1.MessageID,
                                Confirmation = MGCL_ERROR_CODE.MGCL_SUCCESS,
                                ConnectInfo = new NetConnectionInfo()
                                {
                                    AccessKey = data.ClientObject.AccessToken,
                                    SessionKey = data.ClientObject.SessionKey,
                                    TargetWorldID = data.ClientObject.CurrentChannel!.Id,
                                    ServerKey = MediusClass.GlobalAuthPublic,
                                    AddressList = new NetAddressList()
                                    {
                                        AddressList = new NetAddress[Constants.NET_ADDRESS_LIST_COUNT]
                                        {
                                        new NetAddress() { Address = host.AddressList.First().ToString(), Port = MediusClass.Settings.NATPort, AddressType = NetAddressType.NetAddressTypeNATService },
                                        new NetAddress() { Address = MediusClass.ProxyServer.IPAddress.ToString(), Port = MediusClass.ProxyServer.TCPPort, AddressType = NetAddressType.NetAddressTypeExternal },
                                        }
                                    },
                                    Type = NetConnectionType.NetConnectionTypeClientServerUDP
                                }
                            });
                        }

                        break;
                    }

                case MediusServerSessionBeginRequest2 serverSessionBeginRequest2:
                    {
                        if (data.ClientObject != null)
                        {
                            IPHostEntry host = Dns.GetHostEntry(MediusClass.Settings.NATIp ?? "natservice.pdonline.scea.com");

                            data.ClientObject.LocationId = serverSessionBeginRequest2.LocationID;
                            data.ClientObject.ServerType = serverSessionBeginRequest2.ServerType;
                            data.ClientObject.Port = serverSessionBeginRequest2.Port;
                            data.ClientObject.SetIp(((IPEndPoint)clientChannel.RemoteAddress).Address.ToString().Trim(new char[] { ':', 'f', '{', '}' }));
                            data.ClientObject.BeginServerSession();

                            //Send NAT Service
                            data.ClientObject.Queue(new MediusServerSessionBeginResponse()
                            {
                                MessageID = serverSessionBeginRequest2.MessageID,
                                Confirmation = MGCL_ERROR_CODE.MGCL_SUCCESS,
                                ConnectInfo = new NetConnectionInfo()
                                {
                                    AccessKey = data.ClientObject.AccessToken,
                                    SessionKey = data.ClientObject.SessionKey,
                                    TargetWorldID = data.ClientObject.CurrentChannel!.Id,
                                    ServerKey = MediusClass.GlobalAuthPublic,
                                    AddressList = new NetAddressList()
                                    {
                                        AddressList = new NetAddress[Constants.NET_ADDRESS_LIST_COUNT]
                                        {
                                        new NetAddress() { Address = host.AddressList.First().ToString(), Port = MediusClass.Settings.NATPort, AddressType = NetAddressType.NetAddressTypeNATService },
                                        new NetAddress() { Address = MediusClass.ProxyServer.IPAddress.ToString(), Port = MediusClass.ProxyServer.TCPPort, AddressType = NetAddressType.NetAddressTypeExternal },
                                        }
                                    },
                                    Type = NetConnectionType.NetConnectionTypeClientServerUDP
                                }
                            });
                        }

                        break;
                    }

                case MediusServerAuthenticationRequest mgclAuthRequest:
                    {
                        List<int> nonSecure = new() { 10010, 10031, 10190 };

                        if (data.ClientObject != null)
                        {
                            data.ClientObject.MGCL_TRUST_LEVEL = mgclAuthRequest.TrustLevel;
                            data.ClientObject.NetConnectionType = NetConnectionType.NetConnectionTypeClientServerTCP;

                            if (mgclAuthRequest.AddressList.AddressList[0].AddressType == NetAddressType.NetAddressTypeBinaryExternal)
                                data.ClientObject.SetIp(ConvertFromIntegerToIpAddress(mgclAuthRequest.AddressList.AddressList[0].BinaryAddress));
                            else if (mgclAuthRequest.AddressList.AddressList[0].AddressType == NetAddressType.NetAddressTypeBinaryExternalVport
                                || mgclAuthRequest.AddressList.AddressList[0].AddressType == NetAddressType.NetAddressTypeBinaryInternalVport)
                            {
                                data.ClientObject.SetIp(mgclAuthRequest.AddressList.AddressList[0].IPBinaryBitOne + "." +
                                        mgclAuthRequest.AddressList.AddressList[0].IPBinaryBitTwo + "." +
                                        mgclAuthRequest.AddressList.AddressList[0].IPBinaryBitThree + "." +
                                        mgclAuthRequest.AddressList.AddressList[0].IPBinaryBitFour);
                            }
                            else
                                // NetAddressTypeExternal
                                data.ClientObject.SetIp(mgclAuthRequest.AddressList.AddressList[0].Address ?? "0.0.0.0");

                            if (nonSecure.Contains(data.ClientObject.ApplicationId))
                            {
                                data.ClientObject.Queue(new MediusServerAuthenticationResponse()
                                {
                                    MessageID = mgclAuthRequest.MessageID,
                                    Confirmation = MGCL_ERROR_CODE.MGCL_SUCCESS,
                                    ConnectInfo = new NetConnectionInfo()
                                    {
                                        AccessKey = data.ClientObject.AccessToken,
                                        SessionKey = data.ClientObject.SessionKey,
                                        TargetWorldID = data.ClientObject.CurrentChannel!.Id,
                                        ServerKey = new RSA_KEY(),
                                        AddressList = new NetAddressList()
                                        {
                                            AddressList = new NetAddress[Constants.NET_ADDRESS_LIST_COUNT]
                                            {
                                            new NetAddress() { Address = MediusClass.ProxyServer.IPAddress.ToString(), Port = MediusClass.ProxyServer.TCPPort, AddressType = NetAddressType.NetAddressTypeExternal },
                                            new NetAddress() { AddressType = NetAddressType.NetAddressNone }
                                            }
                                        },
                                        Type = NetConnectionType.NetConnectionTypeClientServerTCP
                                    }
                                });

                                // Keep the client alive until the dme objects connects to MPS or times out
                                data.ClientObject.OnConnected();
                                data.ClientObject.KeepAliveUntilNextConnection();
                            }
                            else
                            {
                                data.ClientObject.Queue(new MediusServerAuthenticationResponse()
                                {
                                    MessageID = mgclAuthRequest.MessageID,
                                    Confirmation = MGCL_ERROR_CODE.MGCL_SUCCESS,
                                    ConnectInfo = new NetConnectionInfo()
                                    {
                                        AccessKey = data.ClientObject.AccessToken,
                                        SessionKey = data.ClientObject.SessionKey,
                                        TargetWorldID = data.ClientObject.CurrentChannel!.Id,
                                        ServerKey = MediusClass.GlobalAuthPublic,
                                        AddressList = new NetAddressList()
                                        {
                                            AddressList = new NetAddress[Constants.NET_ADDRESS_LIST_COUNT]
                                            {
                                            new NetAddress() { Address = MediusClass.ProxyServer.IPAddress.ToString(), Port = MediusClass.ProxyServer.TCPPort, AddressType = NetAddressType.NetAddressTypeExternal },
                                            new NetAddress() { AddressType = NetAddressType.NetAddressNone },
                                            }
                                        },
                                        Type = NetConnectionType.NetConnectionTypeClientServerTCP
                                    }
                                });

                                // Keep the client alive until the dme objects connects to MPS or times out
                                data.ClientObject.OnConnected();
                                data.ClientObject.KeepAliveUntilNextConnection();
                            }
                        }

                        break;
                    }

                case MediusServerSetAttributesRequest mgclSetAttrRequest:
                    {
                        ClientObject? dmeObject = data.ClientObject;
                        if (dmeObject == null)
                        {
                            LoggerAccessor.LogError($"[MAS] - Non-DME Client sending MGCL messages.");
                            break;
                        }

                        dmeObject.MGCL_SERVER_ATTRIBUTES = mgclSetAttrRequest.Attributes;
                        dmeObject.SetIpPort(mgclSetAttrRequest.ListenServerAddress);

                        // Reply with success
                        dmeObject.Queue(new MediusServerSetAttributesResponse()
                        {
                            MessageID = mgclSetAttrRequest.MessageID,
                            Confirmation = MGCL_ERROR_CODE.MGCL_SUCCESS
                        });
                        break;
                    }

                case MediusServerSessionEndRequest sessionEndRequest:
                    {
                        if (data.ClientObject == null)
                        {
LoggerAccessor.LogError($"[MAS] - INVALID OPERATION: {clientChannel} is trying to end server session without an Client Object");
                            break;
                        }

                        data.ClientObject.EndServerSession();

                        Queue(new RT_MSG_SERVER_APP()
                        {
                            Message = new MediusServerSessionEndResponse()
                            {
                                MessageID = sessionEndRequest.MessageID,
                                ErrorCode = MGCL_ERROR_CODE.MGCL_SUCCESS
                            }
                        }, clientChannel);

                        break;
                    }

                case MediusServerReport serverReport:
                    {
                        data.ClientObject?.OnServerReport(serverReport);
                        break;
                    }

                #endregion

                #region Session

                case MediusExtendedSessionBeginRequest extendedSessionBeginRequest:
                    {
                        if (data.ClientObject != null)
                        {
                            data.ClientObject.MediusConnectionType = extendedSessionBeginRequest.ConnectionClass;

                            await HorizonServerConfiguration.Database.GetServerFlags().ContinueWith((r) =>
                            {
                                if (r.IsCompletedSuccessfully && r.Result != null && r.Result.MaintenanceMode != null)
                                {
                                    // Ensure that maintenance is active
                                    // Ensure that we're past the from date
                                    // Ensure that we're before the to date (if set)
                                    if (r.Result.MaintenanceMode.IsActive
                                         && DateTimeUtils.GetHighPrecisionUtcTime() > r.Result.MaintenanceMode.FromDt
                                         && (!r.Result.MaintenanceMode.ToDt.HasValue
                                             || r.Result.MaintenanceMode.ToDt > DateTimeUtils.GetHighPrecisionUtcTime()))
                                        QueueBanMessage(data, "Server in maintenance mode.");
                                    else
                                    {
                                        // Reply
                                        data.ClientObject.Queue(new MediusSessionBeginResponse()
                                        {
                                            MessageID = extendedSessionBeginRequest.MessageID,
                                            StatusCode = MediusCallbackStatus.MediusSuccess,
                                            SessionKey = data.ClientObject.SessionKey
                                        });
                                    }
                                }
                            });
                        }
                        break;
                    }
                case MediusSessionBeginRequest sessionBeginRequest:
                    {
                        if (data.ClientObject != null)
                        {
                            data.ClientObject.MediusConnectionType = sessionBeginRequest.ConnectionClass;

LoggerAccessor.LogInfo($"[MAS] - Retrieved ApplicationID {data.ClientObject.ApplicationId} from client connection");

                            await HorizonServerConfiguration.Database.GetServerFlags().ContinueWith((r) =>
                            {
                                if (r.IsCompletedSuccessfully && r.Result != null && r.Result.MaintenanceMode != null)
                                {
                                    #region Maintenance Mode?
                                    // Ensure that maintenance is active
                                    // Ensure that we're past the from date
                                    // Ensure that we're before the to date (if set)
                                    if (r.Result.MaintenanceMode.IsActive
                                         && DateTimeUtils.GetHighPrecisionUtcTime() > r.Result.MaintenanceMode.FromDt
                                         && (!r.Result.MaintenanceMode.ToDt.HasValue
                                             || r.Result.MaintenanceMode.ToDt > DateTimeUtils.GetHighPrecisionUtcTime()))
                                        QueueBanMessage(data, "Server in maintenance mode.");
                                    #endregion

                                    #region Send Response
                                    else
                                    {
                                        // Reply
                                        data.ClientObject.Queue(new MediusSessionBeginResponse()
                                        {
                                            MessageID = sessionBeginRequest.MessageID,
                                            StatusCode = MediusCallbackStatus.MediusSuccess,
                                            SessionKey = data.ClientObject.SessionKey
                                        });
                                    }
                                    #endregion
                                }
                            });
                        }

                        break;
                    }

                case MediusSessionBeginRequest1 sessionBeginRequest1:
                    {
                        if (data.ClientObject != null)
                        {
                            data.ClientObject.MediusConnectionType = sessionBeginRequest1.ConnectionClass;

LoggerAccessor.LogInfo($"[MAS] - Retrieved ApplicationID {data.ClientObject.ApplicationId} from client connection");

                            #region SystemMessageSingleTest Disabled?
                            if (MediusClass.Settings.SystemMessageSingleTest)
                            {
                                await QueueBanMessage(data, "MAS.Notification Test:\nYou have been banned from this server.");

                                await data.ClientObject.Logout();
                            }
                            #endregion
                            else
                            {
                                await HorizonServerConfiguration.Database.GetServerFlags().ContinueWith((r) =>
                                {
                                    if (r.IsCompletedSuccessfully && r.Result != null && r.Result.MaintenanceMode != null)
                                    {
                                        #region Maintenance Mode?
                                        // Ensure that maintenance is active
                                        // Ensure that we're past the from date
                                        // Ensure that we're before the to date (if set)
                                        if (r.Result.MaintenanceMode.IsActive
                                             && DateTimeUtils.GetHighPrecisionUtcTime() > r.Result.MaintenanceMode.FromDt
                                             && (!r.Result.MaintenanceMode.ToDt.HasValue
                                                 || r.Result.MaintenanceMode.ToDt > DateTimeUtils.GetHighPrecisionUtcTime()))
                                            QueueBanMessage(data, "Server in maintenance mode.");

                                        #endregion

                                        #region Send Response
                                        else
                                        {
                                            // Reply
                                            data.ClientObject.Queue(new MediusSessionBeginResponse()
                                            {
                                                MessageID = sessionBeginRequest1.MessageID,
                                                StatusCode = MediusCallbackStatus.MediusSuccess,
                                                SessionKey = data.ClientObject.SessionKey
                                            });
                                        }
                                        #endregion
                                    }
                                });
                            }
                        }
                        break;
                    }

                case MediusSessionEndRequest sessionEndRequest:
                    {
                        ClientObject? clientToEnd = MediusClass.Manager.GetClientBySessionKey(sessionEndRequest.SessionKey, data.ApplicationId);

                        if (clientToEnd == null)
                        {
                            LoggerAccessor.LogError($"[MAS] - INVALID OPERATION: {clientChannel} is trying to end session of a non-existing Client Object with SessionKey: {sessionEndRequest.SessionKey}");
                            break;
                        }

                        clientToEnd.OnDisconnected();

                        Queue(new RT_MSG_SERVER_APP()
                        {
                            Message = new MediusSessionEndResponse()
                            {
                                MessageID = sessionEndRequest.MessageID,
                                StatusCode = MediusCallbackStatus.MediusSuccess,
                            }
                        }, clientChannel);

                        break;
                    }

                #endregion

                #region Localization

                case MediusSetLocalizationParamsRequest setLocalizationParamsRequest:
                    {
                        if (data.ClientObject == null)
                        {
LoggerAccessor.LogError($"[MAS] - INVALID OPERATION: {clientChannel} sent {setLocalizationParamsRequest} without a session.");
                            break;
                        }

                        data.ClientObject.CharacterEncoding = setLocalizationParamsRequest.CharacterEncoding;
                        data.ClientObject.LanguageType = setLocalizationParamsRequest.Language;

                        data.ClientObject.Queue(new MediusStatusResponse()
                        {
                            Type = 0xA4,
                            Class = setLocalizationParamsRequest.PacketClass,
                            MessageID = setLocalizationParamsRequest.MessageID,
                            StatusCode = MediusCallbackStatus.MediusSuccess
                        });
                        break;
                    }

                case MediusSetLocalizationParamsRequest1 setLocalizationParamsRequest1:
                    {
                        if (data.ClientObject == null)
                        {
LoggerAccessor.LogError($"[MAS] - INVALID OPERATION: {clientChannel} sent {setLocalizationParamsRequest1} without a session.");
                            break;
                        }

                        data.ClientObject.CharacterEncoding = setLocalizationParamsRequest1.CharacterEncoding;
                        data.ClientObject.LanguageType = setLocalizationParamsRequest1.Language;
                        data.ClientObject.TimeZone = setLocalizationParamsRequest1.TimeZone;
                        data.ClientObject.LocationId = setLocalizationParamsRequest1.LocationID;

                        data.ClientObject.Queue(new MediusStatusResponse()
                        {
                            Type = 0xA4,
                            Class = (NetMessageClass)1,
                            MessageID = setLocalizationParamsRequest1.MessageID,
                            StatusCode = MediusCallbackStatus.MediusSuccess
                        });
                        break;
                    }
                case MediusSetLocalizationParamsRequest2 setLocalizationParamsRequest2:
                    {
                        if (data.ClientObject == null)
                        {
LoggerAccessor.LogError($"[MAS] - INVALID OPERATION: {clientChannel} sent {setLocalizationParamsRequest2} without a session.");
                            break;
                        }

                        data.ClientObject.CharacterEncoding = setLocalizationParamsRequest2.CharacterEncoding;
                        data.ClientObject.LanguageType = setLocalizationParamsRequest2.Language;
                        data.ClientObject.TimeZone = setLocalizationParamsRequest2.TimeZone;

                        data.ClientObject.Queue(new MediusStatusResponse()
                        {
                            Type = 0xA4,
                            Class = (NetMessageClass)1,
                            MessageID = setLocalizationParamsRequest2.MessageID,
                            StatusCode = MediusCallbackStatus.MediusSuccess
                        });
                        break;
                    }

                #endregion

                #region Game

                case MediusGetTotalGamesRequest getTotalGamesRequest:
                    {
                        // ERROR - Need a session
                        if (data.ClientObject == null)
                        {
LoggerAccessor.LogError($"[MAS] - INVALID OPERATION: {clientChannel} sent {getTotalGamesRequest} without a session.");
                            break;
                        }

                        // ERROR -- Need to be logged in
                        if (!data.ClientObject.IsLoggedIn)
                        {
LoggerAccessor.LogError($"[MAS] - INVALID OPERATION: {clientChannel} sent {getTotalGamesRequest} without being logged in.");
                            break;
                        }

                        data.ClientObject.Queue(new MediusGetTotalGamesResponse()
                        {
                            MessageID = getTotalGamesRequest.MessageID,
                            Total = 0,
                            StatusCode = MediusCallbackStatus.MediusRequestDenied
                        });
                        break;
                    }

                #endregion

                #region Channel

                case MediusGetTotalChannelsRequest getTotalChannelsRequest:
                    {
                        // ERROR - Need a session
                        if (data.ClientObject == null)
                        {
LoggerAccessor.LogError($"[MAS] - INVALID OPERATION: {clientChannel} sent {getTotalChannelsRequest} without a session.");
                            break;
                        }

                        // ERROR -- Need to be logged in
                        if (!data.ClientObject.IsLoggedIn)
                        {
LoggerAccessor.LogError($"[MAS] - INVALID OPERATION: {clientChannel} sent {getTotalChannelsRequest} without being logged in.");
                            break;
                        }

                        data.ClientObject.Queue(new MediusGetTotalChannelsResponse()
                        {
                            MessageID = getTotalChannelsRequest.MessageID,
                            Total = 0,
                            StatusCode = MediusCallbackStatus.MediusRequestDenied,
                        });
                        break;
                    }

                case MediusSetLobbyWorldFilterRequest setLobbyWorldFilterRequest:
                    {
                        Queue(new RT_MSG_SERVER_APP()
                        {
                            Message = new MediusSetLobbyWorldFilterResponse()
                            {
                                MessageID = setLobbyWorldFilterRequest.MessageID,
                                StatusCode = MediusCallbackStatus.MediusRequestDenied
                            }
                        });

                        break;
                    }
                #endregion

                #region DNAS CID Check

                case MediusMachineSignaturePost machineSignaturePost:
                    {
                        if (Settings.DnasEnablePost == true)
                        {
                            //Sets the CachedPlayer's MachineId
                            data.MachineId = BitConverter.ToString(machineSignaturePost.MachineSignature);

LoggerAccessor.LogDebug($"[MAS] - Session Key {machineSignaturePost.SessionKey} | Posting Machine signatures");

                            // Then post to the Database if logged in
                            if (data.ClientObject?.IsLoggedIn ?? false)
                                await HorizonServerConfiguration.Database.PostMachineId(data.ClientObject.AccountId, data.MachineId);
                        }
                        else
                        {
                            //DnasEnablePost set to false;
                        }

                        break;
                    }

                case MediusDnasSignaturePost dnasSignaturePost:
                    {
                        if (Settings.DnasEnablePost)
                        {
                            //If DNAS Signature Post is the PS2/PSP/PS3 Console ID then continue
                            if (dnasSignaturePost.DnasSignatureType == MediusDnasCategory.DnasConsoleID)
                            {
                                data.MachineId = BitConverter.ToString(dnasSignaturePost.DnasSignature);

LoggerAccessor.LogDebug($"[MAS] - Posting ConsoleID - ConsoleSigSize={dnasSignaturePost.DnasSignatureLength}");

                                // Then post to the Database if logged in
                                if (data.ClientObject?.IsLoggedIn ?? false)
                                    await HorizonServerConfiguration.Database.PostMachineId(data.ClientObject.AccountId, data.MachineId);
                            }

                            else if (dnasSignaturePost.DnasSignatureType == MediusDnasCategory.DnasTitleID)
LoggerAccessor.LogError($"[MAS] - DnasSignaturePost Error - Invalid SignatureType");

                            else if (dnasSignaturePost.DnasSignatureType == MediusDnasCategory.DnasDiskID)
LoggerAccessor.LogDebug($"[MAS] - Posting DiskID - DiskSigSize={dnasSignaturePost.DnasSignatureLength}");
                        }
                        else
                        {
                            //DnasEnablePost false, no Post;
                        }
                        break;
                    }
                #endregion

                #region AccessLevel (2.12)

                case MediusGetAccessLevelInfoRequest getAccessLevelInfoRequest:
                    {
                        // ERROR - Need a session
                        if (data.ClientObject == null)
                        {
LoggerAccessor.LogError($"[MAS] - INVALID OPERATION: {clientChannel} sent {getAccessLevelInfoRequest} without a session.");
                            break;
                        }

                        // ERROR -- Need to be logged in
                        if (!data.ClientObject.IsLoggedIn)
                        {
LoggerAccessor.LogError($"[MAS] - INVALID OPERATION: {clientChannel} sent {getAccessLevelInfoRequest} without being logged in.");
                            break;
                        }

                        //int adminAccessLevel = 4;

                        data.ClientObject.Queue(new MediusGetAccessLevelInfoResponse()
                        {
                            MessageID = getAccessLevelInfoRequest.MessageID,
                            StatusCode = MediusCallbackStatus.MediusSuccess,
                            AccessLevel = MediusAccessLevelType.MEDIUS_ACCESSLEVEL_MODERATOR,
                        });
                        break;
                    }

                #endregion 

                #region Version Server
                case MediusVersionServerRequest mediusVersionServerRequest:
                    {
                        List<int> appIdBeforeSession = new() { 10442, 10724 };

                        // ERROR - Need a session
                        if (data.ClientObject == null && !appIdBeforeSession.Contains(data.ApplicationId)) // KILLZONE PS2 GET VERSION SERVER INFO BEFORE SESSION
                        {
LoggerAccessor.LogError($"[MAS] - INVALID OPERATION: {clientChannel} sent {mediusVersionServerRequest} without a session.");
                            break;
                        }

                        if (Settings.MediusServerVersionOverride == true)
                        {
                            #region F1 2005 PAL
                            List<int> F12005AppIds = new List<int> { 10952, 10954 };
                            // F1 2005 PAL SCES / F1 2005 PAL TCES
                            if (F12005AppIds.Contains(data.ApplicationId))
                            {
                                data.ClientObject?.Queue(new MediusVersionServerResponse()
                                {
                                    MessageID = mediusVersionServerRequest.MessageID,
                                    VersionServer = "Medius Authentication Server Version 2.9.0009",
                                    StatusCode = MediusCallbackStatus.MediusSuccess,
                                });
                            }
                            #endregion

                            #region Socom 1
                            else if (data.ApplicationId == 10274)
                            {
                                data.ClientObject?.Queue(new MediusVersionServerResponse()
                                {
                                    MessageID = mediusVersionServerRequest.MessageID,
                                    VersionServer = "Medius Authentication Server Version 1.40.PRE8",
                                    StatusCode = MediusCallbackStatus.MediusSuccess,
                                });
                            }
                            #endregion

                            #region EyeToy Chat Beta
                            else if (data.ApplicationId == 10550)
                            {
                                data.ClientObject?.Queue(new MediusVersionServerResponse()
                                {
                                    MessageID = mediusVersionServerRequest.MessageID,
                                    VersionServer = "Medius Authentication Server Version 1.43.0000",
                                    StatusCode = MediusCallbackStatus.MediusSuccess,
                                });
                            }
                            #endregion

                            #region Killzone Beta/Retail
                            else if (appIdBeforeSession.Contains(data.ApplicationId))
                            {
                                //data.ClientObject = MediusClass.LobbyServer.ReserveClient(mediusVersionServerRequest);

                                data.SendQueue.Enqueue(new RT_MSG_SERVER_APP()
                                {
                                    Message = new MediusVersionServerResponse()
                                    {
                                        MessageID = mediusVersionServerRequest.MessageID,
                                        VersionServer = "Medius Authentication Server Version 1.50.0009",
                                        StatusCode = MediusCallbackStatus.MediusSuccess,
                                    }
                                });
                            }
                            else
                            #endregion

                            //Default
                            {
                                data.ClientObject?.Queue(new MediusVersionServerResponse()
                                {
                                    MessageID = mediusVersionServerRequest.MessageID,
                                    VersionServer = "Medius Authentication Server Version 3.05.201109161400",
                                    StatusCode = MediusCallbackStatus.MediusSuccess,
                                });
                            }
                        }
                        else
                        {
                            // If MediusServerVersionOverride is false, we send our own Version String
                            // AND if its Killzone PS2 we make the ClientObject BEFORE SESSIONBEGIN
                            if (appIdBeforeSession.Contains(data.ApplicationId))
                            {
                                //data.ClientObject = Program.LobbyServer.ReserveClient(mediusVersionServerRequest);
                                data.SendQueue.Enqueue(new RT_MSG_SERVER_APP()
                                {
                                    Message = new MediusVersionServerResponse()
                                    {
                                        MessageID = mediusVersionServerRequest.MessageID,
                                        VersionServer = "Medius Authentication Server Version 1.50.0009",
                                        StatusCode = MediusCallbackStatus.MediusSuccess,
                                    }
                                });
                            }
                            else
                            {
                                data.ClientObject?.Queue(new MediusVersionServerResponse()
                                {
                                    MessageID = mediusVersionServerRequest.MessageID,
                                    VersionServer = Settings.MASVersion,
                                    StatusCode = MediusCallbackStatus.MediusSuccess,
                                });
                            }
                        }

                        break;
                    }

                #endregion

                #region Co-Locations
                case MediusGetLocationsRequest getLocationsRequest:
                    {
                        // ERROR - Need a session but doesn't need to be logged in
                        if (data.ClientObject == null)
                        {
LoggerAccessor.LogError($"[MAS] - INVALID OPERATION: {clientChannel} sent {getLocationsRequest} without a session.");
                            break;
                        }

                        await HorizonServerConfiguration.Database.GetLocations(data.ClientObject.ApplicationId).ContinueWith(r =>
                        {
                            LocationDTO[]? locations = r.Result;

                            if (r.IsCompletedSuccessfully)
                            {
                                if (locations?.Length == 0)
                                {
                                    data.ClientObject.Queue(new MediusGetLocationsResponse()
                                    {
                                        MessageID = getLocationsRequest.MessageID,
                                        StatusCode = MediusCallbackStatus.MediusNoResult,
                                        EndOfList = true
                                    });
                                }
                                else
                                {
                                    var responses = locations?.Select(x => new MediusGetLocationsResponse()
                                    {
                                        MessageID = getLocationsRequest.MessageID,
                                        StatusCode = MediusCallbackStatus.MediusSuccess,
                                        LocationId = x.Id,
                                        LocationName = x.Name
                                    }).ToList();

                                    if (responses != null)
                                    {
                                        responses[responses.Count - 1].EndOfList = true;
                                        data.ClientObject.Queue(responses);
                                    }
                                }
                            }
                            else
                            {
LoggerAccessor.LogError($"[MAS] - GetLocationsRequest failed [{r.Exception}]");

                                data.ClientObject.Queue(new MediusGetLocationsResponse()
                                {
                                    MessageID = getLocationsRequest.MessageID,
                                    StatusCode = MediusCallbackStatus.MediusDBError,
                                    LocationId = -1,
                                    LocationName = "0",
                                    EndOfList = true
                                });
                            }
                        });
                        break;
                    }

                case MediusPickLocationRequest pickLocationRequest:
                    {
                        // ERROR - Need a session
                        if (data.ClientObject == null)
                        {
LoggerAccessor.LogError($"[MAS] - INVALID OPERATION: {clientChannel} sent {pickLocationRequest} without a session.");
                            break;
                        }

                        data.ClientObject.LocationId = pickLocationRequest.LocationID;

                        data.ClientObject.Queue(new MediusPickLocationResponse()
                        {
                            MessageID = pickLocationRequest.MessageID,
                            StatusCode = MediusCallbackStatus.MediusSuccess
                        });
                        break;
                    }

                #endregion

                #region Account

                case MediusAccountRegistrationRequest accountRegRequest:
                    {
                        if (data.ClientObject == null)
                        {
LoggerAccessor.LogError($"[MAS] - INVALID OPERATION: {clientChannel} sent {accountRegRequest} without a session.");
                            break;
                        }

                        // Check that account creation is enabled
                        if (appSettings.DisableAccountCreation)
                        {
                            // Reply error
                            data.ClientObject.Queue(new MediusAccountRegistrationResponse()
                            {
                                MessageID = accountRegRequest.MessageID,
                                StatusCode = MediusCallbackStatus.MediusFail
                            });

                            return;
                        }

                        // validate name
                        if (!MediusClass.PassTextFilter(data.ApplicationId, TextFilterContext.ACCOUNT_NAME, accountRegRequest.AccountName))
                        {
                            data.ClientObject.Queue(new MediusAccountRegistrationResponse()
                            {
                                MessageID = accountRegRequest.MessageID,
                                StatusCode = MediusCallbackStatus.MediusFail,
                            });
                            return;
                        }

                        await HorizonServerConfiguration.Database.CreateAccount(new CreateAccountDTO()
                        {
                            AccountName = accountRegRequest.AccountName,
                            AccountPassword = ComputeSHA256(accountRegRequest.Password),
                            MachineId = data.MachineId,
                            MediusStats = Convert.ToBase64String(new byte[Constants.ACCOUNTSTATS_MAXLEN]),
                            AppId = data.ClientObject.ApplicationId
                        }, clientChannel).ContinueWith((r) =>
                        {
                            if (r.IsCompletedSuccessfully && r.Result != null)
                            {
                                // Reply with account id
                                data.ClientObject.Queue(new MediusAccountRegistrationResponse()
                                {
                                    MessageID = accountRegRequest.MessageID,
                                    StatusCode = MediusCallbackStatus.MediusSuccess,
                                    AccountID = r.Result.AccountId
                                });
                            }
                            else
                            {
                                // Reply error
                                data.ClientObject.Queue(new MediusAccountRegistrationResponse()
                                {
                                    MessageID = accountRegRequest.MessageID,
                                    StatusCode = MediusCallbackStatus.MediusAccountAlreadyExists
                                });
                            }
                        });
                        break;
                    }
                case MediusAccountGetIDRequest accountGetIdRequest:
                    {
                        if (data.ClientObject == null)
                        {
LoggerAccessor.LogError($"[MAS] - INVALID OPERATION: {clientChannel} sent {accountGetIdRequest} without a session.");
                            break;
                        }

                        await HorizonServerConfiguration.Database.GetAccountByName(accountGetIdRequest.AccountName, data.ClientObject.ApplicationId).ContinueWith((r) =>
                        {
                            if (r.IsCompletedSuccessfully && r.Result != null)
                            {
                                // Success
                                data?.ClientObject?.Queue(new MediusAccountGetIDResponse()
                                {
                                    MessageID = accountGetIdRequest.MessageID,
                                    AccountID = r.Result.AccountId,
                                    StatusCode = MediusCallbackStatus.MediusSuccess
                                });
                            }
                            else
                            {
                                // Fail
                                data?.ClientObject?.Queue(new MediusAccountGetIDResponse()
                                {
                                    MessageID = accountGetIdRequest.MessageID,
                                    AccountID = -1,
                                    StatusCode = MediusCallbackStatus.MediusAccountNotFound
                                });
                            }
                        });

                        break;
                    }
                case MediusAccountDeleteRequest accountDeleteRequest:
                    {
                        // ERROR - Need a session
                        if (data.ClientObject == null)
                        {
LoggerAccessor.LogError($"[MAS] - INVALID OPERATION: {clientChannel} sent {accountDeleteRequest} without a session.");
                            break;
                        }

                        // ERROR -- Need to be logged in
                        if (!data.ClientObject.IsLoggedIn)
                        {
LoggerAccessor.LogError($"[MAS] - INVALID OPERATION: {clientChannel} sent {accountDeleteRequest} without being logged in.");
                            break;
                        }

                        if (!string.IsNullOrEmpty(data.ClientObject.AccountName))
                        {
                            await HorizonServerConfiguration.Database.DeleteAccount(data.ClientObject.AccountName, data.ClientObject.ApplicationId).ContinueWith((r) =>
                            {
                                if (r.IsCompletedSuccessfully && r.Result)
                                {
LoggerAccessor.LogInfo($"[MAS] - Logging out {data?.ClientObject?.AccountName}'s account\nDeleting from Medius Server");

                                    data?.ClientObject?.Logout();

                                    data?.ClientObject?.Queue(new MediusAccountDeleteResponse()
                                    {
                                        MessageID = accountDeleteRequest.MessageID,
                                        StatusCode = MediusCallbackStatus.MediusSuccess
                                    });
                                }
                                else
                                {
LoggerAccessor.LogWarn($"[MAS] - Logout FAILED for {data?.ClientObject?.AccountName}'s account\nData still persistent on Medius Server");

                                    data?.ClientObject?.Queue(new MediusAccountDeleteResponse()
                                    {
                                        MessageID = accountDeleteRequest.MessageID,
                                        StatusCode = MediusCallbackStatus.MediusDBError
                                    });
                                }
                            });
                        }
                        break;
                    }
                case MediusAnonymousLoginRequest anonymousLoginRequest:
                    {
                        if (data.ClientObject == null)
                        {
LoggerAccessor.LogError($"[MAS] - INVALID OPERATION: {clientChannel} sent {anonymousLoginRequest} without a session.");
                            break;
                        }

                        // get client ip
                        string clientIp = Regex.Match(clientChannel.RemoteAddress.ToString(), @"\[(?:.*?:)?(?<ip>\d+\.\d+\.\d+\.\d+)\]").Groups["ip"].Value;

                        // ip is banned
                        if (await HorizonServerConfiguration.Database.GetIsIpBanned(IPAddress.Parse(clientIp)) || (MultiServerLibraryConfiguration.VpnCheck != null && MultiServerLibraryConfiguration.VpnCheck.IsVpnOrProxy(clientIp)))
                        {
                            LoggerAccessor.LogWarn($"[MAS] - MediusAnonymousLoginRequest: Anonymous User tried to login with banned IP {clientIp}");

                            data?.ClientObject?.Queue(new MediusAnonymousLoginResponse()
                            {
                                MessageID = anonymousLoginRequest.MessageID,
                                StatusCode = MediusCallbackStatus.MediusAccountBanned
                            });

                            // Then queue send ban message
                            await QueueBanMessage(data, "Your IP has been banned");

                            break;
                        }

                        // cid is banned
                        else if (await HorizonServerConfiguration.Database.GetIsMacBanned(data.MachineId ?? string.Empty))
                        {
                            LoggerAccessor.LogWarn($"[MAS] - MediusAnonymousLoginRequest: Anonymous User tried to login with banned CID {data.MachineId}");

                            data?.ClientObject?.Queue(new MediusAnonymousLoginResponse()
                            {
                                MessageID = anonymousLoginRequest.MessageID,
                                StatusCode = MediusCallbackStatus.MediusAccountBanned
                            });

                            // Then queue send ban message
                            await QueueBanMessage(data, "Your CID has been banned");

                            break;
                        }

                        await LoginAnonymous(anonymousLoginRequest, clientChannel, data);
                        break;
                    }
                case MediusAccountLoginRequest accountLoginRequest:
                    {
                        // ERROR - Need a session
                        if (data.ClientObject == null)
                        {
LoggerAccessor.LogError($"[MAS] - INVALID OPERATION: {clientChannel} sent {accountLoginRequest} without a session.");
                            break;
                        }

                        await MediusClass.Plugins.OnEvent(PluginEvent.MEDIUS_ACCOUNT_LOGIN_REQUEST, new OnAccountLoginRequestArgs()
                        {
                            Player = data.ClientObject,
                            Request = accountLoginRequest
                        });

                        // Check the client isn't already logged in
                        if (MediusClass.Manager.GetClientByAccountName(accountLoginRequest.Username, data.ClientObject.ApplicationId)?.IsLoggedIn ?? false)
                        {
                            data.ClientObject.Queue(new MediusAccountLoginResponse()
                            {
                                MessageID = accountLoginRequest.MessageID,
                                StatusCode = MediusCallbackStatus.MediusAccountLoggedIn
                            });
                        }
                        else
                        {
                            // get client ip
                            string clientIp = Regex.Match(clientChannel.RemoteAddress.ToString(), @"\[(?:.*?:)?(?<ip>\d+\.\d+\.\d+\.\d+)\]").Groups["ip"].Value;

                            // ip is banned
                            if (await HorizonServerConfiguration.Database.GetIsIpBanned(IPAddress.Parse(clientIp)) || (MultiServerLibraryConfiguration.VpnCheck != null && MultiServerLibraryConfiguration.VpnCheck.IsVpnOrProxy(clientIp)))
                            {
                                LoggerAccessor.LogWarn($"[MAS] - MediusAccountLoginRequest: User {accountLoginRequest.Username} tried to login with banned IP {clientIp}");

                                data?.ClientObject?.Queue(new MediusAccountLoginResponse()
                                {
                                    MessageID = accountLoginRequest.MessageID,
                                    StatusCode = MediusCallbackStatus.MediusAccountBanned
                                });

                                // Then queue send ban message
                                await QueueBanMessage(data, "Your IP has been banned");

                                break;
                            }

                            // cid is banned
                            else if (await HorizonServerConfiguration.Database.GetIsMacBanned(data.MachineId ?? string.Empty))
                            {
                                LoggerAccessor.LogWarn($"[MAS] - MediusAccountLoginRequest: User {accountLoginRequest.Username} tried to login with banned CID {data.MachineId}");

                                data?.ClientObject?.Queue(new MediusAccountLoginResponse()
                                {
                                    MessageID = accountLoginRequest.MessageID,
                                    StatusCode = MediusCallbackStatus.MediusAccountBanned
                                });

                                // Then queue send ban message
                                await QueueBanMessage(data, "Your CID has been banned");

                                break;
                            }

                            #region SystemMessageSingleTest Disabled?
                            if (MediusClass.Settings.SystemMessageSingleTest)
                            {
                                await QueueBanMessage(data, "MAS.Notification Test:\nYou have been banned from this server.");

                                await data.ClientObject.Logout();
                            }
                            #endregion
                            else
                            {
                                _ = HorizonServerConfiguration.Database.GetAccountByName(accountLoginRequest.Username, data.ClientObject.ApplicationId, true).ContinueWith(async (r) =>
                                {
                                    if (data == null || data.ClientObject == null || !data.ClientObject.IsConnected)
                                        return;

                                    if (r.IsCompletedSuccessfully && r.Result != null && data != null && data.ClientObject != null && data.ClientObject.IsConnected)
                                    {

                                        if (r.Result.IsBanned)
                                        {
                                            // Send ban message
                                            //await QueueBanMessage(data);

                                            // Account is banned
                                            // Temporary solution is to tell the client the login failed
                                            data?.ClientObject?.Queue(new MediusAccountLoginResponse()
                                            {
                                                MessageID = accountLoginRequest.MessageID,
                                                StatusCode = MediusCallbackStatus.MediusAccountBanned
                                            });

                                        }
                                        else if (appSettings.EnableAccountWhitelist && !appSettings.AccountIdWhitelist.Contains(r.Result.AccountId))
                                        {
                                            // Account not allowed to sign in
                                            data?.ClientObject?.Queue(new MediusAccountLoginResponse()
                                            {
                                                MessageID = accountLoginRequest.MessageID,
                                                StatusCode = MediusCallbackStatus.MediusFail
                                            });
                                        }
                                        else if (MediusClass.Manager.GetClientByAccountName(accountLoginRequest.Username, data.ClientObject.ApplicationId)?.IsLoggedIn ?? false)
                                        {
                                            data.ClientObject.Queue(new MediusAccountLoginResponse()
                                            {
                                                MessageID = accountLoginRequest.MessageID,
                                                StatusCode = MediusCallbackStatus.MediusAccountLoggedIn
                                            });
                                        }

                                        else if (ComputeSHA256(accountLoginRequest.Password) == r.Result.AccountPassword)
                                            await Login(accountLoginRequest.MessageID, clientChannel, data, r.Result, false);
                                        else
                                        {
                                            // Incorrect password
                                            data?.ClientObject?.Queue(new MediusAccountLoginResponse()
                                            {
                                                MessageID = accountLoginRequest.MessageID,
                                                StatusCode = MediusCallbackStatus.MediusInvalidPassword
                                            });
                                        }
                                    }
                                    else if (appSettings.CreateAccountOnNotFound)
                                    {
                                        // Account not found, create new and login
                                        // Check that account creation is enabled
                                        if (appSettings.DisableAccountCreation)
                                        {
                                            // Reply error
                                            data?.ClientObject?.Queue(new MediusAccountLoginResponse()
                                            {
                                                MessageID = accountLoginRequest.MessageID,
                                                StatusCode = MediusCallbackStatus.MediusFail,
                                            });
                                            return;
                                        }

                                        // validate name
                                        if (data != null && !MediusClass.PassTextFilter(data.ApplicationId, TextFilterContext.ACCOUNT_NAME, accountLoginRequest.Username))
                                        {
                                            data.ClientObject?.Queue(new MediusAccountLoginResponse()
                                            {
                                                MessageID = accountLoginRequest.MessageID,
                                                StatusCode = MediusCallbackStatus.MediusVulgarityFound,
                                            });
                                            return;
                                        }

                                        await MediusClass.Plugins.OnEvent(PluginEvent.MEDIUS_PRE_ACCOUNT_CREATE_ON_NOT_FOUND, new OnAccountLoginRequestArgs()
                                        {
                                            Player = data?.ClientObject,
                                            Request = accountLoginRequest
                                        });

                                        if (data != null && data.ClientObject != null)
                                        {
                                            _ = HorizonServerConfiguration.Database.CreateAccount(new CreateAccountDTO()
                                            {
                                                AccountName = accountLoginRequest.Username,
                                                AccountPassword = ComputeSHA256(accountLoginRequest.Password),
                                                MachineId = data.MachineId,
                                                MediusStats = Convert.ToBase64String(new byte[Constants.ACCOUNTSTATS_MAXLEN]),
                                                AppId = data.ClientObject.ApplicationId
                                            }, clientChannel).ContinueWith(async (r) =>
                                            {
                                                if (r.IsCompletedSuccessfully && r.Result != null)
                                                {
                                                    await MediusClass.Plugins.OnEvent(PluginEvent.MEDIUS_POST_ACCOUNT_CREATE_ON_NOT_FOUND, new OnAccountLoginRequestArgs()
                                                    {
                                                        Player = data.ClientObject,
                                                        Request = accountLoginRequest
                                                    });
                                                    await Login(accountLoginRequest.MessageID, clientChannel, data, r.Result, false);
                                                }
                                                else
                                                {
                                                    // Reply error
                                                    data.ClientObject.Queue(new MediusAccountLoginResponse()
                                                    {
                                                        MessageID = accountLoginRequest.MessageID,
                                                        StatusCode = MediusCallbackStatus.MediusInvalidPassword
                                                    });
                                                }
                                            });
                                        }
                                    }
                                    else
                                    {
                                        // Account not found
                                        data?.ClientObject?.Queue(new MediusAccountLoginResponse()
                                        {
                                            MessageID = accountLoginRequest.MessageID,
                                            StatusCode = MediusCallbackStatus.MediusAccountNotFound,
                                        });
                                    }
                                });
                            }
                        }
                        break;
                    }

                case MediusAccountUpdatePasswordRequest accountUpdatePasswordRequest:
                    {
                        // ERROR - Need a session
                        if (data.ClientObject == null)
                        {
LoggerAccessor.LogError($"[MAS] - INVALID OPERATION: {clientChannel} sent {accountUpdatePasswordRequest} without a session.");
                            break;
                        }

                        // ERROR -- Need to be logged in
                        if (!data.ClientObject.IsLoggedIn)
                        {
LoggerAccessor.LogError($"[MAS] - INVALID OPERATION: {clientChannel} sent {accountUpdatePasswordRequest} without being logged in.");
                            break;
                        }

                        // Post New Password to Database
                        await HorizonServerConfiguration.Database.PostAccountUpdatePassword(data.ClientObject.AccountId, accountUpdatePasswordRequest.OldPassword, accountUpdatePasswordRequest.NewPassword).ContinueWith((r) =>
                        {
                            if (data == null || data.ClientObject == null || !data.ClientObject.IsConnected)
                                return;

                            if (r.IsCompletedSuccessfully && r.Result)
                            {
                                data.ClientObject.Queue(new MediusAccountUpdatePasswordStatusResponse()
                                {
                                    MessageID = accountUpdatePasswordRequest.MessageID,
                                    StatusCode = MediusCallbackStatus.MediusSuccess
                                });
                            }
                            else
                            {
                                data.ClientObject.Queue(new MediusAccountUpdatePasswordStatusResponse()
                                {
                                    MessageID = accountUpdatePasswordRequest.MessageID,
                                    StatusCode = MediusCallbackStatus.MediusDBError
                                });
                            }
                        });
                        break;
                    }

                case MediusAccountLogoutRequest accountLogoutRequest:
                    {
                        // ERROR - Need a session
                        if (data.ClientObject == null)
                        {
LoggerAccessor.LogError($"[MAS] - INVALID OPERATION: {clientChannel} sent {accountLogoutRequest} without a session.");
                            break;
                        }

                        MediusCallbackStatus result = MediusCallbackStatus.MediusFail;

                        // Check token
                        if (data.ClientObject.IsLoggedIn && accountLogoutRequest.SessionKey == data.ClientObject.SessionKey)
                        {
                            result = MediusCallbackStatus.MediusSuccess;

                            // Logout
                            await data.ClientObject.Logout();
                        }

                        data.ClientObject.Queue(new MediusAccountLogoutResponse()
                        {
                            MessageID = accountLogoutRequest.MessageID,
                            StatusCode = result
                        });
                        break;
                    }

                case MediusAccountUpdateStatsRequest accountUpdateStatsRequest:
                    {
                        // ERROR - Need a session
                        if (data.ClientObject == null)
                        {
LoggerAccessor.LogError($"[MAS] - INVALID OPERATION: {clientChannel} sent {accountUpdateStatsRequest} without a session.");
                            break;
                        }

                        // ERROR -- Need to be logged in
                        if (!data.ClientObject.IsLoggedIn)
                        {
LoggerAccessor.LogError($"[MAS] - INVALID OPERATION: {clientChannel} sent {accountUpdateStatsRequest} without being logged in.");
                            break;
                        }

                        await HorizonServerConfiguration.Database.PostMediusStats(data.ClientObject.AccountId, Convert.ToBase64String(accountUpdateStatsRequest.Stats)).ContinueWith((r) =>
                        {
                            if (data == null || data.ClientObject == null || !data.ClientObject.IsConnected)
                                return;

                            if (r.IsCompletedSuccessfully && r.Result)
                            {
                                data.ClientObject.Queue(new MediusAccountUpdateStatsResponse()
                                {
                                    MessageID = accountUpdateStatsRequest.MessageID,
                                    StatusCode = MediusCallbackStatus.MediusSuccess
                                });
                            }
                            else
                            {
                                data.ClientObject.Queue(new MediusAccountUpdateStatsResponse()
                                {
                                    MessageID = accountUpdateStatsRequest.MessageID,
                                    StatusCode = MediusCallbackStatus.MediusDBError
                                });
                            }
                        });
                        break;
                    }

                case MediusTicketLoginRequest ticketLoginRequest:
                    {
                        // ERROR - Need a session and XI5 Ticket
                        if (data.ClientObject == null || ticketLoginRequest.TicketData == null)
                        {
LoggerAccessor.LogError($"[MAS] - INVALID OPERATION: {clientChannel} sent {ticketLoginRequest} without a session or XI5 Ticket.");
                            break;
                        }

                        // get ticket
                        XI5Ticket ticket = XI5Ticket.ReadFromBytes(ByteUtils.CombineByteArrays(BitConverter.GetBytes(
                            BitConverter.IsLittleEndian ? EndianUtils.ReverseUint(ticketLoginRequest.Version) : ticketLoginRequest.Version), BitConverter.GetBytes(
                            BitConverter.IsLittleEndian ? EndianUtils.ReverseUint(ticketLoginRequest.Size) : ticketLoginRequest.Size), ticketLoginRequest.TicketData));

                        // setup username
                        string username = ticket.Username;

                        // invalid ticket
                        if (!ticket.Valid)
                        {
                            // log to console
                            LoggerAccessor.LogWarn($"[MAS] - MediusTicketLoginRequest: User {username} tried to alter their ticket data");

                            data.ClientObject.Queue(new MediusTicketLoginResponse()
                            {
                                MessageID = ticketLoginRequest.MessageID,
                                StatusCodeTicketLogin = MediusCallbackStatus.MediusDBError
                            });

                            break;
                        }

                        // RPCN
                        if (ticket.IsSignedByRPCN)
                        {
                            LoggerAccessor.LogInfo($"[MAS] - MediusTicketLoginRequest : User {username} connected at: {DateTime.Now} and is on RPCN");

                            data.ClientObject.IsOnRPCN = true;
                            username += $"@{XI5Ticket.RPCNSigner}";
                        }
                        else if (username.EndsWith($"@{XI5Ticket.RPCNSigner}"))
                        {
                            LoggerAccessor.LogError($"[MAS] - MediusTicketLoginRequest : User {username} was caught using a RPCN suffix while not on it!");

                            await HorizonServerConfiguration.Database.BanIp(data.ClientObject.IP).ConfigureAwait(false);

                            // Banned
                            await QueueBanMessage(data).ConfigureAwait(false);

                            // Account is banned
                            data.ClientObject.Queue(new MediusTicketLoginResponse()
                            {
                                MessageID = ticketLoginRequest.MessageID,
                                StatusCodeTicketLogin = MediusCallbackStatus.MediusMachineBanned
                            });

                            break;
                        }
                        else
                            LoggerAccessor.LogInfo($"[MAS] - MediusTicketLoginRequest : User {username} connected at: {DateTime.Now} and is on PSN");

                        // get existing account
                        ClientObject? existingClient = MediusClass.Manager.GetClientByAccountName(username, data.ClientObject.ApplicationId);

                        // account already logged in
                        if (existingClient != null && existingClient.IsLoggedIn)
                        {
                            data.ClientObject.Queue(new MediusTicketLoginResponse()
                            {
                                MessageID = ticketLoginRequest.MessageID,
                                StatusCodeTicketLogin = MediusCallbackStatus.MediusAccountLoggedIn
                            });

                            break;
                        }

                        // get client ip
                        string clientIp = Regex.Match(clientChannel.RemoteAddress.ToString(), @"\[(?:.*?:)?(?<ip>\d+\.\d+\.\d+\.\d+)\]").Groups["ip"].Value;

                        // ip is banned
                        if (await HorizonServerConfiguration.Database.GetIsIpBanned(IPAddress.Parse(clientIp)) || (MultiServerLibraryConfiguration.VpnCheck != null && MultiServerLibraryConfiguration.VpnCheck.IsVpnOrProxy(clientIp)))
                        {
                            LoggerAccessor.LogWarn($"[MAS] - MediusTicketLoginRequest: User {username} tried to login with banned IP {clientIp}");

                            data?.ClientObject?.Queue(new MediusTicketLoginResponse()
                            {
                                MessageID = ticketLoginRequest.MessageID,
                                StatusCodeTicketLogin = MediusCallbackStatus.MediusAccountBanned
                            });

                            // Then queue send ban message
                            await QueueBanMessage(data, "Your IP has been banned");

                            break;
                        }

                        // cid is banned
                        else if (await HorizonServerConfiguration.Database.GetIsMacBanned(data.MachineId ?? string.Empty))
                        {
                            LoggerAccessor.LogWarn($"[MAS] - MediusTicketLoginRequest: User {username} tried to login with banned CID {data.MachineId}");

                            data?.ClientObject?.Queue(new MediusTicketLoginResponse()
                            {
                                MessageID = ticketLoginRequest.MessageID,
                                StatusCodeTicketLogin = MediusCallbackStatus.MediusAccountBanned
                            });

                            // Then queue send ban message
                            await QueueBanMessage(data, "Your CID has been banned");

                            break;
                        }

                        // get account
                        AccountDTO? account = await HorizonServerConfiguration.Database.GetAccountByName(username, data.ClientObject.ApplicationId, true);

                        // account doesn't exist
                        if (account == null)
                        {
                            // account creation disabled
                            if (appSettings.DisableAccountCreation)
                            {
                                LoggerAccessor.LogError($"[MAS] - MediusTicketLoginRequest: AppId {data?.ClientObject?.ApplicationId} has account creation disabled");

                                // Reply error
                                data?.ClientObject?.Queue(new MediusTicketLoginResponse()
                                {
                                    MessageID = ticketLoginRequest.MessageID,
                                    StatusCodeTicketLogin = MediusCallbackStatus.MediusFail,
                                });

                                break;
                            }

                            // create new account
                            account = await HorizonServerConfiguration.Database.CreateAccount(new CreateAccountDTO()
                            {
                                AccountName = username,
                                AccountPassword = "UNSET",
                                MachineId = data.MachineId,
                                MediusStats = Convert.ToBase64String(new byte[Constants.ACCOUNTSTATS_MAXLEN]),
                                AppId = data.ClientObject.ApplicationId
                            }, clientChannel);

                            // error creating account
                            if (account == null)
                            {
                                data.ClientObject.Queue(new MediusTicketLoginResponse()
                                {
                                    MessageID = ticketLoginRequest.MessageID,
                                    StatusCodeTicketLogin = MediusCallbackStatus.MediusDBError
                                });

                                break;
                            }
                        }

                        // account not whitelisted
                        else if (appSettings.EnableAccountWhitelist &&
                            !appSettings.AccountIdWhitelist.Contains(account.AccountId))
                        {
                            LoggerAccessor.LogError($"[MAS] - MediusTicketLoginRequest: {username} tried to login without being whitelisted");

                            data?.ClientObject?.Queue(new MediusTicketLoginResponse()
                            {
                                MessageID = ticketLoginRequest.MessageID,
                                StatusCodeTicketLogin = MediusCallbackStatus.MediusFail
                            });

                            break;
                        }

                        // account is banned
                        if (account.IsBanned)
                        {
                            LoggerAccessor.LogWarn($"[MAS] - MediusTicketLoginRequest: User {username} tried to login with banned account");

                            data?.ClientObject?.Queue(new MediusTicketLoginResponse()
                            {
                                MessageID = ticketLoginRequest.MessageID,
                                StatusCodeTicketLogin = MediusCallbackStatus.MediusAccountBanned
                            });

                            // Then queue send ban message
                            await QueueBanMessage(data, "Your account has been banned");

                            break;
                        }

                        // get anti poke ban records (PS Home only right now)
                        bool isAntiPoke = !MediusClass.Settings.PlaystationHomeAllowAnyEboot &&
                            (data.ClientObject.ApplicationId == 20371 || data.ClientObject.ApplicationId == 20374) &&
                            data.ClientObject.ClientHomeData == null;

                        // account disabled poking in the eboot
                        if (isAntiPoke)
                        {
                            LoggerAccessor.LogWarn($"[MAS] - MediusTicketLoginRequest: User {username} tried to login while using an anti Poke/Query eboot");

                            data?.ClientObject?.Queue(new MediusTicketLoginResponse()
                            {
                                MessageID = ticketLoginRequest.MessageID,
                                StatusCodeTicketLogin = MediusCallbackStatus.MediusAccountBanned
                            });

                            // Then queue send ban message
                            await QueueBanMessage(data, "You was caught using an anti Poke/Query eboot");

                            break;
                        }

                        // login user
                        await Login(ticketLoginRequest.MessageID, clientChannel, data, account, true);
                        break;
                    }

                #endregion

                #region Policy / Announcements

                case MediusGetAllAnnouncementsRequest getAllAnnouncementsRequest:
                    {
                        // Send to plugins
                        await MediusClass.Plugins.OnEvent(PluginEvent.MEDIUS_PLAYER_ON_GET_ALL_ANNOUNCEMENTS, new OnPlayerRequestArgs()
                        {
                            Player = data.ClientObject,
                            Request = getAllAnnouncementsRequest
                        });

                        await HorizonServerConfiguration.Database.GetLatestAnnouncementsList(data.ApplicationId).ContinueWith((r) =>
                        {
                            if (data == null || data.ClientObject == null || !data.ClientObject.IsConnected)
                                return;

                            if (r.IsCompletedSuccessfully && r.Result != null && r.Result.Length > 0)
                            {
                                List<MediusGetAnnouncementsResponse> responses = new List<MediusGetAnnouncementsResponse>();
                                foreach (var result in r.Result)
                                {
                                    responses.Add(new MediusGetAnnouncementsResponse()
                                    {
                                        MessageID = getAllAnnouncementsRequest.MessageID,
                                        StatusCode = MediusCallbackStatus.MediusSuccess,
                                        Announcement = string.IsNullOrEmpty(result.AnnouncementTitle) ? $"{result.AnnouncementBody}" : $"{result.AnnouncementTitle}\n{result.AnnouncementBody}\n",
                                        AnnouncementID = result.Id++,
                                        EndOfList = false
                                    });
                                }

                                responses[responses.Count - 1].EndOfList = true;
                                data.ClientObject.Queue(responses);
                            }
                            else
                            {
                                data.ClientObject.Queue(new MediusGetAnnouncementsResponse()
                                {
                                    MessageID = getAllAnnouncementsRequest.MessageID,
                                    StatusCode = MediusCallbackStatus.MediusSuccess,
                                    Announcement = "",
                                    AnnouncementID = 0,
                                    EndOfList = true
                                });
                            }
                        });
                        break;
                    }

                case MediusGetAnnouncementsRequest getAnnouncementsRequest:
                    {
                        // Send to plugins
                        await MediusClass.Plugins.OnEvent(PluginEvent.MEDIUS_PLAYER_ON_GET_ANNOUNCEMENTS, new OnPlayerRequestArgs()
                        {
                            Player = data.ClientObject,
                            Request = getAnnouncementsRequest
                        });

                        await HorizonServerConfiguration.Database.GetLatestAnnouncement(data.ApplicationId).ContinueWith((r) =>
                        {
                            if (data == null || data.ClientObject == null || !data.ClientObject.IsConnected)
                                return;

                            if (r.IsCompletedSuccessfully && r.Result != null)
                            {
                                data.ClientObject.Queue(new MediusGetAnnouncementsResponse()
                                {
                                    MessageID = getAnnouncementsRequest.MessageID,
                                    StatusCode = MediusCallbackStatus.MediusSuccess,
                                    Announcement = string.IsNullOrEmpty(r.Result.AnnouncementTitle) ? $"{r.Result.AnnouncementBody}" : $"{r.Result.AnnouncementTitle}\n{r.Result.AnnouncementBody}\n",
                                    AnnouncementID = r.Result.Id++,
                                    EndOfList = true
                                });
                            }
                            else
                            {
                                data.ClientObject.Queue(new MediusGetAnnouncementsResponse()
                                {
                                    MessageID = getAnnouncementsRequest.MessageID,
                                    StatusCode = MediusCallbackStatus.MediusSuccess,
                                    Announcement = "",
                                    AnnouncementID = 0,
                                    EndOfList = true
                                });
                            }
                        });
                        break;
                    }

                case MediusGetPolicyRequest getPolicyRequest:
                    {
                        // Send to plugins
                        await MediusClass.Plugins.OnEvent(PluginEvent.MEDIUS_PLAYER_ON_GET_POLICY, new OnPlayerRequestArgs()
                        {
                            Player = data.ClientObject,
                            Request = getPolicyRequest
                        });

                        switch (getPolicyRequest.Policy)
                        {
                            case MediusPolicyType.Privacy:
                                {
                                    if (data.ClientObject != null)
                                    {
                                        await HorizonServerConfiguration.Database.GetPolicy((int)MediusPolicyType.Privacy, data.ClientObject.ApplicationId).ContinueWith((r) =>
                                        {
                                            if (data == null || data.ClientObject == null || !data.ClientObject.IsConnected)
                                                return;

                                            if (r.IsCompletedSuccessfully && r.Result != null)
                                            {
                                                string? txt = r.Result.EulaBody;
                                                if (!string.IsNullOrEmpty(r.Result.EulaTitle))
                                                    txt = r.Result.EulaTitle + "\n" + txt;
                                                else
                                                    txt = string.Empty;
LoggerAccessor.LogDebug($"[MAS] - GetPolicy Succeeded:{getPolicyRequest.MessageID}");
                                                data.ClientObject.Queue(MediusClass.GetPolicyFromText(getPolicyRequest.MessageID, txt));
                                            }
                                            else if (r.IsCompletedSuccessfully && r.Result == null)
                                            {
                                                LoggerAccessor.LogDebug($"Sending blank Policy since no chunks were found");
                                                data.ClientObject.Queue(new MediusGetPolicyResponse() { MessageID = getPolicyRequest.MessageID, StatusCode = MediusCallbackStatus.MediusSuccess, Policy = string.Empty, EndOfText = true });
                                            }
                                            else
                                            {
LoggerAccessor.LogError($"[MAS] - GetPolicy Failed = [{r.Exception}]");
                                                data.ClientObject.Queue(new MediusGetPolicyResponse() { MessageID = getPolicyRequest.MessageID, StatusCode = MediusCallbackStatus.MediusSuccess, Policy = "NONE", EndOfText = true });
                                            }
                                        });
                                    }
                                    break;
                                }
                            case MediusPolicyType.Usage:
                                {
                                    if (data.ClientObject != null)
                                    {
                                        await HorizonServerConfiguration.Database.GetPolicy((int)MediusPolicyType.Usage, data.ClientObject.ApplicationId).ContinueWith((r) =>
                                        {
                                            if (data == null || data.ClientObject == null || !data.ClientObject.IsConnected)
                                                return;

                                            if (r.IsCompletedSuccessfully && r.Result != null)
                                            {
                                                string? txt = r.Result.EulaBody;
                                                if (!string.IsNullOrEmpty(r.Result.EulaTitle))
                                                    txt = r.Result.EulaTitle + "\n" + txt;
                                                else
                                                    txt = string.Empty;
LoggerAccessor.LogDebug($"[MAS] - GetPolicy Succeeded:{getPolicyRequest.MessageID}");
                                                data.ClientObject.Queue(MediusClass.GetPolicyFromText(getPolicyRequest.MessageID, txt));
                                            }
                                            else if (r.IsCompletedSuccessfully && r.Result == null)
                                            {
                                                LoggerAccessor.LogDebug($"Sending blank Policy since no chunks were found");
                                                data.ClientObject.Queue(new MediusGetPolicyResponse() { MessageID = getPolicyRequest.MessageID, StatusCode = MediusCallbackStatus.MediusSuccess, Policy = string.Empty, EndOfText = true });
                                            }
                                            else
                                            {
LoggerAccessor.LogError($"[MAS] - GetPolicy Failed = [{r.Exception}]");
                                                data.ClientObject.Queue(new MediusGetPolicyResponse() { MessageID = getPolicyRequest.MessageID, StatusCode = MediusCallbackStatus.MediusSuccess, Policy = "NONE", EndOfText = true });
                                            }
                                        });
                                    }
                                    break;
                                }
                        }
                        break;
                    }

                #endregion

                #region Ladders

                case MediusGetLadderStatsRequest getLadderStatsRequest:
                    {
                        // ERROR - Need a session
                        if (data.ClientObject == null)
                        {
LoggerAccessor.LogError($"[MAS] - INVALID OPERATION: {clientChannel} sent {getLadderStatsRequest} without a session.");
                            break;
                        }

                        // ERROR -- Need to be logged in
                        if (!data.ClientObject.IsLoggedIn)
                        {
LoggerAccessor.LogError($"[MAS] - INVALID OPERATION: {clientChannel} sent {getLadderStatsRequest} without being logged in.");
                            break;
                        }

                        switch (getLadderStatsRequest.LadderType)
                        {
                            case MediusLadderType.MediusLadderTypePlayer:
                                {
                                    await HorizonServerConfiguration.Database.GetAccountById(getLadderStatsRequest.AccountID_or_ClanID).ContinueWith((r) =>
                                    {
                                        if (data == null || data.ClientObject == null || !data.ClientObject.IsConnected)
                                            return;

                                        if (r.IsCompletedSuccessfully && r.Result != null && r.Result.AccountStats != null)
                                        {
                                            data.ClientObject.Queue(new MediusGetLadderStatsResponse()
                                            {
                                                MessageID = getLadderStatsRequest.MessageID,
                                                StatusCode = MediusCallbackStatus.MediusSuccess,
                                                Stats = Array.ConvertAll(r.Result.AccountStats, Convert.ToInt32)
                                            });
                                        }
                                        else
                                        {
                                            data.ClientObject.Queue(new MediusGetLadderStatsResponse()
                                            {
                                                MessageID = getLadderStatsRequest.MessageID,
                                                StatusCode = MediusCallbackStatus.MediusDBError
                                            });
                                        }
                                    });
                                    break;
                                }
                            case MediusLadderType.MediusLadderTypeClan:
                                {
                                    await HorizonServerConfiguration.Database.GetClanById(getLadderStatsRequest.AccountID_or_ClanID,
                                        data.ClientObject.ApplicationId)
                                    .ContinueWith((r) =>
                                    {
                                        if (data == null || data.ClientObject == null || !data.ClientObject.IsConnected)
                                            return;

                                        if (r.IsCompletedSuccessfully && r.Result != null && r.Result.ClanStats != null)
                                        {
                                            data.ClientObject.Queue(new MediusGetLadderStatsResponse()
                                            {
                                                MessageID = getLadderStatsRequest.MessageID,
                                                StatusCode = MediusCallbackStatus.MediusSuccess,
                                                Stats = r.Result.ClanStats
                                            });
                                        }
                                        else
                                        {
                                            data.ClientObject.Queue(new MediusGetLadderStatsResponse()
                                            {
                                                MessageID = getLadderStatsRequest.MessageID,
                                                StatusCode = MediusCallbackStatus.MediusDBError
                                            });
                                        }
                                    });
                                    break;
                                }
                            default:
                                {
LoggerAccessor.LogWarn($"[MAS] - Unhandled MediusGetLadderStatsRequest {getLadderStatsRequest}");
                                    break;
                                }
                        }
                        break;
                    }

                case MediusGetLadderStatsWideRequest getLadderStatsWideRequest:
                    {
                        // ERROR - Need a session
                        if (data.ClientObject == null)
                        {
LoggerAccessor.LogError($"[MAS] - INVALID OPERATION: {clientChannel} sent {getLadderStatsWideRequest} without a session.");
                            break;
                        }

                        // ERROR -- Need to be logged in
                        if (!data.ClientObject.IsLoggedIn)
                        {
LoggerAccessor.LogError($"[MAS] - INVALID OPERATION: {clientChannel} sent {getLadderStatsWideRequest} without being logged in.");
                            break;
                        }

                        switch (getLadderStatsWideRequest.LadderType)
                        {
                            case MediusLadderType.MediusLadderTypePlayer:
                                {
                                    await HorizonServerConfiguration.Database.GetAccountById(getLadderStatsWideRequest.AccountID_or_ClanID).ContinueWith((r) =>
                                    {
                                        if (data == null || data.ClientObject == null || !data.ClientObject.IsConnected)
                                            return;

                                        if (r.IsCompletedSuccessfully && r.Result != null && r.Result.AccountWideStats != null)
                                        {
                                            data.ClientObject.Queue(new MediusGetLadderStatsWideResponse()
                                            {
                                                MessageID = getLadderStatsWideRequest.MessageID,
                                                StatusCode = MediusCallbackStatus.MediusSuccess,
                                                AccountID_or_ClanID = r.Result.AccountId,
                                                Stats = r.Result.AccountWideStats
                                            });
                                        }
                                        else
                                        {
                                            data.ClientObject.Queue(new MediusGetLadderStatsWideResponse()
                                            {
                                                MessageID = getLadderStatsWideRequest.MessageID,
                                                StatusCode = MediusCallbackStatus.MediusDBError
                                            });
                                        }
                                    });
                                    break;
                                }
                            case MediusLadderType.MediusLadderTypeClan:
                                {
                                    await HorizonServerConfiguration.Database.GetClanById(getLadderStatsWideRequest.AccountID_or_ClanID,
                                        data.ClientObject.ApplicationId)
                                    .ContinueWith((r) =>
                                    {
                                        if (data == null || data.ClientObject == null || !data.ClientObject.IsConnected)
                                            return;

                                        if (r.IsCompletedSuccessfully && r.Result != null && r.Result.ClanWideStats != null)
                                        {
                                            data.ClientObject.Queue(new MediusGetLadderStatsWideResponse()
                                            {
                                                MessageID = getLadderStatsWideRequest.MessageID,
                                                StatusCode = MediusCallbackStatus.MediusSuccess,
                                                AccountID_or_ClanID = r.Result.ClanId,
                                                Stats = r.Result.ClanWideStats
                                            });
                                        }
                                        else
                                        {
                                            data.ClientObject.Queue(new MediusGetLadderStatsWideResponse()
                                            {
                                                MessageID = getLadderStatsWideRequest.MessageID,
                                                StatusCode = MediusCallbackStatus.MediusDBError
                                            });
                                        }
                                    });
                                    break;
                                }
                            default:
                                {
LoggerAccessor.LogWarn($"[MAS] - Unhandled MediusGetLadderStatsWideRequest {getLadderStatsWideRequest}");
                                    break;
                                }
                        }
                        break;
                    }

                #endregion

                #region Channels

                case MediusChannelListRequest channelListRequest:
                    {
                        // ERROR - Need a session
                        if (data.ClientObject == null)
                        {
LoggerAccessor.LogError($"[MAS] - INVALID OPERATION: {clientChannel} sent {channelListRequest} without a session.");
                            break;
                        }

                        // ERROR -- Need to be logged in
                        if (!data.ClientObject.IsLoggedIn)
                        {
LoggerAccessor.LogError($"[MAS] - INVALID OPERATION: {clientChannel} sent {channelListRequest} without being logged in.");
                            break;
                        }

                        List<MediusChannelListResponse> channelResponses = new List<MediusChannelListResponse>();

                        var lobbyChannels = MediusClass.Manager.GetChannelList(
                            data.ClientObject.ApplicationId,
                            channelListRequest.PageID,
                            channelListRequest.PageSize,
                            ChannelType.Lobby
                        );

                        foreach (var channel in lobbyChannels)
                        {
                            channelResponses.Add(new MediusChannelListResponse()
                            {
                                MessageID = channelListRequest.MessageID,
                                StatusCode = MediusCallbackStatus.MediusSuccess,
                                MediusWorldID = channel.Id,
                                LobbyName = channel.Name,
                                PlayerCount = channel.PlayerCount,
                                EndOfList = false
                            });
                        }

                        if (channelResponses.Count == 0)
                        {
                            // Return none
                            data.ClientObject.Queue(new MediusChannelListResponse()
                            {
                                MessageID = channelListRequest.MessageID,
                                StatusCode = MediusCallbackStatus.MediusNoResult,
                                EndOfList = true
                            });
                        }
                        else
                        {
                            // Ensure the end of list flag is set
                            channelResponses[channelResponses.Count - 1].EndOfList = true;

                            // Add to responses
                            data.ClientObject.Queue(channelResponses);
                        }


                        break;
                    }

                #endregion

                #region Deadlocked No-op Messages (MAS)

                case MediusGetBuddyList_ExtraInfoRequest getBuddyList_ExtraInfoRequest:
                    {
                        Queue(new RT_MSG_SERVER_APP()
                        {
                            Message = new MediusGetBuddyList_ExtraInfoResponse()
                            {
                                MessageID = getBuddyList_ExtraInfoRequest.MessageID,
                                StatusCode = MediusCallbackStatus.MediusNoResult,
                                EndOfList = true
                            }
                        }, clientChannel);
                        break;
                    }

                case MediusGetIgnoreListRequest getIgnoreListRequest:
                    {
                        Queue(new RT_MSG_SERVER_APP()
                        {
                            Message = new MediusGetIgnoreListResponse()
                            {
                                MessageID = getIgnoreListRequest.MessageID,
                                StatusCode = MediusCallbackStatus.MediusNoResult,
                                EndOfList = true
                            }
                        }, clientChannel);
                        break;
                    }

                case MediusGetMyClansRequest getMyClansRequest:
                    {
                        Queue(new RT_MSG_SERVER_APP()
                        {
                            Message = new MediusGetMyClansResponse()
                            {
                                MessageID = getMyClansRequest.MessageID,
                                StatusCode = MediusCallbackStatus.MediusNoResult,
                                EndOfList = true
                            }
                        }, clientChannel);
                        break;
                    }

                #endregion

                #region TextFilter

                case MediusTextFilterRequest textFilterRequest:
                    {
                        if (data.ClientObject == null)
                        {
LoggerAccessor.LogError($"[MAS] - INVALID OPERATION: {clientChannel} sent {textFilterRequest} without a session.");
                            break;
                        }

                        // Deny special characters
                        // Also trim any whitespace
                        switch (textFilterRequest.TextFilterType)
                        {
                            case MediusTextFilterType.MediusTextFilterPassFail:
                                {
                                    // validate name
                                    if (!MediusClass.PassTextFilter(data.ApplicationId, Config.TextFilterContext.ACCOUNT_NAME, textFilterRequest.Text))
                                    {
                                        // Failed due to special characters
                                        data.ClientObject.Queue(new MediusTextFilterResponse()
                                        {
                                            MessageID = textFilterRequest.MessageID,
                                            StatusCode = MediusCallbackStatus.MediusFail
                                        });
                                        return;
                                    }
                                    else
                                    {
                                        data.ClientObject.Queue(new MediusTextFilterResponse()
                                        {
                                            MessageID = textFilterRequest.MessageID,
                                            StatusCode = MediusCallbackStatus.MediusPass,
                                            Text = textFilterRequest.Text.Trim()
                                        });
                                    }
                                    break;
                                }
                            case MediusTextFilterType.MediusTextFilterReplace:
                                {
                                    data.ClientObject.Queue(new MediusTextFilterResponse()
                                    {
                                        MessageID = textFilterRequest.MessageID,
                                        StatusCode = MediusCallbackStatus.MediusPass,
                                        Text = MediusClass.FilterTextFilter(data.ApplicationId, TextFilterContext.ACCOUNT_NAME, textFilterRequest.Text).Trim()
                                    });
                                    break;
                                }
                        }
                        break;
                    }

                #endregion

                #region Time
                case MediusGetServerTimeRequest getServerTimeRequest:
                    {
                        // ERROR - Need a session
                        if (data.ClientObject == null)
                        {
LoggerAccessor.LogError($"[MAS] - INVALID OPERATION: {clientChannel} sent {getServerTimeRequest} without a session.");
                            break;
                        }

                        //Doesn't need to be logged in to get ServerTime

                        var time = DateTime.Now;

                        await GetTimeZone(time).ContinueWith((r) =>
                        {
                            if (r.IsCompletedSuccessfully)
                            {
                                //Fetched
                                data.ClientObject.Queue(new MediusGetServerTimeResponse()
                                {
                                    MessageID = getServerTimeRequest.MessageID,
                                    StatusCode = MediusCallbackStatus.MediusSuccess,
                                    Local_server_timezone = r.Result,
                                });
                            }
                            else
                            {
                                //default
                                data.ClientObject.Queue(new MediusGetServerTimeResponse()
                                {
                                    MessageID = getServerTimeRequest.MessageID,
                                    StatusCode = MediusCallbackStatus.MediusSuccess,
                                    Local_server_timezone = MediusTimeZone.MediusTimeZone_GMT,
                                });
                            }
                        });
                        break;
                    }
                #endregion

                #region GetMyIP
                //Syphon Filter - The Omega Strain Beta

                case MediusGetMyIPRequest getMyIpRequest:
                    {
                        // ERROR - Need a session
                        if (data.ClientObject == null)
                        {
LoggerAccessor.LogError($"[MAS] - INVALID OPERATION: {clientChannel} sent {getMyIpRequest} without a session.");
                            break;
                        }

                        IPAddress? ClientIP = (clientChannel.RemoteAddress as IPEndPoint)?.Address;

                        if (ClientIP == null)
                        {
LoggerAccessor.LogInfo($"[MAS] - Error: Retrieving Client IP address {clientChannel.RemoteAddress} = [{ClientIP}]");
                            data.ClientObject.Queue(new MediusGetMyIPResponse()
                            {
                                MessageID = getMyIpRequest.MessageID,
                                IP = null,
                                StatusCode = MediusCallbackStatus.MediusDMEError
                            });
                        }
                        else
                        {
                            data.ClientObject.Queue(new MediusGetMyIPResponse()
                            {
                                MessageID = getMyIpRequest.MessageID,
                                IP = ClientIP,
                                StatusCode = MediusCallbackStatus.MediusSuccess
                            });
                        }

                        break;
                    }

                #endregion

                #region UpdateUserState
                case MediusUpdateUserState updateUserState:
                    {
                        // ERROR - Need a session
                        if (data.ClientObject == null)
                        {
LoggerAccessor.LogError($"[MAS] - INVALID OPERATION: {clientChannel} sent {updateUserState} without a session.");
                            break;
                        }

                        // ERROR - Needs to be logged in --Doesn't need to be logged in on older clients

                        switch (updateUserState.UserAction)
                        {
                            case MediusUserAction.KeepAlive:
                                {
                                    data.ClientObject.KeepAliveUntilNextConnection();
                                    break;
                                }
                            case MediusUserAction.JoinedChatWorld:
                                {
                                    LoggerAccessor.LogInfo($"[MAS] - Successfully Joined ChatWorld [{data.ClientObject.CurrentChannel?.Id}] {data.ClientObject.AccountId}:{data.ClientObject.AccountName}");
                                    break;
                                }
                            case MediusUserAction.LeftGameWorld:
                                {
                                    LoggerAccessor.LogInfo($"[MAS] - Successfully Left GameWorld {data.ClientObject.AccountId}:{data.ClientObject.AccountName}");
                                    MediusClass.AntiCheatPlugin.mc_anticheat_event_msg_UPDATEUSERSTATE(AnticheatEventCode.anticheatLEAVEGAME, data.ClientObject.MediusWorldID, data.ClientObject.AccountId, MediusClass.AntiCheatClient, updateUserState, 256);
                                    break;
                                }
                            case MediusUserAction.LeftPartyWorld:
                                {
                                    LoggerAccessor.LogInfo($"[MAS] - Successfully Left PartyWorld {data.ClientObject.AccountId}:{data.ClientObject.AccountName}");
                                    break;
                                }
                            default:
                                {
                                    LoggerAccessor.LogWarn($"[MAS] - Requested a non-existant UserState {data.ClientObject.AccountId}:{data.ClientObject.AccountName}, please report to GITHUB.");
                                    break;
                                }
                        }

                        break;
                    }

                #endregion

                default:
                    {
LoggerAccessor.LogWarn($"[MAS] - Unhandled Medius Message: {message}");
                        break;
                    }
            }
        }

        #region Login
        private async Task Login(MessageId messageId, IChannel clientChannel, ChannelData data, AccountDTO accountDto, bool ticket)
        {
            var fac = new PS2CipherFactory();
            var rsa = fac.CreateNew(CipherContext.RSA_AUTH) as PS2_RSA;
            IPHostEntry host = Dns.GetHostEntry(MediusClass.Settings.NATIp ?? "natservice.pdonline.scea.com");

            List<int> pre108Secure = new() { 10010, 10031, 10190, 10124, 10680, 10681, 10683, 10684 };

            if (data.ClientObject!.IP == IPAddress.Any)
                data.ClientObject!.SetIp(((IPEndPoint)clientChannel.RemoteAddress).Address.ToString().Trim(new char[] { ':', 'f', '{', '}' }));

            if ((data.ApplicationId == 20371 || data.ApplicationId == 20374) && data.ClientObject.ClientHomeData != null)
            {
                if (data.ClientObject.IsOnRPCN && data.ClientObject.ClientHomeData.VersionAsDouble >= 01.82)
                {
                    if (!string.IsNullOrEmpty(data.ClientObject.ClientHomeData.Type) && (data.ClientObject.ClientHomeData.Type.Contains("HDK") || data.ClientObject.ClientHomeData.Type.Contains("Online Debug")))
                        _ = Task.Delay(4000).ContinueWith(r => HomeRTMTools.SendRemoteCommand(data.ClientObject, "lc Debug.System( 'mlaaenable 0' )"));
                    else
                        _ = Task.Delay(4000).ContinueWith(r => HomeRTMTools.SendRemoteCommand(data.ClientObject, "mlaaenable 0"));
                }

                switch (data.ClientObject.ClientHomeData.Type)
                {
                    case "HDK With Offline":
                        switch (data.ClientObject.ClientHomeData.Version)
                        {
                            case "01.86.09":
                                if (MediusClass.Settings.PokePatchOn)
                                {
                                    CheatQuery(0x005478dc, 4, clientChannel);

                                    CheatQuery(0x0016cc6c, 4, clientChannel);
                                }

                                CheatQuery(0x1054e5c0, 4, clientChannel);
                                break;
                            default:
                                break;
                        }
                        break;
                    case "HDK Online Only":
                        switch (data.ClientObject.ClientHomeData.Version)
                        {
                            default:
                                break;
                        }
                        break;
                    case "HDK Online Only (Dbg Symbols)":
                        switch (data.ClientObject.ClientHomeData.Version)
                        {
                            case "01.82.09":
                                if (MediusClass.Settings.PokePatchOn)
                                {
                                    CheatQuery(0x00531370, 4, clientChannel);

                                    CheatQuery(0x0016b4d0, 4, clientChannel);
                                }

                                CheatQuery(0x1053e160, 4, clientChannel);
                                CheatQuery(0x0001191B, 1, clientChannel);
                                break;
                            default:
                                break;
                        }
                        break;
                    case "Online Debug":
                    case "Online Debug (QA)":
                        switch (data.ClientObject.ClientHomeData.Version)
                        {
                            case "01.83.12":
                                if (MediusClass.Settings.PokePatchOn)
                                {
                                    CheatQuery(0x00548bc0, 4, clientChannel);

                                    CheatQuery(0x001709e0, 4, clientChannel);
                                }

                                CheatQuery(0x1054e1c0, 4, clientChannel);
                                CheatQuery(0x0001193F, 1, clientChannel);
                                break;
                            case "01.86.09":
                                if (MediusClass.Settings.PokePatchOn)
                                {
                                    CheatQuery(0x00555cb4, 4, clientChannel);

                                    CheatQuery(0x0016dac0, 4, clientChannel);
                                }

                                CheatQuery(0x1054e358, 4, clientChannel);
                                CheatQuery(0x0001193F, 1, clientChannel);
                                break;
                            default:
                                break;
                        }
                        break;
                    case "Retail":
                        switch (data.ClientObject.ClientHomeData.Version)
                        {
                            case "01.86.09":
                                if (MediusClass.Settings.PokePatchOn)
                                {
                                    CheatQuery(0x006f59b8, 4, clientChannel);
                                    CheatQuery(0x002aa960, 4, clientChannel);
                                }

                                CheatQuery(0x105c24c8, 4, clientChannel);
                                CheatQuery(0x0017A8DF, 1, clientChannel);
                                break;
                            default:
                                break;
                        }
                        break;
                }
            }

            await data.ClientObject.Login(accountDto, false);

            #region Update DB IP and CID
            await HorizonServerConfiguration.Database.PostAccountIp(accountDto.AccountId, ((IPEndPoint)clientChannel.RemoteAddress).Address.MapToIPv4().ToString());

            CIDManager.CreateCIDPair(data.ClientObject.AccountName, data.MachineId);

            if (!string.IsNullOrEmpty(data.MachineId))
                await HorizonServerConfiguration.Database.PostMachineId(data.ClientObject.AccountId, data.MachineId);
            #endregion

            // Add to logged in clients
            MediusClass.Manager.AddOrUpdateLoggedInClient(data.ClientObject);

LoggerAccessor.LogInfo($"[MAS] - LOGGING IN AS {data.ClientObject.AccountName} with access token {data.ClientObject.AccessToken}");

            // Tell client
            if (ticket)
            {
                #region IF PS3 Client
                data.ClientObject.Queue(new MediusTicketLoginResponse()
                {
                    //TicketLoginResponse
                    MessageID = messageId,
                    StatusCodeTicketLogin = MediusCallbackStatus.MediusSuccess,
                    PasswordType = MediusPasswordType.MediusPasswordNotSet,

                    //AccountLoginResponse Wrapped
                    MessageID2 = messageId,
                    StatusCodeAccountLogin = MediusCallbackStatus.MediusSuccess,
                    AccountID = data.ClientObject.AccountId,
                    AccountType = MediusAccountType.MediusMasterAccount,
                    ConnectInfo = new NetConnectionInfo()
                    {
                        AccessKey = data.ClientObject.AccessToken,
                        SessionKey = data.ClientObject.SessionKey,
                        TargetWorldID = data.ClientObject.CurrentChannel!.Id,
                        ServerKey = new RSA_KEY(), //MediusStarter.GlobalAuthPublic,
                        AddressList = new NetAddressList()
                        {
                            AddressList = new NetAddress[Constants.NET_ADDRESS_LIST_COUNT]
                            {
                                    new NetAddress() {Address = !string.IsNullOrEmpty(MediusClass.Settings.NpMLSIpOverride) ? MediusClass.Settings.NpMLSIpOverride : MediusClass.LobbyServer.IPAddress.ToString(), Port = (MediusClass.Settings.NpMLSPortOverride != -1) ? MediusClass.Settings.NpMLSPortOverride : MediusClass.LobbyServer.TCPPort , AddressType = NetAddressType.NetAddressTypeExternal},
                                    new NetAddress() {Address = host.AddressList.First().ToString(), Port = MediusClass.Settings.NATPort, AddressType = NetAddressType.NetAddressTypeNATService},
                            }
                        },
                        Type = NetConnectionType.NetConnectionTypeClientServerTCP
                    },
                });
                #endregion

                // Prepare for transition to lobby server
                data.ClientObject.KeepAliveUntilNextConnection();
            }
            else
            {
                #region If PS2/PSP

                if (data.ClientObject.MediusVersion > 108)
                {
                    data.ClientObject.Queue(new MediusAccountLoginResponse()
                    {
                        MessageID = messageId,
                        StatusCode = MediusCallbackStatus.MediusSuccess,
                        AccountID = data.ClientObject.AccountId,
                        AccountType = MediusAccountType.MediusMasterAccount,
                        WorldID = data.ClientObject.CurrentChannel!.Id,
                        ConnectInfo = new NetConnectionInfo()
                        {
                            AccessKey = data.ClientObject.AccessToken,
                            SessionKey = data.ClientObject.SessionKey,
                            TargetWorldID = data.ClientObject.CurrentChannel!.Id,
                            ServerKey = MediusClass.GlobalAuthPublic,
                            AddressList = new NetAddressList()
                            {
                                AddressList = new NetAddress[Constants.NET_ADDRESS_LIST_COUNT]
                                {
                                    new NetAddress() {Address = MediusClass.LobbyServer.IPAddress.ToString(), Port = MediusClass.LobbyServer.TCPPort, AddressType = NetAddressType.NetAddressTypeExternal},
                                    new NetAddress() {Address = host.AddressList.First().ToString(), Port = MediusClass.Settings.NATPort, AddressType = NetAddressType.NetAddressTypeNATService},
                                }
                            },
                            Type = NetConnectionType.NetConnectionTypeClientServerTCP
                        },
                    });
                }
                else if (pre108Secure.Contains(data.ClientObject.ApplicationId)) //10683 / 10684
                {
                    data.ClientObject.Queue(new MediusAccountLoginResponse()
                    {
                        MessageID = messageId,
                        StatusCode = MediusCallbackStatus.MediusSuccess,
                        AccountID = data.ClientObject.AccountId,
                        AccountType = MediusAccountType.MediusMasterAccount,
                        WorldID = data.ClientObject.CurrentChannel!.Id,
                        ConnectInfo = new NetConnectionInfo()
                        {
                            AccessKey = data.ClientObject.AccessToken,
                            SessionKey = data.ClientObject.SessionKey,
                            TargetWorldID = data.ClientObject.CurrentChannel!.Id,
                            ServerKey = new RSA_KEY(),
                            AddressList = new NetAddressList()
                            {
                                AddressList = new NetAddress[Constants.NET_ADDRESS_LIST_COUNT]
                                {
                                    new NetAddress() {Address = MediusClass.LobbyServer.IPAddress.ToString(), Port = MediusClass.LobbyServer.TCPPort, AddressType = NetAddressType.NetAddressTypeExternal},
                                    new NetAddress() {Address = host.AddressList.First().ToString(), Port = MediusClass.Settings.NATPort, AddressType = NetAddressType.NetAddressTypeNATService},
                                }
                            },
                            Type = NetConnectionType.NetConnectionTypeClientServerTCP
                        },
                    });
                }
                else
                {
                    data.ClientObject.Queue(new MediusAccountLoginResponse()
                    {
                        MessageID = messageId,
                        StatusCode = MediusCallbackStatus.MediusSuccess,
                        AccountID = data.ClientObject.AccountId,
                        AccountType = MediusAccountType.MediusMasterAccount,
                        WorldID = data.ClientObject.CurrentChannel!.Id,
                        ConnectInfo = new NetConnectionInfo()
                        {
                            AccessKey = data.ClientObject.AccessToken,
                            SessionKey = data.ClientObject.SessionKey,
                            TargetWorldID = data.ClientObject.CurrentChannel!.Id,
                            ServerKey = MediusClass.GlobalAuthPublic, //Some Older Medius games don't set a RSA Key
                            AddressList = new NetAddressList()
                            {
                                AddressList = new NetAddress[Constants.NET_ADDRESS_LIST_COUNT]
                                {
                                    new NetAddress() {Address = MediusClass.LobbyServer.IPAddress.ToString(), Port = MediusClass.LobbyServer.TCPPort, AddressType = NetAddressType.NetAddressTypeExternal},
                                    new NetAddress() {Address = host.AddressList.First().ToString(), Port = MediusClass.Settings.NATPort, AddressType = NetAddressType.NetAddressTypeNATService},
                                }
                            },
                            Type = NetConnectionType.NetConnectionTypeClientServerTCP
                        },
                    });
                }

                // Prepare for transition to lobby server
                data.ClientObject.KeepAliveUntilNextConnection();
                #endregion
            }
        }
        #endregion

        #region AnonymousLogin
        private async Task LoginAnonymous(MediusAnonymousLoginRequest anonymousLoginRequest, IChannel clientChannel, ChannelData data)
        {
            IPHostEntry host = Dns.GetHostEntry(MediusClass.Settings.NATIp ?? "natservice.pdonline.scea.com");
            PS2CipherFactory fac = new();
            var rsa = fac.CreateNew(CipherContext.RSA_AUTH) as PS2_RSA;

            int iAccountID = MediusClass.Manager.AnonymousAccountIDGenerator(MediusClass.Settings.AnonymousIDRangeSeed);
#if DEBUG
LoggerAccessor.LogInfo($"[MAS] - AnonymousIDRangeSeedGenerator AccountID returned {iAccountID}");
#endif
            if (data.ClientObject != null)
            {
                char[] charsToRemove = { ':', 'f', '{', '}' };
                if (data.ClientObject.IP == IPAddress.Any)
                    data.ClientObject.SetIp(((IPEndPoint)clientChannel.RemoteAddress).Address.ToString().Trim(charsToRemove));
            }

            if (data.ClientObject != null)
            {
                CIDManager.CreateCIDPair("AnonymousClient", data.MachineId);

                // Login
                await data.ClientObject.LoginAnonymous(anonymousLoginRequest, iAccountID);

                #region Update DB IP and CID

                //We don't post to the database as anonymous... This ain't it chief

                #endregion

                // Add to logged in clients
                MediusClass.Manager.AddOrUpdateLoggedInClient(data.ClientObject);

LoggerAccessor.LogInfo($"[MAS] - LOGGING IN ANONYMOUSLY AS {data.ClientObject.AccountDisplayName} with access token {data.ClientObject.AccessToken}");

                // Tell client
                data.ClientObject.Queue(new MediusAnonymousLoginResponse()
                {
                    MessageID = anonymousLoginRequest.MessageID,
                    StatusCode = MediusCallbackStatus.MediusSuccess,
                    AccountID = iAccountID,
                    AccountType = MediusAccountType.MediusMasterAccount,
                    WorldID = data.ClientObject.CurrentChannel!.Id,
                    ConnectInfo = new NetConnectionInfo()
                    {
                        AccessKey = data.ClientObject.AccessToken,
                        SessionKey = data.ClientObject.SessionKey,
                        TargetWorldID = data.ClientObject.CurrentChannel!.Id,
                        ServerKey = new RSA_KEY(), // Null for 108 clients
                        AddressList = new NetAddressList()
                        {
                            AddressList = new NetAddress[Constants.NET_ADDRESS_LIST_COUNT]
                            {
                               new NetAddress() {Address = MediusClass.LobbyServer.IPAddress.ToString(), Port = MediusClass.LobbyServer.TCPPort, AddressType = NetAddressType.NetAddressTypeExternal},
                               new NetAddress() {Address = host.AddressList.First().ToString(), Port = MediusClass.Settings.NATPort, AddressType = NetAddressType.NetAddressTypeNATService},
                            }
                        },
                        Type = NetConnectionType.NetConnectionTypeClientServerTCP
                    }
                });

                data.ClientObject.KeepAliveUntilNextConnection();
            }
        }
        #endregion

        #region TimeZone
        public Task<MediusTimeZone> GetTimeZone(DateTime time)
        {
            var tz = TimeZoneInfo.Local;
            var tzStanName = tz.StandardName;

            if (tzStanName == "CEST")
                return Task.FromResult(MediusTimeZone.MediusTimeZone_CEST);
            else if (tz.Id == "Swedish Standard Time")
                return Task.FromResult(MediusTimeZone.MediusTimeZone_SWEDISHST);
            else if (tz.Id == "FST")
                return Task.FromResult(MediusTimeZone.MediusTimeZone_FST);
            else if (tz.Id == "Central Africa Time")
                return Task.FromResult(MediusTimeZone.MediusTimeZone_CAT);
            else if (tzStanName == "South Africa Standard Time")
                return Task.FromResult(MediusTimeZone.MediusTimeZone_SAST);
            else if (tz.Id == "EET")
                return Task.FromResult(MediusTimeZone.MediusTimeZone_EET);
            else if (tz.Id == "Israel Standard Time")
                return Task.FromResult(MediusTimeZone.MediusTimeZone_ISRAELST);

            return Task.FromResult(MediusTimeZone.MediusTimeZone_GMT);
        }
        #endregion

        #region ConvertFromIntegerToIpAddress
        /// <summary>
        /// Convert from Binary Ip Address to UInt
        /// </summary>
        /// <param name="ipAddress">Binary formatted IP Address</param>
        /// <returns></returns>
        public static string ConvertFromIntegerToIpAddress(uint ipAddress)
        {
            byte[] bytes = BitConverter.GetBytes(ipAddress);

            if (!BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            return new IPAddress(bytes).ToString();
        }
        #endregion

        #region ConvertFromIntegerToPort
        /// <summary>
        /// Convert from Binary Ip Address to UInt
        /// </summary>
        /// <param name="port">Binary formatted IP Address</param>
        /// <returns></returns>
        public static int ConvertFromIntegerToPort(string port)
        {
            int i = Convert.ToInt32(port, 2);

            // flip little-endian to big-endian(network order)
            /* NOT NEEDED
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            */
            return i;
        }
        #endregion

        #region ConvertFromIntegerToIpAddress
        /// <summary>
        /// Convert from Binary Ip Address to UInt
        /// </summary>
        /// <param name="ipAddress">Binary formatted IP Address</param>
        /// <returns></returns>
        public static uint ConvertFromIpAddressToBinary(IPAddress ipAddress)
        {
            uint Uint = (uint)BitConverter.ToInt32(ipAddress.GetAddressBytes());

            // flip little-endian to big-endian(network order)
            /* NOT NEEDED
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            */
            return Uint;
        }
        #endregion

        #region PokeEngine

        private void PokePatch(IChannel clientChannel, ChannelData data)
        {
            if (MediusClass.Settings.PokePatchOn)
            {
                if (File.Exists(Directory.GetCurrentDirectory() + $"/static/poke_config.json"))
                {
                    try
                    {
                        JObject? jsonObject = JObject.Parse(File.ReadAllText(Directory.GetCurrentDirectory() + $"/static/poke_config.json"));

                        foreach (JProperty? appProperty in jsonObject.Properties())
                        {
                            string? appId = appProperty.Name;

                            if (!string.IsNullOrEmpty(appId) && appId == data.ApplicationId.ToString())
                            {
                                if (appProperty.Value is JObject innerObject)
                                {
                                    foreach (JProperty? offsetProperty in innerObject.Properties())
                                    {
                                        string? offset = offsetProperty.Name;
                                        string? valuestr = offsetProperty.Value.ToString();

                                        if (!string.IsNullOrEmpty(offset) && !string.IsNullOrEmpty(valuestr) && uint.TryParse(offset.Replace("0x", string.Empty), NumberStyles.HexNumber, null, out uint offsetValue) && uint.TryParse(valuestr, NumberStyles.Any, null, out uint hexValue))
                                        {
                                            LoggerAccessor.LogInfo($"[MAS] - MemoryPoke sent to appid {appId} with infos : offset:{offset} - value:{valuestr}");
                                            Queue(new RT_MSG_SERVER_MEMORY_POKE()
                                            {
                                                start_Address = offsetValue,
                                                Payload = BitConverter.GetBytes(hexValue),
                                                SkipEncryption = true

                                            }, clientChannel);
                                        }
                                        else
                                            LoggerAccessor.LogWarn($"[MAS] - MemoryPoke failed to convert json properties! Check your Json syntax.");
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LoggerAccessor.LogWarn($"[MAS] - MemoryPoke failed to initialise! {ex}.");
                    }
                }
                else
                    LoggerAccessor.LogWarn($"[MAS] - No MemoryPoke config found.");
            }
        }

        private bool CheatQuery(uint address, int Length, IChannel? clientChannel, CheatQueryType Type = CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY, int SequenceId = 1)
        {
            // address = 0, don't read
            if (address == 0)
                return false;

            // client channel is null, don't read
            if (clientChannel == null)
                return false;

            // read client memory
            Queue(new RT_MSG_SERVER_CHEAT_QUERY()
            {
                QueryType = Type,
                SequenceId = SequenceId,
                StartAddress = address,
                Length = Length,
            }, clientChannel);

            // return read
            return true;
        }

        private bool PatchHttpsSVOCheck(uint patchLocation, IChannel? clientChannel)
        {
            // patch location = 0, don't patch
            if (patchLocation == 0)
                return false;

            // client channel is null, don't patch
            if (clientChannel == null)
                return false;

            // poke client memory
            Queue(new RT_MSG_SERVER_MEMORY_POKE()
            {
                start_Address = patchLocation,
                Payload = new byte[] { 0x3A, 0x2F }, // We patch to :/ instead of s: as the check only compare first 6 characters.
                SkipEncryption = false,
            }, clientChannel);

            // return patched
            return true;
        }

        private bool PokeAddress(uint patchLocation, byte[] Payload, IChannel? clientChannel)
        {
            // patch location = 0, don't patch
            if (patchLocation == 0)
                return false;

            // client channel is null, don't patch
            if (clientChannel == null)
                return false;

            // poke client memory
            Queue(new RT_MSG_SERVER_MEMORY_POKE()
            {
                start_Address = patchLocation,
                Payload = Payload,
                SkipEncryption = false,
            }, clientChannel);

            // return patched
            return true;
        }
        #endregion

        #region SHA256

        /// <summary>
        /// Compute the SHA256 checksum of a string.
        /// <para>Calcul la somme des contr�les en SHA256 d'un string.</para>
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>A string.</returns>
        private static string ComputeSHA256(string input)
        {
            // ComputeHash - returns byte array  
            byte[] bytes = NetHasher.DotNetHasher.ComputeSHA256(Encoding.UTF8.GetBytes(input));

            // Convert byte array to a string   
            StringBuilder builder = new();
            for (int i = 0; i < bytes.Length; i++)
                builder.Append(bytes[i].ToString("x2"));

            return builder.ToString();
        }
        #endregion
    }
}
