using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct GetNewsResponse
    {
        
        [TdfMember("NWLI")]
        public List<ClubLocalizedNews> mLocalizedNewsList;
        
    }
}
