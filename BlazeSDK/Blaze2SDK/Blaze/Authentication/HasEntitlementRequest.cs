using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct HasEntitlementRequest
    {
        
        [TdfMember("BUID")]
        public uint mUserId;
        
        [TdfMember("CNID")]
        public byte[] mConsoleId;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("ETAG")]
        [StringLength(64)]
        public string mEntitlementTag;
        
        [TdfMember("FLAG")]
        public EntitlementSearchFlag mEntitlementSearchFlag;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("GNLS")]
        public List<string> mGroupNameList;
        
        [TdfMember("PID")]
        public long mPersonaId;
        
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
        
    }
}
