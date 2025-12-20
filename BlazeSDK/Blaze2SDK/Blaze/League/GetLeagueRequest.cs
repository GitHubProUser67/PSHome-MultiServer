using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct GetLeagueRequest
    {
        
        [TdfMember("LGID")]
        public uint mLeagueId;
        
    }
}
