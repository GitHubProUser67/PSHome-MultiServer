using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Locker
{
    [TdfStruct]
    public struct ListContentRequest
    {
        
        [TdfMember("ATTR")]
        public List<Attribute> mAttributes;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("CCAT")]
        [StringLength(32)]
        public string mContentCategory;
        
        [TdfMember("EID")]
        public uint mEntityId;
        
        [TdfMember("GPID")]
        public ulong mGroupId;
        
        [TdfMember("PERM")]
        public PermissionFlag mPermissionFlag;
        
        [TdfMember("REFF")]
        public RefSearchFlag mReferenceFlag;
        
        [TdfMember("TAGS")]
        public List<Tag> mTags;
        
    }
}
