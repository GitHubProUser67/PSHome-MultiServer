using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct GetClubAwardsResponse
    {
        
        [TdfMember("AWRL")]
        public List<ClubAward> mClubAwardList;
        
    }
}
