using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct MemberOnlineStatusClass
    {
        
        [TdfMember("MOST")]
        public MemberOnlineStatus mMemberOnlineStatus;
        
        [TdfMember("MSST")]
        public MembershipStatus mMembershipStatus;
        
    }
}
