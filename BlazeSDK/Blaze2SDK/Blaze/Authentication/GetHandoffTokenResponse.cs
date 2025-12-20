using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct GetHandoffTokenResponse
    {
        
        /// <summary>
        /// Max String Length: 1024
        /// </summary>
        [TdfMember("HOFF")]
        [StringLength(1024)]
        public string mHandoffToken;
        
    }
}
