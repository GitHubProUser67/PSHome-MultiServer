using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct AcceptTosRequest
    {
        
        /// <summary>
        /// Max String Length: 128
        /// </summary>
        [TdfMember("TURI")]
        [StringLength(128)]
        public string mTosUri;
        
    }
}
