using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Registration
{
    [TdfStruct]
    public struct RegistrationBanRequest
    {
        
        [TdfMember("BAN")]
        public byte mIsBanned;
        
        [TdfMember("EVID")]
        public int mEventID;
        
        /// <summary>
        /// Max String Length: 10
        /// </summary>
        [TdfMember("PFRM")]
        [StringLength(10)]
        public string mGamePlatform;
        
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
        
    }
}
