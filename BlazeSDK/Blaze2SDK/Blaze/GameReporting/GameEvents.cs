using Tdf;

namespace Blaze2SDK.Blaze.GameReporting
{
    [TdfStruct]
    public struct GameEvents
    {
        
        [TdfMember("GMES")]
        public List<GameEvent> mGameEvents;
        
    }
}
