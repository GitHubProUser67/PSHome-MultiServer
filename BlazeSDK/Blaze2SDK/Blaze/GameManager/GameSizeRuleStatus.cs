using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct GameSizeRuleStatus
    {
        
        [TdfMember("PMAX")]
        public uint mMaxPlayerCountAccepted;
        
        [TdfMember("PMIN")]
        public uint mMinPlayerCountAccepted;
        
    }
}
