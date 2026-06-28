using Horizon.LIBRARY.Common.Stream;
using Horizon.RT.Common;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.AccountGetIDResponse)]
    public class MediusAccountGetIDResponse : BaseLobbyMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.AccountGetIDResponse;

        public bool IsSuccess => StatusCode >= 0;

        public MessageId MessageID { get; set; }

        public int AccountID;
        public MediusCallbackStatus StatusCode;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();

            reader.ReadBytes(3);
            AccountID = reader.ReadInt32();
            StatusCode = reader.Read<MediusCallbackStatus>();
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);

            writer.Write(new byte[3]);
            writer.Write(AccountID);
            writer.Write(StatusCode);
        }

        public override string ToString()
        {
            return base.ToString()
                + " "
                + $"MessageID:{MessageID} "
                + $"AccountID:{AccountID} "
                + $"StatusCode:{StatusCode}";
        }
    }
}
