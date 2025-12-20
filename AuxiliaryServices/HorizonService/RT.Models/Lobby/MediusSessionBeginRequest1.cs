using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.SessionBegin1)]
    public class MediusSessionBeginRequest1 : BaseLobbyExtMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.SessionBegin1;

        public MessageId MessageID { get; set; }

        public MediusConnectionType ConnectionClass;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();

            // 
            reader.ReadBytes(3);
            ConnectionClass = reader.Read<MediusConnectionType>();
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            writer.Write(MessageID ?? MessageId.Empty);

            // 
            writer.Write(new byte[3]);
            writer.Write(ConnectionClass);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"ConnectionClass: {ConnectionClass}";
        }
    }
}
