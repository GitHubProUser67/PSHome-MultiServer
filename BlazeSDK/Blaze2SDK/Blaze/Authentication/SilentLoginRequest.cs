using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct SilentLoginRequest
    {
        
        /// <summary>
        /// Max String Length: 1024
        /// </summary>
        [TdfMember("AUTH")]
        [StringLength(1024)]
        public string mAuthToken;
        
        [TdfMember("PID")]
        public long mPersonaId;
        
        [TdfMember("TYPE")]
        public TOKENTYPE mTokenType;
        
    }
}
