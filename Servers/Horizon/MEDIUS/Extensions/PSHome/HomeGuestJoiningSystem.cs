using System.Numerics;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using CastleLibrary.NetHasher.CRC;
using CustomLogger;
using Horizon.MUM.Models;
using Horizon.PlaystationHomePlugin.Models;
using Horizon.RT.Common;
using Horizon.RT.Models;
using MultiServerLibrary.Extension;
using MultiServerLibrary.Extension.Windows;
using MultiServerLibrary.GeoLocalization;
using MultiServerLibrary.HTTP;
using SimdJsonSharp;

namespace Horizon.MEDIUS.Extensions.PSHome
{
    public static class HomeGuestJoiningSystem
    {
        private static readonly byte[] RandCRCKey = ByteUtils.GenerateRandomBytes(24);
        private static readonly byte[] RandCRCIV = ByteUtils.GenerateRandomBytes(8);

        public static bool ProcessGJSQueue(
            MediusGameListRequest gameListRequest,
            ClientObject rClient,
            Action<int> clientCallback
        )
        {
            if (
                (rClient.ApplicationId == 20371 || rClient.ApplicationId == 20374)
                && !string.IsNullOrEmpty(rClient.LobbyKeyOverride)
            )
            {
                var requestedLobbyKey = rClient.LobbyKeyOverride;
                rClient.LobbyKeyOverride = null;
                var foundLobby = false;
                var foundPersonalSpaceRequest = false;

                // Check for generic field 2 presence (only for personal spaces).
                foreach (var filter in rClient.GameListFilters)
                {
                    if (
                        filter.FilterField
                        == MediusGameListFilterField.MEDIUS_FILTER_GENERIC_FIELD_2
                    )
                        foundPersonalSpaceRequest = true;
                }

                if (!foundPersonalSpaceRequest)
                    return foundLobby;

                foreach (
                    var homeLobby in Program.MUMManager.GetAllGamesByAppId(rClient.ApplicationId)
                )
                {
                    var descriptor = LobbyDescriptor.Parse(homeLobby.GameName);

                    if (homeLobby.Host != null && descriptor != null && descriptor.Type == "AP")
                    {
                        var LobbyName = descriptor.Description;

                        if (
                            GetGJSCRC(
                                homeLobby.Host.AccountName!,
                                LobbyName + "H3m0",
                                homeLobby.utcTimeCreated
                            ) == requestedLobbyKey
                        )
                        {
                            foundLobby = true;

                            rClient.Queue(
                                new MediusGameListResponse()
                                {
                                    MessageID = gameListRequest.MessageID,
                                    StatusCode = MediusCallbackStatus.MediusSuccess,
                                    MediusWorldID = homeLobby.MediusWorldId,
                                    GameName = homeLobby.GameName,
                                    WorldStatus = homeLobby.WorldStatus,
                                    GameHostType = homeLobby.GameHostType,
                                    PlayerCount = (ushort)homeLobby.PlayerCount,
                                    EndOfList = true,
                                }
                            );

                            if (rClient.WorldCorePointer != 0 && rClient.ClientHomeData != null)
                            {
                                const uint guestPtrPrefix = 0x00020000;

                                switch (rClient.ClientHomeData.Type)
                                {
                                    case "HDK With Offline":
                                        switch (rClient.ClientHomeData.Version)
                                        {
                                            case "01.86.09":
                                                rClient.WorldCoreSpaceTypePointer =
                                                    rClient.WorldCorePointer
                                                    + guestPtrPrefix
                                                    - 0x6194;
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
                                                rClient.WorldCoreSpaceTypePointer =
                                                    rClient.WorldCorePointer
                                                    + guestPtrPrefix
                                                    - 0x61a8;
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
                                                rClient.WorldCoreSpaceTypePointer =
                                                    rClient.WorldCorePointer
                                                    + guestPtrPrefix
                                                    - 0x6194;
                                                break;
                                            case "01.86.09":
                                                rClient.WorldCoreSpaceTypePointer =
                                                    rClient.WorldCorePointer
                                                    + guestPtrPrefix
                                                    - 0x6194;
                                                break;
                                            default:
                                                break;
                                        }
                                        break;
                                    case "Retail":
                                        switch (rClient.ClientHomeData.Version)
                                        {
                                            case "01.86.09":
                                                rClient.WorldCoreSpaceTypePointer =
                                                    rClient.WorldCorePointer
                                                    + guestPtrPrefix
                                                    - 0x62a4;
                                                break;
                                            default:
                                                break;
                                        }
                                        break;
                                }

                                if (rClient.WorldCoreSpaceTypePointer != 0)
                                    rClient.TryAddTask(
                                        "GJS GUEST BRUTEFORCE",
                                        clientCallback,
                                        homeLobby.MediusWorldId
                                    );
                            }

                            break;
                        }
                    }
                }

                if (foundLobby)
                    return true;
                else if (!string.IsNullOrEmpty(rClient.SSFWid))
                    HTTPProcessor.RequestURLPOST(
                        $"{HorizonServerConfiguration.SSFWUrl}/WebService/R3moveLayoutOverride/",
                        new Dictionary<string, string>() { { "sessionid", rClient.SSFWid } },
                        string.Empty,
                        "text/plain"
                    );
            }

            return false;
        }

