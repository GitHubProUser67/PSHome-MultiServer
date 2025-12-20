using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.InvitePlayerToClan_ByName)]
    public class MediusInvitePlayerToClan_ByNameRequest : BaseLobbyMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.InvitePlayerToClan_ByName;



        public MessageId MessageID { get; set; }
        public string AccountName; // ACCOUNTNAME_MAXLEN
        public string InviteMessage; // CLANMSG_MAXLEN

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            // 
            MessageID = reader.Read<MessageId>();
            AccountName = reader.ReadString(Constants.ACCOUNTNAME_MAXLEN);
            InviteMessage = reader.ReadString(Constants.CLANMSG_MAXLEN);
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            // 
            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(AccountName, Constants.ACCOUNTNAME_MAXLEN);
            writer.Write(InviteMessage, Constants.CLANMSG_MAXLEN);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID:{MessageID}" + " " +
                $"AccountName:{AccountName}" + " " +
                $"InviteMessage:{InviteMessage}";
        }
    }
}
