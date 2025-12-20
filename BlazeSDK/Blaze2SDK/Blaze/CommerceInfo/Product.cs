using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.CommerceInfo
{
    [TdfStruct]
    public struct Product
    {
        
        [TdfMember("DLOC")]
        public uint mDefaultLocale;
        
        /// <summary>
        /// Max String Length: 255
        /// </summary>
        [TdfMember("FID")]
        [StringLength(255)]
        public string mFinanceId;
        
        /// <summary>
        /// Max Key String Length: 255
        /// Max Value String Length: 256
        /// </summary>
        [TdfMember("PATT")]
        public SortedDictionary<string, string> mAttribs;
        
        /// <summary>
        /// Max String Length: 255
        /// </summary>
        [TdfMember("PID")]
        [StringLength(255)]
        public string mId;
        
        /// <summary>
        /// Max String Length: 255
        /// </summary>
        [TdfMember("PLFM")]
        [StringLength(255)]
        public string mPlatForm;
        
        /// <summary>
        /// Max String Length: 255
        /// </summary>
        [TdfMember("PNAM")]
        [StringLength(255)]
        public string mName;
        
        [TdfMember("PPP")]
        public PricePoints mPricePoints;
        
        /// <summary>
        /// Max String Length: 255
        /// </summary>
        [TdfMember("PTPE")]
        [StringLength(255)]
        public string mType;
        
    }
}
