using Tdf;

namespace Blaze2SDK.Blaze.Tournaments
{
    [TdfStruct]
    public struct ResetTournamentRequest
    {
        
        [TdfMember("BZID")]
        public uint mBlazeId;
        
        [TdfMember("TNID")]
        public uint mTournamentId;
        
    }
}
