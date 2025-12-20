using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.PartyJoinResponse)] // Set GameState
    public class MediusPartyJoinResponse : BaseLobbyExtMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.PartyJoinResponse; // Set GameState

        public bool IsSuccess => StatusCode >= 0;

        /// <summary>
        /// Message ID
        /// </summary>
        public MessageId MessageID { get; set; }
        /// <summary>
        /// Response code for the request to join a party
        /// </summary>
        public MediusCallbackStatus StatusCode;
        /// <summary>
        /// PartyHostType
        /// </summary>
        public MGCL_GAME_HOST_TYPE PartyHostType;
        /// <summary>
        /// ConnectionInfo of the player to return for this session
        /// </summary>
        public NetConnectionInfo ConnectionInfo;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>(); 
            reader.ReadBytes(3);

            StatusCode = reader.Read<MediusCallbackStatus>();
            PartyHostType = reader.Read<MGCL_GAME_HOST_TYPE>();
            ConnectionInfo = reader.Read<NetConnectionInfo>();
            //MatchGameState = reader.Read<MediusMatchGameState>();
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(new byte[3]);

            writer.Write(StatusCode);
            writer.Write(PartyHostType);
            writer.Write(ConnectionInfo);
            //writer.Write(MatchGameState);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"StatusCode: {StatusCode} " +
                $"PartyHostType: {PartyHostType} " +
                $"ConnectionInfo: {ConnectionInfo} ";
            //$"GameState: {MatchGameState}";
        }
    }
}
