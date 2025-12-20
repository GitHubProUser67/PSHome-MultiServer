using Tdf;

namespace Blaze2SDK.Blaze.Locker
{
    [TdfStruct]
    public struct ContentUpdateBroadcast
    {
        
        [TdfMember("UPDT")]
        public List<CacheRowUpdate> mCacheUpdates;
        
    }
}
