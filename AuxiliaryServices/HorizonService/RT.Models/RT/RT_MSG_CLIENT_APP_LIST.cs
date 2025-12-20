using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Horizon.RT.Models
{
    [ScertMessage(RT_MSG_TYPE.RT_MSG_CLIENT_APP_LIST)]
    public class RT_MSG_CLIENT_APP_LIST : BaseScertMessage
    {
        public override RT_MSG_TYPE Id => RT_MSG_TYPE.RT_MSG_CLIENT_APP_LIST;

        public List<int> Targets { get; set; } = new List<int>();
        public byte[] Payload { get; set; }

        public override void Deserialize(MessageReader reader)
        {
            byte size = reader.ReadByte();
            byte[] mask = reader.ReadBytes(size);
            Payload = reader.ReadRest();

            Targets = new List<int>();
            for (int b = 0; b < size; ++b)
                for (int i = 0; i < 8; ++i)
                    if ((mask[b] & (1 << i)) != 0)
                        Targets.Add(i + (b * 8));
        }

        public override void Serialize(MessageWriter writer)
        {
            // Determine size of bitmask in bytes
            byte size = 1;
            if (Targets != null && Targets.Count > 0)
                size = (byte)Math.Ceiling((Targets.Max() + 1) / 8d);

            // Populate bitmask
            byte[] mask = new byte[size];
            if (Targets != null)
                foreach (var target in Targets)
                    mask[target / 8] |= (byte)(1 << (target % 8));

            writer.Write(size);
            writer.Write(mask);
            writer.Write(Payload);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"Targets: {string.Join(",", Targets)} " +
                $"Payload: {BitConverter.ToString(Payload)}";
        }
    }
}
