using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct FindClubsResponse
    {
        
        [TdfMember("CLST")]
        public List<Club> mClubList;
        
    }
}
