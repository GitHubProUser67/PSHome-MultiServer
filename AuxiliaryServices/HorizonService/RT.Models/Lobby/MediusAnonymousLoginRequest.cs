using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.AnonymousLogin)]
    public class MediusAnonymousLoginRequest : BaseLobbyMessage, IMediusRequest
    {

        public override byte PacketType => (byte)MediusLobbyMessageIds.AnonymousLogin;

        /// <summary>
        /// Message ID
        /// </summary>
        public MessageId MessageID { get; set; }
        /// <summary>
        /// Session Key
        /// </summary>
        public string SessionKey; // SESSIONKEY_MAXLEN
        /// <summary>
        /// SessionDisplayName
        /// </summary>
        public string SessionDisplayName; // ACCOUNTNAME_MAXLEN
        /// <summary>
        /// SessionDisplayStats
        /// </summary>
        public byte[] SessionDisplayStats; // ACCOUNTSTATS_MAXLEN

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();

            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            SessionDisplayName = reader.ReadString(Constants.ACCOUNTNAME_MAXLEN);
            SessionDisplayStats = reader.ReadBytes(Constants.ACCOUNTSTATS_MAXLEN);
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);

            writer.Write(SessionKey, Constants.SESSIONKEY_MAXLEN);
            writer.Write(SessionDisplayName, Constants.ACCOUNTNAME_MAXLEN);
            writer.Write(SessionDisplayStats, Constants.ACCOUNTSTATS_MAXLEN);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"SessionKey: {SessionKey} " +
                $"SessionDisplayName: {SessionDisplayName} " +
                $"SessionDisplayStats: {string.Join("", SessionDisplayStats)}";
        }
    }
}
