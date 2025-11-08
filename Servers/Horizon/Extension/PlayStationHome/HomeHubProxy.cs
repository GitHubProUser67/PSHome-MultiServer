using CustomLogger;
using EndianTools;
using Horizon.DME.Models;
using Horizon.MUM.Models;
using Horizon.RT.Models;
using Horizon.SERVER;
using MultiServerLibrary.Extension;
using static Horizon.Extension.PlayStationHome.Models.m_Presence;
using static Horizon.SERVER.Medius.BaseMediusComponent;

namespace Horizon.Extension.PlayStationHome
{
    public static class HomeHubProxy
    {
        public static bool ProcessDMEProxyTunneling(byte[] MessagePayload, DMEObject client, ref Action<RT_MSG_CLIENT_APP_SINGLE, DMEObject>? modifyMessagePerClient)
        {
            string? HomeUserEntry = null;
            ClientObject? mumClient = MediusClass.Manager.GetClientBySessionKey(client.SessionKey, client.ApplicationId);

            if (mumClient != null)
                HomeUserEntry = mumClient.AccountName + ":" + mumClient.IP;

            if (MessagePayload.Length > 8)
            {
                int HubPathernOffset = -1;

                foreach (ProtocolVersion version in Enum.GetValues(typeof(ProtocolVersion)))
                {
                    // Only grab the first match.
                    byte versionByte = (byte)version;
                    int offset = ByteUtils.FindBytePattern(MessagePayload, new byte[] { versionByte, 0x00 });

                    if (offset != -1 && MessagePayload.Length >= offset + 8)
                    {
                        HubPathernOffset = offset;
#if DEBUG
                        LoggerAccessor.LogInfo($"[DME] - TcpServer - Found HUB protocol version: {version} at offset {offset}");
#endif
                        modifyMessagePerClient = (msg, client) =>
                        {
                            msg.Payload[offset] = client.mumClient.ProtocolVersion;
                        };
                        break;
                    }
                }

                if (HubPathernOffset != -1) // Hub command.
                {
                    string? value;

                    switch (BitConverter.IsLittleEndian ? EndianUtils.ReverseInt(BitConverter.ToInt32(MessagePayload, HubPathernOffset + 4)) : BitConverter.ToInt32(MessagePayload, HubPathernOffset + 4))
                    {
                        case -85: // IGA
                            if (!string.IsNullOrEmpty(HomeUserEntry) && MediusClass.Settings.PlaystationHomeUsersServersAccessList.TryGetValue(HomeUserEntry, out value) && !string.IsNullOrEmpty(value))
                            {
                                switch (value)
                                {
                                    case "ADMIN":
                                    case "IGA":
                                        break;
                                    default:
                                        LoggerAccessor.LogError($"[DME] - TcpServer - HOME ANTI-CHEAT - DETECTED MALICIOUS USAGE (Reason: UNAUTHORISED IGA COMMAND) - DmeId:{client.DmeId}");
                                        return true;
                                }
                            }
                            else
                            {
                                string SupplementalMessage = "Unknown";

                                switch (MessagePayload[HubPathernOffset + 3]) // TODO, add all the other codes.
                                {
                                    case 0x0B:
                                        SupplementalMessage = "Kick";
                                        break;
                                }

                                LoggerAccessor.LogError($"[DME] - TcpServer - HOME ANTI-CHEAT - DETECTED MALICIOUS USAGE (Reason: UNAUTHORISED IGA COMMAND - {SupplementalMessage}) - DmeId:{client.DmeId}");

                                return true;
                            }
                            break;
                        case -27: // REXEC
                            if (!string.IsNullOrEmpty(HomeUserEntry) && MediusClass.Settings.PlaystationHomeUsersServersAccessList.TryGetValue(HomeUserEntry, out value) && !string.IsNullOrEmpty(value))
                            {
                                switch (value)
                                {
                                    case "ADMIN":
                                        break;
                                    default:
                                        LoggerAccessor.LogError($"[DME] - TcpServer - HOME ANTI-CHEAT - DETECTED MALICIOUS USAGE (Reason: UNAUTHORISED REXEC COMMAND) - DmeId:{client.DmeId}");
                                        return true;
                                }
                            }
                            else
                            {
                                LoggerAccessor.LogError($"[DME] - TcpServer - HOME ANTI-CHEAT - DETECTED MALICIOUS USAGE (Reason: UNAUTHORISED REXEC COMMAND) - DmeId:{client.DmeId}");

                                return true;
                            }
                            break;
                    }
                }
            }

            return false;
        }

