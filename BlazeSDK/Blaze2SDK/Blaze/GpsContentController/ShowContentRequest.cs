using Tdf;

namespace Blaze2SDK.Blaze.GpsContentController
{
    [TdfStruct]
    public struct ShowContentRequest
    {
        
        [TdfMember("COID")]
        public ulong mContentId;
        
        [TdfMember("SHOW")]
        public bool mShow;
        
    }
}
