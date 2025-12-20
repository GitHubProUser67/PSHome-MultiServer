using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct ResetLeagueRequest
    {
        
        [TdfMember("LGID")]
        public uint mLeagueId;
        
    }
}
