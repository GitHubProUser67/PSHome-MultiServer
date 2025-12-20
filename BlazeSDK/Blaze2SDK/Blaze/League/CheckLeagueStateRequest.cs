using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct CheckLeagueStateRequest
    {
        
        [TdfMember("LGID")]
        public uint mLeagueId;
        
    }
}
