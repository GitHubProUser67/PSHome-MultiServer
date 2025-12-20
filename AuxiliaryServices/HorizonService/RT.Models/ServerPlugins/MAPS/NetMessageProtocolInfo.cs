using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;
using EndianTools.ZipperEndian;
using Horizon.RT.Models;

namespace HorizonService.RT.Models.ServerPlugins.MAPS
{
    [MediusMessage(NetMessageClass.MessageClassApplication, NetMessageTypeIds.NetMessageTypeProtocolInfo)]
    public class NetMessageProtocolInfo : BaseApplicationMessage
    {
        public override NetMessageTypeIds PacketType => NetMessageTypeIds.NetMessageTypeProtocolInfo;

        public override byte IncomingMessage => 0;
        public override int Size => 5;

        public override byte PluginId => 31;

        public uint protocolInfo;
        public uint buildNumber;
#if DEBUG
        private static bool debug = true;
#else
        private static bool debug = false;
#endif
        public override void DeserializePlugin(MessageReader reader)
        {
            int BitIndex = 0;
            byte[] buffer = reader.ReadBytes(8);
            BufferImpl.ReadPrimitive(buffer, ref protocolInfo, ref BitIndex, debug);
            BufferImpl.ReadPrimitive(buffer, ref buildNumber, ref BitIndex, debug);
        }

        public override void SerializePlugin(MessageWriter writer)
        {
            int BitIndex = 0;
            byte[] buffer = new byte[8];
            BufferImpl.WritePrimitive(buffer, protocolInfo, ref BitIndex, debug);
            BufferImpl.WritePrimitive(buffer, buildNumber, ref BitIndex, debug);
            writer.Write(buffer, buffer.Length);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"protocolInfo: {protocolInfo} " +
                $"buildNumber: {buildNumber}";
        }
    }
}
