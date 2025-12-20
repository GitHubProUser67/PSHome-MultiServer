using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    public class MediusStatusResponse : BaseMediusMessage, IMediusResponse
    {
        public NetMessageClass Class;
        public byte Type;

        public override byte PacketType => Type;
        public override NetMessageClass PacketClass => Class;

        public bool IsSuccess => StatusCode >= 0;

        /// <summary>
        /// Message ID
        /// </summary>
        public MessageId MessageID { get; set; }
        /// <summary>
        /// Response code to a Ban Player request
        /// </summary>
        public MediusCallbackStatus StatusCode;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();

            reader.ReadBytes(3);
            StatusCode = reader.Read<MediusCallbackStatus>();
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);

            writer.Write(new byte[3]);
            writer.Write(StatusCode);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"StatusCode: {StatusCode}";
        }
    }
}
