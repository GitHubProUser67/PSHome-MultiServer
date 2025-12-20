using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct GetPlayoffSeriesRequest
    {
        
        [TdfMember("LGID")]
        public uint mLeagueId;
        
    }
}
