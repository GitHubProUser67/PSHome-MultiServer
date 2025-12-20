using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.CommerceInfo
{
    [TdfStruct]
    public struct Category
    {
        
        /// <summary>
        /// Max String Length: 255
        /// </summary>
        [TdfMember("CGID")]
        [StringLength(255)]
        public string mId;
        
        /// <summary>
        /// Max String Length: 255
        /// </summary>
        [TdfMember("CTYP")]
        [StringLength(255)]
        public string mType;
        
        [TdfMember("DLOC")]
        public uint mDefaultLocale;
        
        /// <summary>
        /// Max Key String Length: 255
        /// Max Value String Length: 256
        /// </summary>
        [TdfMember("LAMP")]
        public SortedDictionary<string, string> mAttribs;
        
        [TdfMember("PCNT")]
        public uint mProductCount;
        
        /// <summary>
        /// Max String Length: 255
        /// </summary>
        [TdfMember("SLST")]
        public List<string> mSubCategoryList;
        
        [TdfMember("TCTE")]
        public bool mIsTopCategory;
        
    }
}
