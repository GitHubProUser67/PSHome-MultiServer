using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.GetClanByNameResponse)]
    public class MediusGetClanByNameResponse : BaseLobbyMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.GetClanByNameResponse;

        public bool IsSuccess => StatusCode >= 0;

        public MessageId MessageID { get; set; }
        public MediusCallbackStatus StatusCode;
        public int ClanID;
        public int LeaderAccountID;
        public string LeaderAccountName; // ACCOUNTNAME_MAXLEN
        public byte[] Stats; // CLANSTATS_MAXLEN
        public MediusClanStatus Status;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            // 
            MessageID = reader.Read<MessageId>();
            reader.ReadBytes(3);
            StatusCode = reader.Read<MediusCallbackStatus>();
            ClanID = reader.ReadInt32();
            LeaderAccountID = reader.ReadInt32();
            LeaderAccountName = reader.ReadString(Constants.ACCOUNTNAME_MAXLEN);
            Stats = reader.ReadBytes(Constants.CLANSTATS_MAXLEN);
            Status = reader.Read<MediusClanStatus>();
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            // 
            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(new byte[3]);
            writer.Write(StatusCode);
            writer.Write(ClanID);
            writer.Write(LeaderAccountID);
            writer.Write(LeaderAccountName, Constants.ACCOUNTNAME_MAXLEN);
            writer.Write(Stats, Constants.CLANSTATS_MAXLEN);
            writer.Write(Status);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID:{MessageID}" + " " +
                $"StatusCode:{StatusCode}" + " " +
                $"ClanID:{ClanID}" + " " +
                $"LeaderAccountID:{LeaderAccountID}" + " " +
                $"LeaderAccountName:{LeaderAccountName}" + " " +
                $"Stats:{Stats}" + " " +
                $"Status:{Status}";
        }
    }
}
