using Horizon.LIBRARY.Common.Stream;
using Horizon.RT.Common;

namespace Horizon.RT.Models
{
    public class MessageId : IStreamSerializer
    {
        public static readonly MessageId Empty = new(string.Empty);

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
