using Tdf;

namespace Blaze2SDK.Blaze
{
    [TdfStruct]
    public struct UserDataResponse
    {
        
        [TdfMember("ULST")]
        public List<UserData> mUserDataList;
        
    }
}
