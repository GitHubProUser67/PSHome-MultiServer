using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct RosterSizeRulePrefs
    {
        
        [TdfMember("PCAP")]
        public ushort mMaxPlayerCount;
        
        [TdfMember("PMIN")]
        public ushort mMinPlayerCount;
        
    }
}
