using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyReport, MediusMGCLMessageIds.ServerMoveGameWorldOnMeResponse)]
    public class MediusServerMoveGameWorldOnMeResponse : BaseMGCLMessage, IMediusResponse
    {

        public override byte PacketType => (byte)MediusMGCLMessageIds.ServerMoveGameWorldOnMeResponse;

        public MessageId MessageID { get; set; }
        public MGCL_ERROR_CODE Confirmation;
        public int MediusWorldID;

        public bool IsSuccess => Confirmation >= 0;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();
            Confirmation = reader.Read<MGCL_ERROR_CODE>();
            reader.ReadBytes(2);
            MediusWorldID = reader.ReadInt32();
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(Confirmation);
            writer.Write(new byte[2]);
            writer.Write(MediusWorldID);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"Confirmation: {Confirmation} " +
                $"MediusWorldID: {MediusWorldID}";
        }
    }
}
