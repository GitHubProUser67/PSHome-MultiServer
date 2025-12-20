using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.CheckMyClanInvitationsResponse)]
    public class MediusCheckMyClanInvitationsResponse : BaseLobbyMessage, IMediusResponse
    {

        public override byte PacketType => (byte)MediusLobbyMessageIds.CheckMyClanInvitationsResponse;

        public bool IsSuccess => StatusCode >= 0;

        public MessageId MessageID { get; set; }

        public MediusCallbackStatus StatusCode;
        public int ClanInvitationID;
        public int ClanID;
        public MediusClanInvitationsResponseStatus ResponseStatus;
        public string Message; // CLANMSG_MAXLEN
        public int LeaderAccountID;
        public string LeaderAccountName; // ACCOUNTNAME_MAXLEN
        public bool EndOfList;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();

            reader.ReadBytes(3);
            StatusCode = reader.Read<MediusCallbackStatus>();
            ClanInvitationID = reader.ReadInt32();
            ClanID = reader.ReadInt32();
            ResponseStatus = reader.Read<MediusClanInvitationsResponseStatus>();
            if(reader.MediusVersion == 113)
                Message = reader.ReadString(Constants.CLANMSG_MAXLEN_113);
            else
                Message = reader.ReadString(Constants.CLANMSG_MAXLEN);
            LeaderAccountID = reader.ReadInt32();
            LeaderAccountName = reader.ReadString(Constants.ACCOUNTNAME_MAXLEN);
            EndOfList = reader.ReadBoolean();
            reader.ReadBytes(3);
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);

            writer.Write(new byte[3]);
            writer.Write(StatusCode);
            writer.Write(ClanInvitationID);
            writer.Write(ClanID);
            writer.Write(ResponseStatus);
            if(writer.MediusVersion == 113)
                writer.Write(Message, Constants.CLANMSG_MAXLEN_113);
            else
                writer.Write(Message, Constants.CLANMSG_MAXLEN);
            writer.Write(LeaderAccountID);
            writer.Write(LeaderAccountName, Constants.ACCOUNTNAME_MAXLEN);
            writer.Write(EndOfList);
            writer.Write(new byte[3]);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"StatusCode: {StatusCode} " +
                $"ClanInvitationID: {ClanInvitationID} " +
                $"ClanID: {ClanID} " +
                $"ResponseStatus: {ResponseStatus} " +
                $"Message: {Message} " +
                $"LeaderAccountID: {LeaderAccountID} " +
                $"LeaderAccountName: {LeaderAccountName} " +
                $"EndOfList: {EndOfList}";
        }
    }
}
