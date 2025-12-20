using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.ClanLadderList)]
    public class MediusClanLadderListRequest : BaseLobbyMessage, IMediusRequest
    {

        public override byte PacketType => (byte)MediusLobbyMessageIds.ClanLadderList;

        public MessageId MessageID { get; set; }

        public int ClanLadderStatIndex;
        public MediusSortOrder SortOrder;
        public int StartPosition;
        public int PageSize;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();

            reader.ReadBytes(3);
            ClanLadderStatIndex = reader.ReadInt32();
            SortOrder = reader.Read<MediusSortOrder>();
            StartPosition = reader.ReadInt32();
            PageSize = reader.ReadInt32();
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);

            writer.Write(new byte[3]);
            writer.Write(ClanLadderStatIndex);
            writer.Write(SortOrder);
            writer.Write(StartPosition);
            writer.Write(PageSize);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID:{MessageID} " +
                $"ClanLadderStatIndex:{ClanLadderStatIndex} " +
                $"SortOrder:{SortOrder} " +
                $"StartPosition:{StartPosition} " +
                $"PageSize:{PageSize}";
        }
    }
}
