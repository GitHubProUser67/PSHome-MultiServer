using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct GetMemberRequest
    {
        
        [TdfMember("LGID")]
        public uint mLeagueId;
        
        [TdfMember("MMBR")]
        public uint mMemberId;
        
    }
}
