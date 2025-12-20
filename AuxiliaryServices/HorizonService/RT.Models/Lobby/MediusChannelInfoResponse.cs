using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.ChannelInfoResponse)]
    public class MediusChannelInfoResponse : BaseLobbyMessage, IMediusResponse
    {

        public override byte PacketType => (byte)MediusLobbyMessageIds.ChannelInfoResponse;

        public bool IsSuccess => StatusCode >= 0;

        /// <summary>
        /// Message ID
        /// </summary>
        public MessageId MessageID { get; set; }
        /// <summary>
        /// Response status from the request to get information about a channel
        /// </summary>
        public MediusCallbackStatus StatusCode;
        /// <summary>
        /// Chat channel name
        /// </summary>
        public string LobbyName; // LOBBYNAME_MAXLEN
        /// <summary>
        /// Number of players
        /// </summary>
        public int ActivePlayerCount;
        /// <summary>
        /// Maximum number of players.
        /// </summary>
        public int MaxPlayers;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();

            reader.ReadBytes(3);
            StatusCode = reader.Read<MediusCallbackStatus>();
            LobbyName = reader.ReadString(Constants.LOBBYNAME_MAXLEN);
            ActivePlayerCount = reader.ReadInt32();
            MaxPlayers = reader.ReadInt32();
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);

            writer.Write(new byte[3]);
            writer.Write(StatusCode);
            writer.Write(LobbyName, Constants.LOBBYNAME_MAXLEN);
            writer.Write(ActivePlayerCount);
            writer.Write(MaxPlayers);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"StatusCode: {StatusCode} " +
                $"LobbyName: {LobbyName} " +
                $"ActivePlayerCount: {ActivePlayerCount} " +
                $"MaxPlayers: {MaxPlayers}";
        }
    }
}
