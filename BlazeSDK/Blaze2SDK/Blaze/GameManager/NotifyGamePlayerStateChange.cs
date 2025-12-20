using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct NotifyGamePlayerStateChange
    {
        
        [TdfMember("GID")]
        public uint mGameId;
        
        [TdfMember("PID")]
        public uint mPlayerId;
        
        [TdfMember("STAT")]
        public PlayerState mPlayerState;
        
    }
}
