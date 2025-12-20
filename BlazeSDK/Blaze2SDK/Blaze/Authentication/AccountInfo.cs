using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct AccountInfo
    {
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("ASRC")]
        [StringLength(64)]
        public string mAuthenticationSource;
        
        /// <summary>
        /// Max String Length: 3
        /// </summary>
        [TdfMember("CO")]
        [StringLength(3)]
        public string mCountry;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("DOB")]
        [StringLength(32)]
        public string mDOB;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("DTCR")]
        [StringLength(32)]
        public string mDateCreated;
        
        [TdfMember("GOPT")]
        public byte mGlobalOptin;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("LATH")]
        [StringLength(32)]
        public string mLastAuth;
        
        /// <summary>
        /// Max String Length: 3
        /// </summary>
        [TdfMember("LN")]
        [StringLength(3)]
        public string mLanguage;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("MAIL")]
        [StringLength(256)]
        public string mEmail;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("PML")]
        [StringLength(256)]
        public string mParentalEmail;
        
        [TdfMember("RC")]
        public StatusReason mReasonCode;
        
        [TdfMember("STAS")]
        public AccountStatus mStatus;
        
        /// <summary>
        /// Max String Length: 8
        /// </summary>
        [TdfMember("TOSV")]
        [StringLength(8)]
        public string mTosVersion;
        
        [TdfMember("TPOT")]
        public byte mThirdPartyOptin;
        
        [TdfMember("UID")]
        public long mUserId;
        
    }
}
