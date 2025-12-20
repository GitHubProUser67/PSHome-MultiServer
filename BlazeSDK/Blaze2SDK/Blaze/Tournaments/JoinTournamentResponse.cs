using Tdf;

namespace Blaze2SDK.Blaze.Tournaments
{
    [TdfStruct]
    public struct JoinTournamentResponse
    {
        
        [TdfMember("TDAT")]
        public TournamentData mTournament;
        
    }
}
