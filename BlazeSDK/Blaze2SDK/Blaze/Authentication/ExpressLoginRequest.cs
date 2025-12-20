using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct ExpressLoginRequest
    {
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("MAIL")]
        [StringLength(256)]
        public string mEmail;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("PASS")]
        [StringLength(64)]
        public string mPassword;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("PNAM")]
        [StringLength(256)]
        public string mPersonaName;
        
    }
}
