using Horizon.MUM.Models;
using MultiServerLibrary.Extension;
using System.Text;

namespace Horizon.SERVER.Extension.PlayStationHome
{
    public static class HomeServerMessage
    {
        internal static byte[] ModeratorMessageCmd = new byte[2] { 0x00, 0x01 };
        internal static byte[] LogOffCmd = new byte[2] { 0x00, 0x02 };
        internal static byte[] SimpleRelocateCmd = new byte[2] { 0x00, 0x03 };
        internal static byte[] AdvancedRelocateCmd = new byte[2] { 0x00, 0x04 };
        internal static byte[] AdminModeCmd = new byte[2] { 0x00, 0x05 };
        internal static byte[] StealthModeCmd = new byte[2] { 0x00, 0x06 };
        internal static byte[] StickUserCmd = new byte[2] { 0x00, 0x07 };
        internal static byte[] FastMoveCmd = new byte[2] { 0x00, 0x08 };
        internal static byte[] ActivateAppCmd = new byte[2] { 0x00, 0x09 };
        internal static byte[] ModeratorBroadcastMessageCmd = BitConverter.GetBytes(BitConverter.IsLittleEndian ? EndianTools.EndianUtils.ReverseUshort(10) : (ushort)10); 

        internal const string RexecMsgPrefix = "ServerMessage ";
        internal const string LcRexecMsgPrefix = "lc Debug.System( 'ServerMessage ";

        private static Task<bool> SendCommand(string targetClientIp, string? accessToken, string? regionCode, byte[] cmd, byte[] payload, bool lc, bool retail)
        {
            if (string.IsNullOrEmpty(regionCode) || regionCode.Length != 4)
                return Task.FromResult(false);

            return HomeRTMTools.SendRemoteCommand(targetClientIp, accessToken, (lc ? LcRexecMsgPrefix : RexecMsgPrefix) + Convert.ToBase64String(ByteUtils.CombineByteArrays(Encoding.ASCII.GetBytes(regionCode), cmd, payload)) + (lc ? "' )" : string.Empty), retail);
        }

        private static Task<bool> SendCommand(ClientObject client, string? regionCode, byte[] cmd, byte[] payload, bool lc)
        {
            if (string.IsNullOrEmpty(regionCode) || regionCode.Length != 4)
                return Task.FromResult(false);

            return HomeRTMTools.SendRemoteCommand(client, (lc ? LcRexecMsgPrefix : RexecMsgPrefix) + Convert.ToBase64String(ByteUtils.CombineByteArrays(Encoding.ASCII.GetBytes(regionCode), cmd, payload)) + (lc ? "' )" : string.Empty));
        }

        private static Task<bool> BroadcastCommand(string? regionCode, byte[] cmd, byte[] payload, bool lc, bool retail)
        {
            if (string.IsNullOrEmpty(regionCode) || regionCode.Length != 4)
                return Task.FromResult(false);

            return HomeRTMTools.BroadcastRemoteCommand((lc ? LcRexecMsgPrefix : RexecMsgPrefix) + Convert.ToBase64String(ByteUtils.CombineByteArrays(Encoding.ASCII.GetBytes(regionCode), cmd, payload)) + (lc ? "' )" : string.Empty), retail);
        }

        private static Task<bool> SendAdminMessageCommand(string targetClientIp, string? accessToken, string? regionCode, int worldId, string payload, bool lc, bool retail)
        {
            if (string.IsNullOrEmpty(regionCode) || regionCode.Length != 4)
                return Task.FromResult(false);

            return HomeRTMTools.SendRemoteCommand(targetClientIp, accessToken, (lc ? LcRexecMsgPrefix : RexecMsgPrefix) + Convert.ToBase64String(ByteUtils.CombineByteArrays(Encoding.ASCII.GetBytes(regionCode),
                ByteUtils.CombineByteArray(ModeratorMessageCmd, BitConverter.IsLittleEndian ? EndianTools.EndianUtils.EndianSwap(BitConverter.GetBytes(worldId)) : BitConverter.GetBytes(worldId)),
                ByteUtils.CombineByteArray("00 44 45 56 00 4D 55 4C 54 49 53 45 52 56 45 52 00".HexStringToByteArray(), ByteUtils.CombineByteArray(Encoding.UTF8.GetBytes(payload), Encoding.ASCII.GetBytes("\0"))))) + (lc ? "' )" : string.Empty), retail);
        }

