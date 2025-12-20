using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.UpdateLadderStats)]
    public class MediusUpdateLadderStatsRequest : BaseLobbyMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.UpdateLadderStats;

        public MessageId MessageID { get; set; }

        public MediusLadderType LadderType;
        public int[] Stats = new int[Constants.LADDERSTATS_MAXLEN];

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();

            //
            reader.ReadBytes(3);
            LadderType = reader.Read<MediusLadderType>();
            for (int i = 0; i < Constants.LADDERSTATS_MAXLEN; ++i) { Stats[i] = reader.ReadInt32(); }
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            writer.Write(MessageID ?? MessageId.Empty);

            // 
            writer.Write(new byte[3]);
            writer.Write(LadderType);
            for (int i = 0; i < Constants.LADDERSTATS_MAXLEN; ++i) { writer.Write(i >= Stats.Length ? 0 : Stats[i]); }
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"LadderType: {LadderType} " +
                $"Stats: {Stats}";
        }
    }
}
