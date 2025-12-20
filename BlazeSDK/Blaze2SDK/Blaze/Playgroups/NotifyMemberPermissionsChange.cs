using Tdf;

namespace Blaze2SDK.Blaze.Playgroups
{
    [TdfStruct]
    public struct NotifyMemberPermissionsChange
    {
        
        [TdfMember("LID")]
        public uint mBlazeId;
        
        [TdfMember("PERM")]
        public MemberPermissions mPermissions;
        
        [TdfMember("PGID")]
        public uint mPlaygroupId;
        
    }
}
