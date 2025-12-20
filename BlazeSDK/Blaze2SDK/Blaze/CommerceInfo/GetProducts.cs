using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.CommerceInfo
{
    [TdfStruct]
    public struct GetProducts
    {
        
        /// <summary>
        /// Max String Length: 255
        /// </summary>
        [TdfMember("CGNM")]
        [StringLength(255)]
        public string mCategoryName;
        
        /// <summary>
        /// Max String Length: 255
        /// </summary>
        [TdfMember("CLNM")]
        [StringLength(255)]
        public string mCatalogName;
        
        [TdfMember("PPSN")]
        public ushort mPageNo;
        
        [TdfMember("PPSZ")]
        public ushort mPageSize;
        
    }
}
