using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.GetAllClanMessagesResponse)]
    public class MediusGetAllClanMessagesResponse : BaseLobbyMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.GetAllClanMessagesResponse;

        public bool IsSuccess => StatusCode >= 0;

        public MessageId MessageID { get; set; }
        public MediusCallbackStatus StatusCode;
        public int ClanMessageID;
        public string Message; // CLANMSG_MAXLEN
        public MediusClanMessageStatus Status;
        public bool EndOfList;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            // 
            MessageID = reader.Read<MessageId>();
            reader.ReadBytes(3);
            StatusCode = reader.Read<MediusCallbackStatus>();
            ClanMessageID = reader.ReadInt32();
            Message = reader.ReadString(Constants.CLANMSG_MAXLEN);
            Status = reader.Read<MediusClanMessageStatus>();
            EndOfList = reader.ReadBoolean();
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            // 
            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(new byte[3]);
            writer.Write(StatusCode);
            writer.Write(ClanMessageID);
            writer.Write(Message, Constants.CLANMSG_MAXLEN);
            writer.Write(Status);
            writer.Write(EndOfList);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID:{MessageID}" + " " +
                $"StatusCode:{StatusCode}" + " " +
                $"ClanMessageID:{ClanMessageID}" + " " +
                $"Message:{Message}" + " " +
                $"Status:{Status}" + " " +
                $"EndOfList:{EndOfList}";
        }
    }
}