        private static Task<bool> SendAdminMessageCommand(ClientObject client, string? regionCode, int worldId, string payload, bool lc)
        {
            if (string.IsNullOrEmpty(regionCode) || regionCode.Length != 4)
                return Task.FromResult(false);

            return HomeRTMTools.SendRemoteCommand(client, (lc ? LcRexecMsgPrefix : RexecMsgPrefix) + Convert.ToBase64String(ByteUtils.CombineByteArrays(Encoding.ASCII.GetBytes(regionCode),
                ByteUtils.CombineByteArray(ModeratorMessageCmd, BitConverter.IsLittleEndian ? EndianTools.EndianUtils.EndianSwap(BitConverter.GetBytes(worldId)) : BitConverter.GetBytes(worldId)),
                ByteUtils.CombineByteArray("00 44 45 56 00 4D 55 4C 54 49 53 45 52 56 45 52 00".HexStringToByteArray(), ByteUtils.CombineByteArray(Encoding.UTF8.GetBytes(payload), Encoding.ASCII.GetBytes("\0"))))) + (lc ? "' )" : string.Empty));
        }

        private static Task<bool> BroadcastAdminMessageCommand(string? regionCode, string payload, bool lc, bool retail)
        {
            if (string.IsNullOrEmpty(regionCode) || regionCode.Length != 4)
                return Task.FromResult(false);

            return HomeRTMTools.BroadcastRemoteCommand((lc ? LcRexecMsgPrefix : RexecMsgPrefix) + Convert.ToBase64String(ByteUtils.CombineByteArrays(Encoding.ASCII.GetBytes(regionCode),
                ByteUtils.CombineByteArray(ModeratorBroadcastMessageCmd, "44455600".HexStringToByteArray()), ByteUtils.CombineByteArray("4D 55 4C 54 49 53 45 52 56 45 52 00".HexStringToByteArray(),
                ByteUtils.CombineByteArray(Encoding.UTF8.GetBytes(payload), Encoding.ASCII.GetBytes("\0"))))) + (lc ? "' )" : string.Empty), retail);
        }

        public static Task<bool> SendAdminMessage(string targetClientIp, string? accessToken, string? regionCode, int worldId, string message, bool lc, bool retail)
            => SendAdminMessageCommand(targetClientIp, accessToken, regionCode, worldId, message, lc, retail);

        public static Task<bool> SendAdminMessage(ClientObject client, string? regionCode, int worldId, string message, bool lc)
            => SendAdminMessageCommand(client, regionCode, worldId, message, lc);

        public static Task<bool> BroadcastAdminMessage(string? regionCode, string message, bool lc, bool retail)
            => BroadcastAdminMessageCommand(regionCode, message, lc, retail);

        public static Task<bool> SendLogOffCommand(string targetClientIp, string? accessToken, string? regionCode, byte[] message, bool lc, bool retail)
            => SendCommand(targetClientIp, accessToken, regionCode, LogOffCmd, message, lc, retail);

        public static Task<bool> SendLogOffCommand(ClientObject client, string? regionCode, byte[] message, bool lc)
            => SendCommand(client, regionCode, LogOffCmd, message, lc);

        public static Task<bool> BroadcastLogOffCommand(string? regionCode, byte[] message, bool lc, bool retail)
            => BroadcastCommand(regionCode, LogOffCmd, message, lc, retail);

        public static Task<bool> SendSimpleRelocate(string targetClientIp, string? accessToken, string? regionCode, byte[] message, bool lc, bool retail)
            => SendCommand(targetClientIp, accessToken, regionCode, SimpleRelocateCmd, message, lc, retail);

        public static Task<bool> SendSimpleRelocate(ClientObject client, string? regionCode, byte[] message, bool lc)
            => SendCommand(client, regionCode, SimpleRelocateCmd, message, lc);

        public static Task<bool> BroadcastSimpleRelocate(string? regionCode, byte[] message, bool lc, bool retail)
            => BroadcastCommand(regionCode, SimpleRelocateCmd, message, lc, retail);

        public static Task<bool> SendAdvancedRelocate(string targetClientIp, string? accessToken, string? regionCode, byte[] message, bool lc, bool retail)
            => SendCommand(targetClientIp, accessToken, regionCode, AdvancedRelocateCmd, message, lc, retail);

