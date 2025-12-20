using Tdf;

namespace Blaze2SDK.Blaze.Playgroups
{
    [TdfStruct]
    public struct NotifyDestroyPlaygroup
    {
        
        [TdfMember("PGID")]
        public uint mId;
        
        [TdfMember("REAS")]
        public PlaygroupDestroyReason mReason;
        
    }
}
