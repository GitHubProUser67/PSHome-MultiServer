using System.Net;
using CustomLogger;
using EdNetService.CRC;
using EdNetService.Models;

namespace EdenServer.EdNet.ProxyMessages.ORB
{
    public class OrbKeepAlive : AbstractProxyMessage
    {
        public override byte[]? Process(
            IPEndPoint endpoint,
            IPEndPoint target,
            ClientTask task,
            ushort PacketMagic
        )
        {
            var sequenceId = task.Request.ExtractUInt32();
            uint kaUnk = task.Request.ExtractUInt16();
#if DEBUG
            LoggerAccessor.LogInfo(
                $"[OrbKeepAlive] - ToProxy - User:{task.Client.Username} Kept-Alive : sequenceId:{sequenceId} unk:{kaUnk}"
            );
#endif
            var response = new EdStore(null, 6);

            response.InsertStart((ushort)ProxyCrcList.CLIENT_ORB_KEEP_ALIVE);
            response.InsertUInt32(0);
            response.InsertEnd();

            task.Response = response;
            task.Target = endpoint;
            task.ClientMode = ClientMode.ProxyServerRaw;

            return null;
        }
    }
}
