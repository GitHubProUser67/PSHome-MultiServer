using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.ChatFwdMessage)]
    public class MediusChatFwdMessage : BaseLobbyMessage
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.ChatFwdMessage;

        public MessageId MessageID { get; set; }

        public int OriginatorAccountID;
        public string OriginatorAccountName;
        public MediusChatMessageType MessageType;
        public string Message;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();

            reader.ReadBytes(3);
            OriginatorAccountID = reader.ReadInt32();
            OriginatorAccountName = reader.ReadString(Constants.ACCOUNTNAME_MAXLEN);
            MessageType = reader.Read<MediusChatMessageType>();
            Message = reader.ReadString(Constants.CHATMESSAGE_MAXLEN);
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);

            writer.Write(new byte[3]);
            writer.Write(OriginatorAccountID);
            writer.Write(OriginatorAccountName, Constants.ACCOUNTNAME_MAXLEN);
            writer.Write(MessageType);
            writer.Write(Message, Constants.CHATMESSAGE_MAXLEN);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID:{MessageID} " +
                $"OriginatorAccountID:{OriginatorAccountID} " +
                $"OriginatorAccountName:{OriginatorAccountName} " +
                $"MessageType:{MessageType} " +
                $"Message:{Message}";
        }
    }
}
