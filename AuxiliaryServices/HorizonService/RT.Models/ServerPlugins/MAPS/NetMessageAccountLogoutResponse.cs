using Horizon.LIBRARY.Common.Stream;
using Horizon.RT.Common;

namespace Horizon.RT.Models.ServerPlugins.MAPS
{
    [MediusMessage(
        NetMessageClass.MessageClassApplication,
        NetMessageTypeIds.NetMessageTypeAccountLogoutResponse
    )]
    public class NetMessageAccountLogoutResponse : BaseMediusPluginMessage
    {
        public override NetMessageTypeIds PacketType =>
            NetMessageTypeIds.NetMessageTypeAccountLogoutResponse;

        public override int Size => 4;
        public override ushort ClientBufferSize => 8;
        public override byte PluginId => (byte)NetPluginType.kNetPluginMAPS;

        public bool m_success;

        public override void DeserializePlugin(MessageReader reader)
        {
            m_success = reader.ReadBoolean();
            reader.ReadBytes(3);
        }

        public override void SerializePlugin(MessageWriter writer)
        {
            writer.Write(m_success);
            writer.Write(new byte[3]);
        }

        public override string ToString()
        {
            return base.ToString() + " " + $"m_success: {m_success}";
        }
    }
}
