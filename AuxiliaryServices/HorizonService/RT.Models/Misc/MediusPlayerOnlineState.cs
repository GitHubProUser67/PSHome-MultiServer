using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    public class MediusPlayerOnlineState : IStreamSerializer
    {
        public static readonly MediusPlayerOnlineState Empty = new MediusPlayerOnlineState();

        public MediusPlayerStatus ConnectStatus;
        public int MediusLobbyWorldID;
        public int MediusGameWorldID;
        public string LobbyName;
        public string GameName;

        public void Deserialize(BinaryReader reader)
        {
            ConnectStatus = reader.Read<MediusPlayerStatus>();
            MediusLobbyWorldID = reader.ReadInt32();
            MediusGameWorldID = reader.ReadInt32();
            LobbyName = reader.ReadString(Constants.WORLDNAME_MAXLEN);
            GameName = reader.ReadString(Constants.GAMENAME_MAXLEN);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(ConnectStatus);
            writer.Write(MediusLobbyWorldID);
            writer.Write(MediusGameWorldID);
            writer.Write(LobbyName, Constants.WORLDNAME_MAXLEN);
            writer.Write(GameName, Constants.GAMENAME_MAXLEN);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"ConnectStatus: {ConnectStatus} " +
                $"MediusLobbyWorldID: {MediusLobbyWorldID} " +
                $"MediusGameWorldID: {MediusGameWorldID} " +
                $"LobbyName: {LobbyName} " +
                $"GameName: {GameName}";
        }
    }
}
