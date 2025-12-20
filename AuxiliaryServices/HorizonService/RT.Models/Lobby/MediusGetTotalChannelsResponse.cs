using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.GetTotalChannelsResponse)]
    public class MediusGetTotalChannelsResponse : BaseLobbyMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.GetTotalChannelsResponse;

        public bool IsSuccess => StatusCode >= 0;

        /// Notes: Status code can be: MediusFail, MediusSuccess, 
        /// MediusRequestDenied (If calling on a non-lobby server connection)
        /// 
        /// Description: Returns total number of channels for an ApplicationID.

        /// <summary>
        /// Message ID
        /// </summary>
        public MessageId MessageID { get; set; }
        /// <summary>
        /// Total number of channels for the given ApplicationID
        /// </summary>
        public uint Total;
        /// <summary>
        /// Response code for the request to get the total number of chat channels
        /// </summary>
        public MediusCallbackStatus StatusCode;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();

            // 
            reader.ReadBytes(3);
            Total = reader.ReadUInt32();
            StatusCode = reader.Read<MediusCallbackStatus>();
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            writer.Write(MessageID ?? MessageId.Empty);

            // 
            writer.Write(new byte[3]);
            writer.Write(Total);
            writer.Write(StatusCode);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"StatusCode: {StatusCode} " +
                $"Total: {Total}";
        }
    }
}
