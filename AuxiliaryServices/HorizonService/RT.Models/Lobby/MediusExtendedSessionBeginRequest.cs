using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    /// <summary>
    /// Wraps the information in a session begin <br></br>
    /// Request to begin a session (first network request to Medius).
    /// </summary>
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.ExtendedSessionBeginRequest)]
    public class MediusExtendedSessionBeginRequest : BaseLobbyExtMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.ExtendedSessionBeginRequest;

        /// <summary>
        /// Message ID
        /// </summary>
        public MessageId MessageID { get; set; }

        /// <summary>
        /// Connection Class: Ethernet, modem, or wireless 
        /// </summary>
        public MediusConnectionType ConnectionClass;
        
        /// <summary>
        /// Major version for the Medius Client
        /// </summary>
        public int ClientVersionMajor;
        /// <summary>
        /// Minor version for the Medius Client
        /// </summary>
        public int ClientVersionMinor;
        /// <summary>
        /// Special Patch version for the Medius Client
        /// </summary>
        public int ClientVersionSpecialPatch;
        /// <summary>
        /// Build version for the Medius Client
        /// </summary>
        public int ClientVersionBuild;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();

            reader.ReadBytes(3);
            ConnectionClass = reader.Read<MediusConnectionType>();
            ClientVersionMajor = reader.ReadInt32();
            ClientVersionMinor = reader.ReadInt32();
            if (reader.AppId == 22920)
                ClientVersionSpecialPatch = reader.ReadInt32();
            ClientVersionBuild = reader.ReadInt32();
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);

            writer.Write(new byte[3]);
            writer.Write(ConnectionClass);
            writer.Write(ClientVersionMajor);
            writer.Write(ClientVersionMinor);
            if (writer.AppId == 22920)
                writer.Write(ClientVersionSpecialPatch);
            writer.Write(ClientVersionBuild);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"ConnectionClass: {ConnectionClass} " +
                $"ClientVersionMajor: {ClientVersionMajor} " +
                $"ClientVersionMinor: {ClientVersionMinor} " +
                $"ClientVersionSpecialPatch: {ClientVersionSpecialPatch} " +
                $"ClientVersionBuild: {ClientVersionBuild}";
        }
    }
}
