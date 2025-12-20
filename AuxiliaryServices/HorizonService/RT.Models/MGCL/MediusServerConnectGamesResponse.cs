using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    /// <summary>
    /// Response to Medius Servers with the status of a particular world and whether it is allowed to be connected to this host
    /// </summary>
    [MediusMessage(NetMessageClass.MessageClassLobbyReport, MediusMGCLMessageIds.ServerConnectGamesResponse)]
    public class MediusServerConnectGamesResponse : BaseMGCLMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusMGCLMessageIds.ServerConnectGamesResponse;

        /// <summary>
        /// Message ID used for asynchronous request processing.
        /// </summary>
        public MessageId MessageID { get; set; }
        /// <summary>
        /// Game World ID of the world being requested.
        /// </summary>
        public int GameWorldID;
        /// <summary>
        /// Spectator World ID of the world being requested.
        /// </summary>
        public int SpectatorWorldID;
        /// <summary>
        /// MGCL_SUCESS or other code to indicate error in Medius
        /// </summary>
        public MGCL_ERROR_CODE Confirmation;

        public bool IsSuccess => Confirmation >= 0;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();
            reader.ReadBytes(3);
            GameWorldID = reader.ReadInt32();
            SpectatorWorldID = reader.ReadInt32();
            Confirmation = reader.Read<MGCL_ERROR_CODE>();
            reader.ReadBytes(3);
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(new byte[3]);
            writer.Write(GameWorldID);
            writer.Write(SpectatorWorldID);
            writer.Write(Confirmation);
            writer.Write(new byte[3]);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"GameWorldID: {GameWorldID} " +
                $"SpectatorWorldID: {SpectatorWorldID} " +
                $"Confirmation: {Confirmation}";
        }
    }
}
