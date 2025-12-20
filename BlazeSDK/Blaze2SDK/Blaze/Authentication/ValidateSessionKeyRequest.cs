using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct ValidateSessionKeyRequest
    {
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("SKEY")]
        [StringLength(64)]
        public string mSessionKey;
        
    }
}
