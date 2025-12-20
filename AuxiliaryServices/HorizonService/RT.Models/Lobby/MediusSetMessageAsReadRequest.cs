using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    /// <summary>
    /// Request to tag message as read
    /// </summary>
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.SetMessageAsRead)]
    public class MediusSetMessageAsReadRequest : BaseLobbyMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.SetMessageAsRead;
        /// <summary>
        /// Message ID
        /// </summary>
        public MessageId MessageID { get; set; }
        /// <summary>
        /// Session Key
        /// </summary>
        public string SessionKey; // SESSIONKEY_MAXLEN
        /// <summary>
        /// MessageType to mark as read
        /// </summary>
        public MediusMessageType MessageType;
        /// <summary>
        /// MessageID to Tag with
        /// </summary>
        public int MessageIDToTag;
        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);
            MessageID = reader.Read<MessageId>();
            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            MessageType = reader.Read<MediusMessageType>();
            MessageIDToTag = reader.ReadInt32();
        }
        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);
            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(SessionKey, Constants.SESSIONKEY_MAXLEN);
            writer.Write(MessageType);
            writer.Write(MessageIDToTag);
        }
        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"SessionKey: {SessionKey} " +
                $"MessageType: {MessageType} " +
                $"MessageIDToTag: {MessageIDToTag}";
        }
    }
}