        public static Task<bool> SendCrcOverride(
            string targetClientIp,
            string? AccessToken,
            string SceneCrc,
            bool Retail,
            string env
        )
        {
            var AccessTokenProvided = !string.IsNullOrEmpty(AccessToken);

            List<ClientObject>? clients = null;

            if (AccessTokenProvided)
            {
                var client = Program.MUMManager.GetClientByAccessToken(
                    AccessToken,
                    Retail ? 20374 : 20371
                );
                if (client != null)
                {
                    clients = new() { client };
                }
            }
            else
                clients = Program.MUMManager.GetClientsByIp(targetClientIp, Retail ? 20374 : 20371);

            if (clients != null)
            {
                foreach (
                    var homeLobby in Program.MUMManager.GetAllGamesByAppId(Retail ? 20374 : 20371)
                )
                {
                    var descriptor = LobbyDescriptor.Parse(homeLobby.GameName);

                    if (homeLobby.Host != null && descriptor != null && descriptor.Type == "AP")
                    {
                        var LobbyName = descriptor.Description;

                        if (
                            GetGJSCRC(
                                homeLobby.Host.AccountName!,
                                LobbyName + "H3m0",
                                homeLobby.utcTimeCreated
                            ) == SceneCrc
                        )
                        {
                            var ssfwSceneNameResult = HTTPProcessor.RequestURLPOST(
                                $"{HorizonServerConfiguration.SSFWUrl}/WebService/GetSceneLike/",
                                new Dictionary<string, string>() { { "like", LobbyName } },
                                string.Empty,
                                "text/plain"
                            );

                            if (
                                !string.IsNullOrEmpty(ssfwSceneNameResult)
                                && ssfwSceneNameResult.Contains(',')
                            )
                            {
                                var sceneData = ssfwSceneNameResult.Split(',');

                                foreach (var client in clients)
                                {
                                    if (client.CurrentGame == homeLobby)
                                        continue;

                                    client.LobbyKeyOverride = SceneCrc;

                                    var isLcCompatible =
                                        !string.IsNullOrEmpty(client.ClientHomeData?.Type)
                                        && (
                                            client.ClientHomeData.Type.Contains("HDK")
                                            || client.ClientHomeData.Type == "Online Debug"
                                        );

                                    if (
                                        !string.IsNullOrEmpty(client.SSFWid)
                                        && !string.IsNullOrEmpty(homeLobby.Host.AccountName)
                                    )
                                    {
                                        var headersToSend = !string.IsNullOrEmpty(env)
                                            ? new Dictionary<string, string>()
                                            {
                                                { "sessionid", client.SSFWid },
                                                { "targetUserName", homeLobby.Host.AccountName },
                                                { "sceneId", sceneData[1] },
                                                { "env", env },
                                            }
                                            : new Dictionary<string, string>()
                                            {
                                                { "sessionid", client.SSFWid },
                                                { "targetUserName", homeLobby.Host.AccountName },
                                                { "sceneId", sceneData[1] },
                                            };
                                        _ = Task.Run(() =>
                                        {
                                            foreach (
                                                var uuidToAdd in ParseJsonStringProperty(
                                                    HTTPProcessor.RequestURLPOST(
                                                        $"{HorizonServerConfiguration.SSFWUrl}/WebService/ApplyLayoutOverride/",
                                                        headersToSend,
                                                        string.Empty,
                                                        "text/plain"
                                                    ),
                                                    "furnitureObjectId"
                                                )
                                            )
                                            {
                                                _ = isLcCompatible
                                                    ? HomeRTMTools.SendRemoteCommand(
                                                        client,
                                                        $"lc Debug.System( 'inv adduserobj {uuidToAdd}' )"
                                                    )
                                                    : HomeRTMTools.SendRemoteCommand(
                                                        client,
                                                        $"inv adduserobj {uuidToAdd}"
                                                    );
                                            }
                                        });
                                    }

                                    _ =
                                        client.ClientHomeData!.Version == "01.86.09"
                                            ? HomeServerMessage.SendSimpleRelocate(
                                                client,
                                                GeoIP.GetCountryLangCodeFromIP(client.IP) ?? "enUS",
                                                Encoding.UTF8.GetBytes(sceneData[0]),
                                                isLcCompatible
                                            )
                                        : isLcCompatible
                                            ? HomeRTMTools.SendRemoteCommand(
                                                client,
                                                $"lc Debug.System( 'map {sceneData[0]}' )"
                                            )
                                        : HomeRTMTools.SendRemoteCommand(
                                            client,
                                            $"map {sceneData[0]}"
                                        );
                                }

                                return Task.FromResult(true);
                            }

                            LoggerAccessor.LogError(
                                $"[HomeGuestJoiningSystem] - {LobbyName} didn't match any SSFW entry!"
                            );

                            return Task.FromResult(false);
                        }
                    }
                }

                LoggerAccessor.LogError(
                    $"[HomeGuestJoiningSystem] - {SceneCrc} didn't match any Private lobby!"
                );

                return Task.FromResult(false);
            }

            LoggerAccessor.LogError(
                $"[HomeGuestJoiningSystem] - {(!AccessTokenProvided ? $"Ip:{targetClientIp}" : $"AccessToken:{AccessToken}")} didn't return any Medius clients!"
            );

            return Task.FromResult(false);
        }

