using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.SetGameListFilterResponse)]
    public class MediusSetGameListFilterResponse : BaseLobbyExtMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.SetGameListFilterResponse;

        public bool IsSuccess => StatusCode >= 0;

        public MessageId MessageID { get; set; }

        public MediusCallbackStatus StatusCode;
        public uint FilterID;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();

            reader.ReadBytes(3);
            StatusCode = reader.Read<MediusCallbackStatus>();
            FilterID = reader.ReadUInt32();
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);

            writer.Write(new byte[3]);
            writer.Write(StatusCode);
            writer.Write(FilterID);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"StatusCode: {StatusCode} " +
                $"FilterID: {FilterID}";
        }
    }
}
