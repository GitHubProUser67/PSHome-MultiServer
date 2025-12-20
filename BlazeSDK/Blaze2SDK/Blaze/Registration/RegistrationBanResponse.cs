using Tdf;

namespace Blaze2SDK.Blaze.Registration
{
    [TdfStruct]
    public struct RegistrationBanResponse
    {
        
        [TdfMember("BAN")]
        public byte mIsBanned;
        
    }
}
