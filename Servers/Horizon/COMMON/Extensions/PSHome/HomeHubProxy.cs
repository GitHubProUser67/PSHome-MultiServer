using CustomLogger;
using EndianTools;
using Horizon.DME.Models;
using Horizon.MEDIUS.Models;
using Horizon.MUM.Models;
using Horizon.RT.Models;
using Horizon.PlaystationHomePlugin.Models;
using static Horizon.PlaystationHomePlugin.Models.m_Presence;

namespace Horizon.COMMON.Extensions.PSHome
{
    public static class HomeHubProxy
    {
        public static bool ProcessDMEProxyTunneling(
            byte[] MessagePayload,
            DMEObject client,
            ref Action<RT_MSG_CLIENT_APP_SINGLE, DMEObject>? modifyMessagePerClient
        )
        {
            string? HomeUserEntry = null;
            var mumClient = Program.MUMManager.GetClientBySessionKey(
                client.SessionKey,
                client.ApplicationId
            );

            if (mumClient != null)
                HomeUserEntry = mumClient.AccountName + ":" + mumClient.IP;

            if (MessagePayload.Length > 8) // Header size
            {
                using (HubMessage mesg = new HubMessage(MessagePayload))
                {
                    if (mesg.MessageId != -1 && mesg.MessageDestinationType != -1)
                    {
                        foreach (ProtocolVersion version in Enum.GetValues<ProtocolVersion>())
                        {
                            if (EndianUtils.ReverseShort(mesg.ExtraInfo1) == (byte)version)
                            {
#if DEBUG
                                LoggerAccessor.LogInfo(
                                    $"[DME] - Found HUB protocol version: {version}"
                                );
#endif
                                // match found
                                modifyMessagePerClient = (msg, client) =>
                                {
                                    // Only modify if we have the protocol version.
                                    if (client.mumClient.ProtocolVersion != byte.MaxValue)
                                        msg.Payload[6] = client.mumClient.ProtocolVersion;
                                };
                            }
                        }

                        short subPacketSize = mesg.DecodeNextShort();

                        if (subPacketSize != -1)
                        {
                            byte[] subPacketData = mesg.DecodeNextRawData(subPacketSize);

                            string? value;
                            var messageId = -1;
                            try
                            {
                                messageId = EndianAwareConverter.ToInt32(
                                    subPacketData,
                                    Endianness.BigEndian,
                                    0
                                );
                            }
                            catch
                            {
                                // Sometimes, Home sends short packets (kinda like a UDP system) if the data is not needed.
                            }
                            var reservedHubMessageId = (ReservedHubMessageId)messageId;

                            switch (reservedHubMessageId)
                            {
                                case ReservedHubMessageId.HUB_ONLINE_MSG_IGA_FUNCTION:
                                    if (
                                        !string.IsNullOrEmpty(HomeUserEntry)
                                        && HorizonServerConfiguration.MEDIUSPlaystationHomeUsersServersAccessList.TryGetValue(
                                            HomeUserEntry,
                                            out value
                                        )
                                        && !string.IsNullOrEmpty(value)
                                    )
                                    {
                                        switch (value)
                                        {
                                            case "ADMIN":
                                            case "IGA":
                                                break;
                                            default:
                                                LoggerAccessor.LogError(
                                                    $"[DME] - HOME ANTI-CHEAT - DETECTED MALICIOUS USAGE (Reason: UNAUTHORISED IGA COMMAND) - DmeId:{client.DmeId}"
                                                );

                                                return true;
                                        }
                                    }
                                    else
                                    {
                                        LoggerAccessor.LogError(
                                            $"[DME] - HOME ANTI-CHEAT - DETECTED MALICIOUS USAGE (Reason: UNAUTHORISED IGA COMMAND) - DmeId:{client.DmeId}"
                                        );

                                        return true;
                                    }
                                    break;
                                case ReservedHubMessageId.HUB_ONLINE_MSG_COMMON_REXEC:
                                    if (
                                        !string.IsNullOrEmpty(HomeUserEntry)
                                        && HorizonServerConfiguration.MEDIUSPlaystationHomeUsersServersAccessList.TryGetValue(
                                            HomeUserEntry,
                                            out value
                                        )
                                        && !string.IsNullOrEmpty(value)
                                    )
                                    {
                                        switch (value)
                                        {
                                            case "ADMIN":
                                                break;
                                            default:
                                                LoggerAccessor.LogError(
                                                    $"[DME] - HOME ANTI-CHEAT - DETECTED MALICIOUS USAGE (Reason: UNAUTHORISED REXEC COMMAND) - DmeId:{client.DmeId}"
                                                );

                                                return true;
                                        }
                                    }
                                    else
                                    {
                                        LoggerAccessor.LogError(
                                            $"[DME] - HOME ANTI-CHEAT - DETECTED MALICIOUS USAGE (Reason: UNAUTHORISED REXEC COMMAND) - DmeId:{client.DmeId}"
                                        );

                                        return true;
                                    }
                                    break;
                                default:
                                    if (Enum.IsDefined(typeof(ReservedHubMessageId), messageId))
                                    {
#if DEBUG
                                        LoggerAccessor.LogInfo(
                                            $"[DME] - ReservedHubMessageId: {reservedHubMessageId}"
                                        );
#endif
                                    }
                                    else
                                        LoggerAccessor.LogWarn(
                                            $"[DME] - Unknown HubMessageId: {messageId}"
                                        );

                                    break;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public static Task<bool> ProcessMediusProxyTunneling(
            ChannelData data,
            MediusBinaryMessage1 binaryMessage
        )
        {
            var HomeUserEntry = data.ClientObject!.AccountName + ":" + data.ClientObject.IP;

            if (binaryMessage.MessageSize > 8) // Header size
            {
                var HubMessagePayload = binaryMessage.Message;

                using (HubMessage mesg = new HubMessage(HubMessagePayload))
                {
                    if (mesg.MessageId != -1 && mesg.MessageDestinationType != -1)
                    {
                        foreach (ProtocolVersion version in Enum.GetValues<ProtocolVersion>())
                        {
                            if (EndianUtils.ReverseShort(mesg.ExtraInfo1) == (byte)version)
                            {
#if DEBUG
                                LoggerAccessor.LogInfo(
                                    $"[MLS] - Found HUB protocol version: {version}"
                                );
#endif
                                // match found
                                var target = Program.MUMManager.GetClientByAccountId(
                                    binaryMessage.TargetAccountID,
                                    data.ClientObject.ApplicationId
                                );
                                if (target != null && target.ProtocolVersion != byte.MaxValue)
                                    HubMessagePayload[6] = target.ProtocolVersion;
                                break;
                            }
                        }

                        short subPacketSize = mesg.DecodeNextShort();

                        if (subPacketSize != -1)
                        {
                            byte[] subPacketData = mesg.DecodeNextRawData(subPacketSize);

                            string? value;
                            var messageId = -1;
                            try
                            {
                                messageId = EndianAwareConverter.ToInt32(
                                    subPacketData,
                                    Endianness.BigEndian,
                                    0
                                );
                            }
                            catch
                            {
                                // Sometimes, Home sends short packets (kinda like a UDP system) if the data is not needed.
                            }
                            var reservedHubMessageId = (ReservedHubMessageId)messageId;

                            switch (reservedHubMessageId)
                            {
                                case ReservedHubMessageId.HUB_ONLINE_MSG_IGA_FUNCTION:
                                    if (
                                        HorizonServerConfiguration.MEDIUSPlaystationHomeUsersServersAccessList.TryGetValue(
                                            HomeUserEntry,
                                            out value
                                        ) && !string.IsNullOrEmpty(value)
                                    )
                                    {
                                        switch (value)
                                        {
                                            case "ADMIN":
                                            case "IGA":
                                                break;
                                            default:
                                                var anticheatMsg =
                                                    $"[MLS] - HOME ANTI-CHEAT - DETECTED MALICIOUS USAGE (Reason: UNAUTHORISED IGA COMMAND) - User:{HomeUserEntry} CID:{data.MachineId}";

                                                _ = Channel.BroadcastSystemMessage(
                                                    data.ClientObject.CurrentChannel.LocalClients.Where(
                                                        client => client != data.ClientObject
                                                    ),
                                                    anticheatMsg,
                                                    byte.MaxValue
                                                );

                                                LoggerAccessor.LogError(anticheatMsg);

                                                return Task.FromResult(true);
                                        }
                                    }
                                    else
                                    {
                                        var anticheatMsg =
                                            $"[MLS] - HOME ANTI-CHEAT - DETECTED MALICIOUS USAGE (Reason: UNAUTHORISED IGA COMMAND) - User:{HomeUserEntry} CID:{data.MachineId}";

                                        _ = Channel.BroadcastSystemMessage(
                                            data.ClientObject.CurrentChannel.LocalClients.Where(
                                                client => client != data.ClientObject
                                            ),
                                            anticheatMsg,
                                            byte.MaxValue
                                        );

                                        LoggerAccessor.LogError(anticheatMsg);

                                        return Task.FromResult(true);
                                    }
                                    break;
                                case ReservedHubMessageId.HUB_ONLINE_MSG_COMMON_REXEC:
                                    if (
                                        HorizonServerConfiguration.MEDIUSPlaystationHomeUsersServersAccessList.TryGetValue(
                                            HomeUserEntry,
                                            out value
                                        ) && !string.IsNullOrEmpty(value)
                                    )
                                    {
                                        switch (value)
                                        {
                                            case "ADMIN":
                                                break;
                                            default:
                                                var anticheatMsg =
                                                    $"[MLS] - HOME ANTI-CHEAT - DETECTED MALICIOUS USAGE (Reason: UNAUTHORISED REXEC COMMAND) - User:{HomeUserEntry} CID:{data.MachineId}";

                                                _ = Channel.BroadcastSystemMessage(
                                                    data.ClientObject.CurrentChannel.LocalClients.Where(
                                                        client => client != data.ClientObject
                                                    ),
                                                    anticheatMsg,
                                                    byte.MaxValue
                                                );

                                                LoggerAccessor.LogError(anticheatMsg);

                                                return Task.FromResult(true);
                                        }
                                    }
                                    else
                                    {
                                        var anticheatMsg =
                                            $"[MLS] - HOME ANTI-CHEAT - DETECTED MALICIOUS USAGE (Reason: UNAUTHORISED REXEC COMMAND) - User:{HomeUserEntry} CID:{data.MachineId}";

                                        _ = Channel.BroadcastSystemMessage(
                                            data.ClientObject.CurrentChannel.LocalClients.Where(
                                                client => client != data.ClientObject
                                            ),
                                            anticheatMsg,
                                            byte.MaxValue
                                        );

                                        LoggerAccessor.LogError(anticheatMsg);

                                        return Task.FromResult(true);
                                    }
                                    break;
                                default:
                                    if (Enum.IsDefined(typeof(ReservedHubMessageId), messageId))
                                    {
#if DEBUG
                                        LoggerAccessor.LogInfo(
                                            $"[MLS] - ReservedHubMessageId: {reservedHubMessageId}"
                                        );
#endif
                                    }
                                    else
                                        LoggerAccessor.LogWarn(
                                            $"[MLS] - Unknown HubMessageId: {messageId}"
                                        );

                                    break;
                            }
                        }
                    }
                }
            }

            return Task.FromResult(false);
        }
    }
}
