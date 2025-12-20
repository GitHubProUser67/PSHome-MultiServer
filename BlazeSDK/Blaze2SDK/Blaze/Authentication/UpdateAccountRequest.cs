using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct UpdateAccountRequest
    {
        
        /// <summary>
        /// Max String Length: 4
        /// </summary>
        [TdfMember("CTRY")]
        [StringLength(4)]
        public string mCountry;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("DOB")]
        [StringLength(32)]
        public string mDOB;
        
        /// <summary>
        /// Max String Length: 4
        /// </summary>
        [TdfMember("LANG")]
        [StringLength(4)]
        public string mLanguage;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("MAIL")]
        [StringLength(256)]
        public string mEmail;
        
        [TdfMember("OPT1")]
        public byte mGlobalOptin;
        
        [TdfMember("OPT3")]
        public byte mThirdPartyOptin;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("PASS")]
        [StringLength(64)]
        public string mPassword;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("PRNT")]
        [StringLength(256)]
        public string mParentalEmail;
        
        [TdfMember("UID")]
        public long mUserId;
        
    }
}
