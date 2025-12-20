using Tdf;

namespace Blaze2SDK.Blaze.Playgroups
{
    [TdfStruct]
    public struct NotifyLeaderChange
    {
        
        [TdfMember("HSID")]
        public byte mNewHostSlotId;
        
        [TdfMember("LID")]
        public uint mNewLeaderId;
        
        [TdfMember("PGID")]
        public uint mPlaygroupId;
        
    }
}
