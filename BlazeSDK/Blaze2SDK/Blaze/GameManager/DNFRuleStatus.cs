using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct DNFRuleStatus
    {
        
        [TdfMember("MDNF")]
        public int mMyDNFValue;
        
        [TdfMember("XDNF")]
        public int mMaxDNFValue;
        
    }
}
