using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyReport, MediusMGCLMessageIds.ServerCreateGameWithAttributesResponse)]
    public class MediusServerCreateGameWithAttributesResponse : BaseMGCLMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusMGCLMessageIds.ServerCreateGameWithAttributesResponse;

        public MessageId MessageID { get; set; }
        public MGCL_ERROR_CODE Confirmation;
        public int MediusWorldId;

        public bool IsSuccess => Confirmation >= 0;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();
            Confirmation = reader.Read<MGCL_ERROR_CODE>();
            reader.ReadBytes(2);
            MediusWorldId = reader.ReadInt32();
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(Confirmation);
            writer.Write(new byte[2]);
            writer.Write(MediusWorldId);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"Confirmation: {Confirmation} " +
                $"WorldID: {MediusWorldId}";
        }
    }
}
