using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyReport, MediusMGCLMessageIds.ServerEndGameOnMeRequest)]
    public class MediusServerEndGameOnMeRequest : BaseMGCLMessage, IMediusRequest
    {

        public override byte PacketType => (byte)MediusMGCLMessageIds.ServerEndGameOnMeRequest;

        public MessageId MessageID { get; set; }
        public int MediusWorldID;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();
            reader.ReadBytes(3);
            MediusWorldID = reader.ReadInt32();
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(new byte[3]);
            writer.Write(MediusWorldID);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"MediusWorldID: {MediusWorldID}";
        }
    }
}
