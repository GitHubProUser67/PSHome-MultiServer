using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;
using EndianTools.ZipperEndian;
using System;
using EndianTools;

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
        private static bool debug = true;
#else
        private static bool debug = false;
#endif
        public override void Serialize(MessageWriter writer)
        {
            if (Message != null)
            {
                byte[] buffer = new byte[3];
                buffer[0] = (byte)((Message.Size >> 16) & byte.MaxValue);
                buffer[1] = (byte)((Message.Size >> 8) & byte.MaxValue);
                buffer[2] = (byte)(Message.Size & byte.MaxValue);
                byte[] buffer1 = new byte[2];
                EndianAwareConverter.WriteUInt16(buffer1, Endianness.BigEndian, 0, (ushort)Message.PacketType);
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
            return base.ToString() + " " +
                $"Message: {Message}";
        }
    }
}
