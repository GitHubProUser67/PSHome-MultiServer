using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.CommerceInfo
{
    [TdfStruct]
    public struct GetProductAssociation
    {
        
        /// <summary>
        /// Max String Length: 255
        /// </summary>
        [TdfMember("CSTR")]
        [StringLength(255)]
        public string mCode;
        
    }
}
