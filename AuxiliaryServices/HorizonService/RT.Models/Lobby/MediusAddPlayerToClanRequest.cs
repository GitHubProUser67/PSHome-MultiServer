using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.AddPlayerToClan)]
    public class MediusAddPlayerToClanRequest : BaseLobbyMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.AddPlayerToClan;

        public MessageId MessageID { get; set; }
        public string SessionKey; // SESSIONKEY_MAXLEN
        public int PlayerAccountID;
        public string WelcomeMessage; // CLANMSG_MAXLEN

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();
            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            reader.ReadBytes(2);
            PlayerAccountID = reader.ReadInt32();
            WelcomeMessage = reader.ReadString(Constants.CLANMSG_MAXLEN);
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(SessionKey, Constants.SESSIONKEY_MAXLEN);
            writer.Write(new byte[2]);
            writer.Write(PlayerAccountID);
            writer.Write(WelcomeMessage, Constants.CLANMSG_MAXLEN);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID}" + " " +
                $"SessionKey: {SessionKey}" + " " +
                $"PlayerAccountID: {PlayerAccountID}" + " " +
                $"WelcomeMessage: {WelcomeMessage}";
        }
    }
}
