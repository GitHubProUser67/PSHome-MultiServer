using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    /// <summary>
    /// Request structure to locate chat channels and/or game world based on name.
    /// </summary>
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.FindWorldByName)]
    public class MediusFindWorldByNameRequest : BaseLobbyMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.FindWorldByName;

        /// <summary>
        /// Message ID
        /// </summary>
        public MessageId MessageID { get; set; }
        /// <summary>
        /// Session Key
        /// </summary>
        public string SessionKey; // SESSIONKEY_MAXLEN
        /// <summary>
        /// Name of the world to look for
        /// </summary>
        public string Name;
        /// <summary>
        /// Type of world to loof for (game or chat channel)
        /// </summary>
        public MediusFindWorldType WorldType;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();

            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            Name = reader.ReadString(Constants.WORLDNAME_MAXLEN);
            reader.ReadBytes(2);
            WorldType = reader.Read<MediusFindWorldType>();
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            writer.Write(MessageID ?? MessageId.Empty);

            writer.Write(SessionKey);
            writer.Write(Name, Constants.WORLDNAME_MAXLEN);
            writer.Write(new byte[2]);
            writer.Write(WorldType);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"SessionKey: {SessionKey} " +
                $"Name: {Name} " +
                $"WorldType: {WorldType}";
        }
    }
}
