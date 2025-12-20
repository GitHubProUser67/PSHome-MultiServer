using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.AccountLogin)]
    public class MediusAccountLoginRequest : BaseLobbyMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.AccountLogin;

        public MessageId MessageID { get; set; }

        public string SessionKey; // SESSIONKEY_MAXLEN
        public string Username; // ACCOUNTNAME_MAXLEN
        public string Password; // PASSWORD_MAXLEN

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();

            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            Username = reader.ReadString(Constants.ACCOUNTNAME_MAXLEN);
            Password = reader.ReadString(Constants.PASSWORD_MAXLEN);
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);

            writer.Write(SessionKey, Constants.SESSIONKEY_MAXLEN);
            writer.Write(Username, Constants.ACCOUNTNAME_MAXLEN);
            writer.Write(Password, Constants.PASSWORD_MAXLEN);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID:{MessageID} " +
                $"SessionKey:{SessionKey} " +
                $"USERNAME:{Username} " +
                $"PASS:{Password}";
        }
    }
}
