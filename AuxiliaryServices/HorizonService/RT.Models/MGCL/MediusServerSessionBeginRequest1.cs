using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{

    /// <summary>
    /// Begins a Peer to Peer MAS Session
    /// </summary>
    [MediusMessage(NetMessageClass.MessageClassLobbyReport, MediusMGCLMessageIds.ServerSessionBeginRequest1)]
    public class MediusServerSessionBeginRequest1 : BaseMGCLMessage, IMediusRequest
    {

        public override byte PacketType => (byte)MediusMGCLMessageIds.ServerSessionBeginRequest1;

        public MessageId MessageID { get; set; }
        public int LocationID;
        public int ApplicationID;
        public MGCL_GAME_HOST_TYPE ServerType;
        /// <summary>
        /// Major version for the Medius Client
        /// </summary>
        public int ClientVersionMajor;
        /// <summary>
        /// Minor version for the Medius Client
        /// </summary>
        public int ClientVersionMinor;
        /// <summary>
        /// Build version for the Medius Client
        /// </summary>
        public int ClientVersionBuild;
        public int Port;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            // 
            MessageID = reader.Read<MessageId>();
            reader.ReadBytes(3);
            LocationID = reader.ReadInt32();
            ApplicationID = reader.ReadInt32();
            ServerType = reader.Read<MGCL_GAME_HOST_TYPE>();
            ClientVersionMajor = reader.ReadInt32();
            ClientVersionMinor = reader.ReadInt32();
            ClientVersionBuild = reader.ReadInt32();
            Port = reader.ReadInt32();
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            // 
            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(new byte[3]);
            writer.Write(LocationID);
            writer.Write(ApplicationID);
            writer.Write(ServerType);

            writer.Write(ClientVersionMajor);
            writer.Write(ClientVersionMinor);
            writer.Write(ClientVersionBuild);
            writer.Write(Port);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"LocationID: {LocationID} " +
                $"ApplicationID: {ApplicationID} " +
                $"ServerType: {ServerType} " +
                $"ClientVersionMajor: {ClientVersionMajor} " +
                $"ClientVersionMinor: {ClientVersionMinor} " +
                $"ClientVersionBuild: {ClientVersionBuild} " +
                $"Port: {Port}";
        }
    }
}
