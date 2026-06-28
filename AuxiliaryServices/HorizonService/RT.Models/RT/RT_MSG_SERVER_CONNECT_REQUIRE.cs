using Horizon.LIBRARY.Common.Stream;
using Horizon.RT.Common;

namespace Horizon.RT.Models
{
    [ScertMessage(RT_MSG_TYPE.RT_MSG_SERVER_CONNECT_REQUIRE)]
    public class RT_MSG_SERVER_CONNECT_REQUIRE : BaseScertMessage
    {
        public override RT_MSG_TYPE Id => RT_MSG_TYPE.RT_MSG_SERVER_CONNECT_REQUIRE;

        public byte ReqServerPassword = 0x00;
        public short MaxPacketSize = 584;
        public short? MaxUdpPacketSize = 584;

        public override void Deserialize(MessageReader reader)
        {
            ReqServerPassword = reader.ReadByte();
            MaxPacketSize = reader.ReadInt16();
            MaxUdpPacketSize =
                reader.BaseStream.Length > reader.BaseStream.Position ? reader.ReadInt16() : null;
        }

        public override void Serialize(MessageWriter writer)
        {
            writer.Write(ReqServerPassword);
            writer.Write(MaxPacketSize);
            if (MaxUdpPacketSize != null)
                writer.Write(MaxUdpPacketSize.Value);
        }

        public override string ToString()
        {
            return base.ToString()
                + " "
                + $"ServerPassword: {ReqServerPassword} "
                + $"MaxPacketSize: {MaxPacketSize} "
                + $"MaxUdpPacketSize: {MaxUdpPacketSize}";
        }
    }
}
