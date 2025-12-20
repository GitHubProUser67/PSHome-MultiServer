using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.CommerceInfo
{
    [TdfStruct]
    public struct GetWalletBalance
    {
        
        /// <summary>
        /// Max String Length: 255
        /// </summary>
        [TdfMember("WLNM")]
        [StringLength(255)]
        public string mWalletName;
        
    }
}
