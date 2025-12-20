using CustomLogger;
using DotNetty.Transport.Channels;
using Horizon.MUM.Models;
using Horizon.RT.Common;
using Horizon.RT.Models;
using HorizonService.PlaystationHomePlugin.Models;
using MultiServerLibrary.Extension;
using MultiServerLibrary.GeoLocalization;
using MultiServerLibrary.HTTP;
using NetHasher.CRC;
using PS2FloatLibrary;
using System.Security.Cryptography;
using System.Text;

namespace Horizon.SERVER.Extension.PlayStationHome
{
    public static class HomeGuestJoiningSystem
    {
        private static readonly byte[] RandCRCKey = ByteUtils.GenerateRandomBytes(24);
        private static readonly byte[] RandCRCIV = ByteUtils.GenerateRandomBytes(8);

        public static bool ProcessGJSQueue(MediusGameListRequest gameListRequest, ClientObject rClient, Action<int> clientCallback)
        {
            if ((rClient.ApplicationId == 20371 || rClient.ApplicationId == 20374) && !string.IsNullOrEmpty(rClient.LobbyKeyOverride))
            {
                string requestedLobbyKey = rClient.LobbyKeyOverride;
                rClient.LobbyKeyOverride = null;
                bool foundLobby = false;
                bool foundPersonalSpaceRequest = false;

                // Check for generic field 2 presence (only for personal spaces).
                foreach (GameListFilter filter in rClient.GameListFilters)
                {
                    if (filter.FilterField == MediusGameListFilterField.MEDIUS_FILTER_GENERIC_FIELD_2)
                        foundPersonalSpaceRequest = true;
                }

                if (!foundPersonalSpaceRequest)
                    return foundLobby;

                foreach (Game homeLobby in MediusClass.Manager.GetAllGamesByAppId(rClient.ApplicationId))
                {
                    LobbyDescriptor descriptor = LobbyDescriptor.Parse(homeLobby.GameName);

                    if (homeLobby.Host != null && descriptor != null && descriptor.Type == "AP")
                    {
                        string LobbyName = descriptor.Description;

                        if (GetGJSCRC(homeLobby.Host.AccountName!, LobbyName + "H3m0", homeLobby.utcTimeCreated) == requestedLobbyKey)
                        {
                            foundLobby = true;

                            rClient.Queue(new MediusGameListResponse()
                            {
                                MessageID = gameListRequest.MessageID,
                                StatusCode = MediusCallbackStatus.MediusSuccess,
                                MediusWorldID = homeLobby.MediusWorldId,
                                GameName = homeLobby.GameName,
                                WorldStatus = homeLobby.WorldStatus,
                                GameHostType = homeLobby.GameHostType,
                                PlayerCount = (ushort)homeLobby.PlayerCount,
                                EndOfList = true
                            });

                            if (rClient.WorldCorePointer != 0 && rClient.ClientHomeData != null)
                            {
                                const uint guestPtrPrefix = 0x00020000;

                                switch (rClient.ClientHomeData.Type)
                                {
                                    case "HDK With Offline":
                                        switch (rClient.ClientHomeData.Version)
                                        {
                                            case "01.86.09":
                                                rClient.WorldCoreSpaceTypePointer = rClient.WorldCorePointer + guestPtrPrefix - 0x6194;
                                                break;
                                            default:
                                                break;
                                        }
                                        break;
                                    case "HDK Online Only":
                                        switch (rClient.ClientHomeData.Version)
                                        {
                                            default:
                                                break;
                                        }
                                        break;
                                    case "HDK Online Only (Dbg Symbols)":
                                        switch (rClient.ClientHomeData.Version)
                                        {
                                            case "01.82.09":
                                                rClient.WorldCoreSpaceTypePointer = rClient.WorldCorePointer + guestPtrPrefix - 0x61a8;
                                                break;
                                            default:
                                                break;
                                        }
                                        break;
                                    case "Online Debug":
                                    case "Online Debug (QA)":
                                        switch (rClient.ClientHomeData.Version)
                                        {
                                            case "01.83.12":
                                                rClient.WorldCoreSpaceTypePointer = rClient.WorldCorePointer + guestPtrPrefix - 0x6194;
                                                break;
                                            case "01.86.09":
                                                rClient.WorldCoreSpaceTypePointer = rClient.WorldCorePointer + guestPtrPrefix - 0x6194;
                                                break;
                                            default:
                                                break;
                                        }
                                        break;
                                    case "Retail":
                                        switch (rClient.ClientHomeData.Version)
                                        {
                                            case "01.86.09":
                                                rClient.WorldCoreSpaceTypePointer = rClient.WorldCorePointer + guestPtrPrefix - 0x62a4;
                                                break;
                                            default:
                                                break;
                                        }
                                        break;
                                }

                                if (rClient.WorldCoreSpaceTypePointer != 0)
                                    rClient.TryAddTask("GJS GUEST BRUTEFORCE", clientCallback, homeLobby.MediusWorldId);
                            }

                            break;
                        }
                    }
                }

                if (foundLobby)
                    return true;
                else if (!string.IsNullOrEmpty(rClient.SSFWid))
                    HTTPProcessor.RequestURLPOST($"{HorizonServerConfiguration.SSFWUrl}/WebService/R3moveLayoutOverride/", new Dictionary<string, string>() { { "sessionid", rClient.SSFWid } }, string.Empty, "text/plain");
            }

            return false;
        }

