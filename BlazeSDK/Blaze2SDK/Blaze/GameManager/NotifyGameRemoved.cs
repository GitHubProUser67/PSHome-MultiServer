using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct NotifyGameRemoved
    {
        
        [TdfMember("GID")]
        public uint mGameId;
        
        [TdfMember("REAS")]
        public GameDestructionReason mDestructionReason;
        
    }
}
