using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct FullLoginResponse
    {
        
        [TdfMember("AGUP")]
        public bool mCanAgeUp;
        
        /// <summary>
        /// Max String Length: 1024
        /// </summary>
        [TdfMember("PCTK")]
        [StringLength(1024)]
        public string mPCLoginToken;
        
        /// <summary>
        /// Max String Length: 128
        /// </summary>
        [TdfMember("PRIV")]
        [StringLength(128)]
        public string mPrivacyPolicyUri;
        
        [TdfMember("SESS")]
        public SessionInfo mSessionInfo;
        
        [TdfMember("SPAM")]
        public bool mIsSpammable;
        
        /// <summary>
        /// Max String Length: 128
        /// </summary>
        [TdfMember("THST")]
        [StringLength(128)]
        public string mTosHost;
        
        /// <summary>
        /// Max String Length: 128
        /// </summary>
        [TdfMember("TURI")]
        [StringLength(128)]
        public string mTosUri;
        
    }
}