        public static Task<bool> SendCrcOverride(string targetClientIp, string? AccessToken, string SceneCrc, bool Retail, string env)
        {
            bool AccessTokenProvided = !string.IsNullOrEmpty(AccessToken);

            List<ClientObject>? clients = null;

            if (AccessTokenProvided)
            {
                ClientObject? client = MediusClass.Manager.GetClientByAccessToken(AccessToken, Retail ? 20374 : 20371);
                if (client != null)
                {
                    clients = new()
                    {
                        client
                    };
                }
            }
            else
                clients = MediusClass.Manager.GetClientsByIp(targetClientIp, Retail ? 20374 : 20371);

            if (clients != null)
            {
                foreach (Game homeLobby in MediusClass.Manager.GetAllGamesByAppId(Retail ? 20374 : 20371))
                {
                    LobbyDescriptor descriptor = LobbyDescriptor.Parse(homeLobby.GameName);

                    if (homeLobby.Host != null && descriptor != null && descriptor.Type == "AP")
                    {
                        string LobbyName = descriptor.Description;

                        if (GetGJSCRC(homeLobby.Host.AccountName!, LobbyName + "H3m0", homeLobby.utcTimeCreated) == SceneCrc)
                        {
                            string ssfwSceneNameResult = HTTPProcessor.RequestURLPOST($"{HorizonServerConfiguration.SSFWUrl}/WebService/GetSceneLike/", new Dictionary<string, string>() { { "like", LobbyName } }, string.Empty, "text/plain");

                            if (!string.IsNullOrEmpty(ssfwSceneNameResult) && ssfwSceneNameResult.Contains(','))
                            {
                                string[] sceneData = ssfwSceneNameResult.Split(',');

                                foreach (ClientObject client in clients)
                                {
                                    if (client.CurrentGame == homeLobby)
                                        continue;

                                    client.LobbyKeyOverride = SceneCrc;

                                    bool isLcCompatible = !string.IsNullOrEmpty(client.ClientHomeData?.Type) && (client.ClientHomeData.Type.Contains("HDK") || client.ClientHomeData.Type == "Online Debug");

                                    if (!string.IsNullOrEmpty(client.SSFWid) && !string.IsNullOrEmpty(homeLobby.Host.AccountName))
                                    {
                                        Dictionary<string, string> headersToSend;

                                        if (!string.IsNullOrEmpty(env))
                                            headersToSend = new Dictionary<string, string>() { { "sessionid", client.SSFWid }, { "targetUserName", homeLobby.Host.AccountName }, { "sceneId", sceneData[1] }, { "env", env } };
                                        else
                                            headersToSend = new Dictionary<string, string>() { { "sessionid", client.SSFWid }, { "targetUserName", homeLobby.Host.AccountName }, { "sceneId", sceneData[1] } };

                                        _ = Task.Run(() =>
                                        {
                                            foreach (var uuidToAdd in HTTPProcessor.RequestURLPOST(
                                                $"{HorizonServerConfiguration.SSFWUrl}/WebService/ApplyLayoutOverride/",
                                                headersToSend,
                                                string.Empty,
                                                "text/plain"
                                            ).ParseJsonStringProperty("furnitureObjectId"))
                                            {
                                                if (isLcCompatible)
                                                    _ = HomeRTMTools.SendRemoteCommand(client, $"lc Debug.System( 'inv adduserobj {uuidToAdd}' )");
                                                else
                                                    _ = HomeRTMTools.SendRemoteCommand(client, $"inv adduserobj {uuidToAdd}");
                                            }

                                        });
                                    }

                                    if (client.ClientHomeData!.Version == "01.86.09")
                                        _ = HomeServerMessage.SendSimpleRelocate(client, GeoIP.GetCountryLangCodeFromIP(client.IP) ?? "enUS", Encoding.UTF8.GetBytes(sceneData[0]), isLcCompatible);
                                    else if (isLcCompatible)
                                        _ = HomeRTMTools.SendRemoteCommand(client, $"lc Debug.System( 'map {sceneData[0]}' )");
                                    else
                                        _ = HomeRTMTools.SendRemoteCommand(client, $"map {sceneData[0]}");
                                }

                                return Task.FromResult(true);
                            }

                            LoggerAccessor.LogError($"[HomeGuestJoiningSystem] - {LobbyName} didn't match any SSFW entry!");

                            return Task.FromResult(false);
                        }
                    }
                }

                LoggerAccessor.LogError($"[HomeGuestJoiningSystem] - {SceneCrc} didn't match any Private lobby!");

                return Task.FromResult(false);
            }

            LoggerAccessor.LogError($"[HomeGuestJoiningSystem] - {(!AccessTokenProvided ? $"Ip:{targetClientIp}" : $"AccessToken:{AccessToken}")} didn't return any Medius clients!");

            return Task.FromResult(false);
        }

