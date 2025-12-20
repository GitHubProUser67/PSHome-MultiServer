using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.LadderPosition_ExtraInfo)]
    public class MediusLadderPosition_ExtraInfoRequest : BaseLobbyMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.LadderPosition_ExtraInfo;

        public MessageId MessageID { get; set; }

        public int AccountID;
        public int LadderStatIndex;
        public MediusSortOrder SortOrder;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();

            // 
            reader.ReadBytes(3);
            AccountID = reader.ReadInt32();
            LadderStatIndex = reader.ReadInt32();
            SortOrder = reader.Read<MediusSortOrder>();
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            writer.Write(MessageID ?? MessageId.Empty);

            // 
            writer.Write(new byte[3]);
            writer.Write(AccountID);
            writer.Write(LadderStatIndex);
            writer.Write(SortOrder);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"AccountID: {AccountID} " +
                $"LadderStatIndex: {LadderStatIndex} " +
                $"SortOrder: {SortOrder}";
        }
    }
}
