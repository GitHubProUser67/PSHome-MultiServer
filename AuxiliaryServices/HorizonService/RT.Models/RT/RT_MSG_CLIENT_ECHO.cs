using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [ScertMessage(RT_MSG_TYPE.RT_MSG_CLIENT_ECHO)]
    public class RT_MSG_CLIENT_ECHO : BaseScertMessage
    {
        public override RT_MSG_TYPE Id => RT_MSG_TYPE.RT_MSG_CLIENT_ECHO;

        public byte[] Value;

        public override void Deserialize(MessageReader reader)
        {
            Value = reader.ReadRest();
        }

        public override void Serialize(MessageWriter writer)
        {
            writer.Write(Value);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"Value: {System.BitConverter.ToString(Value)}";
        }

    }
}
