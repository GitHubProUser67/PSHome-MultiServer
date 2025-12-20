using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct GetAuthTokenResponse
    {
        
        /// <summary>
        /// Max String Length: 1024
        /// </summary>
        [TdfMember("AUTH")]
        [StringLength(1024)]
        public string mAuthToken;
        
    }
}
