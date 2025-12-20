using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    /// <summary>
    /// Introduced in Medius Client Library Version 2.12 <br></br>
    /// Fetches the Access Level of the currently connected player?
    /// </summary>
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.GetAccessLevelInfoRequest)]
    public class MediusGetAccessLevelInfoRequest : BaseLobbyExtMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.GetAccessLevelInfoRequest;

        /// <summary>
        /// Message ID
        /// </summary>
        public MessageId MessageID { get; set; }

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            writer.Write(MessageID ?? MessageId.Empty);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} ";
        }
    }
}
