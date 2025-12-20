using Tdf;

namespace Blaze2SDK.Blaze.GpsContentController
{
    [TdfStruct]
    public struct FetchContentRequest
    {
        
        [TdfMember("COID")]
        public ulong mContentId;
        
    }
}
