using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct ListDeviceAccountsRequest
    {
        
        [TdfMember("DVID")]
        public ulong mDeviceId;
        
    }
}
