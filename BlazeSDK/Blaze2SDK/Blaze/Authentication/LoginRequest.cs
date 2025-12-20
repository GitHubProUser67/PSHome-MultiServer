using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct LoginRequest
    {
        
        [TdfMember("DVID")]
        public ulong mDeviceId;
        
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
        /// Max String Length: 1024
        /// </summary>
        [TdfMember("TOKN")]
        [StringLength(1024)]
        public string mToken;
        
        [TdfMember("TYPE")]
        public TOKENTYPE mTokenType;
        
    }
}
