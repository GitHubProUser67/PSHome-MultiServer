using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.CommerceInfo
{
    [TdfStruct]
    public struct CheckoutProducts
    {
        
        [TdfMember("PDLS")]
        public List<CheckoutProduct> mCheckoutProducts;
        
        /// <summary>
        /// Max String Length: 255
        /// </summary>
        [TdfMember("WLNM")]
        [StringLength(255)]
        public string mWalletName;
        
    }
}
