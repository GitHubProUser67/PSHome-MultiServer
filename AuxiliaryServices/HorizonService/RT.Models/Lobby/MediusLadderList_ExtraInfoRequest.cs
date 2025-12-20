using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.LadderList_ExtraInfo)]
    public class MediusLadderList_ExtraInfoRequest : BaseLobbyExtMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.LadderList_ExtraInfo;

        public MessageId MessageID { get; set; }

        public int LadderStatIndex;
        public MediusSortOrder SortOrder;
        public uint StartPosition;
        public uint PageSize;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();

            //
            reader.ReadBytes(3);
            LadderStatIndex = reader.ReadInt32();
            SortOrder = reader.Read<MediusSortOrder>();
            StartPosition = reader.ReadUInt32();
            PageSize = reader.ReadUInt32();
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            writer.Write(MessageID ?? MessageId.Empty);

            // 
            writer.Write(new byte[3]);
            writer.Write(LadderStatIndex);
            writer.Write(SortOrder);
            writer.Write(StartPosition);
            writer.Write(PageSize);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID:{MessageID} " +
             $"LadderStatIndex:{LadderStatIndex} " +
$"SortOrder:{SortOrder} " +
$"StartPosition:{StartPosition} " +
$"PageSize:{PageSize}";
        }
    }
}
