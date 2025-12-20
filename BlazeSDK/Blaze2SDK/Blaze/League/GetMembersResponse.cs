using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct GetMembersResponse
    {
        
        [TdfMember("MBIF")]
        public List<MemberInfo> mMemberInfo;
        
    }
}
