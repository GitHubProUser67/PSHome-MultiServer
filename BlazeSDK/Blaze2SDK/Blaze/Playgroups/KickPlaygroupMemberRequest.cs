using Tdf;

namespace Blaze2SDK.Blaze.Playgroups
{
    [TdfStruct]
    public struct KickPlaygroupMemberRequest
    {
        
        [TdfMember("EID")]
        public uint mBlazeId;
        
        [TdfMember("PGID")]
        public uint mPlaygroupId;
        
        [TdfMember("REAS")]
        public PlaygroupMemberRemoveReason mKickedReason;
        
    }
}
