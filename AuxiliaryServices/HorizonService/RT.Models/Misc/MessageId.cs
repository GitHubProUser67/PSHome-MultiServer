using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    public class MessageId : IStreamSerializer
    {
        public readonly static MessageId Empty = new MessageId(string.Empty);

        public string Value { get; protected set; }

        public MessageId()
        {
            Value = GenerateMessageId();
        }

        public MessageId(string value)
        {
            Value = value;
        }

        #region Serialization

        public void Deserialize(BinaryReader reader)
        {
            Value = reader.ReadString(Constants.MESSAGEID_MAXLEN);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Value, Constants.MESSAGEID_MAXLEN);
        }

        #endregion

        public override string ToString()
        {
            return Value;
        }

        private static string GenerateMessageId()
        {
            return "1";
        }
    }
}
