using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

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
            SessionKey = reader.ReadString();
            
            reader.ReadBytes(2);
            MediusWorldID = reader.ReadInt32();
            AppType = reader.Read<MediusApplicationType>();
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(SessionKey);
            
            writer.Write(new byte[2]);
            writer.Write(MediusWorldID);
            writer.Write(AppType);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"SessionKey: {SessionKey} " +
                $"MediusWorldID: {MediusWorldID} " +
                $"AppType: {AppType}";
        }
    }
}
