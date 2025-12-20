using Tdf;

namespace Blaze2SDK.Blaze
{
    [TdfStruct]
    public struct UpdateHardwareFlagsRequest
    {
        
        [TdfMember("HWFG")]
        public HardwareFlags mHardwareFlags;
        
    }
}
