using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct Entitlements
    {
        
        [TdfMember("NLST")]
        public List<Entitlement> mEntitlements;
        
    }
}
