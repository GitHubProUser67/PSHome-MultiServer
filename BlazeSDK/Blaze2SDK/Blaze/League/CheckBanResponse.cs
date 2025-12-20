using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct CheckBanResponse
    {
        
        [TdfMember("LGID")]
        public uint mLeagueId;
        
        [TdfMember("STAT")]
        public uint mState;
        
        [TdfMember("USER")]
        public uint mUserId;
        
    }
}