        public static Task<bool> SendAdvancedRelocate(ClientObject client, string? regionCode, byte[] message, bool lc)
            => SendCommand(client, regionCode, AdvancedRelocateCmd, message, lc);

        public static Task<bool> BroadcastAdvancedRelocate(string? regionCode, byte[] message, bool lc, bool retail)
            => BroadcastCommand(regionCode, AdvancedRelocateCmd, message, lc, retail);

        public static Task<bool> SendAdminModeCommand(string targetClientIp, string? accessToken, string? regionCode, byte[] message, bool lc, bool retail)
            => SendCommand(targetClientIp, accessToken, regionCode, AdminModeCmd, message, lc, retail);

        public static Task<bool> SendAdminModeCommand(ClientObject client, string? regionCode, byte[] message, bool lc)
            => SendCommand(client, regionCode, AdminModeCmd, message, lc);

        public static Task<bool> BroadcastAdminModeCommand(string? regionCode, byte[] message, bool lc, bool retail)
            => BroadcastCommand(regionCode, AdminModeCmd, message, lc, retail);

        public static Task<bool> SendStealthModeCommand(string targetClientIp, string? accessToken, string? regionCode, byte[] message, bool lc, bool retail)
            => SendCommand(targetClientIp, accessToken, regionCode, StealthModeCmd, message, lc, retail);

        public static Task<bool> SendStealthModeCommand(ClientObject client, string? regionCode, byte[] message, bool lc)
            => SendCommand(client, regionCode, StealthModeCmd, message, lc);

        public static Task<bool> BroadcastStealthModeCommand(string? regionCode, byte[] message, bool lc, bool retail)
            => BroadcastCommand(regionCode, StealthModeCmd, message, lc, retail);

        public static Task<bool> SendStickUserCommand(string targetClientIp, string? accessToken, string? regionCode, byte[] message, bool lc, bool retail)
            => SendCommand(targetClientIp, accessToken, regionCode, StickUserCmd, message, lc, retail);

        public static Task<bool> SendStickUserCommand(ClientObject client, string? regionCode, byte[] message, bool lc)
            => SendCommand(client, regionCode, StickUserCmd, message, lc);

        public static Task<bool> BroadcastStickUserCommand(string? regionCode, byte[] message, bool lc, bool retail)
            => BroadcastCommand(regionCode, StickUserCmd, message, lc, retail);

        public static Task<bool> SendFastMoveCommand(string targetClientIp, string? accessToken, string? regionCode, byte[] message, bool lc, bool retail)
            => SendCommand(targetClientIp, accessToken, regionCode, FastMoveCmd, message, lc, retail);

        public static Task<bool> SendFastMoveCommand(ClientObject client, string? regionCode, byte[] message, bool lc)
            => SendCommand(client, regionCode, FastMoveCmd, message, lc);

        public static Task<bool> BroadcastFastMoveCommand(string? regionCode, byte[] message, bool lc, bool retail)
            => BroadcastCommand(regionCode, FastMoveCmd, message, lc, retail);

        public static Task<bool> SendActivateAppCommand(string targetClientIp, string? accessToken, string? regionCode, byte[] message, bool lc, bool retail)
            => SendCommand(targetClientIp, accessToken, regionCode, ActivateAppCmd, message, lc, retail);

        public static Task<bool> SendActivateAppCommand(ClientObject client, string? regionCode, byte[] message, bool lc)
            => SendCommand(client, regionCode, ActivateAppCmd, message, lc);

        public static Task<bool> BroadcastActivateAppCommand(string? regionCode, byte[] message, bool lc, bool retail)
            => BroadcastCommand(regionCode, ActivateAppCmd, message, lc, retail);

        public static Task<bool> SendModeratorBroadcastMessage(string targetClientIp, string? accessToken, string? regionCode, byte[] message, bool lc, bool retail)
            => SendCommand(targetClientIp, accessToken, regionCode, ModeratorBroadcastMessageCmd, message, lc, retail);

        public static Task<bool> SendModeratorBroadcastMessage(ClientObject client, string? regionCode, byte[] message, bool lc)
            => SendCommand(client, regionCode, ModeratorBroadcastMessageCmd, message, lc);

        public static Task<bool> BroadcastModeratorBroadcastMessage(string? regionCode, byte[] message, bool lc, bool retail)
            => BroadcastCommand(regionCode, ModeratorBroadcastMessageCmd, message, lc, retail);
    }

}
