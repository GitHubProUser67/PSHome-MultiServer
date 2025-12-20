using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct CreateAccountResponse
    {
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("PNAM")]
        [StringLength(256)]
        public string mPersonaName;
        
        [TdfMember("UID")]
        public long mUserId;
        
    }
}
