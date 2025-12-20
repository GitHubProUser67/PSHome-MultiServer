using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct FullGameData
    {
        
        [TdfMember("GAME")]
        public ReplicatedGameData mGame;
        
        [TdfMember("PROS")]
        public List<ReplicatedGamePlayer> mGameRoster;
        
    }
}
