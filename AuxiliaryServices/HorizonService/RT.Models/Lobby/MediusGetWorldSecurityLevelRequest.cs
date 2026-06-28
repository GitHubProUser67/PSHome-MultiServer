using Horizon.LIBRARY.Common.Stream;
using Horizon.RT.Common;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.GetWorldSecurityLevel)]
    public class MediusGetWorldSecurityLevelRequest : BaseLobbyMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.GetWorldSecurityLevel;

        /// <summary>
        /// Message ID
        /// </summary>
        public MessageId MessageID { get; set; }

        /// <summary>
        /// Session Key
        /// </summary>
        public string SessionKey; // SESSIONKEY_MAXLEN

        /// <summary>
        /// World ID to get the security level for.
        /// </summary>
        public int MediusWorldID;

        /// <summary>
        /// Application Type:lobby chat channel or game world.
        /// </summary>
        public MediusApplicationType AppType;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();
            reader.ReadBytes(2);

            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);

            MediusWorldID = reader.ReadInt32();
            AppType = reader.Read<MediusApplicationType>();
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(new byte[2]);

            writer.Write(SessionKey, Constants.SESSIONKEY_MAXLEN);

            writer.Write(MediusWorldID);
            writer.Write(AppType);
        }

        public override string ToString()
        {
            return base.ToString()
                + " "
                + $"MessageID: {MessageID} "
                + $"SessionKey: {SessionKey} "
                + $"MediusWorldID: {MediusWorldID} "
                + $"AppType: {AppType}";
        }
    }
}
