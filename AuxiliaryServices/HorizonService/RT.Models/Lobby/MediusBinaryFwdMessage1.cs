using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.BinaryFwdMessage1)]
    public class MediusBinaryFwdMessage1 : BaseLobbyExtMessage
    {

        public override byte PacketType => (byte)MediusLobbyExtMessageIds.BinaryFwdMessage1;

        public MessageId MessageID { get; set; }

        public int OriginatorAccountID;
        public MediusBinaryMessageType MessageType;
        public int MessageSize;
        public byte[] Message;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();

            OriginatorAccountID = reader.ReadInt32();
            MessageType = reader.Read<MediusBinaryMessageType>();
            MessageSize = reader.ReadInt32();
            Message = reader.ReadBytes(MessageSize);
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);

            writer.Write(OriginatorAccountID);
            writer.Write(MessageType);
            writer.Write(MessageSize);
            writer.Write(Message, MessageSize);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID:{MessageID} " +
                $"OriginatorAccountID:{OriginatorAccountID} " +
                $"MessageType:{MessageType} " +
                $"MessageSize: {MessageSize} " +
                $"Message:{string.Join(string.Empty, System.BitConverter.ToString(Message))}";
        }
    }
}
