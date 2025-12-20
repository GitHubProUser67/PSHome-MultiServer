using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct UpdateMemberOnlineStatusRequest
    {
        
        [TdfMember("STAT")]
        public MemberOnlineStatus mNewStatus;
        
    }
}
