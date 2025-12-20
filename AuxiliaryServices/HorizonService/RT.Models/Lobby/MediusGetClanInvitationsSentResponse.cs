using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.GetClanInvitationsSentResponse)]
    public class MediusGetClanInvitationsSentResponse : BaseLobbyMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.GetClanInvitationsSentResponse;

        public bool IsSuccess => StatusCode >= 0;

        public MessageId MessageID { get; set; }

        public MediusCallbackStatus StatusCode;
        public int AccountID;
        public string AccountName; // ACCOUNTNAME_MAXLEN
        public string ResponseMsg; // CLANMSG_MAXLEN
        public MediusClanInvitationsResponseStatus ResponseStatus;
        public int ResponseTime;
        public bool EndOfList;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();

            // 
            reader.ReadBytes(3);
            StatusCode = reader.Read<MediusCallbackStatus>();
            AccountID = reader.ReadInt32();
            AccountName = reader.ReadString(Constants.ACCOUNTNAME_MAXLEN);

            if(reader.MediusVersion == 113)
            {
                ResponseMsg = reader.ReadString(Constants.CLANMSG_MAXLEN_113);
            } else
            {
                ResponseMsg = reader.ReadString(Constants.CLANMSG_MAXLEN);
            }
            ResponseStatus = reader.Read<MediusClanInvitationsResponseStatus>();
            ResponseTime = reader.ReadInt32();
            EndOfList = reader.ReadBoolean();
            reader.ReadBytes(3);
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
            writer.Write(AccountID);
            writer.Write(AccountName, Constants.ACCOUNTNAME_MAXLEN);
            if(writer.MediusVersion == 113)
            {
                writer.Write(ResponseMsg, Constants.CLANMSG_MAXLEN_113);
            } else
            {
                writer.Write(ResponseMsg, Constants.CLANMSG_MAXLEN);
            }
            writer.Write(ResponseStatus);
            writer.Write(ResponseTime);
            writer.Write(EndOfList);
            writer.Write(new byte[3]);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"StatusCode: {StatusCode} " +
                $"AccountID: {AccountID} " +
                $"AccountName: {AccountName} " +
                $"ResponseMsg: {ResponseMsg} " +
                $"ResponseStatus: {ResponseStatus} " +
                $"ResponseTime: {ResponseTime} " +
                $"EndOfList: {EndOfList}";
        }
    }
}
