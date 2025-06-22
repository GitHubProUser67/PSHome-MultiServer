using EdNetService.Models;
using System.Net;

namespace EdenServer.EdNet.Messages
{
    public abstract class AbstractMessage
    {
        public virtual bool Process(AbstractEdenServer server, IPEndPoint endpoint, EdStore store)
        {
            return false;
        }
    }
}
