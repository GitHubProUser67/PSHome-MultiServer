using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.TransferClanLeadership)]
    public class MediusTransferClanLeadershipRequest : BaseLobbyMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.TransferClanLeadership;



        public MessageId MessageID { get; set; }
        public string SessionKey; // SESSIONKEY_MAXLEN
        public int NewLeaderAccountID;
        public string NewLeaderAccountName; // ACCOUNTNAME_MAXLEN

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            // 
            MessageID = reader.Read<MessageId>();
            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            reader.ReadBytes(2);
            NewLeaderAccountID = reader.ReadInt32();
            NewLeaderAccountName = reader.ReadString(Constants.ACCOUNTNAME_MAXLEN);
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            // 
            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(SessionKey, Constants.SESSIONKEY_MAXLEN);
            writer.Write(new byte[2]);
            writer.Write(NewLeaderAccountID);
            writer.Write(NewLeaderAccountName, Constants.ACCOUNTNAME_MAXLEN);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID:{MessageID}" + " " +
                $"SessionKey:{SessionKey}" + " " +
                $"NewLeaderAccountID:{NewLeaderAccountID}" + " " +
                $"NewLeaderAccountName:{NewLeaderAccountName}";
        }
    }
}
