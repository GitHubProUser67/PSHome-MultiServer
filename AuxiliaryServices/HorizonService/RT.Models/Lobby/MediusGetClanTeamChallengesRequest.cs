using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.GetClanTeamChallenges)]
    public class MediusGetClanTeamChallengesRequest : BaseLobbyMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.GetClanTeamChallenges;



        public MessageId MessageID { get; set; }
        public string SessionKey; // SESSIONKEY_MAXLEN
        public int ClanID;
        public int Start;
        public int PageSize;
        public MediusClanChallengeStatus Status;
        public int ChallengedOnly;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            // 
            MessageID = reader.Read<MessageId>();
            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            reader.ReadBytes(2);

            //
            ClanID = reader.ReadInt32();
            Start = reader.ReadInt32();
            PageSize = reader.ReadInt32();
            Status = reader.Read<MediusClanChallengeStatus>();
            ChallengedOnly = reader.ReadInt32();
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            // 
            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(SessionKey, Constants.SESSIONKEY_MAXLEN);
            writer.Write(new byte[2]);

            //
            writer.Write(ClanID);
            writer.Write(Start);
            writer.Write(PageSize);
            writer.Write(Status);
            writer.Write(ChallengedOnly);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"SessionKey: {SessionKey} " +
                $"ClanID: {ClanID} " +
                $"Start: {Start} " +
                $"PageSize: {PageSize} " +
                $"Status: {Status} " +
                $"ChallengedOnly: {ChallengedOnly}";
        }
    }
}
