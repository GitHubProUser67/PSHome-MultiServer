using Tdf;

namespace Blaze2SDK.Blaze.Locker
{
    [TdfStruct]
    public struct GetContentTopNResponse
    {
        
        [TdfMember("LBRW")]
        public List<LeaderboardValuesRow> mTopNList;
        
        [TdfMember("SIZE")]
        public int mSize;
        
    }
}
