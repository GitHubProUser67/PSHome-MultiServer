using System.IO;
using Horizon.RT.Common;
using Horizon.RT.Models.Misc;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.ChatMessage)]
    public class MediusChatMessage : BaseLobbyExtMessage, IMediusChatMessage
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.ChatMessage;

        public MessageId MessageID { get; set; }

        public string SessionKey; // SESSIONKEY_MAXLEN
        public MediusChatMessageType MessageType { get; set; }
        public int TargetID { get; set; }
        public string Message { get; set; }

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();

            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            reader.ReadBytes(2);
            MessageType = reader.Read<MediusChatMessageType>();
            TargetID = reader.ReadInt32();
            Message = reader.ReadString(Constants.CHATMESSAGE_MAXLEN);
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);

            writer.Write(SessionKey, Constants.SESSIONKEY_MAXLEN);
            writer.Write(new byte[2]);
            writer.Write(MessageType);
            writer.Write(TargetID);
            writer.Write(Message, Constants.CHATMESSAGE_MAXLEN);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"SessionKey: {SessionKey} " +
                $"MessageType: {MessageType} " +
                $"TargetID: {TargetID} " +
                $"Message: {Message}";
        }
    }
}
