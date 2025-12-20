using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct GetAvailablePlayersRequest
    {
        
        [TdfMember("LGID")]
        public uint mLeagueId;
        
    }
}
