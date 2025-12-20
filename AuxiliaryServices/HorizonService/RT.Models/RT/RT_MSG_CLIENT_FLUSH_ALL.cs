using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [ScertMessage(RT_MSG_TYPE.RT_MSG_CLIENT_FLUSH_ALL)]
    public  class RT_MSG_CLIENT_FLUSH_ALL : BaseScertMessage 
    {
        public override RT_MSG_TYPE Id => RT_MSG_TYPE.RT_MSG_CLIENT_FLUSH_ALL;

        public byte[] Payload;

        public override void Deserialize(MessageReader reader)
        {
            Payload = reader.ReadRest();
        }

        public override void Serialize(MessageWriter writer)
        {
            writer.Write(Payload);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"Contents: {System.BitConverter.ToString(Payload)}";
        }
    }
}
