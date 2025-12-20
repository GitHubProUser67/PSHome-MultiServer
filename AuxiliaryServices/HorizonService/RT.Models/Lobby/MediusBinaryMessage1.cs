using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{

    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.BinaryMessage1)]
    public class MediusBinaryMessage1 : BaseLobbyExtMessage
    {

        public override byte PacketType => (byte)MediusLobbyExtMessageIds.BinaryMessage1;


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
        /// MessageSize of Game Developer binary message
        /// </summary>
        public int MessageSize;
        /// <summary>
        /// Game Developer binary message
        /// </summary>
        public byte[] Message = new byte[Constants.BINARYMESSAGE_MAXLEN];

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();

            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            MessageType = reader.Read<MediusBinaryMessageType>();
            TargetAccountID = reader.ReadInt32();

            MessageSize = reader.ReadInt32();
            Message = reader.ReadBytes(MessageSize);
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);

            writer.Write(SessionKey, Constants.SESSIONKEY_MAXLEN);
            writer.Write(MessageType);
            writer.Write(TargetAccountID);
            writer.Write(MessageSize);
            writer.Write(Message, MessageSize);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID:{MessageID} " +
                $"SessionKey:{SessionKey} " +
                $"MessageType:{MessageType} " +
                $"TargetAccountID:{TargetAccountID} " +
                $"MessageSize: {MessageSize} " +
                $"Message:{string.Join(string.Empty, System.BitConverter.ToString(Message))}";
        }
    }
}
