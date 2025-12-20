using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct PasswordForgotRequest
    {
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("MAIL")]
        [StringLength(256)]
        public string mEmail;
        
    }
}
