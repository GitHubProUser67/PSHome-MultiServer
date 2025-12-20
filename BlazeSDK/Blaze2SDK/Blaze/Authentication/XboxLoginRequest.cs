using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct XboxLoginRequest
    {
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("GTAG")]
        [StringLength(64)]
        public string mGamerTag;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("MAIL")]
        [StringLength(256)]
        public string mEmail;
        
        [TdfMember("XUID")]
        public ulong mXuid;
        
    }
}
