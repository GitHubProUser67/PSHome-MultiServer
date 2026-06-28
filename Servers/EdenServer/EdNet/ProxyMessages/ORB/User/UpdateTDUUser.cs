using System.Net;
using CustomLogger;
using EdNetService.CRC;
using EdNetService.Models;

namespace EdenServer.EdNet.ProxyMessages.ORB.User
{
    public class UpdateTDUUser : AbstractProxyMessage
    {
        public override byte[]? Process(
            IPEndPoint endpoint,
            IPEndPoint target,
            ClientTask task,
            ushort PacketMagic
        )
        {
            var request = task.Request;

            var user_id_in = request.ExtractUInt32();
            var team_id_in = request.ExtractUInt32();
            var gamertag_in = request.ExtractString();
            var nat_type = request.ExtractInt32();
            var xuid = request.ExtractUInt64();
            var client_type = request.ExtractInt16();
            var coord_x_in = request.ExtractFloat32();
            var coord_z_in = request.ExtractFloat32();
            var level_in = request.ExtractInt32();
            var car_type_in = request.ExtractInt32();
            var game_id_in = request.ExtractUInt32();
            var car_cat = request.ExtractUInt64();
#if DEBUG
            LoggerAccessor.LogInfo(
                "[UpdateUser] - ToProxy - "
                    + $"User:{task.Client.Username} "
                    + $"user_id:{user_id_in} "
                    + $"team_id:{team_id_in} "
                    + $"gamertag:\"{gamertag_in}\" "
                    + $"nat_type:{nat_type} "
                    + $"xuid:{xuid} "
                    + $"client_type:{client_type} "
                    + $"coord_x:{coord_x_in} "
                    + $"coord_z:{coord_z_in} "
                    + $"level:{level_in} "
                    + $"car_type:{car_type_in} "
                    + $"game_id:{game_id_in} "
                    + $"car_cat:{car_cat}"
            );
#endif
            var response = new EdStore(null, 3);

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
