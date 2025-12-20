using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct NotifyPlayerJoinCompleted
    {
        
        [TdfMember("GID")]
        public uint mGameId;
        
        [TdfMember("PID")]
        public uint mPlayerId;
        
    }
}
