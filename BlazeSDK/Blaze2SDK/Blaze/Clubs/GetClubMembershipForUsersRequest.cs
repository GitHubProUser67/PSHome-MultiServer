using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct GetClubMembershipForUsersRequest
    {
        
        [TdfMember("IDLT")]
        public List<uint> mBlazeIdList;
        
    }
}
