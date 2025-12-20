using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct SessionInfo
    {
        
        [TdfMember("BUID")]
        public uint mBlazeUserId;
        
        [TdfMember("FRST")]
        public bool mIsFirstLogin;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("KEY")]
        [StringLength(64)]
        public string mSessionKey;
        
        [TdfMember("LLOG")]
        public long mLastLoginDateTime;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("MAIL")]
        [StringLength(256)]
        public string mEmail;
        
        [TdfMember("PDTL")]
        public PersonaDetails mPersonaDetails;
        
        [TdfMember("UID")]
        public long mUserId;
        
    }
}
