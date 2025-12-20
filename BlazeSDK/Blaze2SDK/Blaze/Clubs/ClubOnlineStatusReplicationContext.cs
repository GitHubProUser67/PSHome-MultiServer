using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct ClubOnlineStatusReplicationContext
    {
        
        [TdfMember("CLID")]
        public uint mClubId;
        
        [TdfMember("CURE")]
        public ClubOnlineStatusUpdateReason mUpdateReason;
        
    }
}
