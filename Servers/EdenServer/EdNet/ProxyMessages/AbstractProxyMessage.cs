using System.Net;
using EdNetService.Models;

namespace EdenServer.EdNet.ProxyMessages
{
    public abstract class AbstractProxyMessage
    {
        public virtual byte[]? Process(
            IPEndPoint endpoint,
            IPEndPoint target,
            ClientTask task,
            ushort PacketMagic
        )
        {
            return null;
        }
    }
}
