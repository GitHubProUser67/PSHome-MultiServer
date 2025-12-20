using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Locker
{
    [TdfStruct]
    public struct CacheRowUpdate
    {
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("CAT")]
        [StringLength(32)]
        public string mCategory;
        
        [TdfMember("CID")]
        public int mContentId;
        
        [TdfMember("CTYP")]
        public uint mContextType;
        
        [TdfMember("ETYP")]
        public uint mEntityType;
        
        [TdfMember("HIDE")]
        public bool mHide;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("TAGS")]
        public List<string> mTags;
        
        [TdfMember("UPDT")]
        public List<Attribute> mAttributes;
        
    }
}
