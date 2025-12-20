using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct PersonaInfo
    {
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("DSNM")]
        [StringLength(256)]
        public string mDisplayName;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("DTCR")]
        [StringLength(32)]
        public string mDateCreated;
        
        [TdfMember("LADT")]
        public uint mLastAuthenticated;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("NSNM")]
        [StringLength(32)]
        public string mNameSpaceName;
        
        [TdfMember("PID")]
        public long mPersonaId;
        
        [TdfMember("STAS")]
        public PersonaStatus mStatus;
        
        [TdfMember("STRC")]
        public StatusReason mStatusReasonCode;
        
    }
}
