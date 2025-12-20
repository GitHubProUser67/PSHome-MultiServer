using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct GetTradesRequest
    {
        
        [TdfMember("FORM")]
        public uint mMemberId;
        
        [TdfMember("LGID")]
        public uint mLeagueId;
        
    }
}