        public static Task<List<string>> GetCrcList(
            string targetClientIp,
            string? AccessToken,
            bool Retail,
            bool AllClients
        )
        {
            var AccessTokenProvided = !string.IsNullOrEmpty(AccessToken);
            List<ClientObject>? clients = null;
            List<string> crcList = new();

            if (AllClients)
                clients = Program.MUMManager.GetClients(Retail ? 20374 : 20371);
            else if (AccessTokenProvided)
            {
                var client = Program.MUMManager.GetClientByAccessToken(
                    AccessToken,
                    Retail ? 20374 : 20371
                );
                if (client != null)
                {
                    clients = new() { client };
                }
            }
            else
                clients = Program.MUMManager.GetClientsByIp(targetClientIp, Retail ? 20374 : 20371);

            if (clients != null)
            {
                foreach (var client in clients)
                {
                    if (
                        client.CurrentGame != null
                        && client.CurrentGame.Host != null
                        && !string.IsNullOrEmpty(client.CurrentGame.GameName)
                        && client.CurrentGame.GameName.StartsWith("AP|")
                        && client.CurrentGame.GameName.Split('|').Length >= 5
                    )
                        crcList.Add(
                            $"{client.AccountName}|{GetGJSCRC(client.CurrentGame.Host.AccountName!, client.CurrentGame.GameName!.Split('|')[5] + "H3m0", client.CurrentGame.utcTimeCreated)}"
                        );
                }

                return Task.FromResult(crcList);
            }

            LoggerAccessor.LogError(
                $"[HomeGuestJoiningSystem] - {(!AccessTokenProvided ? $"Ip:{targetClientIp}" : $"AccessToken:{AccessToken}")} didn't return any Medius clients!"
            );

            return Task.FromResult(crcList);
        }

