using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct SendInvitationRequest
    {
        
        [TdfMember("INVT")]
        public uint mInviteeId;
        
        [TdfMember("LGID")]
        public uint mLeagueId;
        
        [TdfMember("META")]
        public byte[] mMetadata;
        
    }
}
