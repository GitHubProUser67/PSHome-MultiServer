using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct AdvanceGameStateRequest
    {
        
        [TdfMember("GID")]
        public uint mGameId;
        
        [TdfMember("GSTA")]
        public GameState mNewGameState;
        
    }
}
