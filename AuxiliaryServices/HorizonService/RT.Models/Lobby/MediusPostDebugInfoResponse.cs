using Horizon.LIBRARY.Common.Stream;
using Horizon.RT.Common;

namespace Horizon.RT.Models
{
    [MediusMessage(
        NetMessageClass.MessageClassLobbyExt,
        MediusLobbyExtMessageIds.PostDebugInfoResponse
    )]
    public class MediusPostDebugInfoResponse : BaseLobbyExtMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.PostDebugInfoResponse;

        public bool IsSuccess => StatusCode >= 0;

        public MessageId MessageID { get; set; }
        public MediusCallbackStatus StatusCode;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();

            reader.ReadBytes(3);
            StatusCode = reader.Read<MediusCallbackStatus>();
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID);

            writer.Write(new byte[3]);
            writer.Write(StatusCode);
        }

        public override string ToString()
        {
            return base.ToString()
                + " "
                + $"MessageID: {MessageID} "
                + $"StatusCode: {StatusCode} ";
        }
    }
}
