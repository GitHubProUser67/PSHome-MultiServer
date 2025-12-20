using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct Invitations
    {
        
        [TdfMember("INVS")]
        public List<Invitation> mInvitations;
        
        public enum InvitationsToGetType : int
        {
            LEAGUE_INVITATIONS_SENT_TO_ME = 0x0,
            LEAGUE_INVITATIONS_SENT_BY_ME = 0x1,
            LEAGUE_INVITATIONS_ALL = 0x2,
        }
        
    }
}
