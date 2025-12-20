using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Locker
{
    [TdfStruct]
    public struct SubContentInfo
    {
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("GURL")]
        [StringLength(256)]
        public string mGetURL;
        
        [TdfMember("STTS")]
        public Status mStatus;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("UURL")]
        [StringLength(256)]
        public string mUploadURL;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("XID")]
        [StringLength(64)]
        public string mXrefId;
        
    }
}
