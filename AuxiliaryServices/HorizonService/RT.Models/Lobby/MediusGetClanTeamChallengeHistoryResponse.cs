using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.GetClanTeamChallengeHistoryResponse)]
    public class MediusGetClanTeamChallengeHistoryResponse : BaseLobbyMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.GetClanTeamChallengeHistoryResponse;

        public bool IsSuccess => StatusCode >= 0;

        public MessageId MessageID { get; set; }
        public MediusCallbackStatus StatusCode;
        public int AgainstClanID;
        public MediusClanChallengeStatus Status;
        public char EndOfList;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            // 
            MessageID = reader.Read<MessageId>();
            reader.ReadBytes(3);
            StatusCode = reader.Read<MediusCallbackStatus>();
            AgainstClanID = reader.ReadInt32();
            Status = reader.Read<MediusClanChallengeStatus>();
            EndOfList = reader.ReadChar();
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            // 
            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(new byte[3]);
            writer.Write(StatusCode);
            writer.Write(AgainstClanID);
            writer.Write(Status);
            writer.Write(EndOfList);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID:{MessageID}" + " " +
                $"StatusCode:{StatusCode}" + " " +
                $"AgainstClanID:{AgainstClanID}" + " " +
                $"Status:{Status}" + " " +
                $"EndOfList:{EndOfList}";
        }
    }
}
