using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    /// <summary>
    /// Response(s) to MediusFindPlayerRequest
    /// </summary>
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.FindPlayerResponse)]
    public class MediusFindPlayerResponse : BaseLobbyMessage, IMediusResponse
    {

        public override byte PacketType => (byte)MediusLobbyMessageIds.FindPlayerResponse;

        public bool IsSuccess => StatusCode >= 0;

        /// <summary>
        /// Message ID
        /// </summary>
        public MessageId MessageID { get; set; }
        /// <summary>
        /// Response code for the request to find a user.
        /// </summary>
        public MediusCallbackStatus StatusCode;
        /// <summary>
        /// Application ID of the user
        /// </summary>
        public int ApplicationID;
        /// <summary>
        /// Application name of the user.
        /// </summary>
        public string ApplicationName; // APPNAME_MAXLEN
        /// <summary>
        /// In a lobby chat channel or game world.
        /// </summary>
        public MediusApplicationType ApplicationType;
        /// <summary>
        /// World ID
        /// </summary>
        public int MediusWorldID;
        /// <summary>
        /// Account ID
        /// </summary>
        public int AccountID;
        /// <summary>
        /// Accoutn Name
        /// </summary>
        public string AccountName; // ACCOUNTNAME_MAXLEN
        /// <summary>
        /// Flag 0 or 1 to determine the end of list.
        /// </summary>
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
            ApplicationID = reader.ReadInt32();
            ApplicationName = reader.ReadString(Constants.APPNAME_MAXLEN);
            ApplicationType = reader.Read<MediusApplicationType>();
            MediusWorldID = reader.ReadInt32();
            AccountID = reader.ReadInt32();
            AccountName = reader.ReadString(Constants.ACCOUNTNAME_MAXLEN);
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
            writer.Write(ApplicationID);
            writer.Write(ApplicationName, Constants.APPNAME_MAXLEN);
            writer.Write(ApplicationType);
            writer.Write(MediusWorldID);
            writer.Write(AccountID);
            writer.Write(AccountName, Constants.ACCOUNTNAME_MAXLEN);
            writer.Write(EndOfList);
            writer.Write(new byte[3]);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"StatusCode: {StatusCode} " +
                $"ApplicationID: {ApplicationID} " +
                $"ApplicationName: {ApplicationName} " +
                $"ApplicationType: {ApplicationType} " +
                $"MediusWorldID: {MediusWorldID} " +
                $"AccountID: {AccountID} " +
                $"AccountName: {AccountName} " +
                $"EndOfList: {EndOfList}";
        }
    }
}
