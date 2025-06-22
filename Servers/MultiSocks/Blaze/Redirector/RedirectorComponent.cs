using Blaze3SDK;
using Blaze3SDK.Blaze.Redirector;
using Blaze3SDK.Components;
using BlazeCommon;
using CustomLogger;
using NetworkLibrary.Extension;

namespace MultiSocks.Blaze.Redirector
{
    internal class RedirectorComponent : RedirectorComponentBase.Server
    {
        public override Task<ServerInstanceInfo> GetServerInstanceAsync(ServerInstanceRequest request, BlazeRpcContext context)
        {
#if DEBUG
            LoggerAccessor.LogInfo($"[Blaze] - Redirector: Connection Id    : {context.Connection.ID}");
            LoggerAccessor.LogInfo($"[Blaze] - Redirector: Client Name  : {request.mClientName}");
            LoggerAccessor.LogInfo($"[Blaze] - Redirector: Client Type      : {request.mClientType}");
            LoggerAccessor.LogInfo($"[Blaze] - Redirector: Client SkuId  : {request.mClientSkuId}");
            LoggerAccessor.LogInfo($"[Blaze] - Redirector: Client Environment  : {request.mEnvironment}");
#endif
            bool secure = MultiSocksServerConfiguration.EnableBlazeEncryption;
            ushort port;

            switch (request.mClientName)
            {
                case "pvz client":
                    secure = false;
                    port = 33302;
                    break;
                case "/PS3/DEADSPACE-3":
                    secure = false;
                    port = 33202;
                    break;
                case "Crysis 3":
                    secure = false;
                    port = 33192;
                    break;
                case "NFS Hot Pursuit":
                    secure = false;
                    port = 33182;
                    break;
                case "FIFA12":
                    secure = false;
                    port = 33172;
                    break;
                case "SSX":
                    secure = false;
                    port = 33162;
                    break;
                case "MassEffect2-ps3":
                    secure = false;
                    port = 33153;
                    break;
                case "MassEffect3-ps3":
                    secure = false;
                    port = 33152;
                    break;
                default:
                    throw new BlazeRpcException(Blaze3RpcError.REDIRECTOR_UNKNOWN_SERVICE_NAME, new ServerInstanceError()
                    {
                        mMessages = new List<string>() {
                        "Unknown game requested! Please report to GITHUB."
                    }
                    });
            }

            string ip = MultiSocksServerConfiguration.ServerBindAddress;

            return Task.FromResult(new ServerInstanceInfo()
            {
                //this is an union type, so we specify only one of the values
                mAddress = new ServerAddress()
                {
                    IpAddress = new IpAddress()
                    {
                        mHostname = ip,
                        mIp = InternetProtocolUtils.GetIPAddressAsUInt(ip),
                        mPort = port
                    },
                },

                mSecure = secure,
                mDefaultDnsAddress = 0
            });
        }
    }
}
