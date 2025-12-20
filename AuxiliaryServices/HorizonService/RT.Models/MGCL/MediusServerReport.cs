using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    /// <summary>
    /// This is the MGCL host/server report (total capacity and total state) used in MGCLMediusServerReport
    /// </summary>
    [MediusMessage(NetMessageClass.MessageClassLobbyReport, MediusMGCLMessageIds.ServerReport)]
    public class MediusServerReport : BaseMGCLMessage
    {
        public override NetMessageClass PacketClass => NetMessageClass.MessageClassLobbyReport;
        public override byte PacketType => (byte)MediusMGCLMessageIds.ServerReport;

        /// <summary>
        /// This is a server session key. It is needed because it <Br></Br>
        /// first comes in on a redirected connection and can <br></br>
        /// not look up the server by connection. MGCL populates internally.
        /// </summary>
        public string SessionKey;
        /// <summary>
        /// Maximum number of game worlds supported by the game server.
        /// </summary>
        public short MaxWorlds;
        /// <summary>
        /// The maximum number of players per game world.
        /// </summary>
        public short MaxPlayersPerWorld;
        /// <summary>
        /// The number of active game worlds on this game server. <br></br> 
        /// Usually 1 for peer-to-peer hosts, or more for DME Servers.
        /// </summary>
        public short ActiveWorldCount;
        /// <summary>
        /// The total number of active players connected to the game server.
        /// </summary>
        public short TotalActivePlayers;
        /// <summary>
        /// Alert level to allow for load balancing.
        /// </summary>
        public MGCL_ALERT_LEVEL AlertLevel;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            SessionKey = reader.ReadString(Constants.MGCL_SESSIONKEY_MAXLEN);
            reader.ReadBytes(1);

            MaxWorlds = reader.ReadInt16();
            MaxPlayersPerWorld = reader.ReadInt16();
            ActiveWorldCount = reader.ReadInt16();
            TotalActivePlayers = reader.ReadInt16();
            reader.ReadBytes(2);

            AlertLevel = reader.Read<MGCL_ALERT_LEVEL>();
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(SessionKey, Constants.MGCL_SESSIONKEY_MAXLEN);
            writer.Write(new byte[1]);

            writer.Write(MaxWorlds);
            writer.Write(MaxPlayersPerWorld);
            writer.Write(ActiveWorldCount);
            writer.Write(TotalActivePlayers);
            writer.Write(new byte[2]);

            writer.Write(AlertLevel);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"SessionKey: {SessionKey} " +
                $"MaxWorlds: {MaxWorlds} " +
                $"MaxPlayersPerWorld: {MaxPlayersPerWorld} " +
                $"ActiveWorldCount: {ActiveWorldCount} " +
                $"TotalActivePlayers: {TotalActivePlayers} " +
                $"AlertLevel: {AlertLevel}";
        }
    }
}
