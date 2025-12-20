using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct GetHandoffTokenRequest
    {
        
        /// <summary>
        /// Max String Length: 128
        /// </summary>
        [TdfMember("CSTR")]
        [StringLength(128)]
        public string mClientString;
        
    }
}