        public static Task<List<string>> getCrcList(string targetClientIp, string? AccessToken, bool Retail, bool AllClients)
        {
            bool AccessTokenProvided = !string.IsNullOrEmpty(AccessToken);
            List<ClientObject>? clients = null;
            List<string> crcList = new();

            if (AllClients)
                clients = MediusClass.Manager.GetClients(Retail ? 20374 : 20371);
            else if (AccessTokenProvided)
            {
                ClientObject? client = MediusClass.Manager.GetClientByAccessToken(AccessToken, Retail ? 20374 : 20371);
                if (client != null)
                {
                    clients = new()
                    {
                        client
                    };
                }
            }
            else
                clients = MediusClass.Manager.GetClientsByIp(targetClientIp, Retail ? 20374 : 20371);

            if (clients != null)
            {
                foreach (ClientObject client in clients)
                {
                    if (client.CurrentGame != null && client.CurrentGame.Host != null && !string.IsNullOrEmpty(client.CurrentGame.GameName) && client.CurrentGame.GameName.StartsWith("AP|") && client.CurrentGame.GameName.Split('|').Length >= 5)
                        crcList.Add($"{client.AccountName}|{GetGJSCRC(client.CurrentGame.Host.AccountName!, client.CurrentGame.GameName!.Split('|')[5] + "H3m0", client.CurrentGame.utcTimeCreated)}");
                }

                return Task.FromResult(crcList);
            }

            LoggerAccessor.LogError($"[HomeGuestJoiningSystem] - {(!AccessTokenProvided ? $"Ip:{targetClientIp}" : $"AccessToken:{AccessToken}")} didn't return any Medius clients!");

            return Task.FromResult(crcList);
        }

        public static string GetGJSCRC(string salt1, string salt2, DateTime dateSalt)
        {
            uint res1;
            uint res2;

            TripleDES des = TripleDES.Create();

            des.Mode = CipherMode.CBC;
            des.Padding = PaddingMode.PKCS7;
            des.Key = RandCRCKey;
            des.IV = RandCRCIV;

            ICryptoTransform cryptoTransform = des.CreateEncryptor();

            byte[] SaltedDateTimeBytes = Encoding.UTF8.GetBytes("S1l3" + dateSalt.ToString());
            byte[] PassCode = Encoding.UTF8.GetBytes(salt1 + salt2 + "H3m0");

            res1 = CRC32.CreateCastagnoli(cryptoTransform.TransformFinalBlock(PassCode, 0, PassCode.Length));
			
            des.Dispose();

            res2 = CRC32.CreateCastagnoli(cryptoTransform.TransformFinalBlock(SaltedDateTimeBytes, 0, SaltedDateTimeBytes.Length));

            return TimeZoneInfo.Local.IsDaylightSavingTime(dateSalt) ? ((res1 ^ dateSalt.Minute).ToString("X8") + (dateSalt.Day ^ dateSalt.DayOfYear ^ res2).ToString("X8"))
                : ((dateSalt.Minute ^ res2).ToString("X8") + (dateSalt.Hour ^ res1 ^ dateSalt.Month).ToString("X8"));
        }

        public static uint IsInOwnApartment(int offsetValue)
        {
            uint uVar2;

            uVar2 = 0;
            if (offsetValue != 0)
            {
                int uVar1 = BitUtils.CountLeadingSignBits(offsetValue ^ 5);
                uVar2 = (uint)uVar1 >> 5;
            }
            return uVar2;
        }
    }
}
