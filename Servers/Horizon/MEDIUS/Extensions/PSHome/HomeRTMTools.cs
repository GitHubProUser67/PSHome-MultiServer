using System.Text;
using CastleLibrary.Utils;
using CustomLogger;
using Horizon.MUM.Models;
using Horizon.PlaystationHomePlugin.Models;
using Horizon.RT.Common;
using Horizon.RT.Models;
using MultiServerLibrary.Extension;

namespace Horizon.MEDIUS.Extensions.PSHome
{
    public static class HomeRTMTools
    {
        private static readonly List<string> ForbiddenWords = new() { "rexec", "ping" };

        public static Task<bool> SendRemoteCommand(
            string targetClientIp,
            string? AccessToken,
            string command,
            bool Retail
        )
        {
            if (
                string.IsNullOrEmpty(command)
                || command.Length > ushort.MaxValue
                || (
                    !command.StartsWith("say", StringComparison.InvariantCultureIgnoreCase)
                    && ForbiddenWords.Any(x =>
                        command.Contains(x, StringComparison.InvariantCultureIgnoreCase)
                    )
                )
            )
                return Task.FromResult(false);

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
                var HubRexecMessage = ByteUtils.CombineByteArrays(
                    new byte[2],
                    new byte[][]
                    {
                        BitConverter.GetBytes(
                            EndianTools.EndianAwareConverter.isLittleEndianSystem
                                ? EndianTools.EndianUtils.ReverseUshort(
                                    (ushort)(command.Length + 9)
                                )
                                : (ushort)(command.Length + 9)
                        ),
                        "FFFFFFE5FFFFFFFF".HexStrToBytes(),
                        EnsureMultipleOfEight(
                            ByteUtils.CombineByteArray(
                                Encoding.UTF8.GetBytes(command),
                                Encoding.ASCII.GetBytes("\0")
                            )
                        ),
                    }
                );

                clients.ForEach(client =>
                {
                    var message = (byte[])HubRexecMessage.Clone();
                    if (client.ProtocolVersion != byte.MaxValue)
                        message[0] = client.ProtocolVersion;

                    client.Queue(
                        new MediusBinaryFwdMessage1
                        {
                            MessageID = new MessageId("o"),
                            MessageType = MediusBinaryMessageType.TargetBinaryMsg,
                            OriginatorAccountID = client.AccountId,
                            MessageSize = message.Length,
                            Message = message,
                        }
                    );
                });

                return Task.FromResult(true);
            }

            LoggerAccessor.LogError(
                $"[HomeRTMTools] - {(!AccessTokenProvided ? $"Ip:{targetClientIp}" : $"AccessToken:{AccessToken}")} didn't return any Medius clients!"
            );

            return Task.FromResult(false);
        }

        public static Task<bool> SendRemoteCommand(ClientObject client, string command)
        {
            if (
                string.IsNullOrEmpty(command)
                || command.Length > ushort.MaxValue
                || (
                    !command.StartsWith("say", StringComparison.InvariantCultureIgnoreCase)
                    && ForbiddenWords.Any(x =>
                        x.Contains(command, StringComparison.InvariantCultureIgnoreCase)
                    )
                )
            )
                return Task.FromResult(false);

            var HubRexecMessage = ByteUtils.CombineByteArrays(
                new byte[2]
                {
                    client.ProtocolVersion != byte.MaxValue
                        ? client.ProtocolVersion
                        : (byte)m_Presence.ProtocolVersion.X64,
                    0x00,
                },
                new byte[][]
                {
                    BitConverter.GetBytes(
                        EndianTools.EndianAwareConverter.isLittleEndianSystem
                            ? EndianTools.EndianUtils.ReverseUshort((ushort)(command.Length + 9))
                            : (ushort)(command.Length + 9)
                    ),
                    "FFFFFFE5FFFFFFFF".HexStrToBytes(),
                    EnsureMultipleOfEight(
                        ByteUtils.CombineByteArray(
                            Encoding.UTF8.GetBytes(command),
                            Encoding.ASCII.GetBytes("\0")
                        )
                    ),
                }
            );

            client.Queue(
                new MediusBinaryFwdMessage1()
                {
                    MessageID = new MessageId("o"),
                    MessageType = MediusBinaryMessageType.TargetBinaryMsg,
                    OriginatorAccountID = client.AccountId,
                    MessageSize = HubRexecMessage.Length,
                    Message = HubRexecMessage,
                }
            );

            return Task.FromResult(true);
        }

        public static Task<bool> BroadcastRemoteCommand(string command, bool Retail)
        {
            if (
                string.IsNullOrEmpty(command)
                || command.Length > ushort.MaxValue
                || (
                    !command.StartsWith("say", StringComparison.InvariantCultureIgnoreCase)
                    && ForbiddenWords.Any(x =>
                        x.Contains(command, StringComparison.InvariantCultureIgnoreCase)
                    )
                )
            )
                return Task.FromResult(false);

            Action<MediusBinaryFwdMessage1, ClientObject>? modifyMessagePerClient = (msg, client) =>
            {
                if (client.ProtocolVersion != byte.MaxValue)
                    msg.Message[0] = client.ProtocolVersion;
            };

            var HubRexecMessage = ByteUtils.CombineByteArrays(
                new byte[2],
                new byte[][]
                {
                    BitConverter.GetBytes(
                        EndianTools.EndianAwareConverter.isLittleEndianSystem
                            ? EndianTools.EndianUtils.ReverseUshort((ushort)(command.Length + 9))
                            : (ushort)(command.Length + 9)
                    ),
                    "FFFFFFE5FFFFFFFF".HexStrToBytes(),
                    EnsureMultipleOfEight(
                        ByteUtils.CombineByteArray(
                            Encoding.UTF8.GetBytes(command),
                            Encoding.ASCII.GetBytes("\0")
                        )
                    ),
                }
            );

            foreach (var channel in Program.MUMManager.GetAllChannels(Retail ? 20374 : 20371))
            {
                _ = channel.BroadcastDirectBinaryMessage(
                    new MediusBinaryFwdMessage1()
                    {
                        MessageID = new MessageId("o"),
                        MessageType = MediusBinaryMessageType.TargetBinaryMsg,
                        OriginatorAccountID = 95481,
                        MessageSize = HubRexecMessage.Length,
                        Message = HubRexecMessage,
                    },
                    modifyMessagePerClient
                );
            }

            return Task.FromResult(true);
        }

        private static byte[] EnsureMultipleOfEight(byte[] input)
        {
            var length = input.Length;
            var remainder = length % 8;

            if (remainder == 0)
                return input; // Already a multiple of 8

            var paddedArray = new byte[length + (8 - remainder)];

            Array.Copy(input, paddedArray, length);

            return paddedArray;
        }
    }
}
