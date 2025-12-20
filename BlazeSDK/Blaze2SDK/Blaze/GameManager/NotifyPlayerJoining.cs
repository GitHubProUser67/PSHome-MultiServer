using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct NotifyPlayerJoining
    {
        
        [TdfMember("GID")]
        public uint mGameId;
        
        [TdfMember("PDAT")]
        public ReplicatedGamePlayer mJoiningPlayer;
        
    }
}
