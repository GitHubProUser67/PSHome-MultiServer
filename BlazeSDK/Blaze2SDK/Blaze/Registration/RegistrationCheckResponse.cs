using Tdf;

namespace Blaze2SDK.Blaze.Registration
{
    [TdfStruct]
    public struct RegistrationCheckResponse
    {
        
        [TdfMember("BAN")]
        public byte mIsBanned;
        
    }
}
