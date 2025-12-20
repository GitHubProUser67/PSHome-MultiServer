using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct HostViabilityRuleStatus
    {
        
        [TdfMember("VVAL")]
        public HostViabilityValues mMatchedHostViabilityValue;
        
        public enum HostViabilityValues : int
        {
            CONNECTION_ASSURED = 0x0,
            CONNECTION_LIKELY = 0x1,
            CONNECTION_UNLIKELY = 0x2,
        }
        
    }
}
