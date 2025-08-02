using CustomLogger;
using Horizon.MUM.Models;
using MultiServerLibrary.Extension;
using MultiServerLibrary.HTTP;
using PS2FloatLibrary;
using System.Security.Cryptography;
using System.Text;
using NetHasher.CRC;
using MultiServerLibrary.GeoLocalization;

namespace Horizon.SERVER.Extension.PlayStationHome
{
    public static class HomeGuestJoiningSystem
    {
        private static readonly byte[] RandCRCKey = ByteUtils.GenerateRandomBytes(24);
        private static readonly byte[] RandCRCIV = ByteUtils.GenerateRandomBytes(8);

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
                    if (homeLobby.Host != null && !string.IsNullOrEmpty(homeLobby.GameName) && homeLobby.GameName.StartsWith("AP|") && homeLobby.GameName.Split('|').Length >= 5)
                    {
                        string LobbyName = homeLobby.GameName!.Split('|')[5];

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

                                        // Process Environment.ProcessorCount uuids at a time, removing the limit is not tolerable as CPU usage goes way too high.
                                        Parallel.ForEach(HTTPProcessor.RequestURLPOST($"{HorizonServerConfiguration.SSFWUrl}/WebService/ApplyLayoutOverride/", headersToSend, string.Empty, "text/plain").ParseJsonStringProperty("furnitureObjectId"),
                                            new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, (uuidToAdd, stateInner) =>
                                        {
                                            if (isLcCompatible)
                                                _ = HomeRTMTools.SendRemoteCommand(client, $"lc Debug.System( 'inv adduserobj {uuidToAdd}' )");
                                            else
                                                _ = HomeRTMTools.SendRemoteCommand(client, $"inv adduserobj {uuidToAdd}");
                                        });
                                    }

                                    _ = HomeServerMessage.SendSimpleRelocate(client, GeoIP.GetCountryLangCodeFromIP(client.IP) ?? "enUS", Encoding.UTF8.GetBytes(sceneData[0]), isLcCompatible);
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
