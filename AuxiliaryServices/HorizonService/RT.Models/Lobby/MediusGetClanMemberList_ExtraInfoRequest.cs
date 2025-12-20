using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.GetClanMemberList_ExtraInfo)]
    public class MediusGetClanMemberList_ExtraInfoRequest : BaseLobbyMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.GetClanMemberList_ExtraInfo;

        public MessageId MessageID { get; set; }

        public int ClanID;
        public int LadderStatIndex;
        public MediusSortOrder SortOrder;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();

            reader.ReadBytes(3);
            ClanID = reader.ReadInt32();
            if (reader.MediusVersion != 113)
                LadderStatIndex = reader.ReadInt32();
            SortOrder = reader.Read<MediusSortOrder>();
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);

            writer.Write(new byte[3]);
            writer.Write(ClanID);
            if (writer.MediusVersion != 113)
                writer.Write(LadderStatIndex);
            writer.Write(SortOrder);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"ClanID: {ClanID} " +
                $"LadderStatIndex: {LadderStatIndex} " +
                $"SortOrder: {SortOrder}";
        }
    }
}
