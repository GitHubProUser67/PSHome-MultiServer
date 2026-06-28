using Horizon.LIBRARY.Common.Stream;
using Horizon.RT.Common;

namespace Horizon.RT.Models
{
    [ScertMessage(RT_MSG_TYPE.RT_MSG_CLIENT_APP_TOSERVER)]
    public class RT_MSG_CLIENT_APP_TOSERVER : BaseScertMessage
    {
        public override RT_MSG_TYPE Id => RT_MSG_TYPE.RT_MSG_CLIENT_APP_TOSERVER;

        public BaseMediusMessage Message { get; set; } = null;

        public override void Deserialize(MessageReader reader)
        {
            Message = BaseMediusMessage.Instantiate(reader);
        }

        public override void Serialize(MessageWriter writer)
        {
            if (Message != null)
            {
                writer.Write(Message.PacketClass);
                writer.Write(Message.PacketType);
                Message.Serialize(writer);
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
            return base.ToString() + " " + $"Message:{Message}";
        }
    }
}
