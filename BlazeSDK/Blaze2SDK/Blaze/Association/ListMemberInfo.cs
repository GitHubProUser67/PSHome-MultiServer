using Tdf;

namespace Blaze2SDK.Blaze.Association
{
    [TdfStruct]
    public struct ListMemberInfo
    {
        
        [TdfMember("MATS")]
        public long mTimeAdded;
        
        [TdfMember("MUIF")]
        public UserData mUserData;
        
    }
}
