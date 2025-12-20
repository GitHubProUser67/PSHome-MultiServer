using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyReport, MediusMGCLMessageIds.ServerEndGameOnMeResponse)]
    public class MediusServerEndGameOnMeResponse : BaseMGCLMessage, IMediusResponse
    {

        public override byte PacketType => (byte)MediusMGCLMessageIds.ServerEndGameOnMeResponse;

        public MessageId MessageID { get; set; }
        public MGCL_ERROR_CODE Confirmation;

        public bool IsSuccess => Confirmation >= 0;


        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();
            Confirmation = reader.Read<MGCL_ERROR_CODE>();
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(Confirmation);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"Confirmation: {Confirmation}";
        }
    }
}
