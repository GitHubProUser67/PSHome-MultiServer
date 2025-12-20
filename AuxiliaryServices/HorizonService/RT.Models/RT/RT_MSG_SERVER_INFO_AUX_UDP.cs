using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;
using System.Net;

namespace Horizon.RT.Models
{
    [ScertMessage(RT_MSG_TYPE.RT_MSG_SERVER_INFO_AUX_UDP)]
    public class RT_MSG_SERVER_INFO_AUX_UDP : BaseScertMessage
    {
        public override RT_MSG_TYPE Id => RT_MSG_TYPE.RT_MSG_SERVER_INFO_AUX_UDP;

        public IPAddress Ip = IPAddress.Any;
        public ushort Port;

        public override void Deserialize(MessageReader reader)
        {
            Ip = IPAddress.Parse(reader.ReadString(16));
            Port = reader.ReadUInt16();
        }

        public override void Serialize(MessageWriter writer)
        {
            writer.Write(Ip?.MapToIPv4()?.ToString(), 16);
            writer.Write(Port);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"IP: {Ip} " +
                $"Port: {Port}";
        }
    }
}
