using CustomLogger;
using MultiServerLibrary.Extension;
using Horizon.MUM.Models;
using Horizon.RT.Common;
using Horizon.RT.Models;
using System.Text;

namespace Horizon.SERVER.Extension.PlayStationHome
{
    public static class HomeRTMTools
    {
        private static readonly byte[] RexecHubMessageHeader = new byte[] { 0x64, 0x00 };

        private static List<string> ForbiddenWords = new() { "rexec", "ping" };

        public static Task<bool> SendRemoteCommand(string targetClientIp, string? AccessToken, string command, bool Retail)
        {
            if (string.IsNullOrEmpty(command) || command.Length > ushort.MaxValue || (!command.StartsWith("say", StringComparison.InvariantCultureIgnoreCase) &&
                 ForbiddenWords.Any(x => command.Contains(x, StringComparison.InvariantCultureIgnoreCase))))
                return Task.FromResult(false);

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
                byte[] HubRexecMessage = ByteUtils.CombineByteArrays(RexecHubMessageHeader, new byte[][] { BitConverter.GetBytes(BitConverter.IsLittleEndian ? EndianTools.EndianUtils.ReverseUshort((ushort)(command.Length + 9)) : (ushort)(command.Length + 9))
                    , "FFFFFFE5FFFFFFFF".HexStringToByteArray(), EnsureMultipleOfEight( ByteUtils.CombineByteArray(Encoding.UTF8.GetBytes(command), Encoding.ASCII.GetBytes("\0"))) });

                clients.ForEach(x => x.Queue(new MediusBinaryFwdMessage1()
                {
                    MessageID = new MessageId("o"),
                    MessageType = MediusBinaryMessageType.TargetBinaryMsg,
                    OriginatorAccountID = x.AccountId,
                    MessageSize = HubRexecMessage.Length,
                    Message = HubRexecMessage
                }));

                return Task.FromResult(true);
            }

            LoggerAccessor.LogError($"[HomeRTMTools] - {(!AccessTokenProvided ? $"Ip:{targetClientIp}" : $"AccessToken:{AccessToken}")} didn't return any Medius clients!");

            return Task.FromResult(false);
        }

        public static Task<bool> SendRemoteCommand(ClientObject client, string command)
        {
            if (string.IsNullOrEmpty(command) || command.Length > ushort.MaxValue || (!command.StartsWith("say", StringComparison.InvariantCultureIgnoreCase) && ForbiddenWords.Any(x => x.Contains(command, StringComparison.InvariantCultureIgnoreCase))))
                return Task.FromResult(false);

            byte[] HubRexecMessage = ByteUtils.CombineByteArrays(RexecHubMessageHeader, new byte[][] { BitConverter.GetBytes(BitConverter.IsLittleEndian ? EndianTools.EndianUtils.ReverseUshort((ushort)(command.Length + 9)) : (ushort)(command.Length + 9))
                    , "FFFFFFE5FFFFFFFF".HexStringToByteArray(), EnsureMultipleOfEight( ByteUtils.CombineByteArray(Encoding.UTF8.GetBytes(command), Encoding.ASCII.GetBytes("\0"))) });

            client.Queue(new MediusBinaryFwdMessage1()
            {
                MessageID = new MessageId("o"),
                MessageType = MediusBinaryMessageType.TargetBinaryMsg,
                OriginatorAccountID = client.AccountId,
                MessageSize = HubRexecMessage.Length,
                Message = HubRexecMessage
            });

            return Task.FromResult(true);
        }

        public static Task<bool> BroadcastRemoteCommand(string command, bool Retail)
        {
            if (string.IsNullOrEmpty(command) || command.Length > ushort.MaxValue || (!command.StartsWith("say", StringComparison.InvariantCultureIgnoreCase) && ForbiddenWords.Any(x => x.Contains(command, StringComparison.InvariantCultureIgnoreCase))))
                return Task.FromResult(false);

            byte[] HubRexecMessage = ByteUtils.CombineByteArrays(RexecHubMessageHeader, new byte[][] { BitConverter.GetBytes(BitConverter.IsLittleEndian ? EndianTools.EndianUtils.ReverseUshort((ushort)(command.Length + 9)) : (ushort)(command.Length + 9))
                    , "FFFFFFE5FFFFFFFF".HexStringToByteArray(), EnsureMultipleOfEight( ByteUtils.CombineByteArray(Encoding.UTF8.GetBytes(command), Encoding.ASCII.GetBytes("\0"))) });

            foreach (Channel channel in MediusClass.Manager.GetAllChannels(Retail ? 20374 : 20371))
            {
                _ = channel.BroadcastDirectBinaryMessage(new MediusBinaryFwdMessage1()
                {
                    MessageID = new MessageId("o"),
                    MessageType = MediusBinaryMessageType.TargetBinaryMsg,
                    OriginatorAccountID = 95481,
                    MessageSize = HubRexecMessage.Length,
                    Message = HubRexecMessage
                });
            }

            return Task.FromResult(true);
        }

        private static byte[] EnsureMultipleOfEight(byte[] input)
        {
            int length = input.Length;
            int remainder = length % 8;

            if (remainder == 0)
                return input; // Already a multiple of 8

            byte[] paddedArray = new byte[length + (8 - remainder)];

            Array.Copy(input, paddedArray, length);

            return paddedArray;
        }
    }
}
