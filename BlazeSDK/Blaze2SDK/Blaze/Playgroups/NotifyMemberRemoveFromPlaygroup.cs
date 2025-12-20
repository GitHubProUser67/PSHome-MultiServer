using Tdf;

namespace Blaze2SDK.Blaze.Playgroups
{
    [TdfStruct]
    public struct NotifyMemberRemoveFromPlaygroup
    {
        
        [TdfMember("MLST")]
        public uint mBlazeId;
        
        [TdfMember("PGID")]
        public uint mPlaygroupId;
        
        [TdfMember("REAS")]
        public PlaygroupMemberRemoveReason mReason;
        
    }
}
