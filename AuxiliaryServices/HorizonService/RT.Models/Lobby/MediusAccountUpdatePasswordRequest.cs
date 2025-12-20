using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    /// <summary>
    /// Request to update currently logged-in account password
    /// </summary>
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.AccountUpdatePassword)]
    public class MediusAccountUpdatePasswordRequest : BaseLobbyMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.AccountUpdatePassword;

        /// <summary>
        /// Message ID
        /// </summary>
        public MessageId MessageID { get; set; }
        /// <summary>
        /// Session Key
        /// </summary>
        public string SessionKey; // SESSIONKEY_MAXLEN
        /// <summary>
        /// Old Password
        /// </summary>
        public string OldPassword; // PASSWORD_MAXLEN
        /// <summary>
        /// New Password
        /// </summary>
        public string NewPassword; // PASSWORD_MAXLEN

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();

            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            OldPassword = reader.ReadString(Constants.PASSWORD_MAXLEN);
            NewPassword = reader.ReadString(Constants.PASSWORD_MAXLEN);
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);

            writer.Write(SessionKey, Constants.SESSIONKEY_MAXLEN);
            writer.Write(OldPassword, Constants.PASSWORD_MAXLEN);
            writer.Write(NewPassword, Constants.PASSWORD_MAXLEN);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"SessionKey: {SessionKey} " +
                $"OldPassword: {OldPassword} " +
                $"NewPassword: {NewPassword}";
        }
    }
}
