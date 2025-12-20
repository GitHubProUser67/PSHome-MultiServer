using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;
using System.Collections.Generic;
using System;

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
            int len = reader.ReadInt32();
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
            return base.ToString() + " " +

                $"Address: {start_Address} " +
                $"PayloadLen: {Payload?.Length} " +
                $"Payload: {string.Join("", Payload ?? new byte[0])}";
        }

        public static List<RT_MSG_SERVER_MEMORY_POKE> FromPayload(uint address, byte[] payload)
        {
            int i = 0;
            List<RT_MSG_SERVER_MEMORY_POKE> msgs = new List<RT_MSG_SERVER_MEMORY_POKE>();

            while (i < payload.Length)
            {
                int len = payload.Length - i;
                if (len > Constants.MEDIUS_MESSAGE_MAXLEN)
                    len = Constants.MEDIUS_MESSAGE_MAXLEN;

                RT_MSG_SERVER_MEMORY_POKE msg = new RT_MSG_SERVER_MEMORY_POKE()
                {
                    start_Address = (uint)(address + i),
                    Payload = new byte[len],
                    SkipEncryption = true
                };

                Array.Copy(payload, i, msg.Payload, 0, len);

                msgs.Add(msg);

                i += len;
            }

            return msgs;
        }
    }
}
