using Horizon.LIBRARY.Common.Stream;
using Horizon.RT.Common;

namespace Horizon.RT.Models
{
    [ScertMessage(RT_MSG_TYPE.RT_MSG_SERVER_MEMORY_POKE)]
    public class RT_MSG_SERVER_MEMORY_POKE : BaseScertMessage
    {
        public override RT_MSG_TYPE Id => RT_MSG_TYPE.RT_MSG_SERVER_MEMORY_POKE;

        public uint start_Address = 0;
        public byte[] Payload;

        public override void Deserialize(MessageReader reader)
        {
            start_Address = reader.ReadUInt32();
            var len = reader.ReadInt32();
            Payload = reader.ReadBytes(len);
        }

        public override void Serialize(MessageWriter writer)
        {
            writer.Write(start_Address);
            writer.Write(Payload?.Length ?? 0);
            if (Payload != null)
                writer.Write(Payload);
        }

        public override string ToString()
        {
            return base.ToString()
                + " "
                + $"Address: {start_Address} "
                + $"PayloadLen: {Payload?.Length} "
                + $"Payload: {string.Join("", Payload ?? Array.Empty<byte>())}";
        }

        public static List<RT_MSG_SERVER_MEMORY_POKE> FromPayload(uint address, byte[] payload)
        {
            var i = 0;
            var msgs = new List<RT_MSG_SERVER_MEMORY_POKE>();

            while (i < payload.Length)
            {
                var len = payload.Length - i;
                if (len > Constants.MEDIUS_MESSAGE_MAXLEN)
                    len = Constants.MEDIUS_MESSAGE_MAXLEN;

                var msg = new RT_MSG_SERVER_MEMORY_POKE()
                {
                    start_Address = (uint)(address + i),
                    Payload = new byte[len],
                    SkipEncryption = true,
                };

                Array.Copy(payload, i, msg.Payload, 0, len);

                msgs.Add(msg);

                i += len;
            }

            return msgs;
        }
    }
}
