using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.GetAccessLevelInfoResponse)]
    public class MediusGetAccessLevelInfoResponse : BaseLobbyExtMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.GetAccessLevelInfoResponse;

        public bool IsSuccess => StatusCode >= 0;

        /// <summary>
        /// Message ID
        /// </summary>
        public MessageId MessageID { get; set; } 
        /// <summary>
        /// Response code to send back to the Access Level Request
        /// </summary>
        public MediusCallbackStatus StatusCode;
        /// <summary>
        /// Access Level that the currently connected client has
        /// </summary>
        public MediusAccessLevelType AccessLevel;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();

            //
            reader.ReadBytes(3);
            StatusCode = reader.Read<MediusCallbackStatus>();
            AccessLevel = reader.Read<MediusAccessLevelType>();
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            writer.Write(MessageID ?? MessageId.Empty);

            // 
            writer.Write(new byte[3]);
            writer.Write(StatusCode);
            writer.Write(AccessLevel);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"StatusCode: {StatusCode} " +
                $"AccessLevel: {AccessLevel}";
        }
    }
}
