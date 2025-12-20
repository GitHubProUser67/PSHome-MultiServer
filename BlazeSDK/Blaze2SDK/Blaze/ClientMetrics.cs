using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze
{
    [TdfStruct]
    public struct ClientMetrics
    {
        
        /// <summary>
        /// Max String Length: 127
        /// </summary>
        [TdfMember("UDEV")]
        [StringLength(127)]
        public string mDeviceInfo;
        
        [TdfMember("USTA")]
        public UpnpStatus mStatus;
        
    }
}
