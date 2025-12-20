using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct ListGameData
    {
        
        [TdfMember("GAME")]
        public ReplicatedGameData mGame;
        
        [TdfMember("PROS")]
        public List<ReplicatedGamePlayer> mGameRoster;
        
    }
}
