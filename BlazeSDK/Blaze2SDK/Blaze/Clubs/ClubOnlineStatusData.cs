using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct ClubOnlineStatusData
    {
        
        [TdfMember("CLML")]
        public SortedDictionary<uint, ClubMember> mMemberMap;
        
        [TdfMember("CSET")]
        public ClubSettings mClubSettings;
        
        [TdfMember("MSCO")]
        public SortedDictionary<MemberOnlineStatus, ushort> mMemberOnlineStatusCounts;
        
    }
}
