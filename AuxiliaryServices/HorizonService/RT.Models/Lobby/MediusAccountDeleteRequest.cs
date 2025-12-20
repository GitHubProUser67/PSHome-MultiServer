using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.AccountDelete)]
    public class MediusAccountDeleteRequest : BaseLobbyMessage, IMediusRequest
    {
        public override NetMessageClass PacketClass => NetMessageClass.MessageClassLobby;

        public override byte PacketType => (byte)MediusLobbyMessageIds.AccountDelete;

        public MessageId MessageID { get; set; }

        public string SessionKey; // SESSIONKEY_MAXLEN
        public string MasterPassword; // PASSWORD_MAXLEN

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();

            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            MasterPassword = reader.ReadString(Constants.PASSWORD_MAXLEN);
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);

            writer.Write(SessionKey, Constants.SESSIONKEY_MAXLEN);
            writer.Write(MasterPassword, Constants.PASSWORD_MAXLEN);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"SessionKey: {SessionKey} " +
                $"MasterPassword: {MasterPassword}";
        }
    }
}
