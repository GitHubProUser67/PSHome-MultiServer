using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    /// <summary>
    /// Introduced in Medius API v2.7<br></br>
    /// - Sends a binary message to everyone in the channel, or to a specific account id.
    /// </summary>
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.BinaryMessage)]
    public class MediusBinaryMessage : BaseLobbyExtMessage
    {

        public override byte PacketType => (byte)MediusLobbyExtMessageIds.BinaryMessage;

        /// <summary>
        /// Message ID
        /// </summary>
        public MessageId MessageID { get; set; }
        /// <summary>
        /// Session Key
        /// </summary>
        public string SessionKey; // SESSIONKEY_MAXLEN
        /// <summary>
        /// BinaryMessageType
        /// </summary>
        public MediusBinaryMessageType MessageType;
        /// <summary>
        /// TargetAccountID to send Binary Message to
        /// </summary>
        public int TargetAccountID;
        /// <summary>
        /// Game Developer binary message
        /// </summary>
        public byte[] Message = new byte[Constants.BINARYMESSAGE_MAXLEN];

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();

            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            reader.ReadBytes(2);

            MessageType = reader.Read<MediusBinaryMessageType>();
            TargetAccountID = reader.ReadInt32();
            Message = reader.ReadBytes(Constants.BINARYMESSAGE_MAXLEN);
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);

            writer.Write(SessionKey, Constants.SESSIONKEY_MAXLEN);
            writer.Write(new byte[2]);

            writer.Write(MessageType);
            writer.Write(TargetAccountID);
            writer.Write(Message, Constants.BINARYMESSAGE_MAXLEN);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"SessionKey: {SessionKey} " +
                $"MessageType: {MessageType} " +
                $"TargetAccountID: {TargetAccountID} " +
                $"Message: {System.BitConverter.ToString(Message)}";
        }
    }
}
