using Tdf;

namespace Blaze2SDK.Blaze.Tournaments
{
    [TdfStruct]
    public struct GetTournamentsResponse
    {
        
        [TdfMember("TRNS")]
        public List<TournamentData> mTournaments;
        
    }
}
