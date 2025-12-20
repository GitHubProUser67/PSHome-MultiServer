using Tdf;

namespace Blaze2SDK.Blaze.Registration
{
    [TdfStruct]
    public struct RegistrationReturnusersResponse
    {
        
        [TdfMember("NUM")]
        public int mNumUsers;
        
        [TdfMember("ROWS")]
        public List<RegistrationUser> mRegisteredUsers;
        
    }
}
