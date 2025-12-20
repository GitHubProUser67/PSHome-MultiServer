using BlazeCommon;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Tdf;

namespace Blaze2SDK
{
    public static class Blaze2
    {
        static TdfFactory factory;
        static TdfLegacyEncoder encoder;
        static TdfLegacyDecoder decoder;

        static Blaze2()
        {
            factory = new TdfFactory();
            encoder = factory.CreateLegacyEncoder();
            decoder = factory.CreateLegacyDecoder();
        }

        public static BlazeServer CreateBlazeServer(string name, IPEndPoint endPoint, X509Certificate2? certificate = null, bool forceSsl = true, ConnectionDelegate? onNewConnection = null, ConnectionDelegate? onDisconnected = null, ConnectionOnRequestDelegate? onRequest = null, ConnectionOnErrorDelegate? onError = null)
        {
            BlazeServerConfiguration blaze2Settings = new BlazeServerConfiguration(name, endPoint, encoder, decoder)
            {
                Certificate = certificate,
                ForceSsl = forceSsl,
                ComponentNotFoundErrorCode = (int)Blaze2RpcError.ERR_COMPONENT_NOT_FOUND,
                CommandNotFoundErrorCode = (int)Blaze2RpcError.ERR_COMMAND_NOT_FOUND,
                ErrSystemErrorCode = (int)Blaze2RpcError.ERR_SYSTEM,
                OnNewConnection = onNewConnection,
                OnDisconnected = onDisconnected,
                OnRequest = onRequest,
                OnError = onError
            };

            return new BlazeServer(blaze2Settings);
        }

        public static BlazeClientConnection CreateBlazeClientConnection(ProtoFireConnection connection)
        {
            BlazeClientConfiguration config = new BlazeClientConfiguration(encoder, decoder);
            return new BlazeClientConnection(connection, config);
        }
    }
}
