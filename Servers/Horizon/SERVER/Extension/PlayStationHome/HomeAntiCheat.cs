using CustomLogger;
using MultiServerLibrary.Extension;
using DotNetty.Transport.Channels;
using EndianTools;
using Horizon.MUM.Models;
using Horizon.RT.Common;
using Horizon.RT.Models;
using static Horizon.SERVER.Medius.BaseMediusComponent;

namespace Horizon.SERVER.Extension.PlayStationHome
{
    public class HomeAntiCheat
    {
        #region Anticheat References

        private static byte[] Ref1 = "j1S4yArhxE6OW2ZPQzq+oA==".IsBase64().Item2;

        private static byte[] Ref2 = "XtKRFh5cJ/iW3RzTcyHa8g==".IsBase64().Item2;

        private static byte[] Ref3 = "VDQrkh5H7aV9T/GbaDwNUw==".IsBase64().Item2;

        private static byte[] Ref4 = "FA5bx20s0liKa36ROeQ8Lw==".IsBase64().Item2;

        private static byte[] Ref5 = "gjhutOUMfyuOPC5gjtt9/Q==".IsBase64().Item2;

        private static byte[] Ref6 = "AAAAAAAAAAE=".IsBase64().Item2;

        private static List<byte[]> ForceInviteRefs = new List<byte[]> { 
            "B4A08C784F3097477A95BAE6ED2360B8".HexStringToByteArray(),
            "5FB36D34F9C08B51C653602A9A1E0EE2".HexStringToByteArray(),
            "3190194C117C510E0414DF77B4BEE615".HexStringToByteArray(),
            "090276717FA70EAEBCA71218898D226F".HexStringToByteArray(),
        };

        #endregion

        protected virtual Task QueueBanMessage(ChannelData? data, string msg = "You have been caught cheating!")
        {
            // Send ban message
            data?.SendQueue.Enqueue(new RT_MSG_SERVER_SYSTEM_MESSAGE()
            {
                Severity = (byte)MediusClass.GetAppSettingsOrDefault(data.ApplicationId).BanSystemMessageSeverity,
                EncodingType = DME_SERVER_ENCODING_TYPE.DME_SERVER_ENCODING_UTF8,
                LanguageType = DME_SERVER_LANGUAGE_TYPE.DME_SERVER_LANGUAGE_US_ENGLISH,
                EndOfMessage = true,
                Message = msg
            });

            return Task.CompletedTask;
        }

