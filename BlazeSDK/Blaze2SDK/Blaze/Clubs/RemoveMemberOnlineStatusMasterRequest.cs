using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct RemoveMemberOnlineStatusMasterRequest
    {
        
        [TdfMember("CLID")]
        public uint mClubId;
        
        [TdfMember("REAS")]
        public ClubOnlineStatusUpdateReason mReason;
        
    }
}
