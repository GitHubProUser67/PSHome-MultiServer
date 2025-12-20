using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.GetWorldSecurityLevelResponse)]
    public class MediusGetWorldSecurityLevelResponse : BaseLobbyMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.GetWorldSecurityLevelResponse;

        /// <summary>
        /// Message ID
        /// </summary>
        public MessageId MessageID { get; set; }
        /// <summary>
        /// Response code for the request to get the security level about a world
        /// </summary>
        public MediusCallbackStatus StatusCode;
        /// <summary>
        /// The world ID of the lobby world or game world.
        /// </summary>
        public int MediusWorldID;
        /// <summary>
        /// Application type; chat channel or game
        /// </summary>
        public MediusApplicationType AppType;
        /// <summary>
        /// Security level information
        /// </summary>
        public MediusWorldSecurityLevelType SecurityLevel;


        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();
            reader.ReadBytes(3);

            StatusCode = reader.Read<MediusCallbackStatus>();
            MediusWorldID = reader.ReadInt32();
            AppType = reader.Read<MediusApplicationType>();
            SecurityLevel = reader.Read<MediusWorldSecurityLevelType>();
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(new byte[3]);

            writer.Write(StatusCode);
            writer.Write(MediusWorldID);
            writer.Write(AppType);
            writer.Write(SecurityLevel);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"StatusCode: {StatusCode} " +
                $"MediusWorldID: {MediusWorldID} " +
                $"AppType: {AppType} " +
                $"SecurityLevel: {SecurityLevel}";
        }
    }
}
