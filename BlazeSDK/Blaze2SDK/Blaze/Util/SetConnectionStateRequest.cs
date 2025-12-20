using Tdf;

namespace Blaze2SDK.Blaze.Util
{
    [TdfStruct]
    public struct SetConnectionStateRequest
    {
        
        [TdfMember("ACTV")]
        public bool mIsActive;
        
    }
}
