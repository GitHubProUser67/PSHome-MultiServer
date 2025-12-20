using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct DestroyGameRequest
    {
        
        [TdfMember("GID")]
        public uint mGameId;
        
        [TdfMember("REAS")]
        public GameDestructionReason mDestructionReason;
        
    }
}
