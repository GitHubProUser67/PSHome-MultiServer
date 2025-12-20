using Tdf;

namespace Blaze2SDK.Blaze.Locker
{
    [TdfStruct]
    public struct ContentDeleteBroadcast
    {
        
        [TdfMember("DELE")]
        public List<CacheDelete> mCacheDeletes;
        
    }
}
