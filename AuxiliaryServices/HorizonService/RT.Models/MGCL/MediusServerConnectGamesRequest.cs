using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    /// <summary>
    /// Request for the Medius Servers to connect a game world to this host
    /// </summary>
    [MediusMessage(NetMessageClass.MessageClassLobbyReport, MediusMGCLMessageIds.ServerConnectGamesRequest)]
    public class MediusServerConnectGamesRequest : BaseMGCLMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusMGCLMessageIds.ServerConnectGamesRequest;

        /// <summary>
        /// Message ID used for asynchronous request processing.
        /// </summary>
        public MessageId MessageID { get; set; }
        /// <summary>
        /// IP Address of the server to connect to
        /// </summary>
        public string ServerIP; // MGCL_SERVERIP_MAXLEN
        /// <summary>
        /// Port of the server to connect to.
        /// </summary>
        public int ServerPort;
        /// <summary>
        /// Game World ID to connect to.
        /// </summary>
        public int GameWorldID;
        /// <summary>
        /// Spectator World ID to connect to.
        /// </summary>
        public int SpectatorWorldID;

        public override void Deserialize(MessageReader reader)
        {
            
            base.Deserialize(reader);

            
            MessageID = reader.Read<MessageId>();
            ServerIP = reader.ReadString(Constants.MGCL_SERVERIP_MAXLEN);
            reader.ReadBytes(3);
            ServerPort = reader.ReadInt32();
            GameWorldID = reader.ReadInt32();
            SpectatorWorldID = reader.ReadInt32();
        }

        public override void Serialize(MessageWriter writer)
        {
            
            base.Serialize(writer);

            
            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(ServerIP, Constants.MGCL_SERVERIP_MAXLEN);
            writer.Write(new byte[3]);
            writer.Write(ServerPort);
            writer.Write(GameWorldID);
            writer.Write(SpectatorWorldID);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"ServerIP: {ServerIP} " +
                $"ServerPort: {ServerPort} " +
                $"GameWorldID: {GameWorldID} " +
                $"SpectatorWorldID: {SpectatorWorldID}";
        }
    }
}
