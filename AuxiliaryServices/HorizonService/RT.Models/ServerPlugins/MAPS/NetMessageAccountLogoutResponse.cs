using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;
using Horizon.RT.Models;

namespace HorizonService.RT.Models.ServerPlugins.MAPS
{
    [MediusMessage(NetMessageClass.MessageClassApplication, NetMessageTypeIds.NetMessageTypeAccountLogoutResponse)]
    public class NetMessageAccountLogoutResponse : BaseApplicationMessage
    {
        public override NetMessageTypeIds PacketType => NetMessageTypeIds.NetMessageTypeAccountLogoutResponse;

        public override byte IncomingMessage => 0;
        public override int Size => 4;

        public override byte PluginId => 0;

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
            return base.ToString() + " " +
                $"m_success: {m_success}";
        }

    }
}
