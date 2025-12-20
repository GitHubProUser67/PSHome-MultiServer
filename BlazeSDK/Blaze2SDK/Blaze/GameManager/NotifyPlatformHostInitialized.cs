using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct NotifyPlatformHostInitialized
    {
        
        [TdfMember("GID")]
        public uint mGameId;
        
        [TdfMember("PHST")]
        public byte mPlatformHostSlotId;
        
    }
}
