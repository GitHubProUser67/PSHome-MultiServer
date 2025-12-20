using Tdf;

namespace Blaze2SDK.Blaze.Playgroups
{
    [TdfStruct]
    public struct LeavePlaygroupRequest
    {
        
        [TdfMember("PGID")]
        public uint mPlaygroupId;
        
    }
}
