using Tdf;

namespace Blaze2SDK.Blaze.Locker
{
    [TdfStruct]
    public struct TopNContentValuesRow
    {
        
        [TdfMember("INFO")]
        public ContentInfo mContentInfo;
        
        [TdfMember("RANK")]
        public int mRank;
        
    }
}
