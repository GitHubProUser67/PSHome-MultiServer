using EdenServer.EdNet.Messages;
using EdNetService.CRC;

namespace EdenServer.EdNet
{
    public class ProxyServer : AbstractEdenServer
    {
        public override Dictionary<ushort, Type?> CrcToClass { get; } = new Dictionary<ushort, Type?>() {
            { (ushort)ProxyCrcList.TO_PROXY, typeof(ToProxy) },
        };

        public ProxyServer(ushort Port, int MaxConcurrentListeners = 10, int awaiterTimeoutInMS = 500) : base(Port, MaxConcurrentListeners, awaiterTimeoutInMS)
        {
            
        }
    }
}
