using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.CommerceInfo
{
    [TdfStruct]
    public struct ProductAssociation
    {
        
        /// <summary>
        /// Max String Length: 255
        /// </summary>
        [TdfMember("CCAT")]
        [StringLength(255)]
        public string mSrcCatalog;
        
        /// <summary>
        /// Max String Length: 255
        /// </summary>
        [TdfMember("CPRD")]
        [StringLength(255)]
        public string mSrcProductId;
        
        [TdfMember("SPDF")]
        public bool mIsDefault;
        
    }
}
