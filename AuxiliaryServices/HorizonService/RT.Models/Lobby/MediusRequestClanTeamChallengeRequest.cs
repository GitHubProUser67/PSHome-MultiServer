using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.RequestClanTeamChallenge)]
    public class MediusRequestClanTeamChallengeRequest : BaseLobbyMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.RequestClanTeamChallenge;



        public MessageId MessageID { get; set; }
        public string SessionKey; // SESSIONKEY_MAXLEN
        public int ClanID;
        public string Message; // CLANMSG_MAXLEN

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

            //
            writer.Write(ClanID);
            writer.Write(Message, Constants.CLANMSG_MAXLEN);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"SessionKey: {SessionKey} " +
                $"ClanID: {ClanID} " +
                $"Message: {Message}";
        }
    }
}
