using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct TeamSizeStatus
    {
        
        [TdfMember("TEAM")]
        public ushort mTeamId;
        
        [TdfMember("TMAX")]
        public ushort mMaxTeamSizeAccepted;
        
        [TdfMember("TMIN")]
        public ushort mMinTeamSizeAccepted;
        
    }
}