        public static Task<bool> ProcessMediusProxyTunneling(ChannelData data, MediusBinaryMessage1 binaryMessage)
        {
            string HomeUserEntry = data.ClientObject!.AccountName + ":" + data.ClientObject.IP;

            if (binaryMessage.MessageSize > 8)
            {
                byte[] HubMessagePayload = binaryMessage.Message;
                int HubPathernOffset = -1;

                foreach (ProtocolVersion version in Enum.GetValues(typeof(ProtocolVersion)))
                {
                    // Only grab the first match.
                    byte versionByte = (byte)version;
                    int offset = ByteUtils.FindBytePattern(HubMessagePayload, new byte[] { versionByte, 0x00 });

                    if (offset != -1 && HubMessagePayload.Length >= offset + 8)
                    {
                        HubPathernOffset = offset;
#if DEBUG
                        LoggerAccessor.LogInfo($"[MLS] - Found HUB protocol version: {version} at offset {offset}");
#endif
                        var target = MediusClass.Manager.GetClientByAccountId(binaryMessage.TargetAccountID, data.ClientObject.ApplicationId);
                        if (target != null)
                            HubMessagePayload[offset] = target.ProtocolVersion;
                        break;
                    }
                }

                if (HubPathernOffset != -1) // Hub command.
                {
                    string? value;

                    switch (BitConverter.IsLittleEndian ? EndianUtils.ReverseInt(BitConverter.ToInt32(HubMessagePayload, HubPathernOffset + 4)) : BitConverter.ToInt32(HubMessagePayload, HubPathernOffset + 4))
                    {
                        case -85: // IGA
                            if (MediusClass.Settings.PlaystationHomeUsersServersAccessList.TryGetValue(HomeUserEntry, out value) && !string.IsNullOrEmpty(value))
                            {
                                switch (value)
                                {
                                    case "ADMIN":
                                    case "IGA":
                                        break;
                                    default:
                                        string anticheatMsg = $"[SECURITY] - HOME ANTI-CHEAT - DETECTED MALICIOUS USAGE (Reason: UNAUTHORISED IGA COMMAND) - User:{HomeUserEntry} CID:{data.MachineId}";

                                        _ = data.ClientObject!.CurrentChannel?.BroadcastSystemMessage(data.ClientObject.CurrentChannel.LocalClients.Where(client => client != data.ClientObject), anticheatMsg, byte.MaxValue);

                                        LoggerAccessor.LogError(anticheatMsg);

                                        return Task.FromResult(true);
                                }
                            }
                            else
                            {
                                string SupplementalMessage = "Unknown";

                                switch (HubMessagePayload[HubPathernOffset + 3]) // TODO, add all the other codes.
                                {
                                    case 0x0B:
                                        SupplementalMessage = "Kick";
                                        break;
                                }

                                string anticheatMsg = $"[SECURITY] - HOME ANTI-CHEAT - DETECTED MALICIOUS USAGE (Reason: UNAUTHORISED IGA COMMAND - {SupplementalMessage}) - User:{HomeUserEntry} CID:{data.MachineId}";

                                _ = data.ClientObject!.CurrentChannel?.BroadcastSystemMessage(data.ClientObject.CurrentChannel.LocalClients.Where(client => client != data.ClientObject), anticheatMsg, byte.MaxValue);

                                LoggerAccessor.LogError(anticheatMsg);

                                return Task.FromResult(true);
                            }
                            break;
                        case -27: // REXEC
                            if (MediusClass.Settings.PlaystationHomeUsersServersAccessList.TryGetValue(HomeUserEntry, out value) && !string.IsNullOrEmpty(value))
                            {
                                switch (value)
                                {
                                    case "ADMIN":
                                        break;
                                    default:
                                        string anticheatMsg = $"[SECURITY] - HOME ANTI-CHEAT - DETECTED MALICIOUS USAGE (Reason: UNAUTHORISED REXEC COMMAND) - User:{HomeUserEntry} CID:{data.MachineId}";

                                        _ = data.ClientObject!.CurrentChannel?.BroadcastSystemMessage(data.ClientObject.CurrentChannel.LocalClients.Where(client => client != data.ClientObject), anticheatMsg, byte.MaxValue);

                                        LoggerAccessor.LogError(anticheatMsg);

                                        return Task.FromResult(true);
                                }
                            }
                            else
                            {
                                string anticheatMsg = $"[SECURITY] - HOME ANTI-CHEAT - DETECTED MALICIOUS USAGE (Reason: UNAUTHORISED REXEC COMMAND) - User:{HomeUserEntry} CID:{data.MachineId}";

                                _ = data.ClientObject!.CurrentChannel?.BroadcastSystemMessage(data.ClientObject.CurrentChannel.LocalClients.Where(client => client != data.ClientObject), anticheatMsg, byte.MaxValue);

                                LoggerAccessor.LogError(anticheatMsg);

                                return Task.FromResult(true);
                            }
                            break;
                    }
                }
            }

            return Task.FromResult(false);
        }
    }
}
