using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct GetClubMembershipForUsersResponse
    {
        
        [TdfMember("MMAP")]
        public SortedDictionary<uint, ClubMembership> mMembershipMap;
        
    }
}
