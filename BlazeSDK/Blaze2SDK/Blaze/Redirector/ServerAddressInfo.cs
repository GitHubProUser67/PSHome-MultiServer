using Tdf;

namespace Blaze2SDK.Blaze.Redirector
{
    [TdfStruct]
    public struct ServerAddressInfo
    {

        [TdfMember("ADDR")]
        public ServerAddress mAddress;

        [TdfMember("TYPE")]
        public ServerAddressType mType;

    }
}
