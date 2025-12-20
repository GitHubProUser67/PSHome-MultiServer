using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.CommerceInfo
{
    [TdfStruct]
    public struct PricePoint
    {
        
        /// <summary>
        /// Max String Length: 255
        /// </summary>
        [TdfMember("PCTP")]
        [StringLength(255)]
        public string mCurrencyType;
        
        /// <summary>
        /// Max String Length: 255
        /// </summary>
        [TdfMember("PCUY")]
        [StringLength(255)]
        public string mCurrency;
        
        /// <summary>
        /// Max String Length: 255
        /// </summary>
        [TdfMember("PP")]
        [StringLength(255)]
        public string mPrice;
        
        /// <summary>
        /// Max String Length: 5
        /// </summary>
        [TdfMember("PPLC")]
        [StringLength(5)]
        public string mLocale;
        
        /// <summary>
        /// Max String Length: 255
        /// </summary>
        [TdfMember("PPT")]
        [StringLength(255)]
        public string mPriceType;
        
    }
}
