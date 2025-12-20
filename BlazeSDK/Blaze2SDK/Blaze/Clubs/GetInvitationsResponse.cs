using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct GetInvitationsResponse
    {
        
        [TdfMember("CIST")]
        public List<ClubMessage> mClubInvList;
        
    }
}
