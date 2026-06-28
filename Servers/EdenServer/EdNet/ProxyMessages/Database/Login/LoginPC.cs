using System.Net;
using EdenServer.Database;
using EdNetService.CRC;
using EdNetService.Models;
using MultiServerLibrary.Extension;

namespace EdenServer.EdNet.ProxyMessages.Database.Login
{
    public class LoginPC : AbstractProxyMessage
    {
        public override byte[]? Process(
            IPEndPoint endpoint,
            IPEndPoint target,
            ClientTask task,
            ushort PacketMagic
        )
        {
            var clientIp = endpoint.Address;

            var request = task.Request;

            var userName = request.ExtractString();
            var userPassword = request.ExtractString();
            var userId = request.ExtractUInt32();
            var XUID = request.ExtractUInt64();
            var unk2 = request.ExtractUInt8();
            var gameKey = request.ExtractString();
            var megapackKey = request.ExtractString();

            var response = new EdStore(null, 8);

            response.InsertStart(edStoreBank.COREREQUESTS_A_LOGIN);

            LoginDatabase.Instance.CreateUser(
                userName,
                userPassword,
                userId,
                XUID,
                unk2,
                gameKey,
                megapackKey,
                "??",
                clientIp
            );

            var userData = LoginDatabase.Instance.GetData(userName);
            if (userData == null)
            {
                response.InsertUInt8(0); // Failure
                response.InsertUInt32(0);
                response.InsertUInt32(0);
            }
            else
            {
                response.InsertUInt8(1); // Success
                response.InsertUInt32(DateTimeUtils.GetUnixTimeU32());
                response.InsertUInt8(
                    LoginDatabase.Instance.LogLogin(userName, clientIp) ? (byte)1 : (byte)0
                );
            }

            response.InsertEnd();

            task.Response = response;
            task.Target = endpoint;
            task.ClientMode = ClientMode.ProxyServer;

            return null;
        }
    }
}
