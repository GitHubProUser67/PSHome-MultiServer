using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.GetTotalUsersResponse)]
    public class MediusGetTotalUsersResponse : BaseLobbyMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.GetTotalUsersResponse;

        /// <summary>
        /// Message ID
        /// </summary>
        public MessageId MessageID { get; set; }
        /// <summary>
        /// Total number of players on the system
        /// </summary>
        public uint TotalInSystem;
        /// <summary>
        /// Total number of players in game
        /// </summary>
        public uint TotalInGame;
        /// <summary>
        /// Response code for the request to get the total number of players.
        /// </summary>
        public MediusCallbackStatus StatusCode;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();
            reader.ReadBytes(3);

            // 
            TotalInSystem = reader.ReadUInt32();
            TotalInGame = reader.ReadUInt32();
            StatusCode = reader.Read<MediusCallbackStatus>();
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(new byte[3]);

            // 
            writer.Write(TotalInSystem);
            writer.Write(TotalInGame);
            writer.Write(StatusCode);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"TotalInSystem: {TotalInSystem} " +
                $"TotalInGame: {TotalInGame} " +
                $"StatusCode: {StatusCode}";
        }
    }
}
