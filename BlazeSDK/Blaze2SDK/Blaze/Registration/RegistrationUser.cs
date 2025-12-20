using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Registration
{
    [TdfStruct]
    public struct RegistrationUser
    {
        
        [TdfMember("BAN")]
        public byte mIsBanned;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("CDAT")]
        [StringLength(32)]
        public string mCreateDate;
        
        /// <summary>
        /// Max String Length: 16
        /// </summary>
        [TdfMember("CO")]
        [StringLength(16)]
        public string mCountry;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("DOB")]
        [StringLength(32)]
        public string mDOB;
        
        [TdfMember("EVID")]
        public int mEventID;
        
        [TdfMember("FLGS")]
        public uint mFlags;
        
        /// <summary>
        /// Max String Length: 21
        /// </summary>
        [TdfMember("FNAM")]
        [StringLength(21)]
        public string mFirstName;
        
        /// <summary>
        /// Max String Length: 21
        /// </summary>
        [TdfMember("LNAM")]
        [StringLength(21)]
        public string mLastName;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("MAIL")]
        [StringLength(256)]
        public string mEmail;
        
        /// <summary>
        /// Max String Length: 1024
        /// </summary>
        [TdfMember("NOTE")]
        [StringLength(1024)]
        public string mNote;
        
        /// <summary>
        /// Max String Length: 10
        /// </summary>
        [TdfMember("PFRM")]
        [StringLength(10)]
        public string mGamePlatform;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("PSNA")]
        [StringLength(256)]
        public string mPersona;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("PW")]
        [StringLength(64)]
        public string mPassword;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("SECA")]
        [StringLength(256)]
        public string mSecruityAnswer;
        
        [TdfMember("SECQ")]
        public int mSecurityQuestion;
        
        [TdfMember("THIR")]
        public byte mWithThird;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("TITL")]
        [StringLength(32)]
        public string mGameTitle;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("UID")]
        [StringLength(32)]
        public string mUserID;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("WHOB")]
        [StringLength(32)]
        public string mWhoBan;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("WHYB")]
        [StringLength(256)]
        public string mWhyBan;
        
        /// <summary>
        /// Max String Length: 16
        /// </summary>
        [TdfMember("ZIP")]
        [StringLength(16)]
        public string mZipCode;
        
    }
}
