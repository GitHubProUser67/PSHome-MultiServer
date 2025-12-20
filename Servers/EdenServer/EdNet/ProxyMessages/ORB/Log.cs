using CustomLogger;
using EdNetService.Models;
using System.Net;

namespace EdenServer.EdNet.ProxyMessages.ORB
{
    public class Log : AbstractProxyMessage
    {
        public override byte[]? Process(IPEndPoint endpoint, IPEndPoint target, ClientTask task, ushort PacketMagic)
        {
            string clientName = task.Request.ExtractString();
            string userName = task.Request.ExtractString();
            string text = task.Request.ExtractString();

#if DEBUG
            LoggerAccessor.LogInfo($"[Log] - User:{userName}|{clientName} sent a message:{text}");
#endif
            task.Target = endpoint;
            task.ClientMode = ClientMode.None;

            return null;
        }
    }
}
