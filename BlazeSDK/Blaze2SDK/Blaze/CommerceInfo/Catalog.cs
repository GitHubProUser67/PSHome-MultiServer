using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.CommerceInfo
{
    [TdfStruct]
    public struct Catalog
    {
        
        /// <summary>
        /// Max String Length: 255
        /// </summary>
        [TdfMember("CCTP")]
        [StringLength(255)]
        public string mCurrencyType;
        
        /// <summary>
        /// Max String Length: 255
        /// </summary>
        [TdfMember("CCUY")]
        [StringLength(255)]
        public string mCurrency;
        
        /// <summary>
        /// Max String Length: 255
        /// </summary>
        [TdfMember("CGID")]
        [StringLength(255)]
        public string mId;
        
        /// <summary>
        /// Max Key String Length: 255
        /// Max Value String Length: 256
        /// </summary>
        [TdfMember("LAMP")]
        public SortedDictionary<string, string> mAttribs;
        
    }
}
