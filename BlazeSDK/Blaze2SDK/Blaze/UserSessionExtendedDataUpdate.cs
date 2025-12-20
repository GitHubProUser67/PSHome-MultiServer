using Tdf;

namespace Blaze2SDK.Blaze
{
    [TdfStruct]
    public struct UserSessionExtendedDataUpdate
    {
        
        [TdfMember("DATA")]
        public UserSessionExtendedData mExtendedData;
        
        [TdfMember("USID")]
        public uint mUserId;
        
    }
}