        public async void ProcessAntiCheatQuery(ChannelData data, RT_MSG_SERVER_CHEAT_QUERY clientCheatQuery, IChannel? clientChannel)
        {
            // client channel is null, don't process
            if (clientChannel == null)
                return;

            // clientCheatQuery.Data is null, don't process
            if (clientCheatQuery.Data == null)
                return;

            ClientObject? client = data.ClientObject;
            byte[] QueryData = clientCheatQuery.Data;

            if (client != null && client.ClientHomeData != null)
            {
                switch (client.ClientHomeData.Type)
                {
                    case "HDK With Offline":
                        switch (client.ClientHomeData.Version)
                        {
                            case "01.86.09":
                                switch (clientCheatQuery.StartAddress)
                                {
                                    case 0x101590b0:
                                        if (clientCheatQuery.QueryType == CheatQueryType.DME_SERVER_CHEAT_QUERY_SHA1_HASH && (QueryData.Length != 16 || !QueryData.EqualsTo(Ref3)))
                                        {
                                            string anticheatMsg = $"[SECURITY] - HOME ANTI-CHEAT - DETECTED MALICIOUS USAGE (Reason: FREEZE ATTEMPT) - User:{client.IP + ":" + client.AccountName} CID:{data.MachineId}";

                                            _ = client.CurrentChannel?.BroadcastSystemMessage(client.CurrentChannel.LocalClients.Where(x => x != client), anticheatMsg, byte.MaxValue);

                                            LoggerAccessor.LogError(anticheatMsg);

                                            await HorizonServerConfiguration.Database.BanIp(client.IP).ContinueWith((r) =>
                                            {
                                                if (r.IsCompletedSuccessfully && r.Result)
                                                {
                                                    // Banned
                                                    QueueBanMessage(data);
                                                }
                                                client.ForceDisconnect();
                                                _ = client.Logout();
                                            });
                                        }
                                        break;
                                    case 0x0016cb68:
                                        if (clientCheatQuery.QueryType == CheatQueryType.DME_SERVER_CHEAT_QUERY_SHA1_HASH && (QueryData.Length != 16 || !QueryData.EqualsTo(ForceInviteRefs[0])))
                                        {
                                            string anticheatMsg = $"[SECURITY] - HOME ANTI-CHEAT - DETECTED MALICIOUS USAGE (Reason: FORCE INVITE BYPASS ATTEMPT) - User:{client.IP + ":" + client.AccountName} CID:{data.MachineId}";

                                            _ = client.CurrentChannel?.BroadcastSystemMessage(client.CurrentChannel.LocalClients.Where(x => x != client), anticheatMsg, byte.MaxValue);

                                            LoggerAccessor.LogError(anticheatMsg);

                                            await HorizonServerConfiguration.Database.BanIp(client.IP).ContinueWith((r) =>
                                            {
                                                if (r.IsCompletedSuccessfully && r.Result)
                                                {
                                                    // Banned
                                                    QueueBanMessage(data);
                                                }
                                                client.ForceDisconnect();
                                                _ = client.Logout();
                                            });
                                        }
                                        break;
                                    case 0x10480d30:
                                        if (clientCheatQuery.QueryType == CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY && QueryData.Length == 4)
                                        {
                                            if (client.Hub_LocalPerson_ms_pInstance == 0)
                                            {
                                                client.SetPointer(BitConverter.ToUInt32(BitConverter.IsLittleEndian ? EndianUtils.ReverseArray(QueryData) : QueryData));

                                                client.TryAddTask("1.86 HDK ANTI FREEZE", () =>
                                                {
                                                    while (true)
                                                    {
                                                        if (client.IsInGame)
                                                        {
                                                            CheatQuery(client.Hub_LocalPerson_ms_pInstance + 5300U, 8, client, CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY);
                                                            CheatQuery(client.Hub_LocalPerson_ms_pInstance + 6928U, 84, client, CheatQueryType.DME_SERVER_CHEAT_QUERY_SHA1_HASH);

                                                            Thread.Sleep(3500);
                                                        }
                                                        else
                                                            Thread.Sleep(6000);
                                                    }

                                                });
                                            }
                                        }
                                        break;
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    case "HDK Online Only":
                        switch (client.ClientHomeData.Version)
                        {
                            default:
                                break;
                        }
                        break;
                    case "HDK Online Only (Dbg Symbols)":
                        switch (client.ClientHomeData.Version)
                        {
                            case "01.82.09":
                                switch (clientCheatQuery.StartAddress)
                                {
                                    case 0x0016b490:
                                        if (clientCheatQuery.QueryType == CheatQueryType.DME_SERVER_CHEAT_QUERY_SHA1_HASH && (QueryData.Length != 16 || !QueryData.EqualsTo(ForceInviteRefs[1])))
                                        {
                                            string anticheatMsg = $"[SECURITY] - HOME ANTI-CHEAT - DETECTED MALICIOUS USAGE (Reason: FORCE INVITE BYPASS ATTEMPT) - User:{client.IP + ":" + client.AccountName} CID:{data.MachineId}";

                                            _ = client.CurrentChannel?.BroadcastSystemMessage(client.CurrentChannel.LocalClients.Where(x => x != client), anticheatMsg, byte.MaxValue);

                                            LoggerAccessor.LogError(anticheatMsg);

                                            await HorizonServerConfiguration.Database.BanIp(client.IP).ContinueWith((r) =>
                                            {
                                                if (r.IsCompletedSuccessfully && r.Result)
                                                {
                                                    // Banned
                                                    QueueBanMessage(data);
                                                }
                                                client.ForceDisconnect();
                                                _ = client.Logout();
                                            });
                                        }
                                        break;
                                    case 0x10470990:
                                        if (clientCheatQuery.QueryType == CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY && QueryData.Length == 4)
                                        {
                                            if (client.Hub_LocalPerson_ms_pInstance == 0)
                                            {
                                                client.SetPointer(BitConverter.ToUInt32(BitConverter.IsLittleEndian ? EndianUtils.ReverseArray(QueryData) : QueryData));

                                                client.TryAddTask("1.82 HDK Online (Dbg Symbols) ANTI FREEZE", () =>
                                                {
                                                    while (true)
                                                    {
                                                        if (client.IsInGame)
                                                        {
                                                            CheatQuery(client.Hub_LocalPerson_ms_pInstance + 5300U, 8, client, CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY);
                                                            CheatQuery(client.Hub_LocalPerson_ms_pInstance + 6928U, 84, client, CheatQueryType.DME_SERVER_CHEAT_QUERY_SHA1_HASH);

                                                            Thread.Sleep(3500);
                                                        }
                                                        else
                                                            Thread.Sleep(6000);
                                                    }

                                                });
                                            }
                                        }
                                        break;
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    case "Online Debug":
                        switch (client.ClientHomeData.Version)
                        {
                            case "01.83.12":
                                switch (clientCheatQuery.StartAddress)
                                {
                                    case 0x001709a0:
                                        if (clientCheatQuery.QueryType == CheatQueryType.DME_SERVER_CHEAT_QUERY_SHA1_HASH && (QueryData.Length != 16 || !QueryData.EqualsTo(ForceInviteRefs[2])))
                                        {
                                            string anticheatMsg = $"[SECURITY] - HOME ANTI-CHEAT - DETECTED MALICIOUS USAGE (Reason: FORCE INVITE BYPASS ATTEMPT) - User:{client.IP + ":" + client.AccountName} CID:{data.MachineId}";

                                            _ = client.CurrentChannel?.BroadcastSystemMessage(client.CurrentChannel.LocalClients.Where(x => x != client), anticheatMsg, byte.MaxValue);

                                            LoggerAccessor.LogError(anticheatMsg);

                                            await HorizonServerConfiguration.Database.BanIp(client.IP).ContinueWith((r) =>
                                            {
                                                if (r.IsCompletedSuccessfully && r.Result)
                                                {
                                                    // Banned
                                                    QueueBanMessage(data);
                                                }
                                                client.ForceDisconnect();
                                                _ = client.Logout();
                                            });
                                        }
                                        break;
                                    case 0x104809f0:
                                        if (clientCheatQuery.QueryType == CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY && QueryData.Length == 4)
                                        {
                                            if (client.Hub_LocalPerson_ms_pInstance == 0)
                                            {
                                                client.SetPointer(BitConverter.ToUInt32(BitConverter.IsLittleEndian ? EndianUtils.ReverseArray(QueryData) : QueryData));

                                                client.TryAddTask("1.83 QA ANTI FREEZE", () =>
                                                {
                                                    while (true)
                                                    {
                                                        if (client.IsInGame)
                                                        {
                                                            CheatQuery(client.Hub_LocalPerson_ms_pInstance + 5300U, 8, client, CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY);
                                                            CheatQuery(client.Hub_LocalPerson_ms_pInstance + 6928U, 84, client, CheatQueryType.DME_SERVER_CHEAT_QUERY_SHA1_HASH);

                                                            Thread.Sleep(3500);
                                                        }
                                                        else
                                                            Thread.Sleep(6000);
                                                    }

                                                });
                                            }
                                        }
                                        break;
                                }
                                break;
                            case "01.86.09":
                                switch (clientCheatQuery.StartAddress)
                                {
                                    case 0x0016da80:
                                        if (clientCheatQuery.QueryType == CheatQueryType.DME_SERVER_CHEAT_QUERY_SHA1_HASH && (QueryData.Length != 16 || !QueryData.EqualsTo(ForceInviteRefs[3])))
                                        {
                                            string anticheatMsg = $"[SECURITY] - HOME ANTI-CHEAT - DETECTED MALICIOUS USAGE (Reason: FORCE INVITE BYPASS ATTEMPT) - User:{client.IP + ":" + client.AccountName} CID:{data.MachineId}";

                                            _ = client.CurrentChannel?.BroadcastSystemMessage(client.CurrentChannel.LocalClients.Where(x => x != client), anticheatMsg, byte.MaxValue);

                                            LoggerAccessor.LogError(anticheatMsg);

                                            await HorizonServerConfiguration.Database.BanIp(client.IP).ContinueWith((r) =>
                                            {
                                                if (r.IsCompletedSuccessfully && r.Result)
                                                {
                                                    // Banned
                                                    QueueBanMessage(data);
                                                }
                                                client.ForceDisconnect();
                                                _ = client.Logout();
                                            });
                                        }
                                        break;
                                    case 0x10480b58:
                                        if (clientCheatQuery.QueryType == CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY && QueryData.Length == 4)
                                        {
                                            if (client.Hub_LocalPerson_ms_pInstance == 0)
                                            {
                                                client.SetPointer(BitConverter.ToUInt32(BitConverter.IsLittleEndian ? EndianUtils.ReverseArray(QueryData) : QueryData));

                                                client.TryAddTask("1.86 QA ANTI FREEZE", () =>
                                                {
                                                    while (true)
                                                    {
                                                        if (client.IsInGame)
                                                        {
                                                            CheatQuery(client.Hub_LocalPerson_ms_pInstance + 5300U, 8, client, CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY);
                                                            CheatQuery(client.Hub_LocalPerson_ms_pInstance + 6928U, 84, client, CheatQueryType.DME_SERVER_CHEAT_QUERY_SHA1_HASH);

                                                            Thread.Sleep(3500);
                                                        }
                                                        else
                                                            Thread.Sleep(6000);
                                                    }

                                                });
                                            }
                                        }
                                        break;
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    case "Retail":
                        switch (client.ClientHomeData.Version)
                        {
                            case "01.86.09":
                                switch (clientCheatQuery.StartAddress)
                                {
                                    case 268759069U:
                                        if (clientCheatQuery.QueryType == CheatQueryType.DME_SERVER_CHEAT_QUERY_SHA1_HASH && QueryData.Length == 16 && (QueryData.EqualsTo(Ref1)
                                            || QueryData.EqualsTo(Ref2) || QueryData.EqualsTo(Ref4)))
                                        {
                                            string anticheatMsg = $"[SECURITY] - HOME ANTI-CHEAT - DETECTED MALICIOUS USAGE (Reason: UNAUTHORIZED TOOL USAGE) - User:{client.IP + ":" + client.AccountName} CID:{data.MachineId}";

                                            _ = client.CurrentChannel?.BroadcastSystemMessage(client.CurrentChannel.LocalClients.Where(x => x != client), anticheatMsg, byte.MaxValue);

                                            LoggerAccessor.LogError(anticheatMsg);

                                            await HorizonServerConfiguration.Database.BanIp(client.IP).ContinueWith((r) =>
                                            {
                                                if (r.IsCompletedSuccessfully && r.Result)
                                                {
                                                    // Banned
                                                    QueueBanMessage(data);
                                                }
                                                client.ForceDisconnect();
                                                _ = client.Logout();
                                            });
                                        }
                                        break;
                                    case 0x100BA820:
                                        if (clientCheatQuery.QueryType == CheatQueryType.DME_SERVER_CHEAT_QUERY_SHA1_HASH && (QueryData.Length != 16 || !QueryData.EqualsTo(Ref3)))
                                        {
                                            string anticheatMsg = $"[SECURITY] - HOME ANTI-CHEAT - DETECTED MALICIOUS USAGE (Reason: FREEZE ATTEMPT) - User:{client.IP + ":" + client.AccountName} CID:{data.MachineId}";

                                            _ = client.CurrentChannel?.BroadcastSystemMessage(client.CurrentChannel.LocalClients.Where(x => x != client), anticheatMsg, byte.MaxValue);

                                            LoggerAccessor.LogError(anticheatMsg);

                                            await HorizonServerConfiguration.Database.BanIp(client.IP).ContinueWith((r) =>
                                            {
                                                if (r.IsCompletedSuccessfully && r.Result)
                                                {
                                                    // Banned
                                                    QueueBanMessage(data);
                                                }
                                                client.ForceDisconnect();
                                                _ = client.Logout();
                                            });
                                        }
                                        break;
                                    case 0x104F7320:
                                        if (clientCheatQuery.QueryType == CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY && QueryData.Length == 4)
                                        {
                                            if (client.Hub_LocalPerson_ms_pInstance == 0)
                                            {
                                                client.SetPointer(BitConverter.ToUInt32(BitConverter.IsLittleEndian ? EndianUtils.ReverseArray(QueryData) : QueryData));

                                                client.TryAddTask("1.86 Retail ANTI FREEZE", () =>
                                                {
                                                    while (true)
                                                    {
                                                        if (client.IsInGame)
                                                        {
                                                            CheatQuery(client.Hub_LocalPerson_ms_pInstance + 5300U, 8, client, CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY);
                                                            CheatQuery(client.Hub_LocalPerson_ms_pInstance + 6928U, 84, client, CheatQueryType.DME_SERVER_CHEAT_QUERY_SHA1_HASH);
															
                                                            Thread.Sleep(3500);
                                                        }
                                                        else
                                                            Thread.Sleep(6000);
                                                    }

                                                });
                                            }
                                        }
                                        break;
                                    default:
                                        if (client != null)
                                        {
                                            if (clientCheatQuery.StartAddress == client.Hub_LocalPerson_ms_pInstance + 6928U && clientCheatQuery.QueryType == CheatQueryType.DME_SERVER_CHEAT_QUERY_SHA1_HASH && QueryData.Length == 16 && QueryData.EqualsTo(Ref5))
                                            {
                                                string anticheatMsg = $"[SECURITY] - HOME ANTI-CHEAT - DETECTED MALICIOUS USAGE (Reason: LAG FREEZE ATTEMPT) - User:{client.IP + ":" + client.AccountName} CID:{data.MachineId}";

                                                _ = client.CurrentChannel?.BroadcastSystemMessage(client.CurrentChannel.LocalClients.Where(x => x != client), anticheatMsg, byte.MaxValue);

                                                LoggerAccessor.LogError(anticheatMsg);

                                                await HorizonServerConfiguration.Database.BanIp(client.IP).ContinueWith((r) =>
                                                {
                                                    if (r.IsCompletedSuccessfully && r.Result)
                                                    {
                                                        // Banned
                                                        QueueBanMessage(data);
                                                    }
                                                    client.ForceDisconnect();
                                                    _ = client.Logout();
                                                });
                                            }
                                            else if (clientCheatQuery.StartAddress == client.Hub_LocalPerson_ms_pInstance + 5300U && clientCheatQuery.QueryType == CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY && QueryData.Length == 8 && QueryData.EqualsTo(Ref6))
                                            {
                                                string anticheatMsg = $"[SECURITY] - HOME ANTI-CHEAT - DETECTED MALICIOUS USAGE (Reason: FREEZE ATTEMPT) - User:{client.IP + ":" + client.AccountName} CID:{data.MachineId}";

                                                _ = client.CurrentChannel?.BroadcastSystemMessage(client.CurrentChannel.LocalClients.Where(x => x != client), anticheatMsg, byte.MaxValue);

                                                LoggerAccessor.LogError(anticheatMsg);

                                                await HorizonServerConfiguration.Database.BanIp(client.IP).ContinueWith((r) =>
                                                {
                                                    if (r.IsCompletedSuccessfully && r.Result)
                                                    {
                                                        // Banned
                                                        QueueBanMessage(data);
                                                    }
                                                    client.ForceDisconnect();
                                                    _ = client.Logout();
                                                });
                                            }
                                        }
                                        break;
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                }
            }
        }

        public List<(uint, int, CheatQueryType, int)> ProcessAntiCheatRequest(ClientObject? client, string HomeUserEntry)
        {
            List<(uint, int, CheatQueryType, int)> results = new();

            if (client?.ClientHomeData != null)
            {
                switch (client.ClientHomeData.Type)
                {
                    case "HDK With Offline":
                        switch (client.ClientHomeData.Version)
                        {
                            case "01.86.09":
                                results.Add((0x101590b0, 20, CheatQueryType.DME_SERVER_CHEAT_QUERY_SHA1_HASH, 1));
                                if (MediusClass.Settings.PlaystationHomeForceInviteExploitPatch)
                                    results.Add((0x0016cb68, 364, CheatQueryType.DME_SERVER_CHEAT_QUERY_SHA1_HASH, 1));
                                results.Add((0x10480d30, 4, CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY, 1));
                                break;
                            default:
                                break;
                        }
                        break;
                    case "HDK Online Only":
                        switch (client.ClientHomeData.Version)
                        {
                            default:
                                break;
                        }
                        break;
                    case "HDK Online Only (Dbg Symbols)":
                        switch (client.ClientHomeData.Version)
                        {
                            case "01.82.09":
                                if (MediusClass.Settings.PlaystationHomeForceInviteExploitPatch)
                                    results.Add((0x0016b490, 172, CheatQueryType.DME_SERVER_CHEAT_QUERY_SHA1_HASH, 1));
                                results.Add((0x10470990, 4, CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY, 1));
                                break;
                            default:
                                break;
                        }
                        break;
                    case "Online Debug":
                        switch (client.ClientHomeData.Version)
                        {
                            case "01.83.12":
                                if (MediusClass.Settings.PlaystationHomeForceInviteExploitPatch)
                                    results.Add((0x001709a0, 168, CheatQueryType.DME_SERVER_CHEAT_QUERY_SHA1_HASH, 1));
                                results.Add((0x104809f0, 4, CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY, 1));
                                break;
                            case "01.86.09":
                                if (MediusClass.Settings.PlaystationHomeForceInviteExploitPatch)
                                    results.Add((0x0016da80, 168, CheatQueryType.DME_SERVER_CHEAT_QUERY_SHA1_HASH, 1));
                                results.Add((0x10480b58, 4, CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY, 1));
                                break;
                            default:
                                break;
                        }
                        break;
                    case "Retail":
                        switch (client.ClientHomeData.Version)
                        {
                            case "01.86.09":
                                if (MediusClass.Settings.PlaystationHomeUsersServersAccessList.TryGetValue(HomeUserEntry, out string? value) && !string.IsNullOrEmpty(value))
                                {
                                    switch (value)
                                    {
                                        case "RTM":
                                            break;
                                        default:
                                            results.Add((268759069U, 27, CheatQueryType.DME_SERVER_CHEAT_QUERY_SHA1_HASH, 1));
                                            break;
                                    }
                                }
                                else
                                    results.Add((268759069U, 27, CheatQueryType.DME_SERVER_CHEAT_QUERY_SHA1_HASH, 1));
                                results.Add((0x100BA820, 20, CheatQueryType.DME_SERVER_CHEAT_QUERY_SHA1_HASH, 1));
                                results.Add((0x104F7320, 4, CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY, 1));
                                break;
                            default:
                                break;
                        }
                        break;
                }
            }

            return results;
        }

        #region PokeEngine
        private bool CheatQuery(uint address, int Length, ClientObject client, CheatQueryType Type = CheatQueryType.DME_SERVER_CHEAT_QUERY_RAW_MEMORY, int SequenceId = 1)
        {
            // address = 0, don't read
            if (address == 0)
                return false;

            // read client memory
            client.Queue(new RT_MSG_SERVER_CHEAT_QUERY()
            {
                QueryType = Type,
                SequenceId = SequenceId,
                StartAddress = address,
                Length = Length,
            });

            // return read
            return true;
        }

        private bool PokeAddress(uint patchLocation, byte[] Payload, ClientObject client)
        {
            // patch location = 0, don't patch
            if (patchLocation == 0)
                return false;

            // poke client memory
            client.Queue(new RT_MSG_SERVER_MEMORY_POKE()
            {
                start_Address = patchLocation,
                Payload = Payload,
                SkipEncryption = false,
            });

            // return patched
            return true;
        }
        #endregion
    }
}
