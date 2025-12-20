using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassDME, MediusDmeMessageIds.Ping)]
    public class TypePing : BaseDMEMessage
    {
        public override byte PacketType => (byte)MediusDmeMessageIds.Ping;

        public uint TimeOfSend;
        public byte PingInstance;
        public bool RequestEcho;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            TimeOfSend = reader.ReadUInt32();
            PingInstance = reader.ReadByte();
            RequestEcho = reader.ReadBoolean();
            reader.ReadBytes(2);
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(TimeOfSend);
            writer.Write(PingInstance);
            writer.Write(RequestEcho);
            writer.Write(new byte[2]);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"TimeOfSend: {TimeOfSend} " +
                $"PingInstance: {PingInstance} " +
                $"RequestEcho: {RequestEcho} ";
        }
    }
}
