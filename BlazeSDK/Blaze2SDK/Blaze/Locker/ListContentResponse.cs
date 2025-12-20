using Tdf;

namespace Blaze2SDK.Blaze.Locker
{
    [TdfStruct]
    public struct ListContentResponse
    {
        
        [TdfMember("LKRS")]
        public List<ContentInfo> mContentInfo;
        
        [TdfMember("MSIZ")]
        public int mSizeAllowed;
        
    }
}
