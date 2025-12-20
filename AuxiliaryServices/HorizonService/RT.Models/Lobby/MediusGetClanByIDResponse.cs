using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.GetClanByIDResponse)]
    public class MediusGetClanByIDResponse : BaseLobbyMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.GetClanByIDResponse;

        public bool IsSuccess => StatusCode >= 0;

        public MessageId MessageID { get; set; }

        public MediusCallbackStatus StatusCode;
        public int ApplicationID;
        public string ClanName; // CLANNAME_MAXLEN
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

            // 
            reader.ReadBytes(3);
            StatusCode = reader.Read<MediusCallbackStatus>();
            ApplicationID = reader.ReadInt32();
            ClanName = reader.ReadString(Constants.CLANNAME_MAXLEN);
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

            // 
            writer.Write(new byte[3]);
            writer.Write(StatusCode);
            writer.Write(ApplicationID);
            writer.Write(ClanName, Constants.CLANNAME_MAXLEN);
            writer.Write(LeaderAccountID);
            writer.Write(LeaderAccountName, Constants.ACCOUNTNAME_MAXLEN);
            writer.Write(Stats, Constants.CLANSTATS_MAXLEN);
            writer.Write(Status);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID:{MessageID} " +
             $"StatusCode:{StatusCode} " +
$"ApplicationID:{ApplicationID} " +
$"ClanName:{ClanName} " +
$"LeaderAccountID:{LeaderAccountID} " +
$"LeaderAccountName:{LeaderAccountName} " +
$"Stats:{Stats} " +
$"Status:{Status}";
        }
    }
}
