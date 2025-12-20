using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [ScertMessage(RT_MSG_TYPE.RT_MSG_SERVER_APP)]
    public class RT_MSG_SERVER_APP : BaseScertMessage
    {
        public override RT_MSG_TYPE Id => RT_MSG_TYPE.RT_MSG_SERVER_APP;

        public BaseMediusMessage Message { get; set; } = null;

        public override bool SkipEncryption
        {
            get => Message?.SkipEncryption ?? base.SkipEncryption;
            set
            {
                if (Message != null) { Message.SkipEncryption = value; }
                base.SkipEncryption = value;
            }
        }

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
            return base.ToString() + " " +
                $"Message: {Message}";
        }
    }
}
