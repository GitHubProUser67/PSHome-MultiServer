using Tdf;

namespace Blaze2SDK.Blaze.Locker
{
    [TdfStruct]
    public struct GetContentInfoResponse
    {
        
        [TdfMember("INFO")]
        public ContentInfo mContentInfo;
        
    }
}
