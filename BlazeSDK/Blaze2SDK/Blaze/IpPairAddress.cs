using Tdf;

namespace Blaze2SDK.Blaze
{
    [TdfStruct]
    public struct IpPairAddress
    {
        
        [TdfMember("EXIP")]
        public IpAddress ExternalAddress;
        
        [TdfMember("INIP")]
        public IpAddress InternalAddress;
        
    }
}
