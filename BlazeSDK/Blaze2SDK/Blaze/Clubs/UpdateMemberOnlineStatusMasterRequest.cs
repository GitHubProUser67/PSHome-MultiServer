using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct UpdateMemberOnlineStatusMasterRequest
    {
        
        [TdfMember("CLID")]
        public uint mClubId;
        
        [TdfMember("CMTP")]
        public MembershipStatus mMembershipStatus;
        
        [TdfMember("CSET")]
        public ClubSettings mClubSettings;
        
        [TdfMember("REAS")]
        public ClubOnlineStatusUpdateReason mReason;
        
        [TdfMember("STAT")]
        public MemberOnlineStatus mNewStatus;
        
        [TdfMember("USID")]
        public uint mUserId;
        
    }
}
