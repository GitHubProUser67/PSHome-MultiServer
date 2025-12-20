using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.GetClanTeamChallengesResponse)]
    public class MediusGetClanTeamChallengesResponse : BaseLobbyMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.GetClanTeamChallengesResponse;

        public bool IsSuccess => StatusCode >= 0;

        public MessageId MessageID { get; set; }
        public MediusCallbackStatus StatusCode;
        public int ChallengerClanID;
        public int AgainstClanID;
        public MediusClanChallengeStatus Status;
        public int ResponseTime;
        public string ChallengeMsg; // CLANMSG_MAXLEN
        public string ResponseMsg; // CLANMSG_MAXLEN
        public bool EndOfList;
        public int ClanChallengeID;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            // 
            MessageID = reader.Read<MessageId>();
            reader.ReadBytes(3);
            StatusCode = reader.Read<MediusCallbackStatus>();
            ChallengerClanID = reader.ReadInt32();
            AgainstClanID = reader.ReadInt32();
            Status = reader.Read<MediusClanChallengeStatus>();
            ResponseTime = reader.ReadInt32();
            ChallengeMsg = reader.ReadString(Constants.CLANMSG_MAXLEN);
            ResponseMsg = reader.ReadString(Constants.CLANMSG_MAXLEN);
            EndOfList = reader.ReadBoolean();
            reader.ReadBytes(3);
            ClanChallengeID = reader.ReadInt32();
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            // 
            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(new byte[3]);
            writer.Write(StatusCode);
            writer.Write(ChallengerClanID);
            writer.Write(AgainstClanID);
            writer.Write(Status);
            writer.Write(ResponseTime);
            writer.Write(ChallengeMsg, Constants.CLANMSG_MAXLEN);
            writer.Write(ResponseMsg, Constants.CLANMSG_MAXLEN);
            writer.Write(EndOfList);
            writer.Write(new byte[3]);
            writer.Write(ClanChallengeID);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"StatusCode: {StatusCode} " +
                $"ChallengerClanID: {ChallengerClanID} " +
                $"AgainstClanID: {AgainstClanID} " +
                $"Status: {Status} " +
                $"ResponseTime: {ResponseTime} " + 
                $"ChallengeMsg: {ChallengeMsg} " + 
                $"ResponseMsg: {ResponseMsg} " + 
                $"EndOfList: {EndOfList} " + 
                $"ClanChallengeID: {ClanChallengeID}";
        }
    }
}
