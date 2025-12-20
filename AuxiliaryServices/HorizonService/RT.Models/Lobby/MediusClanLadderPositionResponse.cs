using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.ClanLadderPositionResponse)]
    public class MediusClanLadderPositionResponse : BaseLobbyMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.ClanLadderPositionResponse;

        public bool IsSuccess => StatusCode >= 0;

        public MessageId MessageID { get; set; }
        public uint LadderPosition;
        public uint TotalRankings;
        public MediusCallbackStatus StatusCode;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();
            reader.ReadBytes(3);
            LadderPosition = reader.ReadUInt32();
            TotalRankings = reader.ReadUInt32();
            StatusCode = reader.Read<MediusCallbackStatus>();
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(new byte[3]);
            writer.Write(LadderPosition);
            writer.Write(TotalRankings);
            writer.Write(StatusCode);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID:{MessageID}" + " " +
                $"LadderPosition:{LadderPosition}" + " " +
                $"TotalRankings:{TotalRankings}" + " " +
                $"StatusCode:{StatusCode}";
        }
    }
}
