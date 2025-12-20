using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct Invitation
    {
        
        [TdfMember("CRTM")]
        public uint mCreationTime;
        
        [TdfMember("INVT")]
        public LeagueUser mInvitee;
        
        [TdfMember("LGID")]
        public uint mLeagueId;
        
        [TdfMember("MEMB")]
        public LeagueUser mInviter;
        
        [TdfMember("META")]
        public byte[] mMetadata;
        
        public enum InvitationOp : int
        {
            INVITATION_REJECT = 0x1,
            INVITATION_REVOKE = 0x2,
        }
        
        public enum InvitationsToGetType : int
        {
            LEAGUE_INVITATIONS_SENT_TO_ME = 0x0,
            LEAGUE_INVITATIONS_SENT_BY_ME = 0x1,
            LEAGUE_INVITATIONS_ALL = 0x2,
        }
        
    }
}
