using CustomLogger;
using EdNetService.CRC;
using EdNetService.Models;
using System.Net;

namespace EdenServer.EdNet.ProxyMessages.ORB.User
{
    public class UpdateTDUUser : AbstractProxyMessage
    {
        public override byte[]? Process(IPEndPoint endpoint, IPEndPoint target, ClientTask task, ushort PacketMagic)
        {
            var request = task.Request;

            uint user_id_in = request.ExtractUInt32();
            uint team_id_in = request.ExtractUInt32();
            string gamertag_in = request.ExtractString();
            int nat_type = request.ExtractInt32();
            ulong xuid = request.ExtractUInt64();
            short client_type = request.ExtractInt16();
            float coord_x_in = request.ExtractFloat32();
            float coord_z_in = request.ExtractFloat32();
            int level_in = request.ExtractInt32();
            int car_type_in = request.ExtractInt32();
            uint game_id_in = request.ExtractUInt32();
            ulong car_cat = request.ExtractUInt64();
#if DEBUG
            LoggerAccessor.LogInfo(
                "[UpdateUser] - ToProxy - " +
                $"User:{task.Client.Username} " +
                $"user_id:{user_id_in} " +
                $"team_id:{team_id_in} " +
                $"gamertag:\"{gamertag_in}\" " +
                $"nat_type:{nat_type} " +
                $"xuid:{xuid} " +
                $"client_type:{client_type} " +
                $"coord_x:{coord_x_in} " +
                $"coord_z:{coord_z_in} " +
                $"level:{level_in} " +
                $"car_type:{car_type_in} " +
                $"game_id:{game_id_in} " +
                $"car_cat:{car_cat}"
            );
#endif
            EdStore response = new EdStore(null, 3);

            // TODO: For now we return valid always due to an issue client side (server response wrong somewhere) which causes some important fields (such as gamertag and user_id) to be wrong.

            response.InsertStart(edStoreBank.COREREQUESTS_E_UPDATE_USER0);
            response.InsertUInt8(1);
            response.InsertEnd();

            task.Response = response;
            task.Target = endpoint;
            task.ClientMode = ClientMode.ProxyServer;

            return null;
        }
    }
}
