using Tdf;

namespace Blaze2SDK.Blaze.Tournaments
{
    [TdfStruct]
    public struct GetMemberCountsRequest
    {
        
        [TdfMember("TNID")]
        public uint mTournamentId;
        
    }
}
