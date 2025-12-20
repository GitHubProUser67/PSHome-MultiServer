using Tdf;

namespace Blaze2SDK.Blaze.Playgroups
{
    [TdfStruct]
    public struct NotifyMemberJoinedPlaygroup
    {
        
        [TdfMember("MEMB")]
        public PlaygroupMemberInfo mPlaygroupMemberInfo;
        
        [TdfMember("PGID")]
        public uint mPlaygroupId;
        
    }
}
