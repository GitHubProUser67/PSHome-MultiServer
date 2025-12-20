using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.GetMyClanMessagesResponse)]
    public class MediusGetMyClanMessagesResponse : BaseLobbyMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.GetMyClanMessagesResponse;

        public bool IsSuccess => StatusCode >= 0;

        public MessageId MessageID { get; set; }
        public MediusCallbackStatus StatusCode;
        public int ClanID;
        public int ClanMessageID;
        public string Message; // CLANMSG_MAXLEN
        public bool EndOfList;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();
            StatusCode = reader.Read<MediusCallbackStatus>();
            ClanID = reader.ReadInt32();
            
            if (reader.MediusVersion == 113)
            {
                ClanMessageID = reader.ReadInt32();
                Message = reader.ReadString(Constants.CLANMSG_MAXLEN_113_2);
            }
            else
                Message = reader.ReadString(Constants.CLANMSG_MAXLEN);

            EndOfList = reader.ReadBoolean();
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(StatusCode);
            writer.Write(ClanID);
            
            if (writer.MediusVersion == 113)
            {
                writer.Write(ClanMessageID);
                writer.Write(Message, Constants.CLANMSG_MAXLEN_113_2);
            }
            else
                writer.Write(Message, Constants.CLANMSG_MAXLEN);

            writer.Write(EndOfList);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"StatusCode: {StatusCode} " +
                $"ClanID: {ClanID} " +
                $"ClanMessageID: {ClanMessageID} " +
                $"Message: {Message} " +
                $"EndOfList: {EndOfList}";
        }
    }
}
