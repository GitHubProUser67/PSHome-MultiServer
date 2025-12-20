using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct GetMembersResponse
    {
        
        [TdfMember("CMLS")]
        public List<ClubMember> mClubMemberList;
        
    }
}
