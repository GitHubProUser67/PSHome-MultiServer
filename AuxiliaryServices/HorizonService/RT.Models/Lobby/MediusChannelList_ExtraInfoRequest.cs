using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.ChannelList_ExtraInfo)]
    public class MediusChannelList_ExtraInfoRequest : BaseLobbyExtMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.ChannelList_ExtraInfo;

        public MessageId MessageID { get; set; }

        public ushort PageID;
        public ushort PageSize;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();

            reader.ReadBytes(1);
            PageID = reader.ReadUInt16();
            PageSize = reader.ReadUInt16();
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);

            writer.Write(new byte[1]);
            writer.Write(PageID);
            writer.Write(PageSize);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                 $"MessageID: {MessageID} " +
                 $"PageID: {PageID} " +
                 $"PageSize: {PageSize}";
        }
    }
}
