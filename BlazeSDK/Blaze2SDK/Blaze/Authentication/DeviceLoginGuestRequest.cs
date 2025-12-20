using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct DeviceLoginGuestRequest
    {
        
        [TdfMember("DVID")]
        public ulong mDeviceId;
        
    }
}
