using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct NotifyGameCapacityChange
    {
        
        [TdfMember("CAP")]
        public List<ushort> mSlotCapacities;
        
        [TdfMember("GID")]
        public uint mGameId;
        
        [TdfMember("TEAM")]
        public List<TeamCapacity> mTeamCapacities;
        
    }
}
