using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    /// <summary>
    /// Introduced in Medius 1.43
    /// </summary>
    [ScertMessage(RT_MSG_TYPE.RT_MSG_SERVER_SYSTEM_MESSAGE)]
    public class RT_MSG_SERVER_SYSTEM_MESSAGE : BaseScertMessage
    {
        public override RT_MSG_TYPE Id => RT_MSG_TYPE.RT_MSG_SERVER_SYSTEM_MESSAGE;

        public byte Severity;
        public DME_SERVER_ENCODING_TYPE EncodingType;
        public DME_SERVER_LANGUAGE_TYPE LanguageType;
        public bool EndOfMessage;
        public string Message;

        public override void Deserialize(MessageReader reader)
        {
            Severity = reader.ReadByte();
            EncodingType = reader.Read<DME_SERVER_ENCODING_TYPE>();
            LanguageType = reader.Read<DME_SERVER_LANGUAGE_TYPE>();
            EndOfMessage = reader.ReadBoolean();
            Message = reader.ReadRestAsString();
        }

        public override void Serialize(MessageWriter writer)
        {
            writer.Write(Severity);
            writer.Write(EncodingType);
            writer.Write(LanguageType);
            writer.Write(EndOfMessage);
            if (Message != null)
                writer.Write(Message, Message.Length + 1);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"Severity: {Severity} " +
                $"EncodingType: {EncodingType} " +
                $"MediusLanguageType: {LanguageType} " +
                $"EndOfMessage: {EndOfMessage} " +
                $"Message: {Message}";
        }
    }
}
