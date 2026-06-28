using System.Net;
using EdNetService.CRC;
using EdNetService.Models;

namespace EdenServer.EdNet.ProxyMessages.ORB.User
{
    public class UpdateTDUUserStats : AbstractProxyMessage
    {
        public override byte[]? Process(
            IPEndPoint endpoint,
            IPEndPoint target,
            ClientTask task,
            ushort PacketMagic
        )
        {
            task.Client.UserRights = task.Request.ExtractDataStore();

            var response = new EdStore(null, 3);

            response.InsertStart(edStoreBank.CRC_E_STATS_USER_STATISTICS_UPDATE_V2);
            response.InsertUInt8(1);
            response.InsertEnd();

            task.Response = response;
            task.Target = endpoint;
            task.ClientMode = ClientMode.ProxyServer;

            return null;
        }
    }
}
