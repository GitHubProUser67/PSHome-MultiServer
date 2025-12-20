using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct HostInfo
    {
        
        [TdfMember("HPID")]
        public uint mPlayerId;
        
        [TdfMember("HSLT")]
        public byte mSlotId;
        
    }
}
