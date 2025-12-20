using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.CommerceInfo
{
    [TdfStruct]
    public struct WalletBalance
    {
        
        /// <summary>
        /// Max String Length: 255
        /// </summary>
        [TdfMember("WBAL")]
        [StringLength(255)]
        public string mBalance;
        
        /// <summary>
        /// Max String Length: 255
        /// </summary>
        [TdfMember("WLCR")]
        [StringLength(255)]
        public string mCurrency;
        
    }
}
