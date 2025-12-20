using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;
using System.Net;

namespace Horizon.RT.Models
{
    [ScertMessage(RT_MSG_TYPE.RT_MSG_SERVER_DISCONNECT_NOTIFY)]
    public class RT_MSG_SERVER_DISCONNECT_NOTIFY : BaseScertMessage
    {

        public override RT_MSG_TYPE Id => RT_MSG_TYPE.RT_MSG_SERVER_DISCONNECT_NOTIFY;

        public short PlayerIndex;
        public short ScertId;
        public short UNK_04 = 0;
        public IPAddress IP = IPAddress.Any;

        public override void Deserialize(MessageReader reader)
        {
            if (reader.MediusVersion <= 108)
            {
                PlayerIndex = reader.ReadInt16();
                IP = reader.Read<IPAddress>();
            }
            else
            {
                PlayerIndex = reader.ReadInt16();
                ScertId = reader.ReadInt16();
                UNK_04 = reader.ReadInt16();
                IP = reader.Read<IPAddress>();
            }
        }

        public override void Serialize(MessageWriter writer)
        {
            if (writer.MediusVersion <= 108)
            {
                writer.Write(PlayerIndex);
                writer.Write(IP ?? IPAddress.Any);
            }
            else
            {
                writer.Write(PlayerIndex);
                writer.Write(ScertId);
                writer.Write(UNK_04);
                writer.Write(IP ?? IPAddress.Any);
            }
        }
    }
}
