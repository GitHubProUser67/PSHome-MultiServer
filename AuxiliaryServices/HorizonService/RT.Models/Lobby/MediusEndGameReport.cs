using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    /// <summary>
    /// Report from the "host" of game at the end of the game instance to the Medius Lobby Server
    /// </summary>
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.EndGameReport)]
    public class MediusEndGameReport : BaseLobbyMessage
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.EndGameReport;

        /// <summary>
        /// Session Key
        /// </summary>
        public string SessionKey; // SESSIONKEY_MAXLEN
        /// <summary>
        /// World ID of the game to terminate
        /// </summary>
        public int MediusWorldID;
        /// <summary>
        /// Winning Team information N/A
        /// </summary>
        public string WinningTeam; // WINNINGTEAM_MAXLEN
        /// <summary>
        /// Winning Player Information N/A
        /// </summary>
        public string WinningPlayer; // ACCOUNTNAME_MAXLEN
        /// <summary>
        /// Final Score N/A
        /// </summary>
        public int FinalScore;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            // 
            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            reader.ReadBytes(3);

            //
            MediusWorldID = reader.ReadInt32();
            WinningTeam = reader.ReadString(Constants.WINNINGTEAM_MAXLEN);
            WinningPlayer = reader.ReadString(Constants.ACCOUNTNAME_MAXLEN);
            FinalScore = reader.ReadInt32();
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            // 
            writer.Write(SessionKey, Constants.SESSIONKEY_MAXLEN);
            writer.Write(new byte[3]);

            //
            writer.Write(MediusWorldID);
            writer.Write(WinningTeam, Constants.WINNINGTEAM_MAXLEN);
            writer.Write(WinningPlayer, Constants.ACCOUNTNAME_MAXLEN);
            writer.Write(FinalScore);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
             $"SessionKey: {SessionKey} " +
             $"MediusWorldID: {MediusWorldID} " +
             $"WinningTeam: {WinningTeam} " +
             $"WinningPlayer: {WinningPlayer} " +
             $"FinalScore: {FinalScore}";
        }
    }
}
