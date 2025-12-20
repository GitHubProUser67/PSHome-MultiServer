using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    /// <summary>
    /// Response structure for the request to set the game server attributes.
    /// </summary>
    [MediusMessage(NetMessageClass.MessageClassLobbyReport, MediusMGCLMessageIds.ServerSetAttributesResponse)]
    public class MediusServerSetAttributesResponse : BaseMGCLMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusMGCLMessageIds.ServerSetAttributesResponse;

        /// <summary>
        /// Message ID used for asynchronous request processing.
        /// </summary>
        public MessageId MessageID { get; set; }
        /// <summary>
        /// MGCL_SUCCESS or other value to indicate an error.
        /// </summary>
        public MGCL_ERROR_CODE Confirmation;

        public bool IsSuccess => Confirmation >= 0;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            // 
            MessageID = reader.Read<MessageId>();
            Confirmation = reader.Read<MGCL_ERROR_CODE>();
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            // 
            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(Confirmation);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"Confirmation: {Confirmation}";
        }
    }
}
