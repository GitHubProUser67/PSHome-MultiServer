using Tdf;

namespace Blaze2SDK.Blaze.Playgroups
{
    [TdfStruct]
    public struct LookupPlaygroupInfoRequest
    {
        
        [TdfMember("PLST")]
        public List<uint> mPlaygroupIdList;
        
    }
}
