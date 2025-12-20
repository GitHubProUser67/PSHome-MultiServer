using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct DecrementUseCountRequest
    {
        
        [TdfMember("DEUC")]
        public uint mDecrementCount;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("ETAG")]
        [StringLength(64)]
        public string mEntitlementTag;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("GNAM")]
        [StringLength(64)]
        public string mGroupName;
        
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
