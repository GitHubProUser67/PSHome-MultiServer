using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.GetLadderStatsWide)]
    public class MediusGetLadderStatsWideRequest : BaseLobbyExtMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.GetLadderStatsWide;

        public MessageId MessageID { get; set; }

        public int AccountID_or_ClanID;
        public MediusLadderType LadderType;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();

            // 
            reader.ReadBytes(3);
            AccountID_or_ClanID = reader.ReadInt32();
            LadderType = reader.Read<MediusLadderType>();
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            writer.Write(MessageID ?? MessageId.Empty);

            // 
            writer.Write(new byte[3]);
            writer.Write(AccountID_or_ClanID);
            writer.Write(LadderType);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"AccountID_or_ClanID: {AccountID_or_ClanID} " +
                $"LadderType: {LadderType}";
        }
    }
}
