using Horizon.LIBRARY.Common.Stream;
using Horizon.RT.Common;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.SendClanMessage)]
    public class MediusSendClanMessageRequest : BaseLobbyMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.SendClanMessage;

        public MessageId MessageID { get; set; }
        public string SessionKey; // SESSIONKEY_MAXLEN
        public string Message; // CLANMSG_MAXLEN

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();
            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            Message =
                reader.MediusVersion == 113
                    ? reader.ReadString(Constants.CLANMSG_MAXLEN_113_2)
                    : reader.ReadString(Constants.CLANMSG_MAXLEN);
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(SessionKey, Constants.SESSIONKEY_MAXLEN);
            if (writer.MediusVersion == 113)
                writer.Write(Message, Constants.CLANMSG_MAXLEN_113_2);
            else
                writer.Write(Message, Constants.CLANMSG_MAXLEN);
        }

        public override string ToString()
        {
            return base.ToString()
                + " "
                + $"MessageID: {MessageID} "
                + $"SessionKey: {SessionKey} "
                + $"Message: {Message}";
        }
    }
}
