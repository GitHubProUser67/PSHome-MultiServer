using System.Net;
using CustomLogger;
using EdNetService.Models;

namespace EdenServer.EdNet.ProxyMessages.ORB
{
    public class Log : AbstractProxyMessage
    {
        public override byte[]? Process(
            IPEndPoint endpoint,
            IPEndPoint target,
            ClientTask task,
            ushort PacketMagic
        )
        {
            var clientName = task.Request.ExtractString();
            var userName = task.Request.ExtractString();
            var text = task.Request.ExtractString();

#if DEBUG
            LoggerAccessor.LogInfo($"[Log] - User:{userName}|{clientName} sent a message:{text}");
#endif
            task.Target = endpoint;
            task.ClientMode = ClientMode.None;

            return null;
        }
    }
}
