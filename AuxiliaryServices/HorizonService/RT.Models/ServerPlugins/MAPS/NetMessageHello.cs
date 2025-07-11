using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;
using Horizon.RT.Models;

namespace HorizonService.RT.Models.ServerPlugins.MAPS
{
    [MediusMessage(NetMessageClass.MessageClassApplication, NetMessageTypeIds.NetMessageTypeHello)]
    public class NetMessageHello : BaseApplicationMessage
    {
        public override NetMessageTypeIds PacketType => NetMessageTypeIds.NetMessageTypeHello;

        public override byte IncomingMessage => 0;
        public override int Size => 0;

        public override byte PluginId => 0;

        public override void DeserializePlugin(MessageReader reader)
        {

        }

        public override void SerializePlugin(MessageWriter writer)
        {

        }

        public override string ToString()
        {
            return base.ToString() + " ";
        }
    }
}