using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.SetGameListSortResponse)]
    public class MediusSetGameListSortResponse : BaseLobbyExtMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.SetGameListSortResponse;
        public bool IsSuccess => StatusCode >= 0;
        public MessageId MessageID { get; set; }
        public MediusCallbackStatus StatusCode;
        public int SortID;
        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);
            MessageID = reader.Read<MessageId>();
            StatusCode = reader.Read<MediusCallbackStatus>();
            SortID = reader.ReadInt32();
        }
        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);
            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(StatusCode);
            writer.Write(SortID);
        }
        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"StatusCode: {StatusCode}";
        }
    }
}
