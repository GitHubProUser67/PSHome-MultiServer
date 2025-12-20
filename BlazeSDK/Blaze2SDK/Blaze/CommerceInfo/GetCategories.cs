using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.CommerceInfo
{
    [TdfStruct]
    public struct GetCategories
    {
        
        /// <summary>
        /// Max String Length: 255
        /// </summary>
        [TdfMember("CLNM")]
        [StringLength(255)]
        public string mCatalogName;
        
    }
}
