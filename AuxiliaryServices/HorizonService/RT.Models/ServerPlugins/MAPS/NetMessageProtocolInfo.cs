using EndianTools.ZipperEndian;
using Horizon.LIBRARY.Common.Stream;
using Horizon.RT.Common;

namespace Horizon.RT.Models.ServerPlugins.MAPS
{
    [MediusMessage(
        NetMessageClass.MessageClassApplication,
        NetMessageTypeIds.NetMessageTypeProtocolInfo
    )]
    public class NetMessageProtocolInfo : BaseMediusPluginMessage
    {
        public override NetMessageTypeIds PacketType =>
            NetMessageTypeIds.NetMessageTypeProtocolInfo;

        public override int Size => 8;
        public override ushort ClientBufferSize => 16;
        public override byte PluginId => (byte)NetPluginType.kNetPluginMAPS;

        public uint protocolVersion;
        public uint buildNumber;
#if DEBUG
        private static readonly bool debug = true;
#else
        private static bool debug = false;
#endif

        public override void DeserializePlugin(MessageReader reader)
        {
            var BitIndex = 0;
            var buffer = reader.ReadBytes(Size);
            BufferImpl.ReadPrimitive(buffer, ref protocolVersion, ref BitIndex, debug);
            BufferImpl.ReadPrimitive(buffer, ref buildNumber, ref BitIndex, debug);
        }

        public override void SerializePlugin(MessageWriter writer)
        {
            var BitIndex = 0;
            var buffer = new byte[Size];
            BufferImpl.WritePrimitive(buffer, protocolVersion, ref BitIndex, debug);
            BufferImpl.WritePrimitive(buffer, buildNumber, ref BitIndex, debug);
            writer.Write(buffer, buffer.Length);
        }

        public override string ToString()
        {
            return base.ToString()
                + " "
                + $"protocolInfo: {protocolVersion} "
                + $"buildNumber: {buildNumber}";
        }
    }
}
