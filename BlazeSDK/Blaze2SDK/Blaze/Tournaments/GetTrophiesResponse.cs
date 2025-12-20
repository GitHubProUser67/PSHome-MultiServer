using Tdf;

namespace Blaze2SDK.Blaze.Tournaments
{
    [TdfStruct]
    public struct GetTrophiesResponse
    {
        
        [TdfMember("TROP")]
        public List<TournamentTrophyData> mTrophies;
        
    }
}
