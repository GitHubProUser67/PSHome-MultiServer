using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct HostBalanceRuleStatus
    {
        
        [TdfMember("BVAL")]
        public HostBalanceValues mMatchedHostBalanceValue;
        
        public enum HostBalanceValues : int
        {
            HOSTS_STRICTLY_BALANCED = 0x0,
            HOSTS_BALANCED = 0x1,
            HOSTS_UNBALANCED = 0x2,
        }
        
    }
}
