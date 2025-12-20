using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.TextFilterResponse)]
    public class MediusTextFilterResponse : BaseLobbyMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.TextFilterResponse;

        public bool IsSuccess => StatusCode >= 0;
        
        /// <summary>
        /// Message ID
        /// </summary>
        public MessageId MessageID { get; set; }
        /// <summary>
        /// Text to Filter
        /// </summary>
        public string Text; // CHATMESSAGE_MAXLEN
        /// <summary>
        /// Status Code to return
        /// </summary>
        public MediusCallbackStatus StatusCode;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();

            // 
            Text = reader.ReadString(Constants.CHATMESSAGE_MAXLEN);
            reader.ReadBytes(3);
            StatusCode = reader.Read<MediusCallbackStatus>();
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            writer.Write(MessageID ?? MessageId.Empty);

            // 
            writer.Write(Text, Constants.CHATMESSAGE_MAXLEN);
            writer.Write(new byte[3]);
            writer.Write(StatusCode);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"Text: {Text} " +
                $"StatusCode: {StatusCode}";
        }
    }
}
