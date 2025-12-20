using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct SetOptionRequest
    {
        
        [TdfMember("GMID")]
        public uint mMemberId;
        
        [TdfMember("LGID")]
        public uint mLeagueId;
        
        [TdfMember("OPID")]
        public uint mOptionId;
        
        [TdfMember("VALU")]
        public uint mValue;
        
    }
}
