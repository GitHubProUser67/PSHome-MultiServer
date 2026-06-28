using Horizon.LIBRARY.Common.Stream;
using Horizon.RT.Common;

namespace Horizon.RT.Models.ServerPlugins.MAPS
{
    [MediusMessage(NetMessageClass.MessageClassApplication, NetMessageTypeIds.NetMessageTypeHello)]
    public class NetMessageHello : BaseMediusPluginMessage
    {
        public override NetMessageTypeIds PacketType => NetMessageTypeIds.NetMessageTypeHello;

        public override int Size => 0;
        public override ushort ClientBufferSize => 0;
        public override byte PluginId => (byte)NetPluginType.kNetPluginMAPS;

        public override void DeserializePlugin(MessageReader reader) { }

        public override void SerializePlugin(MessageWriter writer) { }

        public override string ToString()
        {
            return base.ToString() + " ";
        }
    }
}
