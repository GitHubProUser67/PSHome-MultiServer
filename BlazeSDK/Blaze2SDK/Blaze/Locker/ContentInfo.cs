using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Locker
{
    [TdfStruct]
    public struct ContentInfo
    {
        
        [TdfMember("ATTR")]
        public List<Attribute> mAttributes;
        
        [TdfMember("BOID")]
        public ulong mBlazeObjId;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("CCAT")]
        [StringLength(32)]
        public string mContentCategory;
        
        [TdfMember("CDAT")]
        public int mCreateDate;
        
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
        /// Max String Length: 45
        /// </summary>
        [TdfMember("DFMT")]
        [StringLength(45)]
        public string mDataFormat;
        
        [TdfMember("EID")]
        public uint mEntityId;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("ENAM")]
        [StringLength(64)]
        public string mEntityName;
        
        [TdfMember("GPID")]
        public ulong mGroupId;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("GURL")]
        [StringLength(256)]
        public string mGetURL;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("NAME")]
        [StringLength(32)]
        public string mName;
        
        [TdfMember("PERM")]
        public Permission mPermission;
        
        [TdfMember("RATE")]
        public int mRate;
        
        [TdfMember("RCNT")]
        public uint mTotalRatingCount;
        
        [TdfMember("SIZE")]
        public int mSize;
        
        [TdfMember("STTS")]
        public Status mStatus;
        
        /// <summary>
        /// Max Key String Length: 32
        /// </summary>
        [TdfMember("SUBL")]
        public SortedDictionary<string, SubContentInfo> mSubContentInfos;
        
        [TdfMember("TAGS")]
        public List<Tag> mTags;
        
        [TdfMember("UCNT")]
        public int mUseCount;
        
        [TdfMember("UDAT")]
        public int mUpdateDate;
        
        [TdfMember("URAT")]
        public uint mMyRating;
        
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
