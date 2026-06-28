using EndianTools;
using Horizon.LIBRARY.Common.Stream;
using Horizon.RT.Common;

namespace Horizon.RT.Models
{
    [ScertMessage(RT_MSG_TYPE.RT_MSG_CLIENT_APP_TO_PLUGIN)]
    public class RT_MSG_CLIENT_APP_TO_PLUGIN : BaseScertMessage
    {
        public override RT_MSG_TYPE Id => RT_MSG_TYPE.RT_MSG_CLIENT_APP_TO_PLUGIN;

        public BaseMediusPluginMessage Message { get; set; } = null;

        public override void Deserialize(MessageReader reader)
        {
            Message = BaseMediusPluginMessage.InstantiateClientPlugin(reader);
        }

#if DEBUG
        private static readonly bool debug = true;
#else
        private static bool debug = false;
#endif

        public override void Serialize(MessageWriter writer)
        {
            if (Message != null)
            {
                var buffer = new byte[3];
                EndianAwareConverter.WriteUInt24(
                    buffer,
                    Endianness.BigEndian,
                    0,
                    Message.Size
                );
                var buffer1 = new byte[2];
                EndianAwareConverter.WriteUInt16(
                    buffer1,
                    Endianness.BigEndian,
                    0,
                    (ushort)Message.PacketType
                );
                writer.Write(buffer, buffer.Length);
                writer.Write(buffer1, buffer1.Length);
                Message.SerializePlugin(writer);
            }
        }

        public override bool CanLog()
        {
            return base.CanLog();
        }

        public override string ToString()
        {
            return base.ToString() + " " + $"Message: {Message}";
        }
    }
}
