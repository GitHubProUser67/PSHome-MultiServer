using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct SetDraftProfileRequest
    {
        
        [TdfMember("LGID")]
        public uint mLeagueId;
        
        [TdfMember("PROF")]
        public DraftProfile mDraftProfile;
        
    }
}
