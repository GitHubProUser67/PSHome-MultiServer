using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct GetNewsResponse
    {
        
        [TdfMember("ITMS")]
        public List<NewsItem> mNewsItems;
        
        [TdfMember("TOTL")]
        public uint mTotalItems;
        
    }
}
