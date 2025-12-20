using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.MediusTextFilter1)]
    public class MediusTextFilterRequest1 : BaseLobbyExtMessage, IMediusRequest
    {

        public override byte PacketType => (byte)MediusLobbyExtMessageIds.MediusTextFilter1;

        public MessageId MessageID { get; set; }

        public string SessionKey; // SESSIONKEY_MAXLEN
        public MediusTextFilterType TextFilter;
        public uint TextSize;
        public string Text; // variable len

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();

            // 
            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            TextFilter = reader.Read<MediusTextFilterType>();
            TextSize = reader.ReadUInt32();
            Text = reader.ReadString(Constants.CHATMESSAGE_MAXLEN);
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            writer.Write(MessageID ?? MessageId.Empty);

            // 
            writer.Write(SessionKey, Constants.SESSIONKEY_MAXLEN);
            writer.Write(TextFilter);
            writer.Write(TextSize);
            writer.Write(Text, Constants.CHATMESSAGE_MAXLEN);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"SessionKey: {SessionKey} " +
                $"TextFilter: {TextFilter} " +
                $"TextSize: {TextSize} " +
                $"Text: {Text}";
        }
    }
}
