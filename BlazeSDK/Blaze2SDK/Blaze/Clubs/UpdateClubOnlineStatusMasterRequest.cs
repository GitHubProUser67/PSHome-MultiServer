using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct UpdateClubOnlineStatusMasterRequest
    {
        
        [TdfMember("CLID")]
        public uint mClubId;
        
        [TdfMember("CSET")]
        public ClubSettings mClubSettings;
        
        [TdfMember("REAS")]
        public ClubOnlineStatusUpdateReason mReason;
        
    }
}
