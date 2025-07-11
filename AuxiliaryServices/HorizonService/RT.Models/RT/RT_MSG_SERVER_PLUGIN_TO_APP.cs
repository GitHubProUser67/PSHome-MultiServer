using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;
using System;
using EndianTools.ZipperEndian;
using EndianTools;

namespace Horizon.RT.Models
{
    [ScertMessage(RT_MSG_TYPE.RT_MSG_SERVER_PLUGIN_TO_APP)]
    public class RT_MSG_SERVER_PLUGIN_TO_APP : BaseScertMessage
    {
        public override RT_MSG_TYPE Id => RT_MSG_TYPE.RT_MSG_SERVER_PLUGIN_TO_APP;

        public BaseMediusPluginMessage Message { get; set; } = null;

        public override bool SkipEncryption
        {
            get => Message?.SkipEncryption ?? base.SkipEncryption;
            set
            {
                if (Message != null) { Message.SkipEncryption = value; }
                base.SkipEncryption = value;
            }
        }
#if DEBUG
        private static bool debug = true;
#else
        private static bool debug = false;
#endif
        public override void Deserialize(MessageReader reader)
        {
            Message = BaseMediusPluginMessage.InstantiateServerPlugin(reader);
        }

        public override void Serialize(MessageWriter writer)
        {
            if (Message != null)
            {
                byte[] buffer = new byte[2];
                EndianAwareConverter.WriteUInt16(buffer, Endianness.BigEndian, 0, (ushort)Message.Size);
                byte[] buffer1 = new byte[2];
                EndianAwareConverter.WriteUInt16(buffer1, Endianness.BigEndian, 0, (ushort)Message.PacketType);
                writer.Write(Message.IncomingMessage);
                writer.Write(buffer, buffer.Length);
                writer.Write(Message.PluginId);
                writer.Write(new byte[2]);
                writer.Write(buffer1, buffer1.Length);
                Message.SerializePlugin(writer);
            }
        }

        public override bool CanLog()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"Message: {Message}";
        }
    }
}
