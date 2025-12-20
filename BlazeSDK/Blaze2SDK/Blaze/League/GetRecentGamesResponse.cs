using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct GetRecentGamesResponse
    {
        
        [TdfMember("RCGM")]
        public List<GameResult> mResults;
        
    }
}
