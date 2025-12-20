using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct ListDeviceAccountsResponse
    {
        
        [TdfMember("USRL")]
        public List<UserDetails> mUserList;
        
    }
}
