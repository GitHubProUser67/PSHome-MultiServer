using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze
{
    [TdfStruct]
    public struct QosPingSiteInfo
    {
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("PSA")]
        [StringLength(64)]
        public string mAddress;
        
        [TdfMember("PSP")]
        public ushort mPort;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("SNA")]
        [StringLength(64)]
        public string mSiteName;
        
    }
}
