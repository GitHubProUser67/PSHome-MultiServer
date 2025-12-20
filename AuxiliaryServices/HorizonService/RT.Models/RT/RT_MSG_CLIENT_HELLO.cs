using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [ScertMessage(RT_MSG_TYPE.RT_MSG_CLIENT_HELLO)]
    public class RT_MSG_CLIENT_HELLO : BaseScertMessage
    {
        public override RT_MSG_TYPE Id => RT_MSG_TYPE.RT_MSG_CLIENT_HELLO;

        public ushort[] Parameters = null;

        public override void Deserialize(MessageReader reader)
        {
            long len = reader.BaseStream.Length - reader.BaseStream.Position;
            Parameters = new ushort[len / 2];
            for (int i = 0; i < Parameters.Length; ++i)
                Parameters[i] = reader.ReadUInt16();
        }

        public override void Serialize(MessageWriter writer)
        {
            for (int i = 0; i < 5; ++i)
                writer.Write((Parameters == null || i >= Parameters.Length) ? ushort.MinValue : Parameters[i]);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"Version: {Parameters[1]}";
        }
    }
}
