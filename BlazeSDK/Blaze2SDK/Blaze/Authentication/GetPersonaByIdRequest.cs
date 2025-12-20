using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct GetPersonaByIdRequest
    {
        
        [TdfMember("BUID")]
        public uint mBlazeUserId;
        
    }
}
