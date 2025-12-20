using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct LoginResponse
    {
        
        /// <summary>
        /// Max String Length: 1024
        /// </summary>
        [TdfMember("PCTK")]
        [StringLength(1024)]
        public string mPCLoginToken;
        
        [TdfMember("PLST")]
        public List<PersonaDetails> mPersonaDetailsList;
        
        /// <summary>
        /// Max String Length: 128
        /// </summary>
        [TdfMember("PRIV")]
        [StringLength(128)]
        public string mPrivacyPolicyUri;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("SKEY")]
        [StringLength(64)]
        public string mSessionKey;
        
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
        
        [TdfMember("UID")]
        public long mUserId;
        
    }
}
