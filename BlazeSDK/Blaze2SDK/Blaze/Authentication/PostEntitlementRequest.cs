using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct PostEntitlementRequest
    {
        
        [TdfMember("CNID")]
        public byte[] mConsoleId;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("GNAM")]
        [StringLength(64)]
        public string mGroupName;
        
        [TdfMember("PERS")]
        public bool mWithPersona;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("PJID")]
        [StringLength(64)]
        public string mProjectId;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("PRID")]
        [StringLength(64)]
        public string mProductId;
        
        [TdfMember("STAT")]
        public EntitlementStatus mStatus;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("TAG")]
        [StringLength(64)]
        public string mEntitlementTag;
        
        [TdfMember("UID")]
        public long mUserId;
        
    }
}
