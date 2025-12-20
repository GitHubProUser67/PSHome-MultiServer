using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Locker
{
    [TdfStruct]
    public struct UpdateContentInfoRequest
    {
        
        [TdfMember("ATTR")]
        public List<Attribute> mAttributes;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("CCAT")]
        [StringLength(32)]
        public string mContentCategory;
        
        [TdfMember("CID")]
        public int mContentId;
        
        [TdfMember("COID")]
        public uint mContextId;
        
        /// <summary>
        /// Max String Length: 100
        /// </summary>
        [TdfMember("DESC")]
        [StringLength(100)]
        public string mDescription;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("NAME")]
        [StringLength(32)]
        public string mName;
        
        [TdfMember("PERM")]
        public Permission mPermission;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("TAGS")]
        public List<string> mTags;
        
    }
}
