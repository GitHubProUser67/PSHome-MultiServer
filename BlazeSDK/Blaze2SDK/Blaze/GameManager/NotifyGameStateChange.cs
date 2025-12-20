using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct NotifyGameStateChange
    {
        
        [TdfMember("GID")]
        public uint mGameId;
        
        [TdfMember("GSTA")]
        public GameState mNewGameState;
        
    }
}
