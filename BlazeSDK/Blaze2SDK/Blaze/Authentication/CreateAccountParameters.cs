using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct CreateAccountParameters
    {
        
        [TdfMember("BDAY")]
        public int mBirthDay;
        
        [TdfMember("BMON")]
        public int mBirthMonth;
        
        [TdfMember("BYR")]
        public int mBirthYear;
        
        /// <summary>
        /// Max String Length: 4
        /// </summary>
        [TdfMember("CTRY")]
        [StringLength(4)]
        public string mIsoCountryCode;
        
        [TdfMember("DVID")]
        public ulong mDeviceId;
        
        [TdfMember("GEST")]
        public bool mIsGuest;
        
        /// <summary>
        /// Max String Length: 4
        /// </summary>
        [TdfMember("LANG")]
        [StringLength(4)]
        public string mIsoLanguageCode;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("MAIL")]
        [StringLength(256)]
        public string mEmail;
        
        [TdfMember("OPT1")]
        public byte mEaEmailAllowed;
        
        [TdfMember("OPT3")]
        public byte mThirdPartyEmailAllowed;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("PASS")]
        [StringLength(64)]
        public string mPassword;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("PNAM")]
        [StringLength(256)]
        public string mPersonaName;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("PRNT")]
        [StringLength(256)]
        public string mParentalEmail;
        
        [TdfMember("PROF")]
        public UserProfileInfo mUserProfileInfo;
        
        /// <summary>
        /// Max String Length: 128
        /// </summary>
        [TdfMember("TOSV")]
        [StringLength(128)]
        public string mTosVersion;
        
    }
}
