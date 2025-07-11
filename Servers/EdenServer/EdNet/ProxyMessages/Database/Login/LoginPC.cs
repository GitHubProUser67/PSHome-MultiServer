using EdenServer.Database;
using EdNetService.CRC;
using EdNetService.Models;
using System.Net;

namespace EdenServer.EdNet.ProxyMessages.Database.Login
{
    public class LoginPC : AbstractProxyMessage
    {
        public override byte[]? Process(IPEndPoint endpoint, IPEndPoint target, ClientTask task, ushort PacketMagic)
        {
            IPAddress clientIp = endpoint.Address;

            EdStore request = task.Request;

            string userName = request.ExtractString();
            string userPassword = request.ExtractString();
            uint userId = request.ExtractUInt32();
            ulong XUID = request.ExtractUInt64();
            byte unk2 = request.ExtractUInt8();
            string gameKey = request.ExtractString();
            string megapackKey = request.ExtractString();

            EdStore response = new EdStore(null, 11);

            response.InsertStart(edStoreBank.COREREQUESTS_A_LOGIN);

            LoginDatabase.Instance.CreateUser(userName, userPassword, userId, XUID, unk2, gameKey, megapackKey, "??", clientIp);

            Dictionary<string, object>? userData = LoginDatabase.Instance.GetData(userName);
            if (userData == null)
            {
                response.InsertUInt8(0); // Failure
                response.InsertUInt32(0);
                response.InsertUInt32(0);
            }
            else
            {
                response.InsertUInt8(1); // Success
                response.InsertUInt32(Convert.ToUInt32(userData["id"]));
                response.InsertUInt8(LoginDatabase.Instance.LogLogin(userName, clientIp) ? (byte)1 : (byte)0);
            }

            response.InsertEnd();

            task.Response = response;
            task.Target = endpoint;
            task.ClientMode = ClientMode.ProxyServer;

            return null;
        }
    }
}
