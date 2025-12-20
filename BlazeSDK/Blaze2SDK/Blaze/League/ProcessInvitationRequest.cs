using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct ProcessInvitationRequest
    {
        
        [TdfMember("INVR")]
        public uint mInviterId;
        
        [TdfMember("INVT")]
        public uint mInviteeId;
        
        [TdfMember("LGID")]
        public uint mLeagueId;
        
        [TdfMember("OPER")]
        public InvitationOp mOperation;
        
    }
}
