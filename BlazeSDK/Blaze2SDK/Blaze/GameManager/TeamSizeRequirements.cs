using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct TeamSizeRequirements
    {
        
        [TdfMember("PCAP")]
        public ushort mMaxPlayerCapacity;
        
        [TdfMember("PCNT")]
        public ushort mDesiredPlayerCount;
        
        [TdfMember("PMIN")]
        public ushort mMinPlayerCount;
        
        [TdfMember("TID")]
        public ushort mTeamId;
        
    }
}
