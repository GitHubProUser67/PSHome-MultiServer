using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Registration
{
    [TdfStruct]
    public struct RegistrationCheckRequest
    {
        
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
        
    }
}
