using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.RespondToClanTeamChallenge)]
    public class MediusRespondToClanTeamChallengeRequest : BaseLobbyMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.RespondToClanTeamChallenge;

        public MessageId MessageID { get; set; }
        public string SessionKey; // SESSIONKEY_MAXLEN
        public int ClanChallengeID;
        public MediusClanChallengeStatus ChallengeStatus;
        public string Message; // CLANMSG_MAXLEN

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            // 
            MessageID = reader.Read<MessageId>();
            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            reader.ReadBytes(2);
            ClanChallengeID = reader.ReadInt32();
            ChallengeStatus = reader.Read<MediusClanChallengeStatus>();
            Message = reader.ReadString(Constants.CLANMSG_MAXLEN);
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            // 
            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(SessionKey, Constants.SESSIONKEY_MAXLEN);
            writer.Write(new byte[2]);
            writer.Write(ClanChallengeID);
            writer.Write(ChallengeStatus);
            writer.Write(Message, Constants.CLANMSG_MAXLEN);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"SessionKey: {SessionKey} " + 
                $"ClanChallengeID: {ClanChallengeID} " +
                $"ChallengeStatus: {ChallengeStatus} " +
                $"Message: {Message}";
        }
    }
}
