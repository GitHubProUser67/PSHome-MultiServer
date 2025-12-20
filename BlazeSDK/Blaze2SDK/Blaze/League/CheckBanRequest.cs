using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct CheckBanRequest
    {
        
        [TdfMember("LGID")]
        public uint mLeagueId;
        
        [TdfMember("USER")]
        public uint mUserId;
        
    }
}
