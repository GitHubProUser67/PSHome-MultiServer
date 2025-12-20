using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.CommerceInfo
{
    [TdfStruct]
    public struct CheckoutProduct
    {
        
        /// <summary>
        /// Max String Length: 255
        /// </summary>
        [TdfMember("PDID")]
        [StringLength(255)]
        public string mProductId;
        
        [TdfMember("PDRN")]
        public uint mQuantity;
        
    }
}
