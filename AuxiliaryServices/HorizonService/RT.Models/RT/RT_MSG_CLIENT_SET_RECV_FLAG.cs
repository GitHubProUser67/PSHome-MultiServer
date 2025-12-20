using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [ScertMessage(RT_MSG_TYPE.RT_MSG_CLIENT_SET_RECV_FLAG)]
    public class RT_MSG_CLIENT_SET_RECV_FLAG : BaseScertMessage
    {
        public override RT_MSG_TYPE Id => RT_MSG_TYPE.RT_MSG_CLIENT_SET_RECV_FLAG;

        public RT_RECV_FLAG Flag { get; set; }

        public override void Deserialize(MessageReader reader)
        {
            if (reader.MediusVersion <= 108)
            {
                var bytes = reader.ReadBytes(2);
                Flag = RT_RECV_FLAG.RECV_LIST | RT_RECV_FLAG.RECV_SINGLE | RT_RECV_FLAG.RECV_NOTIFICATION;
                if (bytes[1] == 1)
                    Flag |= RT_RECV_FLAG.RECV_BROADCAST;
            }
            else
                Flag = reader.Read<RT_RECV_FLAG>();
        }

        public override void Serialize(MessageWriter writer)
        {
            if (writer.MediusVersion <= 108)
            {
                writer.Write((byte)2);
                writer.Write(Flag.HasFlag(RT_RECV_FLAG.RECV_BROADCAST));
            }
            else
                writer.Write(Flag);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"Flag: {Flag}";
        }
    }
}