        public static string GetGJSCRC(string salt1, string salt2, DateTime dateSalt)
        {
            uint res1;
            uint res2;

            var des = TripleDES.Create();

            des.Mode = CipherMode.CBC;
            des.Padding = PaddingMode.PKCS7;
            des.Key = RandCRCKey;
            des.IV = RandCRCIV;

            var cryptoTransform = des.CreateEncryptor();

            var SaltedDateTimeBytes = Encoding.UTF8.GetBytes("S1l3" + dateSalt.ToString());
            var PassCode = Encoding.UTF8.GetBytes(salt1 + salt2 + "H3m0");

            res1 = CRC32.CreateCastagnoli(
                cryptoTransform.TransformFinalBlock(PassCode, 0, PassCode.Length)
            );

            des.Dispose();

            res2 = CRC32.CreateCastagnoli(
                cryptoTransform.TransformFinalBlock(
                    SaltedDateTimeBytes,
                    0,
                    SaltedDateTimeBytes.Length
                )
            );

            return TimeZoneInfo.Local.IsDaylightSavingTime(dateSalt)
                ? (
                    (res1 ^ dateSalt.Minute).ToString("X8")
                    + (dateSalt.Day ^ dateSalt.DayOfYear ^ res2).ToString("X8")
                )
                : (
                    (dateSalt.Minute ^ res2).ToString("X8")
                    + (dateSalt.Hour ^ res1 ^ dateSalt.Month).ToString("X8")
                );
        }

        public static uint IsInOwnApartment(int offsetValue)
        {
            uint uVar2;

            uVar2 = 0;
            if (offsetValue != 0)
            {
                var uVar1 = BitOperationsM.CountLeadingSignBits(offsetValue ^ 5);
                uVar2 = (uint)uVar1 >> 5;
            }
            return uVar2;
        }

        public static unsafe List<string> ParseJsonStringProperty(string jsonText, string property)
        {
            var result = new List<string>();

            if (!string.IsNullOrEmpty(jsonText))
            {
                // Uses SIMD Json when possible.
                if (Avx2.IsSupported)
                {
                    var bytes = Encoding.UTF8.GetBytes(jsonText);

                    if (Win32API.IsWindows)
                    {
                        fixed (byte* ptr = bytes) // pin bytes while we are working on them
                            using (var doc = SimdJsonN.ParseJson(ptr, bytes.Length))
                            {
                                if (doc.IsValid)
                                {
                                    // Open iterator
                                    using (var iterator = doc.CreateIterator())
                                    {
                                        while (iterator.MoveForward())
                                        {
                                            if (
                                                iterator.IsString
                                                && iterator.GetUtf16String() == property
                                            )
                                            {
                                                if (iterator.MoveForward())
                                                    result.Add(iterator.GetUtf16String());
                                            }
                                        }
                                    }
                                }
                            }
                    }
                    else
                    {
                        fixed (byte* ptr = bytes) // pin bytes while we are working on them
                            using (var doc = SimdJson.ParseJson(ptr, bytes.Length))
                            {
                                if (doc.IsValid)
                                {
                                    // Open iterator
                                    using (var iterator = doc.CreateIterator())
                                    {
                                        while (iterator.MoveForward())
                                        {
                                            if (
                                                iterator.IsString
                                                && iterator.GetUtf16String() == property
                                            )
                                            {
                                                if (iterator.MoveForward())
                                                    result.Add(iterator.GetUtf16String());
                                            }
                                        }
                                    }
                                }
                            }
                    }
                }
                else
                {
                    try
                    {
                        using (var doc = JsonDocument.Parse(jsonText))
                            FindPropertyValuesNested(doc.RootElement, result, property);
                    }
                    catch { }
                }
            }

            return result;
        }

        // Recursive method to find all the requested property values in any nested structure
        private static void FindPropertyValuesNested(
            JsonElement element,
            List<string> output,
            string property
        )
        {
            // If the element is an object, traverse through its properties
            if (element.ValueKind == JsonValueKind.Object)
            {
                foreach (var nestedProperty in element.EnumerateObject())
                {
                    // If the property name matches the requested property, add it to the output list
                    if (nestedProperty.Name == property)
                        output.Add(nestedProperty.Value.ToString());

                    // Recurse on the value of this property
                    FindPropertyValuesNested(nestedProperty.Value, output, property);
                }
            }
            // If the element is an array, process each item in the array
            else if (element.ValueKind == JsonValueKind.Array)
            {
                foreach (var nestedArrayItem in element.EnumerateArray())
                    FindPropertyValuesNested(nestedArrayItem, output, property);
            }
        }
    }
}
