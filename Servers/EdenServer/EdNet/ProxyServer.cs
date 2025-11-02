using EdenServer.EdNet.Messages;
using EdNetService.CRC;

namespace EdenServer.EdNet
{
    public class ProxyServer : AbstractEdenServer
    {
        public override Dictionary<ushort, Type?> CrcToClass { get; } = new Dictionary<ushort, Type?>() {
            { (ushort)ProxyCrcList.TO_PROXY_HEADER, typeof(ToProxy) },
        };

        public ProxyServer() : base()
        {
            
        }
    }
}
