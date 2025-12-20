using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.GetLadderStatsWideResponse)]
    public class MediusGetLadderStatsWideResponse : BaseLobbyExtMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.GetLadderStatsWideResponse;

        public bool IsSuccess => StatusCode >= 0;

        public MessageId MessageID { get; set; }

        public MediusCallbackStatus StatusCode;
        public int AccountID_or_ClanID;
        public int[] Stats = new int[Constants.LADDERSTATSWIDE_MAXLEN];

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();

            // 
            reader.ReadBytes(3);
            StatusCode = reader.Read<MediusCallbackStatus>();
            AccountID_or_ClanID = reader.ReadInt32();
            for (int i = 0; i < Constants.LADDERSTATSWIDE_MAXLEN; ++i) { Stats[i] = reader.ReadInt32(); }
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            writer.Write(MessageID ?? MessageId.Empty);

            // 
            writer.Write(new byte[3]);
            writer.Write(StatusCode);
            writer.Write(AccountID_or_ClanID);
            for (int i = 0; i < Constants.LADDERSTATSWIDE_MAXLEN; ++i) { writer.Write(i >= Stats.Length ? 0 : Stats[i]); }
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"StatusCode: {StatusCode} " +
                $"AccountID_or_ClanID: {AccountID_or_ClanID} " +
                $"Stats: {System.Convert.ToString(Stats)}";
        }
    }
}
