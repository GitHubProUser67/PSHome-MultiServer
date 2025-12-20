using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct GetInvitationsRequest
    {
        
        [TdfMember("INVT")]
        public InvitationsToGetType mInvitationsToGet;
        
        [TdfMember("LGID")]
        public uint mLeagueId;
        
    }
}
