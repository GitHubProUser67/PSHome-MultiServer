using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct UpdateHostConnectionRequest
    {
        
        [TdfMember("GID")]
        public uint mGameId;
        
        [TdfMember("STAT")]
        public PlayerNetConnectionStatus mPlayerNetConnectionStatus;
        
    }
}
