using System.IO;
using CustomLogger;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.JoinGameResponse)]
    public class MediusJoinGameResponse : BaseLobbyMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.JoinGameResponse;

        public bool IsSuccess => StatusCode >= 0;

        /// <summary>
        /// Message ID
        /// </summary>
        public MessageId MessageID { get; set; }

        public MediusCallbackStatus StatusCode;
        public MGCL_GAME_HOST_TYPE GameHostType;
        public NetConnectionInfo ConnectInfo;
        /// <summary>
        /// MaxPlayers
        /// </summary>
        public long MaxPlayers;

        public bool SetMaxPlayers = false;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();
            reader.ReadBytes(3);

            StatusCode = reader.Read<MediusCallbackStatus>();
            GameHostType = reader.Read<MGCL_GAME_HOST_TYPE>();
            ConnectInfo = reader.Read<NetConnectionInfo>();

            if (reader.MediusVersion == 113 && SetMaxPlayers)
                MaxPlayers = reader.ReadInt64();
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(new byte[3]);

            writer.Write(StatusCode);
            writer.Write(GameHostType);
            writer.Write(ConnectInfo);

            if (writer.MediusVersion == 113 && SetMaxPlayers)
            {
#if DEBUG
                LoggerAccessor.LogInfo($"[MediusJoinGameResponse] - Setting MaxPlayers for {writer.AppId}");
#endif
                writer.Write(MaxPlayers);
            }
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"StatusCode: {StatusCode} " +
                $"GameHostType: {GameHostType} " +
                $"ConnectInfo: {ConnectInfo} " +
                $"MaxPlayers: {MaxPlayers}";
        }
    }
}
