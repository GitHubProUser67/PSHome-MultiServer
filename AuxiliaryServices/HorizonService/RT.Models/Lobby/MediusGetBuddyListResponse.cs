using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.GetBuddyListResponse)]
    public class MediusGetBuddyListResponse : BaseLobbyMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.GetBuddyListResponse;

        public bool IsSuccess => StatusCode >= 0;

        /// <summary>
        /// Message ID
        /// </summary>
        public MessageId MessageID { get; set; }
        /// <summary>
        /// Response codefor the request to get your buddy list.
        /// </summary>
        public MediusCallbackStatus StatusCode;
        /// <summary>
        /// The Account ID of the Buddy.
        /// </summary>
        public int AccountID;
        /// <summary>
        /// The Account Name of the Buddy.
        /// </summary>
        public string AccountName; // ACCOUNTNAME_MAXLEN
        /// <summary>
        /// The player's status
        /// </summary>
        public MediusPlayerStatus PlayerStatus;
        /// <summary>
        /// Flag 0 or 1 to determine th end of list.
        /// </summary>
        public bool EndOfList;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();

            reader.ReadBytes(3);
            StatusCode = reader.Read<MediusCallbackStatus>();
            AccountID = reader.ReadInt32();
            AccountName = reader.ReadString(Constants.ACCOUNTNAME_MAXLEN);
            PlayerStatus = reader.Read<MediusPlayerStatus>();
            EndOfList = reader.ReadBoolean();
            reader.ReadBytes(3);
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);

            writer.Write(new byte[3]);
            writer.Write(StatusCode);
            writer.Write(AccountID);
            writer.Write(AccountName, Constants.ACCOUNTNAME_MAXLEN);
            writer.Write(PlayerStatus);
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
                $"OnlineState: {PlayerStatus} " +
                $"EndOfList: {EndOfList}";
        }
    }
}
