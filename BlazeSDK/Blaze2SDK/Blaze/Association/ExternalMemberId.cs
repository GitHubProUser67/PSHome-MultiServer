using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Association
{
    [TdfStruct]
    public struct ExternalMemberId
    {
        
        /// <summary>
        /// Max String Length: 255
        /// </summary>
        [TdfMember("EID")]
        [StringLength(255)]
        public string mExternalId;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("PNM")]
        [StringLength(256)]
        public string mPersona;
        
    }
}
