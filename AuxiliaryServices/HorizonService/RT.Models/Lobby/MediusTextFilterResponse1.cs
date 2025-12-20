using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.MediusTextFilterResponse1)]
    public class MediusTextFilterResponse1 : BaseLobbyExtMessage, IMediusResponse
    {

        public override byte PacketType => (byte)MediusLobbyExtMessageIds.MediusTextFilterResponse1;
        public bool IsSuccess => StatusCode >= 0;

        public MessageId MessageID { get; set; }
        public uint TextSize;
        public char[] Text;
        public MediusCallbackStatus StatusCode;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();

            // 
            StatusCode = reader.Read<MediusCallbackStatus>();
            TextSize = reader.ReadUInt32();
            Text = reader.ReadChars(Constants.CHATMESSAGE_MAXLEN);
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            writer.Write(MessageID ?? MessageId.Empty);

            // 
            writer.Write(StatusCode);
            writer.Write(TextSize);
            writer.Write(Text);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"StatusCode: {StatusCode} " +
                $"TextSize: {TextSize} " +
                $"Text: {string.Join("", Text)}";
        }
    }
}
