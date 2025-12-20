using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct NotifyJoinGame
    {
        
        [TdfMember("ERR")]
        public uint mJoinErr;
        
        [TdfMember("GAME")]
        public ReplicatedGameData mGameData;
        
        [TdfMember("MMID")]
        public uint mMatchmakingSessionId;
        
        [TdfMember("PROS")]
        public List<ReplicatedGamePlayer> mGameRoster;
        
    }
}
