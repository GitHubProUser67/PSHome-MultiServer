using Tdf;

namespace Blaze2SDK.Blaze
{
    [TdfStruct]
    public struct LookupUsersResponse
    {
        
        [TdfMember("ULST")]
        public List<UserIdentification> mUserIdentificationList;
        
    }
}
