using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [ScertMessage(RT_MSG_TYPE.RT_MSG_SERVER_FORCED_DISCONNECT)]
    public class RT_MSG_SERVER_FORCED_DISCONNECT : BaseScertMessage
    {
        public override RT_MSG_TYPE Id => RT_MSG_TYPE.RT_MSG_SERVER_FORCED_DISCONNECT;

        public SERVER_FORCE_DISCONNECT_REASON Reason;

        public override void Deserialize(MessageReader reader)
        {
            Reason = reader.Read<SERVER_FORCE_DISCONNECT_REASON>();
        }

        public override void Serialize(MessageWriter writer)
        {
            writer.Write(Reason);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"Reason: {Reason}";
        }
    }
}
