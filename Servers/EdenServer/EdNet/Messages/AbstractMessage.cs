using System.Net;
using System.Net.Sockets;
using EdNetService.Models;

namespace EdenServer.EdNet.Messages
{
    public abstract class AbstractMessage
    {
        public virtual bool Process(UdpClient listener, AbstractEdenServer server, IPEndPoint endpoint, EdStore store)
        {
            return false;
        }
    }
}
