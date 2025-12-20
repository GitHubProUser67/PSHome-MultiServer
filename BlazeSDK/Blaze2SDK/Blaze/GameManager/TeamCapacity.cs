using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct TeamCapacity
    {
        
        [TdfMember("TCAP")]
        public ushort mTeamCapacity;
        
        [TdfMember("TID")]
        public ushort mTeamId;
        
        [TdfMember("TIDX")]
        public ushort mTeamIndex;
        
    }
}
