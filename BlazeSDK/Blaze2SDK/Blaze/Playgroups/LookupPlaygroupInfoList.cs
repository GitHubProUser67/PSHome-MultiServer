using Tdf;

namespace Blaze2SDK.Blaze.Playgroups
{
    [TdfStruct]
    public struct LookupPlaygroupInfoList
    {
        
        [TdfMember("PGPS")]
        public List<PlaygroupInfo> mPlaygroupInfoList;
        
    }
}
