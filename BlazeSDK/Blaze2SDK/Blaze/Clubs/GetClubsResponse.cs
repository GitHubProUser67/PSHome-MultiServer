using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct GetClubsResponse
    {
        
        [TdfMember("CLST")]
        public List<Club> mClubList;
        
    }
}
