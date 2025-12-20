using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct ListRivalsResponse
    {
        
        [TdfMember("RIVL")]
        public List<ClubRival> mClubRivalList;
        
    }
}
