using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct SetPlayerCapacityRequest
    {
        
        [TdfMember("GID")]
        public uint mGameId;
        
        [TdfMember("PCAP")]
        public List<ushort> mSlotCapacities;
        
        [TdfMember("TCAP")]
        public List<TeamCapacity> mTeamCapacities;
        
    }
}
